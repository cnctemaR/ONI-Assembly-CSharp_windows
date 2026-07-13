using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	public static class AudioExtensions
	{
		[NativeMethod(Name = "AudioSpeakerModeBindings::InternalIAudioSpeakerModeChannelCount", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int InternalIAudioSpeakerModeChannelCount(AudioSpeakerMode speakerMode);

		public static int ChannelCount(this AudioSpeakerMode speakerMode)
		{
			int num;
			switch (speakerMode)
			{
			case AudioSpeakerMode.Mono:
				num = 1;
				break;
			case AudioSpeakerMode.Stereo:
				num = 2;
				break;
			case AudioSpeakerMode.Quad:
				num = 4;
				break;
			case AudioSpeakerMode.Surround:
				num = 5;
				break;
			case AudioSpeakerMode.Mode5point1:
				num = 6;
				break;
			case AudioSpeakerMode.Mode7point1:
				num = 8;
				break;
			case AudioSpeakerMode.Prologic:
				num = 2;
				break;
			default:
				throw new ArgumentException("speakerMode");
			}
			return num;
		}
	}
}
