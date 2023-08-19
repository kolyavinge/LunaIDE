using Luna.IDE.Outputing;
using Luna.IDE.ProjectChanging;
using Luna.IDE.ProjectExploration;
using WindowsEnvironment.Model;

namespace Luna.IDE.WindowsManagement;

public interface IEnvironmentWindowsInitializer
{
    void Init();
}

internal class EnvironmentWindowsInitializer : IEnvironmentWindowsInitializer
{
    private readonly IEnvironmentWindowsFactory _environmentWindowsFactory;
    private readonly IFlexWindowsEnvironment _flexEnvironment;

    public EnvironmentWindowsInitializer(
        IEnvironmentWindowsFactory environmentWindowsFactory,
        IFlexWindowsEnvironment flexEnvironment)
    {
        _environmentWindowsFactory = environmentWindowsFactory;
        _flexEnvironment = flexEnvironment;
    }

    public void Init()
    {
        var projectExplorer = _environmentWindowsFactory.MakeWindowFor(typeof(ProjectExplorer));
        var projectChanges = _environmentWindowsFactory.MakeWindowFor(typeof(ProjectChanges));
        var outputConsole = _environmentWindowsFactory.MakeWindowFor(typeof(OutputConsole));

        var (projectExplorerPanel, _) = _flexEnvironment.SetPanelPosition(MainPanel.Name, PanelPosition.Left, new(projectExplorer)
        {
            Header = new() { SourceObject = projectExplorer.Model, PropertyName = "Header" },
            View = projectExplorer.View.Content,
        });
        projectExplorerPanel.Size = 200;

        _flexEnvironment.SetPanelPosition(projectExplorerPanel.Name, PanelPosition.Middle, new(projectChanges)
        {
            Header = new() { SourceObject = projectChanges.Model, PropertyName = "Header" },
            View = projectChanges.View.Content,
        });

        var (outputConsolePanel, _) = _flexEnvironment.SetPanelPosition(MainPanel.Name, PanelPosition.Bottom, new(outputConsole)
        {
            Header = new() { SourceObject = outputConsole.Model, PropertyName = "Header" },
            View = outputConsole.View.Content,
        });
        outputConsolePanel.Size = 200;
    }
}
