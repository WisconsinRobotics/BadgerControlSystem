using Prism.Mvvm;
using Prism.Events;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlMainViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;

        public BadgerControlMainViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
        }
    }
}
