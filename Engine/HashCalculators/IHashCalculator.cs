using Engine.Entities;
namespace Engine.HashCalculators
{
    /// <summary>
    /// Calculates file hash.
    /// </summary>
    public interface IHashCalculator
    {
        Checksum ComputeHash(Duplicate duplicate);
    }
}