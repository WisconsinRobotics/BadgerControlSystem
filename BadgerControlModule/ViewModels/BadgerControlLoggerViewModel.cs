using System;
using Prism.Mvvm;
using Prism.Events;

using BadgerControlModule.Services;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlLoggerViewModel : BindableBase
    {
        private String logText;
        protected readonly IEventAggregator _eventAggregator;

        public BadgerControlLoggerViewModel(IEventAggregator eventAggregator)
        {
            logText = "Welcome to the Badger Control GUI!";

            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<LoggerEvent>().Subscribe((text) =>                      
            {
                this.LogText = text;
            });
        }

        public String LogText
        {
            get 
            { 
                return logText; 
            }
            set 
            { 
                SetProperty(ref this.logText, value + "\n" + this.logText);
            }
        }
    }
}
