using System;
using System.Collections.Generic;
using System.Text;

namespace Luna.IDE.Model
{
    public class EnvironmentWindowComponents
    {
        public IEnvironmentWindowModel Model { get; }

        public object View { get; }

        public EnvironmentWindowComponents(IEnvironmentWindowModel model, object view)
        {
            Model = model;
            View = view;
        }
    }
}
