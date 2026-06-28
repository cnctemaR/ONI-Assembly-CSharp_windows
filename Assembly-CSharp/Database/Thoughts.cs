using System;
using STRINGS;

namespace Database
{
	public class Thoughts : ResourceSet<Thought>
	{
		public Thoughts(ResourceSet parent)
			: base("Thoughts", parent)
		{
			this.GotInfected = new Thought("GotInfected", this, "crew_state_sick", DUPLICANTS.THOUGHTS.GOTINFECTED.TOOLTIP, false);
			this.Starving = new Thought("Starving", this, "crew_state_hungry", DUPLICANTS.THOUGHTS.STARVING.TOOLTIP, false);
			this.Hot = new Thought("Hot", this, "crew_state_temp_up", DUPLICANTS.THOUGHTS.HOT.TOOLTIP, false);
			this.Cold = new Thought("Cold", this, "crew_state_temp_down", DUPLICANTS.THOUGHTS.COLD.TOOLTIP, false);
			this.FullBladder = new Thought("FullBladder", this, "crew_state_full_bladder", DUPLICANTS.THOUGHTS.FULLBLADDER.TOOLTIP, false);
			this.PoorDecor = new Thought("PoorDecor", this, "crew_state_decor", DUPLICANTS.THOUGHTS.POORDECOR.TOOLTIP, false);
			this.Happy = new Thought("Happy", this, "crew_state_happy", DUPLICANTS.THOUGHTS.HAPPY.TOOLTIP, false);
			this.Unhappy = new Thought("Unhappy", this, "crew_state_unhappy", DUPLICANTS.THOUGHTS.UNHAPPY.TOOLTIP, false);
			this.Sleepy = new Thought("Sleepy", this, "crew_state_sleepy", DUPLICANTS.THOUGHTS.SLEEPY.TOOLTIP, false);
			this.Suffocating = new Thought("Suffocating", this, "crew_state_cantbreathe", DUPLICANTS.THOUGHTS.SUFFOCATING.TOOLTIP, false);
			this.Angry = new Thought("Angry", this, "crew_state_angry", DUPLICANTS.THOUGHTS.ANGRY.TOOLTIP, false);
			this.Raging = new Thought("Enraged", this, "crew_state_enraged", DUPLICANTS.THOUGHTS.RAGING.TOOLTIP, false);
			this.PutridOdour = new Thought("PutridOdour", this, "crew_state_smelled_putrid_odour", DUPLICANTS.THOUGHTS.PUTRIDODOUR.TOOLTIP, true);
			for (int i = this.Count - 1; i >= 0; i--)
			{
				this.resources[i].priority = 100 * (this.Count - i);
			}
		}

		public Thought Starving;

		public Thought Hot;

		public Thought Cold;

		public Thought FullBladder;

		public Thought Happy;

		public Thought Unhappy;

		public Thought PoorDecor;

		public Thought Sleepy;

		public Thought Suffocating;

		public Thought Angry;

		public Thought Raging;

		public Thought GotInfected;

		public Thought PutridOdour;
	}
}
