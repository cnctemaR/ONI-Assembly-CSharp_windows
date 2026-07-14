using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Database;
using Klei.AI;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/MinionResume")]
public class MinionResume : IExperienceRecipient, ISaveLoadable, ISim200ms
{
	public MinionIdentity GetIdentity
	{
		get
		{
			return this.identity;
		}
	}

	public float TotalExperienceGained
	{
		get
		{
			return this.totalExperienceGained;
		}
	}

	public int TotalSkillPointsGained
	{
		get
		{
			return MinionResume.CalculateTotalSkillPointsGained(this.TotalExperienceGained);
		}
	}

	public static int CalculateTotalSkillPointsGained(float experience)
	{
		return Mathf.FloorToInt(Mathf.Pow(experience / (float)SKILLS.TARGET_SKILLS_CYCLE / 600f, 1f / SKILLS.EXPERIENCE_LEVEL_POWER) * (float)SKILLS.TARGET_SKILLS_EARNED);
	}

	public int SkillsMastered
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
			{
				if (keyValuePair.Value)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int AvailableSkillpoints
	{
		get
		{
			return this.TotalSkillPointsGained - this.SkillsMastered + ((this.GrantedSkillIDs == null) ? 0 : this.GrantedSkillIDs.Count);
		}
	}

	[OnDeserialized]
	private void OnDeserializedMethod()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 7))
		{
			foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryByRoleID)
			{
				if (keyValuePair.Value && keyValuePair.Key != "NoRole")
				{
					this.ForceAddSkillPoint();
				}
			}
			foreach (KeyValuePair<HashedString, float> keyValuePair2 in this.AptitudeByRoleGroup)
			{
				this.AptitudeBySkillGroup[keyValuePair2.Key] = keyValuePair2.Value;
			}
		}
		if (this.TotalSkillPointsGained > 1000 || this.TotalSkillPointsGained < 0)
		{
			this.ForceSetSkillPoints(100);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.MinionResumes.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.GrantedSkillIDs.RemoveAll((string x) => Db.Get().Skills.TryGet(x) == null);
		List<string> list = new List<string>();
		foreach (string text in this.MasteryBySkillID.Keys)
		{
			if (Db.Get().Skills.TryGet(text) == null)
			{
				list.Add(text);
			}
		}
		foreach (string text2 in list)
		{
			this.MasteryBySkillID.Remove(text2);
		}
		if (this.GrantedSkillIDs == null)
		{
			this.GrantedSkillIDs = new List<string>();
		}
		List<string> list2 = new List<string>();
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value && Db.Get().Skills.Get(keyValuePair.Key).deprecated)
			{
				list2.Add(keyValuePair.Key);
			}
		}
		foreach (string text3 in list2)
		{
			this.UnmasterSkill(text3);
		}
		foreach (KeyValuePair<string, bool> keyValuePair2 in this.MasteryBySkillID)
		{
			if (keyValuePair2.Value)
			{
				Skill skill = Db.Get().Skills.Get(keyValuePair2.Key);
				foreach (SkillPerk skillPerk in skill.perks)
				{
					if (Game.IsCorrectDlcActiveForCurrentSave(skillPerk))
					{
						if (skillPerk.OnRemove != null)
						{
							skillPerk.OnRemove(this);
						}
						if (skillPerk.OnApply != null)
						{
							skillPerk.OnApply(this);
						}
					}
				}
				if (!this.ownedHats.ContainsKey(skill.hat))
				{
					this.ownedHats.Add(skill.hat, true);
				}
			}
		}
		this.UpdateExpectations();
		this.UpdateMorale();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		MinionResume.ApplyHat(this.currentHat, component);
		this.ShowNewSkillPointNotification();
	}

	public void RestoreResume(Dictionary<string, bool> MasteryBySkillID, Dictionary<HashedString, float> AptitudeBySkillGroup, List<string> GrantedSkillIDs, float totalExperienceGained)
	{
		this.MasteryBySkillID = MasteryBySkillID;
		this.GrantedSkillIDs = ((GrantedSkillIDs != null) ? GrantedSkillIDs : new List<string>());
		this.AptitudeBySkillGroup = AptitudeBySkillGroup;
		this.totalExperienceGained = totalExperienceGained;
	}

	protected override void OnCleanUp()
	{
		Components.MinionResumes.Remove(this);
		if (this.lastSkillNotification != null)
		{
			Game.Instance.GetComponent<Notifier>().Remove(this.lastSkillNotification);
			this.lastSkillNotification = null;
		}
		base.OnCleanUp();
	}

	public bool HasMasteredSkill(string skillId)
	{
		return this.MasteryBySkillID.ContainsKey(skillId) && this.MasteryBySkillID[skillId];
	}

	public void UpdateUrge()
	{
		if (this.targetHat != this.currentHat)
		{
			if (!base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
			{
				base.gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.LearnSkill);
				return;
			}
		}
		else
		{
			base.gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.LearnSkill);
		}
	}

	public string CurrentRole
	{
		get
		{
			return this.currentRole;
		}
	}

	public string CurrentHat
	{
		get
		{
			return this.currentHat;
		}
	}

	public string TargetHat
	{
		get
		{
			return this.targetHat;
		}
	}

	public void SetHats(string current, string target)
	{
		this.currentHat = current;
		this.targetHat = target;
	}

	public void ClearAdditionalHats()
	{
		this.AdditionalHats.Clear();
	}

	public void AddAdditionalHat(string context, string hat)
	{
		MinionResume.HatInfo hatInfo = null;
		foreach (MinionResume.HatInfo hatInfo2 in this.AdditionalHats)
		{
			if (hatInfo2.Source == context && hatInfo2.Hat == hat)
			{
				hatInfo = hatInfo2;
				break;
			}
		}
		if (hatInfo != null)
		{
			hatInfo.count++;
			return;
		}
		this.AdditionalHats.Add(new MinionResume.HatInfo(context, hat));
	}

	public void RemoveAdditionalHat(string context, string hat)
	{
		MinionResume.HatInfo hatInfo = null;
		foreach (MinionResume.HatInfo hatInfo2 in this.AdditionalHats)
		{
			if (hatInfo2.Source == context && hatInfo2.Hat == hat)
			{
				hatInfo2.count--;
				hatInfo = hatInfo2;
				break;
			}
		}
		if (hatInfo != null && hatInfo.count <= 0)
		{
			this.AdditionalHats.Remove(hatInfo);
			if (this.currentHat == hat)
			{
				this.RemoveHat();
			}
		}
	}

	public void SetCurrentRole(string role_id)
	{
		this.currentRole = role_id;
	}

	public string TargetRole
	{
		get
		{
			return this.targetRole;
		}
	}

	public void ApplyAdditionalSkillPerks(SkillPerk[] perks)
	{
		foreach (SkillPerk skillPerk in perks)
		{
			if (Game.IsCorrectDlcActiveForCurrentSave(skillPerk))
			{
				this.AdditionalGrantedSkillPerkIDs.Add(skillPerk.IdHash);
				if (skillPerk.OnApply != null)
				{
					skillPerk.OnApply(this);
				}
			}
		}
		Game.Instance.Trigger(-1523247426, null);
	}

	public void RemoveAdditionalSkillPerks(SkillPerk[] perks)
	{
		foreach (SkillPerk skillPerk in perks)
		{
			if (Game.IsCorrectDlcActiveForCurrentSave(skillPerk))
			{
				this.AdditionalGrantedSkillPerkIDs.Remove(skillPerk.IdHash);
				if (skillPerk.OnRemove != null)
				{
					skillPerk.OnRemove(this);
				}
			}
		}
		Game.Instance.Trigger(-1523247426, null);
	}

	private void ApplySkillPerksForSkill(string skillId)
	{
		foreach (SkillPerk skillPerk in Db.Get().Skills.Get(skillId).perks)
		{
			if (Game.IsCorrectDlcActiveForCurrentSave(skillPerk) && skillPerk.OnApply != null)
			{
				skillPerk.OnApply(this);
			}
		}
	}

	private void RemoveSkillPerksForSkill(string skillId)
	{
		foreach (SkillPerk skillPerk in Db.Get().Skills.Get(skillId).perks)
		{
			if (Game.IsCorrectDlcActiveForCurrentSave(skillPerk) && skillPerk.OnRemove != null)
			{
				skillPerk.OnRemove(this);
			}
		}
	}

	public void Sim200ms(float dt)
	{
		this.DEBUG_SecondsAlive += dt;
		if (!base.GetComponent<KPrefabID>().HasTag(GameTags.Dead))
		{
			this.DEBUG_PassiveExperienceGained += dt * SKILLS.PASSIVE_EXPERIENCE_PORTION;
			this.AddExperience(dt * SKILLS.PASSIVE_EXPERIENCE_PORTION);
		}
	}

	public bool IsAbleToLearnSkill(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		string choreGroupID = Db.Get().SkillGroups.Get(skill.skillGroup).choreGroupID;
		if (!string.IsNullOrEmpty(choreGroupID))
		{
			Traits component = base.GetComponent<Traits>();
			if (component != null && component.IsChoreGroupDisabled(choreGroupID))
			{
				return false;
			}
		}
		return true;
	}

	public bool BelowMoraleExpectation(Skill skill)
	{
		float num = Db.Get().Attributes.QualityOfLife.Lookup(this).GetTotalValue();
		float totalValue = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(this).GetTotalValue();
		int moraleExpectation = skill.GetMoraleExpectation();
		if (this.AptitudeBySkillGroup.ContainsKey(skill.skillGroup) && this.AptitudeBySkillGroup[skill.skillGroup] > 0f)
		{
			num += 1f;
		}
		return totalValue + (float)moraleExpectation <= num;
	}

	public bool HasMasteredDirectlyRequiredSkillsForSkill(Skill skill)
	{
		for (int i = 0; i < skill.priorSkills.Count; i++)
		{
			if (!this.HasMasteredSkill(skill.priorSkills[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool HasSkillPointsRequiredForSkill(Skill skill)
	{
		return this.AvailableSkillpoints >= 1;
	}

	public bool HasSkillAptitude(Skill skill)
	{
		return this.AptitudeBySkillGroup.ContainsKey(skill.skillGroup) && this.AptitudeBySkillGroup[skill.skillGroup] > 0f;
	}

	public bool HasBeenGrantedSkill(Skill skill)
	{
		return this.GrantedSkillIDs != null && this.GrantedSkillIDs.Contains(skill.Id);
	}

	public bool HasBeenGrantedSkill(string id)
	{
		return this.GrantedSkillIDs != null && this.GrantedSkillIDs.Contains(id);
	}

	public MinionResume.SkillMasteryConditions[] GetSkillMasteryConditions(string skillId)
	{
		List<MinionResume.SkillMasteryConditions> list = new List<MinionResume.SkillMasteryConditions>();
		Skill skill = Db.Get().Skills.Get(skillId);
		if (this.HasSkillAptitude(skill))
		{
			list.Add(MinionResume.SkillMasteryConditions.SkillAptitude);
		}
		if (!this.BelowMoraleExpectation(skill))
		{
			list.Add(MinionResume.SkillMasteryConditions.StressWarning);
		}
		if (!this.IsAbleToLearnSkill(skillId))
		{
			list.Add(MinionResume.SkillMasteryConditions.UnableToLearn);
		}
		if (!this.HasSkillPointsRequiredForSkill(skill))
		{
			list.Add(MinionResume.SkillMasteryConditions.NeedsSkillPoints);
		}
		if (!this.HasMasteredDirectlyRequiredSkillsForSkill(skill))
		{
			list.Add(MinionResume.SkillMasteryConditions.MissingPreviousSkill);
		}
		return list.ToArray();
	}

	public bool CanMasterSkill(MinionResume.SkillMasteryConditions[] masteryConditions)
	{
		return !Array.Exists<MinionResume.SkillMasteryConditions>(masteryConditions, (MinionResume.SkillMasteryConditions element) => element == MinionResume.SkillMasteryConditions.UnableToLearn || element == MinionResume.SkillMasteryConditions.NeedsSkillPoints || element == MinionResume.SkillMasteryConditions.MissingPreviousSkill);
	}

	public bool OwnsHat(string hatId)
	{
		using (List<MinionResume.HatInfo>.Enumerator enumerator = this.AdditionalHats.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Hat == hatId)
				{
					return true;
				}
			}
		}
		return this.ownedHats.ContainsKey(hatId) && this.ownedHats[hatId];
	}

	public List<MinionResume.HatInfo> GetAllHats()
	{
		List<MinionResume.HatInfo> list = new List<MinionResume.HatInfo>();
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value)
			{
				Skill skill = Db.Get().Skills.TryGet(keyValuePair.Key);
				if (!skill.hat.IsNullOrWhiteSpace())
				{
					list.Add(new MinionResume.HatInfo(skill.Name, skill.hat));
				}
			}
		}
		list.AddRange(this.AdditionalHats);
		return list;
	}

	public void SkillLearned()
	{
		if (base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
		{
			base.gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.LearnSkill);
		}
		foreach (string text in this.ownedHats.Keys.ToList<string>())
		{
			this.ownedHats[text] = true;
		}
		if (this.targetHat != null && this.currentHat != this.targetHat)
		{
			this.CreateHatChangeChore();
		}
	}

	public void CreateHatChangeChore()
	{
		if (this.lastHatChore != null)
		{
			this.lastHatChore.Cancel("New Hat");
		}
		this.lastHatChore = new PutOnHatChore(this, Db.Get().ChoreTypes.SwitchHat);
	}

	public void MasterSkill(string skillId)
	{
		if (!base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
		{
			base.gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.LearnSkill);
		}
		this.MasteryBySkillID[skillId] = true;
		this.ApplySkillPerksForSkill(skillId);
		this.UpdateExpectations();
		this.UpdateMorale();
		this.TriggerMasterSkillEvents();
		GameScheduler.Instance.Schedule("Morale Tutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Morale, true);
		}, null, null);
		if (!this.ownedHats.ContainsKey(Db.Get().Skills.Get(skillId).hat))
		{
			this.ownedHats.Add(Db.Get().Skills.Get(skillId).hat, false);
		}
		if (this.AvailableSkillpoints == 0 && this.lastSkillNotification != null)
		{
			Game.Instance.GetComponent<Notifier>().Remove(this.lastSkillNotification);
			this.lastSkillNotification = null;
		}
	}

	public void UnmasterSkill(string skillId)
	{
		if (this.MasteryBySkillID.ContainsKey(skillId))
		{
			this.MasteryBySkillID.Remove(skillId);
			this.RemoveSkillPerksForSkill(skillId);
			this.UpdateExpectations();
			this.UpdateMorale();
			this.TriggerMasterSkillEvents();
		}
	}

	public void GrantSkill(string skillId)
	{
		if (this.GrantedSkillIDs == null)
		{
			this.GrantedSkillIDs = new List<string>();
		}
		if (!this.HasBeenGrantedSkill(skillId))
		{
			this.MasteryBySkillID[skillId] = true;
			this.ApplySkillPerksForSkill(skillId);
			this.GrantedSkillIDs.Add(skillId);
			this.UpdateExpectations();
			this.UpdateMorale();
			this.TriggerMasterSkillEvents();
			if (!this.ownedHats.ContainsKey(Db.Get().Skills.Get(skillId).hat))
			{
				this.ownedHats.Add(Db.Get().Skills.Get(skillId).hat, false);
			}
		}
	}

	public void UngrantSkill(string skillId)
	{
		if (this.GrantedSkillIDs != null)
		{
			this.GrantedSkillIDs.RemoveAll((string match) => match == skillId);
		}
		this.UnmasterSkill(skillId);
	}

	public Sprite GetSkillGrantSourceIcon(string skillID)
	{
		if (!this.GrantedSkillIDs.Contains(skillID))
		{
			return null;
		}
		BionicUpgradesMonitor.Instance smi = base.gameObject.GetSMI<BionicUpgradesMonitor.Instance>();
		if (smi != null)
		{
			foreach (BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot in smi.upgradeComponentSlots)
			{
				if (upgradeComponentSlot.HasUpgradeInstalled)
				{
					return Def.GetUISprite(upgradeComponentSlot.installedUpgradeComponent.gameObject, "ui", false).first;
				}
			}
		}
		return Assets.GetSprite("skill_granted_trait");
	}

	private void TriggerMasterSkillEvents()
	{
		base.Trigger(540773776, null);
		Game.Instance.Trigger(-1523247426, this);
	}

	public void ForceSetSkillPoints(int points)
	{
		this.totalExperienceGained = MinionResume.CalculatePreviousExperienceBar(points);
		if (this.TotalSkillPointsGained != points)
		{
			float num = MinionResume.CalculatePreviousExperienceBar(points);
			float num2 = MinionResume.CalculateNextExperienceBar(points);
			this.totalExperienceGained = num + (num2 - num) * 0.01f;
		}
	}

	public void ForceAddSkillPoint()
	{
		this.ForceSetSkillPoints(this.TotalSkillPointsGained + 1);
	}

	public static float CalculateNextExperienceBar(int current_skill_points)
	{
		return Mathf.Pow((float)(current_skill_points + 1) / (float)SKILLS.TARGET_SKILLS_EARNED, SKILLS.EXPERIENCE_LEVEL_POWER) * (float)SKILLS.TARGET_SKILLS_CYCLE * 600f;
	}

	public static float CalculatePreviousExperienceBar(int current_skill_points)
	{
		return Mathf.Pow((float)current_skill_points / (float)SKILLS.TARGET_SKILLS_EARNED, SKILLS.EXPERIENCE_LEVEL_POWER) * (float)SKILLS.TARGET_SKILLS_CYCLE * 600f;
	}

	private void UpdateExpectations()
	{
		int num = 0;
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value && !this.HasBeenGrantedSkill(keyValuePair.Key))
			{
				Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
				num += skill.tier + 1;
			}
		}
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(this);
		if (this.skillsMoraleExpectationModifier != null)
		{
			attributeInstance.Remove(this.skillsMoraleExpectationModifier);
			this.skillsMoraleExpectationModifier = null;
		}
		if (num > 0)
		{
			this.skillsMoraleExpectationModifier = new AttributeModifier(attributeInstance.Id, (float)num, DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_MOD_NAME, false, false, true);
			attributeInstance.Add(this.skillsMoraleExpectationModifier);
		}
	}

	private void UpdateMorale()
	{
		int num = 0;
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value && !this.HasBeenGrantedSkill(keyValuePair.Key))
			{
				Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
				float num2 = 0f;
				if (this.AptitudeBySkillGroup.TryGetValue(new HashedString(skill.skillGroup), out num2))
				{
					num += (int)num2;
				}
			}
		}
		AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(this);
		if (this.skillsMoraleModifier != null)
		{
			attributeInstance.Remove(this.skillsMoraleModifier);
			this.skillsMoraleModifier = null;
		}
		if (num > 0)
		{
			this.skillsMoraleModifier = new AttributeModifier(attributeInstance.Id, (float)num, DUPLICANTS.NEEDS.QUALITYOFLIFE.APTITUDE_SKILLS_MOD_NAME, false, false, true);
			attributeInstance.Add(this.skillsMoraleModifier);
		}
	}

	private void OnSkillPointGained()
	{
		Game.Instance.Trigger(1505456302, this);
		this.ShowNewSkillPointNotification();
		if (PopFXManager.Instance != null)
		{
			string text = MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME.Replace("{Duplicant}", this.identity.GetProperName());
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, text, base.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
		}
		new UpgradeFX.Instance(base.gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f)).StartSM();
	}

	private void ShowNewSkillPointNotification()
	{
		if (this.AvailableSkillpoints == 1)
		{
			this.lastSkillNotification = new ManagementMenuNotification(global::Action.ManageSkills, NotificationValence.Good, this.identity.GetSoleOwner().gameObject.GetInstanceID().ToString(), MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME.Replace("{Duplicant}", this.identity.GetProperName()), NotificationType.Good, new Func<List<Notification>, object, string>(this.GetSkillPointGainedTooltip), this.identity, true, 0f, delegate(object d)
			{
				ManagementMenu.Instance.OpenSkills(this.identity);
			}, null, null, true);
			base.GetComponent<Notifier>().Add(this.lastSkillNotification, "");
		}
	}

	private string GetSkillPointGainedTooltip(List<Notification> notifications, object data)
	{
		return MISC.NOTIFICATIONS.SKILL_POINT_EARNED.TOOLTIP.Replace("{Duplicant}", ((MinionIdentity)data).GetProperName());
	}

	public void SetAptitude(HashedString skillGroupID, float amount)
	{
		this.AptitudeBySkillGroup[skillGroupID] = amount;
	}

	public float GetAptitudeExperienceMultiplier(HashedString skillGroupId, float buildingFrequencyMultiplier)
	{
		float num = 0f;
		this.AptitudeBySkillGroup.TryGetValue(skillGroupId, out num);
		return 1f + num * SKILLS.APTITUDE_EXPERIENCE_MULTIPLIER * buildingFrequencyMultiplier;
	}

	public void AddExperience(float amount)
	{
		float num = this.totalExperienceGained;
		float num2 = MinionResume.CalculateNextExperienceBar(this.TotalSkillPointsGained);
		this.totalExperienceGained += amount;
		if (base.isSpawned && this.totalExperienceGained >= num2 && num < num2)
		{
			this.OnSkillPointGained();
		}
	}

	public override void AddExperienceWithAptitude(string skillGroupId, float amount, float buildingMultiplier)
	{
		float num = amount * this.GetAptitudeExperienceMultiplier(skillGroupId, buildingMultiplier) * SKILLS.ACTIVE_EXPERIENCE_PORTION;
		this.DEBUG_ActiveExperienceGained += num;
		this.AddExperience(num);
	}

	public bool HasPerk(HashedString perkId)
	{
		using (List<HashedString>.Enumerator enumerator = this.AdditionalGrantedSkillPerkIDs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == perkId)
				{
					return true;
				}
			}
		}
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value && Db.Get().Skills.Get(keyValuePair.Key).GivesPerk(perkId))
			{
				return true;
			}
		}
		return false;
	}

	public bool HasPerk(SkillPerk perk)
	{
		using (List<HashedString>.Enumerator enumerator = this.AdditionalGrantedSkillPerkIDs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == perk.IdHash)
				{
					return true;
				}
			}
		}
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value && Db.Get().Skills.Get(keyValuePair.Key).GivesPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public void RemoveHat()
	{
		MinionResume.RemoveHat(base.GetComponent<KBatchedAnimController>());
		this.currentHat = null;
		this.targetHat = null;
	}

	public static void RemoveHat(KBatchedAnimController controller)
	{
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		Accessorizer component = controller.GetComponent<Accessorizer>();
		if (component != null)
		{
			Accessory accessory = component.GetAccessory(hat);
			if (accessory != null)
			{
				component.RemoveAccessory(accessory);
			}
		}
		else
		{
			controller.GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride(hat.targetSymbolId, 4);
		}
		controller.SetSymbolVisiblity(hat.targetSymbolId, false);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, false);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
	}

	public static void AddHat(string hat_id, KBatchedAnimController controller)
	{
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		Accessory accessory = hat.Lookup(hat_id);
		if (accessory == null)
		{
			global::Debug.LogWarning("Missing hat: " + hat_id);
		}
		Accessorizer component = controller.GetComponent<Accessorizer>();
		if (component != null)
		{
			Accessory accessory2 = component.GetAccessory(Db.Get().AccessorySlots.Hat);
			if (accessory2 != null)
			{
				component.RemoveAccessory(accessory2);
			}
			if (accessory != null)
			{
				component.AddAccessory(accessory);
			}
		}
		else
		{
			SymbolOverrideController component2 = controller.GetComponent<SymbolOverrideController>();
			component2.TryRemoveSymbolOverride(hat.targetSymbolId, 4);
			component2.AddSymbolOverride(hat.targetSymbolId, accessory.symbol, 4);
		}
		controller.SetSymbolVisiblity(hat.targetSymbolId, true);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
		controller.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, false);
	}

	public void ApplyTargetHat()
	{
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		MinionResume.ApplyHat(this.targetHat, component);
		this.currentHat = this.targetHat;
		this.targetHat = null;
	}

	public static void ApplyHat(string hat_id, KBatchedAnimController controller)
	{
		if (hat_id.IsNullOrWhiteSpace())
		{
			MinionResume.RemoveHat(controller);
			return;
		}
		MinionResume.AddHat(hat_id, controller);
	}

	public string GetSkillsSubtitle()
	{
		return string.Format(DUPLICANTS.NEEDS.QUALITYOFLIFE.TOTAL_SKILL_POINTS, this.TotalSkillPointsGained);
	}

	public static bool AnyMinionHasPerk(string perk, int worldId = -1)
	{
		foreach (MinionResume minionResume in ((worldId >= 0) ? Components.MinionResumes.GetWorldItems(worldId, true) : Components.MinionResumes.Items))
		{
			if (!minionResume.HasTag(GameTags.Dead) && minionResume.HasPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyMinionHasAllPerks(string[] perks, int worldId = -1)
	{
		foreach (MinionResume minionResume in ((worldId >= 0) ? Components.MinionResumes.GetWorldItems(worldId, true) : Components.MinionResumes.Items))
		{
			if (!minionResume.HasTag(GameTags.Dead))
			{
				bool flag = false;
				foreach (string text in perks)
				{
					flag |= minionResume.HasPerk(text);
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool AnyOtherMinionHasPerk(string perk, MinionResume me)
	{
		foreach (MinionResume minionResume in Components.MinionResumes.Items)
		{
			if (!(minionResume == me) && minionResume.HasPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public void ResetSkillLevels(bool returnSkillPoints = true)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (string text in list)
		{
			this.UnmasterSkill(text);
		}
	}

	[MyCmpReq]
	private MinionIdentity identity;

	[Serialize]
	public Dictionary<string, bool> MasteryByRoleID = new Dictionary<string, bool>();

	[Serialize]
	public Dictionary<string, bool> MasteryBySkillID = new Dictionary<string, bool>();

	[Serialize]
	public List<string> GrantedSkillIDs = new List<string>();

	private List<HashedString> AdditionalGrantedSkillPerkIDs = new List<HashedString>();

	private List<MinionResume.HatInfo> AdditionalHats = new List<MinionResume.HatInfo>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();

	[Serialize]
	public Dictionary<HashedString, float> AptitudeBySkillGroup = new Dictionary<HashedString, float>();

	[Serialize]
	private string currentRole = "NoRole";

	[Serialize]
	private string targetRole = "NoRole";

	[Serialize]
	private string currentHat;

	[Serialize]
	private string targetHat;

	private Dictionary<string, bool> ownedHats = new Dictionary<string, bool>();

	[Serialize]
	private float totalExperienceGained;

	private Notification lastSkillNotification;

	private PutOnHatChore lastHatChore;

	private AttributeModifier skillsMoraleExpectationModifier;

	private AttributeModifier skillsMoraleModifier;

	public float DEBUG_PassiveExperienceGained;

	public float DEBUG_ActiveExperienceGained;

	public float DEBUG_SecondsAlive;

	public class HatInfo
	{
		public string Source { get; }

		public string Hat { get; }

		public HatInfo(string source, string hat)
		{
			this.Source = source;
			this.Hat = hat;
			this.count = 1;
		}

		public int count;
	}

	public enum SkillMasteryConditions
	{
		SkillAptitude,
		StressWarning,
		UnableToLearn,
		NeedsSkillPoints,
		MissingPreviousSkill
	}
}
