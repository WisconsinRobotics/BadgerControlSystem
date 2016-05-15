using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BadgerJaus.Services.Core;
using BadgerJaus.Messages.PrimitiveDriver;
using BadgerJaus.Util;

using BadgerControlModule.Models;

namespace BadgerControlModule.Utils
{
    class RemotePrimitiveDriverService : RemoteDriverService
    {
        BadgerControlSubsystem badgerControlSubsystem;

        public RemotePrimitiveDriverService(BadgerControlSubsystem badgerControlSubsystem)
        {
            this.badgerControlSubsystem = badgerControlSubsystem;
        }

        public void SendDriveCommand(long xJoystickValue, long yJoystickValue, long zJoystickValue, Component parentComponent)
        {
            SetWrenchEffort setWrenchEffort = new SetWrenchEffort();
            setWrenchEffort.SetSource(badgerControlSubsystem.LocalAddress);
            setWrenchEffort.SetDestination(parentComponent.JausAddress);
            // This is intentional, do not attempt to swap the X and Y values.
            setWrenchEffort.SetPropulsiveLinearEffortX(yJoystickValue);
            setWrenchEffort.SetPropulsiveLinearEffortY(xJoystickValue);
            setWrenchEffort.SetPropulsiveLinearEffortZ((sbyte) (zJoystickValue & 0xFF));
            setWrenchEffort.SetPropulsiveRotationalEffortX(zJoystickValue >> 8);

            Transport.SendMessage(setWrenchEffort);
        }
    }
}
