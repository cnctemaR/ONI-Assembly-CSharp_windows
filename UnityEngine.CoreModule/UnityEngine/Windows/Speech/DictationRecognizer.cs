using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>DictationRecognizer listens to speech input and attempts to determine what phrase was uttered.</para>
	/// </summary>
	public sealed class DictationRecognizer : IDisposable
	{
		/// <summary>
		///   <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
		/// </summary>
		/// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
		/// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
		/// <param name="confidenceLevel"></param>
		public DictationRecognizer()
			: this(ConfidenceLevel.Medium, DictationTopicConstraint.Dictation)
		{
		}

		/// <summary>
		///   <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
		/// </summary>
		/// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
		/// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
		/// <param name="confidenceLevel"></param>
		public DictationRecognizer(ConfidenceLevel confidenceLevel)
			: this(confidenceLevel, DictationTopicConstraint.Dictation)
		{
		}

		/// <summary>
		///   <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
		/// </summary>
		/// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
		/// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
		/// <param name="confidenceLevel"></param>
		public DictationRecognizer(DictationTopicConstraint topic)
			: this(ConfidenceLevel.Medium, topic)
		{
		}

		/// <summary>
		///   <para>Create a DictationRecognizer with the specified minimum confidence and dictation topic constraint. Phrases under the specified minimum level will be ignored.</para>
		/// </summary>
		/// <param name="minimumConfidence">The confidence level at which the recognizer will begin accepting phrases.</param>
		/// <param name="topic">The dictation topic that this dictation recognizer should optimize its recognition for.</param>
		/// <param name="confidenceLevel"></param>
		public DictationRecognizer(ConfidenceLevel minimumConfidence, DictationTopicConstraint topic)
		{
			this.m_Recognizer = DictationRecognizer.Create(this, minimumConfidence, topic);
		}

		[NativeThrows]
		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(object self, ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint);

		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Start(IntPtr self);

		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop(IntPtr self);

		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr self);

		[ThreadSafe]
		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyThreaded(IntPtr self);

		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpeechSystemStatus GetStatus(IntPtr self);

		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAutoSilenceTimeoutSeconds(IntPtr self);

		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAutoSilenceTimeoutSeconds(IntPtr self, float value);

		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetInitialSilenceTimeoutSeconds(IntPtr self);

		[NativeHeader("PlatformDependent/Win/Bindings/SpeechBindings.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetInitialSilenceTimeoutSeconds(IntPtr self, float value);

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event DictationRecognizer.DictationHypothesisDelegate DictationHypothesis;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event DictationRecognizer.DictationResultDelegate DictationResult;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event DictationRecognizer.DictationCompletedDelegate DictationComplete;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event DictationRecognizer.DictationErrorHandler DictationError;

		/// <summary>
		///   <para>Indicates the status of dictation recognizer.</para>
		/// </summary>
		public SpeechSystemStatus Status
		{
			get
			{
				return (!(this.m_Recognizer != IntPtr.Zero)) ? SpeechSystemStatus.Stopped : DictationRecognizer.GetStatus(this.m_Recognizer);
			}
		}

		/// <summary>
		///   <para>The time length in seconds before dictation recognizer session ends due to lack of audio input.</para>
		/// </summary>
		public float AutoSilenceTimeoutSeconds
		{
			get
			{
				float num;
				if (this.m_Recognizer == IntPtr.Zero)
				{
					num = 0f;
				}
				else
				{
					num = DictationRecognizer.GetAutoSilenceTimeoutSeconds(this.m_Recognizer);
				}
				return num;
			}
			set
			{
				if (!(this.m_Recognizer == IntPtr.Zero))
				{
					DictationRecognizer.SetAutoSilenceTimeoutSeconds(this.m_Recognizer, value);
				}
			}
		}

		/// <summary>
		///   <para>The time length in seconds before dictation recognizer session ends due to lack of audio input in case there was no audio heard in the current session.</para>
		/// </summary>
		public float InitialSilenceTimeoutSeconds
		{
			get
			{
				float num;
				if (this.m_Recognizer == IntPtr.Zero)
				{
					num = 0f;
				}
				else
				{
					num = DictationRecognizer.GetInitialSilenceTimeoutSeconds(this.m_Recognizer);
				}
				return num;
			}
			set
			{
				if (!(this.m_Recognizer == IntPtr.Zero))
				{
					DictationRecognizer.SetInitialSilenceTimeoutSeconds(this.m_Recognizer, value);
				}
			}
		}

		~DictationRecognizer()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				DictationRecognizer.DestroyThreaded(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		///   <para>Starts the dictation recognization session. Dictation recognizer can only be started if PhraseRecognitionSystem is not running.</para>
		/// </summary>
		public void Start()
		{
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				DictationRecognizer.Start(this.m_Recognizer);
			}
		}

		/// <summary>
		///   <para>Stops the dictation recognization session.</para>
		/// </summary>
		public void Stop()
		{
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				DictationRecognizer.Stop(this.m_Recognizer);
			}
		}

		/// <summary>
		///   <para>Disposes the resources this dictation recognizer uses.</para>
		/// </summary>
		public void Dispose()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				DictationRecognizer.Destroy(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		[RequiredByNativeCode]
		private void DictationRecognizer_InvokeHypothesisGeneratedEvent(string keyword)
		{
			DictationRecognizer.DictationHypothesisDelegate dictationHypothesis = this.DictationHypothesis;
			if (dictationHypothesis != null)
			{
				dictationHypothesis(keyword);
			}
		}

		[RequiredByNativeCode]
		private void DictationRecognizer_InvokeResultGeneratedEvent(string keyword, ConfidenceLevel minimumConfidence)
		{
			DictationRecognizer.DictationResultDelegate dictationResult = this.DictationResult;
			if (dictationResult != null)
			{
				dictationResult(keyword, minimumConfidence);
			}
		}

		[RequiredByNativeCode]
		private void DictationRecognizer_InvokeCompletedEvent(DictationCompletionCause cause)
		{
			DictationRecognizer.DictationCompletedDelegate dictationComplete = this.DictationComplete;
			if (dictationComplete != null)
			{
				dictationComplete(cause);
			}
		}

		[RequiredByNativeCode]
		private void DictationRecognizer_InvokeErrorEvent(string error, int hresult)
		{
			DictationRecognizer.DictationErrorHandler dictationError = this.DictationError;
			if (dictationError != null)
			{
				dictationError(error, hresult);
			}
		}

		private IntPtr m_Recognizer;

		/// <summary>
		///   <para>Callback indicating a hypothesis change event. You should register with DictationHypothesis event.</para>
		/// </summary>
		/// <param name="text">The text that the recognizer believes may have been recognized.</param>
		public delegate void DictationHypothesisDelegate(string text);

		/// <summary>
		///   <para>Callback indicating a phrase has been recognized with the specified confidence level. You should register with DictationResult event.</para>
		/// </summary>
		/// <param name="text">The recognized text.</param>
		/// <param name="confidence">The confidence level at which the text was recognized.</param>
		public delegate void DictationResultDelegate(string text, ConfidenceLevel confidence);

		/// <summary>
		///   <para>Delegate for DictationComplete event.</para>
		/// </summary>
		/// <param name="cause">The cause of dictation session completion.</param>
		public delegate void DictationCompletedDelegate(DictationCompletionCause cause);

		/// <summary>
		///   <para>Delegate for DictationError event.</para>
		/// </summary>
		/// <param name="error">The error mesage.</param>
		/// <param name="hresult">HRESULT code that corresponds to the error.</param>
		public delegate void DictationErrorHandler(string error, int hresult);
	}
}
