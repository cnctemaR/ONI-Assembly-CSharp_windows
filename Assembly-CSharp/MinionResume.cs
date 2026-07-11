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
public class MinionResume : KMonoBehaviour, ISaveLoadable, ISim200ms
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
			float num = this.TotalExperienceGained / (float)SKILLS.TARGET_SKILLS_CYCLE / 600f;
			float num2 = Mathf.Pow(num, 1f / SKILLS.EXPERIENCE_LEVEL_POWER);
			return Mathf.FloorToInt(num2 * (float)SKILLS.TARGET_SKILLS_EARNED);
		}
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
			return this.TotalSkillPointsGained - this.SkillsMastered;
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
		this.selectable = base.GetComponent<KSelectable>();
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value)
			{
				Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
				foreach (SkillPerk skillPerk in skill.perks)
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
				if (!this.ownedHats.ContainsKey(skill.hat))
				{
					this.ownedHats.Add(skill.hat, true);
				}
			}
		}
		this.UpdateExpectations();
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		MinionResume.ApplyHat(this.currentHat, component);
	}

	public void RestoreResume(Dictionary<string, bool> MasteryBySkillID, Dictionary<HashedString, float> AptitudeBySkillGroup, float totalExperienceGained)
	{
		this.MasteryBySkillID = MasteryBySkillID;
		this.AptitudeBySkillGroup = AptitudeBySkillGroup;
		this.totalExperienceGained = totalExperienceGained;
	}

	protected override void OnCleanUp()
	{
		Components.MinionResumes.Remove(this);
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

	private void ApplySkillPerks(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		foreach (SkillPerk skillPerk in skill.perks)
		{
			if (skillPerk.OnApply != null)
			{
				skillPerk.OnApply(this);
			}
		}
	}

	private void RemoveSkillPerks(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		foreach (SkillPerk skillPerk in skill.perks)
		{
			if (skillPerk.OnRemove != null)
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

	public bool CheckSkillTraitDisabled(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		string choreGroupID = Db.Get().SkillGroups.Get(skill.skillGroup).choreGroupID;
		if (!string.IsNullOrEmpty(choreGroupID))
		{
			Traits component = base.GetComponent<Traits>();
			foreach (Trait trait in component.TraitList)
			{
				if (trait.disabledChoreGroups != null)
				{
					foreach (ChoreGroup choreGroup in trait.disabledChoreGroups)
					{
						if (choreGroup.Id == choreGroupID)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		return false;
	}

	public bool CanMasterSkill(string skillId)
	{
		Skill skill = Db.Get().Skills.Get(skillId);
		if (this.CheckSkillTraitDisabled(skillId))
		{
			return false;
		}
		if (this.AvailableSkillpoints < 1)
		{
			return false;
		}
		for (int i = 0; i < skill.priorSkills.Count; i++)
		{
			if (!this.HasMasteredSkill(skill.priorSkills[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool OwnsHat(string hatId)
	{
		return this.ownedHats.ContainsKey(hatId) && this.ownedHats[hatId];
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
			new PutOnHatChore(this, Db.Get().ChoreTypes.SwitchHat);
		}
	}

	public void MasterSkill(string skillId)
	{
		if (!base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.LearnSkill))
		{
			base.gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.LearnSkill);
		}
		this.MasteryBySkillID[skillId] = true;
		this.ApplySkillPerks(skillId);
		this.UpdateExpectations();
		this.TriggerMasterSkillEvents();
		if (!this.ownedHats.ContainsKey(Db.Get().Skills.Get(skillId).hat))
		{
			this.ownedHats.Add(Db.Get().Skills.Get(skillId).hat, false);
		}
	}

	public void UnmasterSkill(string skillId)
	{
		if (this.MasteryBySkillID.ContainsKey(skillId))
		{
			this.MasteryBySkillID.Remove(skillId);
			this.RemoveSkillPerks(skillId);
			this.UpdateExpectations();
			this.TriggerMasterSkillEvents();
		}
	}

	private void TriggerMasterSkillEvents()
	{
		base.Trigger(540773776, null);
		Game.Instance.Trigger(-1523247426, this);
	}

	public void ForceAddSkillPoint()
	{
		this.AddExperience(this.CalculateNextExperienceBar() - this.totalExperienceGained);
	}

	public float CalculateNextExperienceBar()
	{
		float num = (float)(this.TotalSkillPointsGained + 1) / (float)SKILLS.TARGET_SKILLS_EARNED;
		float num2 = Mathf.Pow(num, SKILLS.EXPERIENCE_LEVEL_POWER);
		return num2 * (float)SKILLS.TARGET_SKILLS_CYCLE * 600f;
	}

	public float CalculatePreviousExperienceBar()
	{
		float num = (float)this.TotalSkillPointsGained / (float)SKILLS.TARGET_SKILLS_EARNED;
		float num2 = Mathf.Pow(num, SKILLS.EXPERIENCE_LEVEL_POWER);
		return num2 * (float)SKILLS.TARGET_SKILLS_CYCLE * 600f;
	}

	private void UpdateExpectations()
	{
		int num = 0;
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryBySkillID)
		{
			if (keyValuePair.Value)
			{
				Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
				num += skill.tier + 1;
				float num2 = 0f;
				if (this.AptitudeBySkillGroup.TryGetValue(new HashedString(skill.skillGroup), out num2))
				{
					num -= (int)num2;
				}
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

	private void OnSkillPointGained()
	{
		Game.Instance.Trigger(1505456302, this);
		SkillMasteredMessage skillMasteredMessage = new SkillMasteredMessage(this);
		MusicManager.instance.PlaySong("Stinger_JobMastered", false);
		Messenger.Instance.QueueMessage(skillMasteredMessage);
		if (PopFXManager.Instance != null)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, MISC.NOTIFICATIONS.SKILL_POINT_EARNED.NAME, base.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
		}
		StateMachine.Instance instance = new UpgradeFX.Instance(base.gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f));
		instance.StartSM();
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
		float num2 = this.CalculateNextExperienceBar();
		this.totalExperienceGained += amount;
		if (this.totalExperienceGained >= num2 && num < num2)
		{
			this.OnSkillPointGained();
		}
	}

	public void AddExperienceWithAptitude(string skillGroupId, float amount, float buildingMultiplier)
	{
		float num = amount * this.GetAptitudeExperienceMultiplier(skillGroupId, buildingMultiplier) * SKILLS.ACTIVE_EXPERIENCE_PORTION;
		this.DEBUG_ActiveExperienceGained += num;
		this.AddExperience(num);
	}

	public bool HasPerk(HashedString perkId)
	{
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
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		MinionResume.RemoveHat(component);
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
		}
		else
		{
			MinionResume.AddHat(hat_id, controller);
		}
	}

	public string GetSkillsSubtitle()
	{
		return "Total Skill Points: " + this.TotalSkillPointsGained;
	}

	public static bool AnyMinionHasPerk(string perk)
	{
		List<MinionResume> list = new List<MinionResume>();
		foreach (MinionResume minionResume in Components.MinionResumes.Items)
		{
			if (minionResume.HasPerk(perk))
			{
				return true;
			}
		}
		return false;
	}

	public static bool AnyOtherMinionHasPerk(string perk, MinionResume me)
	{
		List<MinionResume> list = new List<MinionResume>();
		foreach (MinionResume minionResume in Components.MinionResumes.Items)
		{
			if (!(minionResume == me))
			{
				if (minionResume.HasPerk(perk))
				{
					return true;
				}
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

	private KSelectable selectable;

	private AttributeModifier skillsMoraleExpectationModifier;

	public float DEBUG_PassiveExperienceGained;

	public float DEBUG_ActiveExperienceGained;

	public float DEBUG_SecondsAlive;
}
