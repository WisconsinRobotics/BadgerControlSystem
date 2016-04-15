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
            setWrenchEffort.SetPropulsiveLinearEffortX(yJoystickValue);
            Transport.SendMessage(setWrenchEffort);
        }
    }
}
