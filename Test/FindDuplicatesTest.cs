using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Test
{
    [TestClass]
    public class FindDuplicatesTest
    {
        private IFinder _finder;

        public string[] paths = new string[] { @"Z:\TestFolder", @"Z:\AnotherFolder\Nested folder with space" };
        public string filter = "*.*";
        public string[] ignored = new string[] { };

        public FindDuplicatesTest()
        {
            var files = new Dictionary<string, Stream>
            {
                { @"Z:\TestFolder\File1.txt", new MemoryStream(Encoding.UTF8.GetBytes("This is the content of File1.")) },
                { @"Z:\TestFolder\File2.txt", new MemoryStream(Encoding.UTF8.GetBytes("This is the content of File2.")) },
                { @"Z:\AnotherFolder\Nested folder with space\File3.txt", new MemoryStream(Encoding.UTF8.GetBytes("This is the content of File1.")) }, // Duplicate of File1
                { @"Z:\AnotherFolder\Nested folder with space\File4.txt", new MemoryStream(Encoding.UTF8.GetBytes("This is the content of File4.")) }
            };

            var enumerator = new FakeFileEnumerator(files);
            
            _finder = new FinderProxy(enumerator, new FakeHasher());
        }

        [TestMethod]
        public void Test1()
        {
            _finder.FindDuplicates(paths, filter, ignored);
        }        
    }
}
