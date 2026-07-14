using System;
using STRINGS;

namespace Database
{
	public class MinnowRecruited : VictoryColonyAchievementRequirement
	{
		public MinnowRecruited(MinnowImperativePOIStates.MinnowPOIIdentity minnowIdentity)
		{
			this.shouldUpdateNameAndDescription = true;
			this.minnowIdentity = minnowIdentity;
		}

		public override string GetProgress(bool complete)
		{
			int num = (int)this.minnowIdentity;
			MinnowImperativePOIStates.Instance minnowInstance = this.GetMinnowInstance();
			return Strings.Get((!complete && (minnowInstance == null || !minnowInstance.HasUserEverClicked)) ? MinnowRecruited.requirementDescriptionPATH_HIDDEN[num] : MinnowRecruited.requirementDescriptionPATH[num]);
		}

		public override string Description()
		{
			return COLONY_ACHIEVEMENTS.FINDING_MINNOW.DESCRIPTION;
		}

		private MinnowImperativePOIStates.Instance GetMinnowInstance()
		{
			foreach (MinnowImperativePOIStates.Instance instance in Components.MinnowImperativePOIs.Items)
			{
				if (instance.def.minnowPOIIdentity == this.minnowIdentity)
				{
					return instance;
				}
			}
			return null;
		}

		public override bool Success()
		{
			if (SaveGame.Instance.ColonyAchievementTracker.allMinnowQuestsCompleted)
			{
				return true;
			}
			MinnowImperativePOIStates.Instance minnowInstance = this.GetMinnowInstance();
			return minnowInstance != null && minnowInstance.WasCompletedAndAcknowledged && !MinnowImperativePOIStates.Instance.AllPOIsCompleted();
		}

		public override string Name()
		{
			int num = (int)this.minnowIdentity;
			MinnowImperativePOIStates.Instance minnowInstance = this.GetMinnowInstance();
			bool flag = (minnowInstance == null || !minnowInstance.WasCompletedAndAcknowledged) && (minnowInstance == null || !minnowInstance.HasUserEverClicked);
			float num2 = ((minnowInstance != null) ? minnowInstance.def.requiredMass : 0f);
			Tag tag = ((minnowInstance != null) ? minnowInstance.def.requestedTag : null);
			EdiblesManager.FoodInfo foodInfo = ((tag != null) ? EdiblesManager.GetFoodInfo(tag.Name) : null);
			string text = Strings.Get(flag ? MinnowRecruited.requirementNamePATH_HIDDEN[num] : MinnowRecruited.requirementNamePATH[num]);
			float num3 = ((foodInfo != null) ? (foodInfo.CaloriesPerUnit * num2) : 0f);
			return text.Replace("{AMOUNT}", (num3 != 0f) ? GameUtil.GetFormattedCalories(num3, GameUtil.TimeSlice.None, true) : GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		}

		private static string[] requirementNamePATH = new string[] { "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_NAME_A", "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_NAME_B", "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_NAME_C" };

		private static string[] requirementDescriptionPATH = new string[] { "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_DESCRIPTION_A", "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_DESCRIPTION_B", "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_DESCRIPTION_C" };

		private static string[] requirementNamePATH_HIDDEN = new string[] { "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_NAME_A_HIDDEN", "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_NAME_B_HIDDEN", "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_NAME_C_HIDDEN" };

		private static string[] requirementDescriptionPATH_HIDDEN = new string[] { "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_DESCRIPTION_A_HIDDEN", "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_DESCRIPTION_B_HIDDEN", "STRINGS.COLONY_ACHIEVEMENTS.FINDING_MINNOW.ACHIEVEMENTS.REQUIREMENT_DESCRIPTION_C_HIDDEN" };

		private MinnowImperativePOIStates.MinnowPOIIdentity minnowIdentity;
	}
}
