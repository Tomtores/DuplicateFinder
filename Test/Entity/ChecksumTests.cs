using Engine.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;

namespace Test.Entity
{
    [TestClass]
    public class ChecksumTests
    {
        [TestMethod]
        public void CanCreateInstanceWithAnyByteArray()
        {
            byte[] value = UTF8Encoding.UTF8.GetBytes("this is my random byte array");
            var checksum = new Checksum("Alg", value);

            Assert.AreEqual("Alg", checksum.Type);
            Assert.IsTrue(value.SequenceEqual(checksum.Value));
        }

        [TestMethod]
        public void CanCreateInstanceWithNullByteArray_NullIsConvertedToEmptyByte()
        {
            var checksum = new Checksum("Alg", null);

            Assert.AreEqual("Alg", checksum.Type);
            Assert.IsNotNull(checksum.Value);
            Assert.AreEqual(0, checksum.Value.Length);
        }

#pragma warning disable CS1718 // comparisons with self are intentional

        [TestMethod]
        public void ItemIsEqualToItself()
        {
            var checksum = new Checksum("Alg", new byte[] { 1, 2, 3, 4, 5 });

            var result1 = checksum.Equals(checksum);
            var result2 = checksum == checksum;
            var result3 = checksum != checksum;

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsFalse(result3);
        }

#pragma warning restore CS1718

        [TestMethod]
        public void GivenTwoDifferentItems_ReturnsNotEqual()
        { 
            var checksum1 = new Checksum("Alg", new byte[] { 1, 2, 3, 4, 5 });
            var checksum2 = new Checksum("ori", new byte[] { 5, 4, 3, 2, 1 });

            var result1 = checksum1.Equals(checksum2);
            var result1r = checksum2.Equals(checksum1);
            var result2 = checksum1 == checksum2;
            var result2r = checksum2 == checksum1;
            var result3 = checksum1 != checksum2;
            var result3r = checksum2 != checksum1;

            Assert.IsFalse(result1);
            Assert.IsFalse(result1r);
            Assert.IsFalse(result2);
            Assert.IsFalse(result2r);
            Assert.IsTrue(result3);
            Assert.IsTrue(result3r);
        }

        [TestMethod]
        public void GivenTwoDifferentItems_WithSameAlgorithm_ReturnsNotEqual()
        {
            var checksum1 = new Checksum("Alg", new byte[] { 1, 2, 3, 4, 5 });
            var checksum2 = new Checksum("Alg", new byte[] { 5, 4, 3, 2, 1 });

            var result1 = checksum1.Equals(checksum2);
            var result1r = checksum2.Equals(checksum1);
            var result2 = checksum1 == checksum2;
            var result2r = checksum2 == checksum1;
            var result3 = checksum1 != checksum2;
            var result3r = checksum2 != checksum1;

            Assert.IsFalse(result1);
            Assert.IsFalse(result1r);
            Assert.IsFalse(result2);
            Assert.IsFalse(result2r);
            Assert.IsTrue(result3);
            Assert.IsTrue(result3r);
        }

        [TestMethod]
        public void GivenTwoIdenticalItems_ReturnsEqual()
        {
            var checksum1 = new Checksum("Alg", new byte[] { 1, 2, 3, 4, 5 });
            var checksum2 = new Checksum("Alg", new byte[] { 1, 2, 3, 4, 5 });

            var result1 = checksum1.Equals(checksum2);
            var result1r = checksum2.Equals(checksum1);
            var result2 = checksum1 == checksum2;
            var result2r = checksum2 == checksum1;
            var result3 = checksum1 != checksum2;
            var result3r = checksum2 != checksum1;

            Assert.IsTrue(result1);
            Assert.IsTrue(result1r);
            Assert.IsTrue(result2);
            Assert.IsTrue(result2r);
            Assert.IsFalse(result3);
            Assert.IsFalse(result3r);
        }

