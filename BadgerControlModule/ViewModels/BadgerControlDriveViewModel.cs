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
    class BadgerControlDriveViewModel : BindableBase
    {
        private DriveModeOption driveMode;
        private String driveModeString;
        protected readonly IEventAggregator _eventAggregator;

        public BadgerControlDriveViewModel(IEventAggregator eventAggregator)
        {
            this.Confirm = new DelegateCommand<object>(OnConfirm);
            this._eventAggregator = eventAggregator;
            driveMode = DriveModeOption.REMOTE;
            driveModeString = "Remote";
        }

        public ICommand Confirm
        {
            get;
            private set;
        }

        private void OnConfirm(object arg)
        {
            // clicking the Confirm button will run this function
            _eventAggregator.GetEvent<LoggerEvent>().Publish("Drive Mode changed to " + driveModeString + " (" + (int)driveMode + ")");
            _eventAggregator.GetEvent<UpdateDriveModeEvent>().Publish((int) driveMode);
        }

        // changing the value of the mode combobox will trigger this 
        public String DriveMode
        {
            get { return driveMode + ""; }
            set 
            {
                switch (value)
                {
                    case "Direct":
                        driveMode = DriveModeOption.DIRECT;
                        driveModeString = "Direct";
                        break;
                    case "Remote":
                        driveMode = DriveModeOption.REMOTE;
                        driveModeString = "Remote";
                        break;
                    case "AI":
                        driveMode = DriveModeOption.AI;
                        driveModeString = "AI";
                        break;
                }
            }
        }
    }
}
