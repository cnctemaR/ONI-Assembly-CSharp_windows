using System;

namespace UnityEngine.UIElements
{
	[Flags]
	internal enum VisualNodeCallbackInterest
	{
		None = 0,
		ChildAdded = 1,
		ChildRemoved = 2
	}
}
