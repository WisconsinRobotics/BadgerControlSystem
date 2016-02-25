using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BadgerJaus.Services.Core;
using BadgerJaus.Messages.VelocityStateDriver;
using BadgerJaus.Util;

using BadgerControlModule.Models;

namespace BadgerControlModule.Utils
{
    class RemoteVelocityStateDriverService : RemoteDriverService
    {
        BadgerControlSubsystem badgerControlSubsystem;

        public RemoteVelocityStateDriverService(BadgerControlSubsystem badgerControlSubsystem)
        {
            this.badgerControlSubsystem = badgerControlSubsystem;
        }

        public void SendDriveCommand(long xJoystickValue, long yJoystickValue, long zJoystickValue, Component parentComponent)
        {
            SetVelocityCommand setVelocityCommand = new SetVelocityCommand();
            setVelocityCommand.SetSource(badgerControlSubsystem.LocalAddress);
            setVelocityCommand.SetDestination(parentComponent.JausAddress);

            setVelocityCommand.VelocityX = xJoystickValue;
            setVelocityCommand.VelocityY = yJoystickValue;
            //setVelocityCommand.VelocityZ = zJoystickValue;

            Transport.SendMessage(setVelocityCommand);
        }
    }
}
