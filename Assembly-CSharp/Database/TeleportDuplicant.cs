using System;
using System.Collections.Generic;
using STRINGS;

namespace Database
{
	public class TeleportDuplicant : ColonyAchievementRequirement
	{
		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TELEPORT_DUPLICANT;
		}

		public override bool Success()
		{
			using (List<WarpReceiver>.Enumerator enumerator = Components.WarpReceivers.Items.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Used)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
