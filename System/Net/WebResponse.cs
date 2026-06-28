using System;
using System.IO;
using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public abstract class WebResponse : MarshalByRefObject, IDisposable, ISerializable
	{
		protected WebResponse()
		{
		}

		protected WebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw new NotSupportedException();
		}

		void IDisposable.Dispose()
		{
			this.Close();
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw new NotSupportedException();
		}

		public virtual long ContentLength
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public virtual string ContentType
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public virtual WebHeaderCollection Headers
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[global::System.MonoTODO]
		public virtual bool IsFromCache
		{
			get
			{
				throw WebResponse.GetMustImplement();
			}
		}

		[global::System.MonoTODO]
		public virtual bool IsMutuallyAuthenticated
		{
			get
			{
				throw WebResponse.GetMustImplement();
			}
		}

		public virtual global::System.Uri ResponseUri
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public virtual void Close()
		{
			throw new NotSupportedException();
		}

		public virtual Stream GetResponseStream()
		{
			throw new NotSupportedException();
		}

		[global::System.MonoTODO]
		protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw WebResponse.GetMustImplement();
		}
	}
}
