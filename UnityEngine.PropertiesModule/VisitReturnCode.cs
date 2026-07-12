using System;

namespace Unity.Properties
{
	public enum VisitReturnCode
	{
		Ok,
		NullContainer,
		InvalidContainerType,
		MissingPropertyBag,
		InvalidPath,
		InvalidCast,
		AccessViolation
	}
}
