using Engine.Entities;
using Engine.HashCalculators;

namespace Plugins.Cache
{
    class CachedHasherDecorator : IHashCalculator
    {
        private readonly IHashCache cache;
        private readonly ChecksumKind hashName;
        private readonly IHashCalculator decoratedObject;

        public CachedHasherDecorator(IHashCalculator decoratedObject, ChecksumKind hashName, IHashCache cache)
        {
            this.decoratedObject = decoratedObject;
            this.cache = cache;
            this.hashName = hashName;
        }

        public Checksum ComputeHash(Duplicate duplicate)
        {
            // get hash from cache
            var cached = cache.GetHash(duplicate.FullName, duplicate.Size, hashName, duplicate.Timestamp);
            if (cached != null)
            {
                return new Checksum(hashName.ToString("g"), cached);
            }

            var hash = this.decoratedObject.ComputeHash(duplicate);

            if (hash != null && hash.Value?.Length > 0)
            {
                cache.Store(duplicate.FullName, duplicate.Size, hashName, hash.Value, duplicate.Timestamp);
            }

            return hash;
        }

        public byte[] PeekHash(Duplicate duplicate) => cache.GetHash(duplicate.FullName, duplicate.Size, hashName, duplicate.Timestamp);
    }
}
