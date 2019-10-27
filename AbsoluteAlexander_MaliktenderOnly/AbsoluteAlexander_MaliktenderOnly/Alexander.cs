using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Advanced_Combat_Tracker;
using System.IO;

namespace AbsoluteAlexander_MaliktenderOnly
{
    public partial class Alexander : UserControl, IActPluginV1
    {
        private SettingsSerializer xmlSettings;
        // チェックの入った物をすべて格納しておく
        private List<string> scanList = new List<string>();
        private string fileFullPath_main_local_scan;
        // 選択アイテム名を格納する
        private string select_item_name = "";

        public Alexander()
        {
            InitializeComponent();
            ActHelper.Initialize();
        }

        public void DeInitPlugin()
        {
            // ACT終了時の処理
            // throw new NotImplementedException();
        }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            //lbStatus = pluginStatusText;   // Hand the status label's reference to our local var
            pluginScreenSpace.Controls.Add(this);   // Add this UserControl to the tab ACT provides
            this.Dock = DockStyle.Fill; // Expand the UserControl to fill the tab's client space
                                        // MultiProject.BasePlugin.xmlSettings = new SettingsSerializer(this); // Create a new settings serializer and pass it this instance

            pluginScreenSpace.Text = "アレキ";
            pluginStatusText.Text = "AbsoluteAlexanderPluginStarts";

            // インターフェイス情報を格納する
            this.xmlSettings = new SettingsSerializer(this);

            // コントロール情報を取得する
            Control[] ct = ACTInitSetting.GetAllControls(this);

            // 取得したコントロール情報を全て回し、初期表示用の情報を格納する
            foreach (Control tempct in ct)
            {
                if (tempct.Name.IndexOf("_init") > 0)
                {
                    // コントロールリストの情報を格納する
                    this.xmlSettings.AddControlSetting(tempct.Name, tempct);
                }
            }

            // 設定ファイルを読み込む
            ACTInitSetting.LoadSettings(xmlSettings);

            // ログを取得するイベントを生成する
            ActGlobals.oFormActMain.OnLogLineRead += OFormActMain_OnLogLineRead;

            scanList = new List<string>();
            // scanlistに要素を格納する
            if (checkedListBox_init.CheckedItems.Count != 0)
            {
                for (int x = 0; x < checkedListBox_init.CheckedItems.Count; x++)
                {
                    scanList.Add(checkedListBox_init.CheckedItems[x].ToString());
                }
            }

        }

        private void OFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            // 18文字以下のログは読み捨てる
            // なぜならば、タイムスタンプ＋ログタイプのみのログだから
            if (logInfo.logLine.Length <= 18)
            {
                return;
            }

            // 戦闘開始のお知らせ
            if (logInfo.logLine.Contains("戦闘開始！"))
            {
                scanList = new List<string>();
                // scanlistに要素を格納する
                if (checkedListBox_init.CheckedItems.Count != 0)
                {
                    for (int x = 0; x < checkedListBox_init.CheckedItems.Count; x++)
                    {
                        scanList.Add(checkedListBox_init.CheckedItems[x].ToString());
                    }
                }
            }


            // 対象のログが流れた際は、座標を取得する（座標取得はデフォルト設定）
            foreach (string scanstr in scanList)
            {
                if (logInfo.logLine.Contains(scanstr))
                {
                    fileFullPath_main_local_scan = textBoxlocalPath_init.Text;
                    GetMobInfo(scanstr);
                }

            }
            if (logInfo.logLine.Contains("座標取得!"))
            {
                GetMobInfo("座標取得");
            }

        }
        /// <summary>
        /// 指定したパスにディレクトリが存在しない場合
        /// すべてのディレクトリとサブディレクトリを作成します
        /// </summary>
        public static DirectoryInfo SafeCreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return null;
            }
            return Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Mob情報を取得する
        /// </summary>
        private void GetMobInfo(string FileName)
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

            // ファイルを出力する
            this.FilePush(CombertBeanList, FileName);

        }
        /// <summary>
        /// ログを出力するメソッド
        /// </summary>
        /// <param name="MobName"></param>
        /// <param name="MaxHp"></param>
        /// <param name="CurrentHP"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        private void FilePush(List<CombertBean> CombertBeanList, string FileName)
        {
            string FileOutPath = @fileFullPath_main_local_scan;
            // ファイル出力先を作成する
            if (!(FileOutPath.Substring(FileOutPath.Length - 1)).Equals("\\"))
            {
                FileOutPath = FileOutPath + "\\";
            }


            // ファイルの存在チェックを実施し、存在しない名前になるまで連番作成を行う
            int i = 1;

            string Path = FileOutPath + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + FileName;
            while (File.Exists(Path + i + ".txt"))
            {
                i++;
            }
            Path = Path + i + ".txt";

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
        /// addボタン押下時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_add_Click(object sender, EventArgs e)
        {
            // まず記載の文字を格納する
            string addtext = textBox_list.Text;
            // 空の場合、処理を中断する
            if (addtext == "") return;
            // リストボックスに、アイテムを追加する
            checkedListBox_init.Items.Add(addtext, true);
            // テキストボックスの中身を空にする
            textBox_list.Text = "";
            scanList = new List<string>();
            // scanlistに要素を格納する
            if (checkedListBox_init.CheckedItems.Count != 0)
            {
                for (int x = 0; x < checkedListBox_init.CheckedItems.Count; x++)
                {
                    scanList.Add(checkedListBox_init.CheckedItems[x].ToString());
                }
            }
        }
        /// <summary>
        /// deleteボタン押下時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_delete_Click(object sender, EventArgs e)
        {
            // アイテムが何も選択されていなかった場合は、処理を中断する
            if (select_item_name == "") return;
            // アイテムを削除する
            checkedListBox_init.Items.Remove(select_item_name);
        }
        /// <summary>
        /// アイテムが選択された時のイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox_init_SelectedIndexChanged(object sender, EventArgs e)
        {
            // アイテムが0件だった場合、処理を中断する
            if (checkedListBox_init.Items.Count == 0) return;
            // アイテム名を格納する
            select_item_name = (string)checkedListBox_init.SelectedItem;

            scanList = new List<string>();
            // scanlistに要素を格納する
            if (checkedListBox_init.CheckedItems.Count != 0)
            {
                for (int x = 0; x < checkedListBox_init.CheckedItems.Count; x++)
                {
                    scanList.Add(checkedListBox_init.CheckedItems[x].ToString());
                }
            }
        }
    }
}
