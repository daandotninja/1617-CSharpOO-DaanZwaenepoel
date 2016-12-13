using System;
using System.Text;
using System.Security.Cryptography;

namespace Globals
{
    public struct Sha1Hash:IEquatable<Sha1Hash>
    {
        private byte[] value;
                       
        private Sha1Hash(byte[] bytes)
        {
            if (bytes.Length > 40) throw new ArgumentException("SHA1 hash contains 20 bytes (160 bits);");
            value = bytes;
        }

        /// <summary>
        /// Returns a SHA-1 hash calculated from a string
        /// </summary>       
        public static Sha1Hash CalculateFromString(string text)
        {
            using (var sha = SHA1.Create())
            {
                return new Sha1Hash(sha.ComputeHash(Encoding.UTF8.GetBytes(text)));
            }
        }

        /// <summary>
        /// Converts a hash value (passed as a hex string) to the equivallent SHA-1 hash 
        /// </summary>        
        public static Sha1Hash GetFromHex(string hex)
        {
            hex = hex.ToUpper();
            if (hex.StartsWith("0X")) hex = hex.Substring(2);
            if (hex.Length != 40) throw new ArgumentOutOfRangeException("SHA1 hex string must contain 40 chars.");
            var bytes = new byte[20];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i*2,2 ),16);
            }

            return new Sha1Hash(bytes);
        }

        public override string ToString()
        {
            if (value == null) return "null";
            var sBuilder = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                sBuilder.Append(value[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
               
        public override bool Equals(object obj)
        {
            if (obj is Sha1Hash) return (this == (Sha1Hash) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(Sha1Hash other)
        {
            return (this == other);
        }

        public static bool operator ==(Sha1Hash a, Sha1Hash b)
        {
            if ((a == null) && (b == null)) return false;
            if ((a == null) || (b == null)) return false;
            for (int i = 0; i < a.value.Length; i++)
            {
                if ((a.value[i]) != (b.value[i])) return false;
            }
            return true;
        }

        public static bool operator !=(Sha1Hash a, Sha1Hash b)
        {
            return !(a == b);
        }

        
    }
}
