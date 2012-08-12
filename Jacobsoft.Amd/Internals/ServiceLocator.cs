using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using Jacobsoft.Amd.Config;
using Jacobsoft.Amd.Internals;

namespace Jacobsoft.Amd.Internals
{
    internal class ServiceLocator : IServiceLocator
    {
        private IDictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();

        static ServiceLocator()
        {
            Instance = new ServiceLocator();
        }

        public ServiceLocator()
        {
            this.RegisterFactory<HttpContextBase>(() => {
                var context = HttpContext.Current;
                return context == null ? null : new HttpContextWrapper(context);
            });
            this.RegisterFactory<HttpServerUtilityBase>(() => {
                var context = HttpContext.Current;
                return context == null 
                    ? null 
                    : new HttpServerUtilityWrapper(context.Server);
            });
            
            this.RegisterSingleton<IAmdConfiguration, AmdConfiguration>();
            this.RegisterSingleton<IModuleRepository, ModuleRepository>();
            this.RegisterSingleton<IModuleResolver, ModuleResolver>();
            this.RegisterSingleton<IFileSystem, FileSystem>();
            this.RegisterSingleton<IVersionProvider, AssemblyVersionProvider>();
        }

        public static IServiceLocator Instance { get; internal set; }

        public T Get<T>() where T : class
        {
            return (T)Get(typeof(T));
        }

        public object Get(Type type)
        {
            return factories[type].Invoke();
        }

        private void RegisterSingleton<TService, TImpl>() where TImpl : TService
        {
            this.RegisterLazyInstance<TService, TImpl>(this.GenerateFactory<TImpl>());
        }

        private void RegisterLazyInstance<TService, TImpl>(Func<TImpl> factory)
            where TImpl : TService
        {
            Lazy<TImpl> lazyInitializer = new Lazy<TImpl>(factory);
            this.RegisterFactory<TService>(() => lazyInitializer.Value);
        }

        private void RegisterFactory<TService>(Func<TService> factory)
        {
            factories[typeof(TService)] = () => factory();
        }

        private Func<T> GenerateFactory<T>()
        {
            var constructor = typeof(T)
                .GetConstructors(
                    BindingFlags.Public 
                    | BindingFlags.NonPublic 
                    | BindingFlags.Instance)
                .OrderByDescending(c => c.GetParameters().Count())
                .First();

            return Expression.Lambda<Func<T>>(
                Expression.New(
                    constructor,
                    from param in constructor.GetParameters()
                    select Expression.Call(
                        Expression.Constant(this),
                        "Get",
                        new[] { param.ParameterType }))
            ).Compile();
        }
    }
}
