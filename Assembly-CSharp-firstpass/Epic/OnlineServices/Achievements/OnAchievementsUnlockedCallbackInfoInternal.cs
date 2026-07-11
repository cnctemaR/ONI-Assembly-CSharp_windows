using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Achievements
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct OnAchievementsUnlockedCallbackInfoInternal : ICallbackInfo
	{
		public object ClientData
		{
			get
			{
				object @default = Helper.GetDefault<object>();
				Helper.TryMarshalGet(this.m_ClientData, out @default);
				return @default;
			}
		}

		public IntPtr ClientDataAddress
		{
			get
			{
				return this.m_ClientData;
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
		}

		public string[] AchievementIds
		{
			get
			{
				string[] @default = Helper.GetDefault<string[]>();
				Helper.TryMarshalGet<string>(this.m_AchievementIds, out @default, this.m_AchievementsCount);
				return @default;
			}
		}

		private IntPtr m_ClientData;

		private IntPtr m_UserId;

		private uint m_AchievementsCount;

		private IntPtr m_AchievementIds;
	}
}
