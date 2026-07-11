using System;
using System.IO;
using STRINGS;

namespace Database
{
	public class CreateMasterPainting : ColonyAchievementRequirement
	{
		public override bool Success()
		{
			foreach (BuildingComplete buildingComplete in Components.BuildingCompletes.Items)
			{
				if (buildingComplete.isArtable)
				{
					Painting component = buildingComplete.GetComponent<Painting>();
					if (component != null && component.CurrentStatus == Artable.Status.Great)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void Deserialize(IReader reader)
		{
		}

		public override void Serialize(BinaryWriter writer)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CREATE_A_PAINTING;
		}
	}
}
