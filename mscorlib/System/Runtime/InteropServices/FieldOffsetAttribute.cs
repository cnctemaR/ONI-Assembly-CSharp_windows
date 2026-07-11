using System;
using System.Reflection;
using System.Security;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class FieldOffsetAttribute : Attribute
	{
		[SecurityCritical]
		internal static Attribute GetCustomAttribute(RuntimeFieldInfo field)
		{
			int fieldOffset;
			if (field.DeclaringType != null && (fieldOffset = field.GetFieldOffset()) >= 0)
			{
				return new FieldOffsetAttribute(fieldOffset);
			}
			return null;
		}

		[SecurityCritical]
		internal static bool IsDefined(RuntimeFieldInfo field)
		{
			return FieldOffsetAttribute.GetCustomAttribute(field) != null;
		}

		public FieldOffsetAttribute(int offset)
		{
			this._val = offset;
		}

		public int Value
		{
			get
			{
				return this._val;
			}
		}

		internal int _val;
	}
}
