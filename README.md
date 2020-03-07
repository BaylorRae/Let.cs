## Let.cs

[![build](https://github.com/BaylorRae/Let.cs/workflows/build/badge.svg)](https://github.com/BaylorRae/Let.cs/actions?query=workflow%3Abuild) [![nuget](https://img.shields.io/nuget/v/RSpec-Let)](https://www.nuget.org/packages/RSpec-Let/)

When testing in C# it's not unusual to test instances of a class. The problem we
all face is having a place to initialize our objects.

1. Create a field that is initialized and reset on `[TearDown]`?
2. Create a null field that is initialized during `[SetUp]`?
3. Duplicate the initialization in every `[Test]`?

All three of these are valid, and unfortunately, all three can be found in the
same project and sometimes the same test!

```csharp
namespace DoYourTestsLookLikeThis
{
    [TestFixture]
    public class InsertTearsEmoji
    {
        private UnitOfWork _work = new UnitOfWork();
        private Mock<IArticlesService> MockArticlesService { get; set; }

        [SetUp]
        public void BeforeEach()
        {
            MockArticlesService = new Mock<IArticlesService>(_work);
        }

        [TearDown]
        public void AfterEach()
        {
            // 😭😭😭
            _work = new UnitOfWork();
        }

        [Test]
        public void IndexShouldLoadPublishedArticles()
        {
            var controller = new ArticlesController(MockArticlesService.Object);

            controller.Index();
            MockArticlesService.Verify(service => service.Published(), Times.Once);
        }
    }
}
```

## It's dangerous to go alone, take Let.cs with you 🤺


```csharp
namespace HappyDance
{
    // This allows us to call +Let+ directly
    using static LetTestHelper.LetHelper;

    [TestFixture]
    public class ArticlesControllerTest
    {
        private UnitOfWork Work => Let(() => new UnitOfWork());

        // We can depend on other Lets
        private Mock<IArticlesService> MockArticlesService => Let(() => new Mock<IArticlesService>(Work));

        // It's so easy, why not move it here too
        private ArticlesController Controller => Let(() => new ArticlesController(MockArticlesService.Object);

        [TearDown]
        public void AfterEach()
        {
            // You may cry, but only once (see below 👇)
            LetTestHelper.LetHelper.Flush();
        }

        [Test]
        public void IndexShouldLoadPublishedArticles()
        {
            Controller.Index();
            MockArticlesService.Verify(service => service.Published(), Times.Once);
        }
    }
}
```

## Flush the Cache 🚽

- After each test you will need to manually flush the results. My recommendation
  is creating a test helper that will flush the cache on tear down.

```csharp
namespace MyProject.TestHelper
{
    [TestFixture]
    public class TestBase
    {
        [TearDown]
        public void Clean_LetHelper()
        {
            LetTestHelper.LetHelper.Flush();
        }
    }
}
```

## Inspiration

Coming from Ruby I found instance management in C# to be, well, not fun. This is
is influenced by [RSpec `let`][rspec_let].

[rspec_let]: https://relishapp.com/rspec/rspec-core/v/3-9/docs/helper-methods/let-and-let
[memoization]: https://en.wikipedia.org/wiki/Memoization
