using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DESImplementation {
    class Program {
        private static DESCryptoServiceProvider des;

        static void Main(string[] args) {
            string text = "SIABIRIS";
            string key = "F46Ε986435465354";
            //string key = "54";
            string initVector = "ABCDEFGH";

            var KeyList = new List<string>();
            //the key size is equivalent to 136 bits
            //so we split it into 2 64 bit keys
            //and one key with the last 8 bits and a random character to fill out the required 64 bit key for the DES algorithm
            KeyList.Add("F46Ε986");
            KeyList.Add("43546535");
            KeyList.Add("40000000");

            foreach (var splitKey in KeyList) {
                text = Encrypt(text, splitKey, initVector);
                Console.WriteLine(text);
            }

            foreach(var splitKey in KeyList) {
                //text = Decrypt(text, splitKey, initVector);
            }

            
            string message = "";
        }
        
        private static string Encrypt(string text, string keyValue, string initializationVector) {
            des = new DESCryptoServiceProvider();
            byte[] input = Encoding.UTF8.GetBytes(text);
            byte[] output = Transform(input, des.CreateEncryptor(Encoding.UTF8.GetBytes(keyValue), Encoding.UTF8.GetBytes(initializationVector)));
            return Convert.ToBase64String(output);
        }

        private static string Decrypt(string text, string keyValue, string initializationVector) {
            des = new DESCryptoServiceProvider();
            byte[] input = Convert.FromBase64String(text);
            byte[] output = Transform(input, des.CreateDecryptor(Encoding.UTF8.GetBytes(keyValue), Encoding.UTF8.GetBytes(initializationVector)));
            return Encoding.UTF8.GetString(output);
        }

        private static byte[] Transform(byte[] input, ICryptoTransform cryptoTransform) {
            // Create the necessary streams
            // Transform the bytes as requesed

            byte[] encryptedText;
            using (var memStream = new MemoryStream())
            using (var crypStream = new CryptoStream(memStream, cryptoTransform, CryptoStreamMode.Write)) {


                //encrypt our input and load it in the memory stream
                crypStream.Write(input, 0, input.Length);
                crypStream.FlushFinalBlock();
            
                //read the memory stream and convert to a byte array
                memStream.Position = 0;
                encryptedText = new byte[memStream.Length];
                memStream.Read(encryptedText, 0, encryptedText.Length);

                // Clean up
                memStream.Close();
                crypStream.Close();
            }
            //
            return encryptedText;
        }


        private static string CharListToString(List<char> charList) {
            string result = "";
            foreach(var symbol in charList) {
                result += symbol;
            }
            return result;
        }
    }
}
