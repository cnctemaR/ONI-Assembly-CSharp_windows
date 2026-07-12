using System;

namespace rail
{
	public class IRailFileImpl : RailObject, IRailFile, IRailComponent
	{
		internal IRailFileImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailFileImpl()
		{
		}

		public virtual string GetFilename()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.IRailFile_GetFilename(this.swigCPtr_));
		}

		public virtual uint Read(byte[] buff, uint bytes_to_read, out RailResult result)
		{
			return RAIL_API_PINVOKE.IRailFile_Read__SWIG_0(this.swigCPtr_, buff, bytes_to_read, out result);
		}

		public virtual uint Read(byte[] buff, uint bytes_to_read)
		{
			return RAIL_API_PINVOKE.IRailFile_Read__SWIG_1(this.swigCPtr_, buff, bytes_to_read);
		}

		public virtual uint Write(byte[] buff, uint bytes_to_write, out RailResult result)
		{
			return RAIL_API_PINVOKE.IRailFile_Write__SWIG_0(this.swigCPtr_, buff, bytes_to_write, out result);
		}

		public virtual uint Write(byte[] buff, uint bytes_to_write)
		{
			return RAIL_API_PINVOKE.IRailFile_Write__SWIG_1(this.swigCPtr_, buff, bytes_to_write);
		}

		public virtual RailResult AsyncRead(uint bytes_to_read, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFile_AsyncRead(this.swigCPtr_, bytes_to_read, user_data);
		}

		public virtual RailResult AsyncWrite(byte[] buffer, uint bytes_to_write, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailFile_AsyncWrite(this.swigCPtr_, buffer, bytes_to_write, user_data);
		}

		public virtual uint GetSize()
		{
			return RAIL_API_PINVOKE.IRailFile_GetSize(this.swigCPtr_);
		}

		public virtual void Close()
		{
			RAIL_API_PINVOKE.IRailFile_Close(this.swigCPtr_);
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
