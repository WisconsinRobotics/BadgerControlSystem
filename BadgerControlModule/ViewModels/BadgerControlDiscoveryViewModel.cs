using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using BadgerControlModule.Services;
using BadgerControlModule.Utils;

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
