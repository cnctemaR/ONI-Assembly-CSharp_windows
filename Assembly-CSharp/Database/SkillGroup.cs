using System;
using System.Collections.Generic;
using Klei.AI;

namespace Database
{
	public class SkillGroup : Resource, IListableOption
	{
		string IListableOption.GetProperName()
		{
			return Strings.Get("STRINGS.DUPLICANTS.SKILLGROUPS." + this.Id.ToUpper() + ".NAME");
		}

		public SkillGroup(string id, string choreGroupID, string name, string icon, string archetype_icon)
			: base(id, name)
		{
			this.choreGroupID = choreGroupID;
			this.choreGroupIcon = icon;
			this.archetypeIcon = archetype_icon;
		}

		public string choreGroupID;

		public List<Klei.AI.Attribute> relevantAttributes;

		public List<string> requiredChoreGroups;

		public string choreGroupIcon;

		public string archetypeIcon;

		public bool allowAsAptitude = true;
	}
}
