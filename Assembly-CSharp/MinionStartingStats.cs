using System;
using System.Collections.Generic;
using Database;
using Klei.AI;
using TUNING;
using UnityEngine;

public class MinionStartingStats : ITelepadDeliverable
{
	public MinionStartingStats(Personality personality, string guaranteedAptitudeID = null, string guaranteedTraitID = null, bool isDebugMinion = false)
	{
		this.personality = personality;
		this.GenerateStats(guaranteedAptitudeID, guaranteedTraitID, isDebugMinion, false);
	}

	public MinionStartingStats(bool is_starter_minion, string guaranteedAptitudeID = null, string guaranteedTraitID = null, bool isDebugMinion = false)
	{
		this.personality = Db.Get().Personalities.GetRandom(true, is_starter_minion);
		this.GenerateStats(guaranteedAptitudeID, guaranteedTraitID, isDebugMinion, is_starter_minion);
	}

	public MinionStartingStats(Tag model, bool is_starter_minion, string guaranteedAptitudeID = null, string guaranteedTraitID = null, bool isDebugMinion = false)
	{
		this.personality = Db.Get().Personalities.GetRandom(model, true, is_starter_minion);
		this.GenerateStats(guaranteedAptitudeID, guaranteedTraitID, isDebugMinion, is_starter_minion);
	}

	public MinionStartingStats(List<Tag> models, bool is_starter_minion, string guaranteedAptitudeID = null, string guaranteedTraitID = null, bool isDebugMinion = false)
	{
		this.personality = Db.Get().Personalities.GetRandom(models, true, is_starter_minion);
		this.GenerateStats(guaranteedAptitudeID, guaranteedTraitID, isDebugMinion, is_starter_minion);
	}

	private void GenerateStats(string guaranteedAptitudeID = null, string guaranteedTraitID = null, bool isDebugMinion = false, bool is_starter_minion = false)
	{
		this.voiceIdx = global::UnityEngine.Random.Range(0, 4);
		this.Name = this.personality.Name;
		this.NameStringKey = this.personality.nameStringKey;
		this.GenderStringKey = this.personality.genderStringKey;
		this.Traits.Add(Db.Get().traits.Get(BaseMinionConfig.GetMinionBaseTraitIDForModel(this.personality.model)));
		List<ChoreGroup> list = new List<ChoreGroup>();
		this.GenerateAptitudes(guaranteedAptitudeID);
		int num = this.GenerateTraits(is_starter_minion, list, guaranteedAptitudeID, guaranteedTraitID, isDebugMinion);
		this.GenerateAttributes(num, list);
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
				else if (accessorySlot == Db.Get().AccessorySlots.HatHair)
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
				else if (accessorySlot == Db.Get().AccessorySlots.ArmLower)
				{
					accessory = accessorySlot.Lookup(bodyData.armslower);
				}
				else if (accessorySlot == Db.Get().AccessorySlots.ArmLowerSkin)
				{
					accessory = accessorySlot.Lookup(bodyData.armLowerSkin);
				}
				else if (accessorySlot == Db.Get().AccessorySlots.ArmUpperSkin)
				{
					accessory = accessorySlot.Lookup(bodyData.armUpperSkin);
				}
				else if (accessorySlot == Db.Get().AccessorySlots.LegSkin)
				{
					accessory = accessorySlot.Lookup(bodyData.legSkin);
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Leg)
				{
					accessory = accessorySlot.Lookup(bodyData.legs);
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Belt)
				{
					accessory = accessorySlot.Lookup(bodyData.belt);
					if (accessory == null)
					{
						accessory = accessorySlot.accessories[0];
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Neck)
				{
					accessory = accessorySlot.Lookup(bodyData.neck);
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Pelvis)
				{
					accessory = accessorySlot.Lookup(bodyData.pelvis);
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Foot)
				{
					accessory = accessorySlot.Lookup(bodyData.foot);
					if (accessory == null)
					{
						accessory = accessorySlot.accessories[0];
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Skirt)
				{
					accessory = accessorySlot.Lookup(bodyData.skirt);
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Necklace)
				{
					accessory = accessorySlot.Lookup(bodyData.necklace);
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Cuff)
				{
					accessory = accessorySlot.Lookup(bodyData.cuff);
					if (accessory == null)
					{
						accessory = accessorySlot.accessories[0];
					}
				}
				else if (accessorySlot == Db.Get().AccessorySlots.Hand)
				{
					accessory = accessorySlot.Lookup(bodyData.hand);
					if (accessory == null)
					{
						accessory = accessorySlot.accessories[0];
					}
				}
				this.accessories.Add(accessory);
			}
		}
	}

