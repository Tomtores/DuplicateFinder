using System.Collections.Generic;

namespace Engine
{
    public interface IDuplicateResults<T> 
    {
        IEnumerable<T> Results { get; }
    }
}
