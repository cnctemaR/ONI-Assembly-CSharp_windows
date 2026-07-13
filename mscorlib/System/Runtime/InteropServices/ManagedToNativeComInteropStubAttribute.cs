using System;

namespace System.Runtime.InteropServices
{
	[ComVisible(false)]
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public sealed class ManagedToNativeComInteropStubAttribute : Attribute
	{
		public ManagedToNativeComInteropStubAttribute(Type classType, string methodName)
		{
			this._classType = classType;
			this._methodName = methodName;
		}

		public Type ClassType
		{
			get
			{
				return this._classType;
			}
		}

		public string MethodName
		{
			get
			{
				return this._methodName;
			}
		}

		internal Type _classType;

		internal string _methodName;
	}
}
