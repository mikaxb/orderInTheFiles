using System;
using static System.Console;
using System.IO;
using ExifLib;
namespace PhotoChaosOrganizer
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Starting photo organizer.");
            WriteLine("Enter start path:");
            var path = ReadLine();
            WriteLine("Search for path: " + path);
            if (Directory.Exists(path))
            {
                WriteLine("Directory found");
                try
                {
                    var targetDir = path + @"\Organized";
                    Directory.CreateDirectory(targetDir);
                    var iterator = new FileIterator();
                    iterator.IterateDirectory(path, targetDir);
                }
                catch (Exception ex)
                {
                    Logger.Log("#IterateDirectory TOP: " + DateTime.Now + Environment.NewLine + ex.ToString());
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
    public static class Logger
    {
        private static string filePath = @"d:\chaoslog.txt";


        public static void Log(object tolog)
        {
            try
            {
                using (var stream = File.AppendText(filePath))
                {
                    stream.WriteLine(DateTime.Now.ToString());
                    stream.WriteLine(tolog.ToString());
                }
            }
            catch (Exception) { }
        }

    }


    public class FileIterator
    {


        public void IterateDirectory(string path, string targetPath)
        {
            var files = Directory.EnumerateFiles(path);
            foreach (string filePath in files)
            {
                try
                {
                    var info = new FileInfo(filePath);
                    if (info.Extension.ToLower() == ".jpg")
                    {
                        try
                        {
                            var year = "";
                            var month = "";
                            var day = "";
                            DateTime datePictureTaken;
                            var hasdate = false;
                            using (ExifReader reader = new ExifReader(filePath))
                            {
                                if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized, out datePictureTaken))
                                {
                                    hasdate = true;
                                    WriteLine("READY TO MOVE: " + filePath);
                                    Logger.Log("READY TO MOVE: " + filePath);
                                }
                            }
                            if (hasdate)
                            {
                                year = datePictureTaken.Year.ToString();
                                month = datePictureTaken.Month.ToString();
                                if (month.Length == 1)
                                {
                                    month = "0" + month;
                                }
                                day = datePictureTaken.Day.ToString();
                                if (day.Length == 1)
                                {
                                    day = "0" + day;
                                }
                                var targetdir = targetPath + $@"\{year}-{month}-{day}";
                                try
                                {
                                    Directory.CreateDirectory(targetdir);
                                    File.Move(filePath, targetdir + @"\" + info.Name);
                                }
                                catch (Exception ex)
                                {
                                    WriteLine(ex);
                                    Logger.Log(ex);
                                }
                            }
                            else
                            {
                                WriteLine("LEAVING: " + filePath);
                                Logger.Log("LEAVING: " + filePath);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteLine(ex);
                            Logger.Log(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteLine(ex);
                    Logger.Log("#File loop");
                    Logger.Log(filePath);
                    Logger.Log(ex);
                }
            }
        }


    }

}
