using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SkillsScreen : KModalScreen
{
	public IAssignableIdentity CurrentlySelectedMinion
	{
		get
		{
			if (this.currentlySelectedMinion == null || this.currentlySelectedMinion.IsNull())
			{
				return null;
			}
			return this.currentlySelectedMinion;
		}
		set
		{
			this.currentlySelectedMinion = value;
			this.RefreshSelectedMinion();
		}
	}

	protected override void OnActivate()
	{
		this.ConsumeMouseScroll = true;
		base.OnActivate();
		this.BuildMinions();
		this.RefreshAll();
		Components.LiveMinionIdentities.OnAdd += this.OnAddMinionIdentity;
		Components.LiveMinionIdentities.OnRemove += this.OnRemoveMinionIdentity;
		this.CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		MultiToggle multiToggle = this.dupeSortingToggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.SortByMinon();
		}));
		MultiToggle multiToggle2 = this.moraleSortingToggle;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			this.SortByMorale();
		}));
		MultiToggle multiToggle3 = this.experienceSortingToggle;
		multiToggle3.onClick = (global::System.Action)Delegate.Combine(multiToggle3.onClick, new global::System.Action(delegate
		{
			this.SortByExperience();
		}));
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			if (this.CurrentlySelectedMinion == null && Components.LiveMinionIdentities.Count > 0)
			{
				this.CurrentlySelectedMinion = Components.LiveMinionIdentities.Items[0];
			}
			this.RefreshAll();
		}
		base.OnShow(show);
	}

	private void RefreshAll()
	{
		this.dirty = false;
		this.RefreshSkillWidgets();
		this.RefreshSelectedMinion();
		this.linesPending = true;
	}

	private void RefreshSelectedMinion()
	{
		this.SetPortraitAnimator(this.currentlySelectedMinion);
		this.RefreshProgressBars();
		this.RefreshHat();
	}

	private void RefreshProgressBars()
	{
		if (this.currentlySelectedMinion == null || this.currentlySelectedMinion.IsNull())
		{
			return;
		}
		MinionIdentity minionIdentity = this.currentlySelectedMinion as MinionIdentity;
		HierarchyReferences component = this.expectationsTooltip.GetComponent<HierarchyReferences>();
		component.GetReference("Labels").gameObject.SetActive(minionIdentity != null);
		component.GetReference("MoraleBar").gameObject.SetActive(minionIdentity != null);
		component.GetReference("ExpectationBar").gameObject.SetActive(minionIdentity != null);
		component.GetReference("StoredMinion").gameObject.SetActive(minionIdentity == null);
		this.experienceProgressFill.gameObject.SetActive(minionIdentity != null);
		if (minionIdentity == null)
		{
			this.expectationsTooltip.SetSimpleTooltip(string.Format(UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (this.currentlySelectedMinion as StoredMinionIdentity).GetStorageReason(), this.currentlySelectedMinion.GetProperName()));
			this.experienceBarTooltip.SetSimpleTooltip(string.Format(UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (this.currentlySelectedMinion as StoredMinionIdentity).GetStorageReason(), this.currentlySelectedMinion.GetProperName()));
			this.EXPCount.text = string.Empty;
			this.duplicantLevelIndicator.text = UI.TABLESCREENS.NA;
		}
		else
		{
			MinionResume component2 = minionIdentity.GetComponent<MinionResume>();
			float num = MinionResume.CalculatePreviousExperienceBar(component2.TotalSkillPointsGained);
			float num2 = MinionResume.CalculateNextExperienceBar(component2.TotalSkillPointsGained);
			float num3 = (component2.TotalExperienceGained - num) / (num2 - num);
			this.EXPCount.text = Mathf.RoundToInt(component2.TotalExperienceGained - num) + " / " + Mathf.RoundToInt(num2 - num);
			this.duplicantLevelIndicator.text = (component2.TotalSkillPointsGained - component2.SkillsMastered).ToString();
			this.experienceProgressFill.fillAmount = num3;
			this.experienceBarTooltip.SetSimpleTooltip(string.Format(UI.SKILLS_SCREEN.EXPERIENCE_TOOLTIP, Mathf.RoundToInt(num2 - num) - Mathf.RoundToInt(component2.TotalExperienceGained - num)));
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(component2);
			AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(component2);
			float num4 = 0f;
			if (!string.IsNullOrEmpty(this.hoveredSkillID) && !component2.HasMasteredSkill(this.hoveredSkillID))
			{
				List<string> list = new List<string>();
				List<string> list2 = new List<string>();
				list.Add(this.hoveredSkillID);
				while (list.Count > 0)
				{
					for (int i = list.Count - 1; i >= 0; i--)
					{
						if (!component2.HasMasteredSkill(list[i]))
						{
							num4 += (float)(Db.Get().Skills.Get(list[i]).tier + 1);
							if (component2.AptitudeBySkillGroup.ContainsKey(Db.Get().Skills.Get(list[i]).skillGroup) && component2.AptitudeBySkillGroup[Db.Get().Skills.Get(list[i]).skillGroup] > 0f)
							{
								num4 -= 1f;
							}
							foreach (string text in Db.Get().Skills.Get(list[i]).priorSkills)
							{
								list2.Add(text);
							}
						}
					}
					list.Clear();
					list.AddRange(list2);
					list2.Clear();
				}
			}
			float num5 = attributeInstance.GetTotalValue() / (attributeInstance2.GetTotalValue() + num4);
			float num6 = Mathf.Max(attributeInstance.GetTotalValue(), attributeInstance2.GetTotalValue() + num4);
			while (this.moraleNotches.Count < Mathf.RoundToInt(num6))
			{
				GameObject gameObject = global::UnityEngine.Object.Instantiate<GameObject>(this.moraleNotch, this.moraleNotch.transform.parent);
				gameObject.SetActive(true);
				this.moraleNotches.Add(gameObject);
			}
			while (this.moraleNotches.Count > Mathf.RoundToInt(num6))
			{
				GameObject gameObject2 = this.moraleNotches[this.moraleNotches.Count - 1];
				this.moraleNotches.Remove(gameObject2);
				global::UnityEngine.Object.Destroy(gameObject2);
			}
			for (int j = 0; j < this.moraleNotches.Count; j++)
			{
				if ((float)j < attributeInstance.GetTotalValue())
				{
					this.moraleNotches[j].GetComponentsInChildren<Image>()[1].color = this.moraleNotchColor;
				}
				else
				{
					this.moraleNotches[j].GetComponentsInChildren<Image>()[1].color = Color.clear;
				}
			}
			this.moraleProgressLabel.text = UI.SKILLS_SCREEN.MORALE + ": " + attributeInstance.GetTotalValue().ToString();
			while (this.expectationNotches.Count < Mathf.RoundToInt(num6))
			{
				GameObject gameObject3 = global::UnityEngine.Object.Instantiate<GameObject>(this.expectationNotch, this.expectationNotch.transform.parent);
				gameObject3.SetActive(true);
				this.expectationNotches.Add(gameObject3);
			}
			while (this.expectationNotches.Count > Mathf.RoundToInt(num6))
			{
				GameObject gameObject4 = this.expectationNotches[this.expectationNotches.Count - 1];
				this.expectationNotches.Remove(gameObject4);
				global::UnityEngine.Object.Destroy(gameObject4);
			}
			for (int k = 0; k < this.expectationNotches.Count; k++)
			{
				if ((float)k < attributeInstance2.GetTotalValue() + num4)
				{
					if ((float)k < attributeInstance2.GetTotalValue())
					{
						this.expectationNotches[k].GetComponentsInChildren<Image>()[1].color = this.expectationNotchColor;
					}
					else
					{
						this.expectationNotches[k].GetComponentsInChildren<Image>()[1].color = this.expectationNotchProspectColor;
					}
				}
				else
				{
					this.expectationNotches[k].GetComponentsInChildren<Image>()[1].color = Color.clear;
				}
			}
			this.expectationsProgressLabel.text = UI.SKILLS_SCREEN.MORALE_EXPECTATION + ": " + attributeInstance2.GetTotalValue().ToString();
			if (num4 > 0f)
			{
				LocText locText = this.expectationsProgressLabel;
				locText.text = locText.text + " + " + GameUtil.ApplyBoldString(GameUtil.ColourizeString(new Color(1f, 0.5f, 0.5f, 1f), num4.ToString()));
			}
			if (num5 < 1f)
			{
				this.expectationWarning.SetActive(true);
				this.moraleWarning.SetActive(false);
			}
			else
			{
				this.expectationWarning.SetActive(false);
				this.moraleWarning.SetActive(true);
			}
			string text2 = string.Empty;
			string text3 = text2;
			text2 = string.Concat(new object[]
			{
				text3,
				GameUtil.ApplyBoldString(UI.SKILLS_SCREEN.MORALE),
				": ",
				attributeInstance.GetTotalValue(),
				"\n"
			});
			for (int l = 0; l < attributeInstance.Modifiers.Count; l++)
			{
				text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"    • ",
					attributeInstance.Modifiers[l].GetDescription(),
					": ",
					(attributeInstance.Modifiers[l].Value <= 0f) ? UIConstants.ColorPrefixRed : UIConstants.ColorPrefixGreen,
					attributeInstance.Modifiers[l].GetFormattedString(component2.gameObject),
					UIConstants.ColorSuffix,
					"\n"
				});
			}
			text2 += "\n";
			text3 = text2;
			text2 = string.Concat(new object[]
			{
				text3,
				GameUtil.ApplyBoldString(UI.SKILLS_SCREEN.MORALE_EXPECTATION),
				": ",
				attributeInstance2.GetTotalValue(),
				"\n"
			});
			for (int m = 0; m < attributeInstance2.Modifiers.Count; m++)
			{
				text3 = text2;
				text2 = string.Concat(new string[]
				{
					text3,
					"    • ",
					attributeInstance2.Modifiers[m].GetDescription(),
					": ",
					(attributeInstance2.Modifiers[m].Value <= 0f) ? UIConstants.ColorPrefixGreen : UIConstants.ColorPrefixRed,
					attributeInstance2.Modifiers[m].GetFormattedString(component2.gameObject),
					UIConstants.ColorSuffix,
					"\n"
				});
			}
			this.expectationsTooltip.SetSimpleTooltip(text2);
		}
	}

	private void RefreshHat()
	{
		if (this.currentlySelectedMinion == null || this.currentlySelectedMinion.IsNull())
		{
			return;
		}
		List<IListableOption> list = new List<IListableOption>();
		string text = string.Empty;
		MinionIdentity minionIdentity = this.currentlySelectedMinion as MinionIdentity;
		if (minionIdentity != null)
		{
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			text = ((!string.IsNullOrEmpty(component.TargetHat)) ? component.TargetHat : component.CurrentHat);
			foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
			{
				if (keyValuePair.Value)
				{
					list.Add(new SkillListable(keyValuePair.Key));
				}
			}
			this.hatDropDown.Initialize(list, new Action<IListableOption, object>(this.OnHatDropEntryClick), new Func<IListableOption, IListableOption, object, int>(this.hatDropDownSort), new Action<DropDownEntry, object>(this.hatDropEntryRefreshAction), false, this.currentlySelectedMinion);
		}
		else
		{
			StoredMinionIdentity storedMinionIdentity = this.currentlySelectedMinion as StoredMinionIdentity;
			text = ((!string.IsNullOrEmpty(storedMinionIdentity.targetHat)) ? storedMinionIdentity.targetHat : storedMinionIdentity.currentHat);
		}
		this.hatDropDown.openButton.enabled = minionIdentity != null;
		this.selectedHat.transform.Find("Arrow").gameObject.SetActive(minionIdentity != null);
		this.selectedHat.sprite = Assets.GetSprite((!string.IsNullOrEmpty(text)) ? text : "hat_role_none");
	}

	private void OnHatDropEntryClick(IListableOption skill, object data)
	{
		MinionIdentity minionIdentity = this.currentlySelectedMinion as MinionIdentity;
		if (minionIdentity == null)
		{
			return;
		}
		MinionResume component = minionIdentity.GetComponent<MinionResume>();
		string text = "hat_role_none";
		if (skill != null)
		{
			this.selectedHat.sprite = Assets.GetSprite((skill as SkillListable).skillHat);
			if (component != null)
			{
				text = (skill as SkillListable).skillHat;
				component.SetHats(component.CurrentHat, text);
				if (component.OwnsHat(text))
				{
					new PutOnHatChore(component, Db.Get().ChoreTypes.SwitchHat);
				}
			}
		}
		else
		{
			this.selectedHat.sprite = Assets.GetSprite(text);
			if (component != null)
			{
				component.SetHats(component.CurrentHat, null);
				component.ApplyTargetHat();
			}
		}
		foreach (SkillMinionWidget skillMinionWidget in this.minionWidgets)
		{
			if (skillMinionWidget.minion == this.currentlySelectedMinion)
			{
				skillMinionWidget.RefreshHat(component.TargetHat);
			}
		}
	}

	private void hatDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		if (entry.entryData != null)
		{
			SkillListable skillListable = entry.entryData as SkillListable;
			entry.image.sprite = Assets.GetSprite(skillListable.skillHat);
		}
	}

	private int hatDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		return 0;
	}

	private void Update()
	{
		if (this.dirty)
		{
			this.RefreshAll();
		}
		if (this.linesPending)
		{
			foreach (GameObject gameObject in this.skillWidgets.Values)
			{
				gameObject.GetComponent<SkillWidget>().RefreshLines();
			}
			this.linesPending = false;
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!e.Consumed && !this.scrollRect.isDragging && e.TryConsume(global::Action.MouseRight))
		{
			ManagementMenu.Instance.CloseAll();
			return;
		}
		base.OnKeyUp(e);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed && e.TryConsume(global::Action.Escape))
		{
			ManagementMenu.Instance.CloseAll();
			return;
		}
		base.OnKeyDown(e);
	}

	public void RefreshSkillWidgets()
	{
		int num = 1;
		foreach (SkillGroup skillGroup in Db.Get().SkillGroups.resources)
		{
			List<Skill> skillsBySkillGroup = this.GetSkillsBySkillGroup(skillGroup.Id);
			if (skillsBySkillGroup.Count > 0)
			{
				if (!this.skillGroupRow.ContainsKey(skillGroup.Id))
				{
					this.skillGroupRow.Add(skillGroup.Id, num++);
				}
				for (int i = 0; i < skillsBySkillGroup.Count; i++)
				{
					Skill skill = skillsBySkillGroup[i];
					if (!this.skillWidgets.ContainsKey(skill.Id))
					{
						while (skill.tier >= this.skillColumns.Count)
						{
							GameObject gameObject = Util.KInstantiateUI(this.Prefab_skillColumn, this.Prefab_tableLayout, true);
							this.skillColumns.Add(gameObject);
							HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
							if (this.skillColumns.Count % 2 == 0)
							{
								component.GetReference("BG").gameObject.SetActive(false);
							}
						}
						GameObject gameObject2 = Util.KInstantiateUI(this.Prefab_skillWidget, this.skillColumns[skill.tier], true);
						this.skillWidgets.Add(skill.Id, gameObject2);
					}
					this.skillWidgets[skill.Id].GetComponent<SkillWidget>().Refresh(skill.Id);
				}
			}
		}
		foreach (SkillMinionWidget skillMinionWidget in this.minionWidgets)
		{
			skillMinionWidget.Refresh();
		}
		this.RefreshWidgetPositions();
	}

	public void HoverSkill(string skillID)
	{
		this.hoveredSkillID = skillID;
		if (this.delayRefreshRoutine != null)
		{
			base.StopCoroutine(this.delayRefreshRoutine);
			this.delayRefreshRoutine = null;
		}
		if (string.IsNullOrEmpty(this.hoveredSkillID))
		{
			this.delayRefreshRoutine = base.StartCoroutine(this.DelayRefreshProgressBars());
		}
		else
		{
			this.RefreshProgressBars();
		}
	}

	private IEnumerator DelayRefreshProgressBars()
	{
		yield return new WaitForSecondsRealtime(0.1f);
		this.RefreshProgressBars();
		yield break;
	}

	public void RefreshWidgetPositions()
	{
		float num = 0f;
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.skillWidgets)
		{
			float rowPosition = this.GetRowPosition(keyValuePair.Key);
			num = Mathf.Max(rowPosition, num);
			keyValuePair.Value.rectTransform().anchoredPosition = Vector2.down * rowPosition;
		}
		num = Mathf.Max(num, (float)this.layoutRowHeight);
		float num2 = (float)this.layoutRowHeight;
		foreach (GameObject gameObject in this.skillColumns)
		{
			gameObject.GetComponent<LayoutElement>().minHeight = num + num2;
		}
		this.linesPending = true;
	}

	public float GetRowPosition(string skillID)
	{
		int num = this.skillGroupRow[Db.Get().Skills.Get(skillID).skillGroup];
		return (float)(this.layoutRowHeight * (num - 1));
	}

	private void OnAddMinionIdentity(MinionIdentity add)
	{
		this.BuildMinions();
		this.RefreshAll();
	}

	private void OnRemoveMinionIdentity(MinionIdentity remove)
	{
		this.BuildMinions();
		this.RefreshAll();
	}

	private void BuildMinions()
	{
		for (int i = this.minionWidgets.Count - 1; i >= 0; i--)
		{
			this.minionWidgets[i].DeleteObject();
		}
		this.minionWidgets.Clear();
		foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
		{
			GameObject gameObject = Util.KInstantiateUI(this.Prefab_minion, this.Prefab_minionLayout, true);
			gameObject.GetComponent<SkillMinionWidget>().SetMinon(minionIdentity);
			this.minionWidgets.Add(gameObject.GetComponent<SkillMinionWidget>());
		}
		foreach (MinionStorage minionStorage in Components.MinionStorages.Items)
		{
			foreach (MinionStorage.Info info in minionStorage.GetStoredMinionInfo())
			{
				if (info.serializedMinion != null)
				{
					StoredMinionIdentity storedMinionIdentity = info.serializedMinion.Get<StoredMinionIdentity>();
					GameObject gameObject2 = Util.KInstantiateUI(this.Prefab_minion, this.Prefab_minionLayout, true);
					gameObject2.GetComponent<SkillMinionWidget>().SetMinon(storedMinionIdentity);
					this.minionWidgets.Add(gameObject2.GetComponent<SkillMinionWidget>());
				}
			}
		}
		if (this.CurrentlySelectedMinion == null && Components.LiveMinionIdentities.Count > 0)
		{
			this.CurrentlySelectedMinion = Components.LiveMinionIdentities.Items[0];
		}
	}

	public Vector2 GetSkillWidgetLineTargetPosition(string skillID)
	{
		return this.skillWidgets[skillID].GetComponent<SkillWidget>().lines_right.GetPosition();
	}

	public SkillWidget GetSkillWidget(string skill)
	{
		return this.skillWidgets[skill].GetComponent<SkillWidget>();
	}

	public List<Skill> GetSkillsBySkillGroup(string skillGrp)
	{
		List<Skill> list = new List<Skill>();
		foreach (Skill skill in Db.Get().Skills.resources)
		{
			if (skill.skillGroup == skillGrp)
			{
				list.Add(skill);
			}
		}
		return list;
	}

	private void SelectSortToggle(MultiToggle toggle)
	{
		this.dupeSortingToggle.ChangeState(0);
		this.experienceSortingToggle.ChangeState(0);
		this.moraleSortingToggle.ChangeState(0);
		if (toggle != null)
		{
			if (this.activeSortToggle == toggle)
			{
				this.sortReversed = !this.sortReversed;
			}
			this.activeSortToggle = toggle;
		}
		this.activeSortToggle.ChangeState((!this.sortReversed) ? 1 : 2);
	}

	private void SortByMorale()
	{
		this.SelectSortToggle(this.moraleSortingToggle);
		List<SkillMinionWidget> list = this.minionWidgets;
		list.Sort(delegate(SkillMinionWidget a, SkillMinionWidget b)
		{
			MinionIdentity minionIdentity = a.minion as MinionIdentity;
			MinionIdentity minionIdentity2 = b.minion as MinionIdentity;
			if (minionIdentity == null && minionIdentity2 == null)
			{
				return 0;
			}
			if (minionIdentity == null)
			{
				return -1;
			}
			if (minionIdentity2 == null)
			{
				return 1;
			}
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			MinionResume component2 = minionIdentity2.GetComponent<MinionResume>();
			AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(component);
			AttributeInstance attributeInstance2 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(component);
			AttributeInstance attributeInstance3 = Db.Get().Attributes.QualityOfLife.Lookup(component2);
			AttributeInstance attributeInstance4 = Db.Get().Attributes.QualityOfLifeExpectation.Lookup(component2);
			float num = attributeInstance.GetTotalValue() / attributeInstance2.GetTotalValue();
			float num2 = attributeInstance3.GetTotalValue() / attributeInstance4.GetTotalValue();
			return num.CompareTo(num2);
		});
		this.ReorderEntries(list, this.sortReversed);
	}

	private void SortByMinon()
	{
		this.SelectSortToggle(this.dupeSortingToggle);
		List<SkillMinionWidget> list = this.minionWidgets;
		list.Sort((SkillMinionWidget a, SkillMinionWidget b) => a.minion.GetProperName().CompareTo(b.minion.GetProperName()));
		this.ReorderEntries(list, this.sortReversed);
	}

	private void SortByExperience()
	{
		this.SelectSortToggle(this.experienceSortingToggle);
		List<SkillMinionWidget> list = this.minionWidgets;
		list.Sort(delegate(SkillMinionWidget a, SkillMinionWidget b)
		{
			MinionIdentity minionIdentity = a.minion as MinionIdentity;
			MinionIdentity minionIdentity2 = b.minion as MinionIdentity;
			if (minionIdentity == null && minionIdentity2 == null)
			{
				return 0;
			}
			if (minionIdentity == null)
			{
				return -1;
			}
			if (minionIdentity2 == null)
			{
				return 1;
			}
			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			MinionResume component2 = minionIdentity2.GetComponent<MinionResume>();
			float num = (float)(component.AvailableSkillpoints / (component.TotalSkillPointsGained + 1));
			float num2 = (float)(component2.AvailableSkillpoints / (component2.TotalSkillPointsGained + 1));
			return num.CompareTo(num2);
		});
		this.ReorderEntries(list, this.sortReversed);
	}

	protected void ReorderEntries(List<SkillMinionWidget> sortedEntries, bool reverse)
	{
		for (int i = 0; i < sortedEntries.Count; i++)
		{
			if (reverse)
			{
				sortedEntries[i].transform.SetSiblingIndex(sortedEntries.Count - 1 - i);
			}
			else
			{
				sortedEntries[i].transform.SetSiblingIndex(i);
			}
		}
	}

	private void SetPortraitAnimator(IAssignableIdentity identity)
	{
		if (identity == null || identity.IsNull())
		{
			return;
		}
		if (this.animController == null)
		{
			this.animController = Util.KInstantiateUI(Assets.GetPrefab(new Tag("FullMinionUIPortrait")), this.duplicantAnimAnchor.gameObject, false).GetComponent<KBatchedAnimController>();
			this.animController.gameObject.SetActive(true);
			KCanvasScaler kcanvasScaler = global::UnityEngine.Object.FindObjectOfType<KCanvasScaler>();
			this.animController.animScale = this.baseCharacterScale * (1f / kcanvasScaler.GetCanvasScale());
			ScreenResize instance = ScreenResize.Instance;
			instance.OnResize = (global::System.Action)Delegate.Combine(instance.OnResize, new global::System.Action(this.OnResize));
		}
		string text = string.Empty;
		Accessorizer component = this.animController.GetComponent<Accessorizer>();
		for (int i = component.GetAccessories().Count - 1; i >= 0; i--)
		{
			component.RemoveAccessory(component.GetAccessories()[i].Get());
		}
		MinionIdentity minionIdentity = identity as MinionIdentity;
		StoredMinionIdentity storedMinionIdentity = identity as StoredMinionIdentity;
		Accessorizer accessorizer = null;
		if (minionIdentity != null)
		{
			accessorizer = minionIdentity.GetComponent<Accessorizer>();
			foreach (ResourceRef<Accessory> resourceRef in accessorizer.GetAccessories())
			{
				component.AddAccessory(resourceRef.Get());
			}
			text = minionIdentity.GetComponent<MinionResume>().CurrentHat;
		}
		else if (storedMinionIdentity != null)
		{
			foreach (ResourceRef<Accessory> resourceRef2 in storedMinionIdentity.accessories)
			{
				component.AddAccessory(resourceRef2.Get());
			}
			text = storedMinionIdentity.currentHat;
		}
		HashedString hashedString = "anim_idle_healthy_kanim";
		this.idle_anim = Assets.GetAnim(hashedString);
		if (this.idle_anim != null)
		{
			this.animController.AddAnimOverrides(this.idle_anim, 0f);
		}
		this.animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
		AccessorySlot hat = Db.Get().AccessorySlots.Hat;
		this.animController.SetSymbolVisiblity(hat.targetSymbolId, !string.IsNullOrEmpty(text));
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, string.IsNullOrEmpty(text));
		this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, !string.IsNullOrEmpty(text));
		KAnim.Build.Symbol symbol = null;
		KAnim.Build.Symbol symbol2 = null;
		if (accessorizer)
		{
			symbol = accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		else if (storedMinionIdentity != null)
		{
			symbol = storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
			symbol2 = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
		}
		this.animController.GetComponent<SymbolOverrideController>().AddSymbolOverride(Db.Get().AccessorySlots.HairAlways.targetSymbolId, symbol, 1);
		this.animController.GetComponent<SymbolOverrideController>().AddSymbolOverride(Db.Get().AccessorySlots.HatHair.targetSymbolId, symbol2, 1);
	}

	private void OnResize()
	{
		KCanvasScaler kcanvasScaler = global::UnityEngine.Object.FindObjectOfType<KCanvasScaler>();
		this.animController.animScale = this.baseCharacterScale * (1f / kcanvasScaler.GetCanvasScale());
	}

	public new const float SCREEN_SORT_KEY = 101f;

	[SerializeField]
	private KButton CloseButton;

	[Header("Prefabs")]
	[SerializeField]
	private GameObject Prefab_skillWidget;

	[SerializeField]
	private GameObject Prefab_skillColumn;

	[SerializeField]
	private GameObject Prefab_minion;

	[SerializeField]
	private GameObject Prefab_minionLayout;

	[SerializeField]
	private GameObject Prefab_tableLayout;

	[Header("Sort Toggles")]
	[SerializeField]
	private MultiToggle dupeSortingToggle;

	[SerializeField]
	private MultiToggle experienceSortingToggle;

	[SerializeField]
	private MultiToggle moraleSortingToggle;

	private MultiToggle activeSortToggle;

	private bool sortReversed;

	[Header("Duplicant Animation")]
	[SerializeField]
	private GameObject duplicantAnimAnchor;

	[SerializeField]
	private KBatchedAnimController animController;

	public float baseCharacterScale = 0.38f;

	private KAnimFile idle_anim;

	[Header("Progress Bars")]
	[SerializeField]
	private ToolTip expectationsTooltip;

	[SerializeField]
	private LocText moraleProgressLabel;

	[SerializeField]
	private GameObject moraleWarning;

	[SerializeField]
	private GameObject moraleNotch;

	[SerializeField]
	private Color moraleNotchColor;

	private List<GameObject> moraleNotches = new List<GameObject>();

	[SerializeField]
	private LocText expectationsProgressLabel;

	[SerializeField]
	private GameObject expectationWarning;

	[SerializeField]
	private GameObject expectationNotch;

	[SerializeField]
	private Color expectationNotchColor;

	[SerializeField]
	private Color expectationNotchProspectColor;

	private List<GameObject> expectationNotches = new List<GameObject>();

	[SerializeField]
	private ToolTip experienceBarTooltip;

	[SerializeField]
	private Image experienceProgressFill;

	[SerializeField]
	private LocText EXPCount;

	[SerializeField]
	private LocText duplicantLevelIndicator;

	[SerializeField]
	private KScrollRect scrollRect;

	[SerializeField]
	private DropDown hatDropDown;

	[SerializeField]
	public Image selectedHat;

	private IAssignableIdentity currentlySelectedMinion;

	private List<SkillMinionWidget> minionWidgets = new List<SkillMinionWidget>();

	private string hoveredSkillID = string.Empty;

	private Dictionary<string, GameObject> skillWidgets = new Dictionary<string, GameObject>();

	private Dictionary<string, int> skillGroupRow = new Dictionary<string, int>();

	private List<GameObject> skillColumns = new List<GameObject>();

	private bool dirty;

	private bool linesPending;

	private int layoutRowHeight = 80;

	private Coroutine delayRefreshRoutine;
}
