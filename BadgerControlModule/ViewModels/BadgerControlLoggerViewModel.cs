using System;
using Prism.Mvvm;
using Prism.Events;

using BadgerControlModule.Services;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlLoggerViewModel : BindableBase
    {
        private string logText;
        protected readonly IEventAggregator _eventAggregator;

        public BadgerControlLoggerViewModel(IEventAggregator eventAggregator)
        {
            logText = GetTimeStamp(DateTime.Now) + "Welcome to the Badger Control GUI!";
           
            this._eventAggregator = eventAggregator;
            this._eventAggregator.GetEvent<LoggerEvent>().Subscribe((text) =>                      
            {
                string timeStamp = GetTimeStamp(DateTime.Now);
                this.LogText = timeStamp + text;
            });
        }

        public string GetTimeStamp(DateTime value)
        {
            return value.ToString("[HH:mm:ss dd/MM] ");
        }

        public string LogText
        {
            get 
            { 
                return logText; 
            }
            set 
            { 
                SetProperty(ref this.logText, this.logText + "\n" + value);
            }
        }
    }
}
