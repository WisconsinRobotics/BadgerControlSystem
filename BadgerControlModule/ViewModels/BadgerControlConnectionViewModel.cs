using System;
using System.Windows.Input;
using System.Net;

using Prism.Mvvm;
using Prism.Commands;
using Prism.Events;

using BadgerJaus.Util;

using BadgerControlModule.Models;
using BadgerControlModule.Services;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlConnectionViewModel : BindableBase
    {
        private string ip;
        private int port;
        private int subsystemID;
        protected readonly IEventAggregator _eventAggregator;
        private BadgerControlSubsystem badgerControlSubsystem;

        public BadgerControlConnectionViewModel(IEventAggregator eventAggregator)
        {
            this.Abort = new DelegateCommand<object>(OnAbort);
            this.Connect = new DelegateCommand<object>(OnConnect);

            this._eventAggregator = eventAggregator;
            badgerControlSubsystem = BadgerControlSubsystem.GetInstance();
            port = Subsystem.JAUS_PORT;
        }

        public ICommand Abort
        {
            get;
            private set;
        }

        public ICommand Connect
        {
            get;
            private set;
        }

        private void OnAbort(object arg)
        {
            // clicking the Abort button will run this funciton
        }

        private void OnConnect(object arg)
        {
            IPEndPoint remoteEndpoint;
            IPAddress remoteAddress;
            // attempt to parse the subsystem id
            // allow the IP address string to continue so that it can get validated in the BadgerControlSubsystem
            if (subsystemID <= 0)
                _eventAggregator.GetEvent<LoggerEvent>().Publish("Please enter a valid subsystem ID. IDs must be greater than 0.");
            else
                _eventAggregator.GetEvent<UpdateSubsystemIdEvent>().Publish(subsystemID);

            //_eventAggregator.GetEvent<UpdateIPEvent>().Publish(ip);
            if(!IPAddress.TryParse(ip, out remoteAddress))
            {
                // error case
                return;
            }
            remoteEndpoint = new IPEndPoint(remoteAddress, port);
            badgerControlSubsystem.Connect(subsystemID, remoteEndpoint);
        }

        // changing the value of the IP textbox will trigger this 
        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

        // changing the value of the Subsystem textbox will trigger this 
        public int SubsystemID
        {
            get { return subsystemID; }
            set { subsystemID = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }
    }
}
