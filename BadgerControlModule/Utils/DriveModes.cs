using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BadgerJaus.Util;

using BadgerControlModule.Models;

namespace BadgerControlModule.Utils
{
    class DriveModes
    {
        private Component parentComponent;
        private RemoteDriverService remoteDriveService;
        private BadgerControlSubsystem badgerControlSubsystem;

        public DriveModes(BadgerControlSubsystem badgerControlSubsystem, Component parentComponent, RemoteDriverService remoteDriveService)
        {
            this.badgerControlSubsystem = badgerControlSubsystem;
            this.parentComponent = parentComponent;
            this.remoteDriveService = remoteDriveService;
        }

        public void SendDriveCommand(long xJoystickValue, long yJoystickValue, long zJoystickValue)
        {
            remoteDriveService.SendDriveCommand(xJoystickValue, yJoystickValue, zJoystickValue, parentComponent);
        }
    }
}
