using System;
using System.Collections.Generic;
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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.MasteryByRoleID == null)
		{
			this.MasteryByRoleID = new Dictionary<string, bool>();
		}
		foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (!this.MasteryByRoleID.ContainsKey(roleConfig.id))
			{
				this.MasteryByRoleID.Add(roleConfig.id, false);
			}
			else if (this.MasteryByRoleID[roleConfig.id])
			{
				if (!this.ExperienceByRoleID.ContainsKey(roleConfig.id))
				{
					this.AddExperience(roleConfig.id, roleConfig.experienceRequired, true);
				}
				else
				{
					this.ExperienceByRoleID[roleConfig.id] = roleConfig.experienceRequired;
				}
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.selectable = base.GetComponent<KSelectable>();
		this.UpdateStatusItem();
		if (this.ExperienceByRoleID == null)
		{
			this.ExperienceByRoleID = new Dictionary<string, float>();
		}
		if (this.AptitudeByRoleGroup == null)
		{
			this.AptitudeByRoleGroup = new Dictionary<HashedString, float>();
		}
		this.ExperienceByRoleID["NoRole"] = 0f;
		foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (!this.ExperienceByRoleID.ContainsKey(roleConfig.id))
			{
				this.AddExperience(roleConfig.id, 0f, true);
			}
			else if (this.ExperienceByRoleID[roleConfig.id] >= roleConfig.experienceRequired)
			{
				this.MasteryByRoleID[roleConfig.id] = true;
			}
			if (!this.AptitudeByRoleGroup.ContainsKey(roleConfig.roleGroup))
			{
				this.AddAptitude(roleConfig.roleGroup, 0f);
			}
		}
		foreach (KeyValuePair<string, bool> keyValuePair in this.MasteryByRoleID)
		{
			if (!(keyValuePair.Key == this.currentRole))
			{
				if (keyValuePair.Value)
				{
					foreach (RolePerk rolePerk in Game.Instance.roleManager.GetRole(keyValuePair.Key).perks)
					{
						if (rolePerk.OnRemove != null)
						{
							rolePerk.OnRemove(this);
						}
						if (rolePerk.OnApply != null)
						{
							rolePerk.OnApply(this);
						}
					}
				}
			}
		}
		this.UpgradeExperienceAndMastery();
		if (!string.IsNullOrEmpty(this.currentRole))
		{
			Game.Instance.roleManager.RestoreRole(this, this.currentRole);
		}
	}

	private void UpdateStatusItem()
	{
		if (string.IsNullOrEmpty(this.currentRole) || this.currentRole == "NoRole")
		{
			this.currentRole = "NoRole";
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Role, Db.Get().DuplicantStatusItems.NoRole, this);
		}
		else
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Role, Db.Get().DuplicantStatusItems.Role, this);
		}
	}

	private void UpgradeExperienceAndMastery()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, float> keyValuePair in this.ExperienceByRoleID)
		{
			if (keyValuePair.Value > 0f)
			{
				foreach (RoleAssignmentRequirement roleAssignmentRequirement in Game.Instance.roleManager.GetRole(keyValuePair.Key).requirements)
				{
					if (roleAssignmentRequirement is PreviousRoleAssignmentRequirement && !list.Contains((roleAssignmentRequirement as PreviousRoleAssignmentRequirement).previousRoleID))
					{
						list.Add((roleAssignmentRequirement as PreviousRoleAssignmentRequirement).previousRoleID);
					}
				}
			}
		}
		foreach (string text in list)
		{
			this.MasteryByRoleID[text] = true;
			this.ExperienceByRoleID[text] = Game.Instance.roleManager.GetRole(text).experienceRequired;
		}
	}

	public void UpdateUrge()
	{
		if (this.targetRole != this.currentRole && this.targetRole != "NoRole")
		{
			if (!base.gameObject.GetComponent<ChoreConsumer>().HasUrge(Db.Get().Urges.SwitchRole))
			{
				base.gameObject.GetComponent<ChoreConsumer>().AddUrge(Db.Get().Urges.SwitchRole);
			}
		}
		else
		{
			base.gameObject.GetComponent<ChoreConsumer>().RemoveUrge(Db.Get().Urges.SwitchRole);
		}
	}

	public string CurrentRole
	{
		get
		{
			return this.currentRole;
		}
	}

	public string TargetRole
	{
		get
		{
			return this.targetRole;
		}
	}

	public bool IsChoreGroupInCurrentRoleGroup(ChoreGroup choregroup)
	{
		if (this.CurrentRole == "NoRole")
		{
			return false;
		}
		RoleConfig role = Game.Instance.roleManager.GetRole(this.currentRole);
		return Game.Instance.roleManager.RoleGroups[role.roleGroup].choreGroupID == choregroup.Id;
	}

	public void SetTargetRole(string newRole)
	{
		this.targetRole = newRole;
		this.UpdateUrge();
	}

	public void AssumeTargetRole()
	{
		this.OnEnterRole(this.targetRole, true);
	}

	public void OnExitRole()
	{
		RoleConfig roleConfig = null;
		if (this.CurrentRole != null && this.CurrentRole != "NoRole")
		{
			roleConfig = Game.Instance.roleManager.GetRole(this.CurrentRole);
			this.currentRole = "NoRole";
		}
		if (roleConfig == null)
		{
			return;
		}
		if (this.ExperienceByRoleID[roleConfig.id] < roleConfig.experienceRequired)
		{
			foreach (RolePerk rolePerk in roleConfig.perks)
			{
				if (rolePerk.OnRemove != null)
				{
					rolePerk.OnRemove(this);
				}
			}
		}
		this.UpdateStatusItem();
		this.UpdateExpectations();
		this.UpdateUrge();
		Game.Instance.Trigger(-1523247426, this);
	}

	public void OnEnterRole(string newRole, bool changeTargetRole = true)
	{
		if (newRole == "JuniorResearcher ")
		{
			newRole = JuniorResearcher.ID;
		}
		RoleManager.ApplyRoleHat(Game.Instance.roleManager.GetRole(this.targetRole), base.GetComponent<Accessorizer>(), base.GetComponent<KBatchedAnimController>());
		if (changeTargetRole)
		{
			this.targetRole = newRole;
		}
		this.currentRole = newRole;
		StatusItem statusItem = ((!(newRole == "NoRole")) ? Db.Get().DuplicantStatusItems.Role : Db.Get().DuplicantStatusItems.NoRole);
		this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Role, statusItem, this);
		RoleConfig role = Game.Instance.roleManager.GetRole(newRole);
		base.GetComponent<ChoreConsumer>().preferredChoreTags = role.preferredChoreTags;
		base.Trigger(540773776, newRole);
		this.AddExperience(newRole, 0f, true);
		foreach (RolePerk rolePerk in role.perks)
		{
			if (rolePerk.OnApply != null)
			{
				rolePerk.OnApply(this);
			}
		}
		this.UpdateExpectations();
		Game.Instance.Trigger(-1523247426, this);
		this.UpdateStatusItem();
		this.UpdateUrge();
		ChoreProvider component = base.GetComponent<ChoreProvider>();
		Chore chore = component.chores.Find((Chore test) => test is TakeOffHatChore);
		if (chore != null)
		{
			chore.Cancel("User Canceled");
		}
	}

	private string GetExperienceString()
	{
		return string.Empty;
	}

	public string GetCurrentRoleString()
	{
		if (this.currentRole != "NoRole")
		{
			return Game.Instance.roleManager.GetRole(this.currentRole).name;
		}
		return DUPLICANTS.ROLES.NO_ROLE.NAME;
	}

	public string GetCurrentRoleDescription()
	{
		if (this.currentRole != "NoRole")
		{
			return Game.Instance.roleManager.GetRole(this.currentRole).description;
		}
		return DUPLICANTS.ROLES.NO_ROLE.DESCRIPTION;
	}

	public void Sim200ms(float dt)
	{
		if (!string.IsNullOrEmpty(this.CurrentRole) && this.CurrentRole != "NoRole")
		{
			this.AddExperience(this.CurrentRole, dt * ROLES.PASSIVE_EXPERIENCE_SCALE, false);
		}
	}

	public void AddExperience(string roleID, float amount, bool respectAptitude = true)
	{
		if (roleID == "NoRole")
		{
			return;
		}
		RoleConfig role = Game.Instance.roleManager.GetRole(roleID);
		float num = 0f;
		this.ExperienceByRoleID.TryGetValue(roleID, out num);
		float num2 = 0f;
		if (role.id != "NoRole" && !this.AptitudeByRoleGroup.TryGetValue(role.roleGroup, out num2))
		{
			this.AptitudeByRoleGroup.Add(role.roleGroup, 0f);
		}
		float num3 = ((!respectAptitude) ? amount : (amount * (1f + num2 * (ROLES.APTITUDE_EXPERIENCE_SCALE / 100f))));
		bool flag = num != role.experienceRequired && num3 > 0f && num + num3 >= role.experienceRequired;
		if (flag)
		{
			this.MasteryByRoleID[role.id] = true;
		}
		num = Mathf.Clamp(num + num3, 0f, role.experienceRequired);
		this.ExperienceByRoleID[roleID] = num;
		if (this.selectable == null)
		{
			this.selectable = base.GetComponent<KSelectable>();
		}
		if (num >= role.experienceRequired)
		{
			if (flag)
			{
				this.OnRoleMastered();
			}
		}
		if (this.currentRole == roleID)
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Role, Db.Get().DuplicantStatusItems.Role, this);
		}
	}

	public void UpdateExpectations()
	{
		int num = this.HighestTierRole();
		foreach (KeyValuePair<string, float> keyValuePair in this.ExperienceByRoleID)
		{
			RoleConfig role = Game.Instance.roleManager.GetRole(keyValuePair.Key);
			if (keyValuePair.Key == this.currentRole || keyValuePair.Value >= Game.Instance.roleManager.GetRole(keyValuePair.Key).experienceRequired)
			{
				num = Math.Max(role.tier, num);
			}
		}
		foreach (Expectation[] array in Expectations.ExpectationsByTier)
		{
			foreach (Expectation expectation in array)
			{
				expectation.OnRemove(this);
			}
		}
		foreach (Expectation expectation2 in Expectations.ExpectationsByTier[num])
		{
			expectation2.OnApply(this);
		}
	}

	public int HighestTierRole()
	{
		int num = 0;
		foreach (KeyValuePair<string, float> keyValuePair in this.ExperienceByRoleID)
		{
			RoleConfig role = Game.Instance.roleManager.GetRole(keyValuePair.Key);
			if (keyValuePair.Key == this.currentRole || keyValuePair.Value >= Game.Instance.roleManager.GetRole(keyValuePair.Key).experienceRequired)
			{
				num = Math.Max(role.tier, num);
			}
		}
		return num;
	}

	private void OnRoleMastered()
	{
		RoleMasteredMessage roleMasteredMessage = new RoleMasteredMessage(this);
		Messenger.Instance.QueueMessage(roleMasteredMessage);
		if (PopFXManager.Instance != null)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, DUPLICANTS.ROLES.ROLE_MASTERED, base.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
		}
		StateMachine.Instance instance = new UpgradeFX.Instance(base.gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0f, 0f, -0.1f));
		instance.StartSM();
	}

	public void AddAptitude(HashedString roleGroupID, float amount)
	{
		if (!this.AptitudeByRoleGroup.ContainsKey(roleGroupID))
		{
			this.AptitudeByRoleGroup.Add(roleGroupID, 0f);
		}
		Dictionary<HashedString, float> aptitudeByRoleGroup;
		(aptitudeByRoleGroup = this.AptitudeByRoleGroup)[roleGroupID] = aptitudeByRoleGroup[roleGroupID] + amount;
	}

	public void AddExperienceIfRole(string roleID, float amount)
	{
		if (this.CurrentRole == roleID)
		{
			this.AddExperience(roleID, amount, true);
		}
	}

	public bool HasPerk(HashedString perk)
	{
		foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (roleConfig.HasPerk(perk) && this.MasteryByRoleID[roleConfig.id])
			{
				return true;
			}
		}
		RoleConfig role = Game.Instance.roleManager.GetRole(this.currentRole);
		return role.HasPerk(perk);
	}

	public bool HasPerk(RolePerk perk)
	{
		foreach (RoleConfig roleConfig in Game.Instance.roleManager.RolesConfigs)
		{
			if (roleConfig.HasPerk(perk) && this.MasteryByRoleID[roleConfig.id])
			{
				return true;
			}
		}
		RoleConfig role = Game.Instance.roleManager.GetRole(this.currentRole);
		return role.HasPerk(perk);
	}

	public bool IsFavouredChore(Chore chore)
	{
		RoleConfig role = Game.Instance.roleManager.GetRole(this.currentRole);
		return role.IsFavoredChore(chore);
	}

	public bool IsPreferredChore(Chore chore)
	{
		RoleConfig role = Game.Instance.roleManager.GetRole(this.currentRole);
		if (chore.choreTags != null)
		{
			foreach (Tag tag in chore.choreTags)
			{
				if (role.preferredChoreTags.Contains(tag))
				{
					return true;
				}
			}
		}
		return false;
	}

	[MyCmpReq]
	private MinionIdentity identity;

	[Serialize]
	public Dictionary<string, float> ExperienceByRoleID;

	[Serialize]
	public Dictionary<string, bool> MasteryByRoleID;

	[Serialize]
	public Dictionary<HashedString, float> AptitudeByRoleGroup = new Dictionary<HashedString, float>();

	[Serialize]
	private string currentRole = "NoRole";

	[Serialize]
	private string targetRole = "NoRole";

	private KSelectable selectable;
}
