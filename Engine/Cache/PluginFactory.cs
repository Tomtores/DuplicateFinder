using Engine.HashCalculators;
using EnginePlugins.Cache;
using Plugins.Cache;

namespace Plugins
{
    public static class PluginFactory
    {
        public static IHashCache ConfigureCache(string path)
        {
            return new HashCache(path);
        }

        public static IHashCalculator ApplyCaching(IHashCalculator hasher, ChecksumKind hashname, IHashCache cache)
        {
            return new CachedHasherDecorator(hasher, hashname, cache);
        }        
    }

}
