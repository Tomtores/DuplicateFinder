using System.Collections.Generic;

namespace DuplicateFinder.Commands
{
    public class DeletionParameters
    {
        public DeletionParameters(IEnumerable<string> deletionItems)
        {
            DeletionItems = deletionItems;
        }

        public IEnumerable<string> DeletionItems { get; }
    }
}
