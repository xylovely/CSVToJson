using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SimpleJson;

/// <summary>
/// convert csv file to json
/// by 弦音
/// </summary>
namespace ConsoleApp
{
    class Program
    {
        //string inputPath = 
        /// <summary>
        /// csv file name，csv file path
        /// </summary>
        private static Dictionary<string, string> csvDic;

        /// <summary>
        /// csv file root path
        /// </summary>
        private static string csv_path = "";

        /// <summary>
        /// json file root path
        /// </summary>
        private static string json_path = "";

        /// <summary>
        /// 0:convert a csv file to a json file   1:convert all csv file to a json file
        /// </summary>
        private static int convert_type = 0;

        static void Main(string[] args)
        {
            foreach (var item in args)
            {
                //Console.WriteLine(item);
                convert_type = Convert.ToInt32(item);
            }

            Console.WriteLine("convert tyep:" + convert_type);
            Console.WriteLine();

            InitPath();

            ReadCSV();

            ConvertToJson();

            Console.ReadLine();
        }

        /// <summary>
        /// init file path
        /// </summary>
        private static void InitPath()
        {
            string app_path = AppDomain.CurrentDomain.BaseDirectory;

            csv_path = app_path.Replace("ConsoleApp\\ConsoleApp\\bin\\Debug\\", "csv");

            json_path = app_path.Replace("ConsoleApp\\ConsoleApp\\bin\\Debug\\", "json");

            Console.WriteLine("csv path: {0}", csv_path);
            Console.WriteLine();

            Console.WriteLine("json path: {0}", json_path);
            Console.WriteLine();
        }

        /// <summary>
        /// read csv file list
        /// </summary>
        private static void ReadCSV()
        {
            csvDic = new Dictionary<string, string>();

            Console.WriteLine("csv list：");

            if (Directory.Exists(csv_path))
            {
                DirectoryInfo direction = new DirectoryInfo(csv_path);
                FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    if (!csvDic.ContainsKey(files[i].Name))
                    {
                        csvDic.Add(files[i].Name, files[i].FullName);
                        Console.WriteLine("---> " + files[i].Name);
                    }
                }
            }
            else
            {
                Console.WriteLine("csv file path not exists！");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// convert csv to json
        /// </summary>
        private static void ConvertToJson()
        {
            JsonObject allCsvJson = new JsonObject();
            string allCsvList = "{\n";

            int index = 0;
            foreach (var item in csvDic)
            {
                JsonObject csvJson = new JsonObject();

                Console.WriteLine("convert： {0}", item.Key);
                Console.WriteLine();

                byte[] bytes = Encoding.Convert(Encoding.Default, Encoding.UTF8, FileManager.Read(item.Value));
                string str = bytes == null ? null : Encoding.UTF8.GetString(bytes);
                //Debug.Log("正在转换：" + str);
                //read a line content  
                string[] lineArray = str.Split("\r"[0]);

                string form_name = item.Key.Split('.')[0];
                csvJson.Add("form", form_name);

                //data type
                string[] data_types = lineArray[3].Split(',');

                //field
                string[] fields = lineArray[4].Split(',');


                JsonObject typeJson = new JsonObject();
                for (int i = 0; i < data_types.Length; i++)
                {
                    typeJson.Add(fields[i], data_types[i]);
                }
                csvJson.Add("field", typeJson);

                //Debug.Log("正在转换：" + lineArray.Length);


                List<string> alllineList = new List<string>();
                //a line as a group data
                for (int i = 5; i < lineArray.Length - 1; i++)
                {
                    JsonObject lineJson = new JsonObject();
                    string[] values = lineArray[i].Split(',');
                    for (int j = 0; j < values.Length; j++)
                    {
                        lineJson.Add(fields[j], values[j]);
                    }
                    //Debug.Log(lineJson.ToString().Replace("\\n",""));
                    alllineList.Add(lineJson.ToString().Replace("\\n", "").Trim());

                }
                //Debug.Log(alllineJson);

                csvJson.Add("data", alllineList);

                string fileListStr = ToFileListString(csvJson);
                fileListStr = fileListStr.Replace("\"{", "{");
                fileListStr = fileListStr.Replace("}\"", "}");
                fileListStr = fileListStr.Replace("\\\"", "\"");
                fileListStr = fileListStr.Replace("\\n", "");

                if (convert_type == 0)
                {
                    byte[] fileListBytes = Encoding.UTF8.GetBytes(fileListStr);
                    string fileOut = json_path + "/" + form_name + ".json";
                    FileManager.Write(fileOut, fileListBytes);
                }
                else
                {
                    allCsvList = allCsvList + "\"" + form_name + "\"" + ":" + fileListStr;
                    index++;
                    if (index < csvDic.Count)
                    {
                        allCsvList = allCsvList + ",\n";
                    }
                    else
                    {
                        allCsvList = allCsvList + "\n";
                    }
                    //allCsvList = allCsvList + "\"" + form_name + "\"" +":" + fileListStr + ",\n";
                }
                
            }

            if (convert_type == 1)
            {
                allCsvList = allCsvList + "}";
                byte[] fileListBytes = Encoding.UTF8.GetBytes(allCsvList);
                string fileOut = json_path + "/csv.json";
                FileManager.Write(fileOut, fileListBytes);
            }
            Console.WriteLine("convert finish！");
        }

        /// <summary>
        /// string deal
        /// </summary>
        /// <param name="fileListJo"></param>
        /// <returns></returns>
        private static string ToFileListString(JsonObject fileListJo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{\r\n");
            char[] chars = fileListJo.ToString().ToCharArray();
            int leftBracket = 0;
            for (int index = 1; index < chars.Length - 1; index++)
            {
                sb.Append(chars[index]);
                if (chars[index] == '{')
                {
                    leftBracket++;
                }
                if (chars[index] == '}')
                {
                    leftBracket--;
                }
                if (chars[index] == ',' && leftBracket == 0)
                {
                    sb.Append("\r\n");
                }
            }
            sb.Append("\r\n}");
            return sb.ToString();
        }
    }
}
