using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class FileManager
    {
        public static byte[] Read(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("File is not exist: " + path);
                Console.WriteLine();
                return null;
            }

            FileStream fs = null;
            MemoryStream ms = null;
            try
            {
                //Console.WriteLine("Read: " + path);
                //Console.WriteLine();
                FileInfo file = new FileInfo(path);
                fs = file.OpenRead();
                ms = new MemoryStream();
                byte[] bytesTemp = new byte[4096];
                int readLength = 0;
                while ((readLength = fs.Read(bytesTemp, 0, 4096)) > 0)
                {
                    ms.Write(bytesTemp, 0, readLength);
                }
                ms.Flush();
                return ms.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine("Read [" + path + "] error: " + e);
                Console.WriteLine();
                return null;
            }
            finally
            {
                if (ms != null)
                {
                    ms.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }



        public static void Write(string path, byte[] bytes)
        {
            FileStream fs = null;
            try
            {
                //Console.WriteLine("Write: " + path);
                //Console.WriteLine();
                FileInfo file = GetFile(path);
                fs = file.Open(FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("Write [" + path + "] error: " + e);
                Console.WriteLine();
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }


        private static FileInfo GetFile(string path)
        {
            FileInfo file = new FileInfo(path);
            string directoryName = file.DirectoryName;
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            return file;
        }
    }
}