	private int GenerateTraits(bool is_starter_minion, List<ChoreGroup> disabled_chore_groups, string guaranteedAptitudeID = null, string guaranteedTraitID = null, bool isDebugMinion = false)
	{
		MinionStartingStats.<>c__DisplayClass19_0 CS$<>8__locals1 = new MinionStartingStats.<>c__DisplayClass19_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.is_starter_minion = is_starter_minion;
		CS$<>8__locals1.isDebugMinion = isDebugMinion;
		CS$<>8__locals1.guaranteedAptitudeID = guaranteedAptitudeID;
		CS$<>8__locals1.disabled_chore_groups = disabled_chore_groups;
		CS$<>8__locals1.statDelta = 0;
		CS$<>8__locals1.selectedTraits = new List<string>();
		CS$<>8__locals1.randSeed = new KRandom();
		Trait trait = Db.Get().traits.Get(this.personality.stresstrait);
		this.stressTrait = trait;
		Trait trait2 = Db.Get().traits.Get(this.personality.joyTrait);
		this.joyTrait = trait2;
		this.stickerType = this.personality.stickerType;
		Trait trait3 = Db.Get().traits.TryGet(this.personality.congenitaltrait);
		if (trait3 == null || trait3.Name == "None")
		{
			this.congenitaltrait = null;
		}
		else
		{
			this.congenitaltrait = trait3;
		}
		if (this.personality.model == GameTags.Minions.Models.Bionic)
		{
			DUPLICANTSTATS.TraitVal random = DUPLICANTSTATS.BIONICBUGTRAITS.GetRandom<DUPLICANTSTATS.TraitVal>();
			CS$<>8__locals1.<GenerateTraits>g__SelectTrait|1(random, Db.Get().traits.Get(random.id), false);
			DUPLICANTSTATS.TraitVal traitVal = ((CS$<>8__locals1.guaranteedAptitudeID == null) ? DUPLICANTSTATS.BIONICUPGRADETRAITS.GetRandom<DUPLICANTSTATS.TraitVal>() : this.GetBionicTraitsCompatibleWithArchetype(CS$<>8__locals1.guaranteedAptitudeID).GetRandom<DUPLICANTSTATS.TraitVal>());
			CS$<>8__locals1.<GenerateTraits>g__SelectTrait|1(traitVal, Db.Get().traits.Get(traitVal.id), true);
			this.IsValid = true;
			return CS$<>8__locals1.statDelta;
		}
		Func<List<DUPLICANTSTATS.TraitVal>, bool, bool> func = delegate(List<DUPLICANTSTATS.TraitVal> traitPossibilities, bool positiveTrait)
		{
			if (CS$<>8__locals1.<>4__this.Traits.Count > DUPLICANTSTATS.MAX_TRAITS)
			{
				return false;
			}
			Mathf.Abs(Util.GaussianRandom(0f, 1f));
			int num6 = traitPossibilities.Count;
			int num7;
			if (!positiveTrait)
			{
				if (DUPLICANTSTATS.rarityDeckActive.Count < 1)
				{
					DUPLICANTSTATS.rarityDeckActive.AddRange(DUPLICANTSTATS.RARITY_DECK);
				}
				if (DUPLICANTSTATS.rarityDeckActive.Count == DUPLICANTSTATS.RARITY_DECK.Count)
				{
					DUPLICANTSTATS.rarityDeckActive.ShuffleSeeded<int>(CS$<>8__locals1.randSeed);
				}
				num7 = DUPLICANTSTATS.rarityDeckActive[DUPLICANTSTATS.rarityDeckActive.Count - 1];
				DUPLICANTSTATS.rarityDeckActive.RemoveAt(DUPLICANTSTATS.rarityDeckActive.Count - 1);
			}
			else
			{
				List<int> list = new List<int>();
				if (CS$<>8__locals1.is_starter_minion)
				{
					list.Add(CS$<>8__locals1.<>4__this.rarityBalance - 1);
					list.Add(CS$<>8__locals1.<>4__this.rarityBalance);
					list.Add(CS$<>8__locals1.<>4__this.rarityBalance);
					list.Add(CS$<>8__locals1.<>4__this.rarityBalance + 1);
				}
				else
				{
					list.Add(CS$<>8__locals1.<>4__this.rarityBalance - 2);
					list.Add(CS$<>8__locals1.<>4__this.rarityBalance - 1);
					list.Add(CS$<>8__locals1.<>4__this.rarityBalance);
					list.Add(CS$<>8__locals1.<>4__this.rarityBalance + 1);
					list.Add(CS$<>8__locals1.<>4__this.rarityBalance + 2);
				}
				list.ShuffleSeeded<int>(CS$<>8__locals1.randSeed);
				num7 = list[0];
				num7 = Mathf.Max(DUPLICANTSTATS.RARITY_COMMON, num7);
				num7 = Mathf.Min(DUPLICANTSTATS.RARITY_LEGENDARY, num7);
			}
			List<DUPLICANTSTATS.TraitVal> list2 = new List<DUPLICANTSTATS.TraitVal>(traitPossibilities);
			for (int j = list2.Count - 1; j > -1; j--)
			{
				if (list2[j].rarity != num7)
				{
					list2.RemoveAt(j);
					num6--;
				}
			}
			list2.ShuffleSeeded<DUPLICANTSTATS.TraitVal>(CS$<>8__locals1.randSeed);
			foreach (DUPLICANTSTATS.TraitVal traitVal4 in list2)
			{
				global::Debug.Assert(SaveLoader.Instance != null, "IsDLCActiveForCurrentSave should not be called from the front end");
				if (!SaveLoader.Instance.IsDLCActiveForCurrentSave(traitVal4.dlcId))
				{
					num6--;
				}
				else if (CS$<>8__locals1.selectedTraits.Contains(traitVal4.id))
				{
					num6--;
				}
				else
				{
					Trait trait5 = Db.Get().traits.TryGet(traitVal4.id);
					if (trait5 == null)
					{
						global::Debug.LogWarning("Trying to add nonexistent trait: " + traitVal4.id);
						num6--;
					}
					else if (!CS$<>8__locals1.isDebugMinion || trait5.disabledChoreGroups == null || trait5.disabledChoreGroups.Length == 0)
					{
						if (CS$<>8__locals1.is_starter_minion && !trait5.ValidStarterTrait)
						{
							num6--;
						}
						else if (traitVal4.doNotGenerateTrait)
						{
							num6--;
						}
						else if (CS$<>8__locals1.<>4__this.AreTraitAndAptitudesExclusive(traitVal4, CS$<>8__locals1.<>4__this.skillAptitudes))
						{
							num6--;
						}
						else if (CS$<>8__locals1.is_starter_minion && CS$<>8__locals1.guaranteedAptitudeID != null && CS$<>8__locals1.<>4__this.AreTraitAndArchetypeExclusive(traitVal4, CS$<>8__locals1.guaranteedAptitudeID))
						{
							num6--;
						}
						else
						{
							if (!CS$<>8__locals1.<>4__this.AreTraitsMutuallyExclusive(traitVal4, CS$<>8__locals1.selectedTraits))
							{
								base.<GenerateTraits>g__SelectTrait|1(traitVal4, trait5, positiveTrait);
								return true;
							}
							num6--;
						}
					}
				}
			}
			return false;
		};
		int num;
		int num2;
		if (CS$<>8__locals1.is_starter_minion)
		{
			num = 1;
			num2 = 1;
		}
		else
		{
			if (DUPLICANTSTATS.podTraitConfigurationsActive.Count < 1)
			{
				DUPLICANTSTATS.podTraitConfigurationsActive.AddRange(DUPLICANTSTATS.POD_TRAIT_CONFIGURATIONS_DECK);
			}
			if (DUPLICANTSTATS.podTraitConfigurationsActive.Count == DUPLICANTSTATS.POD_TRAIT_CONFIGURATIONS_DECK.Count)
			{
				DUPLICANTSTATS.podTraitConfigurationsActive.ShuffleSeeded<global::Tuple<int, int>>(CS$<>8__locals1.randSeed);
			}
			num = DUPLICANTSTATS.podTraitConfigurationsActive[DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1].first;
			num2 = DUPLICANTSTATS.podTraitConfigurationsActive[DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1].second;
			DUPLICANTSTATS.podTraitConfigurationsActive.RemoveAt(DUPLICANTSTATS.podTraitConfigurationsActive.Count - 1);
		}
		bool flag = false;
		int num3 = 0;
		int num4 = 0;
		int num5 = (num2 + num) * 4;
		if (!string.IsNullOrEmpty(guaranteedTraitID))
		{
			DUPLICANTSTATS.TraitVal traitVal2 = DUPLICANTSTATS.GetTraitVal(guaranteedTraitID);
			if (traitVal2.id == guaranteedTraitID)
			{
				Trait trait4 = Db.Get().traits.TryGet(traitVal2.id);
				bool positiveTrait2 = trait4.PositiveTrait;
				CS$<>8__locals1.selectedTraits.Add(traitVal2.id);
				CS$<>8__locals1.statDelta += traitVal2.statBonus;
				this.rarityBalance += (positiveTrait2 ? (-traitVal2.rarity) : traitVal2.rarity);
				this.Traits.Add(trait4);
				if (trait4.disabledChoreGroups != null)
				{
					for (int i = 0; i < trait4.disabledChoreGroups.Length; i++)
					{
						CS$<>8__locals1.disabled_chore_groups.Add(trait4.disabledChoreGroups[i]);
					}
				}
				if (positiveTrait2)
				{
					num3++;
				}
				else
				{
					num4++;
				}
			}
		}
		if (!flag)
		{
			if (this.congenitaltrait != null)
			{
				DUPLICANTSTATS.TraitVal traitVal3;
				if (this.congenitaltrait.PositiveTrait)
				{
					num3++;
					traitVal3 = DUPLICANTSTATS.GOODTRAITS.Find((DUPLICANTSTATS.TraitVal match) => match.id == CS$<>8__locals1.<>4__this.congenitaltrait.Id);
				}
				else
				{
					num4++;
					traitVal3 = DUPLICANTSTATS.BADTRAITS.Find((DUPLICANTSTATS.TraitVal match) => match.id == CS$<>8__locals1.<>4__this.congenitaltrait.Id);
				}
				CS$<>8__locals1.<GenerateTraits>g__SelectTrait|1(traitVal3, this.congenitaltrait, this.congenitaltrait.PositiveTrait);
			}
		}
		while (num5 > 0 && (num4 < num2 || num3 < num))
		{
			if (num4 < num2 && func(DUPLICANTSTATS.BADTRAITS, false))
			{
				num4++;
			}
			if (num3 < num && func(DUPLICANTSTATS.GOODTRAITS, true))
			{
				num3++;
			}
			num5--;
		}
		if (num5 > 0)
		{
			this.IsValid = true;
		}
		return CS$<>8__locals1.statDelta;
	}

