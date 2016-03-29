using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Diagnostics;

using MahApps.Metro.Controls;

namespace AigisBrowser
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : MetroWindow
    {
		private readonly string URL_START = "http://www.dmm.com/lp/game/aigis/index008.html/=/navi=none/";
		private readonly string URL_GAME = "http://www.dmm.com/netgame/social/-/gadgets/=/app_id=177037/";
		
		private readonly string URL_START_R18 = "http://www.dmm.co.jp/lp/game/aigis/index012.html/=/navi=none/";
		private readonly string URL_GAME_R18 = "http://www.dmm.co.jp/netgame/social/-/gadgets/=/app_id=156462/";

		// private int VOLUME_DEFAULT = 7;

		private NotifyIconWrapper notifyIcon = new NotifyIconWrapper();
		// private Audio audio = new Audio();

		// Regkey
		Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree);

		string process_name = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe";
		string process_dbg_name = System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".vshost.exe";

		public MainWindow()
		{
			InitializeComponent();

			#if DEBUG
			regkey.SetValue(process_dbg_name, 11000, Microsoft.Win32.RegistryValueKind.DWord);
			#else
			regkey.SetValue(process_name, 11000, Microsoft.Win32.RegistryValueKind.DWord);
			#endif

			regkey.Close();

			SelectMenuNormal.IsChecked = true;

			this.webBrowser.Navigate(new Uri(URL_START));
			this.getTodayQuestName();

			#if !DEBUG
			this.windowButton_Github.Visibility = Visibility.Collapsed;
			this.windowButton_DebugMenu.Visibility = Visibility.Collapsed;
			#endif
		}

        private async void LoadAsync()
		{
			await Task.Run(() => { System.Threading.Thread.Sleep(2000); });

            this.MetroWindow.ResizeMode = ResizeMode.CanMinimize;

			// windowButton の表示切替
			this.windowButton_SelectMenu.Visibility = Visibility.Hidden;
			this.windowButton_Refresh.Visibility = Visibility.Visible;
            this.windowButton_ScreenShot.Visibility = Visibility.Visible;
            // this.windowButton_AudioMute.Visibility = Visibility.Visible;

            // ステータスバーの表示切り替え
            // this.statusBarItem_Address.Visibility = Visibility.Collapsed;
            // this.statusBarItem_TodayQuestName.Visibility = Visibility.Visible;
			
			this.documentChange();
            this.windowResize(640, 960);
        }

		#region Debug

		//#if DEBUG
		// https://dotnetlearning.wordpress.com/2011/02/20/dropdown-menu-in-wpf/
		private void menuButton2_Click(object sender, RoutedEventArgs e)
		{
			(sender as Button).ContextMenu.IsEnabled = true;
			(sender as Button).ContextMenu.PlacementTarget = (sender as Button);
			(sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			(sender as Button).ContextMenu.IsOpen = true;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			notifyIcon.Show("でぱでぱ", "エチゾラム");
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			Window1 w1 = new Window1();
			w1.Show();
		}
		//#endif

		#endregion

		#region WebBrowser

		private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
		{
			if(e.Uri.AbsoluteUri == URL_GAME || e.Uri.AbsoluteUri == URL_GAME_R18)
			{
				LoadAsync();
			}
		}

		/* private void webBrowser_Navigated(object sender, NavigationEventArgs e)
		{
			// statusBar_Address に現在の URL を表示。
			statusBar_Address.Text = webBrowser.Source.ToString();
		}*/

		private void WebBrowserRefreshed()
		{
			webBrowser.Refresh();
			LoadAsync();
		}

		#endregion

		#region <Controls:MetroWindow.LeftWindowCommands>

		// Githubを開く
		private void windowCommand_Github(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/fshianasan/AigisBrowser");
        }

		#endregion

		#region <Controls:MetroWindow.RightWindowCommands>

		// https://dotnetlearning.wordpress.com/2011/02/20/dropdown-menu-in-wpf/
		private void menuButton_Click(object sender, RoutedEventArgs e)
		{
			(sender as Button).ContextMenu.IsEnabled = true;
			(sender as Button).ContextMenu.PlacementTarget = (sender as Button);
			(sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			(sender as Button).ContextMenu.IsOpen = true;
		}

		// 設定(テスト)
		private void windowCommand_Settings(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("申し訳ありませんが未実装です。", MetroWindow.Title, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		// DMM公式コミュニティを開く
		private void windowCommand_Community(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.dmm.com/netgame/community/-/topic/detail/=/cid=1510/tid=14590/");
        }

        // Twitterを開く
        private void windowCommand_Twitter(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://twitter.com/Aigis1000/");
        }
        
        // Wikiを開く
        private void windowCommand_Wiki(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://aigis.gcwiki.info/");
        }

		// webBrowser の再読み込み
		private void windowCommand_Refresh(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("ブラウザーの再読み込みしてもよろしいですか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
			{
				WebBrowserRefreshed();
			}
		}

		// スクリーンショット撮影
		private void windowCommand_ScreenShot(object sender, RoutedEventArgs e)
        {
			string fileName = string.Format("Aigis-{0}.{1}", DateTime.Now.ToString("yyMMdd-HHmmss"), "png");
			string directoryPath = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), "Screenshots");
			if (!Directory.Exists(directoryPath)) { Directory.CreateDirectory(directoryPath); }
			string filePath = Path.Combine(directoryPath, fileName);

			takeScreenShot(fileName, filePath);
		}

		// ミュート
		/* private void windowCommand_AudioMute(object sender, RoutedEventArgs e)
		{
			try
			{
				a.toggleMute();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(string.Format("Exception : {0}.{1} >> {2}", ex.TargetSite.ReflectedType.FullName, ex.TargetSite.Name, ex.Message));
			}
		} */

		#endregion

		private void takeScreenShot(string Name, string Path)
        {
			try
            {
                if (this.webBrowser.Document == null) return;

                // http://qiita.com/hbsnow/items/8ffd3b6d077d84a92900
                Image imgScreen = new Image();
                imgScreen.Width = (int)this.webBrowser.ActualWidth;
                imgScreen.Height = (int)this.webBrowser.ActualHeight;
                imgScreen.Source = new DrawingImage(VisualTreeHelper.GetDrawing(this.webBrowser));

                using (FileStream fs = new FileStream(Path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    var vis = new DrawingVisual();
                    DrawingContext cont = vis.RenderOpen();
                    cont.DrawImage(
                        imgScreen.Source,
                        new Rect(new Size(imgScreen.Width, imgScreen.Height))
                    );
                    cont.Close();

                    var rtb = new RenderTargetBitmap(
                        (int)imgScreen.Width,
                        (int)imgScreen.Height,
                        96d,
                        96d,
                        PixelFormats.Default
                    );
                    rtb.Render(vis);

                    var enc = new PngBitmapEncoder();
                    enc.Frames.Add(BitmapFrame.Create(rtb));
                    enc.Save(fs);

					// 本当は BalloonTipClicked で画像を開きたいけど処理の書き方がわからない
					notifyIcon.Show("スクリーンショット撮影に成功！", string.Format("{0}", Name));
				}
			}
            catch (Exception ex)
            {
                MessageBox.Show("スクリーンショット撮影に失敗しました。\n\n" + ex, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine("Exception: {0}.{1} >> {2}", ex.TargetSite.ReflectedType.FullName, ex.TargetSite.Name, ex.Message);
            }
        }

        private void documentChange()
        {
            try
            {
                #region CSS調整
                StringBuilder css = new StringBuilder();
                css.Append("body");
                css.Append("{");
                css.Append("    margin: 0;");
                css.Append("    overflow: hidden;");
                css.Append("}");
                css.Append("#game_frame");
                css.Append("{");
                css.Append("    left: -5px;");
                css.Append("    position: fixed;");
                css.Append("    z-index: 1;");
                css.Append("}");
                #endregion

                // div
                var document = webBrowser.Document as mshtml.HTMLDocument;
                if(document == null) return;

                document.createStyleSheet().cssText = css.ToString();

                // 各種 Div 要素の変更。
                var div01 = this.getDivElementsByClassName(document, "dmm-ntgnavi");
                var div02 = document.getElementById("ntg-recommend");
                var div03 = this.getDivElementsByClassName(document, "area-naviapp mg-t20");
                var div04 = document.getElementById("foot");

                if (div01 != null) div01.style.display = "none";
                if (div02 != null) div02.style.visibility = "hidden";
                if (div03 != null) div03.style.display = "none";
                if (div04 != null) div04.style.display = "none";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Exception : {0}.{1} >> {2}", ex.TargetSite.ReflectedType.FullName, ex.TargetSite.Name, ex.Message));
            }
        }

        private void windowResize(int height, int width)
        {
            try
            {
                if (webBrowser.Document == null) return;
                
                // MetroWindow の最小値のリサイズ
                this.MetroWindow.MinWidth = width;
                this.MetroWindow.MinHeight = height;

                // webBrowser のリサイズ
                this.webBrowser.Width = width;
                this.webBrowser.Height = height;

                // MainWindow のリサイズ
                this.MetroWindow.Width = width;
                this.MetroWindow.Height = height + 30; // statusBar が WebBrowser に隠れるのを防ぐ為。絶対良い方法があるはず。}               

                // http://stackoverflow.com/questions/4019831/how-do-you-center-your-main-window-in-wpf
                Rect workArea = System.Windows.SystemParameters.WorkArea;
                this.MetroWindow.Left = (workArea.Width - this.MetroWindow.Width) / 2 + workArea.Left;
                this.MetroWindow.Top = (workArea.Height - this.MetroWindow.Height) / 2 + workArea.Top;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Exception : {0}.{1} >> {2}", ex.TargetSite.ReflectedType.FullName, ex.TargetSite.Name, ex.Message));
            }
        }

        // 曜日限定クエスト取得
        private void getTodayQuestName()
        {
            // 曜日限定クエスト
            string[] questName = new string[7]
			{
				"強者の集う戦場, 新魔水晶の守護者", //日曜日
                "黄金の鎧", // 月曜日
                "聖霊救出", // 火曜日
                "新魔水晶の守護者", // 水曜日
                "空からの贈物, 男だけの祝杯", // 木曜日 
                "黄金の鎧", // 金曜日
                "強者の集う戦場, 聖霊救出", // 土曜日
            };

            // 覚醒の宝珠
            string[] questName2 = new string[7]
            {
                "覚醒の宝珠：月影の弓騎兵", // 日曜日
                "覚醒の宝珠：白き射手", // 月曜日
                "覚醒の宝珠：一角獣騎士", // 火曜日
                "覚醒の宝珠：伝説の海賊", // 水曜日
                "覚醒の宝珠：怪力少女", // 木曜日
                "覚醒の宝珠：魔女", // 金曜日
                "覚醒の宝珠：魔導鎧姫", // 土曜日
            };

            DateTime dt = DateTime.Now;
            
            /*statusBar_TodayQuestName.Text = string.Format("【今日の曜日限定クエスト】 {0} / {1}",
                questName[(int)dt.DayOfWeek],
                questName2[(int)dt.DayOfWeek]
            );*/

            Debug.WriteLine("Debug: 曜日: {0}, 曜日限定: {1}, 覚醒: {2}",
                dt.DayOfWeek,
                questName[(int)dt.DayOfWeek],
                questName2[(int)dt.DayOfWeek]
            );

			notifyIcon.Show("今日の曜日限定クエスト", string.Format("{0}\n{1}", questName[(int)dt.DayOfWeek], questName2[(int)dt.DayOfWeek]));
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

		// MainWindow の終了処理
		private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (MessageBox.Show("終了してもよろしいですか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Asterisk) == MessageBoxResult.No)
			{
				e.Cancel = true;
			}

			notifyIcon.Dispose();
		}

		private void selectGamemode_Normal(object sender, RoutedEventArgs e)
		{
			SelectMenuR18.IsChecked = false;
			this.webBrowser.Navigate(new Uri(URL_START));
		}

		private void selectGamemodeMenu_R18(object sender, RoutedEventArgs e)
		{
			SelectMenuNormal.IsChecked = false;
			this.webBrowser.Navigate(new Uri(URL_START_R18));
		}
	}
}
