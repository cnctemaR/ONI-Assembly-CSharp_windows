using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Restricts the value of an attribute to be within the specified bounds.</para>
	/// </summary>
	public class UxmlValueBounds : UxmlTypeRestriction
	{
		/// <summary>
		///   <para>Indicates whether the current UxmlValueBounds object is equal to another object of the same type.</para>
		/// </summary>
		/// <param name="other">The object to compare with.</param>
		/// <returns>
		///   <para>True if the otheer object is equal to this one.</para>
		/// </returns>
		public override bool Equals(UxmlTypeRestriction other)
		{
			UxmlValueBounds uxmlValueBounds = other as UxmlValueBounds;
			return uxmlValueBounds != null && (this.min == uxmlValueBounds.min && this.max == uxmlValueBounds.max && this.excludeMin == uxmlValueBounds.excludeMin) && this.excludeMax == uxmlValueBounds.excludeMax;
		}

		/// <summary>
		///   <para>The minimum value for the attribute.</para>
		/// </summary>
		public string min;

		/// <summary>
		///   <para>The maximum value for the attribute.</para>
		/// </summary>
		public string max;

		/// <summary>
		///   <para>True if the bounds exclude min.</para>
		/// </summary>
		public bool excludeMin;

		/// <summary>
		///   <para>True if the bounds exclude max.</para>
		/// </summary>
		public bool excludeMax;
	}
}
