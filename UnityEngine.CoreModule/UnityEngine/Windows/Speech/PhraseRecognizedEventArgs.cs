using System;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>Provides information about a phrase recognized event.</para>
	/// </summary>
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

		/// <summary>
		///   <para>A measure of correct recognition certainty.</para>
		/// </summary>
		public readonly ConfidenceLevel confidence;

		/// <summary>
		///   <para>A semantic meaning of recognized phrase.</para>
		/// </summary>
		public readonly SemanticMeaning[] semanticMeanings;

		/// <summary>
		///   <para>The text that was recognized.</para>
		/// </summary>
		public readonly string text;

		/// <summary>
		///   <para>The moment in time when uttering of the phrase began.</para>
		/// </summary>
		public readonly DateTime phraseStartTime;

		/// <summary>
		///   <para>The time it took for the phrase to be uttered.</para>
		/// </summary>
		public readonly TimeSpan phraseDuration;
	}
}