        [TestMethod]
        public void WhenComparingWithNull_ReturnsNotEqual()
        {
            var checksum1 = new Checksum("Alg", new byte[] { 1, 2, 3, 4, 5 });
            Checksum checksum2 = null;

            var result1 = checksum1.Equals(checksum2);
            // var result1r = checksum2.Equals(checksum1);  // can't call equals on null
            var result2 = checksum1 == checksum2;
            var result2r = checksum2 == checksum1;
            var result3 = checksum1 != checksum2;
            var result3r = checksum2 != checksum1;

            Assert.IsFalse(result1);
            // Assert.IsFalse(result1r);
            Assert.IsFalse(result2);
            Assert.IsFalse(result2r);
            Assert.IsTrue(result3);
            Assert.IsTrue(result3r);
        }

        [TestMethod]
        public void GivenTwoDifferentItems_WithNullByteArray_ReturnsNotEqual()
        {
            var checksum1 = new Checksum("Alg", new byte[] { 1, 2, 3, 4, 5 });
            var checksum2 = new Checksum("Alg", null);

            var result1 = checksum1.Equals(checksum2);
            var result1r = checksum2.Equals(checksum1);
            var result2 = checksum1 == checksum2;
            var result2r = checksum2 == checksum1;
            var result3 = checksum1 != checksum2;
            var result3r = checksum2 != checksum1;

            Assert.IsFalse(result1);
            Assert.IsFalse(result1r);
            Assert.IsFalse(result2);
            Assert.IsFalse(result2r);
            Assert.IsTrue(result3);
            Assert.IsTrue(result3r);
        }

        [TestMethod]
        public void GivenTwoDifferentItems_WithEmptyByteArray_ReturnsNotEqual()
        {
            var checksum1 = new Checksum("Alg", new byte[] { 1, 2, 3, 4, 5 });
            var checksum2 = new Checksum("Alg", new byte[] { });

            var result1 = checksum1.Equals(checksum2);
            var result1r = checksum2.Equals(checksum1);
            var result2 = checksum1 == checksum2;
            var result2r = checksum2 == checksum1;
            var result3 = checksum1 != checksum2;
            var result3r = checksum2 != checksum1;

            Assert.IsFalse(result1);
            Assert.IsFalse(result1r);
            Assert.IsFalse(result2);
            Assert.IsFalse(result2r);
            Assert.IsTrue(result3);
            Assert.IsTrue(result3r);
        }

        [TestMethod]
        public void GivenTwoIdenticalItems_WithEmptyArrays_ReturnsEqual()
        {
            var checksum1 = new Checksum("Alg", new byte[] { });
            var checksum2 = new Checksum("Alg", new byte[] { });

            var result1 = checksum1.Equals(checksum2);
            var result1r = checksum2.Equals(checksum1);
            var result2 = checksum1 == checksum2;
            var result2r = checksum2 == checksum1;
            var result3 = checksum1 != checksum2;
            var result3r = checksum2 != checksum1;

            Assert.IsTrue(result1);
            Assert.IsTrue(result1r);
            Assert.IsTrue(result2);
            Assert.IsTrue(result2r);
            Assert.IsFalse(result3);
            Assert.IsFalse(result3r);
        }

        [TestMethod]
        public void GivenTwoDifferentItems_WithDifferentByteArrays_ReturnsNotEqual()
        {
            var checksum1 = new Checksum("Alg", new byte[] { 1, 2, 3, 4, 5 });
            var checksum2 = new Checksum("ori", new byte[] { 1, 2, 3, 4, 5, 6, 7 });

            var result1 = checksum1.Equals(checksum2);
            var result1r = checksum2.Equals(checksum1);
            var result2 = checksum1 == checksum2;
            var result2r = checksum2 == checksum1;
            var result3 = checksum1 != checksum2;
            var result3r = checksum2 != checksum1;

            Assert.IsFalse(result1);
            Assert.IsFalse(result1r);
            Assert.IsFalse(result2);
            Assert.IsFalse(result2r);
            Assert.IsTrue(result3);
            Assert.IsTrue(result3r);
        }
    }
}
