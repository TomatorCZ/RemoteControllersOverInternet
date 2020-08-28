using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace RemoteController
{
    public static class MemoryStreamExtensions
    {
        /// <summary>
        /// Writes the whole array to the stream.
        /// </summary>
        public static void WriteBytes(this MemoryStream stream, byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            stream.Write(value, 0, value.Length);
        }
    }
}
