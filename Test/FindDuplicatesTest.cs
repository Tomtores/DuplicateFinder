using Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var files = new (string, byte[])[]
            {
                ( @"Z:\TestFolder\File1.txt", Encoding.UTF8.GetBytes("This is the content of File1.") ),
                ( @"Z:\TestFolder\File2.txt", Encoding.UTF8.GetBytes("This is the content of File2.") ),
                ( @"Z:\AnotherFolder\Nested folder with space\File3.txt", Encoding.UTF8.GetBytes("This is the content of File1.") ), // Duplicate of File1
                ( @"Z:\AnotherFolder\Nested folder with space\File4.txt", Encoding.UTF8.GetBytes("This is the content of File4.") )
            };

            var enumerator = new FakeFileAccessor();
            enumerator.SetupFiles(files);

            _finder = new FinderProxy(enumerator, new FakeHasher(enumerator));
        }

        [TestMethod]
        public void Test1()
        {
            _finder.FindDuplicates(paths, filter, ignored);
        }        
    }
}
