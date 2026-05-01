using Engine.Entities;
using Engine.HashCalculators;
using EnginePlugins.Cache;

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

        public byte[] ComputeHash(Duplicate duplicate)
        {
            // get hash from cache
            var cached = cache.GetHash(duplicate.FullName, duplicate.Size, hashName, duplicate.Timestamp);
            if (cached != null)
            {
                return cached;
            }

            var hash = this.decoratedObject.ComputeHash(duplicate);
            
            cache.Store(duplicate.FullName, duplicate.Size, hashName, hash, duplicate.Timestamp);

            return hash;
        }
    }
}
