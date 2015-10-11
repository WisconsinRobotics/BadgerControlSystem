using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using JoystickLibrary;

namespace BadgerControlModule.Services
{
    class ConfirmJoystickEvent : PubSubEvent<int>
    {
    }
}
