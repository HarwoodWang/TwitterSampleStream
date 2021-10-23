using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TwitterStreamProduce.Main.Controllers;
using TwitterStreamProduce.SharedLibrary;

namespace TwitterStreamProduce.Test
{
    public class TestController
    {
        private TwitterStreamProduceController controller;

        [SetUp]
        public void Setup()
        {
            ILoggerFactory logger = LoggerConfig.GetLoggerConfig<TwitterStreamProduceController>();

            controller = new TwitterStreamProduceController(logger);
        }

        [Test]
        public void TestGetHistory()
        {
            var result = controller.Get();

            Assert.IsNotNull(result);
            Assert.AreNotEqual(1000, result.Value);
        }
    }
}