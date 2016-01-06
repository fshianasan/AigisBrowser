using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AigisBrowser
{
	class Audio
	{
		[DllImport("winmm.dll")]
		public static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);

		[DllImport("winmm.dll")]
		public static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

		public bool _mute = false;

		public int getVolume()
		{
			uint currentVolume = 0;
			waveOutGetVolume(IntPtr.Zero, out currentVolume);
			ushort calcVolume = (ushort)(currentVolume & 0x0000ffff);
			int volume = calcVolume / (ushort.MaxValue / 10);
			return volume;
		}

		public int setVolume(int volume)
		{
			int newVolume = ((ushort.MaxValue / 10) * volume);
			uint newVolumeAllChannels = (((uint)newVolume & 0x0000ffff) | ((uint)newVolume << 16));
			return waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
		}
		
		public void toggleMute()
		{
			if (_mute == false) {
				_mute = true;
				Debug.WriteLine("_mute false => true");
				return;
			}
			else if (_mute == true) {
				_mute = false;
				Debug.WriteLine("_mute true => false");
				return;
			}
		}

		public bool isMute()
		{
			_mute = true;
			return _mute;
		}
	}
}
