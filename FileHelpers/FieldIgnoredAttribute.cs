using System;
using System.ComponentModel;

namespace FileHelpers
{
	[AttributeUsage(AttributeTargets.Field)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("You must use [FieldNotInFile] instead", false)]
	public sealed class FieldIgnoredAttribute : FieldAttribute
	{
	}
}
