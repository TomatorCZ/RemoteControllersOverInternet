using RemoteController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Shared
{
    public class ControllersEventManager
    {
        Encoding _encoding; // Send by config msg
        public Dictionary<byte, Type> Args { get; }

        public ControllersEventManager(ConfigurationMessage msg)
        {
            Args = msg.Args;
            _encoding = Encoding.UTF8;
        }

        public ControllerEvent GetEvent(byte id, byte[] data)
        {
            if (!Args.ContainsKey(id))
                throw new InvalidDataException();

            ControllerEvent @event = null;

            var methodInfo = Args[id].GetMethod("TryDecode", BindingFlags.Public | BindingFlags.Static);
            if (methodInfo == null)
                throw new InvalidCastException();
            var parameters = new object[3] { data, _encoding, @event };

            bool result = (bool)methodInfo.Invoke(null, parameters);
            if (result == false)
                throw new NotImplementedException();

            return (ControllerEvent)parameters[2];
        }
    }
}
