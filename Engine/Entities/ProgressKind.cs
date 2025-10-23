using System;

namespace Engine.Entities
{
    [Flags]
    public enum ProgressKind
    {
        None = 0,
        StatusMessage = 1,
        ItemList = 2,
    }
}
