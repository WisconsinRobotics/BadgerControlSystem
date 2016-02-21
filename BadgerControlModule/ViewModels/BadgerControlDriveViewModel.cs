/*
 * Copyright (c) 2015, Wisconsin Robotics
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * * Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 * * Neither the name of Wisconsin Robotics nor the
 *   names of its contributors may be used to endorse or promote products
 *   derived from this software without specific prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL WISCONSIN ROBOTICS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
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

        ObservableCollection<DriveModes> driveModesList;

        BadgerControlSubsystem badgerControlSubsystem;

        public BadgerControlDriveViewModel(IEventAggregator eventAggregator)
        {
            this.Confirm = new DelegateCommand<object>(OnConfirm);
            this._eventAggregator = eventAggregator;
            driveMode = DriveModeOption.REMOTE;
            driveModeString = "Remote";
            driveServices = new List<string>();
            badgerControlSubsystem = BadgerControlSubsystem.GetInstance();
            driveModesList = badgerControlSubsystem.ObservableDriveModes;
            driveModesList.CollectionChanged += DriveModesChanged;
        }

        private void DriveModesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new System.Action(() =>
            {
                DriveModesList = badgerControlSubsystem.ObservableDriveModes;
            }
            ));
        }

        public ObservableCollection<DriveModes> DriveModesList
        {
            get { return driveModesList; }
            private set { driveModesList = value; }
        }

        public ICommand Confirm
        {
            get;
            private set;
        }

        private void OnConfirm(object arg)
        {
            // clicking the Confirm button will run this function
            //_eventAggregator.GetEvent<LoggerEvent>().Publish("Drive Mode changed to " + driveModeString + " (" + (int)driveMode + ")");
            //_eventAggregator.GetEvent<UpdateDriveModeEvent>().Publish((int) driveMode);
            badgerControlSubsystem.UpdateDriveModes();
        }
    }
}
