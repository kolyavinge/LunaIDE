using System;
using System.Collections.Generic;
using System.Text;

namespace Luna.Parsing
{
    public interface IScope
    {
        bool IsConstExist(string name);

        bool IsFunctionExist(string name);
        bool IsRunFunctionExist();
    }
}
