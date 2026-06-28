using System;

namespace System.Runtime.InteropServices
{
	[Serializable]
	public sealed class VariantWrapper
	{
		public VariantWrapper(object obj)
		{
			this._wrappedObject = obj;
		}

		public object WrappedObject
		{
			get
			{
				return this._wrappedObject;
			}
		}

		private object _wrappedObject;
	}
}
