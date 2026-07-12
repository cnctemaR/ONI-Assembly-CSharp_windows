using System;
using ProcGen;
using UnityEngine;

namespace Database
{
	public class Stories : ResourceSet<Story>
	{
		public Stories(ResourceSet parent)
			: base("Stories", parent)
		{
			this.MegaBrainTank = base.Add(new Story("MegaBrainTank", "storytraits/MegaBrainTank", 0, 1, 43).SetKeepsake("keepsake_megabrain"));
			this.CreatureManipulator = base.Add(new Story("CreatureManipulator", "storytraits/CritterManipulator", 1, 2, 43).SetKeepsake("keepsake_crittermanipulator"));
			this.LonelyMinion = base.Add(new Story("LonelyMinion", "storytraits/LonelyMinion", 2, 3, 44).SetKeepsake("keepsake_lonelyminion"));
			this.FossilHunt = base.Add(new Story("FossilHunt", "storytraits/FossilHunt", 3, 4, 44).SetKeepsake("keepsake_fossilhunt"));
			this.resources.Sort();
		}

		public void AddStoryMod(Story mod)
		{
			mod.kleiUseOnlyCoordinateOffset = -1;
			base.Add(mod);
			this.resources.Sort();
		}

		public int GetHighestCoordinateOffset()
		{
			int num = 0;
			foreach (Story story in this.resources)
			{
				num = Mathf.Max(num, story.kleiUseOnlyCoordinateOffset);
			}
			return num;
		}

		public WorldTrait GetStoryTrait(string id, bool assertMissingTrait = false)
		{
			Story story = this.resources.Find((Story x) => x.Id == id);
			if (story != null)
			{
				return SettingsCache.GetCachedStoryTrait(story.worldgenStoryTraitKey, assertMissingTrait);
			}
			return null;
		}

		public Story GetStoryFromStoryTrait(string storyTraitTemplate)
		{
			return this.resources.Find((Story x) => x.worldgenStoryTraitKey == storyTraitTemplate);
		}

		public Story MegaBrainTank;

		public Story CreatureManipulator;

		public Story LonelyMinion;

		public Story FossilHunt;
	}
}
