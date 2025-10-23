using Engine.Entities;

namespace Components
{
    internal class DuplicateViewItemHeader : Duplicate
    {
        public DuplicateViewItemHeader(long size, string hash, string fullName) : base(fullName, size)
        {
            this.Hash = hash;
        }
    }
}