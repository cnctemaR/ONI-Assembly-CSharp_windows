using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Restricts the value of an attribute to match a regular expression.</para>
	/// </summary>
	public class UxmlValueMatches : UxmlTypeRestriction
	{
		/// <summary>
		///   <para>Indicates whether the current UxmlValueMatches object is equal to another object of the same type.</para>
		/// </summary>
		/// <param name="other">The object to compare with.</param>
		/// <returns>
		///   <para>True if the otheer object is equal to this one.</para>
		/// </returns>
		public override bool Equals(UxmlTypeRestriction other)
		{
			UxmlValueMatches uxmlValueMatches = other as UxmlValueMatches;
			return uxmlValueMatches != null && this.regex == uxmlValueMatches.regex;
		}

		/// <summary>
		///   <para>The regular expression that should be matched by the value.</para>
		/// </summary>
		public string regex;
	}
}
