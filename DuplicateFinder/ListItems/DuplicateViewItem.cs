using Engine.Entities;

namespace DuplicateFinder
{
    public class DuplicateViewItem : Duplicate
    {
        public int DirectoryDuplicatesCount { get; private set; }
        
        public DuplicateViewItem(Duplicate that, int directoryDuplicateCount):base(that.FullName, that.Size)
        {
            this.DirectoryDuplicatesCount = directoryDuplicateCount;
        }            
    }
}
