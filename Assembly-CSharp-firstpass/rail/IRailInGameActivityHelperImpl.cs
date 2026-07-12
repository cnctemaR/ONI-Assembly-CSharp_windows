using System;

namespace rail
{
	public class IRailInGameActivityHelperImpl : RailObject, IRailInGameActivityHelper
	{
		internal IRailInGameActivityHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailInGameActivityHelperImpl()
		{
		}

		public virtual RailResult AsyncQueryGameActivity(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailInGameActivityHelper_AsyncQueryGameActivity(this.swigCPtr_, user_data);
		}

		public virtual RailResult AsyncOpenDefaultGameActivityWindow(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailInGameActivityHelper_AsyncOpenDefaultGameActivityWindow(this.swigCPtr_, user_data);
		}

		public virtual RailResult AsyncOpenGameActivityWindow(ulong activity_id, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailInGameActivityHelper_AsyncOpenGameActivityWindow(this.swigCPtr_, activity_id, user_data);
		}
	}
}
