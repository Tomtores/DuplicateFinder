using EnginePlugins.Cache;
using System;

namespace Plugins.Cache
{
   public interface IHashCache
    {
        string GetHash(string fullName, long size, ChecksumKind hashName);
        void Store(string fullName, long size, ChecksumKind hashName, string hash);
        void Remove(string fullName);
        void Flush();
        void Trim(Action<int> updateProgress);
    }
}
