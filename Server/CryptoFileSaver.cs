﻿using SharedItems;
using System;
using System.IO;

namespace Server
{
    /// <summary>
    /// Saves file encrypted to savedData.txt
    /// </summary>
    public class CryptoFileSaver
    {
        private readonly string DEFAULT_FILENAME = "savedData.txt";
        private EncyptionService encyptionService;
        private string path;
        public CryptoFileSaver(string folderName)
        {
            encyptionService = new EncyptionService();

            string location = Environment.CurrentDirectory;
            char[] BLACKLIST = new char[] { '/', '\\', '<', '>', ':', '"', '|', '?', '*' };
            foreach (char c in BLACKLIST)
            {
                if (folderName.Contains(c))
                {
                    throw new ArgumentException($"foldername contains invallid character: {c}");
                }
            }
            path = $"{location}/{folderName}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public void WriteUserData(string json, string userName)
        {
            string location = $"{path}/{userName}";

            if (!Directory.Exists(location))
            {
                Directory.CreateDirectory(location);
            }

            try
            {
                if (json != null)
                {
                    using (StreamWriter streamWriter = new StreamWriter($"{location}/{DEFAULT_FILENAME}", false))
                    {
                        int length = json.Length;
                        int amountOfCharactersNeeded = 16 - length % 16;
                        for (int i = 0; i < amountOfCharactersNeeded; i++)
                        {
                            json += '^';
                        }

                        byte[] cypher = encyptionService.EncryptStringToBytes(json);
                        string base64cypher = Convert.ToBase64String(cypher);
                        streamWriter.Write(base64cypher);
                        streamWriter.Flush();
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        public string[] GetSavedUsers()
        {
            try
            {
                string[] directories = Directory.GetDirectories(path);
                string[] users = new string[directories.Length];
                for (int i = 0; i < directories.Length; i++)
                {
                    using (StreamReader streamReader = new StreamReader($"{directories[i]}/{DEFAULT_FILENAME}"))
                    {
                        string cypher = streamReader.ReadToEnd();
                        string user = encyptionService.DecryptStringFromBytes(Convert.FromBase64String(cypher));
                        int place = user.IndexOf('^');
                        if (place >= 0)
                        {
                            user = user.Remove(place);
                        }

                        users[i] = user;
                    }
                }

                return users;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return null;
        }
    }
}
