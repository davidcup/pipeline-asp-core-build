using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using CompanyApi.Core.Auth;
using CompanyApi.Tests.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace CompanyApi.Tests.UnitTests
{
    public class JwtFactoryUnitTests : IClassFixture<TestWebApplicationFactory>
    {
        public JwtFactoryUnitTests(TestWebApplicationFactory factory)
        {
            factory.CreateClient();
        }

        [Fact]
        public void CanEncodeToken()
        {
            // arrange
            var token = Guid.NewGuid().ToString();
            var jwtIssuerOptions = new JwtIssuerOptions
            {
                Issuer = "",
                Audience = "",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes("secretkey")), SecurityAlgorithms.HmacSha256)
            };

            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            mockJwtTokenHandler.Setup(handler => handler.WriteToken(It.IsAny<JwtSecurityToken>())).Returns(token);
            var jwtFactory = new JwtFactory(mockJwtTokenHandler.Object, Options.Create(jwtIssuerOptions));

            // act
            var result = jwtFactory.EncodeToken("userName");

            // assert
            Assert.Equal(token, result);
        }
    }
}
