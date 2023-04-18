using CodeHighlighter;
using CodeHighlighter.Ancillary;
using CodeHighlighter.Model;
using DependencyInjection;
using Luna.IDE.AutoCompletion;
using Luna.IDE.CodeEditing;
using Luna.IDE.Common;
using Luna.IDE.Configuration;
using Luna.IDE.Outputing;
using Luna.IDE.ProjectChanging;
using Luna.IDE.ProjectExploration;
using Luna.IDE.TextDiff;
using Luna.IDE.Versioning;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.Infrastructure;

public class IDEInjectModule : InjectModule
{
    public override void Init(IBindingProvider provider)
    {
        provider.Bind<IDateTimeProvider, DateTimeProvider>();
        provider.Bind<ICodeProviderFactory, CodeProviderFactory>().ToSingleton();
        provider.Bind<IEnvironmentWindowsManager, EnvironmentWindowsManager>().ToSingleton();
        provider.Bind<ILineFoldsItemsUpdater, LineFoldsItemsUpdater>().ToSingleton();
        provider.Bind<ICodeEditorSaver, CodeEditorSaver>().ToSingleton();
        provider.Bind<ICodeEditorUndoChangesLogic, CodeEditorUndoChangesLogic>().ToSingleton();
        provider.Bind<IVersionControlRepositoryFactory, VersionControlRepositoryFactory>().ToSingleton();
        provider.Bind<IProjectRepository, ProjectRepository>().ToSingleton();
        provider.Bind<ITextDiffEngine, TextDiffEngine>().ToSingleton();
        provider.Bind<ITextDiffCodeProviderFactory, TextDiffCodeProviderFactory>().ToSingleton();
        provider.Bind<IFoldableRegions, FoldableRegions>().ToSingleton();
        provider.Bind<ITokenKindsUpdater, TokenKindsUpdater>().ToSingleton();
        provider.Bind<IFoldableRegionsUpdaterFactory, FoldableRegionsUpdaterFactory>().ToSingleton();
        provider.Bind<ILineNumberPanelModel>().ToMethod(_ => LineNumberPanelModelFactory.MakeModel());
        provider.Bind<IProjectLoader, ProjectLoader>().ToSingleton();
        provider.Bind<ISelectedProject>().ToMethod(provider => provider.Resolve<IProjectLoader>());
        provider.Bind<IProjectExplorer, ProjectExplorer>().ToSingleton();
        provider.Bind<IConfigStorage, ConfigStorage>().ToSingleton();
        provider.Bind<ILastOpenedProjectFiles, LastOpenedProjectFiles>().ToSingleton();
        provider.Bind<IRecentProjects, RecentProjects>().ToSingleton();
        provider.Bind<IAutoComplete, AutoComplete>();
        provider.Bind<IAutoCompleteDataContextFactory, AutoCompleteDataContextFactory>();
        provider.Bind<IProjectChanges, ProjectChanges>().ToSingleton();
        provider.Bind<ILinesDecorationProcessor, LinesDecorationProcessor>();
        provider.Bind<ISingleTextDiffGapProcessor, SingleTextDiffGapProcessor>();
        provider.Bind<IDoubleTextDiffGapProcessor, DoubleTextDiffGapProcessor>();
        provider.Bind<IDiffCodeTextBox, DiffCodeTextBox>();
        provider.Bind<ISingleTextDiff, SingleTextDiff>();
        provider.Bind<IDoubleTextDiff, DoubleTextDiff>();
        provider.Bind<IOutputArea, OutputArea>().ToSingleton();
        provider.Bind<IOutputConsole, OutputConsole>().ToSingleton();
    }
}
