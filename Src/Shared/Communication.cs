using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace Shared
{
    /// <summary>
    /// Represents state of connection.
    /// </summary>
    public enum MessageType 
    { 
        Configuration = 1, 
        Controllers = 2,
        ChangeStateMessage = 3,
        Default = 4
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
        public MessageType Type { get;}

        public ChangeStateMessage(MessageType type = MessageType.Default)
        {
            Type = type; 
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
            byte newState = (byte)Type;

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
                case 1:
                    msg = new ChangeStateMessage(MessageType.Configuration);
                    return true;
                case 2:
                    msg = new ChangeStateMessage(MessageType.Controllers);
                    return true;
                default:
                    msg = new ChangeStateMessage();
                    return false;
            }
        }

        #endregion
    }

    /// <summary>
    /// Provides information about user configuration of controllers.
    /// </summary>
    public class ConfigurationMessage:IEncoder
    {
        /// <summary>
        /// Contains binding IDs to controllers
        /// </summary>
        Dictionary<byte, Type> Args;

        public ConfigurationMessage()
        {
            Args = new Dictionary<byte, Type>();
        }

        /// <summary>
        /// Binds ID to type of controller event. If ID is already used, throws exception.
        /// </summary>
        public void AddBinding(byte ID, ControllerEvent args) => AddBinding(ID, args.GetType());

        private void AddBinding(byte ID, Type args)
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
            builder.WriteByte((byte)MessageType.Configuration); // OpCode
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
        public static bool Decode(byte[] data, Encoding encoding, out ConfigurationMessage msg)
        {
            msg = new ConfigurationMessage();

            if (data == null || data.Length < 3)
                return false;

            int countOfControllers = data[2];
            byte[][] records = data.Split(0, 2 + countOfControllers);
            
            // Go through all records and find their types. 
            foreach (var item in records)
            {
                byte id = item[0];
                string nameOfClass = encoding.GetString(item, 1, item.Length - 1);

                try
                {
                    var classType = Type.GetType(nameOfClass);

                    if (classType.IsSubclassOf(typeof(ControllerEvent)))
                        msg.AddBinding(id, classType);
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    return false;
                }           
            }

            return true;
        }
        #endregion
    }
}
