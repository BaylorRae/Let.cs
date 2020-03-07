using Moq;
using NUnit.Framework;
using System;
using LetTestHelper;

namespace Let.cs.Tests
{
    [TestFixture]
    public class IntegrationTest
    {
        private Mock<ISpy> MockSpy => LetHelper.Let("MockSpy", () => {
            var mockSpy = new Mock<ISpy>();
            mockSpy.Setup(spy => spy.DoSomething()).Returns(true);
            return mockSpy;
        });

        private bool DoSomethingWithSpy => LetHelper.Let(() => MockSpy.Object.DoSomething());

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
