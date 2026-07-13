using System;

namespace Database
{
	public class CritterEmotions : ResourceSet<Thought>
	{
		public CritterEmotions(ResourceSet parent)
			: base("CritterEmotions", parent)
		{
			this.Hungry = new CritterEmotion("Hungry", false, Assets.GetSprite("crew_state_hungry"));
			this.Hot = new CritterEmotion("Hot", false, Assets.GetSprite("crew_state_temp_up"));
			this.Cold = new CritterEmotion("Cold", false, Assets.GetSprite("crew_state_temp_down"));
			this.Cramped = new CritterEmotion("Cramped", false, Assets.GetSprite("crew_state_stress"));
			this.Crowded = new CritterEmotion("Crowded", false, Assets.GetSprite("crew_state_stress"));
			this.Suffocating = new CritterEmotion("Suffocating", false, Assets.GetSprite("crew_state_cantbreathe"));
			this.WellFed = new CritterEmotion("WellFed", true, Assets.GetSprite("crew_state_binge_eat"));
			this.Happy = new CritterEmotion("Happy", true, Assets.GetSprite("crew_state_happy"));
		}

		public CritterEmotion Hungry;

		public CritterEmotion Hot;

		public CritterEmotion Cold;

		public CritterEmotion Cramped;

		public CritterEmotion Crowded;

		public CritterEmotion Suffocating;

		public CritterEmotion WellFed;

		public CritterEmotion Happy;
	}
}
