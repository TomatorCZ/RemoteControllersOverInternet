using System.Text;

namespace RemoteController
{
    public struct ControllerEventInfo
    {
        public ControllerEventInfo(byte iD, ControllerEvent controllerEvent)
        {
            ID = iD;
            ControllerEvent = controllerEvent;
        }

        public byte ID { get; }
        public ControllerEvent ControllerEvent { get; }  
    }

    public class ControllerEvent : IEncoder
    {
        public byte SenderID { get; }

        public ControllerEvent(byte senderID)
        {
            SenderID = senderID;
        }

        #region Encode/Decode
        public virtual byte[] Encode(Encoding encoding)
        {
            return new byte[] { SenderID };
        }

        public static bool TryDecode(byte[] data, Encoding encoding, out ControllerEvent controllerEvent)
        {
            if (data == null || data.Length != 1)
            {
                controllerEvent = new ControllerEvent(0);
                return false;
            }
            else
            {
                controllerEvent = new ControllerEvent(data[0]);
                return true;
            }
        }
        #endregion
    }
    
    #region Button
    public enum ButtonEvent { Click }
    public class ButtonControllerEvent : ControllerEvent
    {
        ButtonEvent Event { get; }
        
        public ButtonControllerEvent(byte senderID, ButtonEvent @event) : base(senderID)
        {
            Event = @event;
        }

        public override byte[] Encode(Encoding encoding)
        {
            byte[] header = base.Encode(encoding);
            byte body = (byte)Event;

            return new byte[2] { header[0], body };
        }

        public static new bool TryDecode(byte[] data, Encoding encoding, out ControllerEvent controllerEvent)
        {
            if (data == null || data.Length != 2)
            {
                controllerEvent = new ControllerEvent(0);
                return false;
            }
            else
            {
                switch (data[1])
                {
                    case (byte)ButtonEvent.Click:
                        controllerEvent = new ButtonControllerEvent(data[0], ButtonEvent.Click);
                        return true;
                    default:
                        controllerEvent = new ControllerEvent(0);
                        return false;
                }
            }
        }
    }
    #endregion

    #region TextBox
    #endregion

    #region Slider
    #endregion 
}
