using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DESImplementation {
    class Program {
        private static TripleDESCryptoServiceProvider des;

        static void Main(string[] args) {
            string text = "SIABIRIS";
            string key = "F46Ε986435465354";
            string initVector = "";

            //initialization vector needs to be 8 bytes and different at each execution 
            //so we instantiate it randomly
            while (Encoding.UTF8.GetByteCount(initVector) < 8) {
                initVector += GetRandomChar();
            }

            Console.WriteLine("Would you like to enter a key and plain text or use the defaults ? y/n ");
            string input = Console.ReadLine();
            while (input != "y" && input != "n") {
                Console.WriteLine("Please enter a valid answer (y/n): ");
                input = Console.ReadLine();
            }
            if (input == "y") {
                Console.WriteLine("Please Enter your plain text: ");
                text = Console.ReadLine();
                Console.WriteLine("Please Enter your key: ");
                key = Console.ReadLine();
            }


            //in order to comply with 3DES key requirements we need a 24 byte (192 bit key)
            //our current key is 17*8 = 136 bytes, so we need to pad it out
            while (Encoding.UTF8.GetByteCount(key) < 24) {
                key += GetRandomChar();
            }
            
            text = Encrypt(text, key, initVector);

            Console.WriteLine("Encryption Results: ");
            Console.WriteLine("Cipher Text: " + text);
            Console.WriteLine("Triple DES compliant key: " + key);
            Console.WriteLine("Initialization Vector: " + initVector);
            Console.WriteLine("Execution Complete, press any key to exit.");
            Console.ReadKey();

            input = "";
            Console.WriteLine("Would you like to decrypt the string you just encrypted ? (y/n) ");
            input = Console.ReadLine();
            while (input != "y" && input != "n") {
                Console.WriteLine("Please enter a valid answer (y/n): ");
                input = Console.ReadLine();
            }
            if (input == "y") {
                var decryptedText = Decrypt(text, key, initVector);
                Console.WriteLine("Decryption Results: ");
                Console.WriteLine("Decrypted Plain Text: " + decryptedText);
                Console.WriteLine("Triple DES compliant key: " + key);
                Console.WriteLine("Initialization Vector: " + initVector);
                Console.WriteLine("Execution Complete, press any key to exit.");
                Console.ReadKey();
            }
            
        }

        private static string Encrypt(string text, string keyValue, string initializationVector) {
            des = new TripleDESCryptoServiceProvider();
            byte[] input = Encoding.UTF8.GetBytes(text);
            byte[] output = Transform(input, des.CreateEncryptor(Encoding.UTF8.GetBytes(keyValue), Encoding.UTF8.GetBytes(initializationVector)));
            return Convert.ToBase64String(output);
        }

        private static string Decrypt(string text, string keyValue, string initializationVector) {
            des = new TripleDESCryptoServiceProvider();
            byte[] input = Convert.FromBase64String(text);
            byte[] output = Transform(input, des.CreateDecryptor(Encoding.UTF8.GetBytes(keyValue), Encoding.UTF8.GetBytes(initializationVector)));
            return Encoding.UTF8.GetString(output);
        }

        private static byte[] Transform(byte[] input, ICryptoTransform cryptoTransform) {
            // Create the necessary streams
            // Transform the bytes as requesed

            byte[] resultText;
            using (var memStream = new MemoryStream())
            using (var crypStream = new CryptoStream(memStream, cryptoTransform, CryptoStreamMode.Write)) {


                //encrypt our input and load it in the memory stream
                crypStream.Write(input, 0, input.Length);
                crypStream.FlushFinalBlock();
            
                //read the memory stream and convert to a byte array
                memStream.Position = 0;
                resultText = new byte[memStream.Length];
                memStream.Read(resultText, 0, resultText.Length);
            }
            //return our encrypted/decrypted text
            return resultText;
        }

        private static char GetRandomChar() {
            int i = 0;
            var rand = new Random();
            string chars = "1234567890!@#$%^&*()_+QWERTYUIOP{}|ASDFGHJKLZXCVBNM/<>?";
            int index = rand.Next(chars.Length);
            return chars[index];
        }
    }
}
