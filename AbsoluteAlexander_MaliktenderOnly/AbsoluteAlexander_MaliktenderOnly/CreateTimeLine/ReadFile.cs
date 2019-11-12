using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AbsoluteAlexander_MaliktenderOnly.CreateTimeLine
{
    class ReadFile
    {

        /// <summary>
        /// アビリストを取得する
        /// jsonファイルの読み込み処理
        /// </summary>
        /// <param name="jobList"></param>
        /// <returns></returns>
        public static List<string> AbiList_create(List<string> jobList)
        {
            List<string> AbiList = new List<string>();
            string path = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\AbiList.json");
            StreamReader sr = new StreamReader(@path, Encoding.GetEncoding("UTF -8"));
            var obj = JObject.Parse(@sr.ReadToEnd());
            foreach (string jobName in jobList)
            {
                AbiList.AddRange(jsonTest(jobName, obj));
            }
            return AbiList;
        }

        /// <summary>
        /// jsonを解析する
        /// </summary>
        /// <param name="job"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static List<string> jsonTest(string job, JObject obj)
        {
            List<string> list = new List<string>();
            foreach (string str in obj[job])
            {
                list.Add(str);
            }
            return list;
        }
    }
}
