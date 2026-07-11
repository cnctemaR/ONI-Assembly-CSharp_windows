using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Database;
using KSerialization;

public class ColonyAchievementStatus
{
	public List<ColonyAchievementRequirement> Requirements
	{
		get
		{
			return this.requirements;
		}
	}

	public void UpdateAchievement()
	{
		if (this.requirements == null || this.requirements.Count <= 0)
		{
			return;
		}
		this.success = true;
		foreach (ColonyAchievementRequirement colonyAchievementRequirement in this.requirements)
		{
			colonyAchievementRequirement.Update();
			this.success &= colonyAchievementRequirement.Success();
			this.failed |= colonyAchievementRequirement.Fail();
		}
	}

	public void Deserialize(IReader reader)
	{
		this.success = reader.ReadByte() > 0;
		this.failed = reader.ReadByte() > 0;
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			Type type = Type.GetType(reader.ReadKleiString());
			if (type != null)
			{
				ColonyAchievementRequirement colonyAchievementRequirement = (ColonyAchievementRequirement)FormatterServices.GetUninitializedObject(type);
				colonyAchievementRequirement.Deserialize(reader);
				this.requirements.Add(colonyAchievementRequirement);
			}
		}
	}

	public void SetRequirements(List<ColonyAchievementRequirement> requirementChecklist)
	{
		this.requirements = requirementChecklist;
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write(this.success ? 1 : 0);
		writer.Write(this.failed ? 1 : 0);
		writer.Write((this.requirements != null) ? this.requirements.Count : 0);
		if (this.requirements != null)
		{
			foreach (ColonyAchievementRequirement colonyAchievementRequirement in this.requirements)
			{
				writer.WriteKleiString(colonyAchievementRequirement.GetType().ToString());
				colonyAchievementRequirement.Serialize(writer);
			}
		}
	}

	public bool success;

	public bool failed;

	private List<ColonyAchievementRequirement> requirements = new List<ColonyAchievementRequirement>();
}
