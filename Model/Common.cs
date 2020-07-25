using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication10.Model
{
    public static class Common
    {
        public static T ReadJson<T>(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                var str = sr.ReadToEnd();
                var data = JsonConvert.DeserializeObject<T>(str);
                return data;
            }
        }
        public static void SaveJson(string path, string data)
        {
            using (StreamWriter sr = new StreamWriter(path))
            {
                sr.WriteLine(data);
                sr.Flush();
                sr.Close();
            }
        }
    }
}
