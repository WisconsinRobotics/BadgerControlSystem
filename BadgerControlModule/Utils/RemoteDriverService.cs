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
        void SendWrenchCommandQuasi(long primaryXJoystickValue, long primaryYJoystickValue, long primaryZJoystickValue, long secondaryXJoystickValue, long secondaryYJoystickValue, long secondaryZJoystickValue, Component parentComponent);
        void SendWrenchCommandRelative(long turntable, long shoulder, long elbow, long wrist, long rotationOfClaw, long claw, Component parentComponent);
    }
}
