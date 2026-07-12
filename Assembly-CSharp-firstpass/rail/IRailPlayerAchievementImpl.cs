using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailPlayerAchievementImpl : RailObject, IRailPlayerAchievement, IRailComponent
	{
		internal IRailPlayerAchievementImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailPlayerAchievementImpl()
		{
		}

		public virtual RailID GetRailID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailPlayerAchievement_GetRailID(this.swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(intPtr, railID);
			return railID;
		}

		public virtual RailResult AsyncRequestAchievement(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncRequestAchievement(this.swigCPtr_, user_data);
		}

		public virtual RailResult HasAchieved(string name, out bool achieved)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_HasAchieved(this.swigCPtr_, name, out achieved);
		}

		public virtual RailResult GetAchievementInfo(string name, out string achievement_info)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_GetAchievementInfo__SWIG_0(this.swigCPtr_, name, intPtr);
			}
			finally
			{
				achievement_info = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncTriggerAchievementProgress(string name, uint current_value, uint max_value, string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_0(this.swigCPtr_, name, current_value, max_value, user_data);
		}

		public virtual RailResult AsyncTriggerAchievementProgress(string name, uint current_value, uint max_value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_1(this.swigCPtr_, name, current_value, max_value);
		}

		public virtual RailResult AsyncTriggerAchievementProgress(string name, uint current_value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncTriggerAchievementProgress__SWIG_2(this.swigCPtr_, name, current_value);
		}

		public virtual RailResult MakeAchievement(string name)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_MakeAchievement(this.swigCPtr_, name);
		}

		public virtual RailResult CancelAchievement(string name)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_CancelAchievement(this.swigCPtr_, name);
		}

		public virtual RailResult AsyncStoreAchievement(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_AsyncStoreAchievement(this.swigCPtr_, user_data);
		}

		public virtual RailResult ResetAllAchievements()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_ResetAllAchievements(this.swigCPtr_);
		}

		public virtual RailResult GetAllAchievementsName(List<string> names)
		{
			IntPtr intPtr = ((names == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_GetAllAchievementsName(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (names != null)
				{
					RailConverter.Cpp2Csharp(intPtr, names);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetAchievementInfo(string name, RailPlayerAchievementInfo achievement_info)
		{
			IntPtr intPtr = ((achievement_info == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailPlayerAchievementInfo__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailPlayerAchievement_GetAchievementInfo__SWIG_1(this.swigCPtr_, name, intPtr);
			}
			finally
			{
				if (achievement_info != null)
				{
					RailConverter.Cpp2Csharp(intPtr, achievement_info);
				}
				RAIL_API_PINVOKE.delete_RailPlayerAchievementInfo(intPtr);
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
