using Engine.Entities;
using System;

namespace Components
{
    public class DuplicateViewItemHeader : Duplicate
    {
        public DuplicateViewItemHeader(long size, Checksum hash, string fullName, DateTime timestamp) : base(fullName, size, timestamp)
        {
            this.Hash = hash;
        }
    }
}