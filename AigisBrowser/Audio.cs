using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AigisBrowser
{
	public class Audio
	{
		[DllImport("winmm.dll")]
		public static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);

		[DllImport("winmm.dll")]
		public static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

		public bool _isMute = false;

		public int setVolume(int volume)
		{
			int newVolume = ((ushort.MaxValue / 10) * volume);
			uint newVolumeAllChannels = (((uint)newVolume & 0x0000ffff) | ((uint)newVolume << 16));
			return waveOutSetVolume(IntPtr.Zero, newVolumeAllChannels);
		}

		public int getVolume()
		{
			uint currentVolume = 0;
			waveOutGetVolume(IntPtr.Zero, out currentVolume);
			ushort calcVolume = (ushort)(currentVolume & 0x0000ffff);
			int volume = calcVolume / (ushort.MaxValue / 10);
			return volume;
		}
		
		public void toggleMute()
		{
			switch(_isMute)
			{
				case true:
					setVolume(0);
					break;
				case false:
					setVolume(10);
					break;
			}

			return;
		}
	}
}
