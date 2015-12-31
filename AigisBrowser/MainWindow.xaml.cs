using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using mshtml;

using MahApps.Metro.Controls;

namespace AigisBrowser
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        const string START_URL = "http://www.dmm.com/lp/game/aigis/index008.html/=/navi=none/";
        const string GAME_URL = "http://www.dmm.com/netgame/social/-/gadgets/=/app_id=177037/";

        public MainWindow()
        {
            InitializeComponent();

            this.webBrowser.Navigate(new Uri(START_URL));
            webBrowser.LoadCompleted += WebBrowserOnLoadCompleted;
        }

        private void WebBrowserOnLoadCompleted(object sender, NavigationEventArgs e)
        {
            documentChange();

            if (webBrowser.Source == new Uri(GAME_URL))
            {
                this.MetroWindow.ResizeMode = ResizeMode.CanMinimize;
                this.statusBarItem_Address.Visibility = Visibility.Collapsed;
                this.statusBarItem_TodayQuestName.Visibility = Visibility.Visible;
                windowResize(970, 640);
                getTodayQuestName();
                Console.WriteLine("Debug: webBrowser_Navigated if");
            }
        }

        // Githubを開く
        private void windowCommand_Github(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/fshianasan/AigisBrowser");
            }
            catch
            {

            }
        }

        // DMM公式コミュニティを開く
        private void windowCommand_Community(object sender, RoutedEventArgs e)
        {
            try {
                System.Diagnostics.Process.Start("http://www.dmm.com/netgame/community/-/topic/detail/=/cid=1510/tid=14590/");
            }
            catch
            {

            }
            
        }

        // Twitterを開く
        private void windowCommand_Twitter(object sender, RoutedEventArgs e)
        {
            try {
                System.Diagnostics.Process.Start("https://twitter.com/Aigis1000/");
            }
            catch
            {

            }
            
        }
        
        // Wikiを開く
        private void windowCommand_Wiki(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://aigis.gcwiki.info/");
            }
            catch
            {

            }
        }

        // スクリーンショット撮影

        // サウンドミュート

        private void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            // statusBar_Address に現在の URL を表示。
            statusBar_Address.Text = webBrowser.Source.ToString();

            getTodayQuestName();
        }

        private void documentChange()
        {
            try
            {
                #region CSS
                StringBuilder css = new StringBuilder();
                css.Append("body");
                css.Append("{");
                css.Append("    overflow: hidden;");
                css.Append("}");
                css.Append("iframe");
                css.Append("{");
                css.Append("    position: fixed;");
                css.Append("    z-index: 1;");
                css.Append("}");
                #endregion

                // div
                var document = webBrowser.Document as mshtml.HTMLDocument;
                if(document == null) return;

                document.createStyleSheet().cssText = css.ToString();

                // 各種 Div 要素の変更。
                var div01 = document.getElementById("dmm-ntgnavi-renew");
                var div02 = document.getElementById("ntg-recommend");
                var div03 = this.getDivElementsByClassName(document, "area-naviapp mg-t20");
                var div04 = document.getElementById("foot");

                if (div01 != null) div01.style.display = "none";
                if (div02 != null) div02.style.visibility = "hidden";
                if (div03 != null) div03.style.display = "none";
                if (div04 != null) div04.style.display = "none";

                // game_frame
                var iframe_game = document.frames.item(0) as mshtml.HTMLWindow2;
                var document_game = iframe_game.document as mshtml.HTMLDocument;

                var div11 = document_game.getElementById("aigis") as mshtml.HTMLIFrame;

                // iframe#aigis の width をどうにか変更できないか？
                if (div11 != null) div11.style.width = string.Format("{0}px", -10);

                // game_frame > aigis
                var iframe_aigis = document_game.frames.item(0) as mshtml.HTMLWindow2;
                var document_aigis = iframe_aigis.document as mshtml.HTMLDocument;
                
                var div21 = document_aigis.getElementById("main_frame");

                if (div21 != null) div21.style.display = "none";
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Exception : {0}.{1} >> {2}", ex.TargetSite.ReflectedType.FullName, ex.TargetSite.Name, ex.Message));
            }
        }

        private void windowResize(int width, int height)
        {
            try
            {
                if (webBrowser.Document == null) return;

                this.MetroWindow.MinWidth = width;
                this.MetroWindow.MinHeight = height + 54;

                // webBrowser のリサイズ
                this.webBrowser.Width = width;
                this.webBrowser.Height = height;

                // MainWindow のリサイズ
                this.MetroWindow.Width = width;
                this.MetroWindow.Height = height + 54; // statusBar が WebBrowser に隠れるのを防ぐ為。絶対良い方法があるはず。

                // div
                var document = webBrowser.Document as mshtml.HTMLDocument;
                if (document == null) return;

               // game_frame
                var iframe_game = document.frames.item(0) as mshtml.HTMLWindow2;
                var document_game = iframe_game.document as mshtml.HTMLDocument;
                if (document_game == null) return;

                // game_frame > aigis
                var iframe_aigis = document_game.frames.item(0) as mshtml.HTMLWindow2;
                var document_aigis = iframe_aigis.document as mshtml.HTMLDocument;
                if (document_aigis == null) return;

                var frame = document_game.getElementById("aigis");
                if (frame == null) return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Exception : {0}.{1} >> {2}", ex.TargetSite.ReflectedType.FullName, ex.TargetSite.Name, ex.Message));
            }
        }

        // 曜日限定クエスト取得
        private void getTodayQuestName()
        {
            // 曜日限定クエスト
            string[] questName = new string[7] {
                "黄金の鎧", // 月曜日
                "聖霊救出", // 火曜日
                "空からの贈物, 男だけの祝杯", // 水曜日
                "新魔水晶の守護者", // 木曜日 
                "新魔水晶の守護者", // 金曜日
                "強者の集う戦場", // 土曜日
                "強者の集う戦場" //.日曜日
            };

            // 覚醒の宝珠
            string[] questName2 = new string[7]
            {
                "覚醒の宝珠：白き射手", // 月曜日
                "覚醒の宝珠：一角獣騎士", // 火曜日
                "覚醒の宝珠：伝説の海賊", // 水曜日
                "覚醒の宝珠：怪力少女", // 木曜日
                "覚醒の宝珠：魔女", // 金曜日
                "覚醒の宝珠：魔導鎧姫", // 土曜日
                "覚醒の宝珠：月影の弓騎兵" // 日曜日
            };

            DateTime dt = DateTime.Now;

            Console.WriteLine("曜日: {0}, 曜日限定: {1}, 覚醒: {2}\n", dt.DayOfWeek, questName[Convert.ToInt16(dt.DayOfWeek)], questName2[Convert.ToInt16(dt.DayOfWeek)]);

            statusBar_TodayQuestName.Text = string.Format("【今日の曜日限定クエスト】{0} / {1}", questName[Convert.ToInt16(dt.DayOfWeek)], questName2[Convert.ToInt16(dt.DayOfWeek)]);
        }

        // コピペ
        private mshtml.HTMLDivElement getDivElementsByClassName(mshtml.HTMLDocument document, string className)
        {
            try
            {
                if (document == null) return null;
                var divs = document.getElementsByTagName("div");
                foreach (mshtml.HTMLDivElement item in divs)
                {
                    if (item.className == className)
                    {
                        return item;
                    }
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
