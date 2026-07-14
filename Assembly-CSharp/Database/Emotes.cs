using System;
using Klei.AI;

namespace Database
{
	public class Emotes : ResourceSet<Resource>
	{
		public Emotes(ResourceSet parent)
			: base("Emotes", parent)
		{
			this.Minion = new Emotes.MinionEmotes(this);
			this.Critter = new Emotes.CritterEmotes(this);
		}

		public void ResetProblematicReferences()
		{
			for (int i = 0; i < this.Minion.resources.Count; i++)
			{
				Emote emote = this.Minion.resources[i];
				for (int j = 0; j < emote.StepCount; j++)
				{
					emote[j].UnregisterAllCallbacks();
				}
			}
			for (int k = 0; k < this.Critter.resources.Count; k++)
			{
				Emote emote2 = this.Critter.resources[k];
				for (int l = 0; l < emote2.StepCount; l++)
				{
					emote2[l].UnregisterAllCallbacks();
				}
			}
		}

		public Emotes.MinionEmotes Minion;

		public Emotes.CritterEmotes Critter;

		public class MinionEmotes : ResourceSet<Emote>
		{
			public MinionEmotes(ResourceSet parent)
				: base("Minion", parent)
			{
				this.InitializeCelebrations();
				this.InitializePhysicalStatus();
				this.InitializeEmotionalStatus();
				this.InitializeGreetings();
			}

