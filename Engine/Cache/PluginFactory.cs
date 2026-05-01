using Engine.HashCalculators;
using Engine.Infrastructure;
using EnginePlugins.Cache;
using Plugins.Cache;
using System;

namespace Plugins
{
    public static class PluginFactory
    {
        public static IHashCache ConfigureCache(string path, long sizelimit, Guid? installationSalt, ILogger logger)
        {
            return new HashCache(path, sizelimit, installationSalt, logger);
        }

        public static IHashCalculator ApplyCaching(IHashCalculator hasher, ChecksumKind hashname, IHashCache cache)
        {
            return new CachedHasherDecorator(hasher, hashname, cache);
        }        
    }

}
