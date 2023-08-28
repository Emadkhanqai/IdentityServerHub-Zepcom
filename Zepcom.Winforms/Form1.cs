using IdentityModel.Client;

namespace Zepcom.Winforms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async Task AuthenticateAsync()
        {
            try
            {
                var username = "admin";
                var password = "abc123456";

                var client = new HttpClient();
                var disco = await client.GetDiscoveryDocumentAsync("https://localhost:7114");
                if (disco.IsError)
                {
                    MessageBox.Show($"Discovery error: {disco.Error}");
                    return;
                }

                var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "desktop",
                    ClientSecret = "secret",
                    UserName = username,
                    Password = password,
                    Scope = "api1 openid profile"
                });

                if (tokenResponse.IsError)
                {
                    MessageBox.Show($"Token error: {tokenResponse.Error}");
                    return;
                }

                // Token is valid. Store token and/or use it to make authorized requests
                var token = tokenResponse.AccessToken;
                MessageBox.Show(token is not null ? "Token Validated" : "Error occurred");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                throw;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AuthenticateAsync().GetAwaiter().GetResult();
        }
    }
}