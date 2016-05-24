using System;
using System.Windows.Input;
using System.Net;

using Prism.Mvvm;
using Prism.Commands;
using Prism.Events;

using BadgerJaus.Util;

using BadgerControlModule.Models;
using BadgerControlModule.Services;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlMapViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        private BadgerControlSubsystem badgerControlSubsystem;

        public BadgerControlMapViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            badgerControlSubsystem = BadgerControlSubsystem.GetInstance();
        }
    }
}
