using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace Util.util
{
    public class ClientOptions
    {
        public String Username{ get; private set; }
        public String Keyword { get; private set; }
        public String HashedCombo { get; private set; }
        public int ResolutionHeight { get; private set; }
        public int ResolutionWidth { get; private set; }
        public bool Fullscreen { get; private set; }
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
                        case "resolutionH":
                            ResolutionHeight = Convert.ToInt32(optionsDict["resolutionH"]);
                            break;
                        case "resolutionW":
                            ResolutionWidth = Convert.ToInt32(optionsDict["resolutionW"]);
                            break;
                        case "fullscreen":
                            Fullscreen = Convert.ToBoolean(optionsDict["fullscreen"]);
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
                optionsDict["keyword"] = word;
            else
                optionsDict.Add("keyword", word);
        }

        public void setUsername(String word)
        {
            changed = true;
            Username = word;
            if (optionsDict.ContainsKey("username"))
                optionsDict["username"] = word;
            else
                optionsDict.Add("username", word);
        }

        public void setResolution(int H, int W)
        {
            changed = true;
            ResolutionHeight = H;
            ResolutionWidth = W;
            if (optionsDict.ContainsKey("resolutionH"))
                optionsDict["resolutionH"] = Convert.ToString(H);
            else
                optionsDict.Add("resolutionH", Convert.ToString(H));
            if (optionsDict.ContainsKey("resolutionW"))
                optionsDict["resolutionW"] = Convert.ToString(W);
            else
                optionsDict.Add("resolutionW", Convert.ToString(W));
        }

        public void setFullscreen(bool full)
        {
            changed = true;
            Fullscreen = full;
            if (optionsDict.ContainsKey("fullscreen"))
                optionsDict["fullscreen"] = Convert.ToString(full);
            else
                optionsDict.Add("fullscreen", Convert.ToString(full));
        }
    }
}
