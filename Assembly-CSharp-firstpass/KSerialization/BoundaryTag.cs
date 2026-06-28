using System;

namespace KSerialization
{
	public enum BoundaryTag : uint
	{
		DirectoryStart = 3235774464U,
		DirectoryEnd,
		TemplateStart = 3235774467U,
		TemplateEnd,
		FieldStart,
		FieldEnd,
		PropertyStart,
		PropertyEnd
	}
}
