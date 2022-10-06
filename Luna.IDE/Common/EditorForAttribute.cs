namespace Luna.IDE.Common;

[AttributeUsage(AttributeTargets.Class)]
public class EditorForAttribute : Attribute
{
    public Type ProjectItemType { get; }

    public EditorForAttribute(Type projectItemType)
    {
        ProjectItemType = projectItemType;
    }
}
