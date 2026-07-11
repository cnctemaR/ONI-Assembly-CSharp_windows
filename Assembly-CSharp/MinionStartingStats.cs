using System;
using System.Collections.Generic;
using Klei.AI;
using TUNING;
using UnityEngine;

public class MinionStartingStats : ITelepadDeliverable
{
	public MinionStartingStats(bool is_starter_minion)
	{
		if (is_starter_minion)
		{
			int num = global::UnityEngine.Random.Range(0, 31);
			this.personality = Db.Get().Personalities[num];
		}
		else
		{
			int num2 = global::UnityEngine.Random.Range(0, 35);
			this.personality = Db.Get().Personalities[num2];
		}
		this.voiceIdx = global::UnityEngine.Random.Range(0, 4);
		this.Name = this.personality.Name;
		this.NameStringKey = this.personality.nameStringKey;
		this.GenderStringKey = this.personality.genderStringKey;
		this.Traits.Add(Db.Get().traits.Get(MinionConfig.MINION_BASE_TRAIT_ID));
		List<ChoreGroup> list = new List<ChoreGroup>();
		this.GenerateAptitudes();
		int num3 = this.GenerateTraits(is_starter_minion, list);
		this.GenerateAttributes(num3, list);
		KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(this.personality);
		foreach (AccessorySlot accessorySlot in Db.Get().AccessorySlots.resources)
		{
			if (accessorySlot.accessories.Count != 0)
			{
				Accessory accessory = null;
				if (accessorySlot == Db.Get().AccessorySlots.HeadShape)
				{
					accessory = accessorySlot.Lookup(bodyData.headShape);
					if (accessory == null)
					{
						this.personality.headShape = 0;
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Mouth)
				{
					accessory = accessorySlot.Lookup(bodyData.mouth);
					if (accessory == null)
					{
						this.personality.mouth = 0;
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Eyes)
				{
					accessory = accessorySlot.Lookup(bodyData.eyes);
					if (accessory == null)
					{
						this.personality.eyes = 0;
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Hair)
				{
					accessory = accessorySlot.Lookup(bodyData.hair);
					if (accessory == null)
					{
						this.personality.hair = 0;
					}
				}
				else if (accessorySlot != Db.Get().AccessorySlots.HatHair)
				{
					if (accessorySlot == Db.Get().AccessorySlots.Body)
					{
						accessory = accessorySlot.Lookup(bodyData.body);
						if (accessory == null)
						{
							this.personality.body = 0;
						}
					}
					else if (accessorySlot == Db.Get().AccessorySlots.Arm)
					{
						accessory = accessorySlot.Lookup(bodyData.arms);
					}
				}
				if (accessory == null)
				{
					accessory = accessorySlot.accessories[0];
				}
				this.accessories.Add(accessory);
			}
		}
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
					if (traitVal.requiredNonPositiveAptitudes != null)
					{
						bool flag2 = false;
						foreach (KeyValuePair<HashedString, float> keyValuePair in this.roleAptitudes)
						{
							if (flag2)
							{
								break;
							}
							foreach (HashedString hashedString in traitVal.requiredNonPositiveAptitudes)
							{
								if (hashedString == keyValuePair.Key && keyValuePair.Value > 0f)
								{
									flag2 = true;
									break;
								}
							}
						}
						if (flag2)
						{
							continue;
						}
					}
					if (traitVal.mutuallyExclusiveTraits != null)
					{
						bool flag3 = false;
						foreach (string text in selectedTraits)
						{
							flag3 = traitVal.mutuallyExclusiveTraits.Contains(text);
							if (flag3)
							{
								break;
							}
						}
						if (flag3)
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

	private void GenerateAptitudes()
	{
		int num = global::UnityEngine.Random.Range(1, 4);
		for (int i = 0; i < num; i++)
		{
			RoleConfig random = Game.Instance.roleManager.RolesConfigs.GetRandom<RoleConfig>();
			if (random.id != "NoRole" && !this.roleAptitudes.ContainsKey(random.roleGroup))
			{
				this.roleAptitudes.Add(random.roleGroup, 1f);
			}
			else if (num < this.roleAptitudes.Count)
			{
				i--;
			}
		}
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
				string text2;
				(startingLevels = this.StartingLevels)[text2 = text] = startingLevels[text2] + num3;
				i -= num3;
			}
		}
		if (disabled_chore_groups.Count > 0)
		{
			int num4 = 0;
			int num5 = 0;
			foreach (KeyValuePair<string, int> keyValuePair in this.StartingLevels)
			{
				if (keyValuePair.Value > num4)
				{
					num4 = keyValuePair.Value;
				}
				if (keyValuePair.Key == disabled_chore_groups[0].attribute.Id)
				{
					num5 = keyValuePair.Value;
				}
			}
			if (num4 == num5)
			{
				foreach (string text3 in list)
				{
					if (text3 != disabled_chore_groups[0].attribute.Id)
					{
						int num6 = 0;
						this.StartingLevels.TryGetValue(text3, out num6);
						int num7 = 0;
						if (num6 > 0)
						{
							num7 = 1;
						}
						this.StartingLevels[disabled_chore_groups[0].attribute.Id] = num6 - num7;
						this.StartingLevels[text3] = num4 + num7;
						break;
					}
				}
			}
		}
		foreach (string text4 in DUPLICANTSTATS.ROLLED_ATTRIBUTES)
		{
			this.StartingLevels[text4] = Mathf.RoundToInt(Mathf.Pow(global::UnityEngine.Random.value, DUPLICANTSTATS.ROLLED_ATTRIBUTE_POWER) * (float)DUPLICANTSTATS.ROLLED_ATTRIBUTE_MAX);
		}
	}

	public void Apply(GameObject go)
	{
		MinionIdentity component = go.GetComponent<MinionIdentity>();
		component.SetName(this.Name);
		component.nameStringKey = this.NameStringKey;
		component.genderStringKey = this.GenderStringKey;
		this.ApplyTraits(go);
		this.ApplyRace(go);
		this.ApplyAptitudes(go);
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
	}

	public static KCompBuilder.BodyData CreateBodyData(Personality p)
	{
		return new KCompBuilder.BodyData
		{
			eyes = HashCache.Get().Add(string.Format("eyes_{0:000}", p.eyes)),
			hair = HashCache.Get().Add(string.Format("hair_{0:000}", p.hair)),
			headShape = HashCache.Get().Add(string.Format("headshape_{0:000}", p.headShape)),
			mouth = HashCache.Get().Add(string.Format("mouth_{0:000}", p.mouth)),
			neck = HashCache.Get().Add(string.Format("neck_{0:000}", p.neck)),
			arms = HashCache.Get().Add(string.Format("arm_{0:000}", p.body)),
			body = HashCache.Get().Add(string.Format("body_{0:000}", p.body)),
			hat = HashedString.Invalid
		};
	}

	public void ApplyAptitudes(GameObject go)
	{
		MinionResume component = go.GetComponent<MinionResume>();
		foreach (KeyValuePair<HashedString, float> keyValuePair in this.roleAptitudes)
		{
			component.AddAptitude(keyValuePair.Key, keyValuePair.Value);
		}
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
		go.GetComponent<MinionIdentity>().SetGender(this.GenderStringKey);
	}

	public GameObject Deliver(Vector3 location)
	{
		GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(MinionConfig.ID), null, null);
		gameObject.SetActive(true);
		gameObject.transform.SetLocalPosition(location);
		this.Apply(gameObject);
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		ChoreProvider component = gameObject.GetComponent<ChoreProvider>();
		new EmoteChore(component, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", Telepad.PortalBirthAnim, null);
		return gameObject;
	}

	public string Name;

	public string NameStringKey;

	public string GenderStringKey;

	public List<Trait> Traits = new List<Trait>();

	public Trait stressTrait;

	public Trait congenitaltrait;

	public int voiceIdx;

	public Dictionary<string, int> StartingLevels = new Dictionary<string, int>();

	public Personality personality;

	public List<Accessory> accessories = new List<Accessory>();

	public Dictionary<HashedString, float> roleAptitudes = new Dictionary<HashedString, float>();
}
