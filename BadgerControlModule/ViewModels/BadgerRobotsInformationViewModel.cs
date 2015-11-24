
using Prism.Mvvm;
using Prism.Events;

namespace BadgerControlModule.ViewModels
{
    class BadgerRobotsInformationViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

        public BadgerRobotsInformationViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
        }
    }
}
