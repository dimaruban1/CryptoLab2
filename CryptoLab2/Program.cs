using CryptoLab2.Utility;
using CryptoLab2;
using System.Security.Cryptography;
using System.Numerics;
using System.Diagnostics;
using System.Text;

using System.Security.Cryptography.RandomNumberGenerator rng = System.Security.Cryptography.RandomNumberGenerator.Create();

/*
ЛАБОРАТОРНА 2
Реалізувати власний клас для роботи з RSA. Симулювати обмін текстовими повідомленнями між Алісою та Бобом. +
В реалізації:
Для генерації простих чисел використати алгоритми з 1 лабораторної +
Використовувати функцію Кармайкла замість функції Ейлера. +
Використати китайську теорему про залишки для дешифрування *** +
Порівняти швидкість роботи алгоритму в залежності від бінарної довжини простих чисел. +
Повідомлення не тільки шифруються, але й “підписуються” використовуючи вашу реалізацію будь-якого гєш-алгоритму. *** +
 */


RNG custom_rng = new RNG(rng);
PrimalityTester tester = new PrimalityTester(custom_rng);
var rsa = new CryptoLab2.RSA(custom_rng, tester);

const int BIT_LENGTH = 8;
void TestSendingMessage()
{
    var p = rsa.generatePrimeNumber(BIT_LENGTH);
    var q = rsa.generatePrimeNumber(BIT_LENGTH);

    var (e, d, n) = rsa.GenerateKey(p, q);
    Console.WriteLine($"Keys are: e={e}, d={d}, n={n}");

    var aliceMessage = "Hi Bob!";
    var hash = H(aliceMessage);
    Console.WriteLine($"hash function of sent message: {hash}");

    var res = new List<BigInteger>();
    foreach (var c in aliceMessage)
    {
        var localRes = rsa.Encrypt(n, e, c);
        res.Add(localRes);
        Console.Write("{0} ", localRes);
    }
    Console.WriteLine();
    StringBuilder receivedMessage = new StringBuilder();
    foreach (var r in res)
    {
        var c = rsa.DecryptCRT(r, d, n, p, q);
        receivedMessage.Append((char)c);
        Console.Write("{0} ", (char)c);
    }
    Console.WriteLine();
    Console.WriteLine($"hash function of received message: {H(receivedMessage.ToString())}");
}

void TestPerformanceTime()
{
    Stopwatch sw = new Stopwatch();

    int[] testBitLengths = new int[] { 16, 32, 64, 128, 256, 512, 1028 };


    foreach (var bitLength in testBitLengths)
    {
        sw.Start();
        var p = rsa.generatePrimeNumber(bitLength);
        var q = rsa.generatePrimeNumber(bitLength);
        
        Console.WriteLine($"p q generated, Elapsed={sw.Elapsed}");

        var (e, d, n) = rsa.GenerateKey(p, q);

        Console.WriteLine($"key generated, Elapsed={sw.Elapsed}");

        var aliceMessage = "Hi Bob!";
        var res = new List<BigInteger>();
        foreach (var c in aliceMessage)
        {
            var localRes = rsa.Encrypt(n, e, c);
            res.Add(localRes);
        }
        foreach (var r in res)
        {
            var c = rsa.Decrypt(r, d, n);
        }
        sw.Stop();
        Console.WriteLine($"Bit length: {bitLength} Elapsed={sw.Elapsed}");
        Console.WriteLine();
        sw.Reset();
    }

}
string XOR(string binary1, string binary2)
{
    // Ensure both binary strings have the same length
    int maxLength = Math.Max(binary1.Length, binary2.Length);
    binary1 = binary1.PadLeft(maxLength, '0');
    binary2 = binary2.PadLeft(maxLength, '0');

    // Perform bitwise XOR operation
    char[] result = new char[maxLength];
    for (int i = 0; i < maxLength; i++)
    {
        result[i] = (binary1[i] == binary2[i]) ? '0' : '1';
    }

    return new string(result);
}
string H(string message)
{
    string sum = "00000000";
    foreach (char c in message)
    {
        sum = XOR(sum, Convert.ToString(c, 2));
    }
    return sum;
}

Console.WriteLine("Message exchange test");
TestSendingMessage();
Console.WriteLine();
Console.WriteLine("Time performance measuring");
TestPerformanceTime();
Console.WriteLine();

/*
Message exchange test
6498 4073 8048 2214 6151 7432 4590
H i   B o b !
Time performance measuring
p q generated, Elapsed=00:00:00.0021780
key generated, Elapsed=00:00:00.0048494
Bit length: 16 Elapsed=00:00:00.0049312

p q generated, Elapsed=00:00:00.0162934
key generated, Elapsed=00:00:00.0323747
Bit length: 32 Elapsed=00:00:00.0325741

p q generated, Elapsed=00:00:00.0426185
key generated, Elapsed=00:00:00.0854891
Bit length: 64 Elapsed=00:00:00.0860945

p q generated, Elapsed=00:00:00.1689610
key generated, Elapsed=00:00:00.3427096
Bit length: 128 Elapsed=00:00:00.3455238

p q generated, Elapsed=00:00:00.9671861
key generated, Elapsed=00:00:01.8635152
Bit length: 256 Elapsed=00:00:01.8812948

p q generated, Elapsed=00:00:06.7202019
key generated, Elapsed=00:00:12.8219673
Bit length: 512 Elapsed=00:00:12.8744207

p q generated, Elapsed=00:00:22.3178156
key generated, Elapsed=00:00:52.9559832
Bit length: 1028 Elapsed=00:00:53.5530222

 */