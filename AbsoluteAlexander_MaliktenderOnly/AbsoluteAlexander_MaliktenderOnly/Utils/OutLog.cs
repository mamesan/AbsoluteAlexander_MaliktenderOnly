using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AbsoluteAlexander_MaliktenderOnly
{
    class OutLog
    {

        private void button1_Click(object sender, EventArgs e)
        {
            StackTrace stt = new StackTrace(false);
            String cls = stt.GetFrame(0).GetMethod().ReflectedType.FullName;
            String mth = stt.GetFrame(0).GetMethod().Name;

            try
            {
                WriteTraceLog(cls + "." + mth + "開始");
                // ゼロ割り算Exceptionを発生させる
                int a = 0;
                a = 1 / a;
                WriteTraceLog(cls + "." + mth + "終了");
            }
            catch (Exception ex)
            {
                WriteTraceLog(cls + "." + mth + "でエラーが発生しました", ex);
            }
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="msg">メッセージ</param>
        /// <remarks></remarks>
        public static void WriteTraceLog(String msg, String FolderPath, String FileName)
        {
            WriteTraceLog(msg, FolderPath, FileName, null);
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="msg">メッセージ</param>
        /// <param name="ex">Exception(無指定の場合はメッセージのみ出力)</param>
        /// <remarks></remarks>
        private static void WriteTraceLog(String msg, String FolderPath, String FileName, Exception ex)
        {
            try
            {
                // ログフォルダ名作成
                // ここで、logの出力先を管理している
                String logFolder = @FolderPath + "Log";


                // ログフォルダ名作成
                Directory.CreateDirectory(logFolder);

                // ログファイル名作成
                String logFile = logFolder + "\\" + FileName + ".log";

                // ログ出力文字列作成
                /*
                String logStr;
                logStr = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\t" + msg;
                if (ex != null)
                {
                    logStr = logStr + "\n" + ex.ToString();
                }
                */

                String logStr = msg;

                // Shift-JISでログ出力
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(logFile, true,
                        Encoding.GetEncoding("UTF-8"));
                    sw.WriteLine(logStr);
                }
                catch (Exception ex1)
                {
                    Console.WriteLine(ex1);
                }
                finally
                {
                    if (sw != null) sw.Close();
                }
            }
            catch { }
        }

    }
}
