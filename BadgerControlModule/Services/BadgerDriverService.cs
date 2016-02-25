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
            int leftSpeed, rightSpeed;
            int speedIndex;

            if (joystickQuery == null)
            {
                joystickQuery = new JoystickQueryThread();
                Thread joystickThread = new Thread(joystickQuery.QueryJoystick);
                joystickThread.Start();
            }

            xVelocity = joystickQuery.XVelocity;
            yVelocity = joystickQuery.YVelocity;
            zRotation = joystickQuery.ZRotation;

            if ((prevXVelocity == xVelocity) && (prevYVelocity == yVelocity))
                return;

            prevXVelocity = xVelocity;
            prevYVelocity = yVelocity;

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
