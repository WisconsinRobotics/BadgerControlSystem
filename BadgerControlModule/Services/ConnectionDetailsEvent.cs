
using Prism.Events;

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
