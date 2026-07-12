using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Parameter)]
	internal class NotNullAttribute : Attribute, IBindingsAttribute
	{
		public string Exception { get; set; }

		public NotNullAttribute(string exception = "ArgumentNullException")
		{
			this.Exception = exception;
		}
	}
}
