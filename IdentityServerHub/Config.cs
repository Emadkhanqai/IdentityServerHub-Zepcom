﻿using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServerHub
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api1", "My API"),
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // MVC client
                new Client
                {
                    ClientId = "mvc",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:7087/signin-oidc" },
                    AllowedScopes = { "openid", "profile", "api1" }
                },

                // Desktop client
                new Client
                {
                    ClientId = "desktop",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "openid", "profile", "api1" }
                },

                // API client
                new Client
                {
                    ClientId = "api",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("apisecret".Sha256()) },
                    AllowedScopes = { "api1" }
                }
            };

        public static List<TestUser> TestUsers => new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "admin",
                Password = "abc123456",
                Claims = new List<Claim>
                {
                    new Claim("name", "Zepcom")
                }
            }
        };
    }

}
