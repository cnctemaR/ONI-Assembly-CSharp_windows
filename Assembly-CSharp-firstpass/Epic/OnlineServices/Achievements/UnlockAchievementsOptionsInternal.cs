using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UnlockAchievementsOptionsInternal : IDisposable
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

		public ProductUserId UserId
		{
			get
			{
				ProductUserId @default = Helper.GetDefault<ProductUserId>();
				Helper.TryMarshalGet<ProductUserId>(this.m_UserId, out @default);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet(ref this.m_UserId, value);
			}
		}

		public string[] AchievementIds
		{
			get
			{
				string[] @default = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet<string>(this.m_AchievementIds, out @default, this.m_AchievementsCount);
				return @default;
			}
			set
			{
				Helper.TryMarshalSet<string>(ref this.m_AchievementIds, value, out this.m_AchievementsCount);
			}
		}

		public void Dispose()
		{
			Helper.TryMarshalDispose(ref this.m_AchievementIds);
		}

		private int m_ApiVersion;

		private IntPtr m_UserId;

		private IntPtr m_AchievementIds;

		private uint m_AchievementsCount;
	}
}
