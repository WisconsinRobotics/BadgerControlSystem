using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;

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
