using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Serializable]
	public sealed class UnknownWrapper
	{
		public UnknownWrapper(object obj)
		{
			this.InternalObject = obj;
		}

		public object WrappedObject
		{
			get
			{
				return this.InternalObject;
			}
		}

		private object InternalObject;
	}
}
