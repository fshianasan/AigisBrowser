using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Vannatech.CoreAudio.Constants;
using Vannatech.CoreAudio.Enumerations;
using Vannatech.CoreAudio.Externals;
using Vannatech.CoreAudio.Interfaces;

namespace AigisBrowser
{
	public class Audio : IAudioSessionEvents
	{
		public Action<bool> MuteChangedEvent;

		private ISimpleAudioVolume AudioVolume;
		private IAudioSessionControl AudioControl;

		public Audio()
		{
			this.AudioVolume = null;

			var deviceEnumeratorType = Type.GetTypeFromCLSID(new Guid(ComCLSIDs.MMDeviceEnumeratorCLSID));
			var devenum = (IMMDeviceEnumerator)Activator.CreateInstance(deviceEnumeratorType);

			IMMDevice device = null;
			if (devenum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out device) == 0)
			{
				var iid = new Guid(ComIIDs.IAudioSessionManager2IID);
				object objAudioSessionManager = null;
				if (device.Activate(iid, (uint)CLSCTX.CLSCTX_INPROC_SERVER, IntPtr.Zero, out objAudioSessionManager) == 0)
				{
					var AudioSessionManager = objAudioSessionManager as IAudioSessionManager2;

					ISimpleAudioVolume AudioVolume;
					if (AudioSessionManager.GetSimpleAudioVolume(Guid.Empty, 0, out AudioVolume) == 0)
					{
						this.AudioVolume = AudioVolume;
					}

					IAudioSessionControl AudioControl;
					if (AudioSessionManager.GetAudioSessionControl(Guid.Empty, 0, out AudioControl) == 0)
					{
						this.AudioControl = AudioControl;
						this.AudioControl.RegisterAudioSessionNotification(this);
					}

				}
			}
		}

		[DllImport("winmm.dll")]
		public static extern int waveOutGetVolume(IntPtr h, out uint dwVolume);

		[DllImport("winmm.dll")]
		public static extern int waveOutSetVolume(IntPtr h, uint dwVolume);

		public bool _isMute;

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
			bool isMute = true;
			AudioVolume.GetMute(out isMute);
			AudioVolume.SetMute(!isMute, Guid.Empty);
		}

		public bool isMute()
		{
			bool isMute = true;
			AudioVolume.GetMute(out isMute);
			return isMute;
		}

		#region IAudioSessionEvents members

		int IAudioSessionEvents.OnDisplayNameChanged(string displayName, ref Guid eventContext)
		{
			return 0;
		}

		int IAudioSessionEvents.OnIconPathChanged(string iconPath, ref Guid eventContext)
		{
			return 0;
		}

		int IAudioSessionEvents.OnSimpleVolumeChanged(float volume, bool isMuted, ref Guid eventContext)
		{
			this.MuteChangedEvent?.Invoke(isMuted);
			return 0;
		}

		int IAudioSessionEvents.OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex, ref Guid eventContext)
		{
			return 0;
		}

		int IAudioSessionEvents.OnGroupingParamChanged(ref Guid groupingId, ref Guid eventContext)
		{
			return 0;
		}

		int IAudioSessionEvents.OnStateChanged(AudioSessionState state)
		{
			return 0;
		}

		int IAudioSessionEvents.OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
		{
			return 0;
		}

		#endregion
	}
}
