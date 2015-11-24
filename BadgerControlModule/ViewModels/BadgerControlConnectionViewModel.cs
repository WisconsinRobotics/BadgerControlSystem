using System;
using System.Windows.Input;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Events;
using BadgerControlModule.Services;

namespace BadgerControlModule.ViewModels
{
    class BadgerControlConnectionViewModel : BindableBase
    {
        private string ip;
        private string subsystem;
        protected readonly IEventAggregator _eventAggregator;

        public BadgerControlConnectionViewModel(IEventAggregator eventAggregator)
        {
            this.Abort = new DelegateCommand<object>(OnAbort);
            this.Connect = new DelegateCommand<object>(OnConnect);

            this._eventAggregator = eventAggregator;
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
            int subsystemId = 0;

            // attempt to parse the subsystem id
            // allow the IP address string to continue so that it can get validated in the BadgerControlSubsystem
            if (subsystem == null || !Int32.TryParse(subsystem, out subsystemId) || subsystemId <= 0)
                _eventAggregator.GetEvent<LoggerEvent>().Publish("Please enter a valid subsystem ID. IDs must be greater than 0.");
            else
                _eventAggregator.GetEvent<UpdateSubsystemIdEvent>().Publish(subsystemId);

            _eventAggregator.GetEvent<UpdateIPEvent>().Publish(ip);
        }

        // changing the value of the IP textbox will trigger this 
        public string IP
        {
            get { return ip; }
            set { ip = value; }
        }

        // changing the value of the Subsystem textbox will trigger this 
        public string Subsystem
        {
            get { return subsystem; }
            set { subsystem = value; }
        }
    }
}
