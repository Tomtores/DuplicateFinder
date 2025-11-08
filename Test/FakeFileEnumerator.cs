using Engine.FileEnumerators;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    internal class FakeFileEnumerator : IFileEnumerator
    {
        private IDictionary<string, Stream> _files;

        public FakeFileEnumerator(IDictionary<string, Stream> files)
        {
            _files = files;
        }

        public IEnumerable<string> EnumerateFiles(string[] paths, string filter)
        {
            return _files.Keys;
        }
    }
}