using System;
using Database;

public class SkillListable : IListableOption
{
	public SkillListable(string name)
	{
		this.skillName = name;
		Skill skill = Db.Get().Skills.TryGet(this.skillName);
		if (skill != null)
		{
			this.name = skill.Name;
			this.skillHat = skill.hat;
		}
	}

	public string skillName { get; private set; }

	public string skillHat { get; private set; }

	public string GetProperName()
	{
		return this.name;
	}

	public LocString name;
}
