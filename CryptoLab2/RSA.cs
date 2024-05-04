using CryptoLab2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLab2
{
    internal class RSA
    {
        RNG _rng;
        PrimalityTester _tester;
        const int TEST_NUMBER = 2333;
        public RSA(RNG rng, PrimalityTester tester)
        {
            _rng = rng;
            _tester = tester;
        }

        public BigInteger generatePrimeNumber(int bits)
        {

            BigInteger maxInt = BigInteger.Pow(2, bits);
            BigInteger minInt = BigInteger.Pow(2, bits - 1);
            BigInteger n = _rng.RandomInRange(0, maxInt);
            while (!_tester.isPrime(n, TEST_NUMBER))
            {
                do n = _rng.RandomInRange(minInt, maxInt);
                while (n % 2 != 1);
                continue;
            }
            return n;
        }

        BigInteger LCM(BigInteger a, BigInteger b)
        {
            return (a * b) / BigInteger.GreatestCommonDivisor(a, b);
        }

        BigInteger Carmichael(BigInteger p, BigInteger q)
        {
            return LCM(p - 1, q - 1);
        }

        BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger x0 = 0;
            BigInteger x1 = 1;

            while (a > 1)
            {
                BigInteger q = a / m;

                BigInteger temp = m;

                m = a % m;
                a = temp;
                temp = x0;

                x0 = x1 - q * x0;
                x1 = temp;
            }

            if (x1 < 0)
            {
                x1 += m0;
            }

            return x1;
        }

        /// <summary>
        /// RSA encryption algorithm
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <param name="msg"></param>
        /// <returns>(e, d, n), where (e, n) is a public key for encryption, and d is a private key for decryption</returns>
        /// <exception cref="ArgumentException"></exception>
        public (BigInteger, BigInteger, BigInteger) GenerateKey(BigInteger p, BigInteger q)
        {
            if (!_tester.isPrime(p, TEST_NUMBER) || !_tester.isPrime(q, TEST_NUMBER))
            {
                throw new ArgumentException("p AND q should be prime!");
            }

            BigInteger n = p * q;

            BigInteger lambda = Carmichael(p, q);

            BigInteger e;
            do e = _rng.RandomInRange(2, lambda - 1);
            while (BigInteger.GreatestCommonDivisor(e, lambda) != 1);

            BigInteger d = ModInverse(e, lambda);
            return (e, d, n);
        }

        public BigInteger Encrypt(BigInteger n, BigInteger e, int msg)
        {
            BigInteger encryptedMessage = BigInteger.ModPow(msg, e, n);
            return encryptedMessage;
        }
        public BigInteger Decrypt(BigInteger encryptedMessage, BigInteger d, BigInteger n)
        {
            BigInteger message = BigInteger.ModPow(encryptedMessage, d, n);
            return message;
        }
        public BigInteger DecryptCRT(BigInteger encryptedMessage, BigInteger d, BigInteger n, BigInteger p, BigInteger q)
        {
            BigInteger d_p = d % (p - 1);
            BigInteger d_q = d % (q - 1);
            BigInteger q_inv = ModInverse(q, p);

            BigInteger m1 = BigInteger.ModPow(encryptedMessage, d_p, p);
            BigInteger m2 = BigInteger.ModPow(encryptedMessage, d_q, q);
            BigInteger h = q_inv * (m1 - m2) % p;
            BigInteger m = m2 + h * q;

            return m;
        }
    }

}
