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

        public const long QUASI = 5;
        public const long RELATIVE = 10;


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
            setWrenchEffort.SetPropulsiveLinearEffortZ(zJoystickValue);
            Transport.SendMessage(setWrenchEffort);
        }

        public void SendWrenchCommandQuasi(long primaryXJoystickValue, long primaryYJoystickValue, long primaryZJoystickValue, long secondaryXJoystickValue, long secondaryYJoystickValue, long secondaryZJoystickValue, Component parentComponent)
        {
            SetWrenchEffort setWrenchEffort = new SetWrenchEffort();
            setWrenchEffort.SetSource(badgerControlSubsystem.LocalAddress);
            setWrenchEffort.SetDestination(parentComponent.JausAddress);
            setWrenchEffort.SetPropulsiveLinearEffortX(primaryXJoystickValue);
            setWrenchEffort.SetPropulsiveLinearEffortY(primaryYJoystickValue);
            setWrenchEffort.SetPropulsiveLinearEffortZ(primaryZJoystickValue);
            setWrenchEffort.SetPropulsiveRotationalEffortX(secondaryXJoystickValue);
            setWrenchEffort.SetPropulsiveRotationalEffortY(secondaryYJoystickValue);
            setWrenchEffort.SetPropulsiveRotationalEffortZ(secondaryZJoystickValue);
            setWrenchEffort.SetResistiveLinearEffortX(QUASI);
            Transport.SendMessage(setWrenchEffort);
        }

        public void SendWrenchCommandRelative(long turntable, long shoulder, long elbow, long wrist, long rotationOfClaw, long claw, Component parentComponent)
        {
            SetWrenchEffort setWrenchEffort = new SetWrenchEffort();
            setWrenchEffort.SetSource(badgerControlSubsystem.LocalAddress);
            setWrenchEffort.SetDestination(parentComponent.JausAddress);
            setWrenchEffort.SetPropulsiveLinearEffortX(turntable);
            setWrenchEffort.SetPropulsiveLinearEffortY(shoulder);
            setWrenchEffort.SetPropulsiveLinearEffortZ(elbow);
            setWrenchEffort.SetPropulsiveRotationalEffortX(wrist);
            setWrenchEffort.SetPropulsiveRotationalEffortY(rotationOfClaw);
            setWrenchEffort.SetPropulsiveRotationalEffortZ(claw);
            setWrenchEffort.SetResistiveLinearEffortX(RELATIVE);
            Transport.SendMessage(setWrenchEffort);
        }
    }
}
