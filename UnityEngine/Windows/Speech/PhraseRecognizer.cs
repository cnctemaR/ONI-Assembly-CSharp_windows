using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Windows.Speech
{
	public abstract class PhraseRecognizer : IDisposable
	{
		internal PhraseRecognizer()
		{
		}

		protected IntPtr CreateFromKeywords(string[] keywords, ConfidenceLevel minimumConfidence)
		{
			IntPtr intPtr;
			PhraseRecognizer.INTERNAL_CALL_CreateFromKeywords(this, keywords, minimumConfidence, out intPtr);
			return intPtr;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateFromKeywords(PhraseRecognizer self, string[] keywords, ConfidenceLevel minimumConfidence, out IntPtr value);

		protected IntPtr CreateFromGrammarFile(string grammarFilePath, ConfidenceLevel minimumConfidence)
		{
			IntPtr intPtr;
			PhraseRecognizer.INTERNAL_CALL_CreateFromGrammarFile(this, grammarFilePath, minimumConfidence, out intPtr);
			return intPtr;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateFromGrammarFile(PhraseRecognizer self, string grammarFilePath, ConfidenceLevel minimumConfidence, out IntPtr value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Start_Internal(IntPtr recognizer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Stop_Internal(IntPtr recognizer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsRunning_Internal(IntPtr recognizer);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr recognizer);

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyThreaded(IntPtr recognizer);

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event PhraseRecognizer.PhraseRecognizedDelegate OnPhraseRecognized;

		~PhraseRecognizer()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				PhraseRecognizer.DestroyThreaded(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
				GC.SuppressFinalize(this);
			}
		}

		public void Start()
		{
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				PhraseRecognizer.Start_Internal(this.m_Recognizer);
			}
		}

		public void Stop()
		{
			if (!(this.m_Recognizer == IntPtr.Zero))
			{
				PhraseRecognizer.Stop_Internal(this.m_Recognizer);
			}
		}

		public void Dispose()
		{
			if (this.m_Recognizer != IntPtr.Zero)
			{
				PhraseRecognizer.Destroy(this.m_Recognizer);
				this.m_Recognizer = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		public bool IsRunning
		{
			get
			{
				return this.m_Recognizer != IntPtr.Zero && PhraseRecognizer.IsRunning_Internal(this.m_Recognizer);
			}
		}

		[RequiredByNativeCode]
		private void InvokePhraseRecognizedEvent(string text, ConfidenceLevel confidence, SemanticMeaning[] semanticMeanings, long phraseStartFileTime, long phraseDurationTicks)
		{
			PhraseRecognizer.PhraseRecognizedDelegate onPhraseRecognized = this.OnPhraseRecognized;
			if (onPhraseRecognized != null)
			{
				onPhraseRecognized(new PhraseRecognizedEventArgs(text, confidence, semanticMeanings, DateTime.FromFileTime(phraseStartFileTime), TimeSpan.FromTicks(phraseDurationTicks)));
			}
		}

		[RequiredByNativeCode]
		private unsafe static SemanticMeaning[] MarshalSemanticMeaning(IntPtr keys, IntPtr values, IntPtr valueSizes, int valueCount)
		{
			SemanticMeaning[] array = new SemanticMeaning[valueCount];
			int num = 0;
			for (int i = 0; i < valueCount; i++)
			{
				uint num2 = *(uint*)((byte*)(void*)valueSizes + (IntPtr)i * 4);
				SemanticMeaning semanticMeaning = new SemanticMeaning
				{
					key = new string(*(IntPtr*)((byte*)(void*)keys + (IntPtr)i * (IntPtr)sizeof(char*))),
					values = new string[num2]
				};
				int num3 = 0;
				while ((long)num3 < (long)((ulong)num2))
				{
					semanticMeaning.values[num3] = new string(*(IntPtr*)((byte*)(void*)values + (IntPtr)(num + num3) * (IntPtr)sizeof(char*)));
					num3++;
				}
				array[i] = semanticMeaning;
				num += (int)num2;
			}
			return array;
		}

		protected IntPtr m_Recognizer;

		public delegate void PhraseRecognizedDelegate(PhraseRecognizedEventArgs args);
	}
}
