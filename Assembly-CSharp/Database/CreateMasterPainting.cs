using System;
using STRINGS;

namespace Database
{
	public class CreateMasterPainting : ColonyAchievementRequirement, AchievementRequirementSerialization_Deprecated
	{
		public override bool Success()
		{
			foreach (Painting painting in Components.Paintings.Items)
			{
				if (painting != null)
				{
					ArtableStage artableStage = Db.GetArtableStages().TryGet(painting.CurrentStage);
					if (artableStage != null && artableStage.statusItem == Db.Get().ArtableStatuses.LookingGreat)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void Deserialize(IReader reader)
		{
		}

		public override string GetProgress(bool complete)
		{
			return COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.CREATE_A_PAINTING;
		}
	}
}
