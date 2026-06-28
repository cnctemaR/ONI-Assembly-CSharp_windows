using System;
using System.ComponentModel;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Class)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class TypedRecordAttribute : Attribute
	{
	}
}
