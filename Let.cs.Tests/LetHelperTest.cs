﻿using Moq;
using NUnit.Framework;
using System;
using LetTestHelper;

namespace Let.cs.Tests
{
    [TestFixture]
    public class LetHelperTest
    {
        private Mock<Spy> _spy;
        private Func<bool> _callback;

        [SetUp]
        public void SetUp()
        {
            _spy = new Mock<Spy>();
            _callback = () => LetHelper.Let(() => _spy.Object.Called());
        }

        [TearDown]
        public void TearDown()
        {
            LetHelper.Flush();
        }

        [Test]
        public void ItCachesTheResult()
        {
            Func<int> notSoRandom = () => LetHelper.Let(() => new Random().Next(10000));
            var firstValue = notSoRandom();

            for (var i = 0; i < 10; i++)
                Assert.That(notSoRandom(), Is.EqualTo(firstValue));
        }

        [Test]
        public void ItInvokesTheExpressionOnce()
        {
            for (var i = 0; i < 10; i++)
                _callback();

            _spy.Verify(s => s.Called(), Times.Once);
        }

        [Test]
        public void ItClearsTheCachedResults()
        {
            _callback();
            _callback();

            LetHelper.Flush();

            _callback();
            _callback();

            _spy.Verify(s => s.Called(), Times.Exactly(2));
        }

        [Test]
        public void ItAllowsMultipleTypesToBeStored()
        {
            Func<string> callbackString = () => LetHelper.Let(() => "string");
            Func<int> callbackInteger = () => LetHelper.Let(() => 1337);

            Assert.That(callbackString(), Is.EqualTo("string"));
            Assert.That(callbackInteger(), Is.EqualTo(1337));
        }

        public class Spy
        {
            public virtual bool Called() { return true; }
        }
    }
}