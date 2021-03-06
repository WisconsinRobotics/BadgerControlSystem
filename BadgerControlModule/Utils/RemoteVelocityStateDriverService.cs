﻿using System;
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
            setVelocityCommand.VelocityZ = zJoystickValue;

            Transport.SendMessage(setVelocityCommand);
        }

        public void SendWrenchCommandQuasi(long primaryXJoystickValue, long primaryYJoystickValue, long primaryZJoystickValue, long secondaryXJoystickValue, long secondaryYJoystickValue, long secondaryZJoystickValue, Component parentComponent)
        {
        }

        public void SendWrenchCommandRelative(long turntable, long shoulder, long elbow, long wrist, long rotationOfClaw, long claw, Component parentComponent)
        {
        }

        public void SendArmReady(Component parentComponent)
        {
        }
    }
}
