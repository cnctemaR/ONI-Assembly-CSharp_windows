using System;
using System.Collections.Generic;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>KeywordRecognizer listens to speech input and attempts to match uttered phrases to a list of registered keywords.</para>
	/// </summary>
	public sealed class KeywordRecognizer : PhraseRecognizer
	{
		/// <summary>
		///   <para>Create a KeywordRecognizer which listens to specified keywords with the specified minimum confidence.  Phrases under the specified minimum level will be ignored.</para>
		/// </summary>
		/// <param name="keywords">The keywords that the recognizer will listen to.</param>
		/// <param name="minimumConfidence">The minimum confidence level of speech recognition that the recognizer will accept.</param>
		public KeywordRecognizer(string[] keywords)
			: this(keywords, ConfidenceLevel.Medium)
		{
		}

		/// <summary>
		///   <para>Create a KeywordRecognizer which listens to specified keywords with the specified minimum confidence.  Phrases under the specified minimum level will be ignored.</para>
		/// </summary>
		/// <param name="keywords">The keywords that the recognizer will listen to.</param>
		/// <param name="minimumConfidence">The minimum confidence level of speech recognition that the recognizer will accept.</param>
		public KeywordRecognizer(string[] keywords, ConfidenceLevel minimumConfidence)
		{
			if (keywords == null)
			{
				throw new ArgumentNullException("keywords");
			}
			if (keywords.Length == 0)
			{
				throw new ArgumentException("At least one keyword must be specified.", "keywords");
			}
			int num = keywords.Length;
			for (int i = 0; i < num; i++)
			{
				if (keywords[i] == null)
				{
					throw new ArgumentNullException(string.Format("Keyword at index {0} is null.", i));
				}
			}
			this.Keywords = keywords;
			this.m_Recognizer = PhraseRecognizer.CreateFromKeywords(this, keywords, minimumConfidence);
		}

		/// <summary>
		///   <para>Returns the list of keywords which was supplied when the keyword recognizer was created.</para>
		/// </summary>
		public IEnumerable<string> Keywords { get; private set; }
	}
}
