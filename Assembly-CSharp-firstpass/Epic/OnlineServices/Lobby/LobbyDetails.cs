using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Lobby
{
	public sealed class LobbyDetails : Handle
	{
		public LobbyDetails(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public ProductUserId GetLobbyOwner(LobbyDetailsGetLobbyOwnerOptions options)
		{
			LobbyDetailsGetLobbyOwnerOptionsInternal lobbyDetailsGetLobbyOwnerOptionsInternal = Helper.CopyProperties<LobbyDetailsGetLobbyOwnerOptionsInternal>(options);
			IntPtr intPtr = LobbyDetails.EOS_LobbyDetails_GetLobbyOwner(base.InnerHandle, ref lobbyDetailsGetLobbyOwnerOptionsInternal);
			Helper.TryMarshalDispose<LobbyDetailsGetLobbyOwnerOptionsInternal>(ref lobbyDetailsGetLobbyOwnerOptionsInternal);
			ProductUserId @default = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet<ProductUserId>(intPtr, out @default);
			return @default;
		}

		public Result CopyInfo(LobbyDetailsCopyInfoOptions options, out LobbyDetailsInfo outLobbyDetailsInfo)
		{
			LobbyDetailsCopyInfoOptionsInternal lobbyDetailsCopyInfoOptionsInternal = Helper.CopyProperties<LobbyDetailsCopyInfoOptionsInternal>(options);
			outLobbyDetailsInfo = Helper.GetDefault<LobbyDetailsInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyDetails.EOS_LobbyDetails_CopyInfo(base.InnerHandle, ref lobbyDetailsCopyInfoOptionsInternal, ref zero);
			Helper.TryMarshalDispose<LobbyDetailsCopyInfoOptionsInternal>(ref lobbyDetailsCopyInfoOptionsInternal);
			if (Helper.TryMarshalGet<LobbyDetailsInfoInternal, LobbyDetailsInfo>(zero, out outLobbyDetailsInfo))
			{
				LobbyDetails.EOS_LobbyDetails_Info_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetAttributeCount(LobbyDetailsGetAttributeCountOptions options)
		{
			LobbyDetailsGetAttributeCountOptionsInternal lobbyDetailsGetAttributeCountOptionsInternal = Helper.CopyProperties<LobbyDetailsGetAttributeCountOptionsInternal>(options);
			uint num = LobbyDetails.EOS_LobbyDetails_GetAttributeCount(base.InnerHandle, ref lobbyDetailsGetAttributeCountOptionsInternal);
			Helper.TryMarshalDispose<LobbyDetailsGetAttributeCountOptionsInternal>(ref lobbyDetailsGetAttributeCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyAttributeByIndex(LobbyDetailsCopyAttributeByIndexOptions options, out Attribute outAttribute)
		{
			LobbyDetailsCopyAttributeByIndexOptionsInternal lobbyDetailsCopyAttributeByIndexOptionsInternal = Helper.CopyProperties<LobbyDetailsCopyAttributeByIndexOptionsInternal>(options);
			outAttribute = Helper.GetDefault<Attribute>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyDetails.EOS_LobbyDetails_CopyAttributeByIndex(base.InnerHandle, ref lobbyDetailsCopyAttributeByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<LobbyDetailsCopyAttributeByIndexOptionsInternal>(ref lobbyDetailsCopyAttributeByIndexOptionsInternal);
			if (Helper.TryMarshalGet<AttributeInternal, Attribute>(zero, out outAttribute))
			{
				LobbyDetails.EOS_Lobby_Attribute_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyAttributeByKey(LobbyDetailsCopyAttributeByKeyOptions options, out Attribute outAttribute)
		{
			LobbyDetailsCopyAttributeByKeyOptionsInternal lobbyDetailsCopyAttributeByKeyOptionsInternal = Helper.CopyProperties<LobbyDetailsCopyAttributeByKeyOptionsInternal>(options);
			outAttribute = Helper.GetDefault<Attribute>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyDetails.EOS_LobbyDetails_CopyAttributeByKey(base.InnerHandle, ref lobbyDetailsCopyAttributeByKeyOptionsInternal, ref zero);
			Helper.TryMarshalDispose<LobbyDetailsCopyAttributeByKeyOptionsInternal>(ref lobbyDetailsCopyAttributeByKeyOptionsInternal);
			if (Helper.TryMarshalGet<AttributeInternal, Attribute>(zero, out outAttribute))
			{
				LobbyDetails.EOS_Lobby_Attribute_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetMemberCount(LobbyDetailsGetMemberCountOptions options)
		{
			LobbyDetailsGetMemberCountOptionsInternal lobbyDetailsGetMemberCountOptionsInternal = Helper.CopyProperties<LobbyDetailsGetMemberCountOptionsInternal>(options);
			uint num = LobbyDetails.EOS_LobbyDetails_GetMemberCount(base.InnerHandle, ref lobbyDetailsGetMemberCountOptionsInternal);
			Helper.TryMarshalDispose<LobbyDetailsGetMemberCountOptionsInternal>(ref lobbyDetailsGetMemberCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public ProductUserId GetMemberByIndex(LobbyDetailsGetMemberByIndexOptions options)
		{
			LobbyDetailsGetMemberByIndexOptionsInternal lobbyDetailsGetMemberByIndexOptionsInternal = Helper.CopyProperties<LobbyDetailsGetMemberByIndexOptionsInternal>(options);
			IntPtr intPtr = LobbyDetails.EOS_LobbyDetails_GetMemberByIndex(base.InnerHandle, ref lobbyDetailsGetMemberByIndexOptionsInternal);
			Helper.TryMarshalDispose<LobbyDetailsGetMemberByIndexOptionsInternal>(ref lobbyDetailsGetMemberByIndexOptionsInternal);
			ProductUserId @default = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet<ProductUserId>(intPtr, out @default);
			return @default;
		}

		public uint GetMemberAttributeCount(LobbyDetailsGetMemberAttributeCountOptions options)
		{
			LobbyDetailsGetMemberAttributeCountOptionsInternal lobbyDetailsGetMemberAttributeCountOptionsInternal = Helper.CopyProperties<LobbyDetailsGetMemberAttributeCountOptionsInternal>(options);
			uint num = LobbyDetails.EOS_LobbyDetails_GetMemberAttributeCount(base.InnerHandle, ref lobbyDetailsGetMemberAttributeCountOptionsInternal);
			Helper.TryMarshalDispose<LobbyDetailsGetMemberAttributeCountOptionsInternal>(ref lobbyDetailsGetMemberAttributeCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyMemberAttributeByIndex(LobbyDetailsCopyMemberAttributeByIndexOptions options, out Attribute outAttribute)
		{
			LobbyDetailsCopyMemberAttributeByIndexOptionsInternal lobbyDetailsCopyMemberAttributeByIndexOptionsInternal = Helper.CopyProperties<LobbyDetailsCopyMemberAttributeByIndexOptionsInternal>(options);
			outAttribute = Helper.GetDefault<Attribute>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyDetails.EOS_LobbyDetails_CopyMemberAttributeByIndex(base.InnerHandle, ref lobbyDetailsCopyMemberAttributeByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<LobbyDetailsCopyMemberAttributeByIndexOptionsInternal>(ref lobbyDetailsCopyMemberAttributeByIndexOptionsInternal);
			if (Helper.TryMarshalGet<AttributeInternal, Attribute>(zero, out outAttribute))
			{
				LobbyDetails.EOS_Lobby_Attribute_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopyMemberAttributeByKey(LobbyDetailsCopyMemberAttributeByKeyOptions options, out Attribute outAttribute)
		{
			LobbyDetailsCopyMemberAttributeByKeyOptionsInternal lobbyDetailsCopyMemberAttributeByKeyOptionsInternal = Helper.CopyProperties<LobbyDetailsCopyMemberAttributeByKeyOptionsInternal>(options);
			outAttribute = Helper.GetDefault<Attribute>();
			IntPtr zero = IntPtr.Zero;
			Result result = LobbyDetails.EOS_LobbyDetails_CopyMemberAttributeByKey(base.InnerHandle, ref lobbyDetailsCopyMemberAttributeByKeyOptionsInternal, ref zero);
			Helper.TryMarshalDispose<LobbyDetailsCopyMemberAttributeByKeyOptionsInternal>(ref lobbyDetailsCopyMemberAttributeByKeyOptionsInternal);
			if (Helper.TryMarshalGet<AttributeInternal, Attribute>(zero, out outAttribute))
			{
				LobbyDetails.EOS_Lobby_Attribute_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			LobbyDetails.EOS_LobbyDetails_Release(base.InnerHandle);
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Lobby_Attribute_Release(IntPtr lobbyAttribute);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_LobbyDetails_Info_Release(IntPtr lobbyDetailsInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_LobbyDetails_Release(IntPtr lobbyHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyDetails_CopyMemberAttributeByKey(IntPtr handle, ref LobbyDetailsCopyMemberAttributeByKeyOptionsInternal options, ref IntPtr outAttribute);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyDetails_CopyMemberAttributeByIndex(IntPtr handle, ref LobbyDetailsCopyMemberAttributeByIndexOptionsInternal options, ref IntPtr outAttribute);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_LobbyDetails_GetMemberAttributeCount(IntPtr handle, ref LobbyDetailsGetMemberAttributeCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_LobbyDetails_GetMemberByIndex(IntPtr handle, ref LobbyDetailsGetMemberByIndexOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_LobbyDetails_GetMemberCount(IntPtr handle, ref LobbyDetailsGetMemberCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyDetails_CopyAttributeByKey(IntPtr handle, ref LobbyDetailsCopyAttributeByKeyOptionsInternal options, ref IntPtr outAttribute);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyDetails_CopyAttributeByIndex(IntPtr handle, ref LobbyDetailsCopyAttributeByIndexOptionsInternal options, ref IntPtr outAttribute);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_LobbyDetails_GetAttributeCount(IntPtr handle, ref LobbyDetailsGetAttributeCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_LobbyDetails_CopyInfo(IntPtr handle, ref LobbyDetailsCopyInfoOptionsInternal options, ref IntPtr outLobbyDetailsInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_LobbyDetails_GetLobbyOwner(IntPtr handle, ref LobbyDetailsGetLobbyOwnerOptionsInternal options);
	}
}
