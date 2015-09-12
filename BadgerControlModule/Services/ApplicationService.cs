using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;


namespace BadgerControlModule.Services
{
    internal sealed class ApplicationService
    {
        private ApplicationService()
        {
        }

        private static readonly ApplicationService _instance = new ApplicationService();
        internal static ApplicationService Instance
        {
            get
            { 
                return _instance; 
            }
        }

        private IEventAggregator _eventAggregator;
        internal IEventAggregator EventAggregator
        {
            get
            {
                if (_eventAggregator == null)
                    _eventAggregator = new EventAggregator();

                return _eventAggregator;
            }
        }
    }
}
