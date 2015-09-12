using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;

using BadgerJaus.Util;
using BadgerJaus.Messages.Control;
using BadgerJaus.Services.Core;
using BadgerJaus.Services;

using BadgerControlModule.Services;
using BadgerControlModule.Utils;

using JoystickLibrary;

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

            // start BadgerControlDrive
            guiService = new BadgerControlDrive();
            guiService.Enabled = true;

            // start Management
            Management controlManagement = new Management();
            controlManagement.CurrentState = COMPONENT_STATE.STATE_READY;

            guiNode = new Node(GUI_ROBOT_NODE_ID);
            guiComponent = new Component(GUI_COMPONENT_ID);

            // create subsystem-node-component hierarchy
            this.AddNode(guiNode);
            guiNode.AddComponent(guiComponent);
            guiComponent.AddService(controlManagement);
            guiComponent.AddService(guiService);

            // get transport instance, add subsystem
            Transport transportService = Transport.GetTransportService();
            transportService.AddSubsystem(this);
            Thread transportThread = new Thread(transportService.run);
            transportThread.Start();

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
            String jausAddressHexString = guiService.CurrentDestinationAddress.toHexString();

            // if there was some sort of error with the JAUS address
            if (jausAddressHexString == null)
            {
                guiService.CurrentDestinationAddress = null;
                _eventAggregator.GetEvent<LoggerEvent>().Publish("Error: null JausAddress string");
            }

            // else, we have all the info we need so attempt to connect
            else
            {
                JausAddressPort port = new JausAddressPort(ip, Subsystem.JAUS_PORT);
                Transport transportService = Transport.GetTransportService();
                transportService.AddRemoteAddress(jausAddressHexString, port);
                _eventAggregator.GetEvent<LoggerEvent>().Publish("Attempting to connect...");
            }
        }
    }
}

