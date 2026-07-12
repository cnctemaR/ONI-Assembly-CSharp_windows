using System;
using System.Collections.Generic;

namespace rail
{
	public class IRailGameServerImpl : RailObject, IRailGameServer, IRailComponent
	{
		internal IRailGameServerImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailGameServerImpl()
		{
		}

		public virtual RailID GetGameServerRailID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailGameServer_GetGameServerRailID(this.swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(intPtr, railID);
			return railID;
		}

		public virtual RailResult GetGameServerName(out string name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_GetGameServerName(this.swigCPtr_, intPtr);
			}
			finally
			{
				name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult GetGameServerFullName(out string full_name)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_GetGameServerFullName(this.swigCPtr_, intPtr);
			}
			finally
			{
				full_name = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailID GetOwnerRailID()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailGameServer_GetOwnerRailID(this.swigCPtr_);
			RailID railID = new RailID();
			RailConverter.Cpp2Csharp(intPtr, railID);
			return railID;
		}

		public virtual bool SetHost(string game_server_host)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetHost(this.swigCPtr_, game_server_host);
		}

		public virtual bool GetHost(out string game_server_host)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_GetHost(this.swigCPtr_, intPtr);
			}
			finally
			{
				game_server_host = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return flag;
		}

		public virtual bool SetMapName(string game_server_map)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetMapName(this.swigCPtr_, game_server_map);
		}

		public virtual bool GetMapName(out string game_server_map)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_GetMapName(this.swigCPtr_, intPtr);
			}
			finally
			{
				game_server_map = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return flag;
		}

		public virtual bool SetPasswordProtect(bool has_password)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetPasswordProtect(this.swigCPtr_, has_password);
		}

		public virtual bool GetPasswordProtect()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetPasswordProtect(this.swigCPtr_);
		}

		public virtual bool SetMaxPlayers(uint max_player_count)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetMaxPlayers(this.swigCPtr_, max_player_count);
		}

		public virtual uint GetMaxPlayers()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetMaxPlayers(this.swigCPtr_);
		}

		public virtual bool SetBotPlayers(uint bot_player_count)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetBotPlayers(this.swigCPtr_, bot_player_count);
		}

		public virtual uint GetBotPlayers()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetBotPlayers(this.swigCPtr_);
		}

		public virtual bool SetGameServerDescription(string game_server_description)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetGameServerDescription(this.swigCPtr_, game_server_description);
		}

		public virtual bool GetGameServerDescription(out string game_server_description)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_GetGameServerDescription(this.swigCPtr_, intPtr);
			}
			finally
			{
				game_server_description = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return flag;
		}

		public virtual bool SetGameServerTags(string game_server_tags)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetGameServerTags(this.swigCPtr_, game_server_tags);
		}

		public virtual bool GetGameServerTags(out string game_server_tags)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_GetGameServerTags(this.swigCPtr_, intPtr);
			}
			finally
			{
				game_server_tags = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return flag;
		}

		public virtual bool SetMods(List<string> server_mods)
		{
			IntPtr intPtr = ((server_mods == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (server_mods != null)
			{
				RailConverter.Csharp2Cpp(server_mods, intPtr);
			}
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_SetMods(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return flag;
		}

		public virtual bool GetMods(List<string> server_mods)
		{
			IntPtr intPtr = ((server_mods == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_GetMods(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (server_mods != null)
				{
					RailConverter.Cpp2Csharp(intPtr, server_mods);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return flag;
		}

		public virtual bool SetSpectatorHost(string spectator_host)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetSpectatorHost(this.swigCPtr_, spectator_host);
		}

		public virtual bool GetSpectatorHost(out string spectator_host)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_GetSpectatorHost(this.swigCPtr_, intPtr);
			}
			finally
			{
				spectator_host = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return flag;
		}

		public virtual bool SetGameServerVersion(string version)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetGameServerVersion(this.swigCPtr_, version);
		}

		public virtual bool GetGameServerVersion(out string version)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_GetGameServerVersion(this.swigCPtr_, intPtr);
			}
			finally
			{
				version = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return flag;
		}

		public virtual bool SetIsFriendOnly(bool is_friend_only)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetIsFriendOnly(this.swigCPtr_, is_friend_only);
		}

		public virtual bool GetIsFriendOnly()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetIsFriendOnly(this.swigCPtr_);
		}

		public virtual bool ClearAllMetadata()
		{
			return RAIL_API_PINVOKE.IRailGameServer_ClearAllMetadata(this.swigCPtr_);
		}

		public virtual RailResult GetMetadata(string key, out string value)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_GetMetadata(this.swigCPtr_, key, intPtr);
			}
			finally
			{
				value = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult SetMetadata(string key, string value)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_SetMetadata(this.swigCPtr_, key, value);
		}

		public virtual RailResult AsyncSetMetadata(List<RailKeyValue> key_values, string user_data)
		{
			IntPtr intPtr = ((key_values == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailKeyValue__SWIG_0());
			if (key_values != null)
			{
				RailConverter.Csharp2Cpp(key_values, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncSetMetadata(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailKeyValue(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncGetMetadata(List<string> keys, string user_data)
		{
			IntPtr intPtr = ((keys == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailString__SWIG_0());
			if (keys != null)
			{
				RailConverter.Csharp2Cpp(keys, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncGetMetadata(this.swigCPtr_, intPtr, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayRailString(intPtr);
			}
			return railResult;
		}

		public virtual RailResult AsyncGetAllMetadata(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncGetAllMetadata(this.swigCPtr_, user_data);
		}

		public virtual RailResult AsyncAcquireGameServerSessionTicket(string user_data)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncAcquireGameServerSessionTicket(this.swigCPtr_, user_data);
		}

		public virtual RailResult AsyncStartSessionWithPlayer(RailSessionTicket player_ticket, RailID player_rail_id, string user_data)
		{
			IntPtr intPtr = ((player_ticket == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSessionTicket());
			if (player_ticket != null)
			{
				RailConverter.Csharp2Cpp(player_ticket, intPtr);
			}
			IntPtr intPtr2 = ((player_rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (player_rail_id != null)
			{
				RailConverter.Csharp2Cpp(player_rail_id, intPtr2);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_AsyncStartSessionWithPlayer(this.swigCPtr_, intPtr, intPtr2, user_data);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSessionTicket(intPtr);
				RAIL_API_PINVOKE.delete_RailID(intPtr2);
			}
			return railResult;
		}

		public virtual void TerminateSessionOfPlayer(RailID player_rail_id)
		{
			IntPtr intPtr = ((player_rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (player_rail_id != null)
			{
				RailConverter.Csharp2Cpp(player_rail_id, intPtr);
			}
			try
			{
				RAIL_API_PINVOKE.IRailGameServer_TerminateSessionOfPlayer(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
		}

		public virtual void AbandonGameServerSessionTicket(RailSessionTicket session_ticket)
		{
			IntPtr intPtr = ((session_ticket == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailSessionTicket());
			if (session_ticket != null)
			{
				RailConverter.Csharp2Cpp(session_ticket, intPtr);
			}
			try
			{
				RAIL_API_PINVOKE.IRailGameServer_AbandonGameServerSessionTicket(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailSessionTicket(intPtr);
			}
		}

		public virtual RailResult ReportPlayerJoinGameServer(List<GameServerPlayerInfo> player_infos)
		{
			IntPtr intPtr = ((player_infos == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayGameServerPlayerInfo__SWIG_0());
			if (player_infos != null)
			{
				RailConverter.Csharp2Cpp(player_infos, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_ReportPlayerJoinGameServer(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayGameServerPlayerInfo(intPtr);
			}
			return railResult;
		}

		public virtual RailResult ReportPlayerQuitGameServer(List<GameServerPlayerInfo> player_infos)
		{
			IntPtr intPtr = ((player_infos == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayGameServerPlayerInfo__SWIG_0());
			if (player_infos != null)
			{
				RailConverter.Csharp2Cpp(player_infos, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_ReportPlayerQuitGameServer(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayGameServerPlayerInfo(intPtr);
			}
			return railResult;
		}

		public virtual RailResult UpdateGameServerPlayerList(List<GameServerPlayerInfo> player_infos)
		{
			IntPtr intPtr = ((player_infos == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayGameServerPlayerInfo__SWIG_0());
			if (player_infos != null)
			{
				RailConverter.Csharp2Cpp(player_infos, intPtr);
			}
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_UpdateGameServerPlayerList(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailArrayGameServerPlayerInfo(intPtr);
			}
			return railResult;
		}

		public virtual uint GetCurrentPlayers()
		{
			return RAIL_API_PINVOKE.IRailGameServer_GetCurrentPlayers(this.swigCPtr_);
		}

		public virtual void RemoveAllPlayers()
		{
			RAIL_API_PINVOKE.IRailGameServer_RemoveAllPlayers(this.swigCPtr_);
		}

		public virtual RailResult RegisterToGameServerList()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_RegisterToGameServerList(this.swigCPtr_);
		}

		public virtual RailResult UnregisterFromGameServerList()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_UnregisterFromGameServerList(this.swigCPtr_);
		}

		public virtual RailResult CloseGameServer()
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_CloseGameServer(this.swigCPtr_);
		}

		public virtual RailResult GetFriendsInGameServer(List<RailID> friend_ids)
		{
			IntPtr intPtr = ((friend_ids == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailArrayRailID__SWIG_0());
			RailResult railResult;
			try
			{
				railResult = (RailResult)RAIL_API_PINVOKE.IRailGameServer_GetFriendsInGameServer(this.swigCPtr_, intPtr);
			}
			finally
			{
				if (friend_ids != null)
				{
					RailConverter.Cpp2Csharp(intPtr, friend_ids);
				}
				RAIL_API_PINVOKE.delete_RailArrayRailID(intPtr);
			}
			return railResult;
		}

		public virtual bool IsUserInGameServer(RailID user_rail_id)
		{
			IntPtr intPtr = ((user_rail_id == null) ? IntPtr.Zero : RAIL_API_PINVOKE.new_RailID__SWIG_0());
			if (user_rail_id != null)
			{
				RailConverter.Csharp2Cpp(user_rail_id, intPtr);
			}
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_IsUserInGameServer(this.swigCPtr_, intPtr);
			}
			finally
			{
				RAIL_API_PINVOKE.delete_RailID(intPtr);
			}
			return flag;
		}

		public virtual bool SetServerInfo(string server_info)
		{
			return RAIL_API_PINVOKE.IRailGameServer_SetServerInfo(this.swigCPtr_, server_info);
		}

		public virtual bool GetServerInfo(out string server_info)
		{
			IntPtr intPtr = RAIL_API_PINVOKE.new_RailString__SWIG_0();
			bool flag;
			try
			{
				flag = RAIL_API_PINVOKE.IRailGameServer_GetServerInfo(this.swigCPtr_, intPtr);
			}
			finally
			{
				server_info = UTF8Marshaler.MarshalNativeToString(RAIL_API_PINVOKE.RailString_c_str(intPtr));
				RAIL_API_PINVOKE.delete_RailString(intPtr);
			}
			return flag;
		}

		public virtual RailResult EnableTeamVoice(bool enable)
		{
			return (RailResult)RAIL_API_PINVOKE.IRailGameServer_EnableTeamVoice(this.swigCPtr_, enable);
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
