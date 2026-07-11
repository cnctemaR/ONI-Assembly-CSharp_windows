using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using TUNING;
using UnityEngine;

public class MinionStartingStats : ITelepadDeliverable
{
	public MinionStartingStats(bool is_starter_minion, string guaranteedAptitudeID = null)
	{
		if (is_starter_minion)
		{
			int num = global::UnityEngine.Random.Range(0, 29);
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
		this.GenerateAptitudes(guaranteedAptitudeID);
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
				else if (accessorySlot == Db.Get().AccessorySlots.HatHair || accessorySlot == Db.Get().AccessorySlots.HairAlways || accessorySlot == Db.Get().AccessorySlots.Hat)
				{
					accessory = accessorySlot.accessories[0];
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Body)
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
		Trait trait2 = Db.Get().traits.Get(this.personality.joyTrait);
		this.joyTrait = trait2;
		this.stickerType = this.personality.stickerType;
		Trait trait3 = Db.Get().traits.Get(this.personality.congenitaltrait);
		if (trait3.Name == "None")
		{
			this.congenitaltrait = null;
		}
		else
		{
			this.congenitaltrait = trait3;
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
						foreach (KeyValuePair<SkillGroup, float> keyValuePair in this.skillAptitudes)
						{
							if (flag2)
							{
								break;
							}
							using (List<HashedString>.Enumerator enumerator3 = traitVal.requiredNonPositiveAptitudes.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									if (enumerator3.Current == keyValuePair.Key.IdHash && keyValuePair.Value > 0f)
									{
										flag2 = true;
										break;
									}
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
						Trait trait4 = Db.Get().traits.TryGet(traitVal.id);
						if (trait4 == null)
						{
							global::Debug.LogWarning("Trying to add nonexistent trait: " + traitVal.id);
						}
						else if (!is_starter_minion || trait4.ValidStarterTrait)
						{
							selectedTraits.Add(traitVal.id);
							statDelta += traitVal.statBonus;
							this.Traits.Add(trait4);
							if (trait4.disabledChoreGroups != null)
							{
								for (int k = 0; k < trait4.disabledChoreGroups.Length; k++)
								{
									disabled_chore_groups.Add(trait4.disabledChoreGroups[k]);
								}
							}
							return true;
						}
					}
				}
			}
			return false;
		};
		int num = (is_starter_minion ? 1 : 3);
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

	private void GenerateAptitudes(string guaranteedAptitudeID = null)
	{
		int num = global::UnityEngine.Random.Range(1, 4);
		List<SkillGroup> list = new List<SkillGroup>(Db.Get().SkillGroups.resources);
		list.Shuffle<SkillGroup>();
		if (guaranteedAptitudeID != null)
		{
			this.skillAptitudes.Add(Db.Get().SkillGroups.Get(guaranteedAptitudeID), (float)DUPLICANTSTATS.APTITUDE_BONUS);
			list.Remove(Db.Get().SkillGroups.Get(guaranteedAptitudeID));
			num--;
		}
		for (int i = 0; i < num; i++)
		{
			this.skillAptitudes.Add(list[i], (float)DUPLICANTSTATS.APTITUDE_BONUS);
		}
	}

	private void GenerateAttributes(int pointsDelta, List<ChoreGroup> disabled_chore_groups)
	{
		int num = Mathf.RoundToInt(Util.GaussianRandom(0f, 1f) * ((float)DUPLICANTSTATS.MAX_STAT_POINTS - (float)DUPLICANTSTATS.MIN_STAT_POINTS) / 2f + (float)DUPLICANTSTATS.MIN_STAT_POINTS);
		List<string> list = new List<string>(DUPLICANTSTATS.ALL_ATTRIBUTES);
		int[] randomDistribution = DUPLICANTSTATS.DISTRIBUTIONS.GetRandomDistribution();
		for (int i = 0; i < list.Count; i++)
		{
			if (!this.StartingLevels.ContainsKey(list[i]))
			{
				this.StartingLevels[list[i]] = 0;
			}
		}
		foreach (KeyValuePair<SkillGroup, float> keyValuePair in this.skillAptitudes)
		{
			if (keyValuePair.Key.relevantAttributes.Count > 0)
			{
				for (int j = 0; j < keyValuePair.Key.relevantAttributes.Count; j++)
				{
					Dictionary<string, int> dictionary = this.StartingLevels;
					string text = keyValuePair.Key.relevantAttributes[j].Id;
					dictionary[text] += DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES[this.skillAptitudes.Count - 1];
				}
			}
		}
		list.Shuffle<string>();
		for (int k = 0; k < list.Count; k++)
		{
			string text2 = list[k];
			int num2 = randomDistribution[Mathf.Min(k, randomDistribution.Length - 1)];
			int num3 = Mathf.Min(num, num2);
			if (!this.StartingLevels.ContainsKey(text2))
			{
				this.StartingLevels[text2] = 0;
			}
			Dictionary<string, int> dictionary = this.StartingLevels;
			string text = text2;
			dictionary[text] += num3;
			num -= num3;
		}
		if (disabled_chore_groups.Count > 0)
		{
			int num4 = 0;
			int num5 = 0;
			foreach (KeyValuePair<string, int> keyValuePair2 in this.StartingLevels)
			{
				if (keyValuePair2.Value > num4)
				{
					num4 = keyValuePair2.Value;
				}
				if (keyValuePair2.Key == disabled_chore_groups[0].attribute.Id)
				{
					num5 = keyValuePair2.Value;
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
		go.GetComponent<MinionIdentity>().voiceIdx = this.voiceIdx;
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
		foreach (KeyValuePair<SkillGroup, float> keyValuePair in this.skillAptitudes)
		{
			component.SetAptitude(keyValuePair.Key.Id, keyValuePair.Value);
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
		component.Add(this.joyTrait);
		go.GetComponent<MinionIdentity>().SetStickerType(this.stickerType);
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
		new EmoteChore(gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", Telepad.PortalBirthAnim, null);
		return gameObject;
	}

	public string Name;

	public string NameStringKey;

	public string GenderStringKey;

	public List<Trait> Traits = new List<Trait>();

	public Trait stressTrait;

	public Trait joyTrait;

	public Trait congenitaltrait;

	public string stickerType;

	public int voiceIdx;

	public Dictionary<string, int> StartingLevels = new Dictionary<string, int>();

	public Personality personality;

	public List<Accessory> accessories = new List<Accessory>();

	public Dictionary<SkillGroup, float> skillAptitudes = new Dictionary<SkillGroup, float>();
}
