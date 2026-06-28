using System;
using System.ComponentModel;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class FieldAttribute : Attribute
	{
	}
}
