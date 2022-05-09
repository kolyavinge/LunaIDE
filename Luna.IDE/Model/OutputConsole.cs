namespace Luna.IDE.Model
{
    public interface IOutputConsole
    {
        IOutputArea OutputArea { get; }
    }

    public class OutputConsole : IOutputConsole
    {
        public IOutputArea OutputArea { get; }

        public OutputConsole(IOutputArea outputArea)
        {
            OutputArea = outputArea;
        }
    }
}
