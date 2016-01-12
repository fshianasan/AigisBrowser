using System;
using System.Windows;
using MahApps.Metro.Controls;

namespace AigisBrowser
{
	/// <summary>
	/// Window1.xaml の相互作用ロジック
	/// </summary>
	public partial class Window1
	{
		private readonly string URI_MAIN_FRAME = "http://assets.millennium-war.net/00/html/main.htm";

		public Window1()
		{
			InitializeComponent();

			this.frame.Navigate(new Uri(URI_MAIN_FRAME));
		}
	}
}
