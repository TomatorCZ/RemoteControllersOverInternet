using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RemoteControllers
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Splits a byte array by a separator. If lenghtOfArray is not enough, returns only first lengthOfArray items.
        /// </summary>
        public static byte[][] Split(this byte[] array, byte separator, int lengthOfArray)
        {
            var result = new byte[lengthOfArray][];
            int indexOfResult = 0;

            int lastIndex = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == separator && i != lastIndex)
                {
                    if (indexOfResult >= result.Length)
                    {
                        return result;
                    }
                    else
                    {
                        int length = i - lastIndex;
                        result[indexOfResult++] = array.SubArray(lastIndex, length);
                        
                        lastIndex = i + 1;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creates new array and copies all bytes in given range.
        /// </summary>
        public static byte[] SubArray(this byte[] array, int index, int length)
        {
            if (length < 0)
                throw new ArgumentException("Can not be negative", nameof(length));
            if (index < 0)
                throw new ArgumentException("Can not be negative", nameof(index));

            var result = new byte[length];
            for (int i = 0; i < array.Length && i < length; i++)
            {
                result[i] = array[index + i];
            }

            return result;
        }
    }
}
