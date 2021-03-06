﻿/*
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Prism.Mvvm;
using Prism.Commands;
using Prism.Events;

using BadgerJaus.Messages.Control;
using BadgerJaus.Messages.Management;
using BadgerJaus.Util;
using BadgerJaus.Services.Core;

using BadgerControlModule.Models;

namespace BadgerControlModule.ViewModels
{
    class BadgerRobotsInformationViewModel : BindableBase
    {
        protected readonly IEventAggregator _eventAggregator;
        BadgerControlSubsystem badgerControlSubsystem;
        Subsystem selectedSubsystem;
        Node selectedNode;
        BadgerJaus.Util.Component selectedComponent;
        ObservableCollection<Subsystem> discoveredSubsystems;
        List<Node> currentNodes;

        public BadgerRobotsInformationViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            badgerControlSubsystem = BadgerControlSubsystem.GetInstance();
            Refresh = new DelegateCommand<object>(OnRefresh);
            Activate = new DelegateCommand<object>(OnActivate);
            Deactivate = new DelegateCommand<object>(OnDeactivate);
            RequestControl = new DelegateCommand<object>(OnRequestControl);
            ReleaseControl = new DelegateCommand<object>(OnReleaseControl);
            discoveredSubsystems = badgerControlSubsystem.DiscoveredSubsystems;
            discoveredSubsystems.CollectionChanged += DiscoveredSubsystemsUpdated;
            currentNodes = new List<Node>();
        }

        private void DiscoveredSubsystemsUpdated(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new System.Action(() =>
            {
                SubsystemList = badgerControlSubsystem.DiscoveredSubsystems;
            }
            ));
        }

        public ObservableCollection<Subsystem> SubsystemList
        {
            get { return discoveredSubsystems; }
            private set
            {
                discoveredSubsystems = value;
            }
        }

        public Subsystem SelectedSubsystem
        {
            get { return selectedSubsystem; }
            set { selectedSubsystem = value; }
        }

        public List<Node> CurrentNodes
        {
            get { return currentNodes; }
            private set { currentNodes = value; }
        }

        public ICommand Refresh
        {
            get;
            private set;
        }

        public ICommand Activate
        {
            get;
            private set;
        }

        public ICommand Deactivate
        {
            get;
            private set;
        }

        public ICommand RequestControl
        {
            get;
            private set;
        }

        public ICommand ReleaseControl
        {
            get;
            private set;
        }

        public void OnRefresh(object arg)
        {
            //CurrentNodes = selectedSubsystem.NodeList;
            currentNodes.Clear();
            foreach(Node node in selectedSubsystem.NodeList)
            {
                currentNodes.Add(node);
            }
            //SubsystemList = badgerControlSubsystem.DiscoveredSubsystems;
            /*
            JausAddress targetAddress = GenerateCurrentAddress();

            if (targetAddress == null)
                return;

            QueryStatus queryStatus = new QueryStatus();
            queryStatus.SetSource(badgerControlSubsystem.LocalAddress);
            queryStatus.SetDestination(targetAddress);

            Transport.SendMessage(queryStatus);
            */
        }
        
        public void OnRequestControl(object arg)
        {
            JausAddress targetAddress = GenerateCurrentAddress();

            if (targetAddress == null)
                return;

            RequestControl requestControl = new RequestControl();
            requestControl.SetAuthorityCode(BadgerControlSubsystem.AUTHORITY_LEVEL);
            requestControl.SetSource(badgerControlSubsystem.LocalAddress);
            requestControl.SetDestination(targetAddress);

            Transport.SendMessage(requestControl);
        }

        public void OnReleaseControl(object arg)
        {
            JausAddress targetAddress = GenerateCurrentAddress();

            if (targetAddress == null)
                return;

            ReleaseControl releaseControl = new ReleaseControl();
            releaseControl.SetSource(badgerControlSubsystem.LocalAddress);
            releaseControl.SetDestination(targetAddress);

            Transport.SendMessage(releaseControl);
        }

        public void OnActivate(object arg)
        {
            JausAddress targetAddress = GenerateCurrentAddress();

            if (targetAddress == null)
                return;

            Resume resume = new Resume();
            resume.SetSource(badgerControlSubsystem.LocalAddress);
            resume.SetDestination(targetAddress);

            Transport.SendMessage(resume);
        }

        public void OnDeactivate(object arg)
        {
            JausAddress targetAddress = GenerateCurrentAddress();

            if (targetAddress == null)
                return;

            Shutdown shutdown = new Shutdown();
            shutdown.SetSource(badgerControlSubsystem.LocalAddress);
            shutdown.SetDestination(targetAddress);

            Transport.SendMessage(shutdown);
        }

        private JausAddress GenerateCurrentAddress()
        {
            JausAddress targetAddress;

            if (selectedSubsystem == null)
                return null;

            targetAddress = new JausAddress(selectedSubsystem.SubsystemID, 255, 255);
            if (selectedNode != null)
            {
                targetAddress.setNode(selectedNode.NodeID);
            }
            if (selectedComponent != null)
            {
                targetAddress.setComponent(selectedComponent.ComponentID);
            }

            return targetAddress;
        }

        public void SelectedNodeComponentChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            selectedComponent = e.NewValue as Component;
            if(selectedComponent != null)
            {
                selectedNode = selectedComponent.GetNode();
            }
            else
            {
                selectedNode = e.NewValue as Node;
            }
        }

        public void SubsystemSelected(object sender, SelectionChangedEventArgs e)
        {
            currentNodes.Clear();
            foreach (Node node in selectedSubsystem.NodeList)
            {
                currentNodes.Add(node);
            }
        }
    }
}
