using System;
using System.Collections.Generic;

namespace Database
{
	public class ColonyAchievement : Resource
	{
		public ColonyAchievement(string Id, string steamAchievementId, string Name, string description, bool isVictoryCondition, List<ColonyAchievementRequirement> requirementChecklist, string messageTitle = "", string messageBody = "", string videoDataName = "", string victoryLoopVideo = "", Action<KMonoBehaviour> VictorySequence = null, string victorySnapshot = "", string icon = "")
			: base(Id, Name)
		{
			this.Id = Id;
			this.steamAchievementId = steamAchievementId;
			this.Name = Name;
			this.description = description;
			this.isVictoryCondition = isVictoryCondition;
			this.requirementChecklist = requirementChecklist;
			this.messageTitle = messageTitle;
			this.messageBody = messageBody;
			this.shortVideoName = videoDataName;
			this.loopVideoName = victoryLoopVideo;
			this.victorySequence = VictorySequence;
			this.victoryNISSnapshot = ((!string.IsNullOrEmpty(victorySnapshot)) ? victorySnapshot : AudioMixerSnapshots.Get().VictoryNISGenericSnapshot);
			this.icon = icon;
		}

		public string victoryNISSnapshot { get; private set; }

		public string description;

		public bool isVictoryCondition;

		public string messageTitle;

		public string messageBody;

		public string shortVideoName;

		public string loopVideoName;

		public string steamAchievementId;

		public string icon;

		public List<ColonyAchievementRequirement> requirementChecklist = new List<ColonyAchievementRequirement>();

		public Action<KMonoBehaviour> victorySequence;
	}
}
