using DependencyInjection;
using Luna.Formatting;
using Luna.Infrastructure;
using Luna.Navigation;
using Luna.ProjectModel;
using Luna.Runtime;

namespace Luna.IDE.App.Infrastructure;

public class RuntimeInjectModule : InjectModule
{
    public override void Init(IBindingProvider provider)
    {
        provider.Bind<IFileSystem, FileSystem>().ToSingleton();
        provider.Bind<ITimerManager, TimerManager>().ToSingleton();
        provider.Bind<ICodeModelUpdater, CodeModelUpdater>().ToSingleton();
        provider.Bind<ICodeFormatterFactory, CodeFormatterFactory>().ToSingleton();
        provider.Bind<ICodeModelNavigator, CodeModelNavigator>().ToSingleton();
        provider.Bind<ISingleFileCodeModelBuilder, SingleFileCodeModelBuilder>().ToSingleton();
        provider.Bind<IDeclarationNavigator, DeclarationNavigator>().ToSingleton();
        provider.Bind<IInterpreterFactory, InterpreterFactory>().ToSingleton();
    }
}
