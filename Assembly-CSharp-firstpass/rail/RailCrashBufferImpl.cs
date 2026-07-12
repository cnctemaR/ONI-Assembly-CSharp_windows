using System;

namespace rail
{
	public class RailCrashBufferImpl : RailObject, RailCrashBuffer
	{
		internal RailCrashBufferImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~RailCrashBufferImpl()
		{
		}

		public virtual string GetData()
		{
			return UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailCrashBuffer_GetData(this.swigCPtr_));
		}

		public virtual uint GetBufferLength()
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_GetBufferLength(this.swigCPtr_);
		}

		public virtual uint GetValidLength()
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_GetValidLength(this.swigCPtr_);
		}

		public virtual uint SetData(string data, uint length, uint offset)
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_SetData__SWIG_0(this.swigCPtr_, data, length, offset);
		}

		public virtual uint SetData(string data, uint length)
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_SetData__SWIG_1(this.swigCPtr_, data, length);
		}

		public virtual uint AppendData(string data, uint length)
		{
			return RAIL_API_PINVOKE.RailCrashBuffer_AppendData(this.swigCPtr_, data, length);
		}
	}
}
