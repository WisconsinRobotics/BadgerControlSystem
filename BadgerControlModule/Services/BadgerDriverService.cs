using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Prism.Events;

using BadgerJaus.Messages;
using BadgerJaus.Services;
using BadgerJaus.Util;

using JoystickLibrary;

using BadgerControlModule.Models;
using BadgerControlModule.Views;

namespace BadgerControlModule.Services
{
    class BadgerDriverService : BaseService
    {
        protected readonly IEventAggregator _eventAggregator;

        JoystickQueryThread joystickQuery;

        long prevXVelocity;
        long prevYVelocity;

        bool joystickConfirmedFromVisualView;
        bool joystickConfirmedFromStatusView;
        bool joystickMessageGiven;

        BadgerControlSubsystem badgerControlSubsystem;

        public BadgerDriverService(BadgerControlSubsystem badgerControlSubsystem)
        {
            this.badgerControlSubsystem = badgerControlSubsystem;

            joystickConfirmedFromVisualView = false;
            joystickConfirmedFromStatusView = false;
            joystickMessageGiven = false;

            _eventAggregator = ApplicationService.Instance.EventAggregator;

            _eventAggregator.GetEvent<ConfirmJoystickEvent>().Subscribe((confirmationID) =>
            {
                if (confirmationID == BadgerControlStatusView.JOYSTICK_ID)
                    joystickConfirmedFromStatusView = true;
                else if (confirmationID == BadgerControlVisualView.JOYSTICK_ID)
                    joystickConfirmedFromVisualView = true;

                if (JoyStickConfirmed && !joystickMessageGiven)
                {
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Joystick connected.");
                    joystickMessageGiven = true; // jank hack to fix the duplicate event publishing due to multithreading
                }
            });
        }

        public override bool ImplementsAndHandledMessage(Message message, Component component)
        {
            return false;
        }

        public override bool IsSupported(int commandCode)
        {
            return false;
        }

        protected override void Execute(Component component)
        {
            long xVelocity, yVelocity, zRotation;
            bool[] buttons;
            int primaryJoystickID = JoystickQueryThread.PRIMARY_JOYSTICK_UNASSIGNED;
            int secondaryJoystickID = JoystickQueryThread.PRIMARY_JOYSTICK_UNASSIGNED;

            if (joystickQuery == null)
            {
                joystickQuery = new JoystickQueryThread();
                joystickQuery.Start();
            }

            if (!joystickQuery.PrimaryIDAssigned())
                return;

            if(primaryJoystickID == JoystickQueryThread.PRIMARY_JOYSTICK_UNASSIGNED)
                primaryJoystickID = joystickQuery.GetPrimaryID();

            if (secondaryJoystickID == JoystickQueryThread.PRIMARY_JOYSTICK_UNASSIGNED)
                secondaryJoystickID = joystickQuery.GetSecondaryID();

            // should check for correctness!
            joystickQuery.GetButtons(primaryJoystickID, out buttons);
            joystickQuery.GetXVelocity(primaryJoystickID, out xVelocity);
            joystickQuery.GetYVelocity(primaryJoystickID, out yVelocity);
            joystickQuery.GetYVelocity(secondaryJoystickID, out zRotation);

            // Disable this check for now
            //if ((prevXVelocity == xVelocity) && (prevYVelocity == yVelocity))
            //    return;

            prevXVelocity = xVelocity;
            prevYVelocity = yVelocity;

            // HACK for mining competition
            if(buttons[(int)JoystickButton.Button2])
            {
                zRotation = (zRotation & 0xFF) | 0x100;
            }

            // TODO: FIX ME
            // temporary stopgap for e-stop
                if (buttons[(int)JoystickButton.Button2])
            {
                xVelocity = 0;
                yVelocity = 0;
                zRotation = 0;
            }

            if (badgerControlSubsystem.CurrentDriveMode != null)
                badgerControlSubsystem.CurrentDriveMode.SendDriveCommand(xVelocity, yVelocity, zRotation);
        }

        public override long SleepTime
        {
            get
            {
                return 100;
            }
        }

        private bool JoyStickConfirmed
        {
            get
            {
                if (joystickConfirmedFromStatusView && joystickConfirmedFromVisualView)
                    return true;
                return false;
            }
        }
    }
}
