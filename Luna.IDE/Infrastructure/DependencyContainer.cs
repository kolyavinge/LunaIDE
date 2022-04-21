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

            _container.Bind<IProjectExplorer, ProjectExplorer>().ToSingleton();

            _container.Bind<CodeEditorViewModel, CodeEditorViewModel>();
            _container.Bind<ProjectExplorerViewModel, ProjectExplorerViewModel>().ToSingleton();
            _container.Bind<ProjectTreeViewModel, ProjectTreeViewModel>().ToSingleton();

            _container.Bind<ProjectTreeItemOpenCommand, ProjectTreeItemOpenCommand>().ToSingleton();
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
