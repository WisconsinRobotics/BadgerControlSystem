using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BadgerJaus.Util;

namespace BadgerControlModule.Utils
{
    public interface RemoteDriverService
    {
        void SendDriveCommand(long xJoystickValue, long yJoystickValue, long zJoystickValue, Component parentComponent);
    }
}
