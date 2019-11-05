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
using AbsoluteAlexander_MaliktenderOnly.Utils;
using System.Net;
using System.Threading;

namespace AbsoluteAlexander_MaliktenderOnly
{
    public partial class Alexander : UserControl, IActPluginV1
    {
        private SettingsSerializer xmlSettings;
        // チェックの入った物をすべて格納しておく
        private List<string> scanList = new List<string>();
        // 選択アイテム名を格納する
        private string select_item_name = "";

        private bool userAuthFlg = false;

        private bool logoutFlg = false;
        // 戦闘終了時のフラグ
        private static DateTime lastWipeOutDateTime = DateTime.Now;
        private string dateStr = "";


        private Terops123 terops123 = new Terops123();
        private Size terops123Size = new Size();
        private Size terops123pictureBox1Size = new Size();
        private Size terops123pictureBox2Size = new Size();
        private Size terops123pictureBox3Size = new Size();
        private Size terops123pictureBox4Size = new Size();


        public Alexander()
        {
            InitializeComponent();
            ActHelper.Initialize();
        }

        public void DeInitPlugin()
        {
            ACTInitSetting.SaveSettings(this.xmlSettings);
        }

        public void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText)
        {
            //lbStatus = pluginStatusText;   // Hand the status label's reference to our local var
            pluginScreenSpace.Controls.Add(this);   // Add this UserControl to the tab ACT provides
            Dock = DockStyle.Fill; // Expand the UserControl to fill the tab's client space
                                   // MultiProject.BasePlugin.xmlSettings = new SettingsSerializer(this); // Create a new settings serializer and pass it this instance

            pluginScreenSpace.Text = "絶アレキ_MaliktenderOnly";
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

            // 非同期的にユーザ認証の処理を行う
            Task.Run(CheckUser);
            // ワイプチェック
            // Task.Run(WipeOutCheck);

        }

        /// <summary>
        /// ユーザ認証を実施する
        /// </summary>
        /// <returns></returns>
        private async Task CheckUser()
        {
            for (int r = 1; r <= 6; r++)
            {
                // デフォルトのメンバーは、強制的に利用できるようにする
                if (DefMember.DefMemberList.Contains(ActHelper.MyName()))
                {
                    userAuthFlg = true;
                    break;
                }
                // ユーザ認証フラグが「true」の場合、処理を停止する
                if (userAuthFlg)
                {
                    break;
                }

                // ユーザ認証を行う
                UserAuth();
                if (userAuthFlg)
                {
                    // ユーザ認証フラグがtrueの場合は、ループを抜ける
                    break;
                }
                // ループ毎に10秒ディレイをかける
                await Task.Delay(10000);
            }

            if (!userAuthFlg)
            {
                MessageBox.Show("使用対象外のキャラクターです。\r\n「AbsoluteAlexander_MaliktenderOnly」の機能を使用できなくしております。\r\n\r\n利用をご希望の方は、管理者までお問い合わせ下さい。");
            }
        }

        /// <summary>
        /// ユーザ認証処理のメイン
        /// </summary>
        /// <returns></returns>
        private void UserAuth()
        {
            // 対象者しか、機能を利用できなくする
            if (GetUrl(ActHelper.MyName()))
            {
                userAuthFlg = true;
            }
            else
            {
                userAuthFlg = false;
            }
        }

        /// <summary>
        /// 使えるキャラかを判定する
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool GetUrl(string name)
        {
            try
            {
                return new WebClient().DownloadString("https://sites.google.com/view/actdrivesendersetting/").Contains(name);
            }
            catch (WebException)
            {
                return false;
            }
        }

        /// <summary>
        /// ワイプを判定する
        /// </summary>
        /// <returns></returns>
        public void WipeOutCheck()
        {
            while (true)
            {
                // 1秒待機
                Thread.Sleep(100);
                // 15秒間は次の処理を行わない
                if ((DateTime.Now - lastWipeOutDateTime).TotalSeconds >= 15.0)
                {
                    List<Combatant> ptList = ActHelper.GetPTList();
                    int ptcnt = ptList.Count;
                    int checlptcnt = 0;

                    foreach (Combatant combatant in ptList)
                    {
                        if (combatant.CurrentHP == 0)
                        {
                            checlptcnt++;
                        }
                    }
                    // 全員のHPが0の場合、ワイプ判定を行う
                    if (checlptcnt == ptcnt)
                    {
                        lastWipeOutDateTime = DateTime.Now;
                        ActInvoker.Invoke(() => ActGlobals.oFormActMain.EndCombat(true));
                        //wipflg = true;

                        Task.Run(() =>
                        {
                            // wipeoutログを発生させる
                            Thread.Sleep(TimeSpan.FromSeconds(1.5));
                            string[] wipe = { "TimeLinePlugin_戦闘終了", "FF14" };
                            ActHelper.LogParser.RaiseLog(DateTime.Now, wipe);
                        });
                    }
                    else
                    {
                        //wipflg = false;
                    }
                }
            }
        }

