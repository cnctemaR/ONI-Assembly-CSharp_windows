using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Base class to restricts the value of an attribute.</para>
	/// </summary>
	public abstract class UxmlTypeRestriction : IEquatable<UxmlTypeRestriction>
	{
		/// <summary>
		///   <para>Indicates whether the current UxmlTypeRestriction object is equal to another object of the same type.</para>
		/// </summary>
		/// <param name="other">The object to compare with.</param>
		/// <returns>
		///   <para>True if the otheer object is equal to this one.</para>
		/// </returns>
		public virtual bool Equals(UxmlTypeRestriction other)
		{
			return this == other;
		}
	}
}
