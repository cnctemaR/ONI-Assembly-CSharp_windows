using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PlayerAchievementInternal : IDisposable
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

		public string AchievementId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_AchievementId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_AchievementId, value);
			}
		}

		public double Progress
		{
			get
			{
				double @default = Helper.GetDefault<double>();
				Helper.TryMarshalGet<double>(this.m_Progress, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<double>(ref this.m_Progress, value);
			}
		}

		public DateTimeOffset? UnlockTime
		{
			get
			{
				DateTimeOffset? @default = Helper.GetDefault<DateTimeOffset?>();
				Helper.TryMarshalGet(this.m_UnlockTime, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_UnlockTime, value);
			}
		}

		public PlayerStatInfoInternal[] StatInfo
		{
			get
			{
				PlayerStatInfoInternal[] @default = Helper.GetDefault<PlayerStatInfoInternal[]>();
				Helper.TryMarshalGet<PlayerStatInfoInternal>(this.m_StatInfo, out @default, this.m_StatInfoCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<PlayerStatInfoInternal>(ref this.m_StatInfo, value, out this.m_StatInfoCount);
			}
		}

		public string DisplayName
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_DisplayName, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_DisplayName, value);
			}
		}

		public string Description
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_Description, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_Description, value);
			}
		}

		public string IconURL
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_IconURL, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_IconURL, value);
			}
		}

		public string FlavorText
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_FlavorText, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_FlavorText, value);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_StatInfo);
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AchievementId;

		private double m_Progress;

		private long m_UnlockTime;

		private int m_StatInfoCount;

		private IntPtr m_StatInfo;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_DisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Description;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_IconURL;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_FlavorText;
	}
}
