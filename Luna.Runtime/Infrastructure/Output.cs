namespace Luna.Infrastructure
{
    public interface IOutput
    {
        void AddMessage(OutputMessage message);
        void AddNewLine();
    }

    public class OutputMessage
    {
        public string Text { get; }
        public OutputMessageKind Kind { get; }

        public OutputMessage(string text, OutputMessageKind kind)
        {
            Text = text;
            Kind = kind;
        }
    }

    public enum OutputMessageKind
    {
        Text,
        Info,
        Warning,
        Error
    }
}
