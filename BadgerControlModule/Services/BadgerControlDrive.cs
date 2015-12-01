using System;
using System.Threading;

using Prism.Events;

using BadgerJaus.Services;
using BadgerJaus.Services.Core;
using BadgerJaus.Messages;
using BadgerJaus.Messages.Control;
using BadgerJaus.Messages.LocalVectorDriver;
using BadgerJaus.Messages.Management;
using BadgerJaus.Util;
using BadgerControlModule.Views;

using JoystickLibrary;

namespace BadgerControlModule.Services
{
    public class BadgerControlDrive : BaseService
    {
        const int SLEEP_TIME = 100; // millis

        bool isEnabled;
        bool hasControl;
        bool isReady;
        bool triedJoystick;

        bool componentOneActive;
        bool componentTwoActive;
        bool componentThreeActive;

        bool joystickConfirmedFromVisualView;
        bool joystickConfirmedFromStatusView;
        bool joystickMessageGiven;

        ConnectionDetails connectionDetails;

        JoystickQueryThread joystickQueryThread;
        JausAddress destinationAddress;
        protected readonly IEventAggregator _eventAggregator;

        public BadgerControlDrive()
        {
            isEnabled = false;
            hasControl = false;
            isReady = false;
            triedJoystick = false;
            joystickMessageGiven = false;
            componentOneActive = componentTwoActive = componentThreeActive = false;
            joystickConfirmedFromVisualView = false;
            joystickConfirmedFromStatusView = false;

            _eventAggregator = ApplicationService.Instance.EventAggregator;

            // create an empty ConnectionDetails struct and keep it around 
            connectionDetails = new ConnectionDetails();
            connectionDetails.ai = ConnectionOption.DISCONNECTED;
            connectionDetails.remote = ConnectionOption.DISCONNECTED;
            connectionDetails.direct = ConnectionOption.DISCONNECTED;

            // record which classes have confirmed the joystick. write to the console only when both have confirmed
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

        // returns true when all joystick parties have confirmed
        private bool JoyStickConfirmed
        {
            get
            {
                if (joystickConfirmedFromStatusView && joystickConfirmedFromVisualView)
                    return true;
                return false;
            }
        }

        public override long SleepTime
        {
            get { return SLEEP_TIME; }
        }

        public JausAddress CurrentDestinationAddress 
        {
            get { return destinationAddress; }
            set { destinationAddress = value; }
        }

        public void ResetConnection() 
        {
            isReady = false;
        }

        public bool Enabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        protected override void Execute(Component component)
        {
            LocateJoystick();

            if (!isEnabled || destinationAddress == null)
                return;

            // request control and update the GUI to denote this
            if (!hasControl)
            {
                RequestControl requestControl = new RequestControl();
                requestControl.SetDestination(destinationAddress);
                requestControl.SetSource(component.JausAddress);
                Transport.SendMessage(requestControl);

                // update the connection icon for the correct component to orange
                if (destinationAddress.getComponent() == 1)
                    connectionDetails.direct = ConnectionOption.REQUESTING_CONTROL;
                if (destinationAddress.getComponent() == 2)
                    connectionDetails.remote = ConnectionOption.REQUESTING_CONTROL;
                if (destinationAddress.getComponent() == 3)
                    connectionDetails.ai = ConnectionOption.REQUESTING_CONTROL;
                _eventAggregator.GetEvent<ConnectionDetailsEvent>().Publish(connectionDetails);   

                return;
            }

            // shut down all components, then resume the one that should be active (i.e. jank hax)
            if (!isReady)
            {
                // shut down all active components
                Shutdown shutdown = new Shutdown();
                int subsys = CurrentDestinationAddress.getSubsystem();
                int node = CurrentDestinationAddress.getNode();
                JausAddress newAddress = new JausAddress(subsys, node, 1);
                QueryStatus queryStatus = new QueryStatus();

                if (componentOneActive)
                {
                    shutdown.SetDestination(newAddress);
                    Transport.SendMessage(shutdown);

                    queryStatus.SetDestination(newAddress);
                    queryStatus.SetSource(component.JausAddress);
                    Transport.SendMessage(queryStatus);
                }
                if (componentTwoActive)
                {
                    newAddress.setComponent(2);
                    shutdown.SetDestination(newAddress);
                    Transport.SendMessage(shutdown);

                    queryStatus.SetDestination(newAddress);
                    queryStatus.SetSource(component.JausAddress);
                    Transport.SendMessage(queryStatus);
                }
                if (componentThreeActive)
                {
                    newAddress.setComponent(3);
                    shutdown.SetDestination(newAddress);
                    Transport.SendMessage(shutdown);

                    queryStatus = new QueryStatus();
                    queryStatus.SetDestination(newAddress);
                    queryStatus.SetSource(component.JausAddress);
                    Transport.SendMessage(queryStatus);
                }

                // force component to boot
                Resume resume = new Resume();
                resume.SetDestination(destinationAddress);
                resume.SetSource(component.JausAddress);
                Transport.SendMessage(resume);

                // see if the component is ready
                queryStatus = new QueryStatus();
                queryStatus.SetDestination(destinationAddress);
                queryStatus.SetSource(component.JausAddress);
                Transport.SendMessage(queryStatus);  

                return;
            }

            // send a drive message
            SetLocalVector msg = new SetLocalVector();
            msg.SetDestination(destinationAddress);
            msg.SetSource(component.JausAddress);

            // convert joystick degrees into radians
            msg.SetHeading(joystickQueryThread.XVelocity * (Math.PI / 180));

            //adding 100 to fit into defined setLocalVector MAX_SPEED & MIN_SPEED
            msg.SetSpeed(joystickQueryThread.YVelocity + 100);
            
            Transport.SendMessage(msg);
        }

        public override bool ImplementsAndHandledMessage(BadgerJaus.Messages.Message message, Component component)
        {
            switch (message.GetCommandCode())
            {
                case JausCommandCode.CONFIRM_CONTROL:
                    hasControl = true;

                    // update the connection icon for the correct component to yellow and write to the logger
                    if (destinationAddress.getComponent() == 1)
                        connectionDetails.direct = ConnectionOption.AWAITING_STATUS;
                    if (destinationAddress.getComponent() == 2)
                        connectionDetails.remote = ConnectionOption.AWAITING_STATUS;
                    if (destinationAddress.getComponent() == 3)
                        connectionDetails.ai = ConnectionOption.AWAITING_STATUS;
                    _eventAggregator.GetEvent<ConnectionDetailsEvent>().Publish(connectionDetails);
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Control confirmed!");

                    return true;

                case JausCommandCode.REPORT_STATUS:
                    ReportStatus status = new ReportStatus();
                    status.SetFromJausMessage(message);
                    int sourceComponent = message.GetSource().getComponent();

                    // update the connection icon for the correct component to red or green accordingly
                    if (sourceComponent == 1)
                    {
                        if (status.GetStatus() == (int)ComponentState.STATE_SHUTDOWN)
                        {
                            componentOneActive = false;
                            connectionDetails.direct = ConnectionOption.DISCONNECTED;
                        }
                        else if (status.GetStatus() == (int)ComponentState.STATE_READY)
                        {
                            componentOneActive = true;
                            connectionDetails.direct = ConnectionOption.CONNECTED;
                        }
                    }
                    else if (sourceComponent == 2)
                    {
                        if (status.GetStatus() == (int)ComponentState.STATE_SHUTDOWN)
                        {
                            componentTwoActive = false;
                            connectionDetails.remote = ConnectionOption.DISCONNECTED;
                        }
                        else if (status.GetStatus() == (int)ComponentState.STATE_READY)
                        {
                            componentTwoActive = true;
                            connectionDetails.remote = ConnectionOption.CONNECTED;
                        }
                    }
                    else if (sourceComponent == 3)
                    {
                        if (status.GetStatus() == (int)ComponentState.STATE_SHUTDOWN)
                        {
                            componentThreeActive = false;
                            connectionDetails.ai = ConnectionOption.DISCONNECTED;
                        }
                        else if (status.GetStatus() == (int)ComponentState.STATE_READY)
                        {
                            componentThreeActive = true;
                            connectionDetails.ai = ConnectionOption.CONNECTED;
                        }
                    }
                    _eventAggregator.GetEvent<ConnectionDetailsEvent>().Publish(connectionDetails);

                    // write to the console if there has been a change in isReady activity
                    if (componentOneActive || componentTwoActive || componentThreeActive)
                    {
                        if (!isReady)
                            _eventAggregator.GetEvent<LoggerEvent>().Publish("Component switched to online in BadgerControlDrive");         
                        isReady = true;
                    }
                    else
                    {
                        if (isReady)
                            _eventAggregator.GetEvent<LoggerEvent>().Publish("Component switched to offline in BadgerControlDrive");         
                        isReady = false;
                    }

                    return true;

                default:
                    return false;
            }
        }

        public override bool IsSupported(int commandCode)
        {
            throw new NotImplementedException();
        }

        public void ReleaseControl()
        {
            if (destinationAddress == null)
                return;

            ReleaseControl releaseControl = new ReleaseControl();
            releaseControl.SetDestination(destinationAddress);
            //releaseControl.SetSource(component.JausAddress);
            Transport.SendMessage(releaseControl);
        }

        public void LocateJoystick()
        {
            // if we have the instance and all parties have confrmed, return
            if (JoyStickConfirmed && joystickQueryThread != null)
                return;

            if (joystickQueryThread == null)
            {
                // reset these because we have no joystick
                joystickConfirmedFromStatusView = false;
                joystickConfirmedFromVisualView = false;

                // try to create a joystick
                try
                {
                    joystickQueryThread = new JoystickQueryThread();
                    Thread joystickThread = new Thread(joystickQueryThread.QueryJoystick);
                    joystickThread.Start();
                }
                catch (Exception)
                {
                    // TODO: make this work. this event is activated before the subscriber is attached
                    if (!triedJoystick)
                    {
                        triedJoystick = true;
                        _eventAggregator.GetEvent<LoggerEvent>().Publish("Joystick Not found.");
                    }
                }
            }

            // deliver if we actually produced a joystick
            if (joystickQueryThread != null)
            {
                _eventAggregator.GetEvent<DeliverJoystickEvent>().Publish(joystickQueryThread);
            }
        }
    }
}
