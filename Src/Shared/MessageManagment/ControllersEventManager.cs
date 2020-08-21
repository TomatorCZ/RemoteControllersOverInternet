using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace RemoteController
{
    public class ControllersEventManager
    {
        Encoding _encoding;
        public Dictionary<byte, Type> Args { get; }

        public ControllersEventManager(ConfigurationMessage msg, Encoding encoding)
        {
            Args = msg.Args;
            _encoding = encoding;
        }

        public ControllerEvent GetEvent(byte id, byte[] data)
        {
            if (!Args.ContainsKey(id))
                throw new InvalidDataException("Invalid controller id.");

            ControllerEvent @event = null;

            var methodInfo = Args[id].GetMethod("TryDecode", BindingFlags.Public | BindingFlags.Static);
            if (methodInfo == null)
                throw new InvalidCastException("The type must have TryDecode function.");

            var parameters = new object[3] { data, _encoding, @event };
            bool result = (bool)methodInfo.Invoke(null, parameters);

            if (result == false)
                throw new InvalidDataException("Invalid format of controller event message.");

            return (ControllerEvent)parameters[2];
        }
    }
}
