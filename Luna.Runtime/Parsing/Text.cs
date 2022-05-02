using System.Collections.Generic;
using System.Linq;

namespace Luna.Parsing
{
    internal class Text
    {
        private readonly List<string> _lines = new();

        public Text(string text)
        {
            _lines.AddRange(text.Split('\n').Select(line => line.Replace("\r", "")));
        }

        public string GetLine(int index) => _lines[index];

        public int LinesCount => _lines.Count;
    }
}
