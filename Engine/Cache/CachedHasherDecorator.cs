using Engine.Entities;
using Engine.HashCalculators;
using EnginePlugins.Cache;

namespace Plugins.Cache
{
    class CachedHasherDecorator : IHashCalculator
    {
        private IHashCache cache;
        private ChecksumKind hashName;
        private IHashCalculator decoratedObject;

        public CachedHasherDecorator(IHashCalculator decoratedObject, ChecksumKind hashName, IHashCache cache)
        {
            this.decoratedObject = decoratedObject;
            this.cache = cache;
            this.hashName = hashName;
        }

        public string ComputeHash(Duplicate duplicate)
        {
            // get hash from cache
            var cached = cache.GetHash(duplicate.FullName, duplicate.Size, hashName);
            if (!string.IsNullOrWhiteSpace(cached))
            {
                return cached;
            }

            var hash = this.decoratedObject.ComputeHash(duplicate);

            cache.Store(duplicate.FullName, duplicate.Size, hashName, hash);

            return hash;
        }
    }
}
