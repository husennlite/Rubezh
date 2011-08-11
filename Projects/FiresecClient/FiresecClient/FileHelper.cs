﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace FiresecClient
{
    public class FileHelper
    {
        public FileHelper()
        {
            _directoriesList = new List<string>() { "Sounds" };
        }

        List<string> _directoriesList;

        string CurrentDirectory(string directory)
        {
            return Directory.GetCurrentDirectory() + @"\" + directory;
        }

        void SynchronizeDirectory(string directory)
        {
            var filesDirectory = Directory.CreateDirectory(CurrentDirectory(directory));
            var localDirectoryHash = GetDirectoryHash(directory);
            var remoteDirectoryHash = FiresecManager.GetHashAndNameFiles(directory);

            foreach (var remoteFileHash in remoteDirectoryHash)
            {
                if (localDirectoryHash.ContainsKey(remoteFileHash.Key) == false)
                {
                    DownloadFile(filesDirectory.Name + @"\" + remoteFileHash.Value, filesDirectory.FullName + @"\" + remoteFileHash.Value);
                }
            }

            foreach (var localFileHash in localDirectoryHash)
            {
                if (remoteDirectoryHash.ContainsKey(localFileHash.Key) == false)
                {
                    File.Delete(filesDirectory.FullName + localFileHash.Value);
                }
            }
        }

        void DownloadFile(string directoryAndFileName, string destinationPath)
        {
            Stream stream = FiresecManager.GetFile(directoryAndFileName);
            FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(destinationStream);
            destinationStream.Close();
        }

        Dictionary<string, string> GetDirectoryHash(string directory)
        {
            Dictionary<string, string> hashTable = new Dictionary<string, string>();
            List<string> HashListFiles = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(CurrentDirectory(directory));
            FileInfo[] files = dir.GetFiles();
            byte[] hash;
            StringBuilder sBuilder = new StringBuilder();
            foreach (FileInfo fInfo in files)
            {
                sBuilder.Clear();
                using (FileStream fileStream = fInfo.Open(FileMode.Open))
                {
                    hash = MD5.Create().ComputeHash(fileStream);
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sBuilder.Append(hash[i].ToString());
                    }
                }
                hashTable.Add(sBuilder.ToString(), fInfo.Name);
            }
            return hashTable;
        }
        
        List<string> GetFileNamesList(string directory)
        {
            List<string> fileNames = new List<string>();
            foreach (var str in Directory.GetFiles(CurrentDirectory(directory)))
            {
                fileNames.Add(Path.GetFileName(str));
            }
            return fileNames;
        }

        public void Synchronize()
        {
            foreach (var directory in _directoriesList)
            {
                SynchronizeDirectory(directory);
            }
        }

        public List<string> GetListSounds
        {
            get { return GetFileNamesList(_directoriesList[0]); }
        }

        public string GetFilePath(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return null;
            }
            else
            {
                return CurrentDirectory(_directoriesList[0]) + @"\" + file;
            }
        }
    }
}
