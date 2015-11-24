using Prism.Mvvm;
using Prism.Events;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlDiscoveryViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

        public BadgerControlDiscoveryViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
        }
    }
}
