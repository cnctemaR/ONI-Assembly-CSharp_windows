using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryLeaderboardUserScoresOptionsInternal : IDisposable
	{
		public int ApiVersion
		{
			get
			{
				int @default = Helper.GetDefault<int>();
				Helper.TryMarshalGet<int>(this.m_ApiVersion, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<int>(ref this.m_ApiVersion, value);
			}
		}

		public ProductUserId[] UserIds
		{
			get
			{
				ProductUserId[] @default = Helper.GetDefault<ProductUserId[]>();
				Helper.TryMarshalGet<ProductUserId>(this.m_UserIds, out @default, this.m_UserIdsCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<ProductUserId>(ref this.m_UserIds, value, out this.m_UserIdsCount);
			}
		}

		public UserScoresQueryStatInfoInternal[] StatInfo
		{
			get
			{
				UserScoresQueryStatInfoInternal[] @default = Helper.GetDefault<UserScoresQueryStatInfoInternal[]>();
				Helper.TryMarshalGet<UserScoresQueryStatInfoInternal>(this.m_StatInfo, out @default, this.m_StatInfoCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<UserScoresQueryStatInfoInternal>(ref this.m_StatInfo, value, out this.m_StatInfoCount);
			}
		}

		public DateTimeOffset? StartTime
		{
			get
			{
				DateTimeOffset? @default = Helper.GetDefault<DateTimeOffset?>();
				Helper.TryMarshalGet(this.m_StartTime, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_StartTime, value);
			}
		}

		public DateTimeOffset? EndTime
		{
			get
			{
				DateTimeOffset? @default = Helper.GetDefault<DateTimeOffset?>();
				Helper.TryMarshalGet(this.m_EndTime, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_EndTime, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_UserIds);
			Helper.TryMarshalDispose(ref this.m_StatInfo);
		}

		private int m_ApiVersion;

		private IntPtr m_UserIds;

		private uint m_UserIdsCount;

		private IntPtr m_StatInfo;

		private uint m_StatInfoCount;

		private long m_StartTime;

		private long m_EndTime;
	}
}
