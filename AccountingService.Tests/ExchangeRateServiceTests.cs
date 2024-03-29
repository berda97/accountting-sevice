﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AccountingService.Services;
using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace AccountingService.Tests
{
    [TestFixture]
    internal class ExchangeRateServiceTests
    {
        private IConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            var configurationMock = new Mock<IConfiguration>();
            _configuration = configurationMock.Object;
            configurationMock.SetupGet(x => x["AppSettings:ApiKey"]).Returns("your_api_key_here");
        }
        [Test]
        public void GetCurrencyExchangeRate_ReturnsDefaultRateForNullCurrency()
        {
            var exchangeRateService = new ExchangeRateService(_configuration);
            var result = exchangeRateService.GetCurrencyExchangeRate(null);
            Assert.AreEqual(1, result);
        }
        
        [Test]
        public void GetCurrencyExchangeRate_ReturnsDefaultRateForNonexistentCurrency()
        {
            var exchangeRateService = new ExchangeRateService(_configuration);
            var nonexistentCurrency = "XYZ";  
            var result = exchangeRateService.GetCurrencyExchangeRate(nonexistentCurrency);

            Assert.AreEqual(1, result);
        }

    }
}




