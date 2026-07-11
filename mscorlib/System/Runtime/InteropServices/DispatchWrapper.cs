using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public sealed class DispatchWrapper
	{
		public DispatchWrapper(object obj)
		{
			Marshal.GetIDispatchForObject(obj);
			this.wrappedObject = obj;
		}

		public object WrappedObject
		{
			get
			{
				return this.wrappedObject;
			}
		}

		private object wrappedObject;
	}
}
