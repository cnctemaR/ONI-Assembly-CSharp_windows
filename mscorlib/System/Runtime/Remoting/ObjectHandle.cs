using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting
{
	[ClassInterface(ClassInterfaceType.AutoDual)]
	[ComVisible(true)]
	public class ObjectHandle : MarshalByRefObject, IObjectHandle
	{
		public ObjectHandle(object o)
		{
			this._wrapped = o;
		}

		public override object InitializeLifetimeService()
		{
			return base.InitializeLifetimeService();
		}

		public object Unwrap()
		{
			return this._wrapped;
		}

		private object _wrapped;
	}
}
