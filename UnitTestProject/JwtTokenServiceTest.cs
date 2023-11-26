﻿using AccountingService.Services;
using IdentityModel;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SystemWrapper;

using NUnit.Framework;
using Microsoft.IdentityModel.Tokens;
using NodaTime.Testing;
using NodaTime;
using AccountingService.Interface;

namespace UnitTestProject
{
    [TestFixture]
    public class JwtTokenServiceTest
    {
        private IConfiguration _configuration;
        private ITimeService _timeService;
        private List<Claim> _claims;

        [SetUp]
        public void Setup()
        {
            var configurationMock = new Mock<IConfiguration>();
            var timeServiceMock = new Mock<ITimeService>();

            configurationMock.SetupGet(x => x["JwtConfig:Key"]).Returns("your_newly_generated_128_bit_key_here");
            configurationMock.SetupGet(x => x["JwtConfig:Issuer"]).Returns("your_issuer");
            configurationMock.SetupGet(x => x["JwtConfig:Audience"]).Returns("your_audience");
            configurationMock.Setup(x => x["JwtConfig:ExpiryInMinutes"]).Returns("15");

            timeServiceMock.Setup(ts => ts.Now).Returns(() => DateTime.UtcNow.AddMinutes(20));


            _configuration = configurationMock.Object;
            _timeService = timeServiceMock.Object;

            _claims = new List<Claim>
            {
            new Claim(ClaimTypes.Email, "Miki123"),
            };
        }

        [TestCase]
        public void GetToken_ReturnsValidJwtToken()
        {
            var jwtTokenService = new JwtTokenService(_configuration, _timeService);
            var token = jwtTokenService.GetToken(_claims);

            Assert.NotNull(token);

            Assert.NotNull(token.Value);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token.Value);
            Assert.That(securityToken.Issuer, Is.EqualTo("your_issuer"));
            Assert.That(securityToken.Audiences.FirstOrDefault(), Is.EqualTo("your_audience"));
            Assert.IsTrue(securityToken.Claims.Any(c => c.Type == ClaimTypes.Email && c.Value == "Miki123"));
            Assert.True(securityToken.ValidTo > DateTime.UtcNow);
        }
        [TestCase]
        public void GetToken_ReturnsInvalidJwtTokenForInvalidEmail()
        {
            var jwtTokenService = new JwtTokenService(_configuration, _timeService);
            var token = jwtTokenService.GetToken(_claims);

            Assert.NotNull(token);

            Assert.NotNull(token.Value);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token.Value);

            Assert.IsFalse(securityToken.Claims.Any(c => c.Type == ClaimTypes.Email && c.Value == "neispravan_email"));
        }
        [TestCase]
        public void GetToken_ReturnsInvalidJwtTokenForInvalidIssuer()
        {
            var jwtTokenService = new JwtTokenService(_configuration, _timeService);
            var token = jwtTokenService.GetToken(_claims);

            Assert.NotNull(token);
            Assert.NotNull(token.Value);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token.Value);

            // Let's expect the Issuer to be incorrect
            Assert.That(securityToken.Issuer, Is.Not.EqualTo("neispravan_issuer"));
        }

        [TestCase]
        public void GetToken_ReturnsInvalidJwtTokenForInvalidAudience()
        {
            var jwtTokenService = new JwtTokenService(_configuration, _timeService);
            var token = jwtTokenService.GetToken(_claims);

            Assert.NotNull(token);
            Assert.NotNull(token.Value);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token.Value);

            // Let's expect that the Audience is not valid
            Assert.That(securityToken.Audiences.FirstOrDefault(), Is.Not.EqualTo("neispravan_audience"));
        }
        [TestCase]
        public void GetToken_ReturnsInvalidJwtTokenForExpiredToken()
        {
            var jwtTokenService = new JwtTokenService(_configuration, _timeService);
            // Act & Assert
            Assert.Throws<SecurityTokenExpiredException>(() => jwtTokenService.GetToken(_claims));
        }
    }
}

