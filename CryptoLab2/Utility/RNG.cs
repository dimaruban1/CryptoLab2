using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLab2.Utility
{
    internal class RNG
    {
        RandomNumberGenerator rng;
        public RNG(RandomNumberGenerator rng)
        {
            this.rng = rng;
        }
        /// <summary>
        /// including min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public BigInteger RandomInRange(BigInteger min, BigInteger max)
        {
            if (min > max)
            {
                var buff = min;
                min = max;
                max = buff;
            }
            BigInteger offset = -min;
            min = 0;
            max += offset;
            var value = randomInRangeFromZeroToPositive(max) - offset;
            return value;
        }
        private BigInteger randomInRangeFromZeroToPositive(BigInteger max)
        {
            BigInteger value;
            var bytes = max.ToByteArray();
            byte zeroBitsMask = 0b00000000;
            var mostSignificantByte = bytes[bytes.Length - 1];
            for (var i = 7; i >= 0; i--)
            {
                if ((mostSignificantByte & 0b1 << i) != 0)
                {
                    var zeroBits = 7 - i;
                    zeroBitsMask = (byte)(0b11111111 >> zeroBits);
                    break;
                }
            }

            do
            {
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] &= zeroBitsMask;
                value = new BigInteger(bytes);
            } while (value > max);

            return value;
        }
    }
}
