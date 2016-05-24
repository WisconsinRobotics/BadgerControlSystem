using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Modularity;
using Prism.Regions;
using BadgerControlModule.Models;

namespace BadgerControlModule
{
    public class BadgerControlModule : IModule
    {
        private readonly IRegionManager regionManager;
        private BadgerControlSubsystem subsystem;

        public BadgerControlModule(RegionManager regionManager)
        {
            this.regionManager = regionManager;
            subsystem = BadgerControlSubsystem.CreateInstance();
            subsystem.InitializeComponents();
        }

        public void Initialize()
        {
            regionManager.RegisterViewWithRegion("BadgerControlConnectionView", typeof(Views.BadgerControlConnectionView));
            regionManager.RegisterViewWithRegion("BadgerControlDriveView", typeof(Views.BadgerControlDriveView));
            regionManager.RegisterViewWithRegion("BadgerControlLoggerView", typeof(Views.BadgerControlLoggerView));
            regionManager.RegisterViewWithRegion("BadgerControlStatusView", typeof(Views.BadgerControlStatusView));
            regionManager.RegisterViewWithRegion("BadgerControlVisualView", typeof(Views.BadgerControlVisualView));
            regionManager.RegisterViewWithRegion("BadgerControlDiscoveryView", typeof(Views.BadgerControlDiscoveryView));
            regionManager.RegisterViewWithRegion("BadgerRobotsInformationView", typeof(Views.BadgerRobotsInformationView));
            regionManager.RegisterViewWithRegion("BadgerControlMapView", typeof(Views.BadgerControlMapView));
        }
    }
}