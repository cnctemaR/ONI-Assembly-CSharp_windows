using System;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
	internal class NativeThrowsAttribute : Attribute, IBindingsThrowsProviderAttribute, IBindingsAttribute
	{
		public bool ThrowsException { get; set; }

		public NativeThrowsAttribute()
		{
			this.ThrowsException = true;
		}

		public NativeThrowsAttribute(bool throwsException)
		{
			this.ThrowsException = throwsException;
		}
	}
}
