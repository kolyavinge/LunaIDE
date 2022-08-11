using DependencyInjection;
using Luna.Infrastructure;
using Luna.Navigation;
using Luna.ProjectModel;
using Luna.Runtime;

namespace Luna.IDE.Infrastructure;

public class RuntimeInjectModule : InjectModule
{
    public override void Init(IBindingProvider provider)
    {
        provider.Bind<IFileSystem, FileSystem>().ToSingleton();
        provider.Bind<ITimerManager, TimerManager>().ToSingleton();
        provider.Bind<ICodeModelUpdater, CodeModelUpdater>().ToSingleton();
        provider.Bind<ICodeModelNavigator, CodeModelNavigator>().ToSingleton();
        provider.Bind<IDeclarationNavigator, DeclarationNavigator>().ToSingleton();
        provider.Bind<IInterpreter, Interpreter>().ToSingleton();
    }
}
