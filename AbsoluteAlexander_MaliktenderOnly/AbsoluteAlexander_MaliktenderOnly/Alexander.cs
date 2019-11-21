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
        //　Mob監視用
        private static IDataSubscription DataSubscription { get; set; }

        // フラグ管理
        private bool userAuthFlg = false;
        private bool initButtoleFlg = false;
        private bool logoutFlg = false;
        private bool combatFlg = false;

        // 一般フィールド
        private int time = 0;
        private string dateStr = "";
        private string select_item_name = "";
        private string MyName = "";

        // リスト管理
        private List<string> scanList = new List<string>();
        private List<string> AbiList = new List<string>();
        private List<string> Moblist = new List<string>();
        private List<Combatant> PtList = new List<Combatant>();

        // チェイサー用
        private bool リミカ判定開始flg = false;
        private int リミカMyNumber = 0;
        private Dictionary<int, string> リミッターカットdic = new Dictionary<int, string>();
        private Dictionary<int, Dictionary<string, string>> チェイサー座標dic = new Dictionary<int, Dictionary<string, string>>();
        private int ホークブラスター回数 = 0;
        private int ホークブラスターlog回数 = 0;

        // ジャスティス用
        private int CntPage = 0;
        private bool ジャスティス判定開始flg = false;
        private Dictionary<string, int> PTNamedic = new Dictionary<string, int>();


        // 時間停止カンペ用フラグ
        private bool 時間停止kanpeflg = false;
        private List<string> 接近禁止命令List = new List<string>();
        private List<string> 接近強制命令List = new List<string>();
        private List<string> 加重罰List = new List<string>();
        private List<string> 無印List = new List<string>();

        // 時空潜行カンペ用フラグ
        private bool 時空潜行flg = false;


        // Terops123 初期化
        private Terops123 terops123 = new Terops123();
        private Size terops123Size = new Size();
        private Size terops123pictureBox1Size = new Size();
        private Size terops123pictureBox2Size = new Size();
        private Size terops123pictureBox3Size = new Size();
        private Size terops123pictureBox4Size = new Size();
        private Size terops123pictureBox5Size = new Size();
        private Size terops123pictureBox6Size = new Size();
        private Size terops123pictureBox7Size = new Size();
        private Size terops123pictureBox8Size = new Size();

        // Terops123 初期化
        private TeropsABCD teropsABCD = new TeropsABCD();
        private Size teropsABCDSize = new Size();
        private Size teropsABCDpictureBoxASize = new Size();
        private Size teropsABCDpictureBoxBSize = new Size();
        private Size teropsABCDpictureBoxCSize = new Size();
        private Size teropsABCDpictureBoxDSize = new Size();

        // Teropshork
        private Teropshork teropshork = new Teropshork();
        private Size teropshorkSize = new Size();
        private Size teropshorkpictureBox北Size = new Size();
        private Size teropshorkpictureBox北東Size = new Size();
        private Size teropshorkpictureBox東Size = new Size();
        private Size teropshorkpictureBox南東Size = new Size();
        private Size teropshorkpictureBox南Size = new Size();
        private Size teropshorkpictureBox南西Size = new Size();
        private Size teropshorkpictureBox西Size = new Size();
        private Size teropshorkpictureBox北西Size = new Size();


        // Terops上下右左
        private Terops上下右左 terops上下右左 = new Terops上下右左();
        private Size terops上下右左size = new Size();
        private Size terops上下右左pictureBox上Size = new Size();
        private Size terops上下右左pictureBox右Size = new Size();
        private Size terops上下右左pictureBox下Size = new Size();
        private Size terops上下右左pictureBox左Size = new Size();

        // TeropTimeLine
        private TeropTimeLine teropTimeLine = new TeropTimeLine();


        // --------------------------------------- コンストラクタ ---------------------------------------
        public Alexander()
        {
            InitializeComponent();
            ActHelper.Initialize();
        }
        // --------------------------------------- コンストラクタ ---------------------------------------


        // --------------------------------------- DeInit処理 ---------------------------------------
        public void DeInitPlugin()
        {
            ACTInitSetting.SaveSettings(this.xmlSettings);
            terops123.Hide();
            teropsABCD.Hide();

            terops123.Close();
            teropsABCD.Close();
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

            // アビリティリストが存在していない場合は、ダウンロードを実施する
            ACTInitSetting.CheckAbiText();

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
            // ログを取得するイベントを生成する
            ActGlobals.oFormActMain.OnLogLineRead += OFormActMain_OnLogLineRead;

            // 戦闘開始をチェックする
            ActGlobals.oFormActMain.OnCombatStart += OFormActMain_OnCombatStart;

            // 戦闘終了をキャッチする
            ActGlobals.oFormActMain.OnCombatEnd += OFormActMain_OnCombatEnd;

            checkBox_logout_flg_init.Visible = false;
            checkBox1_TimeLine_init.Visible = false;
            checkBox1_Abi_init.Visible = false;



            // 非同期的にユーザ認証の処理を行う
            Task.Run(CheckUser);


            // テロップの初期サイズ設定を行う
            terops123Size = new Size(150, 150);
            terops123pictureBox1Size = new Size(150, 150);
            terops123pictureBox2Size = new Size(150, 150);
            terops123pictureBox3Size = new Size(150, 150);
            terops123pictureBox4Size = new Size(150, 150);
            terops123pictureBox5Size = new Size(150, 150);
            terops123pictureBox6Size = new Size(150, 150);
            terops123pictureBox7Size = new Size(150, 150);
            terops123pictureBox8Size = new Size(150, 150);

            terops123.pictureBox1.Visible = false;
            terops123.pictureBox2.Visible = false;
            terops123.pictureBox3.Visible = false;
            terops123.pictureBox4.Visible = false;
            terops123.pictureBox5.Visible = false;
            terops123.pictureBox6.Visible = false;
            terops123.pictureBox7.Visible = false;
            terops123.pictureBox8.Visible = false;

            terops123.pictureBox1.Visible = true;
            terops123.pictureBox2.Visible = true;
            terops123.pictureBox3.Visible = true;
            terops123.pictureBox4.Visible = true;
            terops123.pictureBox5.Visible = true;
            terops123.pictureBox6.Visible = true;
            terops123.pictureBox7.Visible = true;
            terops123.pictureBox8.Visible = true;
            terops123.Show();
            terops123.Hide();

            teropsABCDSize = new Size(150, 150);
            teropsABCDpictureBoxASize = new Size(150, 150);
            teropsABCDpictureBoxBSize = new Size(150, 150);
            teropsABCDpictureBoxCSize = new Size(150, 150);
            teropsABCDpictureBoxDSize = new Size(150, 150);

            teropsABCD.pictureBoxA.Visible = true;
            teropsABCD.pictureBoxB.Visible = true;
            teropsABCD.pictureBoxC.Visible = true;
            teropsABCD.pictureBoxD.Visible = true;

            teropsABCD.pictureBoxA.Visible = false;
            teropsABCD.pictureBoxB.Visible = false;
            teropsABCD.pictureBoxC.Visible = false;
            teropsABCD.pictureBoxD.Visible = false;

            teropshorkSize = new Size(150, 150);
            teropshorkpictureBox北Size = new Size(150, 150);
            teropshorkpictureBox北東Size = new Size(150, 150);
            teropshorkpictureBox東Size = new Size(150, 150);
            teropshorkpictureBox南東Size = new Size(150, 150);
            teropshorkpictureBox南Size = new Size(150, 150);
            teropshorkpictureBox南西Size = new Size(150, 150);
            teropshorkpictureBox西Size = new Size(150, 150);
            teropshorkpictureBox北西Size = new Size(150, 150);

            teropshork.pictureBox北.Visible = true;
            teropshork.pictureBox北東.Visible = true;
            teropshork.pictureBox東.Visible = true;
            teropshork.pictureBox南東.Visible = true;
            teropshork.pictureBox南.Visible = true;
            teropshork.pictureBox南西.Visible = true;
            teropshork.pictureBox西.Visible = true;
            teropshork.pictureBox北西.Visible = true;
            teropshork.pictureBox北.Visible = false;
            teropshork.pictureBox北東.Visible = false;
            teropshork.pictureBox東.Visible = false;
            teropshork.pictureBox南東.Visible = false;
            teropshork.pictureBox南.Visible = false;
            teropshork.pictureBox南西.Visible = false;
            teropshork.pictureBox西.Visible = false;
            teropshork.pictureBox北西.Visible = false;
            teropshork.Show();
            teropshork.Hide();


            teropTimeLine.pictureBox1.Visible = true;
            teropTimeLine.pictureBox2.Visible = true;
            teropTimeLine.pictureBox3.Visible = true;
            teropTimeLine.pictureBox4.Visible = true;
            teropTimeLine.pictureBox5.Visible = true;
            teropTimeLine.pictureBox6.Visible = true;
            teropTimeLine.pictureBox7.Visible = true;
            teropTimeLine.pictureBox8.Visible = true;
            teropTimeLine.pictureBox9.Visible = true;
            teropTimeLine.pictureBox10.Visible = true;
            teropTimeLine.pictureBox11.Visible = true;
            teropTimeLine.pictureBox12.Visible = true;
            teropTimeLine.pictureBox1.Visible = false;
            teropTimeLine.pictureBox2.Visible = false;
            teropTimeLine.pictureBox3.Visible = false;
            teropTimeLine.pictureBox4.Visible = false;
            teropTimeLine.pictureBox5.Visible = false;
            teropTimeLine.pictureBox6.Visible = false;
            teropTimeLine.pictureBox7.Visible = false;
            teropTimeLine.pictureBox8.Visible = false;
            teropTimeLine.pictureBox9.Visible = false;
            teropTimeLine.pictureBox10.Visible = false;
            teropTimeLine.pictureBox11.Visible = false;
            teropTimeLine.pictureBox12.Visible = false;
            teropshork.Show();
            teropshork.Hide();

            terops上下右左size = new Size(315, 190);
            terops上下右左pictureBox上Size = new Size(150, 150);
            terops上下右左pictureBox右Size = new Size(150, 150);
            terops上下右左pictureBox下Size = new Size(150, 150);
            terops上下右左pictureBox左Size = new Size(150, 150);

            terops上下右左.pictureBox上.Visible = true;
            terops上下右左.pictureBox右.Visible = true;
            terops上下右左.pictureBox下.Visible = true;
            terops上下右左.pictureBox左.Visible = true;
            terops上下右左.pictureBox上.Visible = false;
            terops上下右左.pictureBox右.Visible = false;
            terops上下右左.pictureBox下.Visible = false;
            terops上下右左.pictureBox左.Visible = false;
            terops上下右左.Show();
            terops上下右左.Hide();

            // フォームの初期処理
            InitForm();
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
            PtList = ActHelper.GetPTList();
            List<string> AbiGetJobList = new List<string>();
            foreach (Combatant combatant in PtList)
            {
                string jobName = Job.Instance.GetJobName(combatant.Job);
                if (!AbiGetJobList.Contains(jobName))
                {
                    AbiGetJobList.Add(jobName);
                }
            }
            AbiList = CreateTimeLine.ReadFile.AbiList_create(AbiGetJobList);

            // mobListを取得する
            Moblist = ActHelper.CreatemobList();

            // 戦闘が開始したら、タイマーを起動する
            time = 0;
            Task.Run(ButtoleTimer);

            // 戦闘開始時の時間を記憶する
            dateStr = DateTime.Now.ToString("yyyyMMddhhmmss");

            リミカ判定開始flg = false;
            リミカMyNumber = 0;
            CntPage = 0;
            ホークブラスターlog回数 = 0;
            ホークブラスター回数 = 1;

            リミッターカットdic = new Dictionary<int, string>();
            リミッターカットdic.Add(1, ":004F:0000:");
            リミッターカットdic.Add(2, ":0050:0000:");
            リミッターカットdic.Add(3, ":0051:0000:");
            リミッターカットdic.Add(4, ":0052:0000:");
            リミッターカットdic.Add(5, ":0053:0000:");
            リミッターカットdic.Add(6, ":0054:0000:");
            リミッターカットdic.Add(7, ":0055:0000:");
            リミッターカットdic.Add(8, ":0056:0000:");

            チェイサー座標dic = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<string, string> チェイサー座標詳細dic = new Dictionary<string, string>();
            // 北
            チェイサー座標詳細dic.Add("100", "115");
            チェイサー座標dic.Add(1, チェイサー座標詳細dic);
            // 北東
            チェイサー座標詳細dic = new Dictionary<string, string>();
            チェイサー座標詳細dic.Add("110", "110");
            チェイサー座標dic.Add(2, チェイサー座標詳細dic);
            // 東
            チェイサー座標詳細dic = new Dictionary<string, string>();
            チェイサー座標詳細dic.Add("115", "100");
            チェイサー座標dic.Add(3, チェイサー座標詳細dic);
            // 南東
            チェイサー座標詳細dic = new Dictionary<string, string>();
            チェイサー座標詳細dic.Add("110", "89.");
            チェイサー座標dic.Add(4, チェイサー座標詳細dic);

            MyName = ActHelper.MyName();

            // 時間停止の初期化
            時間停止kanpeflg = false;
            時空潜行flg = false;


            接近禁止命令List = new List<string>();
            接近強制命令List = new List<string>();
            加重罰List = new List<string>();
            無印List = new List<string>();
            foreach (string name in Utils.DefMember.DefMemberList)
            {
                無印List.Add(name);
            }

            PTNamedic = new Dictionary<string, int>();
            PTNamedic.Add("Toki Tokinoa", 1);
            PTNamedic.Add("Ray Willpolis", 1);
            PTNamedic.Add("Hina Spring", 2);
            PTNamedic.Add("Mame San", 2);
            PTNamedic.Add("Ren Aizen", 3);
            PTNamedic.Add("Fifflearn Cook", 3);
            PTNamedic.Add("Ashe Highwind", 3);
            PTNamedic.Add("C'c' Lemon", 3);



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

            リミカ判定開始flg = false;
            ジャスティス判定開始flg = false;
            CntPage = 0;

            時間停止kanpeflg = false;
            時空潜行flg = false;
            接近禁止命令List = new List<string>();
            接近強制命令List = new List<string>();
            加重罰List = new List<string>();
            無印List = new List<string>();

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
            terops123.pictureBox1.Visible = false;
            terops123.pictureBox2.Visible = false;
            terops123.pictureBox3.Visible = false;
            terops123.pictureBox4.Visible = false;
            terops123.pictureBox5.Visible = false;
            terops123.pictureBox6.Visible = false;
            terops123.pictureBox7.Visible = false;
            terops123.pictureBox8.Visible = false;
            terops123.Show();
            terops123.Hide();

            // teropsABCD 設定初期化
            /*
            teropsABCD.Location = SettingPoint(textBox4_リフト_X_init, textBox2_リフト_Y_init);
            BairituSettingABCD();
            teropsABCD.pictureBoxA.Visible = false;
            teropsABCD.pictureBoxB.Visible = false;
            teropsABCD.pictureBoxC.Visible = false;
            teropsABCD.pictureBoxD.Visible = false;
            teropsABCD.Show();
            teropsABCD.Hide();
            */

            // teropshork 設定初期化
            teropshork.Location = SettingPoint(textBox_terop_X_init, textBox_terop_Y_init);
            BairituSettinghork();
            teropshork.pictureBox北.Visible = false;
            teropshork.pictureBox北東.Visible = false;
            teropshork.pictureBox東.Visible = false;
            teropshork.pictureBox南東.Visible = false;
            teropshork.pictureBox南.Visible = false;
            teropshork.pictureBox南西.Visible = false;
            teropshork.pictureBox西.Visible = false;
            teropshork.pictureBox北西.Visible = false;
            teropshork.Show();
            teropshork.Hide();

            teropTimeLine.Location = SettingPoint(textBox_terop_X_init, textBox_terop_Y_init);
            teropTimeLine.pictureBox1.Visible = false;
            teropTimeLine.pictureBox2.Visible = false;
            teropTimeLine.pictureBox3.Visible = false;
            teropTimeLine.pictureBox4.Visible = false;
            teropTimeLine.pictureBox5.Visible = false;
            teropTimeLine.pictureBox6.Visible = false;
            teropTimeLine.pictureBox7.Visible = false;
            teropTimeLine.pictureBox8.Visible = false;
            teropTimeLine.pictureBox9.Visible = false;
            teropTimeLine.pictureBox10.Visible = false;
            teropTimeLine.pictureBox11.Visible = false;
            teropTimeLine.pictureBox12.Visible = false;
            teropTimeLine.Show();
            teropTimeLine.Hide();

            // terops上下右左用
            terops上下右左.pictureBox上.Visible = false;
            terops上下右左.pictureBox右.Visible = false;
            terops上下右左.pictureBox下.Visible = false;
            terops上下右左.pictureBox左.Visible = false;
            terops上下右左.Show();
            terops上下右左.Hide();


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
                MessageBox.Show("使用対象外のキャラクターです。\r\n「AbsoluteAlexander_MaliktenderOnly」の機能を使用できなくしております。\r\n\r\n利用をご希望の方は、作成者までお問い合わせ下さい。");
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
            MyName = checkName;
            // 対象者しか、機能を利用できなくする
            if (GetUrl(checkName) || DefMember.DefMemberList.Contains(checkName))
            {
                try
                {
                    // 管理権限用
                    if (KanriMem.KanriMemList.Contains(checkName) || GetUrl(checkName + " kanri"))
                    {
                        checkBox_kanrisya_init.Checked = true;
                        checkBox_kanrisya_init.Visible = true;
                        checkBox_logout_flg_init.Visible = true;
                        checkBox_logout_flg_init.Checked = false;
                        checkBox1_TimeLine_init.Visible = true;
                        checkBox1_Abi_init.Visible = true;
                    }
                    else
                    {
                        checkBox_kanrisya_init.Checked = false;
                        checkBox_kanrisya_init.Visible = false;
                        checkBox_logout_flg_init.Checked = false;
                        checkBox_logout_flg_init.Visible = false;
                        checkBox1_TimeLine_init.Visible = false;
                        checkBox1_TimeLine_init.Checked = false;
                        checkBox1_Abi_init.Visible = false;
                        checkBox1_Abi_init.Checked = false;
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
                return new WebClient().DownloadString("https://sites.google.com/view/absolutealexandermaliktenderon").Contains(name);
            }
            catch (WebException)
            {
                return false;
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
                && !string.IsNullOrWhiteSpace(MobName)
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
            try
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


                    // -------------------------- リミッターカットを判定する --------------------------
                    if (checkBoxrimita_check_init.Checked)
                    {
                        try
                        {

                            if (logInfo.logLine.Contains("クルーズチェイサー:コードネーム「ブラスティー」！ 階差閉宇宙ヲ、脅カス敵ヲ発見……撃滅スル！"))
                            {
                                リミカ判定開始flg = true;
                            }

                            if (リミカ判定開始flg)
                            {
                                if (logInfo.logLine.Contains(MyName) &&
                                    リミカMyNumber == 0)
                                {

                                    foreach (KeyValuePair<int, string> kvp in リミッターカットdic)
                                    {
                                        if (logInfo.logLine.Contains(kvp.Value))
                                        {
                                            リミカMyNumber = kvp.Key;
                                            //string TTSstr = "";
                                            //terops123.Show();
                                            /*
                                            switch (リミカMyNumber)
                                            {
                                                case 1:
                                                    terops123.pictureBox1.Visible = true;
                                                    TTSstr = "みなみ";
                                                    break;
                                                case 2:
                                                    terops123.pictureBox2.Visible = true;
                                                    TTSstr = "きた";
                                                    break;
                                                case 3:
                                                    terops123.pictureBox3.Visible = true;
                                                    TTSstr = "みなみ";
                                                    break;
                                                case 4:
                                                    terops123.pictureBox4.Visible = true;
                                                    TTSstr = "きた";
                                                    break;
                                                case 5:
                                                    terops123.pictureBox5.Visible = true;
                                                    TTSstr = "みなみ";
                                                    break;
                                                case 6:
                                                    terops123.pictureBox6.Visible = true;
                                                    TTSstr = "きた";
                                                    break;
                                                case 7:
                                                    terops123.pictureBox7.Visible = true;
                                                    TTSstr = "みなみ";
                                                    break;
                                                case 8:
                                                    terops123.pictureBox8.Visible = true;
                                                    TTSstr = "きた";
                                                    break;
                                                default:
                                                    リミカMyNumber = 0;
                                                    break;
                                            }
                                            */
                                            //ActGlobals.oFormActMain.TTS(リミカMyNumber + "番、" + TTSstr);
                                            //terops123.Show();
                                            break;
                                        }
                                    }
                                }

                                // ホークブラスターを確認する
                                if (リミカMyNumber != 0)
                                {
                                    // リミカ用のmobListを取得する
                                    List<Combatant> リミカMobList = ActHelper.GetMobCombatantList();
                                    FileOutPut.GetMobInfo("リミカ確認用", textBoxlocalPath_init.Text);
                                    List<Combatant> newList = new List<Combatant>();


                                    foreach (Combatant combatant in リミカMobList)
                                    {
                                        if ("クルーズチェイサー".Equals(combatant.Name)
                                            && combatant.CurrentHP == 44)
                                        {
                                            newList.Add(combatant);
                                        }
                                    }

                                    foreach (KeyValuePair<int, Dictionary<string, string>> kvp in チェイサー座標dic)
                                    {
                                        foreach (Combatant combatant in newList)
                                        {
                                            foreach (KeyValuePair<string, string> pair in kvp.Value)
                                            {
                                                string X = combatant.PosX.ToString().Substring(0, 3);
                                                string Y = combatant.PosY.ToString().Substring(0, 3);

                                                if (pair.Key.Equals(X) && pair.Value.Equals(Y))
                                                {
                                                    string TTSstr = "";
                                                    int num = kvp.Key;
                                                    teropshork.Show();
                                                    switch (num)
                                                    {
                                                        // 1、5
                                                        case 1:
                                                            if (リミカMyNumber == 3 || リミカMyNumber == 4 || リミカMyNumber == 7 || リミカMyNumber == 8)
                                                            {
                                                                teropshork.pictureBox北.Visible = true;
                                                                TTSstr = "えー";
                                                            }
                                                            else
                                                            {
                                                                teropshork.pictureBox南.Visible = true;
                                                                TTSstr = "しー";
                                                            }
                                                            break;
                                                        // 2、6
                                                        case 2:
                                                            if (リミカMyNumber == 3 || リミカMyNumber == 4 || リミカMyNumber == 7 || リミカMyNumber == 8)
                                                            {
                                                                teropshork.pictureBox北東.Visible = true;
                                                                TTSstr = "えーびー";
                                                            }
                                                            else
                                                            {
                                                                teropshork.pictureBox南西.Visible = true;
                                                                TTSstr = "しーでー";
                                                            }
                                                            break;
                                                        // 3、7
                                                        case 3:
                                                            if (リミカMyNumber == 3 || リミカMyNumber == 4 || リミカMyNumber == 7 || リミカMyNumber == 8)
                                                            {
                                                                teropshork.pictureBox東.Visible = true;
                                                                TTSstr = "びー";
                                                            }
                                                            else
                                                            {
                                                                teropshork.pictureBox西.Visible = true;
                                                                TTSstr = "でー";
                                                            }
                                                            break;
                                                        // 4、8 
                                                        case 4:
                                                            if (リミカMyNumber == 3 || リミカMyNumber == 4 || リミカMyNumber == 7 || リミカMyNumber == 8)
                                                            {
                                                                teropshork.pictureBox南東.Visible = true;
                                                                TTSstr = "びーしー";
                                                            }
                                                            else
                                                            {
                                                                teropshork.pictureBox北西.Visible = true;
                                                                TTSstr = "えーでー";
                                                            }
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                    teropshork.Show();
                                                    ActGlobals.oFormActMain.TTS(TTSstr);
                                                    リミカ判定開始flg = false;
                                                    リミカMyNumber = 0;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                // 表示を消す
                                if (logInfo.logLine.Contains("ブルートジャスティスの「ジャスティスキック」"))
                                {
                                    リミカ判定開始flg = false;
                                    リミカMyNumber = 0;
                                    InitForm();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            // 全てのExceptionを握りつぶす
                            //　処理中断
                            リミカ判定開始flg = false;
                            リミカMyNumber = 0;
                            InitForm();
                            OutLog.WriteTraceLog(e.Message, textBoxlocalPath_init.Text, dateStr + "_errorリミカ処理");
                        }
                    }
                    // -------------------------- リミッターカットを判定する --------------------------

                    // -------------------------- 時間停止を判定する --------------------------
                    if (checkBox2_kanpe_init.Checked)
                    {
                        if (logInfo.logLine.Contains("アレキサンダー・プライム:我はアレキサンダー……機械仕掛けの神なり……"))
                        {
                            時間停止kanpeflg = true;
                        }
                        // 時間停止の判定を行う
                        if (時間停止kanpeflg)
                        {
                            string log = logInfo.logLine;
                            // 接近禁止のlogを取得
                            if (log.Contains("に「確定判決：接近禁止命令」の効果。"))
                            {
                                foreach (string name in DefMember.DefMemberList)
                                {
                                    if (log.Contains(name))
                                    {
                                        接近禁止命令List.Add(name);
                                        無印List.Remove(name);
                                        break;
                                    }
                                }
                            }
                            // 接近強制命令のlogを取得
                            if (log.Contains("に「確定判決：接近強制命令」の効果。"))
                            {
                                foreach (string name in DefMember.DefMemberList)
                                {
                                    if (log.Contains(name))
                                    {
                                        接近強制命令List.Add(name);
                                        無印List.Remove(name);
                                        break;
                                    }
                                }
                            }
                            // 加重罰のlogを取得
                            if (log.Contains("に「確定判決：加重罰」の効果。"))
                            {
                                foreach (string name in DefMember.DefMemberList)
                                {
                                    if (log.Contains(name))
                                    {
                                        加重罰List.Add(name);
                                        無印List.Remove(name);
                                        break;
                                    }
                                }
                            }
                            // 全て出そろったら判定を行う
                            if (加重罰List.Count == 2 &&
                                接近禁止命令List.Count == 2 &&
                                接近強制命令List.Count == 2 &&
                                無印List.Count == 2)
                            {
                                terops上下右左.Show();
                                if (加重罰List.Contains(MyName))
                                {
                                    ActGlobals.oFormActMain.TTS("ジャスティスの外周に立つ");
                                }
                                else if (接近禁止命令List.Contains(MyName))
                                {
                                    // チェック
                                    // DPSの場合
                                    if (PTNamedic[MyName] == 3)
                                    {
                                        ActGlobals.oFormActMain.TTS("みぎがわにいく");
                                        terops上下右左.pictureBox右.Visible = true;
                                    }
                                    else
                                    {
                                        ActGlobals.oFormActMain.TTS("ひだりがわにいく");
                                        terops上下右左.pictureBox左.Visible = true;
                                    }
                                }
                                else if (接近強制命令List.Contains(MyName))
                                {
                                    // チェック
                                    // DPSの場合
                                    if (PTNamedic[MyName] == 3)
                                    {
                                        ActGlobals.oFormActMain.TTS("みぎぼすのしたがわ");
                                        terops上下右左.pictureBox下.Visible = true;
                                        terops上下右左.pictureBox右.Visible = true;
                                    }
                                    else
                                    {
                                        ActGlobals.oFormActMain.TTS("みぎぼすのうえがわ");
                                        terops上下右左.pictureBox上.Visible = true;
                                        terops上下右左.pictureBox右.Visible = true;
                                    }
                                }
                                else if (無印List.Contains(MyName))
                                {
                                    // チェック
                                    // DPSの場合
                                    if (PTNamedic[MyName] == 3)
                                    {
                                        ActGlobals.oFormActMain.TTS("ひだりぼすのうえがわ");
                                        terops上下右左.pictureBox上.Visible = true;
                                        terops上下右左.pictureBox左.Visible = true;
                                    }
                                    else
                                    {
                                        ActGlobals.oFormActMain.TTS("ひだりぼすのしたがわ");
                                        terops上下右左.pictureBox下.Visible = true;
                                        terops上下右左.pictureBox左.Visible = true;
                                    }

                                }
                            }
                        }
                    }
                    // -------------------------- 時間停止を判定する --------------------------

                    // -------------------------- 時空潜行を判定する --------------------------
                    if (logInfo.logLine.Contains("アレキサンダー・プライムは「時空潜行のマーチ」の構え。"))
                    {
                        時空潜行flg = true;
                    }

                    if (時空潜行flg)
                    {

                    }

                    // -------------------------- 時空潜行を判定する --------------------------


                    // -------------------------- アビリティファイルの出力処理(いったん廃止) --------------------------
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
                                    if (log.Contains(combatant.Name) & log.Contains("の「" + skil + "」"))
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
                                if (log.Length - 18 == 0)
                                {
                                    break;
                                }
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


                    // -------------------------- 以下管理者領域 --------------------------
                    if (checkBox_kanrisya_init.Checked)
                    {
                        // -------------------------- log出力の処理開始 --------------------------
                        // log出力フラグ用の処理
                        // 戦闘中のlog以外は取得しない(いったん廃止)
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
            catch (Exception e)
            {
                OutLog.WriteTraceLog(e.Message, textBoxlocalPath_init.Text, dateStr + "_errorAlllog");
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
            terops123.pictureBox5.Size = new Size((int)(terops123pictureBox5Size.Width * bairitsu), (int)(terops123pictureBox5Size.Height * bairitsu));
            terops123.pictureBox6.Size = new Size((int)(terops123pictureBox6Size.Width * bairitsu), (int)(terops123pictureBox6Size.Height * bairitsu));
            terops123.pictureBox7.Size = new Size((int)(terops123pictureBox7Size.Width * bairitsu), (int)(terops123pictureBox7Size.Height * bairitsu));
            terops123.pictureBox8.Size = new Size((int)(terops123pictureBox8Size.Width * bairitsu), (int)(terops123pictureBox8Size.Height * bairitsu));
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
                teropshork.Location = SettingPoint(textBox4_リフト_X_init, textBox2_リフト_Y_init);
            }
            catch
            { }
        }

        private void textBox2_リフト_Y_init_TextChanged(object sender, EventArgs e)
        {
            try
            {
                teropshork.Location = SettingPoint(textBox4_リフト_X_init, textBox2_リフト_Y_init);
            }
            catch
            { }
        }

        private void comboBox1_リフト_init_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BairituSettinghork();
            }
            catch { }
        }

        private void button1_リフト_init_Click(object sender, EventArgs e)
        {
            if (textBox1_リフト_init.Text == "表示位置確認")
            {
                int X = textBox4_リフト_X_init.Text == "" ? 100 : int.Parse(textBox4_リフト_X_init.Text);
                int Y = textBox2_リフト_Y_init.Text == "" ? 100 : int.Parse(textBox2_リフト_Y_init.Text);
                Point point = new Point(X, Y);
                // 位置を指定してしまう
                teropshork.Location = point;
                teropshork.Show();

                textBox1_リフト_init.Text = "表示確認終了";
                button1_リフト_init.Text = "表示確認終了";
            }
            else
            {
                teropshork.Hide();

                textBox1_リフト_init.Text = "表示位置確認";
                button1_リフト_init.Text = "表示位置確認";
            }
        }


        private void BairituSettinghork()
        {
            listBox_倍率_text_init.Text = listBox_倍率_init.Text;

            double bairitsu = double.Parse(listBox_倍率_text_init.Text.ToString().Replace("倍", ""));

            teropshork.Size = new Size((int)(teropshorkSize.Width * bairitsu), (int)(teropshorkSize.Height * bairitsu));
            teropshork.pictureBox北.Size = new Size((int)(terops123pictureBox1Size.Width * bairitsu), (int)(terops123pictureBox1Size.Height * bairitsu));
            teropshork.pictureBox北東.Size = new Size((int)(terops123pictureBox2Size.Width * bairitsu), (int)(terops123pictureBox2Size.Height * bairitsu));
            teropshork.pictureBox東.Size = new Size((int)(terops123pictureBox3Size.Width * bairitsu), (int)(terops123pictureBox3Size.Height * bairitsu));
            teropshork.pictureBox南東.Size = new Size((int)(terops123pictureBox4Size.Width * bairitsu), (int)(terops123pictureBox4Size.Height * bairitsu));
            teropshork.pictureBox南.Size = new Size((int)(terops123pictureBox5Size.Width * bairitsu), (int)(terops123pictureBox5Size.Height * bairitsu));
            teropshork.pictureBox南西.Size = new Size((int)(terops123pictureBox6Size.Width * bairitsu), (int)(terops123pictureBox6Size.Height * bairitsu));
            teropshork.pictureBox西.Size = new Size((int)(terops123pictureBox7Size.Width * bairitsu), (int)(terops123pictureBox7Size.Height * bairitsu));
            teropshork.pictureBox北西.Size = new Size((int)(terops123pictureBox8Size.Width * bairitsu), (int)(terops123pictureBox8Size.Height * bairitsu));
        }



        private void button2_ロックフラクチャー_init_Click(object sender, EventArgs e)
        {
            if (textBox5_ロックフラクチャー_init.Text == "表示位置確認")
            {
                int X = textBox8_ロックフラクチャー_X_init.Text == "" ? 100 : int.Parse(textBox8_ロックフラクチャー_X_init.Text);
                int Y = textBox6_ロックフラクチャー_Y_init.Text == "" ? 100 : int.Parse(textBox6_ロックフラクチャー_Y_init.Text);
                Point point = new Point(X, Y);
                // 位置を指定してしまう
                teropTimeLine.Location = point;

                teropTimeLine.Show();

                textBox5_ロックフラクチャー_init.Text = "表示確認終了";
                button2_ロックフラクチャー_init.Text = "表示確認終了";
            }
            else
            {
                teropTimeLine.Hide();

                textBox5_ロックフラクチャー_init.Text = "表示位置確認";
                button2_ロックフラクチャー_init.Text = "表示位置確認";
            }
        }


        private void textBox8_ロックフラクチャー_X_init_TextChanged(object sender, EventArgs e)
        {
            try
            {
                teropTimeLine.Location = SettingPoint(textBox8_ロックフラクチャー_X_init, textBox6_ロックフラクチャー_Y_init);
            }
            catch
            { }
        }


        private void textBox6_ロックフラクチャー_Y_init_TextChanged(object sender, EventArgs e)
        {
            try
            {
                teropTimeLine.Location = SettingPoint(textBox8_ロックフラクチャー_X_init, textBox6_ロックフラクチャー_Y_init);
            }
            catch
            { }
        }








        //
        private void button2_認証_Click(object sender, EventArgs e)
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

        private void button_delete_Click(object sender, EventArgs e)
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
    }
}
