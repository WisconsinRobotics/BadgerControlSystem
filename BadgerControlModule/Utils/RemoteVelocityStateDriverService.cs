using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BadgerJaus.Messages.VelocityStateDriver;
using BadgerJaus.Util;

namespace BadgerControlModule.Utils
{
    class RemoteVelocityStateDriverService : RemoteDriverService
    {
        public void SendDriveCommand(long xJoystickValue, long yJoystickValue, long zJoystickValue, Component parentComponent)
        {
            throw new NotImplementedException();
        }
    }
}
