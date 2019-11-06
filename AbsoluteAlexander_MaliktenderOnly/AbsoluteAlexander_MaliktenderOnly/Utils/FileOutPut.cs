using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AbsoluteAlexander_MaliktenderOnly.Utils
{
    class FileOutPut
    {

        /// <summary>
        /// Mob情報を取得する
        /// </summary>
        public static void GetMobInfo(string FileName, string FileOutPutPath)
        {
            // ファイルを出力する
            FilePush(createCombartList(), FileName, FileOutPutPath);

        }
        /// <summary>
        /// ログを出力するメソッド
        /// </summary>
        /// <param name="MobName"></param>
        /// <param name="MaxHp"></param>
        /// <param name="CurrentHP"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        private static void FilePush(List<CombertBean> CombertBeanList, string FileName, string FileOutPutPath)
        {
            string FileOutPath = @FileOutPutPath;
            // ファイル出力先を作成する
            if (!(FileOutPath.Substring(FileOutPath.Length - 1)).Equals("\\"))
            {
                FileOutPath = FileOutPath + "\\";
            }


            // ファイルの存在チェックを実施し、存在しない名前になるまで連番作成を行う
            int i = 1;

            string Path = FileOutPath + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + FileName;
            while (File.Exists(Path + i + ".txt"))
            {
                i++;
            }
            Path = Path + i + ".txt";

            string OutPutString = createMobInfoString(CombertBeanList);

            // UTF - 8で書き込む
            //書き込むファイルが既に存在している場合は、上書きする
            StreamWriter sw = new StreamWriter(
                @Path,
                false,
                Encoding.GetEncoding("UTF-8"));
            //内容を書き込む
            sw.Write(OutPutString);
            //閉じる
            sw.Close();
        }
        /// <summary>
        /// 指定したパスにディレクトリが存在しない場合
        /// すべてのディレクトリとサブディレクトリを作成します
        /// </summary>
        private static DirectoryInfo SafeCreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return null;
            }
            return Directory.CreateDirectory(path);
        }

        public static string createMobInfoString(List<CombertBean> CombertBeanList)
        {
            string OutPutString = "";
            string 改行 = "\r\n";

            foreach (CombertBean combertBean in CombertBeanList)
            {
                OutPutString += "ID：" + combertBean.ID + 改行;
                OutPutString += "名前：" + combertBean.Name + 改行;
                OutPutString += "X：" + combertBean.X + 改行;
                OutPutString += "Y：" + combertBean.Y + 改行;
                OutPutString += "Z：" + combertBean.Z + 改行;
                OutPutString += "Job：" + Job.Instance.GetJobName(combertBean.Job) + 改行;
                OutPutString += "MaxHp：" + combertBean.MaxHp + 改行;
                OutPutString += "CurrentHP：" + combertBean.CurrentHP + 改行;
                OutPutString += "IsCasting：" + combertBean.IsCasting + 改行;
                OutPutString += "OwnerID：" + combertBean.OwnerID + 改行;
                OutPutString += "type：" + combertBean.type + 改行;
                OutPutString += "Level：" + combertBean.Level + 改行;
                OutPutString += 改行;
            }
            return OutPutString;
        }

        public static List<CombertBean> createCombartList()
        {

            dynamic list = ActHelper.GetCombatantList();

            List<CombertBean> CombertBeanList = new List<CombertBean>();

            foreach (dynamic item in list.ToArray())
            {
                if (item == null)
                {
                    continue;
                }

                var combatant = new Combatant();
                combatant.Name = (string)item.Name;
                combatant.ID = (uint)item.ID;
                combatant.Job = (int)item.Job;
                combatant.IsCasting = (bool)item.IsCasting;
                combatant.OwnerID = (uint)item.OwnerID;
                combatant.type = (byte)item.type;
                combatant.Level = (int)item.Level;
                combatant.CurrentHP = (int)item.CurrentHP;
                combatant.MaxHP = (int)item.MaxHP;
                combatant.PosX = (float)item.PosX;
                combatant.PosY = (float)item.PosY;
                combatant.PosZ = (float)item.PosZ;

                CombertBean combertBean = new CombertBean();
                combertBean.Name = combatant.Name.ToString();
                combertBean.ID = combatant.ID;
                combertBean.MaxHp = combatant.MaxMP;
                combertBean.CurrentHP = combatant.CurrentHP;
                combertBean.Job = combatant.Job;
                combertBean.IsCasting = combatant.IsCasting;
                combertBean.OwnerID = combatant.OwnerID;
                combertBean.type = combatant.type;
                combertBean.Level = combatant.Level;
                combertBean.X = combatant.PosX.ToString();
                combertBean.Y = combatant.PosY.ToString();
                combertBean.Z = combatant.PosZ.ToString();

                CombertBeanList.Add(combertBean);
            }
            return CombertBeanList;
        }

    }
    class CombertBean
    {
        public String Name;
        public int MaxHp;
        public int CurrentHP;
        public uint ID;
        public int Job;
        public bool IsCasting;
        public uint OwnerID;
        public byte type;
        public int Level;
        public String X;
        public String Y;
        public String Z;
    }
}
