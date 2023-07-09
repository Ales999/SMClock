/*

    This file is part of SmClock.

    SmClock is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    SmClock is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Foobar.  If not, see <https://www.gnu.org/licenses/>.

  (Этот файл — часть SmClock.

   SmClock - свободная программа: вы можете перераспространять ее и/или
   изменять ее на условиях Стандартной общественной лицензии GNU в том виде,
   в каком она была опубликована Фондом свободного программного обеспечения;
   либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
   версии.

   SmClock распространяется в надежде, что она будет полезной,
   но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
   или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Стандартной
   общественной лицензии GNU.

   Вы должны были получить копию Стандартной общественной лицензии GNU
   вместе с этой программой. Если это не так, см.
   <https://www.gnu.org/licenses/>.)

*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using CommonLib;
using CommonLib.Interfaces;
using SMClock.Modules.SheduleConfig;
using SMClock.Modules.TrayIcon;
using SMClock.Modules.TrayIcon.Views;
using StructureMap;
using StructureMap.TypeRules;
using Registry = StructureMap.Registry;

#if DEBUG
using System.Diagnostics;
#endif

namespace SMClock
{
    internal class StructureMapBootstrapper : Caliburn.Micro.BootstrapperBase, IDisposable
    {

        public static readonly Container ContainerInstance = new Container();

        public StructureMapBootstrapper()
        {
#if DEBUG
            LogManager.GetLog = type => new DebugLog(type);
#endif
            PreInitialize();
            Initialize();
        }

        #region PreInitialize - Localization app

        private void PreInitialize()
        {

            var code = Properties.Settings.Default.LanguageCode;

            if (string.IsNullOrWhiteSpace(code)) return;

            var culture = CultureInfo.GetCultureInfo(code);
            // TODO: Локализация
            //Translator.Culture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
        }
        #endregion

        
        protected override void Configure()
        {
            if (!Execute.InDesignMode)
            {
                this.ConfigureIocContainer();
                //this.MyTest();
            }
        }

        private void ConfigureIocContainer()
        {
            ContainerInstance.Configure(GetStructureMapConfig);
        }

        private void GetStructureMapConfig(ConfigurationExpression cfg)
        {
            cfg.For<IWindowManager>().Use<WindowManager>().Singleton();
            cfg.For<IEventAggregator>().Use<EventAggregator>().Singleton();
            cfg.For<ILogger>().Singleton().Use<Logger>();


            var exeDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (exeDir != null)
            {
                var modDir = Path.Combine(exeDir, "Modules");

                var registry = new Registry();


                #region MediatR registry

                // ------- START MediatR Configure -----------------------

                // INFO: Пока в этом сканере нет необходимости
                /*
                // используем внешний mediator pattern
                registry.Scan(scanner =>
                {
                    scanner.TheCallingAssembly();
                    //scanner.Assembly(this.GetType().Assembly); // ?
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                    scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<>)); // Async handlers with no response
                    scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>)); // Async Handlers with a response
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                });
                */

                // Scan other assembly modules Regisry Types with DLL like name *Extension.dll
                registry.Scan(scanner =>
                {
                    if (Directory.Exists(modDir))
                    {
                        //  More examples Scan:
                        // https://github.com/structuremap/structuremap/blob/master/src/StructureMap.Testing/Examples.cs
                        scanner.AssembliesFromPath(modDir, assembly => assembly.GetName().Name.Contains("Extension"));
                    }

                    scanner.WithDefaultConventions();
                    // Если есть, автоматичесски запускать регистрацию классов производных от Registry
                    scanner.LookForRegistries(); 
                    //canner.Convention<ViewRegistrationConvention>();
                });

                //registry.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
                //registry.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
                //registry.For<IMediator>().Use<Mediator>();

                // ------- END MediatR Configure -----------------------

                #endregion

                registry.IncludeRegistry<SheduleConfigRegistry>();
                registry.IncludeRegistry<SystemTrayIconRegistry>();

                cfg.AddRegistry(registry);
            }
        }

        

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            var windowManager = ContainerInstance.GetInstance<IWindowManager>();
            var appViewModel = ContainerInstance.GetInstance<ISystemTrayIcon>();

            //windowManager.ShowWindow(appViewModel); // Old caliburn
            windowManager.ShowWindowAsync(appViewModel);
        }

        protected override void BuildUp(object instance)
        {
           ContainerInstance.BuildUp(instance);
        }

        #region Get Instance/ces

        protected override object GetInstance(Type serviceType, string key)
        {
            
            //return _container.GetInstance(serviceType);
            if (serviceType == null) serviceType = typeof(object);
            var returnValue = key == null
                ? ContainerInstance.GetInstance(serviceType) : ContainerInstance.GetInstance(serviceType, key);
            return returnValue;
            
            
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            //yield return _container.GetAllInstances(serviceType);
            return ContainerInstance.GetAllInstances(serviceType).OfType<object>();
        }
        #endregion

        /// <summary>
        /// Нужно для Caliburn.Micro в поиске ViewMode самой View
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            /*

             if (Execute.InDesignMode)
             {
                 var assemblies = new HashSet<Assembly>();
                 foreach (var assembly in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                 {
                     assemblies.Add(Assembly.Load(assembly));
                 }
                 return assemblies;
             }
             */

            yield return typeof(SystemTrayIconView).GetTypeInfo().Assembly;
            yield return typeof(App).GetTypeInfo().Assembly;
            //yield return typeof(AClockView).GetTypeInfo().Assembly;
            //yield return typeof(SystemTrayIconView).GetTypeInfo().Assembly;

            // example add this:
            //yield return typeof(LoginViewModel).GetTypeInfo().Assembly;
            //yield return typeof(LoginView).GetTypeInfo().Assembly;



            /*
            IEnumerable<Assembly> retAssemblies = new List<Assembly>
            {
                Assembly.GetExecutingAssembly(),
               // AssemblySource.Instance.Select(x=> new Assembly)
                typeof(IAClock).Assembly
            };
            return retAssemblies;
            */

            /*
            return new[]
            {
                Assembly.GetExecutingAssembly(),
                typeof(IAClock).Assembly
            };
            */
            /*
            //var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Modules";
            return new[]
            {
                Assembly.GetExecutingAssembly()
                //,
                //typeof(IAClock).Assembly,
                //Assembly.LoadFile(Path.Combine(path, "AClockLibraryExtension.dll"))
            };
            */


            /*
            foreach (var assembly in base.SelectAssemblies())
            {
                yield return assembly;
            }

            yield return typeof(SystemTrayIconView).Assembly; // !!!IMPORTANT!!!
            yield return Assembly.GetEntryAssembly();
            //yield return typeof(TViewModel).Assembly;
            */
            /*
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Modules";

            return base.SelectAssemblies().Concat(
                new Assembly[]
                {
                    Assembly.LoadFile(Path.Combine(path, "AClockLibraryExtension.dll")),
                    Assembly.GetExecutingAssembly()
                });
            */
        }

        #region Log Unhandled Exception

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {

            var logger = ContainerInstance.TryGetInstance<ILogger>();
            if (logger != null)
            {

                logger.Error($"Unhandled Exception: {e.Exception.Message}");

                if (e.Exception.InnerException != null)
                    logger.Error($"Unhandled Inner Exception: {e.Exception.InnerException.Message}");
            }
        
            base.OnUnhandledException(sender, e);
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты).
                    
                    //ContainerInstance.Release();
                    ContainerInstance.Dispose();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже метод завершения.
                // TODO: задать большим полям значение NULL.

                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если Dispose(bool disposing) выше включает код для освобождения неуправляемых ресурсов.
        // ~StructureMapBootstrapper() {
        //   // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
        //   Dispose(false);
        // }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        void IDisposable.Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
}
