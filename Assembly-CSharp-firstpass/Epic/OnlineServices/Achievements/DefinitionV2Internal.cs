using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DefinitionV2Internal : IDisposable
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

		public string UnlockedDisplayName
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_UnlockedDisplayName, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_UnlockedDisplayName, value);
			}
		}

		public string UnlockedDescription
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_UnlockedDescription, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_UnlockedDescription, value);
			}
		}

		public string LockedDisplayName
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_LockedDisplayName, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_LockedDisplayName, value);
			}
		}

		public string LockedDescription
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_LockedDescription, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_LockedDescription, value);
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

		public string UnlockedIconURL
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_UnlockedIconURL, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_UnlockedIconURL, value);
			}
		}

		public string LockedIconURL
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_LockedIconURL, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_LockedIconURL, value);
			}
		}

		public bool IsHidden
		{
			get
			{
				bool @default = Helper.GetDefault<bool>();
				Helper.TryMarshalGet(this.m_IsHidden, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_IsHidden, value);
			}
		}

		public StatThresholdsInternal[] StatThresholds
		{
			get
			{
				StatThresholdsInternal[] @default = Helper.GetDefault<StatThresholdsInternal[]>();
				Helper.TryMarshalGet<StatThresholdsInternal>(this.m_StatThresholds, out @default, this.m_StatThresholdsCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<StatThresholdsInternal>(ref this.m_StatThresholds, value, out this.m_StatThresholdsCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_StatThresholds);
		}

		private int m_ApiVersion;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_AchievementId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UnlockedDisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UnlockedDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedDisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_FlavorText;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UnlockedIconURL;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedIconURL;

		private int m_IsHidden;

		private uint m_StatThresholdsCount;

		private IntPtr m_StatThresholds;
	}
}
