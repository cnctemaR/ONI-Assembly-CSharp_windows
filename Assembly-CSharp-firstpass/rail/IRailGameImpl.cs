using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailGameImpl : RailObject, IRailGame
	{
		internal IRailGameImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailGameImpl()
		{
		}

		public virtual RailGameID GetGameID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailGame_GetGameID(this.swigCPtr_);
			RailGameID railGameID = new RailGameID();
			RailConverter.Cpp2Csharp(intPtr, railGameID);
			return railGameID;
		}

		public virtual RailResult ReportGameContentDamaged(EnumRailGameContentDamageFlag flag)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGame_ReportGameContentDamaged(this.swigCPtr_, (int)flag);
		}

		public virtual RailResult GetGameInstallPath(out string app_path)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGame_GetGameInstallPath(this.swigCPtr_, intPtr);
			}
			finally
			{
				app_path = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncQuerySubscribeWishPlayState(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGame_AsyncQuerySubscribeWishPlayState(this.swigCPtr_, user_data);
		}

		public virtual RailResult GetPlayerSelectedLanguageCode(out string language_code)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGame_GetPlayerSelectedLanguageCode(this.swigCPtr_, intPtr);
			}
			finally
			{
				language_code = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetGameSupportedLanguageCodes(List<string> language_codes)
		{
			IntPtr intPtr = ((language_codes == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGame_GetGameSupportedLanguageCodes(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (language_codes != null)
				{
					RailConverter.Cpp2Csharp(intPtr, language_codes);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult SetGameState(EnumRailGamePlayingState game_state)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGame_SetGameState(this.swigCPtr_, (int)game_state);
		}

		public virtual RailResult GetGameState(out EnumRailGamePlayingState game_state)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.NewInt();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGame_GetGameState(this.swigCPtr_, intPtr);
			}
			finally
			{
				game_state = (EnumRailGamePlayingState)RAIL_API_PINVOKE.GetInt(intPtr);
				RAIL_API_PINVOKE.DeleteInt(intPtr);
			}
			return railResult;
		}

		public virtual RailResult RegisterGameDefineGamePlayingState(List<RailGameDefineGamePlayingState> game_playing_states)
		{
			IntPtr intPtr = ((game_playing_states == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailGameDefineGamePlayingState__SWIG_0());
			if (game_playing_states != null)
			{
				RailConverter.Csharp2Cpp(game_playing_states, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGame_RegisterGameDefineGamePlayingState(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailGameDefineGamePlayingState(intPtr);
			}
			return railResult;
		}

		public virtual RailResult SetGameDefineGamePlayingState(uint game_playing_state)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGame_SetGameDefineGamePlayingState(this.swigCPtr_, game_playing_state);
		}

		public virtual RailResult GetGameDefineGamePlayingState(out uint game_playing_state)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGame_GetGameDefineGamePlayingState(this.swigCPtr_, out game_playing_state);
		}

		public virtual RailResult GetBranchBuildNumber(out string build_number)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGame_GetBranchBuildNumber(this.swigCPtr_, intPtr);
			}
			finally
			{
				build_number = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetCurrentBranchInfo(RailBranchInfo branch_info)
		{
			IntPtr intPtr = ((branch_info == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailBranchInfo__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGame_GetCurrentBranchInfo(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (branch_info != null)
				{
					RailConverter.Cpp2Csharp(intPtr, branch_info);
				}
				RAIL_API_PINVOKE.delete_RailBranchInfo(intPtr);
			}
			return railResult;
		}

		public virtual RailResult StartGameTimeCounting(string counting_key)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGame_StartGameTimeCounting(this.swigCPtr_, counting_key);
		}

		public virtual RailResult EndGameTimeCounting(string counting_key)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGame_EndGameTimeCounting(this.swigCPtr_, counting_key);
		}

		public virtual RailID GetGamePurchasePlayerRailID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailGame_GetGamePurchasePlayerRailID(this.swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(intPtr, railID);
			return railID;
		}

		public virtual uint GetGameEarliestPurchaseTime()
		{
			return RAIL_API_PINVOKE.IRailGame_GetGameEarliestPurchaseTime(this.swigCPtr_);
		}

		public virtual uint GetTimeCountSinceGameActivated()
		{
			return RAIL_API_PINVOKE.IRailGame_GetTimeCountSinceGameActivated(this.swigCPtr_);
		}

		public virtual uint GetTimeCountSinceLastMouseMoved()
		{
			return RAIL_API_PINVOKE.IRailGame_GetTimeCountSinceLastMouseMoved(this.swigCPtr_);
		}
	}
}
