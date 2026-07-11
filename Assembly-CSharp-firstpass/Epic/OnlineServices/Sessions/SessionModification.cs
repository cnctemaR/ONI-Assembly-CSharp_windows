using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	public sealed class SessionModification : Handle
	{
		public SessionModification(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result SetBucketId(SessionModificationSetBucketIdOptions options)
		{
			SessionModificationSetBucketIdOptionsInternal sessionModificationSetBucketIdOptionsInternal = Helper.CopyProperties<SessionModificationSetBucketIdOptionsInternal>(options);
			Result result = SessionModification.EOS_SessionModification_SetBucketId(base.InnerHandle, ref sessionModificationSetBucketIdOptionsInternal);
			Helper.TryMarshalDispose<SessionModificationSetBucketIdOptionsInternal>(ref sessionModificationSetBucketIdOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetHostAddress(SessionModificationSetHostAddressOptions options)
		{
			SessionModificationSetHostAddressOptionsInternal sessionModificationSetHostAddressOptionsInternal = Helper.CopyProperties<SessionModificationSetHostAddressOptionsInternal>(options);
			Result result = SessionModification.EOS_SessionModification_SetHostAddress(base.InnerHandle, ref sessionModificationSetHostAddressOptionsInternal);
			Helper.TryMarshalDispose<SessionModificationSetHostAddressOptionsInternal>(ref sessionModificationSetHostAddressOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetPermissionLevel(SessionModificationSetPermissionLevelOptions options)
		{
			SessionModificationSetPermissionLevelOptionsInternal sessionModificationSetPermissionLevelOptionsInternal = Helper.CopyProperties<SessionModificationSetPermissionLevelOptionsInternal>(options);
			Result result = SessionModification.EOS_SessionModification_SetPermissionLevel(base.InnerHandle, ref sessionModificationSetPermissionLevelOptionsInternal);
			Helper.TryMarshalDispose<SessionModificationSetPermissionLevelOptionsInternal>(ref sessionModificationSetPermissionLevelOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetJoinInProgressAllowed(SessionModificationSetJoinInProgressAllowedOptions options)
		{
			SessionModificationSetJoinInProgressAllowedOptionsInternal sessionModificationSetJoinInProgressAllowedOptionsInternal = Helper.CopyProperties<SessionModificationSetJoinInProgressAllowedOptionsInternal>(options);
			Result result = SessionModification.EOS_SessionModification_SetJoinInProgressAllowed(base.InnerHandle, ref sessionModificationSetJoinInProgressAllowedOptionsInternal);
			Helper.TryMarshalDispose<SessionModificationSetJoinInProgressAllowedOptionsInternal>(ref sessionModificationSetJoinInProgressAllowedOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetMaxPlayers(SessionModificationSetMaxPlayersOptions options)
		{
			SessionModificationSetMaxPlayersOptionsInternal sessionModificationSetMaxPlayersOptionsInternal = Helper.CopyProperties<SessionModificationSetMaxPlayersOptionsInternal>(options);
			Result result = SessionModification.EOS_SessionModification_SetMaxPlayers(base.InnerHandle, ref sessionModificationSetMaxPlayersOptionsInternal);
			Helper.TryMarshalDispose<SessionModificationSetMaxPlayersOptionsInternal>(ref sessionModificationSetMaxPlayersOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetInvitesAllowed(SessionModificationSetInvitesAllowedOptions options)
		{
			SessionModificationSetInvitesAllowedOptionsInternal sessionModificationSetInvitesAllowedOptionsInternal = Helper.CopyProperties<SessionModificationSetInvitesAllowedOptionsInternal>(options);
			Result result = SessionModification.EOS_SessionModification_SetInvitesAllowed(base.InnerHandle, ref sessionModificationSetInvitesAllowedOptionsInternal);
			Helper.TryMarshalDispose<SessionModificationSetInvitesAllowedOptionsInternal>(ref sessionModificationSetInvitesAllowedOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result AddAttribute(SessionModificationAddAttributeOptions options)
		{
			SessionModificationAddAttributeOptionsInternal sessionModificationAddAttributeOptionsInternal = Helper.CopyProperties<SessionModificationAddAttributeOptionsInternal>(options);
			Result result = SessionModification.EOS_SessionModification_AddAttribute(base.InnerHandle, ref sessionModificationAddAttributeOptionsInternal);
			Helper.TryMarshalDispose<SessionModificationAddAttributeOptionsInternal>(ref sessionModificationAddAttributeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result RemoveAttribute(SessionModificationRemoveAttributeOptions options)
		{
			SessionModificationRemoveAttributeOptionsInternal sessionModificationRemoveAttributeOptionsInternal = Helper.CopyProperties<SessionModificationRemoveAttributeOptionsInternal>(options);
			Result result = SessionModification.EOS_SessionModification_RemoveAttribute(base.InnerHandle, ref sessionModificationRemoveAttributeOptionsInternal);
			Helper.TryMarshalDispose<SessionModificationRemoveAttributeOptionsInternal>(ref sessionModificationRemoveAttributeOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			SessionModification.EOS_SessionModification_Release(base.InnerHandle);
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_SessionModification_Release(IntPtr sessionModificationHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionModification_RemoveAttribute(IntPtr handle, ref SessionModificationRemoveAttributeOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionModification_AddAttribute(IntPtr handle, ref SessionModificationAddAttributeOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionModification_SetInvitesAllowed(IntPtr handle, ref SessionModificationSetInvitesAllowedOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionModification_SetMaxPlayers(IntPtr handle, ref SessionModificationSetMaxPlayersOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionModification_SetJoinInProgressAllowed(IntPtr handle, ref SessionModificationSetJoinInProgressAllowedOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionModification_SetPermissionLevel(IntPtr handle, ref SessionModificationSetPermissionLevelOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionModification_SetHostAddress(IntPtr handle, ref SessionModificationSetHostAddressOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionModification_SetBucketId(IntPtr handle, ref SessionModificationSetBucketIdOptionsInternal options);
	}
}
