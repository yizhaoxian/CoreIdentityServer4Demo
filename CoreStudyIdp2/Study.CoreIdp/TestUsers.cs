using IdentityModel;
using IdentityServer4;
using IdentityServer4.Test;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;

namespace Study.CoreIdp
{
    public class TestUsers
    {
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    country = "China",
                    locality = "BeiJing",
                    street_address = "ChaoYang",
                    postal_code = 10010
                };

                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "818727",
                        Username = "alice",
                        Password = "alice",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "TestUser.Alice Smith"),
                            new Claim(JwtClaimTypes.GivenName, "TestUser.Alice"),
                            new Claim(JwtClaimTypes.FamilyName, "TestUser.Smith"),
                            new Claim(JwtClaimTypes.Email, "TestUser.AliceSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "TestUser.http://alice.com"),
                            new Claim(JwtClaimTypes.Address, JsonConvert.SerializeObject(address), IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim(JwtClaimTypes.Profile, "TestUser.Profile"),
                            new Claim(JwtClaimTypes.PhoneNumber, "TestUser.13812345678"),
                            new Claim(JwtClaimTypes.Role,"System")
                        }
                    } ,
                    new TestUser
                    {
                        SubjectId = "88421113",
                        Username = "bob",
                        Password = "bob",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Bob Smith"),
                            new Claim(JwtClaimTypes.GivenName, "Bob"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim(JwtClaimTypes.Address,    JsonConvert.SerializeObject(address), IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim(JwtClaimTypes.Role,"Admin")
                        }
                    }
                };
            }
        }
    }
}
