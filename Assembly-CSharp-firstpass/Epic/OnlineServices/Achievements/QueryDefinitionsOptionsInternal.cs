using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct QueryDefinitionsOptionsInternal : IDisposable
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

		public ProductUserId LocalUserId
		{
			get
			{
				ProductUserId @default = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet<ProductUserId>(this.m_LocalUserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_LocalUserId, value);
			}
		}

		public EpicAccountId EpicUserId_DEPRECATED
		{
			get
			{
				EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
				Helper.TryMarshalGet<EpicAccountId>(this.m_EpicUserId_DEPRECATED, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_EpicUserId_DEPRECATED, value);
			}
		}

		public string[] HiddenAchievementIds_DEPRECATED
		{
			get
			{
				string[] @default = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet<string>(this.m_HiddenAchievementIds_DEPRECATED, out @default, this.m_HiddenAchievementsCount_DEPRECATED);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_HiddenAchievementIds_DEPRECATED, value, out this.m_HiddenAchievementsCount_DEPRECATED);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_HiddenAchievementIds_DEPRECATED);
		}

		private int m_ApiVersion;

		private IntPtr m_LocalUserId;

		private IntPtr m_EpicUserId_DEPRECATED;

		private IntPtr m_HiddenAchievementIds_DEPRECATED;

		private uint m_HiddenAchievementsCount_DEPRECATED;
	}
}
