using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RoleManager
{
	public RoleManager()
	{
		List<RoleSlotUnlock> list = new List<RoleSlotUnlock>();
		list.Add(new RoleSlotUnlock("Default", "Default", "Default", new List<Tuple<string, int>>
		{
			new Tuple<string, int>("NoRole", 128),
			new Tuple<string, int>(JuniorMiner.ID, 128),
			new Tuple<string, int>(Miner.ID, 128),
			new Tuple<string, int>(SeniorMiner.ID, 128),
			new Tuple<string, int>("JuniorFarmer", 128),
			new Tuple<string, int>("Farmer", 128),
			new Tuple<string, int>("SeniorFarmer", 128),
			new Tuple<string, int>("Rancher", 128),
			new Tuple<string, int>("SeniorRancher", 128),
			new Tuple<string, int>(JuniorResearcher.ID, 128),
			new Tuple<string, int>(Researcher.ID, 128),
			new Tuple<string, int>(SeniorResearcher.ID, 128),
			new Tuple<string, int>("Hauler", 128),
			new Tuple<string, int>(JuniorBuilder.ID, 128),
			new Tuple<string, int>(Builder.ID, 128),
			new Tuple<string, int>(SeniorBuilder.ID, 128),
			new Tuple<string, int>(JuniorCook.ID, 128),
			new Tuple<string, int>(Cook.ID, 128),
			new Tuple<string, int>(MachineTechnician.ID, 128),
			new Tuple<string, int>(JuniorArtist.ID, 128),
			new Tuple<string, int>(Artist.ID, 128),
			new Tuple<string, int>(Handyman.ID, 128),
			new Tuple<string, int>("SuitExpert", 128),
			new Tuple<string, int>("OilTechnician", 128),
			new Tuple<string, int>("PowerTechnician", 128),
			new Tuple<string, int>(MaterialsManager.ID, 128),
			new Tuple<string, int>("MechatronicEngineer", 128),
			new Tuple<string, int>(Plumber.ID, 128)
		}, () => true));
		this.SlotUnlocks = list;
		base..ctor();
		Game.Instance.roleManager = this;
		this.roleAssignmentRequirements = new RoleAssignmentRequirements(this);
		this.InitRoleConfigs();
		this.RestoreSlots();
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			if (component != null)
			{
				this.minionResumes.Add(component);
			}
		}
		Components.LiveMinionIdentities.OnAdd += this.OnIDsChanged;
		Components.LiveMinionIdentities.OnRemove += this.OnIDsChanged;
		foreach (KeyValuePair<string, int> keyValuePair in this.roleRowIndex)
		{
			if (keyValuePair.Value > this.NumberOfRows)
			{
				this.NumberOfRows = keyValuePair.Value;
			}
		}
	}

	public List<RoleConfig> RolesConfigs { get; private set; }

	public string GetHat(string roleID)
	{
		if (RoleManager.roleHatIndex.ContainsKey(roleID))
		{
			return RoleManager.roleHatIndex[roleID];
		}
		return string.Empty;
	}

	public int GetRowIndex(string roleID)
	{
		if (this.roleRowIndex.ContainsKey(roleID))
		{
			return this.roleRowIndex[roleID];
		}
		return -1;
	}

	private void OnIDsChanged(MinionIdentity changedID)
	{
		MinionResume component = changedID.GetComponent<MinionResume>();
		if (component == null)
		{
			foreach (MinionResume minionResume in this.minionResumes)
			{
				if (minionResume.GetComponent<MinionIdentity>() == changedID)
				{
					this.minionResumes.Remove(minionResume);
					return;
				}
			}
			return;
		}
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			if (minionIdentity == changedID)
			{
				if (!this.minionResumes.Contains(component))
				{
					this.minionResumes.Add(component);
				}
				return;
			}
		}
		this.Unassign(component, false);
		this.minionResumes.Remove(component);
	}

	public void RestoreRole(MinionResume resume, string roleID)
	{
		this.AssignToRole(roleID, resume, true, true);
	}

	public int NumberOfSlotsUnlocked(string role_id)
	{
		int num = 0;
		foreach (string text in this.achievedSlotUnlocks)
		{
			foreach (Tuple<string, int> tuple in this.GetSlotUnlock(text).slots)
			{
				if (tuple.first == role_id)
				{
					num += tuple.second;
				}
			}
		}
		return num;
	}

	private void InitRoleConfigs()
	{
		List<List<RoleConfig>> list = new List<List<RoleConfig>>
		{
			new List<RoleConfig>
			{
				new NoRole()
			},
			new List<RoleConfig>
			{
				new Hauler(),
				new JuniorMiner(),
				new JuniorBuilder()
			},
			new List<RoleConfig>
			{
				new JuniorFarmer(),
				new JuniorResearcher(),
				new JuniorCook(),
				new JuniorArtist(),
				new MachineTechnician(),
				new Handyman()
			},
			new List<RoleConfig>
			{
				new Miner(),
				new Builder(),
				new MaterialsManager(),
				new Plumber()
			},
			new List<RoleConfig>
			{
				new Farmer(),
				new Rancher(),
				new Researcher(),
				new Cook(),
				new Artist(),
				new PowerTechnician()
			},
			new List<RoleConfig>
			{
				new MechatronicEngineer(),
				new SeniorMiner(),
				new SeniorBuilder()
			},
			new List<RoleConfig>
			{
				new SeniorResearcher(),
				new SeniorFarmer(),
				new SeniorRancher(),
				new SuitExpert()
			}
		};
		this.RolesConfigs = new List<RoleConfig>();
		for (int i = 0; i < list.Count; i++)
		{
			foreach (RoleConfig roleConfig in list[i])
			{
				roleConfig.SetTier(i);
				this.RolesConfigs.Add(roleConfig);
			}
		}
		foreach (RoleConfig roleConfig2 in this.RolesConfigs)
		{
			this.SlotsByRoleID.Add(roleConfig2.id, 0);
			roleConfig2.InitRequirements();
			if (roleConfig2.id != "NoRole")
			{
				this.RoleGroups[roleConfig2.roleGroup].roles.Add(roleConfig2);
			}
		}
	}

	private void RestoreSlots()
	{
		foreach (RoleSlotUnlock roleSlotUnlock in this.SlotUnlocks)
		{
			if (!this.achievedSlotUnlocks.Contains(roleSlotUnlock.id) && roleSlotUnlock.isSatisfied())
			{
				this.achievedSlotUnlocks.Add(roleSlotUnlock.id);
			}
		}
		using (List<string>.Enumerator enumerator2 = this.achievedSlotUnlocks.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				string id = enumerator2.Current;
				this.UnlockSlots(this.SlotUnlocks.Find((RoleSlotUnlock slot) => slot.id == id));
			}
		}
	}

	public List<MinionResume> GetRoleAssignees(string role_id)
	{
		this.Assignees.Clear();
		foreach (MinionResume minionResume in this.minionResumes)
		{
			if (minionResume.TargetRole == role_id)
			{
				this.Assignees.Add(minionResume);
			}
		}
		return this.Assignees;
	}

	public List<MinionResume> GetRoleAssigneesWithPerk(HashedString perk_id)
	{
		this.Assignees.Clear();
		foreach (MinionResume minionResume in this.minionResumes)
		{
			if (minionResume.HasPerk(perk_id))
			{
				this.Assignees.Add(minionResume);
			}
		}
		return this.Assignees;
	}

	public RoleConfig GetRole(string id)
	{
		if (id == "JuniorResearcher ")
		{
			id = JuniorResearcher.ID;
		}
		foreach (RoleConfig roleConfig in this.RolesConfigs)
		{
			if (roleConfig.id == id)
			{
				return roleConfig;
			}
		}
		if (id != "NoRole")
		{
			global::Debug.LogError("Missing role config: " + id, null);
		}
		return this.noRole;
	}

	public List<RoleConfig> GetRolesWithPerk(HashedString perk_id)
	{
		return this.RolesConfigs.FindAll((RoleConfig r) => r.HasPerk(perk_id));
	}

	private RoleSlotUnlock GetSlotUnlock(string id)
	{
		return this.SlotUnlocks.Find((RoleSlotUnlock r) => r.id == id);
	}

	public void UnlockSlots(RoleSlotUnlock roleSlotUnlock)
	{
		foreach (Tuple<string, int> tuple in roleSlotUnlock.slots)
		{
			if (this.SlotsByRoleID.ContainsKey(tuple.first))
			{
				Dictionary<string, int> slotsByRoleID;
				string first;
				(slotsByRoleID = this.SlotsByRoleID)[first = tuple.first] = slotsByRoleID[first] + tuple.second;
			}
		}
		if (!this.achievedSlotUnlocks.Contains(roleSlotUnlock.id))
		{
			this.achievedSlotUnlocks.Add(roleSlotUnlock.id);
		}
	}

	public string RoleTooltip(string roleID)
	{
		string text = string.Empty;
		RoleConfig role = this.GetRole(roleID);
		text = text + "<b><size=16>" + role.name + "</size></b>";
		text = text + UI.HORIZONTAL_BR_RULE + role.description;
		if (roleID != "NoRole")
		{
			text = text + "\n\n" + this.RolePerkString(roleID);
			text = text + "\n\n" + this.RoleCriteriaString(roleID, null);
		}
		return text;
	}

	public string RolePerkString(string roleID)
	{
		string text = string.Empty;
		RoleConfig role = this.GetRole(roleID);
		if (!(roleID == "NoRole"))
		{
			if (role.perks.Length > 0)
			{
				if (role.tier < 3)
				{
					text = text + "<b>" + UI.ROLES_SCREEN.PERKS.TITLE_BASICTRAINING + "</b>\n";
				}
				else
				{
					text = text + "<b>" + UI.ROLES_SCREEN.PERKS.TITLE_MORETRAINING + "</b>\n";
				}
				for (int i = 0; i < role.perks.Length; i++)
				{
					text = text + "    • " + role.perks[i].description;
					if (i < role.perks.Length - 1)
					{
						text += "\n";
					}
				}
			}
			else if (role.tier < 3)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"<b>",
					UI.ROLES_SCREEN.PERKS.TITLE_BASICTRAINING,
					"</b>\n    • ",
					UI.ROLES_SCREEN.PERKS.NO_PERKS
				});
			}
			else
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"<b>",
					UI.ROLES_SCREEN.PERKS.TITLE_MORETRAINING,
					"</b>\n    • ",
					UI.ROLES_SCREEN.PERKS.NO_PERKS
				});
			}
		}
		return text;
	}

	public string RoleCriteriaString(string roleID, MinionResume resume)
	{
		string text = string.Empty;
		RoleConfig role = this.GetRole(roleID);
		if (resume != null)
		{
			if (resume.CurrentRole == roleID && resume.TargetRole == roleID)
			{
				if (resume.CurrentRole == "NoRole")
				{
					text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ALREADY_IS_JOBLESS, resume.GetProperName());
					text += "\n\n";
				}
				else
				{
					text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ALREADY_IS_ROLE, resume.GetProperName(), role.name);
					text += "\n\n";
				}
			}
			else if (resume.HasMasteredRole(roleID))
			{
				text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.MASTERED, resume.GetProperName(), role.name);
				text += "\n\n";
			}
			else if (resume.CurrentRole == roleID && resume.TargetRole != roleID)
			{
				text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ELIGIBILITY.ELIGIBLE, resume.GetProperName(), role.name);
				text += "\n\n";
			}
			else if (this.CanAssignToRole(roleID, resume))
			{
				text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ELIGIBILITY.ELIGIBLE, resume.GetProperName(), role.name);
				if (resume.CurrentRole != "NoRole")
				{
					text = text + "\n" + string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.WILL_BE_UNASSIGNED, resume.GetProperName(), role.name, this.GetRole(resume.CurrentRole).name);
				}
				text += "\n\n";
			}
			else
			{
				text += string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.ELIGIBILITY.INELIGIBLE, resume.GetProperName(), role.name);
				text += "\n\n";
			}
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(resume);
			int num = role.QOLExpectation();
			if (role.tier > resume.HighestTierRoleMastered() && (float)num > attributeInstance.GetTotalValue())
			{
				text = text + UIConstants.ColorPrefixRed + string.Format(UI.ROLES_SCREEN.EXPECTATION_ALERT_TARGET_JOB, new object[]
				{
					attributeInstance.GetTotalValue(),
					num,
					resume.GetProperName(),
					role.name
				}) + UIConstants.ColorSuffix;
				text = text + "\n" + UI.ROLES_SCREEN.EXPECTATION_ALERT_DESC_TARGET_JOB;
				text += "\n\n";
			}
			text += UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.RELEVANT_APTITUDES;
			bool flag = false;
			foreach (KeyValuePair<HashedString, float> keyValuePair in resume.AptitudeByRoleGroup)
			{
				float value = keyValuePair.Value;
				if (value != 0f && role.roleGroup == keyValuePair.Key)
				{
					flag = true;
					text += "\n";
					string text2 = ((value <= 0f) ? UIConstants.ColorPrefixRed : UIConstants.ColorPrefixGreen);
					string text3 = text;
					text = string.Concat(new object[]
					{
						text3,
						"    • ",
						Game.Instance.roleManager.RoleGroups[role.roleGroup].Name,
						": ",
						text2,
						"<b>",
						value,
						"</b>",
						UIConstants.ColorSuffix
					});
				}
			}
			if (!flag)
			{
				text = text + "\n    • <color=#F44A47FF>" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.NO_APTITUDE + "</color>";
			}
			text += "\n\n";
			text += UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.RELEVANT_ATTRIBUTES;
			foreach (Klei.AI.Attribute attribute in role.relevantAttributes)
			{
				string text4 = "<color=#FFFFFFFF>";
				text += "\n";
				float totalDisplayValue = resume.GetAttributes().Get(attribute).GetTotalDisplayValue();
				if (totalDisplayValue != 0f)
				{
					text4 = ((totalDisplayValue <= 0f) ? "<color=#F44A47FF>" : "<color=#BF5389FF>");
				}
				string text3 = text;
				text = string.Concat(new object[]
				{
					text3,
					"    • ",
					attribute.Name,
					": ",
					text4,
					"<b>",
					resume.GetAttributes().Get(attribute).GetTotalDisplayValue(),
					"</b></color>"
				});
			}
		}
		if (resume != null)
		{
			text += "\n\n";
		}
		if (!(roleID == "NoRole"))
		{
			if (role.requirements.Length > 0)
			{
				text = text + "<b>" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.TITLE + "</b>\n";
				if (resume != null)
				{
				}
				for (int j = 0; j < role.requirements.Length; j++)
				{
					text += ((!(resume == null) && !role.requirements[j].isSatisfied(resume)) ? "<color=#F44A47FF>" : "<color=#FFFFFF>");
					text = text + "    • " + string.Format("{0}", role.requirements[j].GetDescription());
					text += "</color>";
					if (j != role.requirements.Length - 1)
					{
						text += "\n";
					}
				}
			}
			else
			{
				text = text + "<b>" + UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.TITLE + "</b>\n";
				text = text + "    • " + string.Format(UI.ROLES_SCREEN.ASSIGNMENT_REQUIREMENTS.NONE, role.name);
			}
		}
		return text;
	}

	public bool CanAssignToRole(string roleID, MinionResume resume)
	{
		RoleConfig role = this.GetRole(roleID);
		if (resume.TargetRole == roleID)
		{
			return false;
		}
		if (resume.CurrentRole == roleID && resume.TargetRole == roleID)
		{
			return false;
		}
		if (this.GetRoleAssignees(roleID).Count >= this.SlotsByRoleID[roleID])
		{
			return false;
		}
		if (DebugHandler.InstantBuildMode)
		{
			return true;
		}
		foreach (RoleAssignmentRequirement roleAssignmentRequirement in role.requirements)
		{
			if (!roleAssignmentRequirement.isSatisfied(resume))
			{
				return false;
			}
		}
		return true;
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

	public static void AddHat(string hat_idx, KBatchedAnimController controller)
	{
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		Accessory accessory = hat.Lookup(hat_idx);
		Accessorizer component = controller.GetComponent<Accessorizer>();
		if (component != null)
		{
			Accessory accessory2 = component.GetAccessory(Db.Get().AccessorySlots.Hat);
			if (accessory2 != null)
			{
				component.RemoveAccessory(accessory2);
			}
			component.AddAccessory(accessory);
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

	public static void ApplyRoleHat(RoleConfig role, Accessorizer accessorizer, KBatchedAnimController controller)
	{
		if (role == null || string.IsNullOrEmpty(role.hat))
		{
			RoleManager.RemoveHat(controller);
		}
		else
		{
			RoleManager.AddHat(role.hat, controller);
		}
	}

	public IApproachable ClosestJobStation(GameObject from_go)
	{
		Navigator component = from_go.GetComponent<Navigator>();
		RoleStation roleStation = null;
		float num = float.PositiveInfinity;
		foreach (RoleStation roleStation2 in Components.RoleStations.Items)
		{
			float num2 = (float)component.GetNavigationCost(roleStation2);
			if (num2 < num)
			{
				num = num2;
				roleStation = roleStation2;
			}
		}
		return roleStation;
	}

	public void AssignToRole(string roleID, MinionResume resume, bool instant = false, bool restoring = false)
	{
		RoleConfig role = this.GetRole(roleID);
		if (!restoring)
		{
			this.Unassign(resume, true);
		}
		if (!instant && resume.CurrentRole != roleID)
		{
			if (resume.GetComponent<ChoreProvider>().chores.Find((Chore chore) => chore.choreType == Db.Get().ChoreTypes.SwitchRole) == null && !DebugHandler.InstantBuildMode)
			{
				resume.SetTargetRole(roleID);
				goto IL_0093;
			}
		}
		resume.OnEnterRole(roleID, !instant);
		RoleManager.ApplyRoleHat(role, resume.GetComponent<Accessorizer>(), resume.GetComponent<KBatchedAnimController>());
		IL_0093:
		if (!restoring)
		{
			this.AutoAssignPersonalPriorities(roleID, resume.gameObject);
		}
		if (JobsTableScreen.Instance != null)
		{
			JobsTableScreen.Instance.Refresh(resume);
		}
	}

	public void Unassign(MinionResume resume, bool skip_refresh = false)
	{
		if (resume == null)
		{
			return;
		}
		ChoreConsumer component = resume.GetComponent<ChoreConsumer>();
		if (component != null)
		{
			string text = ((!(resume.TargetRole != resume.CurrentRole)) ? resume.CurrentRole : resume.TargetRole);
			RoleConfig role = Game.Instance.roleManager.GetRole(text);
			RoleGroup roleGroup;
			if (role != null && Game.Instance.roleManager.RoleGroups.TryGetValue(role.roleGroup, out roleGroup))
			{
				foreach (ChoreGroup choreGroup in Db.Get().ChoreGroups.resources)
				{
					if (roleGroup.choreGroupID == choreGroup.Id)
					{
						bool flag;
						component.GetPersonalPriority(choreGroup, out flag);
						if (flag)
						{
							int priorityBeforeAutoAssignment = component.GetPriorityBeforeAutoAssignment(choreGroup);
							component.SetPersonalPriority(choreGroup, priorityBeforeAutoAssignment, false);
						}
					}
				}
			}
		}
		Worker component2 = resume.GetComponent<Worker>();
		resume.GetComponent<ChoreConsumer>();
		new TakeOffHatChore(component2, Db.Get().ChoreTypes.SwitchHat);
		if (!skip_refresh && JobsTableScreen.Instance != null)
		{
			JobsTableScreen.Instance.Refresh(resume);
		}
	}

	public void ResetPersonalPriorities(MinionIdentity minion)
	{
		MinionResume component = minion.GetComponent<MinionResume>();
		string text = ((!(component.TargetRole != component.CurrentRole)) ? component.CurrentRole : component.TargetRole);
		this.AutoAssignPersonalPriorities(text, minion.gameObject);
	}

	private void AutoAssignPersonalPriorities(string roleID, GameObject minion)
	{
		if (Game.Instance.autoPrioritizeRoles)
		{
			ChoreConsumer component = minion.GetComponent<ChoreConsumer>();
			if (component != null)
			{
				RoleConfig role = Game.Instance.roleManager.GetRole(roleID);
				RoleGroup roleGroup;
				if (role != null && Game.Instance.roleManager.RoleGroups.TryGetValue(role.roleGroup, out roleGroup))
				{
					foreach (ChoreGroup choreGroup in Db.Get().ChoreGroups.resources)
					{
						if (roleGroup.choreGroupID == choreGroup.Id)
						{
							component.SetPersonalPriority(choreGroup, 5, true);
						}
					}
				}
			}
		}
	}

	private RoleConfig noRole = new NoRole();

	private Dictionary<string, int> SlotsByRoleID = new Dictionary<string, int>();

	[Serialize]
	private List<string> achievedSlotUnlocks = new List<string>();

	private List<MinionResume> minionResumes = new List<MinionResume>();

	public RoleAssignmentRequirements roleAssignmentRequirements;

	public static readonly RolePerks rolePerks = new RolePerks();

	public Dictionary<HashedString, RoleGroup> RoleGroups = new Dictionary<HashedString, RoleGroup>
	{
		{
			"Farming",
			new RoleGroup("Farming", "Farming", DUPLICANTS.CHOREGROUPS.FARMING.NAME)
		},
		{
			"Ranching",
			new RoleGroup("Ranching", "Ranching", DUPLICANTS.CHOREGROUPS.RANCHING.NAME)
		},
		{
			"Mining",
			new RoleGroup("Mining", "Dig", DUPLICANTS.CHOREGROUPS.DIG.NAME)
		},
		{
			"Cooking",
			new RoleGroup("Cooking", "Cook", DUPLICANTS.CHOREGROUPS.COOK.NAME)
		},
		{
			"Art",
			new RoleGroup("Art", "Art", DUPLICANTS.CHOREGROUPS.ART.NAME)
		},
		{
			"Building",
			new RoleGroup("Building", "Build", DUPLICANTS.CHOREGROUPS.BUILD.NAME)
		},
		{
			"Management",
			new RoleGroup("Management", string.Empty, string.Empty)
		},
		{
			"Research",
			new RoleGroup("Research", "Research", DUPLICANTS.CHOREGROUPS.RESEARCH.NAME)
		},
		{
			"Suits",
			new RoleGroup("Suits", string.Empty, string.Empty)
		},
		{
			"Hauling",
			new RoleGroup("Hauling", "Hauling", DUPLICANTS.CHOREGROUPS.HAULING.NAME)
		},
		{
			"Technicals",
			new RoleGroup("Technicals", "MachineOperating", DUPLICANTS.CHOREGROUPS.MACHINEOPERATING.NAME)
		},
		{
			"MedicalAid",
			new RoleGroup("Doctor", "MedicalAid", DUPLICANTS.CHOREGROUPS.MEDICALAID.NAME)
		},
		{
			"Basekeeping",
			new RoleGroup("Basekeeping", "Basekeeping", DUPLICANTS.CHOREGROUPS.BASEKEEPING.NAME)
		}
	};

	public static Dictionary<string, string> roleHatIndex = new Dictionary<string, string>
	{
		{ "JuniorFarmer", "hat_role_farming1" },
		{ "Farmer", "hat_role_farming2" },
		{ "SeniorFarmer", "hat_role_farming3" },
		{ "Rancher", "hat_role_rancher1" },
		{ "SeniorRancher", "hat_role_rancher2" },
		{
			JuniorResearcher.ID,
			"hat_role_research1"
		},
		{
			Researcher.ID,
			"hat_role_research2"
		},
		{
			SeniorResearcher.ID,
			"hat_role_research3"
		},
		{
			JuniorMiner.ID,
			"hat_role_mining1"
		},
		{
			Miner.ID,
			"hat_role_mining2"
		},
		{
			SeniorMiner.ID,
			"hat_role_mining3"
		},
		{
			JuniorCook.ID,
			"hat_role_cooking1"
		},
		{
			Cook.ID,
			"hat_role_cooking2"
		},
		{
			JuniorArtist.ID,
			"hat_role_art1"
		},
		{
			Artist.ID,
			"hat_role_art2"
		},
		{ "Hauler", "hat_role_hauling1" },
		{
			MaterialsManager.ID,
			"hat_role_hauling2"
		},
		{ "SuitExpert", "hat_role_suits1" },
		{
			MachineTechnician.ID,
			"hat_role_technicals1"
		},
		{ "PowerTechnician", "hat_role_technicals2" },
		{ "MechatronicEngineer", "hat_role_engineering1" },
		{
			JuniorBuilder.ID,
			"hat_role_building1"
		},
		{
			Builder.ID,
			"hat_role_building2"
		},
		{
			SeniorBuilder.ID,
			"hat_role_building3"
		},
		{
			Handyman.ID,
			"hat_role_basekeeping1"
		},
		{
			Plumber.ID,
			"hat_role_basekeeping1"
		}
	};

	private Dictionary<string, int> roleRowIndex = new Dictionary<string, int>
	{
		{ "NoRole", 0 },
		{
			JuniorMiner.ID,
			1
		},
		{
			Miner.ID,
			1
		},
		{
			SeniorMiner.ID,
			1
		},
		{
			JuniorBuilder.ID,
			2
		},
		{
			Builder.ID,
			2
		},
		{
			SeniorBuilder.ID,
			2
		},
		{ "Hauler", 3 },
		{
			MaterialsManager.ID,
			3
		},
		{ "SuitExpert", 3 },
		{ "MechatronicEngineer", 4 },
		{
			MachineTechnician.ID,
			5
		},
		{ "PowerTechnician", 5 },
		{ "JuniorFarmer", 6 },
		{ "Farmer", 6 },
		{ "SeniorFarmer", 6 },
		{ "Rancher", 7 },
		{ "SeniorRancher", 7 },
		{
			JuniorResearcher.ID,
			8
		},
		{
			Researcher.ID,
			8
		},
		{
			SeniorResearcher.ID,
			8
		},
		{
			Handyman.ID,
			9
		},
		{
			Plumber.ID,
			9
		},
		{
			JuniorCook.ID,
			10
		},
		{
			Cook.ID,
			10
		},
		{
			JuniorArtist.ID,
			11
		},
		{
			Artist.ID,
			11
		}
	};

	public int NumberOfRows;

	private List<MinionResume> Assignees = new List<MinionResume>();

	private const int DEFAULT_MAX_SLOTS = 128;

	public List<RoleSlotUnlock> SlotUnlocks;
}
