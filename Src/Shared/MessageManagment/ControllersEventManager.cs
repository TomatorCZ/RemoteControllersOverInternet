using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace RemoteController
{

    /// <summary>
    /// Takes care about handling and encoding, decoding events.
    /// </summary>
    public class ControllersEventManager
    {
        Encoding _encoding;
        public Dictionary<byte, Type> Args { get; }

        /// <summary>
        /// The constructor. Binding between ids and events are obtained from <see cref="ConfigurationMessage"/>.
        /// </summary>
        public ControllersEventManager(ConfigurationMessage msg, Encoding encoding)
        {
            Args = msg.Args;
            _encoding = encoding;
        }

        /// <summary>
        /// Decodes bytes to an event due to id and type class which is obtained from <see cref="ConfigurationMessage">.
        /// </summary>
        /// <param name="id">The first byte of received message.</param>
        /// <param name="data">The data.</param>
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
