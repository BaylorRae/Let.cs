using Moq;
using NUnit.Framework;
using System;
using LetTestHelper;

namespace Let.cs.Tests
{
    [TestFixture]
    public class IntegrationTest
    {
        private Mock<ISpy> MockSpy;
        private bool DoSomethingWithSpy => LetHelper.Let(() => MockSpy.Object.DoSomething());

        [SetUp]
        public void BeforeEach()
        {
            MockSpy = new Mock<ISpy>();
            MockSpy.Setup(spy => spy.DoSomething()).Returns(true);
        }

        [TearDown]
        public void AfterEach()
        {
            LetHelper.Flush();
        }

        [Test]
        public void ItLazilyInvokesTheCallback()
        {
          MockSpy.Verify(spy => spy.DoSomething(), Times.Exactly(0));
          Assert.That(DoSomethingWithSpy, Is.True);
          MockSpy.Verify(spy => spy.DoSomething(), Times.Exactly(1));
        }
    }
}
