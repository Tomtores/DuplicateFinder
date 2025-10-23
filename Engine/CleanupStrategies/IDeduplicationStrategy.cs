using Engine.Entities;
using System.Collections.Generic;

namespace Engine.CleanupStrategies
{
    public interface IDeduplicationStrategy
    {
        IEnumerable<string> MarkTrash(IEnumerable<Duplicate[]> items);
    }
}
