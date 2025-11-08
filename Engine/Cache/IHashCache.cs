using EnginePlugins.Cache;
using System;

namespace Plugins.Cache
{
   public interface IHashCache
    {
        string GetHash(string fullName, long size, ChecksumKind hashName, DateTime fileModifiedDateUtc);
        void Store(string fullName, long size, ChecksumKind hashName, string hash, DateTime fileModifiedDateUtc);
        void Remove(string fullName);
        void Flush();
        void Trim(Action<int> updateProgress);
    }
}
