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

using BadgerControlModule.Utils;

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

        double multiplier = 0;

        double speed = 0;
        const double SPEED6 = 6;
        const double SPEED5 = 5;
        const double SPEED4 = 4;
        const double SPEED3 = 3;
        const double SPEED2 = 2;
        const double SPEED1 = 1;

        const long CLAW_SPEED = 30;

        bool wristMode = false;

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
            long primaryXVelocity, primaryYVelocity, primaryZRotation;
            long secondaryXVelocity, secondaryYVelocity, secondaryZRotation;
            bool[] primaryButtons, secondaryButtons;
            int primaryJoystickID = JoystickQueryThread.PRIMARY_JOYSTICK_UNASSIGNED;
            int secondaryJoystickID = JoystickQueryThread.PRIMARY_JOYSTICK_UNASSIGNED;

            double turntableSpeed = 0;
            double shoulderSpeed = 0;
            double elbowSpeed = 0;
            double wristLinearSpeed = 0;
            double wristRotateSpeed = 0;

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
            joystickQuery.GetButtons(primaryJoystickID, out primaryButtons);
            joystickQuery.GetXVelocity(primaryJoystickID, out primaryXVelocity);
            joystickQuery.GetYVelocity(primaryJoystickID, out primaryYVelocity);
            joystickQuery.GetZRotation(primaryJoystickID, out primaryZRotation);

            joystickQuery.GetButtons(secondaryJoystickID, out secondaryButtons);
            joystickQuery.GetXVelocity(secondaryJoystickID, out secondaryXVelocity);
            joystickQuery.GetYVelocity(secondaryJoystickID, out secondaryYVelocity);
            joystickQuery.GetZRotation(secondaryJoystickID, out secondaryZRotation);


            // FIXME: Check whether this actually works in C#
            RemotePrimitiveDriverService remotePrimitiveDriverService = badgerControlSubsystem.CurrentDriveMode.remoteDriveService as RemotePrimitiveDriverService;
            if (remotePrimitiveDriverService != null)
            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           
                if (primaryButtons[(int)JoystickButton.Button7])
                {
                    multiplier = SPEED6;
                    speed = SPEED6;
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Speed = 6");
                }
                else if (primaryButtons[(int)JoystickButton.Button8])
                {
                    multiplier = SPEED5;
                    speed = SPEED5;
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Speed = 5");
                }
                else if (primaryButtons[(int)JoystickButton.Button9])
                {
                    multiplier = SPEED4;
                    speed = SPEED4;
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Speed = 4");
                }
                else if (primaryButtons[(int)JoystickButton.Button10])
                {
                    multiplier = SPEED3;
                    speed = SPEED3;
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Speed = 3");
                }
                else if (primaryButtons[(int)JoystickButton.Button11])
                {
                    multiplier = SPEED2;
                    speed = SPEED2;
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Speed = 2");
                }
                else if (primaryButtons[(int)JoystickButton.Button12])
                {
                    multiplier = SPEED1;
                    speed = SPEED1;
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Speed = 1");
                }

                //primaryXVelocity = (long)(primaryXVelocity * multiplier);
                //primaryYVelocity = (long)(primaryYVelocity * multiplier);
                //primaryZRotation = (long)(primaryZRotation * multiplier);
                //secondaryXVelocity = (long)(secondaryXVelocity * multiplier);
                //secondaryYVelocity = (long)(secondaryYVelocity * multiplier);
                //secondaryZRotation = (long)(secondaryZRotation * multiplier);

                turntableSpeed = speed;
                shoulderSpeed = speed;
                elbowSpeed = speed;
                wristLinearSpeed = speed;
                wristRotateSpeed = speed;


                if (secondaryButtons[(int)JoystickButton.Button11])
                {
                    wristMode = !wristMode;
                }

                if (wristMode)
                {
                    if (primaryYVelocity < 0)
                        wristLinearSpeed *= -1;
                    if (secondaryXVelocity < 0)
                        wristRotateSpeed *= -1;

                    if (primaryYVelocity > -20 && primaryYVelocity < 20)
                        wristLinearSpeed = 0;
                    if (secondaryXVelocity > -20 && secondaryXVelocity < 20)
                        wristRotateSpeed = 0;

                    if (primaryButtons[(int)JoystickButton.Button3])
                        if (badgerControlSubsystem.CurrentDriveMode != null)
                            badgerControlSubsystem.CurrentDriveMode.SendWrenchCommand(0, 0, 0, (long)wristLinearSpeed, (long)wristRotateSpeed, (long)speed);
                    else if (primaryButtons[(int)JoystickButton.Button4])
                        if (badgerControlSubsystem.CurrentDriveMode != null)
                            badgerControlSubsystem.CurrentDriveMode.SendWrenchCommand(0, 0, 0, (long)wristLinearSpeed, (long)wristRotateSpeed, (long)-speed);
                    else
                        if (badgerControlSubsystem.CurrentDriveMode != null)
                            badgerControlSubsystem.CurrentDriveMode.SendWrenchCommand(0, 0, 0, (long)wristLinearSpeed, (long)wristRotateSpeed, 0);
                }
                else
                {
                    if (primaryZRotation < 0)
                        turntableSpeed *= -1;
                    if (primaryYVelocity < 0)
                        shoulderSpeed *= -1;
                    if (secondaryYVelocity < 0)
                        elbowSpeed *= -1;
                    
                    if (primaryZRotation > -20 && primaryZRotation < 20)
                        turntableSpeed = 0;
                    if (primaryYVelocity > -20 && primaryYVelocity < 20)
                        shoulderSpeed = 0;
                    if (secondaryYVelocity > -20 && secondaryYVelocity < 20)
                        elbowSpeed = 0;

                    if (badgerControlSubsystem.CurrentDriveMode != null)
                        badgerControlSubsystem.CurrentDriveMode.SendWrenchCommand((long)turntableSpeed, (long)shoulderSpeed, (long)elbowSpeed, 0, 0, 0);
                }


            }
            else
            {
                primaryZRotation = 0;

                // temporary stopgap for e-stop
                if (primaryButtons[(int)JoystickButton.Button2])
                {
                    primaryYVelocity = 0;
                    secondaryYVelocity = 0;
                    primaryZRotation = 0;
                }

                if (badgerControlSubsystem.CurrentDriveMode != null)
                    badgerControlSubsystem.CurrentDriveMode.SendDriveCommand(secondaryYVelocity, primaryYVelocity, primaryZRotation);
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
