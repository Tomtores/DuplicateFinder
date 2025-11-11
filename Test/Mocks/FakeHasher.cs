using Engine.Entities;
using Engine.HashCalculators;
using System;
using System.Security.Cryptography;

namespace Test
{
    /// <summary>
    /// Fake hasher, returns same value regardles of file passed.
    /// </summary>
    internal class FakeHasher : IHashCalculator
    {
        private FakeFileAccessor filesystemProxy;

        public FakeHasher(FakeFileAccessor filesystemProxy)
        {
            this.filesystemProxy = filesystemProxy;
        }

        public string ComputeHash(Duplicate duplicate)
        {
            var file = filesystemProxy.GetFile(duplicate.FullName);
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(file);
                return BitConverter.ToString(hash);
            }
        }
    }
}
