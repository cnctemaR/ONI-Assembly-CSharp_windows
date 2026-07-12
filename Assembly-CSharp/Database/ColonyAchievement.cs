using System;
using System.Collections.Generic;

namespace Database
{
	public class ColonyAchievement : Resource
	{
		public string victoryNISSnapshot { get; private set; }

		public ColonyAchievement(string Id, string platformAchievementId, string Name, string description, bool isVictoryCondition, List<ColonyAchievementRequirement> requirementChecklist, string messageTitle = "", string messageBody = "", string videoDataName = "", string victoryLoopVideo = "", Action<KMonoBehaviour> VictorySequence = null, string victorySnapshot = "", string icon = "")
			: base(Id, Name)
		{
			this.Id = Id;
			this.platformAchievementId = platformAchievementId;
			this.Name = Name;
			this.description = description;
			this.isVictoryCondition = isVictoryCondition;
			this.requirementChecklist = requirementChecklist;
			this.messageTitle = messageTitle;
			this.messageBody = messageBody;
			this.shortVideoName = videoDataName;
			this.loopVideoName = victoryLoopVideo;
			this.victorySequence = VictorySequence;
			this.victoryNISSnapshot = (string.IsNullOrEmpty(victorySnapshot) ? AudioMixerSnapshots.Get().VictoryNISGenericSnapshot : victorySnapshot);
			this.icon = icon;
		}

		public string description;

		public bool isVictoryCondition;

		public string messageTitle;

		public string messageBody;

		public string shortVideoName;

		public string loopVideoName;

		public string platformAchievementId;

		public string icon;

		public List<ColonyAchievementRequirement> requirementChecklist = new List<ColonyAchievementRequirement>();

		public Action<KMonoBehaviour> victorySequence;
	}
}
