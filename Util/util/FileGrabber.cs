using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Util.util
{
    public class FileGrabber
    {
        public static PlayerSave getPlayerSave(bool isSingleplayer, string playerName, string keyword)
        {
            String directory;

            if (isSingleplayer)
            {
                String documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                directory = Path.Combine(documentsPath, "SpireVenture");
            }
            else
            {
                String currentPath = Directory.GetCurrentDirectory();
                directory = Path.Combine(currentPath, "Saves");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }

            String savename = Path.Combine(directory, playerName + ".sav");
            if (File.Exists(savename))
            {
                Stream streamRead = File.OpenRead(savename);
                BinaryFormatter binaryRead = new BinaryFormatter();
                PlayerSave player = (PlayerSave)binaryRead.Deserialize(streamRead);
                streamRead.Close();
                return player;
            }
            else
            {
                PlayerSave playerSave = new PlayerSave(playerName, keyword);
                Stream streamWrite = File.Create(savename);
                BinaryFormatter binaryWrite = new BinaryFormatter();
                binaryWrite.Serialize(streamWrite, playerSave);
                streamWrite.Close();
                return playerSave;
            }
        }

        public static void SavePlayer(bool isSingleplayer, PlayerSave player)
        {
            String directory;

            if (isSingleplayer)
            {
                String documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                directory = Path.Combine(documentsPath, "SpireVenture");
            }
            else
            {
                String currentPath = Directory.GetCurrentDirectory();
                directory = Path.Combine(currentPath, "Saves");

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
            }

            String savename = Path.Combine(directory, player.Username + ".sav");

            Stream streamWrite = File.Create(savename);
            BinaryFormatter binaryWrite = new BinaryFormatter();
            binaryWrite.Serialize(streamWrite, player);
            streamWrite.Close();
        }

        public static string[] findLocalProfiles()
        {
            String documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String clientPath = Path.Combine(documentsPath, "SpireVenture");

            return Directory.GetFiles(clientPath, "*.sav");
        }

        public static void createNewProfile(string name)
        {
            String documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String directory = Path.Combine(documentsPath, "SpireVenture");
            String profileFilePath = Path.Combine(directory, name + ".sav");

            PlayerSave save = new PlayerSave(name, "local");

            Stream streamWrite = File.Create(profileFilePath);
            BinaryFormatter binaryWrite = new BinaryFormatter();
            binaryWrite.Serialize(streamWrite, save);
            streamWrite.Close();
        }
    }
}
