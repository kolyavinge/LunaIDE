using Luna.IDE.ViewModel;
using System;

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
            _container.Bind<EditorViewModel, EditorViewModel>();
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
