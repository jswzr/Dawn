using AdonisUI;
using DryIoc;
using Jot;
using Serilog;
using System.Windows;
using System.Windows.Threading;

namespace Dawn.Wpf
{
    public partial class App : Application
    {
        private readonly IContainer _container;
        private readonly Tracker _tracker;
        private readonly ConfigurationService _configurationService;

        public App()
        {
            _container = CompositionRoot.Get();
            _tracker = _container.Resolve<Tracker>();

            _configurationService = _container.Resolve<ConfigurationService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var model = _configurationService.Get();

            ResourceLocator.SetColorScheme(Current.Resources, model.IsLightTheme ? ResourceLocator.LightColorScheme : ResourceLocator.DarkColorScheme);

            var shell = _container.Resolve<Shell>();
            shell.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _tracker.PersistAll();
            _configurationService.Save();

            Log.CloseAndFlush();

            _container.Dispose();

            base.OnExit(e);
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is System.Runtime.InteropServices.COMException comException && comException.ErrorCode == -2147221040) // copy to clipboard failed
            {
                e.Handled = true;
            }
        }
    }
}
