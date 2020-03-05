## Let.cs

Let.cs is a clone of [RSpec `let`][rspec_let]. It allows code to be defined in a
lambda expression and [resolved only once][memoization]. The primary goal is to
move values out of the test initializer and into the class definition itself.

The other benefit the values are "lazy loaded". This allows multiple test pieces
that rely on each other to be defined without actually being invoked until
called.

## Example

Here's an example of 

```csharp
namespace Example
{
  using static LetTestHelper.LetHelper;

  [TestFixture]
  public class ApiExample
  {
      private ArticleDto _articleDto => Let(() => Factory<ArticleDto>.Build());
      private ArticleService _service => Let(() => new ArticleService());
      private Article article => Let(() => _service.BuildArticleFromDto(_articleDto));
      
      [TearDown]
      public void FlushLetHelper()
      {
          LetHelper.Flush();
      }

      [Test]
      public void ItAssignsTheTitle()
      {
          Assert.That(article.Title, Is.EqualTo("article from factory"));
      }

      [Test]
      public void ItAssignsTheBody()
      {
          Assert.That(article.Body, Is.EqualTo("article-body"));
      }
  }
}
```

## Caveat

- After each test you will need to manually flush the results. My recommendation
  is creating a test helper that all test classes inherit from that flushes the
  cache.

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

[rspec_let]: https://www.relishapp.com/rspec/rspec-core/v/2-5/docs/helper-methods/let-and-let
[memoization]: https://en.wikipedia.org/wiki/Memoization
