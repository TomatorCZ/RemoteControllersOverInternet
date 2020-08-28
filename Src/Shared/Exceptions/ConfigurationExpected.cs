using System;

namespace RemoteController
{
    public class ConfigurationExpectedException : Exception
    {
        /// <summary>
        /// Represents a problem when <see cref="MessageManager"/> waits for a configuration message, but it could not be decoded.
        /// </summary>
        public ConfigurationExpectedException()
        {
        }
    }
}
