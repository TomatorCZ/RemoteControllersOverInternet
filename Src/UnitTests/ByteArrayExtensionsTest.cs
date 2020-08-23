using Microsoft.VisualStudio.TestTools.UnitTesting;
using RemoteController;
using System.Diagnostics;

namespace UnitTests
{
    [TestClass]
    public class ByteArrayExtensionsTest
    {
        [TestMethod]
        public void Split_1()
        {
            //Arrange
            byte[] sample = new byte[0];

            //Act
            byte[][] result = sample.Split(0, 1);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Length);
        }

        [TestMethod]
        public void Split_2()
        {
            //Arrange
            byte[] sample = new byte[8] { 1,0,1,0,1,1,1,0};
            byte[][] expected = new byte[][] 
            { 
                new byte[1] { 1},
                new byte[1] { 1},
                new byte[3] { 1,1,1} 
            };

            //Act
            byte[][] result = sample.Split(0, 3);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Length, result.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                AssertHelper.AreEqual(expected[i], result[i]);
            } 
        }

        [TestMethod]
        public void Split_3()
        {
            //Arrange
            byte[] sample = new byte[5] { 1, 0, 97, 0, 98};
            byte[][] expected = new byte[][]
            {
                new byte[1] { 1},
                new byte[1] { 97},
                new byte[1] { 98 }
            };

            //Act
            byte[][] result = sample.Split(0, 3);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Length, result.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                AssertHelper.AreEqual(expected[i], result[i]);
            }
        }

        [TestMethod]
        public void SubArray_1()
        {
            //Arrange
            byte[] sample = new byte[10] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 1 };
            byte[] expected = new byte[10] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 1 };
            
            //Act
            byte[] result = sample.SubArray(0, 10);

            //Assert
            AssertHelper.AreEqual(expected, result);
        }

        [TestMethod]
        public void SubArray_2()
        {
            //Arrange
            byte[] sample = new byte[10] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 1 };
            byte[] expected = new byte[0];

            //Act
            byte[] result = sample.SubArray(0, 0);

            //Assert
            AssertHelper.AreEqual(expected, result);
        }

        [TestMethod]
        public void SubArray_3()
        {
            //Arrange
            byte[] sample = new byte[10] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 1 };
            byte[] expected = new byte[6] { 1, 2, 1, 2, 1, 1 };

            //Act
            byte[] result = sample.SubArray(4, 6);

            //Assert
            AssertHelper.AreEqual(expected, result);
        }

        [TestMethod]
        public void SubArray_4()
        {
            //Arrange
            byte[] sample = new byte[10] { 1, 2, 1, 2, 1, 2, 1, 2, 1, 1 };
            byte[] expected = new byte[0];

            //Act
            byte[] result = sample.SubArray(9, 0);

            //Assert
            AssertHelper.AreEqual(expected, result);
        }
    }
}
