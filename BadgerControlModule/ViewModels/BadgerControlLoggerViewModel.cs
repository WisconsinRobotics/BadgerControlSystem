using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
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
