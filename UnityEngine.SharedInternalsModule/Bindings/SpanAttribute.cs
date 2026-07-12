using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	internal class SpanAttribute : Attribute, IBindingsMarshalAsSpan
	{
		public bool IsReadOnly { get; }

		public string SizeParameter { get; }

		public SpanAttribute(string sizeParameter, bool isReadOnly = false)
		{
			this.SizeParameter = sizeParameter;
			this.IsReadOnly = isReadOnly;
		}
	}
}
