using System;

namespace rail
{
	public class IRailGlobalAchievementImpl : RailObject, IRailGlobalAchievement, IRailComponent
	{
		internal IRailGlobalAchievementImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailGlobalAchievementImpl()
		{
		}

		public virtual RailResult AsyncRequestAchievement(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGlobalAchievement_AsyncRequestAchievement(this.swigCPtr_, user_data);
		}

		public virtual RailResult GetGlobalAchievedPercent(string name, out double percent)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGlobalAchievement_GetGlobalAchievedPercent(this.swigCPtr_, name, out percent);
		}

		public virtual RailResult GetGlobalAchievedPercentDescending(int index, out string name, out double percent)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGlobalAchievement_GetGlobalAchievedPercentDescending(this.swigCPtr_, index, intPtr, out percent);
			}
			finally
			{
				name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
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
