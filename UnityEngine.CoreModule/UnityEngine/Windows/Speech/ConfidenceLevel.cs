using System;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>Used by KeywordRecognizer, GrammarRecognizer, DictationRecognizer. Phrases under the specified minimum level will be ignored.</para>
	/// </summary>
	public enum ConfidenceLevel
	{
		/// <summary>
		///   <para>High confidence level.</para>
		/// </summary>
		High,
		/// <summary>
		///   <para>Medium confidence level.</para>
		/// </summary>
		Medium,
		/// <summary>
		///   <para>Low confidence level.</para>
		/// </summary>
		Low,
		/// <summary>
		///   <para>Everything is rejected.</para>
		/// </summary>
		Rejected
	}
}
