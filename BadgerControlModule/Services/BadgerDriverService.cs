using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
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

		public Object initSetup;
		bool joystickDisconnected;
		int rightJoystickId;
		int leftJoystickId;
		bool rotateMode = false;
		const int ARM_ARRAY_LENGTH = 6;
		// Rotate button
		int rotateButtonCount = 0;
		bool rotateButtonToggled = false;
		int rotateButtonDebounce = 5;


		public BadgerDriverService(BadgerControlSubsystem badgerControlSubsystem)
        {

			rightJoystickId = 0;
			leftJoystickId = 0;

			initSetup = new Object();
			joystickDisconnected = false;

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
			bool success;
	
			double turntableSpeed = 0;
			double shoulderSpeed = 0;
			double elbowSpeed = 0;
			double wristLinearSpeed = 0;
			double clawRotateSpeed = 0;
			double clawGripSpeed = 0;


			if (joystickQuery == null)
			{
				lock (initSetup) // indetermined behavior when multiple JoystickQueryThreads get created - ensure only one gets created
				{
					if (joystickQuery == null)
					{
						joystickQuery = new JoystickQueryThread(2);
						joystickQuery.JoystickDisconnected += OnJoystickDisconnected;
						Thread joystickThread = new Thread(joystickQuery.QueryJoystick);
						joystickThread.Start();
					}
				}
			}

			if (joystickDisconnected)
			{
				bool isButtonPressed;
				List<int> ids = joystickQuery.GetJoystickIDs();
				foreach (int id in ids)
				{
					if (!joystickQuery.GetButton(id, JoystickButton.Trigger, out isButtonPressed))
						continue;
					if (isButtonPressed)
					{
						joystickDisconnected = false;
						break;
					}
				}
			}


			int numJoysticks = 0;
			if (joystickQuery != null)
				numJoysticks = joystickQuery.GetJoystickIDs().Count;

			if (numJoysticks == 2)
			{

				if (leftJoystickId == 0 || rightJoystickId == 0)
				{
					if (joystickQuery.PrimaryIDAssigned())
					{
						rightJoystickId = joystickQuery.GetPrimaryID();
						leftJoystickId = joystickQuery.GetSecondaryID();
					}
				}

				success = true;
				success &= joystickQuery.GetButtons(rightJoystickId, out primaryButtons);
				success &= joystickQuery.GetButtons(leftJoystickId, out secondaryButtons);

				success &= joystickQuery.GetXVelocity(rightJoystickId, out primaryXVelocity);
				success &= joystickQuery.GetYVelocity(rightJoystickId, out primaryYVelocity);
				success &= joystickQuery.GetZRotation(rightJoystickId, out primaryZRotation);

				success &= joystickQuery.GetXVelocity(leftJoystickId, out secondaryXVelocity);
				success &= joystickQuery.GetYVelocity(leftJoystickId, out secondaryYVelocity);
				success &= joystickQuery.GetZRotation(leftJoystickId, out secondaryZRotation);

				if (!success)
				{
					return;
				}

				RemotePrimitiveDriverService remotePimitiveDriverService = badgerControlSubsystem.CurrentDriveMode.remoteDriveService as RemotePrimitiveDriverService;
				if (remotePimitiveDriverService != null)
				{
					// SendWrenchCommand(long primaryXJoystickValue, long primaryYJoystickValue, long primaryZJoystickValue, long secondaryXJoystickValue, long secondaryYJoystickValue, long secondaryZJoystickValue, Component parentComponent);
					// sign gives direction, value gives magnitude (speed) -> [turntable, shoulder, elbow, wrist, claw rotation, claw grip]

					// Get speed
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
					

					// Rotate or Non Rotate Mode
					if (secondaryButtons[(int)JoystickButton.Button2])
					{
						if (++rotateButtonCount == rotateButtonDebounce)
						{
							rotateMode = !rotateMode;
							if (rotateMode)
								_eventAggregator.GetEvent<LoggerEvent>().Publish("Rotate Mode");
							else
								_eventAggregator.GetEvent<LoggerEvent>().Publish("Non Rotate Mode");
						}
					}
					else
					{
						rotateButtonCount = 0;
						rotateButtonToggled = false;
					}


					// Turntable [0] and claw rotate [4]
					if (rotateMode)
					{
						// Claw rotation
						if (primaryButtons[(int)JoystickButton.Button4])
							clawRotateSpeed = -1 * speed;
						else if (primaryButtons[(int)JoystickButton.Button3])
							clawRotateSpeed = speed;
						else
							clawRotateSpeed = 0;

						// Turntable
						if (secondaryButtons[(int)JoystickButton.Button4])
							turntableSpeed = -1 * speed;
						else if (secondaryButtons[(int)JoystickButton.Button3])
							turntableSpeed = speed;
						else
							turntableSpeed = 0;

						// All others
						shoulderSpeed = 0;
						elbowSpeed = 0;
						wristLinearSpeed = 0;
						clawGripSpeed = 0;
					}
					// Shoulder [1], elbow [2], wrist [3], claw grip [5]
					else
					{
						// Elbow
						if (primaryButtons[(int)JoystickButton.Button5])
							elbowSpeed = -1 * speed;
						else if (primaryButtons[(int)JoystickButton.Button3])
							elbowSpeed = speed;
						else
							elbowSpeed = 0;

						// Shoulder
						if (secondaryButtons[(int)JoystickButton.Button5])
							shoulderSpeed = -1 * speed;
						else if (secondaryButtons[(int)JoystickButton.Button3])
							shoulderSpeed = speed;
						else
							shoulderSpeed = 0;

						// Wrist
						if (secondaryButtons[(int)JoystickButton.Button6])
							wristLinearSpeed = -1 * speed;
						else if (secondaryButtons[(int)JoystickButton.Button4])
							wristLinearSpeed = speed;
						else
							wristLinearSpeed = 0;

						// Claw grip
						if (primaryButtons[(int)JoystickButton.Button6])
							clawGripSpeed = -1 * speed;
						else if (primaryButtons[(int)JoystickButton.Button4])
							clawGripSpeed = speed;
						else
							clawGripSpeed = 0;

						// All others
						turntableSpeed = 0;
						clawRotateSpeed = 0;
					}

					badgerControlSubsystem.CurrentDriveMode.SendWrenchCommand((long)turntableSpeed, (long)shoulderSpeed, (long)elbowSpeed, (long)wristLinearSpeed, (long)clawRotateSpeed, (long)clawGripSpeed);
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
			else
			{
				// ?
			}
		}

		public override long SleepTime
        {
            get
            {
                return 100;
            }
        }

		private void OnJoystickDisconnected(object sender, EventArgs e)
		{
			joystickDisconnected = true;
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
