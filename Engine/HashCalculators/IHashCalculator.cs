using Engine.Entities;
namespace Engine.HashCalculators
{
    /// <summary>
    /// Calculates file hash.
    /// </summary>
    public interface IHashCalculator
    {
        byte[] ComputeHash(Duplicate duplicate);
    }
}