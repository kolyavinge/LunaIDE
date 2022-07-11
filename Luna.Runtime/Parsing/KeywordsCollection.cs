using System.Collections;
using System.Collections.Generic;

namespace Luna.Parsing;

public class KeywordsCollection : IEnumerable<string>
{
    private readonly List<string> _keywords = new()
    {
        "import",
        "const",
        "lambda",
        "true",
        "false",
        "run",
    };

    public IEnumerator<string> GetEnumerator()
    {
        return _keywords.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _keywords.GetEnumerator();
    }
}
