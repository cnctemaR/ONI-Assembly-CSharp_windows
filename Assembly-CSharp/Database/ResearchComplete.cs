using System;
using System.IO;

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
	}
}
