using System;
using Luna.IDE.Commands;
using Luna.IDE.Model;
using Luna.IDE.ViewModel;
using Luna.Infrastructure;

namespace Luna.IDE.Infrastructure
{
    public static class DependencyContainer
    {
        private static readonly DependencyInjection.IDependencyContainer _container;

        static DependencyContainer()
        {
            _container = DependencyInjection.DependencyContainerFactory.MakeLiteContainer();
            InitContainer();
        }

        private static void InitContainer()
        {
            _container.Bind<IFileSystem, FileSystem>().ToSingleton();
            _container.Bind<IOpenFileDialog, OpenFileDialog>();

            _container.Bind<IProjectItemEditorFactory, ProjectItemEditorFactory>().ToSingleton();

            _container.Bind<IEnvironmentWindowsManager, EnvironmentWindowsManager>().ToSingleton();
            _container.Bind<EnvironmentWindowsViewModel, EnvironmentWindowsViewModel>().ToSingleton();

            _container.Bind<IProjectExplorer, ProjectExplorer>().ToSingleton();
            _container.Bind<ProjectExplorerViewModel, ProjectExplorerViewModel>().ToSingleton();
            _container.Bind<ProjectTreeViewModel, ProjectTreeViewModel>().ToSingleton();

            _container.Bind<IOpenProjectCommand, OpenProjectCommand>().ToSingleton();
            _container.Bind<IProjectItemOpenCommand, ProjectItemOpenCommand>().ToSingleton();
            _container.Bind<IProjectTreeItemOpenCommand, ProjectTreeItemOpenCommand>().ToSingleton();

            _container.Bind<MainViewModel, MainViewModel>().ToSingleton();
        }

        public static TDependency Resolve<TDependency>()
        {
            return _container.Resolve<TDependency>();
        }

        public static void Dispose()
        {
            _container.Dispose();
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class InjectAttribute : DependencyInjection.InjectAttribute { }
}
