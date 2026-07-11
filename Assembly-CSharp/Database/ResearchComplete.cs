using System;
using System.IO;
using STRINGS;

namespace Database
{
	public class ResearchComplete : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			foreach (Tech tech in Db.Get().Techs.resources)
			{
				if (!tech.IsComplete())
				{
					return false;
				}
			}
			return true;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override string GetProgress(bool complete)
		{
			if (complete)
			{
				return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TECH_RESEARCHED, Db.Get().Techs.resources.Count, Db.Get().Techs.resources.Count);
			}
			int num = 0;
			foreach (Tech tech in Db.Get().Techs.resources)
			{
				if (tech.IsComplete())
				{
					num++;
				}
			}
			return string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TECH_RESEARCHED, num, Db.Get().Techs.resources.Count);
		}
	}
}
