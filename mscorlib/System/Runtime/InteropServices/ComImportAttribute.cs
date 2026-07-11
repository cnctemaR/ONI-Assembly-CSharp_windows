using System;
using System.Reflection;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	public sealed class ComImportAttribute : Attribute
	{
		internal static Attribute GetCustomAttribute(RuntimeType type)
		{
			if ((type.Attributes & TypeAttributes.Import) == TypeAttributes.NotPublic)
			{
				return null;
			}
			return new ComImportAttribute();
		}

		internal static bool IsDefined(RuntimeType type)
		{
			return (type.Attributes & TypeAttributes.Import) > TypeAttributes.NotPublic;
		}
	}
}
