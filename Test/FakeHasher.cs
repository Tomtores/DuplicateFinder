using Engine.Entities;
using Engine.HashCalculators;

namespace Test
{
    /// <summary>
    /// Fake hasher, returns same value regardles of file passed.
    /// </summary>
    public class FakeHasher : IHashCalculator
    {
        public string ComputeHash(Duplicate duplicate)
        {
            return "FAKE";
        }
    }
}
