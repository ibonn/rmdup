using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace rmdup
{
    class Program
    {
        private static string md5hash(byte[] bytes)
        {
            MD5 md5 = MD5.Create();
            return hash(bytes, md5);
        }

        private static string sha1hash(byte[] bytes)
        {
            SHA1 sha1 = SHA1.Create();
            return hash(bytes, sha1);
        }

        private static string hash(byte[] bytes, HashAlgorithm algo)
        {
            byte[] hashBytes = algo.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                sb.Append(hashBytes[i].ToString("X2"));
            return sb.ToString();
        }

        private static Dictionary<string, string> hashPathMap = new Dictionary<string, string>();

        private static void rmdup(string path)
        {
            foreach (string file in Directory.GetFiles(path))
            {
                byte[] bytes = File.ReadAllBytes(file);
                // Currently hashes using sha1
                string hash = sha1hash(bytes);
                if (hashPathMap.ContainsKey(hash))
                {
                    // Remove duplicates (Preserve shortest path)
                    string duplicate = hashPathMap[hash];
                    if (duplicate.Length > file.Length)
                    {
                        File.Delete(duplicate);
                        Console.WriteLine(duplicate + " removed.");
                        hashPathMap[hash] = file;
                    }
                    else
                    {
                        File.Delete(file);
                        Console.WriteLine(file + " removed.");
                    }
                }
                else
                {
                    hashPathMap.Add(hash, file);
                }
                
            }
            foreach (string dir in Directory.GetDirectories(path))
            {
                rmdup(dir);
            }
        }

        static void Main(string[] args)
        {
            args = new string[1];
            args[0] = "F:\\";
            if (args.Length == 0)
            {
                Console.WriteLine("A path must be specified");
            }
            else
            {
                rmdup(args[0]);
            }
            Console.ReadLine();
        }
    }
}
