using RemoteController;
using System;
using System.IO;
using System.Text;

namespace Client
{
    public class NotificatorControllerEvent : ControllerEvent
    {
        public string Name { get; }
        public string Button { get; }

        public NotificatorControllerEvent(string name, string button, byte senderID) : base(senderID)
        {
            Name = name;
            Button = button;
        }

        #region Encode/Decode

        public override byte[] Encode(Encoding encoding)
        {
            byte[] name_encoded = encoding.GetBytes(Name);
            byte[] button_encoded = encoding.GetBytes(Button);

            if (name_encoded == null || button_encoded == null)
                throw new InvalidDataException();

            byte[] result = new byte[name_encoded.Length + button_encoded.Length + 3];
            result[0] = SenderID;
            result[1] = 0;
            Buffer.BlockCopy(name_encoded, 0, result, 2, name_encoded.Length);
            result[name_encoded.Length + 2] = 0;
            Buffer.BlockCopy(button_encoded, 0, result, name_encoded.Length + 3, button_encoded.Length);

            return result;
        }

        public static new bool TryDecode(byte[] data, Encoding encoding, out ControllerEvent @event)
        {
            if (data == null || data.Length == 0)
            {
                @event = new NotificatorControllerEvent(null, null, 0);
                return false;
            }

            byte[][] tokens = data.Split(0, 3);

            byte sender = tokens[0][0];
            string name = encoding.GetString(tokens[1]);
            string button = encoding.GetString(tokens[2]);

            @event = new NotificatorControllerEvent(name, button, sender);
            return true;
        }

        #endregion
    }
}
