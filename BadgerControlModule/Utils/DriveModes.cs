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
        public RemoteDriverService remoteDriveService;
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

        public void SendWrenchCommandRelative(long turntable, long shoulder, long elbow, long wrist, long rotationOfClaw, long claw)
        {
            remoteDriveService.SendWrenchCommandRelative(turntable, shoulder, elbow, wrist, rotationOfClaw, claw, parentComponent);
        }

        public void SendWrenchCommandQuasi(long primaryXJoystickValue, long primaryYJoystickValue, long primaryZJoystickValue, long secondaryXJoystickValue, long secondaryYJoystickValue, long secondaryZJoystickValue)
        {
            remoteDriveService.SendWrenchCommandQuasi(primaryXJoystickValue, primaryYJoystickValue, primaryZJoystickValue, secondaryXJoystickValue, secondaryYJoystickValue, secondaryZJoystickValue, parentComponent);
        }

        public override string ToString()
        {
            return parentComponent.JausAddress.ToString() + ": " + serviceName;
        }
    }
}
