using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RemoteController
{
    public enum MessageType 
    {
        ConfigurationMessage = 1, 
        ChangeStateMessage = 2,
        InitialMessage = 3
    };

    interface IEncoder
    {
        byte[] Encode(Encoding encoding);
    }

    /// <summary>
    /// Message requests change of state of server listenner. When configuration is enable, a server listens in a verbose mode, else a server listens only <see cref="ControllerEvent"/>.
    /// </summary>
    public class ChangeStateMessage : IEncoder
    {
        public MessageManagerState State { get;}

        public ChangeStateMessage(MessageManagerState state = MessageManagerState.Initial)
        {
            State = state; 
        }

        #region Encoding/Decoding
        
        /// <summary>
        /// Encodes this with pattern |MessageType.ChangeStateMessage|Type|.
        /// </summary>
        public byte[] Encode(Encoding encoding)
        {
            // Header
            byte opCode = (byte)MessageType.ChangeStateMessage;

            // Body
            byte newState = (byte)State;

            return new byte[] { opCode, newState };
        }

        /// <summary>
        /// Tries to decode given data.
        /// </summary>
        public static bool TryDecode(byte[] data, Encoding encoding, out ChangeStateMessage msg)
        {
            if (data == null || data.Length != 2 || data[0] != (byte)MessageType.ChangeStateMessage)
            {
                msg = new ChangeStateMessage();
                return false;
            }

            switch (data[1])
            {
                case (byte)MessageManagerState.Initial:
                    msg = new ChangeStateMessage(MessageManagerState.Initial);
                    return true;
                case (byte)MessageManagerState.Configuration:
                    msg = new ChangeStateMessage(MessageManagerState.Configuration);
                    return true;
                case (byte)MessageManagerState.Controllers:
                    msg = new ChangeStateMessage(MessageManagerState.Controllers);
                    return true;
                default:
                    msg = new ChangeStateMessage();
                    return false;
            }
        }

        #endregion
    }

    public class InitialMessage : IEncoder
    {
        public byte[] Encode(Encoding encoding)
        {
            return new byte[] { (byte)MessageType.InitialMessage };
        }

        public static bool TryDecode(byte[] data, Encoding encoding, out InitialMessage msg)
        {
            msg = new InitialMessage();

            return data != null && data.Length == 1 && data[0] == (byte)MessageType.InitialMessage;
        }
    }

    /// <summary>
    /// Provides information about user configuration of controllers.
    /// </summary>
    public class ConfigurationMessage:IEncoder
    {
        /// <summary>
        /// Contains binding IDs to controllers
        /// </summary>
        public Dictionary<byte, Type> Args { get; }

        public ConfigurationMessage()
        {
            Args = new Dictionary<byte, Type>();
        }

        /// <summary>
        /// Binds ID to type of controller event. If ID is already used, throws exception.
        /// </summary>
        public void AddBinding(byte ID, ControllerEvent args) => AddBinding(ID, args.GetType());

        public void AddBinding(byte ID, Type args)
        {
            if (Args.ContainsKey(ID))
            {
                throw new ArgumentException("This id is already used!", nameof(ID));
            }
            else
            {
                Args.Add(ID, args);
            }
        }

        /// <summary>
        /// Removes binding which is associated with ID.
        /// </summary>
        public void RemoveBinding(byte ID)
        {
            if (Args.ContainsKey(ID))
                Args.Remove(ID);
        }

        #region Encode/Decode

        /// <summary>
        /// Encodes this into pattern |(byte)MessageType.Configuration|0|(byte)ID|(byte)FullName of controller event class.
        /// </summary>
        public byte[] Encode(Encoding encoding)
        {
            MemoryStream builder = new MemoryStream();

            // Header
            builder.WriteByte((byte)MessageType.ConfigurationMessage); // OpCode
            builder.WriteByte(0); // Separator
            builder.WriteByte((byte)Args.Count); // Count of controllers
            builder.WriteByte(0); // Separator

            // Body
            foreach (KeyValuePair<byte, Type> item in Args)
            {
                builder.WriteByte(item.Key);
                builder.WriteBytes(encoding.GetBytes(item.Value.FullName));
                builder.WriteByte(0); // Separator
            }

            return builder.ToArray();
        }

        /// <summary>
        /// Decodes s message and finds Type instances by their name in the data.
        /// </summary>
        public static bool TryDecode(byte[] data, Encoding encoding, out ConfigurationMessage msg)
        {
            msg = new ConfigurationMessage();

            if (data == null || data.Length < 3)
                return false;

            int countOfControllers = data[2];
            int skip = countOfControllers == 0 ? 1 : 2;
            byte[][] records = data.Split(0, skip + countOfControllers);

            
            // Go through all records and find their types. 
            for (int i = skip; i < skip + countOfControllers; i++)
            {
                byte id = records[i][0];
                string nameOfClass = encoding.GetString(records[i], 1, records[i].Length - 1);

                try
                {
                    var classType = Type.GetType(nameOfClass);

                    if (classType.IsSubclassOf(typeof(ControllerEvent)))
                        msg.AddBinding(id, classType);
                    else
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }    

            return true;
        }
        #endregion
    }
}
