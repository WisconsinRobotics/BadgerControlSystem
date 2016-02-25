using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BadgerJaus.Messages;
using BadgerJaus.Messages.Control;
using BadgerJaus.Services;
using BadgerJaus.Util;

using BadgerControlModule.Models;

namespace BadgerControlModule.Services
{
    class BadgerControlService : BaseService
    {
        BadgerControlSubsystem badgerControlSubsystem;
        ConfirmControl confirmControlMessage;
        RejectControl rejectControlMessage;

        public BadgerControlService(BadgerControlSubsystem badgerControlSubsystem)
        {
            this.badgerControlSubsystem = badgerControlSubsystem;

            confirmControlMessage = new ConfirmControl();
            rejectControlMessage = new RejectControl();
        }

        public override bool ImplementsAndHandledMessage(Message message, Component component)
        {
            Subsystem remoteSubsystem;
            Node remoteNode;
            Component remoteComponent;

            switch (message.GetCommandCode())
            {
                case JausCommandCode.CONFIRM_CONTROL:
                    confirmControlMessage.SetFromJausMessage(message);
                    if(confirmControlMessage.GetResponseCode() == ConfirmControl.CONTROL_ACCEPTED)
                    {
                        if (!badgerControlSubsystem.DiscoveredSubsystemsDictionary.TryGetValue(confirmControlMessage.GetSource().SubsystemID, out remoteSubsystem))
                            return true;

                        if (!remoteSubsystem.NodeDictionary.TryGetValue(confirmControlMessage.GetSource().getNode(), out remoteNode))
                            return true;

                        if (!remoteNode.ComponentDictionary.TryGetValue(confirmControlMessage.GetSource().getComponent(), out remoteComponent))
                            return true;

                        remoteComponent.ControlState = ControlState.STATE_CONTROLLED;
                    }

                    return true;

                case JausCommandCode.REJECT_CONTROL:
                    rejectControlMessage.SetFromJausMessage(message);
                    if(rejectControlMessage.GetResponseCode() == RejectControl.CONTROL_RELEASED)
                    {
                        if (!badgerControlSubsystem.DiscoveredSubsystemsDictionary.TryGetValue(confirmControlMessage.GetSource().SubsystemID, out remoteSubsystem))
                            return true;

                        if (!remoteSubsystem.NodeDictionary.TryGetValue(confirmControlMessage.GetSource().getNode(), out remoteNode))
                            return true;

                        if (!remoteNode.ComponentDictionary.TryGetValue(confirmControlMessage.GetSource().getComponent(), out remoteComponent))
                            return true;

                        remoteComponent.ControlState = ControlState.STATE_NOT_CONTROLLED;
                    }
                    return true;

                default: return false;
            }
        }

        public override bool IsSupported(int commandCode)
        {
            switch (commandCode)
            {
                case JausCommandCode.CONFIRM_CONTROL:
                case JausCommandCode.REJECT_CONTROL:
                    return true;
                default: return false;
            }
        }
    }
}
