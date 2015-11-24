
using System.Net;

using Prism.Events;

using BadgerJaus.Util;
using BadgerJaus.Services.Core;

using BadgerControlModule.Services;

namespace BadgerControlModule.Models
{
    public class BadgerControlSubsystem : Subsystem
    {
        // Local constants for the JAUS GUI
        private const int GUI_SUBSYSTEM_ID = 0;
        private const int GUI_ROBOT_NODE_ID = 1;
        private const int GUI_COMPONENT_ID = 1;

        // Remote JAUS constants
        private const int REMOTE_NODE_ID = 1;

        // Remote JAUS variables
        private string currentIP;
        private int currentRemoteSubsystemID = 0;
        private int currentRemoteComponent = 0;

        // Local JAUS objects for the GUI 
        private Node guiNode;
        private Component guiComponent;
        private BadgerControlDrive guiService;

        // Event handler
        protected readonly IEventAggregator _eventAggregator;

        public BadgerControlSubsystem() : base(GUI_SUBSYSTEM_ID)
        {
            // setup event listeners
            #region EVENT_LISTENERS
            _eventAggregator = ApplicationService.Instance.EventAggregator;
            _eventAggregator.GetEvent<UpdateIPEvent>().Subscribe((ip) =>
            {
                CurrentRemoteIP = ip;
            });
            _eventAggregator.GetEvent<UpdateSubsystemIdEvent>().Subscribe((subsystem) =>
            {
                CurrentRemoteSubsystemID = subsystem;
            });
            _eventAggregator.GetEvent<UpdateDriveModeEvent>().Subscribe((drive) =>
            {
                CurrentDriveComponent = drive;
            });
            #endregion
        }

        public void InitializeComponents()
        {
            // start BadgerControlDrive
            guiService = new BadgerControlDrive();
            guiService.Enabled = true;

            guiNode = new Node(GUI_ROBOT_NODE_ID);
            guiComponent = new Component(GUI_COMPONENT_ID);

            // create subsystem-node-component hierarchy
            this.AddNode(guiNode);
            guiNode.AddComponent(guiComponent);
            guiComponent.AddService(guiService);
            guiComponent.ComponentState = ComponentState.STATE_READY;
            /*
            UdpClient udpSocket = new UdpClient(Subsystem.JAUS_PORT);
            ConcurrentDictionary<long, IPEndPoint> jausAddrMap = new ConcurrentDictionary<long, IPEndPoint>();
            Transport transportService = Transport.CreateTransportInstance(udpSocket, jausAddrMap);
            */
            // start execute loop
            InitializeTimer();
        }

        public int CurrentDriveComponent
        {
            get { return currentRemoteComponent; }
            set 
            {
                if (value != currentRemoteComponent)
                {
                    guiService.ResetConnection();
                    currentRemoteComponent = value; 
                }
            }
        }

        public string CurrentRemoteIP
        {
            get { return currentIP; }
            set 
            {
                IPAddress ip = null;      

                if (IPAddress.TryParse(value, out ip) == false)
                {
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Please fill in a valid IP Address.");
                }
                else
                {
                    if (value != currentIP)
                    {
                        currentIP = value;
                        _eventAggregator.GetEvent<LoggerEvent>().Publish("Remote IP Address has been updated.");
                    }
                    if (currentRemoteSubsystemID > 0 && currentRemoteComponent > 0)
                    {
                        Connect(ip);
                    }
                    else if (currentRemoteComponent <= 0)
                    {
                         _eventAggregator.GetEvent<LoggerEvent>().Publish("Please set a remote drive component.");
                    }
                }
            }
        }

        public int CurrentRemoteSubsystemID
        {
            get { return currentRemoteSubsystemID; }
            set 
            {
                if (value > 0)
                {
                    if (value != currentRemoteSubsystemID)
                    {
                        currentRemoteSubsystemID = value;
                        _eventAggregator.GetEvent<LoggerEvent>().Publish("Remote Subsystem ID has been updated.");
                    }
                }
                else
                {
                    _eventAggregator.GetEvent<LoggerEvent>().Publish("Please fill in a valid Subsystem ID.");
                }
            }
        }

        private void Connect(IPAddress ip)
        {
            // add a new destination address - this will begin the main body of the excecute loop in the main service
            guiService.CurrentDestinationAddress = new JausAddress(currentRemoteSubsystemID, REMOTE_NODE_ID, (int)currentRemoteComponent);
            long jausAddressValue = guiService.CurrentDestinationAddress.Value;

            // if there was some sort of error with the JAUS address
            if (guiService.CurrentDestinationAddress == null)
            {
                guiService.CurrentDestinationAddress = null;
                _eventAggregator.GetEvent<LoggerEvent>().Publish("Error: null JausAddress string");
            }

            // else, we have all the info we need so attempt to connect
            else
            {
                IPEndPoint ipEndpoint = new IPEndPoint(ip, Subsystem.JAUS_PORT);
                Transport transportService = Transport.GetTransportService();
                transportService.AddRemoteAddress(jausAddressValue, ipEndpoint);
                _eventAggregator.GetEvent<LoggerEvent>().Publish("Attempting to connect...");
            }
        }
    }
}