        /// <summary>
        /// logを読むイベントを発生させる
        /// </summary>
        /// <param name="isImport"></param>
        /// <param name="logInfo"></param>
        private void OFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            // ユーザ認証されていない場合は、処理を行わせないように処理を行う
            if (userAuthFlg)
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
                    // log出力フラグを立てる
                    if (checkBox_logout_flg_init.Checked)
                    {
                        logoutFlg = true;
                        dateStr = DateTime.Now.ToString("mmddHHMM");
                    }


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

                // log出力フラグ用の処理
                if (logoutFlg)
                {
                    OutLog.WriteTraceLog(logInfo.logLine, textBoxlocalPath_init.Text, dateStr + "_battle");
                }


                // 対象のログが流れた際は、座標を取得する（座標取得はデフォルト設定）
                if (!string.IsNullOrWhiteSpace(textBoxlocalPath_init.Text))
                {
                    foreach (string scanstr in scanList)
                    {
                        if (logInfo.logLine.Contains(scanstr)) FileOutPut.GetMobInfo(scanstr, textBoxlocalPath_init.Text);
                    }
                    if (logInfo.logLine.Contains("座標取得!")) FileOutPut.GetMobInfo("座標取得", textBoxlocalPath_init.Text);
                }

                // 空なら何もしない
                if (!string.IsNullOrWhiteSpace(textBox_only_init.Text))
                {
                    if (logInfo.logLine.Contains(textBox_only_init.Text))
                    {
                        // mob情報を取得する
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

                        textBox1_only_cond.Text = OutPutString;
                    }
                }

            }
        }

        private void button_add_Click_1(object sender, EventArgs e)
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

        private void checkedListBox_init_SelectedIndexChanged_1(object sender, EventArgs e)
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

        private void button_delete_Click_1(object sender, EventArgs e)
        {
            // アイテムが何も選択されていなかった場合は、処理を中断する
            if (select_item_name == "") return;
            // アイテムを削除する
            checkedListBox_init.Items.Remove(select_item_name);
            // scanlistに要素を格納する
            if (checkedListBox_init.CheckedItems.Count != 0)
            {
                for (int x = 0; x < checkedListBox_init.CheckedItems.Count; x++)
                {
                    scanList.Add(checkedListBox_init.CheckedItems[x].ToString());
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox_ボタン切り替え用隠し.Text == "表示位置確認")
            {
                int X = textBox_terop_X_init.Text == "" ? 100 : int.Parse(textBox_terop_X_init.Text);
                int Y = textBox_terop_Y_init.Text == "" ? 100 : int.Parse(textBox_terop_Y_init.Text);
                Point point = new Point(X, Y);
                // 位置を指定してしまう
                terops123.Location = point;

                terops123.Show();

                textBox_ボタン切り替え用隠し.Text = "表示確認終了";
                button1_表示位置確認.Text = "表示確認終了";
            }
            else
            {
                terops123.Hide();

                textBox_ボタン切り替え用隠し.Text = "表示位置確認";
                button1_表示位置確認.Text = "表示位置確認";
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox_倍率_text_init.Text = listBox_倍率_init.Text;

            double bairitsu = double.Parse(listBox_倍率_text_init.Text.ToString().Replace("倍", ""));

            terops123.Size = new Size((int)(terops123Size.Width * bairitsu), (int)(terops123Size.Height * bairitsu));
            terops123.pictureBox1.Size = new Size((int)(terops123pictureBox1Size.Width * bairitsu), (int)(terops123pictureBox1Size.Height * bairitsu));
            terops123.pictureBox2.Size = new Size((int)(terops123pictureBox2Size.Width * bairitsu), (int)(terops123pictureBox2Size.Height * bairitsu));
            terops123.pictureBox3.Size = new Size((int)(terops123pictureBox3Size.Width * bairitsu), (int)(terops123pictureBox3Size.Height * bairitsu));
            terops123.pictureBox4.Size = new Size((int)(terops123pictureBox4Size.Width * bairitsu), (int)(terops123pictureBox4Size.Height * bairitsu));

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int X = textBox_terop_X_init.Text == "" ? 100 : int.Parse(textBox_terop_X_init.Text);
                int Y = textBox_terop_Y_init.Text == "" ? 100 : int.Parse(textBox_terop_Y_init.Text);
                Point point = new Point(X, Y);
                // 位置を指定してしまう
                terops123.Location = point;
            }
            catch
            {

            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int X = textBox_terop_X_init.Text == "" ? 100 : int.Parse(textBox_terop_X_init.Text);
                int Y = textBox_terop_Y_init.Text == "" ? 100 : int.Parse(textBox_terop_Y_init.Text);
                Point point = new Point(X, Y);
                // 位置を指定してしまう
                terops123.Location = point;
            }
            catch
            {

            }
        }
    }
}
