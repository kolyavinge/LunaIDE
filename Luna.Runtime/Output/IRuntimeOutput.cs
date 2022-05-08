namespace Luna.Output
{
    public interface IRuntimeOutput
    {
        void NewMessage(OutputMessage message);
        void NewLine();
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
