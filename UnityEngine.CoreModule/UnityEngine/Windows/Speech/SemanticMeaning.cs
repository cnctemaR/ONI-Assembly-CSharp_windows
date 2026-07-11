using System;

namespace UnityEngine.Windows.Speech
{
	/// <summary>
	///   <para>Semantic meaning is a collection of semantic properties of a recognized phrase. These semantic properties can be specified in SRGS grammar files.</para>
	/// </summary>
	public struct SemanticMeaning
	{
		/// <summary>
		///   <para>A key of semaning meaning.</para>
		/// </summary>
		public string key;

		/// <summary>
		///   <para>Values of semantic property that the correspond to the semantic meaning key.</para>
		/// </summary>
		public string[] values;
	}
}
