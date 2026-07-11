using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Restricts the value of an attribute to be taken from a list of values.</para>
	/// </summary>
	public class UxmlEnumeration : UxmlTypeRestriction
	{
		/// <summary>
		///   <para>Indicates whether the current UxmlEnumeration object is equal to another object of the same type.</para>
		/// </summary>
		/// <param name="other">The object to compare with.</param>
		/// <returns>
		///   <para>True if the otheer object is equal to this one.</para>
		/// </returns>
		public override bool Equals(UxmlTypeRestriction other)
		{
			UxmlEnumeration uxmlEnumeration = other as UxmlEnumeration;
			return uxmlEnumeration != null && this.values.All<string>(new Func<string, bool>(uxmlEnumeration.values.Contains)) && this.values.Count == uxmlEnumeration.values.Count;
		}

		/// <summary>
		///   <para>The list of values the attribute can take.</para>
		/// </summary>
		public List<string> values = new List<string>();
	}
}
