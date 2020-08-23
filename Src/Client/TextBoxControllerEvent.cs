using Microsoft.AspNetCore.Components;
using RemoteController;
using System;
using System.Data;
using System.Text;

namespace Client
{
    public class TextBoxControllerEvent : ControllerEvent
    {
        public string Text { get; }

        public TextBoxControllerEvent(string text, byte senderID) : base(senderID)
        {
            Text = text;
        }

        #region Encode/Decode
        public override byte[] Encode(Encoding encoding)
        {
            byte[] text = (Text != null) ? encoding.GetBytes(Text) : null;

            if (text == null)
            {
                return new byte[] { SenderID };
            }
            else
            {
                byte[] result = new byte[text.Length + 1];
                result[0] = SenderID;
                Buffer.BlockCopy(text, 0, result, 1, text.Length);

                return result;
            }
        }

        public static new bool TryDecode(byte[] data, Encoding encoding, out ControllerEvent @event)
        {
            if (data == null || data.Length == 0)
            {
                @event = new TextBoxControllerEvent(null, 0);
                return false;
            }

            byte sender = data[0];
            string text = null;

            if (data.Length > 1)
                text = encoding.GetString(data, 1, data.Length - 1);

            @event = new TextBoxControllerEvent(text, sender);
            return true;
        }
        #endregion
    }
}
