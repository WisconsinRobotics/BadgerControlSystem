using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;

namespace BadgerControlModule.Services
{
    public struct ConnectionDetails
    {
        public ConnectionOption direct;
        public ConnectionOption remote;
        public ConnectionOption ai;
    }

    public enum ConnectionOption : int
    {
        DISCONNECTED = 1,
        REQUESTING_CONTROL = 2,
        AWAITING_STATUS = 3,
        CONNECTED = 4
    }

    class ConnectionDetailsEvent : PubSubEvent<ConnectionDetails>
    {
    }
}
