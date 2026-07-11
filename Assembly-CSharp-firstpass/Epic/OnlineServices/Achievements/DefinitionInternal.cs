using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DefinitionInternal : IDisposable
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

		public string HiddenDescription
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_HiddenDescription, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_HiddenDescription, value);
			}
		}

		public string CompletionDescription
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_CompletionDescription, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_CompletionDescription, value);
			}
		}

		public string UnlockedIconId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_UnlockedIconId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_UnlockedIconId, value);
			}
		}

		public string LockedIconId
		{
			get
			{
				string @default = Helper.GetDefault<string>();
				Helper.TryMarshalGet<string>(this.m_LockedIconId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_LockedIconId, value);
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
		private string m_DisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_Description;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedDisplayName;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_HiddenDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_CompletionDescription;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_UnlockedIconId;

		[MarshalAs(UnmanagedType.LPStr)]
		private string m_LockedIconId;

		private int m_IsHidden;

		private int m_StatThresholdsCount;

		private IntPtr m_StatThresholds;
	}
}
