using System;
using System.Runtime.Serialization;
using System.Security;
using Unity;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	public sealed class RuntimeWrappedException : Exception
	{
		private RuntimeWrappedException(object thrownObject)
			: base(Environment.GetResourceString("An object that does not derive from System.Exception has been wrapped in a RuntimeWrappedException."))
		{
			base.SetErrorCode(-2146233026);
			this.m_wrappedException = thrownObject;
		}

		public object WrappedException
		{
			get
			{
				return this.m_wrappedException;
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
			info.AddValue("WrappedException", this.m_wrappedException, typeof(object));
		}

		internal RuntimeWrappedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.m_wrappedException = info.GetValue("WrappedException", typeof(object));
		}

		internal RuntimeWrappedException()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private object m_wrappedException;
	}
}
