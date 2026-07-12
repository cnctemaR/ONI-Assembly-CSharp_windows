using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal interface IBindingsMarshalAsSpan
	{
		bool IsReadOnly { get; }

		string SizeParameter { get; }
	}
}
