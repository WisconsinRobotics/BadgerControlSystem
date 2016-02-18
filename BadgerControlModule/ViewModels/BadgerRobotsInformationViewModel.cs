using System.Collections.Generic;
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
        Component selectedComponent;

        public BadgerRobotsInformationViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            badgerControlSubsystem = BadgerControlSubsystem.GetInstance();
            Refresh = new DelegateCommand<object>(OnRefresh);
            Activate = new DelegateCommand<object>(OnActivate);
            Deactivate = new DelegateCommand<object>(OnDeactivate);
            RequestControl = new DelegateCommand<object>(OnRequestControl);
            ReleaseControl = new DelegateCommand<object>(OnReleaseControl);
        }

        public IEnumerable<Subsystem> SubsystemList
        {
            get { return badgerControlSubsystem.DiscoveredSubsystems; }
        }

        public Subsystem SelectedSubsystem
        {
            get { return selectedSubsystem; }
            set { selectedSubsystem = value; }
        }

        public IEnumerable<Node> CurrentNodes
        {
            get { return selectedSubsystem.NodeList; }
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
            JausAddress targetAddress = GenerateCurrentAddress();

            if (targetAddress == null)
                return;

            QueryStatus queryStatus = new QueryStatus();
            queryStatus.SetSource(badgerControlSubsystem.LocalAddress);
            queryStatus.SetDestination(targetAddress);

            Transport.SendMessage(queryStatus);
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

        private void SelectedNodeComponentChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
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
    }
}
