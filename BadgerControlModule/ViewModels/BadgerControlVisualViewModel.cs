using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Commands;
using BadgerControlModule.Views;
using BadgerJaus.Util;
using BadgerJaus.Services.Core;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using BadgerControlModule.Services;
using SlimDX;
using SlimDX.DirectInput;
using JoystickLibrary;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlVisualViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        JoystickQueryThread joystick;


        public BadgerControlVisualViewModel(IEventAggregator eventAggregator)
        {
            joystick = new JoystickQueryThread();
            Thread joystickThread = new Thread(joystick.QueryJoystick);
            joystickThread.Start();

            this._eventAggregator = eventAggregator;
        }
    }
}
