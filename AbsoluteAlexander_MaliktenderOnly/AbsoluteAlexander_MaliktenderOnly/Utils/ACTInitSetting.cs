using Advanced_Combat_Tracker;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace AbsoluteAlexander_MaliktenderOnly
{
    /// <summary>
    /// ファイル操作を実施するクラス
    /// </summary>
    static class ACTInitSetting
    {

        // 設定ファイル保存先パス
        static String settingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\AbsoluteAlexander_MaliktenderOnly.config.xml");

        /// <summary>
        /// 保存されている設定を読み込む
        /// </summary>
        /// <param name="xmlSettings"></param>
        public static void LoadSettings(SettingsSerializer xmlSettings)
        {
            if (File.Exists(settingsFile))
            {
                FileStream fs = new FileStream(settingsFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                XmlTextReader xReader = new XmlTextReader(fs);

                try
                {
                    while (xReader.Read())
                    {
                        if (xReader.NodeType == XmlNodeType.Element)
                        {
                            if (xReader.LocalName == "SettingsSerializer")
                            {
                                xmlSettings.ImportFromXml(xReader);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
                xReader.Close();
            }
            else
            {
                // XMLを作成する
                CreateXML();
            }
        }

        /// <summary>
        /// 設定を保存する
        /// </summary>
        /// <param name="xmlSettings"></param>        
        public static void SaveSettings(SettingsSerializer xmlSettings)
        {
            FileStream fs = new FileStream(settingsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            XmlTextWriter xWriter = new XmlTextWriter(fs, Encoding.UTF8);
            xWriter.Formatting = Formatting.Indented;
            xWriter.Indentation = 1;
            xWriter.IndentChar = '\t';
            xWriter.WriteStartDocument(true);
            xWriter.WriteStartElement("Config");    // <Config>
            xWriter.WriteStartElement("SettingsSerializer");    // <Config><SettingsSerializer>
            xmlSettings.ExportToXml(xWriter);   // Fill the SettingsSerializer XML
            xWriter.WriteEndElement();  // </SettingsSerializer>
            xWriter.WriteEndElement();  // </Config>
            xWriter.WriteEndDocument(); // Tie up loose ends (shouldn't be any)
            xWriter.Flush();    // Flush the file buffer to disk
            xWriter.Close();
        }

        /// <summary>
        /// 全てのユーザコントロール情報を取得する
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public static Control[] GetAllControls(Control top)
        {
            ArrayList buf = new ArrayList();
            foreach (Control c in top.Controls)
            {
                buf.Add(c);
                buf.AddRange(GetAllControls(c));
            }
            return (Control[])buf.ToArray(typeof(Control));
        }


        /// <summary>
        /// 新しくXMLを生成する
        /// </summary>
        private static void CreateXML()
        {
            String str = ActGlobals.oFormActMain.AppDataFolder.FullName + "\\AbsoluteAlexander_MaliktenderOnly.config.xml";

            // UTF - 8で書き込む
            //書き込むファイルが既に存在している場合は、上書きする
            StreamWriter sw = new StreamWriter(
                @str,
                false,
                Encoding.GetEncoding("UTF-8"));
            //内容を書き込む
            sw.Write("");
            //閉じる
            sw.Close();

            FileStream fs = new FileStream(settingsFile, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite);
            XmlTextWriter xWriter = new XmlTextWriter(fs, Encoding.UTF8);
            xWriter.Formatting = Formatting.Indented;
            xWriter.Indentation = 1;
            xWriter.IndentChar = '\t';
            xWriter.WriteStartDocument(true);
            xWriter.WriteStartElement("Config");    // <Config>
            xWriter.WriteEndElement();  // </Config>
            xWriter.WriteEndDocument(); // Tie up loose ends (shouldn't be any)
            xWriter.Flush();    // Flush the file buffer to disk

        }
        /// <summary>
        /// アビリティリストが存在するかチェックを行う
        /// なければGithubよりダウンロードを行う
        /// </summary>
        public static void CheckAbiText()
        {
            string AbiTxtFilePath = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\");
            if (!File.Exists(AbiTxtFilePath + "AbiList.json"))
            {
                WebClient wc = new WebClient();
                wc.DownloadFile("https://raw.githubusercontent.com/mamesan/AbsoluteAlexander_MaliktenderOnly/master/AbsoluteAlexander_MaliktenderOnly/AbsoluteAlexander_MaliktenderOnly/lib/AbiList.json", AbiTxtFilePath + "AbiList.json");
                wc.Dispose();
            }
        }
    }
}