	private void GenerateAptitudes(string guaranteedAptitudeID = null)
	{
		if (this.personality.model == BionicMinionConfig.MODEL)
		{
			return;
		}
		int num = global::UnityEngine.Random.Range(1, 4);
		List<SkillGroup> list = new List<SkillGroup>(Db.Get().SkillGroups.resources);
		list.RemoveAll((SkillGroup match) => !match.allowAsAptitude);
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
		List<string> list = new List<string>(DUPLICANTSTATS.ALL_ATTRIBUTES);
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
					if (!this.StartingLevels.ContainsKey(keyValuePair.Key.relevantAttributes[j].Id))
					{
						global::Debug.LogError("Need to add " + keyValuePair.Key.relevantAttributes[j].Id + " to TUNING.DUPLICANTSTATS.ALL_ATTRIBUTES");
					}
					Dictionary<string, int> dictionary = this.StartingLevels;
					string text = keyValuePair.Key.relevantAttributes[j].Id;
					dictionary[text] += DUPLICANTSTATS.APTITUDE_ATTRIBUTE_BONUSES[this.skillAptitudes.Count - 1];
				}
			}
		}
		List<SkillGroup> list2 = new List<SkillGroup>(this.skillAptitudes.Keys);
		if (pointsDelta > 0)
		{
			for (int k = pointsDelta; k > 0; k--)
			{
				list2.Shuffle<SkillGroup>();
				for (int l = 0; l < list2[0].relevantAttributes.Count; l++)
				{
					Dictionary<string, int> dictionary = this.StartingLevels;
					string text = list2[0].relevantAttributes[l].Id;
					dictionary[text]++;
				}
			}
		}
		if (disabled_chore_groups.Count > 0)
		{
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<string, int> keyValuePair2 in this.StartingLevels)
			{
				if (keyValuePair2.Value > num)
				{
					num = keyValuePair2.Value;
				}
				if (keyValuePair2.Key == disabled_chore_groups[0].attribute.Id)
				{
					num2 = keyValuePair2.Value;
				}
			}
			if (num == num2)
			{
				foreach (string text2 in list)
				{
					if (text2 != disabled_chore_groups[0].attribute.Id)
					{
						int num3 = 0;
						this.StartingLevels.TryGetValue(text2, out num3);
						int num4 = 0;
						if (num3 > 0)
						{
							num4 = 1;
						}
						this.StartingLevels[disabled_chore_groups[0].attribute.Id] = num3 - num4;
						this.StartingLevels[text2] = num + num4;
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
		component.personalityResourceId = this.personality.IdHash;
		component.model = this.personality.model;
		this.ApplyTraits(go);
		this.ApplyRace(go);
		this.ApplyAptitudes(go);
		this.ApplyAccessories(go);
		this.ApplyExperience(go);
		this.ApplyOutfit(this.personality, go);
		this.ApplyJoyResponseOutfit(this.personality, go);
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
		component.ApplyMinionPersonality(this.personality);
		component.UpdateHairBasedOnHat();
	}

	public void ApplyOutfit(Personality personality, GameObject go)
	{
		WearableAccessorizer component = go.GetComponent<WearableAccessorizer>();
		Option<ClothingOutfitTarget> option = ClothingOutfitTarget.TryFromTemplateId(personality.GetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType.Clothing));
		if (option.IsSome())
		{
			component.ApplyClothingItems(ClothingOutfitUtility.OutfitType.Clothing, option.Unwrap().ReadItemValues());
		}
	}

	public void ApplyJoyResponseOutfit(Personality personality, GameObject go)
	{
		JoyResponseOutfitTarget joyResponseOutfitTarget = JoyResponseOutfitTarget.FromPersonality(personality);
		JoyResponseOutfitTarget.FromMinion(go).WriteFacadeId(joyResponseOutfitTarget.ReadFacadeId());
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
			neck = HashCache.Get().Add("neck"),
			arms = HashCache.Get().Add(string.Format("arm_sleeve_{0:000}", p.body)),
			armslower = HashCache.Get().Add(string.Format("arm_lower_sleeve_{0:000}", p.body)),
			body = HashCache.Get().Add(string.Format("torso_{0:000}", p.body)),
			hat = HashedString.Invalid,
			faceFX = HashedString.Invalid,
			armLowerSkin = HashCache.Get().Add(string.Format("arm_lower_{0:000}", (p.arm_skin != 0) ? p.arm_skin : p.headShape)),
			armUpperSkin = HashCache.Get().Add(string.Format("arm_upper_{0:000}", (p.arm_skin != 0) ? p.arm_skin : p.headShape)),
			legSkin = HashCache.Get().Add(string.Format("leg_skin_{0:000}", (p.leg_skin != 0) ? p.leg_skin : p.headShape)),
			neck = HashCache.Get().Add((p.neck != 0) ? string.Format("neck_{0:000}", p.neck) : "neck"),
			legs = HashCache.Get().Add((p.leg != 0) ? string.Format("leg_{0:000}", p.leg) : "leg"),
			belt = HashCache.Get().Add((p.belt != 0) ? string.Format("belt_{0:000}", p.belt) : "belt"),
			pelvis = HashCache.Get().Add((p.pelvis != 0) ? string.Format("pelvis_{0:000}", p.pelvis) : "pelvis"),
			foot = HashCache.Get().Add((p.foot != 0) ? string.Format("foot_{0:000}", p.foot) : "foot"),
			hand = HashCache.Get().Add((p.hand != 0) ? string.Format("hand_paint_{0:000}", p.hand) : "hand_paint"),
			cuff = HashCache.Get().Add((p.cuff != 0) ? string.Format("cuff_{0:000}", p.cuff) : "cuff")
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
		component.Add(this.joyTrait);
		go.GetComponent<MinionIdentity>().SetStickerType(this.stickerType);
		MinionIdentity component2 = go.GetComponent<MinionIdentity>();
		component2.SetName(this.Name);
		component2.nameStringKey = this.NameStringKey;
		go.GetComponent<MinionIdentity>().SetGender(this.GenderStringKey);
	}

	public GameObject Deliver(Vector3 location)
	{
		GameObject prefab = Assets.GetPrefab(this.personality.model);
		GameObject gameObject = Util.KInstantiate(prefab, null, null);
		gameObject.name = prefab.name;
		gameObject.SetActive(true);
		gameObject.transform.SetLocalPosition(location);
		this.Apply(gameObject);
		Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
		new EmoteChore(gameObject.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", Telepad.PortalBirthAnim, null);
		return gameObject;
	}

	private List<DUPLICANTSTATS.TraitVal> GetBionicTraitsCompatibleWithArchetype(string guaranteedAptitudeID)
	{
		if (!DUPLICANTSTATS.ARCHETYPE_BIONIC_TRAIT_COMPATIBILITY.ContainsKey(guaranteedAptitudeID))
		{
			global::Debug.LogError("Need to add attribute " + guaranteedAptitudeID + " to ARCHETYPE_BIONIC_TRAIT_COMPATIBILITY");
		}
		List<DUPLICANTSTATS.TraitVal> list = DUPLICANTSTATS.BIONICUPGRADETRAITS.FindAll((DUPLICANTSTATS.TraitVal t) => DUPLICANTSTATS.ARCHETYPE_BIONIC_TRAIT_COMPATIBILITY[guaranteedAptitudeID].Contains(t.id) || DUPLICANTSTATS.ARCHETYPE_BIONIC_TRAIT_COMPATIBILITY[guaranteedAptitudeID].Contains(t.id.Replace("StartWith", "")));
		if (list.Count <= 0)
		{
			list = DUPLICANTSTATS.BIONICUPGRADETRAITS;
		}
		return list;
	}

	private bool AreTraitAndAptitudesExclusive(DUPLICANTSTATS.TraitVal traitVal, Dictionary<SkillGroup, float> aptitudes)
	{
		if (traitVal.mutuallyExclusiveAptitudes == null)
		{
			return false;
		}
		foreach (KeyValuePair<SkillGroup, float> keyValuePair in this.skillAptitudes)
		{
			using (List<HashedString>.Enumerator enumerator2 = traitVal.mutuallyExclusiveAptitudes.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == keyValuePair.Key.IdHash && keyValuePair.Value > 0f)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private bool AreTraitAndArchetypeExclusive(DUPLICANTSTATS.TraitVal traitVal, string guaranteedAptitudeID)
	{
		if (!DUPLICANTSTATS.ARCHETYPE_TRAIT_EXCLUSIONS.ContainsKey(guaranteedAptitudeID))
		{
			global::Debug.LogError("Need to add attribute " + guaranteedAptitudeID + " to ARCHETYPE_TRAIT_EXCLUSIONS");
		}
		using (List<string>.Enumerator enumerator = DUPLICANTSTATS.ARCHETYPE_TRAIT_EXCLUSIONS[guaranteedAptitudeID].GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == traitVal.id)
				{
					return true;
				}
			}
		}
		return false;
	}

	private bool AreTraitsMutuallyExclusive(DUPLICANTSTATS.TraitVal traitVal, List<string> selectedTraits)
	{
		foreach (string text in selectedTraits)
		{
			foreach (DUPLICANTSTATS.TraitVal traitVal2 in DUPLICANTSTATS.GOODTRAITS)
			{
				if (text == traitVal2.id && traitVal2.mutuallyExclusiveTraits != null && traitVal2.mutuallyExclusiveTraits.Contains(traitVal.id))
				{
					return true;
				}
			}
			foreach (DUPLICANTSTATS.TraitVal traitVal3 in DUPLICANTSTATS.BADTRAITS)
			{
				if (text == traitVal3.id && traitVal3.mutuallyExclusiveTraits != null && traitVal3.mutuallyExclusiveTraits.Contains(traitVal.id))
				{
					return true;
				}
			}
			foreach (DUPLICANTSTATS.TraitVal traitVal4 in DUPLICANTSTATS.CONGENITALTRAITS)
			{
				if (text == traitVal4.id && traitVal4.mutuallyExclusiveTraits != null && traitVal4.mutuallyExclusiveTraits.Contains(traitVal.id))
				{
					return true;
				}
			}
			foreach (DUPLICANTSTATS.TraitVal traitVal5 in DUPLICANTSTATS.SPECIALTRAITS)
			{
				if (text == traitVal5.id && traitVal5.mutuallyExclusiveTraits != null && traitVal5.mutuallyExclusiveTraits.Contains(traitVal.id))
				{
					return true;
				}
			}
			if (traitVal.mutuallyExclusiveTraits != null && traitVal.mutuallyExclusiveTraits.Contains(text))
			{
				return true;
			}
		}
		return false;
	}

	public string Name;

	public string NameStringKey;

	public string GenderStringKey;

	public List<Trait> Traits = new List<Trait>();

	public int rarityBalance;

	public Trait stressTrait;

	public Trait joyTrait;

	public Trait congenitaltrait;

	public string stickerType;

	public int voiceIdx;

	public Dictionary<string, int> StartingLevels = new Dictionary<string, int>();

	public Personality personality;

	public List<Accessory> accessories = new List<Accessory>();

	public bool IsValid;

	public Dictionary<SkillGroup, float> skillAptitudes = new Dictionary<SkillGroup, float>();
}
