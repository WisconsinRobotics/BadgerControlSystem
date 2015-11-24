
using System.Windows;

using Prism.Modularity;
using Prism.Unity;

namespace BadgerControlSystem
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override void InitializeShell()
        {
            base.InitializeShell();

            Application.Current.MainWindow = (Window)this.Shell;
            Application.Current.MainWindow.Show();
        }

        protected override DependencyObject CreateShell()
        {
            return new Shell();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
            moduleCatalog.AddModule(typeof(BadgerControlModule.BadgerControlModule));
            /*
            moduleCatalog.AddModule(typeof(BadgerControlModule.Views.BadgerControlConnectionView));
            moduleCatalog.AddModule(typeof(BadgerControlModule.Views.BadgerControlDriveView));
            moduleCatalog.AddModule(typeof(BadgerControlModule.Views.BadgerControlStatusView));
            moduleCatalog.AddModule(typeof(BadgerControlModule.Views.BadgerControlLoggerView));
            moduleCatalog.AddModule(typeof(BadgerControlModule.Views.BadgerControlVisualView));
             * */
        }
    }
}
