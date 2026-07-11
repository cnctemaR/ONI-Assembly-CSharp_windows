using System;

namespace UnityEngine.Windows.Speech
{
	public struct PhraseRecognizedEventArgs
	{
		internal PhraseRecognizedEventArgs(string text, ConfidenceLevel confidence, SemanticMeaning[] semanticMeanings, DateTime phraseStartTime, TimeSpan phraseDuration)
		{
			this.text = text;
			this.confidence = confidence;
			this.semanticMeanings = semanticMeanings;
			this.phraseStartTime = phraseStartTime;
			this.phraseDuration = phraseDuration;
		}

		public readonly ConfidenceLevel confidence;

		public readonly SemanticMeaning[] semanticMeanings;

		public readonly string text;

		public readonly DateTime phraseStartTime;

		public readonly TimeSpan phraseDuration;
	}
}
