using System;

namespace rail
{
	public class IRailStreamFileImpl : RailObject, IRailStreamFile, IRailComponent
	{
		internal IRailStreamFileImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailStreamFileImpl()
		{
		}

		public virtual string GetFilename()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailStreamFile_GetFilename(this.swigCPtr_));
		}

		public virtual RailResult AsyncRead(int offset, uint bytes_to_read, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStreamFile_AsyncRead(this.swigCPtr_, offset, bytes_to_read, user_data);
		}

		public virtual RailResult AsyncWrite(byte[] buff, uint bytes_to_write, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStreamFile_AsyncWrite(this.swigCPtr_, buff, bytes_to_write, user_data);
		}

		public virtual ulong GetSize()
		{
			return RAIL_API_PINVOKE.IRailStreamFile_GetSize(this.swigCPtr_);
		}

		public virtual RailResult Close()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailStreamFile_Close(this.swigCPtr_);
		}

		public virtual void Cancel()
		{
			RAIL_API_PINVOKE.IRailStreamFile_Cancel(this.swigCPtr_);
		}

		public virtual ulong GetComponentVersion()
		{
			return RAIL_API_PINVOKE.IRailComponent_GetComponentVersion(this.swigCPtr_);
		}

		public virtual void Release()
		{
			RAIL_API_PINVOKE.IRailComponent_Release(this.swigCPtr_);
		}
	}
}
