using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>Phrase recognition system is responsible for managing phrase recognizers and dispatching recognition events to them.</para>
	/// </summary>
	public static class PhraseRecognitionSystem
	{
		/// <summary>
		///   <para>Returns whether speech recognition is supported on the machine that the application is running on.</para>
		/// </summary>
		public static extern bool isSupported
		{
			[ThreadSafe]
			[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns the current status of the phrase recognition system.</para>
		/// </summary>
		public static extern SpeechSystemStatus Status
		{
			[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Attempts to restart the phrase recognition system.</para>
		/// </summary>
		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Restart();

		/// <summary>
		///   <para>Shuts phrase recognition system down.</para>
		/// </summary>
		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
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

		/// <summary>
		///   <para>Delegate for OnError event.</para>
		/// </summary>
		/// <param name="errorCode">Error code for the error that occurred.</param>
		public delegate void ErrorDelegate(SpeechError errorCode);

		/// <summary>
		///   <para>Delegate for OnStatusChanged event.</para>
		/// </summary>
		/// <param name="status">The new status of the phrase recognition system.</param>
		public delegate void StatusDelegate(SpeechSystemStatus status);
	}
}
