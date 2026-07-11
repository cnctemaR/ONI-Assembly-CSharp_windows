using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	public sealed class LobbyModification : Handle
	{
		public LobbyModification(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result SetPermissionLevel(LobbyModificationSetPermissionLevelOptions options)
		{
			LobbyModificationSetPermissionLevelOptionsInternal lobbyModificationSetPermissionLevelOptionsInternal = Helper.CopyProperties<LobbyModificationSetPermissionLevelOptionsInternal>(options);
			Result result = LobbyModification.EOS_LobbyModification_SetPermissionLevel(base.InnerHandle, ref lobbyModificationSetPermissionLevelOptionsInternal);
			Helper.TryMarshalDispose<LobbyModificationSetPermissionLevelOptionsInternal>(ref lobbyModificationSetPermissionLevelOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetMaxMembers(LobbyModificationSetMaxMembersOptions options)
		{
			LobbyModificationSetMaxMembersOptionsInternal lobbyModificationSetMaxMembersOptionsInternal = Helper.CopyProperties<LobbyModificationSetMaxMembersOptionsInternal>(options);
			Result result = LobbyModification.EOS_LobbyModification_SetMaxMembers(base.InnerHandle, ref lobbyModificationSetMaxMembersOptionsInternal);
			Helper.TryMarshalDispose<LobbyModificationSetMaxMembersOptionsInternal>(ref lobbyModificationSetMaxMembersOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result AddAttribute(LobbyModificationAddAttributeOptions options)
		{
			LobbyModificationAddAttributeOptionsInternal lobbyModificationAddAttributeOptionsInternal = Helper.CopyProperties<LobbyModificationAddAttributeOptionsInternal>(options);
			Result result = LobbyModification.EOS_LobbyModification_AddAttribute(base.InnerHandle, ref lobbyModificationAddAttributeOptionsInternal);
			Helper.TryMarshalDispose<LobbyModificationAddAttributeOptionsInternal>(ref lobbyModificationAddAttributeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result RemoveAttribute(LobbyModificationRemoveAttributeOptions options)
		{
			LobbyModificationRemoveAttributeOptionsInternal lobbyModificationRemoveAttributeOptionsInternal = Helper.CopyProperties<LobbyModificationRemoveAttributeOptionsInternal>(options);
			Result result = LobbyModification.EOS_LobbyModification_RemoveAttribute(base.InnerHandle, ref lobbyModificationRemoveAttributeOptionsInternal);
			Helper.TryMarshalDispose<LobbyModificationRemoveAttributeOptionsInternal>(ref lobbyModificationRemoveAttributeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result AddMemberAttribute(LobbyModificationAddMemberAttributeOptions options)
		{
			LobbyModificationAddMemberAttributeOptionsInternal lobbyModificationAddMemberAttributeOptionsInternal = Helper.CopyProperties<LobbyModificationAddMemberAttributeOptionsInternal>(options);
			Result result = LobbyModification.EOS_LobbyModification_AddMemberAttribute(base.InnerHandle, ref lobbyModificationAddMemberAttributeOptionsInternal);
			Helper.TryMarshalDispose<LobbyModificationAddMemberAttributeOptionsInternal>(ref lobbyModificationAddMemberAttributeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result RemoveMemberAttribute(LobbyModificationRemoveMemberAttributeOptions options)
		{
			LobbyModificationRemoveMemberAttributeOptionsInternal lobbyModificationRemoveMemberAttributeOptionsInternal = Helper.CopyProperties<LobbyModificationRemoveMemberAttributeOptionsInternal>(options);
			Result result = LobbyModification.EOS_LobbyModification_RemoveMemberAttribute(base.InnerHandle, ref lobbyModificationRemoveMemberAttributeOptionsInternal);
			Helper.TryMarshalDispose<LobbyModificationRemoveMemberAttributeOptionsInternal>(ref lobbyModificationRemoveMemberAttributeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			LobbyModification.EOS_LobbyModification_Release(base.InnerHandle);
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_LobbyModification_Release(IntPtr lobbyModificationHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyModification_RemoveMemberAttribute(IntPtr handle, ref LobbyModificationRemoveMemberAttributeOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyModification_AddMemberAttribute(IntPtr handle, ref LobbyModificationAddMemberAttributeOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyModification_RemoveAttribute(IntPtr handle, ref LobbyModificationRemoveAttributeOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyModification_AddAttribute(IntPtr handle, ref LobbyModificationAddAttributeOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyModification_SetMaxMembers(IntPtr handle, ref LobbyModificationSetMaxMembersOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyModification_SetPermissionLevel(IntPtr handle, ref LobbyModificationSetPermissionLevelOptionsInternal options);
	}
}
