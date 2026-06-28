using System;
using System.Collections.Generic;
using Klei.AI;
using TUNING;
using UnityEngine;

public class MinionStartingStats
{
	public MinionStartingStats(bool is_starter_minion)
	{
		int num = global::UnityEngine.Random.Range(0, Db.Get().Personalities.Count);
		this.personality = Db.Get().Personalities[num];
		this.voiceIdx = global::UnityEngine.Random.Range(0, 4);
		this.Name = this.personality.Name;
		this.OriginalName = this.personality.Name;
		List<Race> list = new List<Race>();
		foreach (Race race in MinionResources.Get().races)
		{
			if (!race.Disabled)
			{
				list.Add(race);
			}
		}
		int num2 = global::UnityEngine.Random.Range(0, list.Count);
		this.Race = list[num2];
		Trait trait = Db.Get().traits.TryGet(this.Race.Id + "BaseTrait");
		if (trait != null)
		{
			this.Traits.Add(trait);
		}
		List<ChoreGroup> list2 = new List<ChoreGroup>();
		int num3 = this.GenerateTraits(is_starter_minion, list2);
		this.GenerateAttributes(num3, list2);
		foreach (AccessorySlot accessorySlot in Db.Get().AccessorySlots)
		{
			if (accessorySlot.accessories.Count != 0)
			{
				int num4 = 0;
				if (accessorySlot == Db.Get().AccessorySlots.HeadShape)
				{
					num4 = this.personality.headShape;
					if (num4 > accessorySlot.accessories.Count - 1)
					{
						global::Debug.LogWarning(string.Format("CHECK DB AccessorySlots Invalid index {0} for accessory slot {1} setting to 0", num4, accessorySlot.Id), null);
						num4 = 0;
						this.personality.headShape = 0;
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Mouth)
				{
					num4 = this.personality.mouth;
					if (num4 > accessorySlot.accessories.Count - 1)
					{
						global::Debug.LogWarning(string.Format("CHECK DB AccessorySlots Invalid index {0} for accessory slot {1} setting to 0", num4, accessorySlot.Id), null);
						num4 = 0;
						this.personality.mouth = 0;
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Neck)
				{
					num4 = this.personality.neck;
					if (num4 > accessorySlot.accessories.Count - 1)
					{
						global::Debug.LogWarning(string.Format("CHECK DB AccessorySlots Invalid index {0} for accessory slot {1} setting to 0", num4, accessorySlot.Id), null);
						num4 = 0;
						this.personality.neck = 0;
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Eyes)
				{
					num4 = this.personality.eyes;
					if (num4 > accessorySlot.accessories.Count - 1)
					{
						global::Debug.LogWarning(string.Format("CHECK DB AccessorySlots Invalid index {0} for accessory slot {1} setting to 0", num4, accessorySlot.Id), null);
						num4 = 0;
						this.personality.eyes = 0;
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Hair)
				{
					num4 = this.personality.hair;
					if (num4 > accessorySlot.accessories.Count - 1)
					{
						global::Debug.LogWarning(string.Format("CHECK DB AccessorySlots Invalid index {0} for accessory slot {1} setting to 0", num4, accessorySlot.Id), null);
						num4 = 0;
						this.personality.hair = 0;
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Body || accessorySlot == Db.Get().AccessorySlots.Arm)
				{
					num4 = this.personality.body;
					if (num4 > accessorySlot.accessories.Count - 1)
					{
						global::Debug.LogWarning(string.Format("CHECK DB AccessorySlots Invalid index {0} for accessory slot {1} setting to 0", num4, accessorySlot.Id), null);
						num4 = 0;
						this.personality.body = 0;
					}
				}
				if (num4 == -1)
				{
					num4 = global::UnityEngine.Random.Range(0, accessorySlot.accessories.Count);
				}
				if (num4 > accessorySlot.accessories.Count - 1)
				{
					global::Debug.LogWarning(string.Format("Invalid index {0} for accessory slot {1}", num4, accessorySlot.Id), null);
					num4 = 0;
				}
				Accessory accessory = accessorySlot.accessories[num4];
				this.accessories.Add(accessory);
			}
		}
		int num5 = global::UnityEngine.Random.Range(0, this.Race.bodies.Count);
		this.BodyType = this.Race.bodies[num5].bodyType;
	}

	private int GenerateTraits(bool is_starter_minion, List<ChoreGroup> disabled_chore_groups)
	{
		int statDelta = 0;
		List<string> selectedTraits = new List<string>();
		global::System.Random randSeed = new global::System.Random();
		Trait trait = Db.Get().traits.Get(this.personality.stresstrait);
		this.stressTrait = trait;
		Trait trait2 = Db.Get().traits.Get(this.personality.congenitaltrait);
		if (trait2.Name == "None")
		{
			this.congenitaltrait = null;
		}
		else
		{
			this.congenitaltrait = trait2;
		}
		Func<List<DUPLICANTSTATS.TraitVal>, bool> func = delegate(List<DUPLICANTSTATS.TraitVal> traitPossibilities)
		{
			if (this.Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
			{
				return false;
			}
			float num2 = Util.GaussianRandom(0f, 1f);
			List<DUPLICANTSTATS.TraitVal> list = new List<DUPLICANTSTATS.TraitVal>(traitPossibilities);
			list.ShuffleSeeded<DUPLICANTSTATS.TraitVal>(randSeed);
			list.Sort((DUPLICANTSTATS.TraitVal t1, DUPLICANTSTATS.TraitVal t2) => -t1.probability.CompareTo(t2.probability));
			foreach (DUPLICANTSTATS.TraitVal traitVal in list)
			{
				if (!selectedTraits.Contains(traitVal.id))
				{
					if (traitVal.mutuallyExclusiveTraits != null)
					{
						bool flag2 = false;
						foreach (string text in selectedTraits)
						{
							flag2 = traitVal.mutuallyExclusiveTraits.Contains(text);
							if (flag2)
							{
								break;
							}
						}
						if (flag2)
						{
							continue;
						}
					}
					if (num2 > traitVal.probability)
					{
						Trait trait3 = Db.Get().traits.TryGet(traitVal.id);
						if (trait3 == null)
						{
							global::Debug.LogWarning("Trying to add nonexistent trait: " + traitVal.id, null);
						}
						else if (!is_starter_minion || trait3.ValidStarterTrait)
						{
							selectedTraits.Add(traitVal.id);
							statDelta += traitVal.statBonus;
							this.Traits.Add(trait3);
							if (trait3.disabledChoreGroups != null)
							{
								for (int k = 0; k < trait3.disabledChoreGroups.Length; k++)
								{
									disabled_chore_groups.Add(trait3.disabledChoreGroups[k]);
								}
							}
							return true;
						}
					}
				}
			}
			return false;
		};
		int num = ((!is_starter_minion) ? 3 : 1);
		bool flag = false;
		while (!flag)
		{
			for (int i = 0; i < num; i++)
			{
				flag = func(DUPLICANTSTATS.BADTRAITS) || flag;
			}
		}
		flag = false;
		while (!flag)
		{
			for (int j = 0; j < num; j++)
			{
				flag = func(DUPLICANTSTATS.GOODTRAITS) || flag;
			}
		}
		return statDelta;
	}

	private void GenerateAttributes(int pointsDelta, List<ChoreGroup> disabled_chore_groups)
	{
		float num = Util.GaussianRandom(0f, 1f) * ((float)DUPLICANTSTATS.MAX_STAT_POINTS - (float)DUPLICANTSTATS.MIN_STAT_POINTS) / 2f + (float)DUPLICANTSTATS.MIN_STAT_POINTS;
		int i = pointsDelta + Mathf.RoundToInt(num);
		List<string> list = new List<string>(DUPLICANTSTATS.DISTRIBUTED_ATTRIBUTES);
		int[] randomDistribution = DUPLICANTSTATS.DISTRIBUTIONS.GetRandomDistribution();
		while (i > 0)
		{
			list.Shuffle<string>();
			for (int j = 0; j < list.Count; j++)
			{
				if (i <= 0)
				{
					break;
				}
				string text = list[j];
				int num2 = randomDistribution[Mathf.Min(j, randomDistribution.Length - 1)];
				int num3 = Mathf.Min(i, num2);
				if (!this.StartingLevels.ContainsKey(text))
				{
					this.StartingLevels[text] = 0;
				}
				Dictionary<string, int> startingLevels;
				Dictionary<string, int> dictionary = (startingLevels = this.StartingLevels);
				string text3;
				string text2 = (text3 = text);
				int num4 = startingLevels[text3];
				dictionary[text2] = num4 + num3;
				i -= num3;
			}
		}
		if (disabled_chore_groups.Count > 0)
		{
			int num5 = 0;
			int num6 = 0;
			foreach (KeyValuePair<string, int> keyValuePair in this.StartingLevels)
			{
				if (keyValuePair.Value > num5)
				{
					num5 = keyValuePair.Value;
				}
				if (keyValuePair.Key == disabled_chore_groups[0].attribute.Id)
				{
					num6 = keyValuePair.Value;
				}
			}
			if (num5 == num6)
			{
				foreach (string text4 in list)
				{
					if (text4 != disabled_chore_groups[0].attribute.Id)
					{
						int num7 = 0;
						this.StartingLevels.TryGetValue(text4, out num7);
						int num8 = 0;
						if (num7 > 0)
						{
							num8 = 1;
						}
						this.StartingLevels[disabled_chore_groups[0].attribute.Id] = num7 - num8;
						this.StartingLevels[text4] = num5 + num8;
						break;
					}
				}
			}
		}
		foreach (string text5 in DUPLICANTSTATS.ROLLED_ATTRIBUTES)
		{
			this.StartingLevels[text5] = Mathf.RoundToInt(Mathf.Pow(global::UnityEngine.Random.value, 4f) * 10f);
		}
	}

	public void Apply(GameObject go)
	{
		MinionIdentity component = go.GetComponent<MinionIdentity>();
		component.SetName(this.Name);
		component.originalName = this.OriginalName;
		this.ApplyTraits(go);
		this.ApplyRace(go);
		this.ApplyAccessories(go);
		this.ApplyExperience(go);
	}

	public void ApplyExperience(GameObject go)
	{
		foreach (KeyValuePair<string, int> keyValuePair in this.StartingLevels)
		{
			go.GetComponent<AttributeLevels>().SetLevel(keyValuePair.Key, keyValuePair.Value);
		}
	}

	public void ApplyAccessories(GameObject go)
	{
		Accessorizer component = go.GetComponent<Accessorizer>();
		foreach (Accessory accessory in this.accessories)
		{
			component.AddAccessory(accessory);
		}
	}

	public void ApplyRace(GameObject go)
	{
		MinionIdentity component = go.GetComponent<MinionIdentity>();
		component.voiceIdx = this.voiceIdx;
		KCompBuilder.BodyData bodyData = default(KCompBuilder.BodyData);
		bodyData.eyes = this.personality.eyes;
		bodyData.hair = this.personality.hair;
		bodyData.headShape = this.personality.headShape;
		bodyData.mouth = this.personality.mouth;
		bodyData.neck = this.personality.neck;
		bodyData.arms = (bodyData.body = this.personality.body);
		MinionStartingStats.ApplyRace(go, this.Race, this.BodyType, bodyData);
	}

	public void ApplyTraits(GameObject go)
	{
		Traits component = go.GetComponent<Traits>();
		component.Clear();
		foreach (Trait trait in this.Traits)
		{
			component.Add(trait);
		}
		component.Add(this.stressTrait);
		if (this.congenitaltrait != null)
		{
			component.Add(this.congenitaltrait);
		}
		go.GetComponent<MinionIdentity>().SetName(this.Name);
	}

	public static KCompBuildInstance ApplyRace(GameObject go, Race race, BodyType body_type, KCompBuilder.BodyData personality)
	{
		KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
		component.ClearAnims();
		Body body = race.GetBody(body_type);
		component.AddAnims(body.buildFiles);
		BodyAnim body2 = MinionResources.Get().GetBody(body_type);
		component.AddAnims(body2.animFiles);
		KCompBuildInstance kcompBuildInstance = new KCompBuildInstance(personality, component);
		component.UpdateSymbolLookups();
		return kcompBuildInstance;
	}

	public string Name;

	public string OriginalName;

	public List<Trait> Traits = new List<Trait>();

	public Trait stressTrait;

	public Trait congenitaltrait;

	public Race Race;

	public BodyType BodyType;

	public int voiceIdx;

	public Dictionary<string, int> StartingLevels = new Dictionary<string, int>();

	public Personality personality;

	public List<Accessory> accessories = new List<Accessory>();
}
