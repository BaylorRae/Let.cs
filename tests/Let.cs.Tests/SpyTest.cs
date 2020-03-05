using NUnit.Framework;

namespace Let.cs.Tests
{
    [TestFixture]
    public class SpyTest
    {
        [Test]
        public void ItHasToBeExplicityCalled()
        {
            var spy = new Spy();
            Assert.That(spy.Called, Is.False);

            spy.DoSomething();
            Assert.That(spy.Called, Is.True);
        }

        [Test]
        public void ItCanBeInstantiatedCalled()
        {
            var spy = Spy.AlreadyCalled();
            Assert.That(spy.Called, Is.True);
        }
    }
}
