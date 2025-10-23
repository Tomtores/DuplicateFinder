using System.Collections.Generic;
using System.Linq;
using Engine.Entities;
using System.IO;

namespace Engine.CleanupStrategies
{
    internal class RemoveExtraCopiesWithinFolderStrategy : IDeduplicationStrategy
    {
        public IEnumerable<string> MarkTrash(IEnumerable<Duplicate[]> items)
        {
            return items.Select(i => GetFolderDupes(i)).SelectMany(i => i);                
        }

        private IEnumerable<string> GetFolderDupes(Duplicate[] group)
        {
            return group.GroupBy(i => i.DirectoryName)
                .Select(g => g.OrderBy(i => Path.GetFileName(i.FullName)).Skip(1))
                .SelectMany(i => i)
                .Select(i => i.FullName);
        }
    }
}
