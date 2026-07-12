using System;
using STRINGS;

namespace Database
{
	public static class PermitRarityExtensions
	{
		public static string GetLocStringName(this PermitRarity rarity)
		{
			switch (rarity)
			{
			case PermitRarity.Unknown:
				return UI.PERMIT_RARITY.UNKNOWN;
			case PermitRarity.Universal:
				return UI.PERMIT_RARITY.UNIVERSAL;
			case PermitRarity.Loyalty:
				return UI.PERMIT_RARITY.LOYALTY;
			case PermitRarity.Common:
				return UI.PERMIT_RARITY.COMMON;
			case PermitRarity.Decent:
				return UI.PERMIT_RARITY.DECENT;
			case PermitRarity.Nifty:
				return UI.PERMIT_RARITY.NIFTY;
			case PermitRarity.Splendid:
				return UI.PERMIT_RARITY.SPLENDID;
			}
			DebugUtil.DevAssert(false, string.Format("Couldn't get name for rarity {0}", rarity), null);
			return "-";
		}
	}
}
