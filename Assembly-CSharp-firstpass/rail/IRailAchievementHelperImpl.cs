using System;

namespace rail
{
	public class IRailAchievementHelperImpl : RailObject, IRailAchievementHelper
	{
		internal IRailAchievementHelperImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailAchievementHelperImpl()
		{
		}

		public virtual IRailPlayerAchievement CreatePlayerAchievement(RailID player)
		{
			IntPtr intPtr = ((player == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (player != null)
			{
				RailConverter.Csharp2Cpp(player, intPtr);
			}
			IRailPlayerAchievement railPlayerAchievement;
			try
			{
				IntPtr intPtr2 = RAIL_API_PINVOKE.IRailAchievementHelper_CreatePlayerAchievement(this.swigCPtr_, intPtr);
				railPlayerAchievement = ((intPtr2 == IntPtr.Zero) ? null : new IRailPlayerAchievementImpl(intPtr2));
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
			return railPlayerAchievement;
		}

		public virtual IRailGlobalAchievement GetGlobalAchievement()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailAchievementHelper_GetGlobalAchievement(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailGlobalAchievementImpl(intPtr);
			}
			return null;
		}
	}
}
