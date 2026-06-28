using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech
{
	public sealed class DictationRecognizer : IDisposable
	{
		public DictationRecognizer()
			: this(ConfidenceLevel.Medium, DictationTopicConstraint.Dictation)
		{
		}

		public DictationRecognizer(ConfidenceLevel confidenceLevel)
			: this(confidenceLevel, DictationTopicConstraint.Dictation)
		{
		}

		public DictationRecognizer(DictationTopicConstraint topic)
			: this(ConfidenceLevel.Medium, topic)
		{
		}

		public DictationRecognizer(ConfidenceLevel minimumConfidence, DictationTopicConstraint topic)
		{
			this.m_Recognizer = this.Create(minimumConfidence, topic);
		}

		private IntPtr Create(ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint)
		{
			IntPtr intPtr;
			DictationRecognizer.INTERNAL_CALL_Create(this, minimumConfidence, topicConstraint, out intPtr);
			return intPtr;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_Create(DictationRecognizer self, ConfidenceLevel minimumConfidence, DictationTopicConstraint topicConstraint, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Start(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr self);

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyThreaded(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SpeechSystemStatus GetStatus(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetAutoSilenceTimeoutSeconds(IntPtr self);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetAutoSilenceTimeoutSeconds(IntPtr self, float value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetInitialSilenceTimeoutSeconds(IntPtr self);

		[GeneratedByOldBindingsGenerator]
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

		public SpeechSystemStatus Status
		{
			get
			{
				return (!(this.m_Recognizer != IntPtr.Zero)) ? SpeechSystemStatus.Stopped : DictationRecognizer.GetStatus(this.m_Recognizer);
			}
		}

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

		public void Start()
		{
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				DictationRecognizer.Start(this.m_Recognizer);
			}
		}

		public void Stop()
		{
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				DictationRecognizer.Stop(this.m_Recognizer);
			}
		}

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

		public delegate void DictationHypothesisDelegate(string text);

		public delegate void DictationResultDelegate(string text, ConfidenceLevel confidence);

		public delegate void DictationCompletedDelegate(DictationCompletionCause cause);

		public delegate void DictationErrorHandler(string error, int hresult);
	}
}
