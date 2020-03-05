namespace Let.cs.Tests
{
    public interface ISpy
    {
        bool Called { get; set; }
        bool DoSomething();
    }

    public class Spy : ISpy
    {
        public bool Called { get; set; }

        public bool DoSomething() {
          return Called = true;
        }

        public static Spy AlreadyCalled()
        {
            return new Spy { Called = true };
        }
    }
}
