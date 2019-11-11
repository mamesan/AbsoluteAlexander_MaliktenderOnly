using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AbsoluteAlexander_MaliktenderOnly.CreateTimeLine
{
    class ReadFile
    {
        /// <summary>
        /// アビリストを取得する
        /// </summary>
        public static List<string> AbiList_create()
        {
            List<string> AbiList = new List<string>();
            try
            {
                string path = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\");

                StreamReader sr = new StreamReader(Path.Combine(@path + "AbiList.txt"), Encoding.GetEncoding("UTF-8"));

                while (sr.Peek() != -1)
                {
                    // コメントアウト対応
                    if (!sr.ReadLine().Contains("#"))
                    {
                        string str22 = sr.ReadLine();
                        AbiList.Add(str22);
                    }
                }
            }
            catch { }
            return AbiList;
        }
    }
}
