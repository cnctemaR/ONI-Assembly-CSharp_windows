using System;

namespace rail
{
	public class IRailAppsImpl : RailObject, IRailApps
	{
		internal IRailAppsImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailAppsImpl()
		{
		}

		public virtual bool IsGameInstalled(RailGameID game_id)
		{
			IntPtr intPtr = ((game_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGameID__SWIG_0());
			if (game_id != null)
			{
				RailConverter.Csharp2Cpp(game_id, intPtr);
			}
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailApps_IsGameInstalled(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailGameID(intPtr);
			}
			return flag;
		}

		public virtual RailResult AsyncQuerySubscribeWishPlayState(RailGameID game_id, string user_data)
		{
			IntPtr intPtr = ((game_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailGameID__SWIG_0());
			if (game_id != null)
			{
				RailConverter.Csharp2Cpp(game_id, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailApps_AsyncQuerySubscribeWishPlayState(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailGameID(intPtr);
			}
			return railResult;
		}
	}
}
