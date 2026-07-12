using System;
using System.Runtime.Serialization;
using System.Security;
using Unity;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	public sealed class RuntimeWrappedException : Exception
	{
		public RuntimeWrappedException(object thrownObject)
			: base("An object that does not derive from System.Exception has been wrapped in a RuntimeWrappedException.")
		{
			base.HResult = -2146233026;
			this._wrappedException = thrownObject;
		}

		private RuntimeWrappedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this._wrappedException = info.GetValue("WrappedException", typeof(object));
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("WrappedException", this._wrappedException, typeof(object));
		}

		public object WrappedException
		{
			get
			{
				return this._wrappedException;
			}
		}

		internal RuntimeWrappedException()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private object _wrappedException;
	}
}
