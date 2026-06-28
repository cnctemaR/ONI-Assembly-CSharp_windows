using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech
{
	public static class PhraseRecognitionSystem
	{
		[ThreadAndSerializationSafe]
		public static extern bool isSupported
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static extern SpeechSystemStatus Status
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Restart();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Shutdown();

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event PhraseRecognitionSystem.ErrorDelegate OnError;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event PhraseRecognitionSystem.StatusDelegate OnStatusChanged;

		[RequiredByNativeCode]
		private static void PhraseRecognitionSystem_InvokeErrorEvent(SpeechError errorCode)
		{
			PhraseRecognitionSystem.ErrorDelegate onError = PhraseRecognitionSystem.OnError;
			if (onError != null)
			{
				onError(errorCode);
			}
		}

		[RequiredByNativeCode]
		private static void PhraseRecognitionSystem_InvokeStatusChangedEvent(SpeechSystemStatus status)
		{
			PhraseRecognitionSystem.StatusDelegate onStatusChanged = PhraseRecognitionSystem.OnStatusChanged;
			if (onStatusChanged != null)
			{
				onStatusChanged(status);
			}
		}

		public delegate void ErrorDelegate(SpeechError errorCode);

		public delegate void StatusDelegate(SpeechSystemStatus status);
	}
}
