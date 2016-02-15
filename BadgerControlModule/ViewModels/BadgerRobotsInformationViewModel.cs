using System.Collections.Generic;

using Prism.Mvvm;
using Prism.Events;

using BadgerJaus.Util;

using BadgerControlModule.Models;

namespace BadgerControlModule.ViewModels
{
    class BadgerRobotsInformationViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        BadgerControlSubsystem badgerControlSubsystem;
        Subsystem selectedSubsystem;

        public BadgerRobotsInformationViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            badgerControlSubsystem = BadgerControlSubsystem.GetInstance();
        }

        public IEnumerable<Subsystem> SubsystemList
        {
            get { return badgerControlSubsystem.DiscoveredSubsystems; }
        }

        public Subsystem SelectedSubsystem
        {
            get { return selectedSubsystem; }
            set { selectedSubsystem = value; }
        }
    }
}
