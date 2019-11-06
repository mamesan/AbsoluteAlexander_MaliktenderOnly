using Advanced_Combat_Tracker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                string path = "D:\\Actor_v2.0.4.2\\ACT";

                StreamReader sr = new StreamReader(Path.Combine(@path + "\\lib\\AbiList.txt"), Encoding.GetEncoding("UTF-8"));

                while (sr.Peek() != -1)
                {
                    // コメントアウト対応
                    if (!sr.ReadLine().Contains("#"))
                    {
                        AbiList.Add(sr.ReadLine());
                    }
                }
            }
            catch { }
            return AbiList;
        }

    }
}
