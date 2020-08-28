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

        /// <summary>
        /// Sender of the event.
        /// </summary>
        public Guid Sender { get; }

        /// <summary>
        /// The event.
        /// </summary>
        public ControllerEvent Event { get; }
    }
}
