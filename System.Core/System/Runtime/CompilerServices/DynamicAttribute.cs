using System;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
	public sealed class DynamicAttribute : Attribute
	{
		public DynamicAttribute()
		{
			this._transformFlags = new bool[] { true };
		}

		public DynamicAttribute(bool[] transformFlags)
		{
			if (transformFlags == null)
			{
				throw new ArgumentNullException("transformFlags");
			}
			this._transformFlags = transformFlags;
		}

		public IList<bool> TransformFlags
		{
			get
			{
				return Array.AsReadOnly<bool>(this._transformFlags);
			}
		}

		private readonly bool[] _transformFlags;
	}
}
