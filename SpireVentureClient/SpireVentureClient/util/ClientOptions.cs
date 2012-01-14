using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace SpireVenture.util
{
    class ClientOptions
    {
        public String Username { get; private set; }
        public String Keyword { get; private set; }
        public String HashedCombo { get; private set; }
        Dictionary<string, string> optionsDict;
        private bool changed = false;
        private static ClientOptions instance;

        // i'm a singleton!
        public static ClientOptions Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClientOptions();
                }
                return instance;
            }
        }

        public void initialize() { /* this exists so that i can control when the instance is initialized */ }

        private ClientOptions()
        {
            // load from flat file
            String documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String clientPath = Path.Combine(documentsPath, "SpireVenture");
            String optionsFilePath = Path.Combine(clientPath, "options.txt");

            optionsDict = new Dictionary<string, string>();

            if (!Directory.Exists(clientPath))
                Directory.CreateDirectory(clientPath);

            if (File.Exists(optionsFilePath))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(optionsFilePath))
                    {
                        String line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] pair = line.Split(':');
                            optionsDict.Add(pair[0], pair[1]);
                        }
                    }
                }
                catch (Exception e)
                {
                    // Let the user know what went wrong.
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }

                foreach (string key in optionsDict.Keys)
                {
                    switch (key)
                    {
                        case "username":
                            Username = optionsDict["username"];
                            break;
                        case "keyword":
                            Keyword = optionsDict["keyword"];
                            break;
                    }
                }
            }
        }

        public void Save()
        {
            if (changed)
            {
                String documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                String clientPath = Path.Combine(documentsPath, "SpireVenture");
                String optionsFilePath = Path.Combine(clientPath, "options.txt");

                if (!Directory.Exists(clientPath))
                    Directory.CreateDirectory(clientPath);

                File.Delete(optionsFilePath); // delete if there, doesn't throw error if not

                StringBuilder sb = new StringBuilder();
                foreach (string key in optionsDict.Keys)
                {
                    sb.AppendLine(key + ":" + optionsDict[key]);
                }

                using (StreamWriter outfile = new StreamWriter(optionsFilePath))
                    outfile.Write(sb.ToString());

                changed = false;

                Console.Write("wrote");
            }
        }

        public void setKeyword(String word)
        {
            changed = true;
            Keyword = word;
            if (optionsDict.ContainsKey("keyword"))
            {
                optionsDict["keyword"] = word;
            }
            else
            {
                optionsDict.Add("keyword", word);
            }
        }

        public void setUsername(String word)
        {
            changed = true;
            Username = word;
            if (optionsDict.ContainsKey("username"))
            {
                optionsDict["username"] = word;
            }
            else 
            {
                optionsDict.Add("username", word);
            }
        }
    }
}
