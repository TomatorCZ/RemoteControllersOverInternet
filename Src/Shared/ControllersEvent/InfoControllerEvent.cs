using System;

namespace RemoteController
{
    /// <summary>
    /// Provides information about an incomming message.
    /// </summary>
    public struct InfoControllerEvent
    {
        public InfoControllerEvent(Guid sender, ControllerEvent @event)
        {
            Sender = sender;
            Event = @event;
        }

        public Guid Sender { get; }
        public ControllerEvent Event { get; }
    }
}
