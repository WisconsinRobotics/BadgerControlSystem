/*
 * Copyright (c) 2015, Wisconsin Robotics
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 * * Redistributions of source code must retain the above copyright
 *   notice, this list of conditions and the following disclaimer.
 * * Redistributions in binary form must reproduce the above copyright
 *   notice, this list of conditions and the following disclaimer in the
 *   documentation and/or other materials provided with the distribution.
 * * Neither the name of Wisconsin Robotics nor the
 *   names of its contributors may be used to endorse or promote products
 *   derived from this software without specific prior written permission.
 *   
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL WISCONSIN ROBOTICS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

using Prism.Events;

using BadgerJaus.Messages.Discovery;
using BadgerJaus.Util;
using BadgerJaus.Services;
using BadgerJaus.Services.Core;
using BadgerJaus.Services.Mobility;

using BadgerControlModule.Services;
using BadgerControlModule.Utils;

namespace BadgerControlModule.Models
{
    public class BadgerControlSubsystem : JausSubsystem
    {
        // Local constants for the JAUS GUI
        private const int GUI_SUBSYSTEM_ID = 0;
        private const int GUI_NODE_ID = 1;
        private const int GUI_COMPONENT_ID = 1;

        public const int AUTHORITY_LEVEL = 254;

        // Remote JAUS constants
        private const int REMOTE_NODE_ID = 1;

        // Remote JAUS variables
        private int currentRemoteSubsystemID = 0;
        private int currentRemoteComponent = 0;

        /*
        // Local JAUS objects for the GUI 
        private Node guiNode;
        private Component guiComponent;
        private BadgerControlDrive guiService;
        */

        Node badgerNode;
        Component badgerComponent;
        BadgerControlService badgerControlService;
        BadgerDriverService badgerDriveService;

        JausAddress localJausAddress;

        RemoteVelocityStateDriverService remoteVelocityStateDriverService;
        RemotePrimitiveDriverService remotePrimitiveDriverService;

        Dictionary<long, DriveModes> discoveredDriveModes;
        ObservableCollection<DriveModes> observableDriveModes;
        DriveModes currentDriveMode;

        // Event handler
        protected readonly IEventAggregator _eventAggregator;

        protected static BadgerControlSubsystem badgerControlSubsystemInstance;

        public static BadgerControlSubsystem CreateInstance()
        {
            if (badgerControlSubsystemInstance == null)
                badgerControlSubsystemInstance = new BadgerControlSubsystem();

            return badgerControlSubsystemInstance;
        }

        public static BadgerControlSubsystem GetInstance()
        {
            return badgerControlSubsystemInstance;
        }

        private BadgerControlSubsystem() : base(GUI_SUBSYSTEM_ID, "BadgerControlSubsystem")
        {
            // setup event listeners
            #region EVENT_LISTENERS
            _eventAggregator = ApplicationService.Instance.EventAggregator;
            _eventAggregator.GetEvent<UpdateIPEvent>().Subscribe((ip) =>
            {
                //CurrentRemoteIP = ip;
            });
            _eventAggregator.GetEvent<UpdateSubsystemIdEvent>().Subscribe((subsystem) =>
            {
                CurrentRemoteSubsystemID = subsystem;
            });
            _eventAggregator.GetEvent<UpdateDriveModeEvent>().Subscribe((drive) =>
            {
                //CurrentDriveComponent = drive;
            });
            #endregion

            localJausAddress = new JausAddress(GUI_SUBSYSTEM_ID, GUI_NODE_ID, GUI_COMPONENT_ID);
            badgerControlSubsystemInstance = this;
            remoteVelocityStateDriverService = new RemoteVelocityStateDriverService(this);
            remotePrimitiveDriverService = new RemotePrimitiveDriverService(this);
            discoveredDriveModes = new Dictionary<long, DriveModes>();
            observableDriveModes = new ObservableCollection<DriveModes>();
        }

        public void InitializeComponents()
        {
            // start BadgerControlDrive
            //guiService = new BadgerControlDrive();
            //guiService.Enabled = true;

            //guiNode = new Node(GUI_NODE_ID);
            //guiComponent = new Component(GUI_COMPONENT_ID);

            // create subsystem-node-component hierarchy
            //this.AddNode(guiNode);
            //guiNode.AddComponent(guiComponent);
            //guiComponent.AddService(guiService);
            //guiComponent.ComponentState = ComponentState.STATE_READY;


            badgerNode = new Node(GUI_NODE_ID);
            badgerComponent = new Component(GUI_COMPONENT_ID);
            badgerControlService = new BadgerControlService(this);
            badgerDriveService = new BadgerDriverService(this);

            AddNode(badgerNode);
            badgerNode.AddComponent(badgerComponent);
            badgerComponent.AddService(badgerControlService);
            badgerComponent.AddService(badgerDriveService);
            badgerComponent.ComponentState = ComponentState.STATE_READY;

            discoveryService.ObservableDiscoveredSubsystems.CollectionChanged += SubsystemListModified;

            // start execute loop
            InitializeTimer();
        }

        private void SubsystemListModified(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(Subsystem subsystem in e.NewItems)
            {
                subsystem.ObservableNodes.CollectionChanged += NodesUpdated;
            }
        }

        private void NodesUpdated(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(Node node in e.NewItems)
            {
                node.ObservableComponents.CollectionChanged += ComponentsUpdated;
            }
        }

        private void ComponentsUpdated(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(Component component in e.NewItems)
            {
                component.Services.CollectionChanged += ServicesUpdated;
            }
        }

        private void ServicesUpdated(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateDriveModes();
        }

        public JausAddress LocalAddress
        {
            get { return localJausAddress; }
        }

        /*
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
        */

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

        public void Connect(int remoteSubsystemID, IPEndPoint remoteAddress)
        {
            Subsystem remoteSubsystem = new Subsystem(remoteSubsystemID, remoteAddress);
            remoteSubsystem.Identification = "UnknownSubsystem";
            discoveryService.AddRemoteSubsystem(remoteSubsystemID, remoteSubsystem);
            JausAddress remoteJausAddress = new JausAddress(remoteSubsystemID, 255, 255);
            QueryIdentification queryIdentification = new QueryIdentification();
            queryIdentification.SetDestination(remoteJausAddress);
            queryIdentification.SetSource(localJausAddress);
            Transport.SendMessage(queryIdentification);

            _eventAggregator.GetEvent<LoggerEvent>().Publish("Attempting to connect...");
        }

        public ObservableCollection<Subsystem> DiscoveredSubsystems
        {
            get { return discoveryService.ObservableDiscoveredSubsystems; }
        }

        public ConcurrentDictionary<long, Subsystem> DiscoveredSubsystemsDictionary
        {
            get { return discoveryService.DiscoveredSubsystems; }
        }

        public ObservableCollection<DriveModes> ObservableDriveModes
        {
            get { return observableDriveModes; }
        }

        public void UpdateDriveModes()
        {
            DriveModes driveMode;
            foreach (Subsystem subsystem in DiscoveredSubsystems)
            {
                foreach(Node node in subsystem.NodeList)
                {
                    foreach(Component component in node.ComponentList)
                    {
                        foreach(BaseService service in component.Services)
                        {
                            switch(service.ServiceName)
                            {
                                case LocalVectorDriver.SERVICE_NAME:
                                    driveMode = new DriveModes(this, component, remoteVelocityStateDriverService, LocalVectorDriver.SERVICE_NAME);
                                    discoveredDriveModes.Add(component.JausAddress.Value, driveMode);
                                    observableDriveModes.Add(driveMode);
                                    break;
                                case VelocityStateDriver.SERVICE_NAME:
                                    if (discoveredDriveModes.ContainsKey(component.JausAddress.Value))
                                        break;
                                    driveMode = new DriveModes(this, component, remoteVelocityStateDriverService, VelocityStateDriver.SERVICE_NAME);
                                    discoveredDriveModes.Add(component.JausAddress.Value, driveMode);
                                    observableDriveModes.Add(driveMode);
                                    break;
                                case PrimitiveDriver.SERVICE_NAME:
                                    if (discoveredDriveModes.ContainsKey(component.JausAddress.Value))
                                        break;
                                    driveMode = new DriveModes(this, component, remotePrimitiveDriverService, PrimitiveDriver.SERVICE_NAME);
                                    discoveredDriveModes.Add(component.JausAddress.Value, driveMode);
                                    observableDriveModes.Add(driveMode);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public DriveModes CurrentDriveMode
        {
            get { return currentDriveMode; }
            set { currentDriveMode = value; }
        }
    }
}
