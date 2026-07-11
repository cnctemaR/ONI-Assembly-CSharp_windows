using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Represents a string as an int for efficient lookup and comparison. Use this for common PropertyNames.
	///
	/// Internally stores just an int to represent the string. A PropertyName can be created from a string but can not be converted back to a string. The same string always results in the same int representing that string. Thus this is a very efficient string representation in both memory and speed when all you need is comparison.
	///
	/// PropertyName is serializable.
	///
	/// ToString() is only implemented for debugging purposes in the editor it returns "theName:3737" in the player it returns "Unknown:3737".</para>
	/// </summary>
	[UsedByNativeCode]
	public struct PropertyName : IEquatable<PropertyName>
	{
		/// <summary>
		///   <para>Initializes the PropertyName using a string.</para>
		/// </summary>
		/// <param name="name"></param>
		public PropertyName(string name)
		{
			this = new PropertyName(PropertyNameUtils.PropertyNameFromString(name));
		}

		public PropertyName(PropertyName other)
		{
			this.id = other.id;
		}

		public PropertyName(int id)
		{
			this.id = id;
		}

		/// <summary>
		///   <para>Indicates whether the specified PropertyName is an Empty string.</para>
		/// </summary>
		/// <param name="prop"></param>
		public static bool IsNullOrEmpty(PropertyName prop)
		{
			return prop.id == 0;
		}

		public static bool operator ==(PropertyName lhs, PropertyName rhs)
		{
			return lhs.id == rhs.id;
		}

		public static bool operator !=(PropertyName lhs, PropertyName rhs)
		{
			return lhs.id != rhs.id;
		}

		/// <summary>
		///   <para>Returns the hash code for this PropertyName.</para>
		/// </summary>
		public override int GetHashCode()
		{
			return this.id;
		}

		/// <summary>
		///   <para>Determines whether this instance and a specified object, which must also be a PropertyName object, have the same value.</para>
		/// </summary>
		/// <param name="other"></param>
		public override bool Equals(object other)
		{
			return other is PropertyName && this.Equals((PropertyName)other);
		}

		public bool Equals(PropertyName other)
		{
			return this == other;
		}

		public static implicit operator PropertyName(string name)
		{
			return new PropertyName(name);
		}

		public static implicit operator PropertyName(int id)
		{
			return new PropertyName(id);
		}

		/// <summary>
		///   <para>For debugging purposes only. Returns the string value representing the string in the Editor.
		/// Returns "UnityEngine.PropertyName" in the player.</para>
		/// </summary>
		public override string ToString()
		{
			return string.Format("Unknown:{0}", this.id);
		}

		internal int id;
	}
}
