using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
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
        public virtual byte[] Encode(Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public virtual bool TryDecode(Encoding encoding, out ControllerEvent controllerEvent)
        {
            throw new NotImplementedException();
        }
    }

    public class ButtonControllerEvent : ControllerEvent
    { 
        
    }

}
