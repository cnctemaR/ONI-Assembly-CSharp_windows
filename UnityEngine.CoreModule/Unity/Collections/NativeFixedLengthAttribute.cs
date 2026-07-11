using System;
using UnityEngine.Scripting;

namespace Unity.Collections
{
	/// <summary>
	///   <para>The container has from start a size that will never change.</para>
	/// </summary>
	[RequiredByNativeCode]
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class NativeFixedLengthAttribute : Attribute
	{
		/// <summary>
		///   <para>The specified number of elements will never change.</para>
		/// </summary>
		/// <param name="fixedLength">The fixed number of elements in the container.</param>
		public NativeFixedLengthAttribute(int fixedLength)
		{
			this.FixedLength = fixedLength;
		}

		/// <summary>
		///   <para>The fixed number of elements in the container.</para>
		/// </summary>
		public int FixedLength;
	}
}
