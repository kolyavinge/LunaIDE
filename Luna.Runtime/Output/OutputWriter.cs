using System.Collections.Generic;
using Luna.Parsing;

namespace Luna.Output
{
    internal interface IOutputWriter
    {
        void WriteParserMessage(ParserMessage message);
    }

    internal class OutputWriter : IOutputWriter
    {
        private readonly IRuntimeOutput _output;

        public OutputWriter(IRuntimeOutput output)
        {
            _output = output;
        }

        public void WriteParserMessage(ParserMessage message)
        {

        }
    }
}
