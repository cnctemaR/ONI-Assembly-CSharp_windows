using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public sealed class ReflectionTypeLoadException : SystemException, ISerializable
	{
		private ReflectionTypeLoadException()
			: base(Environment.GetResourceString("Unable to load one or more of the requested types. Retrieve the LoaderExceptions property for more information."))
		{
			base.SetErrorCode(-2146232830);
		}

		private ReflectionTypeLoadException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146232830);
		}

		public ReflectionTypeLoadException(Type[] classes, Exception[] exceptions)
			: base(null)
		{
			this._classes = classes;
			this._exceptions = exceptions;
			base.SetErrorCode(-2146232830);
		}

		public ReflectionTypeLoadException(Type[] classes, Exception[] exceptions, string message)
			: base(message)
		{
			this._classes = classes;
			this._exceptions = exceptions;
			base.SetErrorCode(-2146232830);
		}

		internal ReflectionTypeLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._classes = (Type[])info.GetValue("Types", typeof(Type[]));
			this._exceptions = (Exception[])info.GetValue("Exceptions", typeof(Exception[]));
		}

		public Type[] Types
		{
			get
			{
				return this._classes;
			}
		}

		public Exception[] LoaderExceptions
		{
			get
			{
				return this._exceptions;
			}
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("Types", this._classes, typeof(Type[]));
			info.AddValue("Exceptions", this._exceptions, typeof(Exception[]));
		}

		private Type[] _classes;

		private Exception[] _exceptions;
	}
}
