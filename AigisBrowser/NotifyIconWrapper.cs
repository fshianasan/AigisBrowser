using System;
using System.Drawing;
using System.Windows.Forms;

namespace AigisBrowser
{
	public class NotifyIconWrapper
	{
		private static NotifyIcon _notifyIcon;

		// 初期化
		public NotifyIconWrapper()
		{
			_notifyIcon = new NotifyIcon
			{
				Text = "AigisBrowser",
				Icon = new Icon(@"C:\Users\fshianer\Documents\Visual Studio 2015\Projects\AigisBrowser\AigisBrowser\Resources\icon.ico"),
				Visible = true,
			};
		}

		public void Show(string title, string text)
		{
			if (_notifyIcon != null)
			{
				_notifyIcon.ShowBalloonTip(1000, title, text, ToolTipIcon.None);
			}
		}

		public void Dispose()
		{
			if (_notifyIcon != null)
			{
				_notifyIcon.Dispose();
			}
		}
	}
}
