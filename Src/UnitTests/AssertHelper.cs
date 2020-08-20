using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    public static class AssertHelper
    {
        public static void AreEqual(byte[] expected, byte[] actual)
        {
            if (expected == null || actual == null)
            {
                if (expected == null && actual == null)
                    return;
                else
                    throw new AssertFailedException("Null reference");
            }

            if (expected.Length != actual.Length)
                throw new AssertFailedException("Lengths are different");

            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] != actual[i])
                    throw new AssertFailedException("Items are different");
            }
        }
    }
}
