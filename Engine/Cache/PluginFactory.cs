using Engine.Entities;
using Engine.HashCalculators;
using Engine.Infrastructure;
using Plugins.Cache;
using System;

namespace Plugins
{
    public static class PluginFactory
    {
        public static IHashCache ConfigureCache(string path, long sizelimit, long sizeLimitForMD5, Guid? installationSalt, ILogger logger)
        {
            return new HashCache(path, sizelimit, sizeLimitForMD5, installationSalt, logger);
        }

        public static IHashCalculator ApplyCaching(IHashCalculator hasher, ChecksumKind hashname, IHashCache cache)
        {
            return new CachedHasherDecorator(hasher, hashname, cache);
        }        
    }
}
