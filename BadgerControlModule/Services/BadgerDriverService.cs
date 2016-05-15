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

        long prevZRotation;
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
            long zRotation, yVelocity, secondaryYVelocity, slider;
            bool[] buttons;
            int primaryJoystickID = JoystickQueryThread.PRIMARY_JOYSTICK_UNASSIGNED;
            int secondaryJoystickID = JoystickQueryThread.PRIMARY_JOYSTICK_UNASSIGNED;
            uint scaledSlider;

            if (joystickQuery == null)
            {
                joystickQuery = new JoystickQueryThread(2);
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
            joystickQuery.GetZRotation(primaryJoystickID, out zRotation);
            joystickQuery.GetYVelocity(primaryJoystickID, out yVelocity);
            joystickQuery.GetYVelocity(secondaryJoystickID, out secondaryYVelocity);
            joystickQuery.GetSlider(primaryJoystickID, out slider);

            scaledSlider = (byte)(100 * (65535.0 - slider) / 65535.0);

            // Disable this check for now
            //if ((prevXVelocity == xVelocity) && (prevYVelocity == yVelocity))
            //    return;

            prevZRotation = zRotation;
            prevYVelocity = yVelocity;

            // HACK for mining competition
            if(buttons[(int)JoystickButton.Button3])
            {
                secondaryYVelocity = (secondaryYVelocity & 0xFF) | (scaledSlider << 8);
            }

            // TODO: FIX ME
            // temporary stopgap for e-stop
            if (buttons[(int)JoystickButton.Button2])
            {
                zRotation = 0;
                yVelocity = 0;
                secondaryYVelocity = 0;
            }

            if (badgerControlSubsystem.CurrentDriveMode != null)
            {
                //_eventAggregator.GetEvent<LoggerEvent>().Publish(string.Format("[P] Z: {0} | Y: {1} | [S] Y: {2}", zRotation, yVelocity, secondaryYVelocity));
                badgerControlSubsystem.CurrentDriveMode.SendDriveCommand(zRotation, yVelocity, secondaryYVelocity);
            }
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
