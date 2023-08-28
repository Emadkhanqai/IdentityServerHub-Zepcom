using System.Linq;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Services;
using System.Threading.Tasks;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServerHub.Controllers
{
    public class AccountController : Controller
    {
        private readonly TestUserStore _users;
        private readonly IIdentityServerInteractionService _interaction;

        public AccountController(TestUserStore users, IIdentityServerInteractionService interaction)
        {
            _users = users;
            _interaction = interaction;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var vm = new LoginViewModel
            {
                ReturnUrl = returnUrl ?? Url.Content("~/") // Default to home if returnUrl is null
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _users.FindByUsername(model.Username);
                if (user != null && _users.ValidateCredentials(model.Username, model.Password))
                {
                    var isUser = new IdentityServer4.IdentityServerUser(user.SubjectId)
                    {
                        DisplayName = user.Username,
                        IdentityProvider = IdentityServer4.IdentityServerConstants.LocalIdentityProvider,
                        AdditionalClaims = user.Claims.ToList()
                    };

                    await HttpContext.SignInAsync(isUser);

                    if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password");
            }

            // Model state isn't valid or login failed, return the model to repopulate the fields
            return View(model);
        }
    }

    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
