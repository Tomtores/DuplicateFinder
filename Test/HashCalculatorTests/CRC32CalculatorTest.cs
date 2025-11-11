using System.IO;
using Engine.HashCalculators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.HashCalculatorTests
{
    [TestClass]
    public class CRC32CalculatorTest
    {
        [TestMethod]
        public void GivenEmptyFile_CRCIsZero()
        {
            var file = new MemoryStream();

            var result = CRC32Calculator.Calculate(file);

            Assert.AreEqual((uint)0, result);
        }

        [TestMethod]
        public void GivenTestSequence_CRCShouldMatch()
        {
            var file = new MemoryStream();
            var writer = new StreamWriter(file);
            writer.Write("123456789");
            writer.Flush();
            file.Position = 0;
            
            var result = CRC32Calculator.Calculate(file);

            var check_for_1to9 = 0xCBF43926;   //this is supposedly the check value for crc32 used in pkzip etc
            Assert.AreEqual(check_for_1to9, result);
        }

        [TestMethod]
        public void GivenDifferentSequences_ShouldGiveDifferentResults()
        {
            var file1 = new MemoryStream();
            var writer1 = new StreamWriter(file1);
            writer1.Write("123456789");
            writer1.Flush();
            file1.Position = 0;

            var file2 = new MemoryStream();
            var writer2 = new StreamWriter(file2);
            writer2.Write("987654321");
            writer2.Flush();
            file2.Position = 0;

            var result1 = CRC32Calculator.Calculate(file1);
            var result2 = CRC32Calculator.Calculate(file2);

            Assert.AreNotEqual(result1, result2);
        }
    }
}
