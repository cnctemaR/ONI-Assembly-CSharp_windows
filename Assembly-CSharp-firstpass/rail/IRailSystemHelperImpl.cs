using System;

namespace rail
{
	public class IRailSystemHelperImpl : RailObject, IRailSystemHelper
	{
		internal IRailSystemHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailSystemHelperImpl()
		{
		}

		public virtual RailResult SetTerminationTimeoutOwnershipExpired(int timeout_seconds)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailSystemHelper_SetTerminationTimeoutOwnershipExpired(this.swigCPtr_, timeout_seconds);
		}

		public virtual RailSystemState GetPlatformSystemState()
		{
			return (RailSystemState)RAIL_API_PINVOKE.IRailSystemHelper_GetPlatformSystemState(this.swigCPtr_);
		}
	}
}
