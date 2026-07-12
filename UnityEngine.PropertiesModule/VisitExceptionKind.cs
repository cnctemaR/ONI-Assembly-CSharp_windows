using System;

namespace Unity.Properties
{
	[Flags]
	public enum VisitExceptionKind
	{
		None = 0,
		Internal = 1,
		Visitor = 2,
		All = 3
	}
}
