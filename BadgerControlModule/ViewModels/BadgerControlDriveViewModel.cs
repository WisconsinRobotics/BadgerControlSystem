using System;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Events;

using BadgerControlModule.Models;
using BadgerControlModule.Services;
using BadgerControlModule.Utils;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlDriveViewModel : BindableBase
    {
        private DriveModeOption driveMode;
        private String driveModeString;
        protected readonly IEventAggregator _eventAggregator;
        private List<string> driveServices;

        BadgerControlSubsystem badgerControlSubsystem;

        public BadgerControlDriveViewModel(IEventAggregator eventAggregator)
        {
            this.Confirm = new DelegateCommand<object>(OnConfirm);
            this._eventAggregator = eventAggregator;
            driveMode = DriveModeOption.REMOTE;
            driveModeString = "Remote";
            driveServices = new List<string>();
            badgerControlSubsystem = BadgerControlSubsystem.GetInstance();
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

        public List<string> DriveServices
        {
            get { return driveServices; }
        }
    }
}
