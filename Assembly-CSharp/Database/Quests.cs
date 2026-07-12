using System;
using System.Collections.Generic;

namespace Database
{
	public class Quests : ResourceSet<Quest>
	{
		public Quests(ResourceSet parent)
			: base("Quests", parent)
		{
			this.LonelyMinionGreetingQuest = base.Add(new Quest("KnockQuest", new QuestCriteria[]
			{
				new QuestCriteria("Neighbor", null, 1, null, QuestCriteria.BehaviorFlags.None)
			}));
			this.LonelyMinionFoodQuest = base.Add(new Quest("FoodQuest", new QuestCriteria[]
			{
				new QuestCriteria_GreaterOrEqual("FoodQuality", new float[] { 4f }, 3, new HashSet<Tag> { GameTags.Edible }, QuestCriteria.BehaviorFlags.UniqueItems)
			}));
			this.LonelyMinionPowerQuest = base.Add(new Quest("PluggedIn", new QuestCriteria[]
			{
				new QuestCriteria_GreaterOrEqual("SuppliedPower", new float[] { 3000f }, 1, null, QuestCriteria.BehaviorFlags.TrackValues)
			}));
			this.LonelyMinionDecorQuest = base.Add(new Quest("HighDecor", new QuestCriteria[]
			{
				new QuestCriteria_GreaterOrEqual("Decor", new float[] { 120f }, 1, null, (QuestCriteria.BehaviorFlags)6)
			}));
			this.FossilHuntQuest = base.Add(new Quest("FossilHuntQuest", new QuestCriteria[]
			{
				new QuestCriteria_Equals("LostSpecimen", new float[] { 1f }, 1, null, QuestCriteria.BehaviorFlags.TrackValues),
				new QuestCriteria_Equals("LostIceFossil", new float[] { 1f }, 1, null, QuestCriteria.BehaviorFlags.TrackValues),
				new QuestCriteria_Equals("LostResinFossil", new float[] { 1f }, 1, null, QuestCriteria.BehaviorFlags.TrackValues),
				new QuestCriteria_Equals("LostRockFossil", new float[] { 1f }, 1, null, QuestCriteria.BehaviorFlags.TrackValues)
			}));
		}

		public Quest LonelyMinionGreetingQuest;

		public Quest LonelyMinionFoodQuest;

		public Quest LonelyMinionPowerQuest;

		public Quest LonelyMinionDecorQuest;

		public Quest FossilHuntQuest;
	}
}
