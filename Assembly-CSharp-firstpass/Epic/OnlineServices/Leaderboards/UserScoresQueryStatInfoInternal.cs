using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Leaderboards
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UserScoresQueryStatInfoInternal : IDisposable
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

		public string StatName
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_StatName, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_StatName, value);
			}
		}

		public LeaderboardAggregation Aggregation
		{
			get
			{
				LeaderboardAggregation @default = Helper.GetDefault<LeaderboardAggregation>();
				Helper.TryMarshalGet<LeaderboardAggregation>(this.m_Aggregation, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<LeaderboardAggregation>(ref this.m_Aggregation, value);
			}
		}

		public void Dispose()
		{
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_StatName;

		private LeaderboardAggregation m_Aggregation;
	}
}