			public void InitializeCelebrations()
			{
				this.ClapCheer = new Emote(this, "ClapCheer", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "clapcheer_pre"
					},
					new EmoteStep
					{
						anim = "clapcheer_loop"
					},
					new EmoteStep
					{
						anim = "clapcheer_pst"
					}
				}, "anim_clapcheer_kanim", "anim_clapcheer_swim_kanim");
				this.Cheer = new Emote(this, "Cheer", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "cheer_pre"
					},
					new EmoteStep
					{
						anim = "cheer_loop"
					},
					new EmoteStep
					{
						anim = "cheer_pst"
					}
				}, "anim_cheer_kanim", null);
				this.ProductiveCheer = new Emote(this, "Productive Cheer", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "productive"
					}
				}, "anim_productive_kanim", "anim_productive_swim_kanim");
				this.ResearchComplete = new Emote(this, "ResearchComplete", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_research_complete_kanim", null);
				this.ThumbsUp = new Emote(this, "ThumbsUp", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_thumbsup_kanim", "anim_react_thumbsup_swim_kanim");
			}

			private void InitializePhysicalStatus()
			{
				this.CloseCall_Fall = new Emote(this, "Near Fall", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_floor_missing_kanim", null);
				this.Cold = new Emote(this, "Cold", Emotes.MinionEmotes.DEFAULT_IDLE_STEPS, "anim_idle_cold_kanim", "anim_idle_cold_swim_kanim");
				this.Cough = new Emote(this, "Cough", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_slimelungcough_kanim", "anim_slimelungcough_swim_kanim");
				this.Cough_Small = new Emote(this, "Small Cough", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "react_small"
					}
				}, "anim_slimelungcough_kanim", null);
				this.FoodPoisoning = new Emote(this, "Food Poisoning", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_contaminated_food_kanim", "anim_react_contaminated_food_swim_kanim");
				this.Hot = new Emote(this, "Hot", Emotes.MinionEmotes.DEFAULT_IDLE_STEPS, "anim_idle_hot_kanim", "anim_idle_hot_swim_kanim");
				this.IritatedEyes = new Emote(this, "Irritated Eyes", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "irritated_eyes"
					}
				}, "anim_irritated_eyes_kanim", "anim_irritated_eyes_swim_kanim");
				this.MorningStretch = new Emote(this, "Morning Stretch", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_morning_stretch_kanim", "anim_react_morning_stretch_swim_kanim");
				this.Radiation_Glare = new Emote(this, "Radiation Glare", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "react_radiation_glare"
					}
				}, "anim_react_radiation_kanim", null);
				this.Radiation_Itch = new Emote(this, "Radiation Itch", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "react_radiation_itch"
					}
				}, "anim_react_radiation_kanim", null);
				this.Sick = new Emote(this, "Sick", Emotes.MinionEmotes.DEFAULT_IDLE_STEPS, "anim_idle_sick_kanim", "anim_idle_sick_swim_kanim");
				this.SoreBack = new Emote(this, "SoreBack", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_sore_back_kanim", "anim_react_sore_back_swim_kanim");
				this.Sneeze = new Emote(this, "Sneeze", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "sneeze"
					},
					new EmoteStep
					{
						anim = "sneeze_pst"
					}
				}, "anim_sneeze_kanim", "anim_sneeze_swim_kanim");
				this.WaterDamage = new Emote(this, "WaterDamage", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "zapped"
					}
				}, "anim_bionic_kanim", null);
				this.GrindingGears = new Emote(this, "GrindingGears", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "react"
					}
				}, "anim_bionic_react_grinding_gears_kanim", null);
				this.Sneeze_Short = new Emote(this, "Short Sneeze", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "sneeze_short"
					},
					new EmoteStep
					{
						anim = "sneeze_short_pst"
					}
				}, "anim_sneeze_kanim", null);
			}

			private void InitializeEmotionalStatus()
			{
				this.Concern = new Emote(this, "Concern", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_concern_kanim", null);
				this.Cringe = new Emote(this, "Cringe", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "cringe_pre"
					},
					new EmoteStep
					{
						anim = "cringe_loop"
					},
					new EmoteStep
					{
						anim = "cringe_pst"
					}
				}, "anim_cringe_kanim", null);
				this.Disappointed = new Emote(this, "Disappointed", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "disappointed_pre"
					},
					new EmoteStep
					{
						anim = "disappointed_loop"
					},
					new EmoteStep
					{
						anim = "disappointed_pst"
					}
				}, "anim_disappointed_kanim", null);
				this.Shock = new Emote(this, "Shock", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_shock_kanim", null);
				this.Sing = new Emote(this, "Sing", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_singer_kanim", null);
			}

			private void InitializeGreetings()
			{
				this.FingerGuns = new Emote(this, "Finger Guns", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_fingerguns_kanim", null);
				this.Wave = new Emote(this, "Wave", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_wave_kanim", null);
				this.Wave_Shy = new Emote(this, "Shy Wave", Emotes.MinionEmotes.DEFAULT_STEPS, "anim_react_wave_shy_kanim", null);
			}

			private static EmoteStep[] DEFAULT_STEPS = new EmoteStep[]
			{
				new EmoteStep
				{
					anim = "react"
				}
			};

			private static EmoteStep[] DEFAULT_IDLE_STEPS = new EmoteStep[]
			{
				new EmoteStep
				{
					anim = "idle_pre"
				},
				new EmoteStep
				{
					anim = "idle_default"
				},
				new EmoteStep
				{
					anim = "idle_pst"
				}
			};

			public Emote ClapCheer;

			public Emote Cheer;

			public Emote ProductiveCheer;

			public Emote ResearchComplete;

			public Emote ThumbsUp;

			public Emote CloseCall_Fall;

			public Emote Cold;

			public Emote Cough;

			public Emote Cough_Small;

			public Emote FoodPoisoning;

			public Emote Hot;

			public Emote IritatedEyes;

			public Emote MorningStretch;

			public Emote Radiation_Glare;

			public Emote Radiation_Itch;

			public Emote Sick;

			public Emote Sneeze;

			public Emote SoreBack;

			public Emote WaterDamage;

			public Emote Sneeze_Short;

			public Emote GrindingGears;

			public Emote Concern;

			public Emote Cringe;

			public Emote Disappointed;

			public Emote Shock;

			public Emote Sing;

			public Emote FingerGuns;

			public Emote Wave;

			public Emote Wave_Shy;
		}

		public class CritterEmotes : ResourceSet<Emote>
		{
			public CritterEmotes(ResourceSet parent)
				: base("Critter", parent)
			{
				this.InitializeEmotes();
			}

			private void InitializeEmotes()
			{
				this.Positive = new Emote(this, "Positive", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "react_pos"
					}
				}, null, null);
				this.Negative = new Emote(this, "Negative", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "react_neg"
					}
				}, null, null);
				this.Roar = new Emote(this, "Roar", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "roar"
					}
				}, null, null);
				this.RaptorSignal = new Emote(this, "Signal", new EmoteStep[]
				{
					new EmoteStep
					{
						anim = "signal"
					}
				}, null, null);
			}

			public Emote Positive;

			public Emote Negative;

			public Emote Roar;

			public Emote RaptorSignal;
		}
	}
}
