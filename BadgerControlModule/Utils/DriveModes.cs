using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BadgerJaus.Util;

using BadgerControlModule.Models;

namespace BadgerControlModule.Utils
{
    public class DriveModes
    {
        private Component parentComponent;
        private RemoteDriverService remoteDriveService;
        private BadgerControlSubsystem badgerControlSubsystem;
        string serviceName;

        public DriveModes(BadgerControlSubsystem badgerControlSubsystem, Component parentComponent, RemoteDriverService remoteDriveService, string serviceName)
        {
            this.badgerControlSubsystem = badgerControlSubsystem;
            this.parentComponent = parentComponent;
            this.remoteDriveService = remoteDriveService;
            this.serviceName = serviceName;
        }

        public void SendDriveCommand(long xJoystickValue, long yJoystickValue, long zJoystickValue)
        {
            remoteDriveService.SendDriveCommand(xJoystickValue, yJoystickValue, zJoystickValue, parentComponent);
        }

        public void SendWrenchCommand(long primaryXJoystickValue, long primaryYJoystickValue, long primaryZJoystickValue, long secondaryXJoystickValue, long secondaryYJoystickValue, long secondaryZJoystickValue)
        {
            remoteDriveService.SendWrenchCommand(primaryXJoystickValue, primaryYJoystickValue, primaryZJoystickValue, secondaryXJoystickValue, secondaryYJoystickValue, secondaryZJoystickValue, parentComponent);
        }

        public override string ToString()
        {
            return parentComponent.JausAddress.ToString() + ": " + serviceName;
        }
    }
}
