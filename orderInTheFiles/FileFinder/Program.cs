using System;
using static System.Console;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using ExifLib;

namespace FileFinder
{

    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Starting organizer.");
            WriteLine("Enter start path:");
            var path = ReadLine();
            WriteLine("Search for path: " + path);
            if (Directory.Exists(path))
            {
                WriteLine("Directory found");
                try
                {
                    var iterator = new FileIterator();
                    iterator.IterateDirectory(path);
                }
                catch (Exception ex)
                {
                    WriteLine(ex);
                }
            }
            else
            {
                WriteLine("Directory not found at: " + path);
            }


            WriteLine("Done press enter to exit.");
            ReadLine();
        }
    }


    public class FileIterator
    {

        private static string connectionString = "DESKTOP-2JVC76M\SQLEXPRESS";
        public void IterateDirectory(string path)
        {
            var subDirs = Directory.EnumerateDirectories(path);
            foreach (string dirPath in subDirs)
            {
                IterateDirectory(dirPath);
            }
            var files = Directory.EnumerateFiles(path);
            foreach (string filePath in files)
            {
                var ffInfo = GenerateFileFinderInfo(filePath, path);
            }
        }

        private void SaveInfoToDB(FileFinderInfo ffInfo)
        {

        }

        private FileFinderInfo GenerateFileFinderInfo(string filePath, string dirPath)
        {
            WriteLine(filePath);
            var ffInfo = new FileFinderInfo();
            ffInfo.DirPath = dirPath;
            ffInfo.FullPath = filePath;
            using (var md5 = MD5.Create())
            {
                using (var str = File.OpenRead(filePath))
                {
                    var hash = md5.ComputeHash(str);
                    ffInfo.Checksum = ToHex(hash, false);
                }
            }
            var info = new FileInfo(filePath);
            ffInfo.Suffix = info.Extension;
            ffInfo.Name = info.Name;
            ffInfo.SizeInBytes = info.Length;
            ffInfo.CreationTime = info.CreationTime;

            if (ffInfo.Suffix == ".jpg")
            {
                using (ExifReader reader = new ExifReader(filePath))
                {
                    if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out DateTime datePictureTaken))
                    {
                        ffInfo.exif_datetaken = datePictureTaken;
                    }
                }
            }
            return ffInfo;
        }

        public static string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }
    }

    public class FileFinderInfo
    {
        public string Name { get; set; }
        public string DirPath { get; set; }
        public string FullPath { get; set; }
        public string Checksum { get; set; }
        public string Suffix { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? exif_datetaken { get; set; }
        public long SizeInBytes { get; set; }

    }
}
