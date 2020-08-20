using RemoteControllers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class ControllersEventManager
    {
        public Dictionary<byte, Type> Args { get; }

        public ControllersEventManager(ConfigurationMessage msg)
        {
            Args = msg.Args;
        }
    }
}
