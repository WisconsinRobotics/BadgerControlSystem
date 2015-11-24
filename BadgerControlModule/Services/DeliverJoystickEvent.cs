using Prism.Events;
using JoystickLibrary;

namespace BadgerControlModule.Services
{
    class DeliverJoystickEvent : PubSubEvent<JoystickQueryThread>
    {
    }
}
