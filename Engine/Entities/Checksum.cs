using System.Linq;

namespace Engine.Entities
{
    public class Checksum
    {
        public readonly string Type;

        public readonly byte[] Value;

        public Checksum(string type, byte[] value)
        {
            Type = type;
            Value = value ?? new byte[0];
        }

        public static bool operator== (Checksum left, Checksum right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            if (left.Type != right.Type)
            {
                return false;
            }

            if (left.Value.Length != right.Value.Length)
            {
                return false;
            }

            return left.Value.SequenceEqual(right.Value);
        }

        public static bool operator !=(Checksum left, Checksum right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is Checksum other)
            {
                return this == other;
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Value == null || Value.Length == 0)
            {
                return Type.GetHashCode();
            }

            return Type.GetHashCode() ^ Value[0];
        }
    }
}
