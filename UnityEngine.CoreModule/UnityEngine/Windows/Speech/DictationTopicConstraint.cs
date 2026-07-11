using System;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>DictationTopicConstraint enum specifies the scenario for which a specific dictation recognizer should optimize.</para>
	/// </summary>
	public enum DictationTopicConstraint
	{
		/// <summary>
		///   <para>Dictation recognizer will optimize for web search scenario.</para>
		/// </summary>
		WebSearch,
		/// <summary>
		///   <para>Dictation recognizer will optimize for form-filling scenario.</para>
		/// </summary>
		Form,
		/// <summary>
		///   <para>Dictation recognizer will optimize for dictation scenario.</para>
		/// </summary>
		Dictation
	}
}
