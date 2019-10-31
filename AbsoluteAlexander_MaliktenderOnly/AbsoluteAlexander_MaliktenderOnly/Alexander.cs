﻿using System;
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
        private bool wipflg = false;
        private string dateStr = "";


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
                        wipflg = true;

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
                        wipflg = false;
                    }
                }
            }
        }

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
                        dateStr = DateTime.Now.ToString("mmdd HH:MM");
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
                    OutLog.WriteTraceLog(logInfo.logLine , textBoxlocalPath_init.Text, dateStr + "_battle");
                }


                // 対象のログが流れた際は、座標を取得する（座標取得はデフォルト設定）
                if (!string.IsNullOrWhiteSpace(textBoxlocalPath_init.Text)) { 
                    foreach (string scanstr in scanList)
                    {
                        if (logInfo.logLine.Contains(scanstr)) FileOutPut.GetMobInfo(scanstr, textBoxlocalPath_init.Text);
                    }
                    if (logInfo.logLine.Contains("座標取得!"))  FileOutPut.GetMobInfo("座標取得", textBoxlocalPath_init.Text);
                }

            }
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
