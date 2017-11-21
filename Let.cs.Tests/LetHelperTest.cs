using Moq;
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

        [Test]
        public void ItAllowsGenericsToBeAssigned()
        {
            Mock<I1> WrappedI1() => LetHelper.Let(() => new Mock<I1>());
            Mock<I2> WrappedI2() => LetHelper.Let(() => new Mock<I2>());

            Assert.That(WrappedI1().Object, Is.Not.EqualTo(WrappedI2().Object));
        }
            
        [Test]
        public void NamedInstancesWithFuncs()
        {
            Spy Spy1() => LetHelper.Let("spy1", () => new Spy());
            Spy Spy2() => LetHelper.Let("spy2", () => new Spy());

            Assert.That(Spy1(), Is.Not.EqualTo(Spy2()));
        }

        [Test, Ignore("should this throw an exception or overwrite the previous definition?")]
        public void ItDoesntAllowNamingCollisions()
        {
            Spy Spy1() => LetHelper.Let("spy1", () => new Spy());
            Spy Spy2() => LetHelper.Let("spy1", () => new Spy());

            // option 1
            Assert.That(Spy2, Throws.InvalidOperationException);

            // option 2
            // this feels wrong but it's also a
            // very simple example
            Assert.That(Spy1(), Is.EqualTo(Spy2()));
        }

        public class Spy
        {
            public virtual bool Called() { return true; }
        }

        public interface I1 { }
        public interface I2 { }
    }
}
