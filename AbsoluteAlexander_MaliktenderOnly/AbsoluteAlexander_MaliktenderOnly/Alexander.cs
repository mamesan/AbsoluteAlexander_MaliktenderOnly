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
using FFXIV_ACT_Plugin.Common;
using AbsoluteAlexander_MaliktenderOnly.Utils;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

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
        private bool initButtoleFlg = false;
        private bool logoutFlg = false;
        // 戦闘終了時のフラグ
        private bool combatFlg = false;
        private static DateTime lastWipeOutDateTime = DateTime.Now;
        private string dateStr = "";

        private static IDataSubscription DataSubscription { get; set; }

        private List<string> AbiList = new List<string>();
        private List<Combatant> PtList = new List<Combatant>();
        private List<string> Moblist = new List<string>();
        private int time = 0;

        // Terops123 初期化
        private Terops123 terops123 = new Terops123();
        private Size terops123Size = new Size();
        private Size terops123pictureBox1Size = new Size();
        private Size terops123pictureBox2Size = new Size();
        private Size terops123pictureBox3Size = new Size();
        private Size terops123pictureBox4Size = new Size();

        // Terops123 初期化
        private TeropsABCD teropsABCD = new TeropsABCD();
        private Size teropsABCDSize = new Size();
        private Size teropsABCDpictureBoxASize = new Size();
        private Size teropsABCDpictureBoxBSize = new Size();
        private Size teropsABCDpictureBoxCSize = new Size();
        private Size teropsABCDpictureBoxDSize = new Size();

        // --------------------------------------- コンストラクタ ---------------------------------------
        public Alexander()
        {
            InitializeComponent();
            ActHelper.Initialize();
            terops123.Hide();
            teropsABCD.Hide();

            terops123.Close();
            teropsABCD.Close();
        }
        // --------------------------------------- コンストラクタ ---------------------------------------


        // --------------------------------------- DeInit処理 ---------------------------------------
        public void DeInitPlugin()
        {
            ACTInitSetting.SaveSettings(this.xmlSettings);
        }
        // --------------------------------------- DeInit処理 ---------------------------------------



        // --------------------------------------- init処理 ---------------------------------------
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

            // コンボボックスの初期値を設定
            if (listBox_倍率_init.SelectedIndex == -1)
            {
                listBox_倍率_init.SelectedIndex = 0;
            }
            if (comboBox1_リフト_init.SelectedIndex == -1)
            {
                comboBox1_リフト_init.SelectedIndex = 0;
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

            // 戦闘開始をチェックする
            ActGlobals.oFormActMain.OnCombatStart += OFormActMain_OnCombatStart;

            // 戦闘終了をキャッチする
            ActGlobals.oFormActMain.OnCombatEnd += OFormActMain_OnCombatEnd;

            // 非同期的にユーザ認証の処理を行う
            Task.Run(CheckUser);
            // ワイプチェック
            // Task.Run(WipeOutCheck);

            // テロップの初期サイズ設定を行う
            terops123Size = new Size(150, 150);
            terops123pictureBox1Size = new Size(150, 150);
            terops123pictureBox2Size = new Size(150, 150);
            terops123pictureBox3Size = new Size(150, 150);
            terops123pictureBox4Size = new Size(150, 150);

            teropsABCDSize = new Size(150, 150);
            teropsABCDpictureBoxASize = new Size(150, 150);
            teropsABCDpictureBoxBSize = new Size(150, 150);
            teropsABCDpictureBoxCSize = new Size(150, 150);
            teropsABCDpictureBoxDSize = new Size(150, 150);

        }

        // --------------------------------------- init処理 ---------------------------------------


        // 戦闘開始前init
        /// <summary>
        /// 戦闘前設定等の処理を全て初期化する処理
        /// </summary>
        private void battleInitSetting()
        {
            // initを走らせた判定を行う
            initButtoleFlg = true;

            // 戦闘開始フラグ
            combatFlg = true;

            // アビリティの一覧を取得する
            AbiList = CreateTimeLine.ReadFile.AbiList_create();

            PtList = ActHelper.GetPTList();

            // mobListを取得する
            Moblist = ActHelper.CreatemobList();

            // 戦闘が開始したら、タイマーを起動する
            time = 0;
            Task.Run(ButtoleTimer);

            // 戦闘開始時の時間を記憶する
            dateStr = DateTime.Now.ToString("mmddHHMM");

            // フォームの初期処理
            InitForm();











            // 以下、管理者向け機能の初期化処理
            if (checkBox_kanrisya_init.Checked)
            {
                // log出力フラグを立てる
                if (checkBox_logout_flg_init.Checked)
                {
                    logoutFlg = true;
                }

                // scanlistに要素を格納する（座標取得用）
                scanList = new List<string>();
                if (checkedListBox_init.CheckedItems.Count != 0)
                {
                    for (int x = 0; x < checkedListBox_init.CheckedItems.Count; x++)
                    {
                        scanList.Add(checkedListBox_init.CheckedItems[x].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 戦闘終了時の初期化処理
        /// </summary>
        private void battoleEndInitSetting()
        {
            // 停止処理initを走らせた判定を行う
            initButtoleFlg = false;

            // 戦闘開始フラグ
            combatFlg = false;



            // フォームの初期処理
            InitForm();
        }

        /// <summary>
        /// 各フォームの初期処理
        /// </summary>
        private void InitForm()
        {
            // 各フォームの座標固定処理
            // terops123 設定初期化
            terops123.Location = SettingPoint(textBox_terop_X_init, textBox_terop_Y_init);
            BairituSetting1234();
            terops123.Hide();

            // teropsABCD 設定初期化
            teropsABCD.Location = SettingPoint(textBox4_リフト_X_init, textBox2_リフト_Y_init);
            BairituSettingABCD();
            teropsABCD.Hide();
        }


        /// <summary>
        /// ユーザ認証を実施する
        /// </summary>
        /// <returns></returns>
        private async Task CheckUser()
        {
            for (int r = 1; r <= 6; r++)
            {
                // ユーザ認証を行う
                UserAuth();
                if (userAuthFlg)
                {
                    // ユーザ認証フラグがtrueの場合は、ループを抜ける
                    break;
                }
                // ループ毎に10秒ディレイをかける
                await Task.Delay(1000);
            }

            if (!userAuthFlg)
            {
                MessageBox.Show("使用対象外のキャラクターです。\r\n「AbsoluteAlexander_MaliktenderOnly」の機能を使用できなくしております。\r\n\r\n利用をご希望の方は、管理者までお問い合わせ下さい。");
            }
        }

        /// <summary>
        /// Timer処理
        /// </summary>
        public async Task ButtoleTimer()
        {
            while (combatFlg)
            {
                await Task.Delay(1000);
                time++;
            }
        }

        /// <summary>
        /// ユーザ認証処理のメイン
        /// </summary>
        /// <returns></returns>
        private void UserAuth()
        {
            string checkName = ActHelper.MyName();
            // 対象者しか、機能を利用できなくする
            if (GetUrl(checkName) || DefMember.DefMemberList.Contains(checkName))
            {
                try
                {
                    // 管理権限用
                    if (KanriMem.KanriMemList.Contains(checkName))
                    {
                        checkBox_kanrisya_init.Checked = true;
                        checkBox_kanrisya_init.Visible = true;

                    }
                    else
                    {
                        checkBox_kanrisya_init.Checked = false;
                        checkBox_kanrisya_init.Visible = false;
                    }
                    button2_認証.Visible = false;
                    userAuthFlg = true;
                    // モブ情報をキャッチする
                    ActHelper.DataSubscription.CombatantAdded += DataSubscription_CombatantAdded;
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
            }
            else
            {
                button2_認証.Visible = true;
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
        /// ワイプを判定する　　現在未使用
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
        /// 戦闘開始時のイベント
        /// </summary>
        /// <param name="isImport"></param>
        /// <param name="encounterInfo"></param>
        private void OFormActMain_OnCombatStart(bool isImport, CombatToggleEventArgs encounterInfo)
        {
            combatFlg = true;
        }
        /// <summary>
        /// 戦闘終了時のイベント
        /// </summary>
        /// <param name="isImport"></param>
        /// <param name="encounterInfo"></param>
        private void OFormActMain_OnCombatEnd(bool isImport, CombatToggleEventArgs encounterInfo)
        {
            combatFlg = false;
        }

        /// <summary>
        /// mob情報をキャッチする
        /// </summary>
        /// <param name="Combatant"></param>
        private void DataSubscription_CombatantAdded(object Combatant)
        {
            FFXIV_ACT_Plugin.Common.Models.Combatant combatant = (FFXIV_ACT_Plugin.Common.Models.Combatant)Combatant;
            string MobName = combatant.Name;
            if (!Moblist.Contains(MobName)
                && combatant.type == 2
                && combatant.OwnerID == 0)
            {
                Moblist.Add(MobName);
            }
        }

        // ------------------------------------------------- 以下、log出力イベントエリア -------------------------------------------------

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

                // -------------------------- 戦闘開始時の処理 --------------------------
                // 戦闘開始のお知らせ
                if (combatFlg && !initButtoleFlg)
                {
                    // 戦闘前の初期処理
                    battleInitSetting();
                }
                // -------------------------- 戦闘開始時の処理 --------------------------




                // -------------------------- 戦闘終了時の処理 --------------------------
                // 戦闘終了時
                if (!combatFlg && initButtoleFlg)
                {
                    // 戦闘終了時の共通初期化処理
                    battoleEndInitSetting();
                }
                // -------------------------- 戦闘終了時の処理 --------------------------



                // -------------------------- アビリティファイルの出力処理 --------------------------
                if (combatFlg && checkBox1_Abi_init.Checked)
                {
                    string log = logInfo.logLine;
                    // 味方の名前+の「が付いている場合、処理にかける
                    foreach (Combatant combatant in PtList)
                    {
                        if (log.Contains(combatant.Name) && log.Contains("の「") && !log.Contains("が切れた"))
                        {
                            foreach (string skil in AbiList)
                            {
                                // 味方のアビlogの場合、logを出力する
                                if (log.Contains(combatant.Name + "の「" + skil + "」"))
                                {
                                    OutLog.WriteTraceLog(time + " " + combatant.Name + "の「" + skil + "」", textBoxlocalPath_init.Text, dateStr + "_AbilityTimeLine");
                                    break;
                                }
                            }
                        }
                    }
                }
                // -------------------------- アビリティファイルの出力処理 --------------------------


                // -------------------------- MobTimeLineファイルの出力処理 --------------------------
                if (combatFlg && checkBox1_TimeLine_init.Checked)
                {
                    string log = logInfo.logLine;
                    // 技名のみ取得するようにする
                    foreach (string mobName in Moblist)
                    {
                        // 取得するlogを厳選する
                        if (log.Contains(mobName)
                        && (log.Contains("」の構え。")
                        || log.Contains(":Unknown")
                        || log.Contains("」を唱えた。")))
                        {
                            // 技名のみ取得するようにする
                            // Unknownは、前回のログと重複していなければ全てタイムラインに記載をする
                            string tmp = log.Substring(18, log.Length - 18);

                            string str = CreatetimeLine(tmp);
                            // logが、構えの場合sync を行う
                            if (log.Contains("」の構え。"))
                            {
                                str += " sync /" + mobName + "は" + CreatetimeLine_wName(tmp) + "の構え。/ window 5,5";
                            }
                            else if (log.Contains("」を唱えた。"))
                            {
                                str += " sync /" + mobName + "は" + CreatetimeLine_wName(tmp) + "を唱えた。/ window 5,5";
                            }
                            // 指定されたアビリティに該当した場合、logを出力する
                            // 10文字以下の場合は書き込まない
                            if (str.Length > 15)
                            {
                                OutLog.WriteTraceLog(str, textBoxlocalPath_init.Text, dateStr + "_MobTimeLine");
                                break;
                            }
                        }
                    }
                }
                // -------------------------- MobTimeLineファイルの出力処理 --------------------------


                // テスト用
                if (logInfo.logLine.Contains("wipeout"))
                {
                    FileOutPut.GetMobInfo("wipeout", textBoxlocalPath_init.Text);
                }





                // -------------------------- 以下管理者領域 --------------------------
                if (checkBox_kanrisya_init.Checked)
                {

                    // -------------------------- log出力の処理開始 --------------------------
                    // log出力フラグ用の処理
                    // 戦闘中のlog以外は取得しない
                    if (logoutFlg && combatFlg)
                    {
                        OutLog.WriteTraceLog(logInfo.logLine, textBoxlocalPath_init.Text, dateStr + "_battle");
                    }
                    // -------------------------- log出力の処理終了 --------------------------

                    // -------------------------- 座標取得の処理開始 --------------------------
                    // 対象のログが流れた際は、座標を取得する（座標取得はデフォルト設定）
                    if (!string.IsNullOrWhiteSpace(textBoxlocalPath_init.Text))
                    {
                        foreach (string scanstr in scanList)
                        {
                            if (logInfo.logLine.Contains(scanstr)) FileOutPut.GetMobInfo(scanstr, textBoxlocalPath_init.Text);
                        }
                        if (logInfo.logLine.Contains("座標取得!")) FileOutPut.GetMobInfo("座標取得", textBoxlocalPath_init.Text);
                    }
                    // -------------------------- 座標取得の処理終了 --------------------------

                    // -------------------------- 座標取得の処理テキストボックス運用開始 --------------------------
                    // 空なら何もしない
                    if (!string.IsNullOrWhiteSpace(textBox_only_init.Text)) textBox1_only_cond.Text = FileOutPut.createMobInfoString(FileOutPut.createCombartList());
                    // -------------------------- 座標取得の処理テキストボックス運用終了 --------------------------
                }
                // -------------------------- 以下管理者領域 --------------------------
            }
        }


        // ------------------------------------------------- 以下、log出力外イベントエリア -------------------------------------------------

        /// <summary>
        /// タイムラインを生成するメソッド
        /// </summary>
        /// <returns></returns>
        private string CreatetimeLine_wName(string str)
        {
            return Regex.Match(str, "\\「.*?\\」").Value;
        }

        /// <summary>
        /// タイムラインを生成するメソッド
        /// </summary>
        /// <returns></returns>
        private string CreatetimeLine(string str)
        {
            // logがUnknown系の場合、そのまま返却する
            if (str.Contains("Unknown"))
            {
                return time + " " + "\"" + str + "\"";
            }
            else
            {
                return time + " " + @"""" + Regex.Match(str, "\\「.*?\\」").Value.Replace("「", "").Replace("」", "") + @"""";
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
            try
            {
                // 倍率を呼び出す
                BairituSetting1234();
            }
            catch { }
        }

        private void BairituSetting1234()
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
                terops123.Location = SettingPoint(textBox_terop_X_init, textBox_terop_Y_init);
            }
            catch
            { }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                terops123.Location = SettingPoint(textBox_terop_X_init, textBox_terop_Y_init);
            }
            catch
            { }
        }

        private Point SettingPoint(TextBox textBoxX, TextBox textBoxY)
        {
            int X = textBoxX.Text == "" ? 100 : int.Parse(textBoxX.Text);
            int Y = textBoxY.Text == "" ? 100 : int.Parse(textBoxY.Text);
            return new Point(X, Y);
        }

        private void textBox4_リフト_X_init_TextChanged(object sender, EventArgs e)
        {
            try
            {
                teropsABCD.Location = SettingPoint(textBox4_リフト_X_init, textBox2_リフト_Y_init);
            }
            catch
            { }
        }

        private void textBox2_リフト_Y_init_TextChanged(object sender, EventArgs e)
        {
            try
            {
                teropsABCD.Location = SettingPoint(textBox4_リフト_X_init, textBox2_リフト_Y_init);
            }
            catch
            { }
        }

        private void comboBox1_リフト_init_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BairituSettingABCD();
            }
            catch { }
        }
        private void BairituSettingABCD()
        {
            textBox3_リフト_init.Text = comboBox1_リフト_init.Text;

            double bairitsu = double.Parse(textBox3_リフト_init.Text.ToString().Replace("倍", ""));

            teropsABCD.Size = new Size((int)(terops123Size.Width * bairitsu), (int)(teropsABCDSize.Height * bairitsu));
            teropsABCD.pictureBoxA.Size = new Size((int)(teropsABCDpictureBoxASize.Width * bairitsu), (int)(teropsABCDpictureBoxASize.Height * bairitsu));
            teropsABCD.pictureBoxB.Size = new Size((int)(teropsABCDpictureBoxBSize.Width * bairitsu), (int)(teropsABCDpictureBoxBSize.Height * bairitsu));
            teropsABCD.pictureBoxC.Size = new Size((int)(teropsABCDpictureBoxCSize.Width * bairitsu), (int)(teropsABCDpictureBoxCSize.Height * bairitsu));
            teropsABCD.pictureBoxD.Size = new Size((int)(teropsABCDpictureBoxDSize.Width * bairitsu), (int)(teropsABCDpictureBoxDSize.Height * bairitsu));

        }

        private void button1_リフト_init_Click(object sender, EventArgs e)
        {
            if (textBox1_リフト_init.Text == "表示位置確認")
            {
                int X = textBox4_リフト_X_init.Text == "" ? 100 : int.Parse(textBox4_リフト_X_init.Text);
                int Y = textBox2_リフト_Y_init.Text == "" ? 100 : int.Parse(textBox2_リフト_Y_init.Text);
                Point point = new Point(X, Y);
                // 位置を指定してしまう
                teropsABCD.Location = point;

                teropsABCD.Show();

                textBox1_リフト_init.Text = "表示確認終了";
                button1_リフト_init.Text = "表示確認終了";
            }
            else
            {
                teropsABCD.Hide();

                textBox1_リフト_init.Text = "表示位置確認";
                button1_リフト_init.Text = "表示位置確認";
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // 認証処理を実施する
            UserAuth();

            if (!userAuthFlg)
            {
                MessageBox.Show("使用対象外のキャラクターです。\r\n「AbsoluteAlexander_MaliktenderOnly」の機能を使用できなくしております。\r\n\r\n利用をご希望の方は、管理者までお問い合わせ下さい。");
            }
            else
            {
                MessageBox.Show("認証に成功しました。");
            }
        }
    }
}
