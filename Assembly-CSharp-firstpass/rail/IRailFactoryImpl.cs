using System;

namespace rail
{
	public class IRailFactoryImpl : RailObject, IRailFactory
	{
		internal IRailFactoryImpl(IntPtr cPtr)
		{
			this.swigCPtr_ = cPtr;
		}

		~IRailFactoryImpl()
		{
		}

		public virtual IRailPlayer RailPlayer()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailPlayer(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailPlayerImpl(intPtr);
			}
			return null;
		}

		public virtual IRailUsersHelper RailUsersHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailUsersHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailUsersHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailFriends RailFriends()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailFriends(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFriendsImpl(intPtr);
			}
			return null;
		}

		public virtual IRailFloatingWindow RailFloatingWindow()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailFloatingWindow(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailFloatingWindowImpl(intPtr);
			}
			return null;
		}

		public virtual IRailBrowserHelper RailBrowserHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailBrowserHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailBrowserHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailInGamePurchase RailInGamePurchase()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailInGamePurchase(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailInGamePurchaseImpl(intPtr);
			}
			return null;
		}

		public virtual IRailInGameCoin RailInGameCoin()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailInGameCoin(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailInGameCoinImpl(intPtr);
			}
			return null;
		}

		public virtual IRailRoomHelper RailRoomHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailRoomHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailRoomHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailGameServerHelper RailGameServerHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailGameServerHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailGameServerHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailStorageHelper RailStorageHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailStorageHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailStorageHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailUserSpaceHelper RailUserSpaceHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailUserSpaceHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailUserSpaceHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailStatisticHelper RailStatisticHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailStatisticHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailStatisticHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailLeaderboardHelper RailLeaderboardHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailLeaderboardHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailLeaderboardHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailAchievementHelper RailAchievementHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailAchievementHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailAchievementHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailNetwork RailNetworkHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailNetworkHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailNetworkImpl(intPtr);
			}
			return null;
		}

		public virtual IRailApps RailApps()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailApps(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailAppsImpl(intPtr);
			}
			return null;
		}

		public virtual IRailGame RailGame()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailGame(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailGameImpl(intPtr);
			}
			return null;
		}

		public virtual IRailUtils RailUtils()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailUtils(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailUtilsImpl(intPtr);
			}
			return null;
		}

		public virtual IRailAssetsHelper RailAssetsHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailAssetsHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailAssetsHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailDlcHelper RailDlcHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailDlcHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailDlcHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailScreenshotHelper RailScreenshotHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailScreenshotHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailScreenshotHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailVoiceHelper RailVoiceHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailVoiceHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailVoiceHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailSystemHelper RailSystemHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailSystemHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailSystemHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailTextInputHelper RailTextInputHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailTextInputHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailTextInputHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailIMEHelper RailIMETextInputHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailIMETextInputHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailIMEHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailHttpSessionHelper RailHttpSessionHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailHttpSessionHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailHttpSessionHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailSmallObjectServiceHelper RailSmallObjectServiceHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailSmallObjectServiceHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailSmallObjectServiceHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailZoneServerHelper RailZoneServerHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailZoneServerHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailZoneServerHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailGroupChatHelper RailGroupChatHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailGroupChatHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailGroupChatHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailInGameStorePurchaseHelper RailInGameStorePurchaseHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailInGameStorePurchaseHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailInGameStorePurchaseHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailInGameActivityHelper RailInGameActivityHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailInGameActivityHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailInGameActivityHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailAntiAddictionHelper RailAntiAddictionHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailAntiAddictionHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailAntiAddictionHelperImpl(intPtr);
			}
			return null;
		}

		public virtual IRailThirdPartyAccountLoginHelper RailThirdPartyAccountLoginHelper()
		{
			IntPtr intPtr = RAIL_API_PINVOKE.IRailFactory_RailThirdPartyAccountLoginHelper(this.swigCPtr_);
			if (!(intPtr == IntPtr.Zero))
			{
				return new IRailThirdPartyAccountLoginHelperImpl(intPtr);
			}
			return null;
		}
	}
}
