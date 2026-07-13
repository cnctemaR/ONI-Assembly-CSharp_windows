using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Database;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterContainer : KScreen, ITelepadDeliverableContainer
{
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public MinionStartingStats Stats
	{
		get
		{
			return this.stats;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.allAvailableClothingOutfits = new List<Option<ClothingOutfitTarget>>();
		foreach (ClothingOutfitTarget clothingOutfitTarget in from outfit in ClothingOutfitTarget.GetAllTemplates()
			where outfit.OutfitType == ClothingOutfitUtility.OutfitType.Clothing
			select outfit)
		{
			bool flag = false;
			foreach (string text in clothingOutfitTarget.ReadItems())
			{
				ClothingItemResource clothingItemResource = Db.Get().Permits.ClothingItems.TryGet(text);
				if (clothingItemResource != null && !clothingItemResource.IsUnlocked())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.allAvailableClothingOutfits.Add(clothingOutfitTarget);
			}
		}
		this.Initialize();
		this.characterNameTitle.OnStartedEditing += this.OnStartedEditing;
		this.characterNameTitle.OnNameChanged += this.OnNameChanged;
		this.reshuffleButton.onClick += delegate
		{
			this.Reshuffle(true);
		};
		List<IListableOption> list = new List<IListableOption>();
		foreach (SkillGroup skillGroup in new List<SkillGroup>(Db.Get().SkillGroups.resources))
		{
			list.Add(skillGroup);
		}
		list.Remove(Db.Get().SkillGroups.BionicSkills);
		this.archetypeDropDown.Initialize(list, new Action<IListableOption, object>(this.OnArchetypeEntryClick), new Func<IListableOption, IListableOption, object, int>(this.archetypeDropDownSort), new Action<DropDownEntry, object>(this.archetypeDropEntryRefreshAction), false, null);
		this.archetypeDropDown.CustomizeEmptyRow(Strings.Get("STRINGS.UI.CHARACTERCONTAINER_NOARCHETYPESELECTED"), this.noArchetypeIcon);
		List<IListableOption> list2 = new List<IListableOption>
		{
			new CharacterContainer.MinionModelOption(DUPLICANTS.MODEL.STANDARD.NAME, new List<Tag> { GameTags.Minions.Models.Standard }, Assets.GetSprite("ui_duplicant_minion_selection")),
			new CharacterContainer.MinionModelOption(DUPLICANTS.MODEL.BIONIC.NAME, new List<Tag> { GameTags.Minions.Models.Bionic }, Assets.GetSprite("ui_duplicant_bionicminion_selection"))
		};
		this.modelDropDown.Initialize(list2, new Action<IListableOption, object>(this.OnModelEntryClick), new Func<IListableOption, IListableOption, object, int>(this.modelDropDownSort), new Action<DropDownEntry, object>(this.modelDropEntryRefreshAction), true, null);
		this.modelDropDown.CustomizeEmptyRow(UI.CHARACTERCONTAINER_ALL_MODELS, Assets.GetSprite(this.allModelSprite));
		base.StartCoroutine(this.DelayedGeneration());
	}

	public void ForceStopEditingTitle()
	{
		this.characterNameTitle.ForceStopEditing();
	}

	public override float GetSortKey()
	{
		return 50f;
	}

	private IEnumerator DelayedGeneration()
	{
		yield return SequenceUtil.WaitForEndOfFrame;
		this.GenerateCharacter(this.controller.IsStarterMinion, null);
		yield break;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if (this.animController != null)
		{
			this.animController.gameObject.DeleteObject();
			this.animController = null;
		}
	}

	protected override void OnForcedCleanUp()
	{
		CharacterContainer.containers.Remove(this);
		base.OnForcedCleanUp();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.controller != null)
		{
			CharacterSelectionController characterSelectionController = this.controller;
			characterSelectionController.OnLimitReachedEvent = (global::System.Action)Delegate.Remove(characterSelectionController.OnLimitReachedEvent, new global::System.Action(this.OnCharacterSelectionLimitReached));
			CharacterSelectionController characterSelectionController2 = this.controller;
			characterSelectionController2.OnLimitUnreachedEvent = (global::System.Action)Delegate.Remove(characterSelectionController2.OnLimitUnreachedEvent, new global::System.Action(this.OnCharacterSelectionLimitUnReached));
			CharacterSelectionController characterSelectionController3 = this.controller;
			characterSelectionController3.OnReshuffleEvent = (Action<bool>)Delegate.Remove(characterSelectionController3.OnReshuffleEvent, new Action<bool>(this.Reshuffle));
		}
	}

	private void Initialize()
	{
		this.iconGroups = new List<GameObject>();
		this.traitEntries = new List<GameObject>();
		this.expectationLabels = new List<LocText>();
		this.aptitudeEntries = new List<GameObject>();
		if (CharacterContainer.containers == null)
		{
			CharacterContainer.containers = new List<CharacterContainer>();
		}
		CharacterContainer.containers.Add(this);
	}

	private void OnNameChanged(string newName)
	{
		this.stats.Name = newName;
		this.stats.personality.Name = newName;
		this.description.text = this.stats.personality.description;
	}

	private void OnStartedEditing()
	{
		KScreenManager.Instance.RefreshStack();
	}

	public void SetMinion(MinionStartingStats statsProposed)
	{
		if (this.controller != null && this.controller.IsSelected(this.stats))
		{
			this.DeselectDeliverable();
		}
		this.stats = statsProposed;
		if (this.animController != null)
		{
			global::UnityEngine.Object.Destroy(this.animController.gameObject);
			this.animController = null;
		}
		this.SetAnimator();
		this.SetInfoText();
		base.StartCoroutine(this.SetAttributes());
		this.selectButton.ClearOnClick();
		if (!this.controller.IsStarterMinion)
		{
			this.selectButton.enabled = true;
			this.selectButton.onClick += delegate
			{
				this.SelectDeliverable();
			};
		}
	}

	public void GenerateCharacter(bool is_starter, string guaranteedAptitudeID = null)
	{
		int num = 0;
		do
		{
			this.stats = new MinionStartingStats(this.permittedModels, is_starter, guaranteedAptitudeID, null, false);
			num++;
		}
		while (this.IsCharacterInvalid() && num < 20);
		if (this.animController != null)
		{
			global::UnityEngine.Object.Destroy(this.animController.gameObject);
			this.animController = null;
		}
		this.SetAnimator();
		this.SetInfoText();
		base.StartCoroutine(this.SetAttributes());
		this.selectButton.ClearOnClick();
		if (!this.controller.IsStarterMinion)
		{
			this.selectButton.enabled = true;
			this.selectButton.onClick += delegate
			{
				this.SelectDeliverable();
			};
		}
		Option<ClothingOutfitTarget> selectedOutfit = ClothingOutfitTarget.TryFromTemplateId(this.stats.personality.GetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType.Clothing));
		if (selectedOutfit.IsSome())
		{
			this.outfitSelectorIndex = this.allAvailableClothingOutfits.FindIndex((Option<ClothingOutfitTarget> outfit) => outfit.Unwrap().OutfitId == selectedOutfit.Unwrap().OutfitId);
		}
		else
		{
			this.outfitSelectorIndex = this.allAvailableClothingOutfits.FindIndex((Option<ClothingOutfitTarget> outfit) => outfit.Unwrap().OutfitId == this.stats.personality.GetSelectedTemplateOutfitId(ClothingOutfitUtility.OutfitType.Clothing));
		}
		if (this.outfitSelectorIndex == -1)
		{
			this.outfitSelectorIndex = this.allAvailableClothingOutfits.FindIndex((Option<ClothingOutfitTarget> outfit) => outfit.Unwrap().OutfitId == CharacterContainer.defaultShirtIdxToDefaultOutfitID[this.stats.personality.body]);
		}
		this.RefreshOutfitSelector();
	}

	private void SetAnimator()
	{
		if (this.animController == null)
		{
			this.animController = Util.KInstantiateUI(Assets.GetPrefab(GameTags.MinionSelectPreview), this.contentBody.gameObject, false).GetComponent<KBatchedAnimController>();
			this.animController.gameObject.SetActive(true);
			this.animController.animScale = this.baseCharacterScale;
		}
		BaseMinionConfig.ConfigureSymbols(this.animController.gameObject, true);
		this.stats.ApplyTraits(this.animController.gameObject);
		this.stats.ApplyRace(this.animController.gameObject);
		this.stats.ApplyAccessories(this.animController.gameObject);
		this.stats.ApplyOutfit(this.stats.personality, this.animController.gameObject, this.stats.GetSelectedOutfitOption());
		this.stats.ApplyJoyResponseOutfit(this.stats.personality, this.animController.gameObject);
		this.stats.ApplyExperience(this.animController.gameObject);
		HashedString idleAnim = this.GetIdleAnim(this.stats);
		this.idle_anim = Assets.GetAnim(idleAnim);
		if (this.idle_anim != null)
		{
			this.animController.AddAnimOverrides(this.idle_anim, 0f);
		}
		KAnimFile anim = Assets.GetAnim(new HashedString("crewSelect_fx_kanim"));
		this.bgAnimController.SwapAnims(new KAnimFile[] { Assets.GetAnim(CharacterContainer.portraitBGAnims[this.stats.personality.model]) });
		this.bgAnimController.Play("crewSelect_bg", KAnim.PlayMode.Loop, 1f, 0f);
		if (anim != null)
		{
			this.animController.AddAnimOverrides(anim, 0f);
		}
		this.animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private HashedString GetIdleAnim(MinionStartingStats minionStartingStats)
	{
		List<HashedString> list = new List<HashedString>();
		foreach (KeyValuePair<HashedString, string[]> keyValuePair in CharacterContainer.traitIdleAnims)
		{
			foreach (Trait trait in minionStartingStats.Traits)
			{
				if (keyValuePair.Value.Contains(trait.Id))
				{
					list.Add(keyValuePair.Key);
				}
			}
			if (keyValuePair.Value.Contains(minionStartingStats.joyTrait.Id) || keyValuePair.Value.Contains(minionStartingStats.stressTrait.Id))
			{
				list.Add(keyValuePair.Key);
			}
		}
		if (list.Count > 0)
		{
			return list.ToArray()[global::UnityEngine.Random.Range(0, list.Count)];
		}
		return CharacterContainer.idleAnims[global::UnityEngine.Random.Range(0, CharacterContainer.idleAnims.Length)];
	}

	private string GetOutfitName(int index)
	{
		if (index == -1)
		{
			return Strings.Get("STRINGS.UI.CHARACTERCONTAINER_NO_OUTFIT");
		}
		return this.allAvailableClothingOutfits[index].Unwrap().ReadName();
	}

	private void RefreshOutfitSelector()
	{
		CharacterContainer.<>c__DisplayClass76_0 CS$<>8__locals1 = new CharacterContainer.<>c__DisplayClass76_0();
		CS$<>8__locals1.<>4__this = this;
		Image reference = this.outfitSelectorReferences.GetReference<Image>("CurrentOutfitIcon");
		Image reference2 = this.outfitSelectorReferences.GetReference<Image>("NextOutfitIcon");
		MultiToggle component = reference2.transform.parent.GetComponent<MultiToggle>();
		Image reference3 = this.outfitSelectorReferences.GetReference<Image>("PreviousOutfitIcon");
		MultiToggle component2 = reference3.transform.parent.GetComponent<MultiToggle>();
		MultiToggle reference4 = this.outfitSelectorReferences.GetReference<MultiToggle>("PreviousOutfitButton");
		MultiToggle reference5 = this.outfitSelectorReferences.GetReference<MultiToggle>("NextOutfitButton");
		CS$<>8__locals1.expandedMenu = this.outfitSelectorReferences.GetReference<RectTransform>("ExpandedMenu");
		CS$<>8__locals1.expandButton = this.outfitSelectorReferences.GetReference<MultiToggle>("CollapsedButton");
		CS$<>8__locals1.expandButton.onClick = null;
		MultiToggle expandButton = CS$<>8__locals1.expandButton;
		expandButton.onClick = (global::System.Action)Delegate.Combine(expandButton.onClick, new global::System.Action(delegate
		{
			CS$<>8__locals1.<>4__this.outfitSelectorExpanded = !CS$<>8__locals1.<>4__this.outfitSelectorExpanded;
			CS$<>8__locals1.expandButton.gameObject.SetActive(!CS$<>8__locals1.<>4__this.outfitSelectorExpanded);
			CS$<>8__locals1.expandedMenu.gameObject.SetActive(CS$<>8__locals1.<>4__this.outfitSelectorExpanded);
			CS$<>8__locals1.<>4__this.RefreshOutfitSelector();
		}));
		MultiToggle reference6 = this.outfitSelectorReferences.GetReference<MultiToggle>("CurrentOutfitButton");
		reference6.onClick = null;
		reference6.onClick = (global::System.Action)Delegate.Combine(reference6.onClick, new global::System.Action(delegate
		{
			CS$<>8__locals1.<>4__this.outfitSelectorExpanded = !CS$<>8__locals1.<>4__this.outfitSelectorExpanded;
			CS$<>8__locals1.expandButton.gameObject.SetActive(!CS$<>8__locals1.<>4__this.outfitSelectorExpanded);
			CS$<>8__locals1.expandedMenu.gameObject.SetActive(CS$<>8__locals1.<>4__this.outfitSelectorExpanded);
			CS$<>8__locals1.<>4__this.RefreshOutfitSelector();
		}));
		reference.sprite = CS$<>8__locals1.<RefreshOutfitSelector>g__GetClothingIcon|0(0);
		CS$<>8__locals1.expandButton.gameObject.GetComponentInChildrenOnly<Image>().sprite = reference.sprite;
		reference2.sprite = CS$<>8__locals1.<RefreshOutfitSelector>g__GetClothingIcon|0(-1);
		reference3.sprite = CS$<>8__locals1.<RefreshOutfitSelector>g__GetClothingIcon|0(1);
		CS$<>8__locals1.expandButton.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.SafeStringFormat(Strings.Get("STRINGS.UI.CHARACTERCONTAINER_EXPAND_OUTFIT_SELECTOR_BUTTON"), new object[] { this.GetOutfitName(this.outfitSelectorIndex) }));
		string outfitName = this.GetOutfitName(this.outfitSelectorIndex);
		reference.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(outfitName + "\n\n" + UI.CHARACTERCONTAINER_CONFIRM_OUTFIT_SELECTION_TOOLTIP);
		reference4.onClick = null;
		MultiToggle multiToggle = reference4;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			CS$<>8__locals1.<>4__this.outfitSelectorIndex = CS$<>8__locals1.<>4__this.GetOutfitSelectorIndex(1);
			CS$<>8__locals1.<>4__this.RefreshOutfitSelector();
			CS$<>8__locals1.<>4__this.stats.ApplyOutfit(CS$<>8__locals1.<>4__this.stats.personality, CS$<>8__locals1.<>4__this.animController.gameObject, (CS$<>8__locals1.<>4__this.outfitSelectorIndex == -1) ? default(Option<ClothingOutfitTarget>) : CS$<>8__locals1.<>4__this.allAvailableClothingOutfits[CS$<>8__locals1.<>4__this.outfitSelectorIndex]);
			if (CS$<>8__locals1.<>4__this.fxAnim != null)
			{
				CS$<>8__locals1.<>4__this.fxAnim.Play("loop", KAnim.PlayMode.Once, 1f, 0f);
			}
			UISounds.Instance.PlaySound3D(GlobalAssets.GetSound("DupeShuffle", false));
		}));
		component2.onClick = reference4.onClick;
		int num = this.GetOutfitSelectorIndex(1);
		string outfitName2 = this.GetOutfitName(num);
		reference3.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(outfitName2);
		reference5.onClick = null;
		MultiToggle multiToggle2 = reference5;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			CS$<>8__locals1.<>4__this.outfitSelectorIndex = CS$<>8__locals1.<>4__this.GetOutfitSelectorIndex(-1);
			CS$<>8__locals1.<>4__this.RefreshOutfitSelector();
			CS$<>8__locals1.<>4__this.stats.ApplyOutfit(CS$<>8__locals1.<>4__this.stats.personality, CS$<>8__locals1.<>4__this.animController.gameObject, (CS$<>8__locals1.<>4__this.outfitSelectorIndex == -1) ? default(Option<ClothingOutfitTarget>) : CS$<>8__locals1.<>4__this.allAvailableClothingOutfits[CS$<>8__locals1.<>4__this.outfitSelectorIndex]);
			if (CS$<>8__locals1.<>4__this.fxAnim != null)
			{
				CS$<>8__locals1.<>4__this.fxAnim.Play("loop", KAnim.PlayMode.Once, 1f, 0f);
			}
			UISounds.Instance.PlaySound3D(GlobalAssets.GetSound("DupeShuffle", false));
		}));
		component.onClick = reference5.onClick;
		int num2 = this.GetOutfitSelectorIndex(-1);
		string outfitName3 = this.GetOutfitName(num2);
		reference2.transform.parent.GetComponent<ToolTip>().SetSimpleTooltip(outfitName3);
		this.stats.overrideOutfitID = ((this.outfitSelectorIndex == -1) ? null : this.allAvailableClothingOutfits[this.outfitSelectorIndex].Unwrap().OutfitId);
	}

	private int GetOutfitSelectorIndex(int indexOffset)
	{
		int count = this.allAvailableClothingOutfits.Count;
		int num = this.outfitSelectorIndex + indexOffset;
		if (num >= count)
		{
			num = -1;
		}
		if (num < -1)
		{
			num = count - 1;
		}
		return num;
	}

	private void SetInfoText()
	{
		this.traitEntries.ForEach(delegate(GameObject tl)
		{
			global::UnityEngine.Object.Destroy(tl.gameObject);
		});
		this.traitEntries.Clear();
		this.characterNameTitle.SetTitle(this.stats.Name);
		this.traitHeaderLabel.SetText((this.stats.personality.model == GameTags.Minions.Models.Bionic) ? UI.CHARACTERCONTAINER_TRAITS_TITLE_BIONIC : UI.CHARACTERCONTAINER_TRAITS_TITLE);
		for (int i = 1; i < this.stats.Traits.Count; i++)
		{
			Trait trait = this.stats.Traits[i];
			LocText locText = (trait.PositiveTrait ? this.goodTrait : this.badTrait);
			LocText locText2 = Util.KInstantiateUI<LocText>(locText.gameObject, locText.transform.parent.gameObject, false);
			locText2.gameObject.SetActive(true);
			locText2.text = this.stats.Traits[i].GetName();
			locText2.color = (trait.PositiveTrait ? Constants.POSITIVE_COLOR : Constants.NEGATIVE_COLOR);
			locText2.GetComponent<ToolTip>().SetSimpleTooltip(trait.GetTooltip());
			for (int j = 0; j < trait.SelfModifiers.Count; j++)
			{
				GameObject gameObject = Util.KInstantiateUI(this.attributeLabelTrait.gameObject, locText.transform.parent.gameObject, false);
				gameObject.SetActive(true);
				LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
				string text = ((trait.SelfModifiers[j].Value > 0f) ? UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_INCREASED : UI.CHARACTERCONTAINER_ATTRIBUTEMODIFIER_DECREASED);
				componentInChildren.text = string.Format(text, Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[j].AttributeId.ToUpper() + ".NAME"));
				trait.SelfModifiers[j].AttributeId == "GermResistance";
				Klei.AI.Attribute attribute = Db.Get().Attributes.Get(trait.SelfModifiers[j].AttributeId);
				string text2 = attribute.Description;
				text2 = string.Concat(new string[]
				{
					text2,
					"\n\n",
					Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + trait.SelfModifiers[j].AttributeId.ToUpper() + ".NAME"),
					": ",
					trait.SelfModifiers[j].GetFormattedString()
				});
				List<AttributeConverter> convertersForAttribute = Db.Get().AttributeConverters.GetConvertersForAttribute(attribute);
				for (int k = 0; k < convertersForAttribute.Count; k++)
				{
					string text3 = convertersForAttribute[k].DescriptionFromAttribute(convertersForAttribute[k].multiplier * trait.SelfModifiers[j].Value, null);
					if (text3 != "")
					{
						text2 = text2 + "\n    • " + text3;
					}
				}
				componentInChildren.GetComponent<ToolTip>().SetSimpleTooltip(text2);
				this.traitEntries.Add(gameObject);
			}
			if (trait.disabledChoreGroups != null)
			{
				GameObject gameObject2 = Util.KInstantiateUI(this.attributeLabelTrait.gameObject, locText.transform.parent.gameObject, false);
				gameObject2.SetActive(true);
				LocText componentInChildren2 = gameObject2.GetComponentInChildren<LocText>();
				componentInChildren2.text = trait.GetDisabledChoresString(false);
				string text4 = "";
				string text5 = "";
				for (int l = 0; l < trait.disabledChoreGroups.Length; l++)
				{
					if (l > 0)
					{
						text4 += ", ";
						text5 += "\n";
					}
					text4 += trait.disabledChoreGroups[l].Name;
					text5 += trait.disabledChoreGroups[l].description;
				}
				componentInChildren2.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(DUPLICANTS.TRAITS.CANNOT_DO_TASK_TOOLTIP, text4, text5));
				this.traitEntries.Add(gameObject2);
			}
			if (trait.ignoredEffects != null && trait.ignoredEffects.Length != 0)
			{
				GameObject gameObject3 = Util.KInstantiateUI(this.attributeLabelTrait.gameObject, locText.transform.parent.gameObject, false);
				gameObject3.SetActive(true);
				LocText componentInChildren3 = gameObject3.GetComponentInChildren<LocText>();
				componentInChildren3.text = trait.GetIgnoredEffectsString(false);
				string text6 = "";
				for (int m = 0; m < trait.ignoredEffects.Length; m++)
				{
					if (m > 0)
					{
						text6 += "\n";
					}
					text6 += string.Format(DUPLICANTS.TRAITS.IGNORED_EFFECTS_TOOLTIP, Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + trait.ignoredEffects[m].ToUpper() + ".NAME"), Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + trait.ignoredEffects[m].ToUpper() + ".CAUSE"));
					if (m < trait.ignoredEffects.Length - 1)
					{
						text6 += ",";
					}
				}
				componentInChildren3.GetComponent<ToolTip>().SetSimpleTooltip(text6);
				this.traitEntries.Add(gameObject3);
			}
			StringEntry stringEntry = null;
			if (trait.ShortDescCB != null || Strings.TryGet("STRINGS.DUPLICANTS.TRAITS." + trait.Id.ToUpper() + ".SHORT_DESC", out stringEntry))
			{
				string text7 = ((trait.ShortDescCB != null) ? trait.ShortDescCB() : stringEntry.String);
				string text8 = ((trait.ShortDescTooltipCB != null) ? trait.ShortDescTooltipCB() : Strings.Get("STRINGS.DUPLICANTS.TRAITS." + trait.Id.ToUpper() + ".SHORT_DESC_TOOLTIP"));
				GameObject gameObject4 = Util.KInstantiateUI(this.attributeLabelTrait.gameObject, locText.transform.parent.gameObject, false);
				gameObject4.SetActive(true);
				LocText componentInChildren4 = gameObject4.GetComponentInChildren<LocText>();
				componentInChildren4.text = text7;
				componentInChildren4.GetComponent<ToolTip>().SetSimpleTooltip(text8);
				this.traitEntries.Add(gameObject4);
			}
			this.traitEntries.Add(locText2.gameObject);
		}
		this.aptitudeEntries.ForEach(delegate(GameObject al)
		{
			global::UnityEngine.Object.Destroy(al.gameObject);
		});
		this.aptitudeEntries.Clear();
		this.expectationLabels.ForEach(delegate(LocText el)
		{
			global::UnityEngine.Object.Destroy(el.gameObject);
		});
		this.expectationLabels.Clear();
		if (this.stats.personality.model == GameTags.Minions.Models.Bionic)
		{
			this.aptitudeContainer.SetActive(false);
		}
		else
		{
			this.aptitudeContainer.SetActive(true);
			List<string> list = new List<string>();
			foreach (KeyValuePair<SkillGroup, float> keyValuePair in this.stats.skillAptitudes)
			{
				if (keyValuePair.Value != 0f)
				{
					SkillGroup skillGroup = Db.Get().SkillGroups.Get(keyValuePair.Key.IdHash);
					if (skillGroup == null)
					{
						global::Debug.LogWarningFormat("Role group not found for aptitude: {0}", new object[] { keyValuePair.Key });
					}
					else
					{
						GameObject gameObject5 = Util.KInstantiateUI(this.aptitudeEntry.gameObject, this.aptitudeContainer, false);
						LocText locText3 = Util.KInstantiateUI<LocText>(this.aptitudeLabel.gameObject, gameObject5, false);
						locText3.gameObject.SetActive(true);
						locText3.text = skillGroup.Name;
						string text9;
						if (skillGroup.choreGroupID != "")
						{
							ChoreGroup choreGroup = Db.Get().ChoreGroups.Get(skillGroup.choreGroupID);
							text9 = string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION_CHOREGROUP, skillGroup.Name, DUPLICANTSTATS.APTITUDE_BONUS, choreGroup.description);
						}
						else
						{
							text9 = string.Format(DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, skillGroup.Name, DUPLICANTSTATS.APTITUDE_BONUS);
						}
						locText3.GetComponent<ToolTip>().SetSimpleTooltip(text9);
						string id = keyValuePair.Key.relevantAttributes[0].Id;
						float num = (float)this.stats.StartingLevels[id];
						LocText locText4 = Util.KInstantiateUI<LocText>(this.attributeLabelAptitude.gameObject, gameObject5, false);
						locText4.gameObject.SetActive(!list.Contains(id));
						locText4.text = "+" + num.ToString() + " " + keyValuePair.Key.relevantAttributes[0].Name;
						string text10 = keyValuePair.Key.relevantAttributes[0].Description;
						text10 = string.Concat(new string[]
						{
							text10,
							"\n\n",
							keyValuePair.Key.relevantAttributes[0].Name,
							": +",
							num.ToString()
						});
						List<AttributeConverter> convertersForAttribute2 = Db.Get().AttributeConverters.GetConvertersForAttribute(keyValuePair.Key.relevantAttributes[0]);
						for (int n = 0; n < convertersForAttribute2.Count; n++)
						{
							text10 = text10 + "\n    • " + convertersForAttribute2[n].DescriptionFromAttribute(convertersForAttribute2[n].multiplier * num, null);
						}
						list.Add(id);
						locText4.GetComponent<ToolTip>().SetSimpleTooltip(text10);
						gameObject5.gameObject.SetActive(true);
						this.aptitudeEntries.Add(gameObject5);
					}
				}
			}
		}
		if (this.stats.stressTrait != null)
		{
			LocText locText5 = Util.KInstantiateUI<LocText>(this.expectationRight.gameObject, this.expectationRight.transform.parent.gameObject, false);
			locText5.gameObject.SetActive(true);
			locText5.text = string.Format(UI.CHARACTERCONTAINER_STRESSTRAIT, this.stats.stressTrait.GetName());
			locText5.GetComponent<ToolTip>().SetSimpleTooltip(this.stats.stressTrait.GetTooltip());
			this.expectationLabels.Add(locText5);
		}
		if (this.stats.joyTrait != null)
		{
			LocText locText6 = Util.KInstantiateUI<LocText>(this.expectationRight.gameObject, this.expectationRight.transform.parent.gameObject, false);
			locText6.gameObject.SetActive(true);
			locText6.text = string.Format(UI.CHARACTERCONTAINER_JOYTRAIT, this.stats.joyTrait.GetName());
			locText6.GetComponent<ToolTip>().SetSimpleTooltip(this.stats.joyTrait.GetTooltip());
			this.expectationLabels.Add(locText6);
		}
		this.description.text = this.stats.personality.description;
	}

	private IEnumerator SetAttributes()
	{
		yield return null;
		this.iconGroups.ForEach(delegate(GameObject icg)
		{
			global::UnityEngine.Object.Destroy(icg);
		});
		this.iconGroups.Clear();
		List<AttributeInstance> list = new List<AttributeInstance>(this.animController.gameObject.GetAttributes().AttributeTable);
		list.RemoveAll((AttributeInstance at) => at.Attribute.ShowInUI != Klei.AI.Attribute.Display.Skill);
		list = list.OrderBy<AttributeInstance, string>((AttributeInstance at) => at.Name).ToList<AttributeInstance>();
		for (int i = 0; i < list.Count; i++)
		{
			GameObject gameObject = Util.KInstantiateUI(this.iconGroup.gameObject, this.iconGroup.transform.parent.gameObject, false);
			LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
			gameObject.SetActive(true);
			float totalValue = list[i].GetTotalValue();
			if (totalValue > 0f)
			{
				componentInChildren.color = Constants.POSITIVE_COLOR;
			}
			else if (totalValue == 0f)
			{
				componentInChildren.color = Constants.NEUTRAL_COLOR;
			}
			else
			{
				componentInChildren.color = Constants.NEGATIVE_COLOR;
			}
			componentInChildren.text = string.Format(UI.CHARACTERCONTAINER_SKILL_VALUE, GameUtil.AddPositiveSign(totalValue.ToString(), totalValue > 0f), list[i].Name);
			AttributeInstance attributeInstance = list[i];
			string text = attributeInstance.Description;
			if (attributeInstance.Attribute.converters.Count > 0)
			{
				text += "\n";
				foreach (AttributeConverter attributeConverter in attributeInstance.Attribute.converters)
				{
					AttributeConverterInstance converter = this.animController.gameObject.GetComponent<Klei.AI.AttributeConverters>().GetConverter(attributeConverter.Id);
					string text2 = converter.DescriptionFromAttribute(converter.Evaluate(), converter.gameObject);
					if (text2 != null)
					{
						text = text + "\n" + text2;
					}
				}
			}
			gameObject.GetComponent<ToolTip>().SetSimpleTooltip(text);
			this.iconGroups.Add(gameObject);
		}
		yield break;
	}

	public void SelectDeliverable()
	{
		if (this.controller != null)
		{
			this.controller.AddDeliverable(this.stats);
		}
		if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
		{
			MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 1f, true);
		}
		this.selectButton.GetComponent<ImageToggleState>().SetActive();
		this.selectButton.ClearOnClick();
		this.selectButton.onClick += delegate
		{
			this.DeselectDeliverable();
			if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
			{
				MusicManager.instance.SetSongParameter("Music_SelectDuplicant", "songSection", 0f, true);
			}
		};
		this.selectedBorder.SetActive(true);
		this.titleBar.color = this.selectedTitleColor;
		this.animController.Play("cheer_pre", KAnim.PlayMode.Once, 1f, 0f);
		this.animController.Play("cheer_loop", KAnim.PlayMode.Loop, 1f, 0f);
	}

	public void DeselectDeliverable()
	{
		if (this.controller != null)
		{
			this.controller.RemoveDeliverable(this.stats);
		}
		this.selectButton.GetComponent<ImageToggleState>().SetInactive();
		this.selectButton.Deselect();
		this.selectButton.ClearOnClick();
		this.selectButton.onClick += delegate
		{
			this.SelectDeliverable();
		};
		this.selectedBorder.SetActive(false);
		this.titleBar.color = this.deselectedTitleColor;
		this.animController.Queue("cheer_pst", KAnim.PlayMode.Once, 1f, 0f);
		this.animController.Queue("idle_default", KAnim.PlayMode.Loop, 1f, 0f);
	}

	private void OnReplacedEvent(ITelepadDeliverable deliverable)
	{
		if (deliverable == this.stats)
		{
			this.DeselectDeliverable();
		}
	}

	private void OnCharacterSelectionLimitReached()
	{
		if (this.controller != null && this.controller.IsSelected(this.stats))
		{
			return;
		}
		this.selectButton.ClearOnClick();
		if (this.controller.AllowsReplacing)
		{
			this.selectButton.onClick += this.ReplaceCharacterSelection;
			return;
		}
		this.selectButton.onClick += this.CantSelectCharacter;
	}

	private void CantSelectCharacter()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
	}

	private void ReplaceCharacterSelection()
	{
		if (this.controller == null)
		{
			return;
		}
		this.controller.RemoveLast();
		this.SelectDeliverable();
	}

	private void OnCharacterSelectionLimitUnReached()
	{
		if (this.controller != null && this.controller.IsSelected(this.stats))
		{
			return;
		}
		this.selectButton.ClearOnClick();
		this.selectButton.onClick += delegate
		{
			this.SelectDeliverable();
		};
	}

	public void SetReshufflingState(bool enable)
	{
		this.reshuffleButton.gameObject.SetActive(enable);
		this.archetypeDropDown.gameObject.SetActive(enable);
		this.modelDropDown.transform.parent.gameObject.SetActive(enable && Game.IsDlcActiveForCurrentSave("DLC3_ID"));
	}

	public void Reshuffle(bool is_starter)
	{
		if (this.controller != null && this.controller.IsSelected(this.stats))
		{
			this.DeselectDeliverable();
		}
		if (this.fxAnim != null)
		{
			this.fxAnim.Play("loop", KAnim.PlayMode.Once, 1f, 0f);
		}
		this.GenerateCharacter(is_starter, this.guaranteedAptitudeID);
	}

	public void SetController(CharacterSelectionController csc)
	{
		if (csc == this.controller)
		{
			return;
		}
		this.controller = csc;
		CharacterSelectionController characterSelectionController = this.controller;
		characterSelectionController.OnLimitReachedEvent = (global::System.Action)Delegate.Combine(characterSelectionController.OnLimitReachedEvent, new global::System.Action(this.OnCharacterSelectionLimitReached));
		CharacterSelectionController characterSelectionController2 = this.controller;
		characterSelectionController2.OnLimitUnreachedEvent = (global::System.Action)Delegate.Combine(characterSelectionController2.OnLimitUnreachedEvent, new global::System.Action(this.OnCharacterSelectionLimitUnReached));
		CharacterSelectionController characterSelectionController3 = this.controller;
		characterSelectionController3.OnReshuffleEvent = (Action<bool>)Delegate.Combine(characterSelectionController3.OnReshuffleEvent, new Action<bool>(this.Reshuffle));
		CharacterSelectionController characterSelectionController4 = this.controller;
		characterSelectionController4.OnReplacedEvent = (Action<ITelepadDeliverable>)Delegate.Combine(characterSelectionController4.OnReplacedEvent, new Action<ITelepadDeliverable>(this.OnReplacedEvent));
	}

	public void DisableSelectButton()
	{
		this.selectButton.soundPlayer.AcceptClickCondition = () => false;
		this.selectButton.GetComponent<ImageToggleState>().SetDisabled();
		this.selectButton.soundPlayer.Enabled = false;
	}

	private bool IsCharacterInvalid()
	{
		return CharacterContainer.containers.Find((CharacterContainer container) => container != null && container.stats != null && container != this && container.stats.personality.Id == this.stats.personality.Id && container.stats.IsValid) != null || (Game.Instance != null && !Game.IsDlcActiveForCurrentSave(this.stats.personality.requiredDlcId)) || (this.stats.personality.model != GameTags.Minions.Models.Bionic && Components.LiveMinionIdentities.Items.Any<MinionIdentity>((MinionIdentity id) => id.personalityResourceId == this.stats.personality.Id));
	}

	public string GetValueColor(bool isPositive)
	{
		if (!isPositive)
		{
			return "<color=#ff2222ff>";
		}
		return "<color=green>";
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		this.scroll_rect.mouseIsOver = true;
		base.OnPointerEnter(eventData);
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		this.scroll_rect.mouseIsOver = false;
		base.OnPointerExit(eventData);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.IsAction(global::Action.Escape) || e.IsAction(global::Action.MouseRight))
		{
			this.characterNameTitle.ForceStopEditing();
			this.controller.OnPressBack();
			this.archetypeDropDown.scrollRect.gameObject.SetActive(false);
		}
		if (!KInputManager.currentControllerIsGamepad)
		{
			e.Consumed = true;
			return;
		}
		if (this.archetypeDropDown.scrollRect.activeInHierarchy)
		{
			KScrollRect component = this.archetypeDropDown.scrollRect.GetComponent<KScrollRect>();
			Vector2 vector = component.rectTransform().InverseTransformPoint(KInputManager.GetMousePos());
			if (component.rectTransform().rect.Contains(vector))
			{
				component.mouseIsOver = true;
			}
			else
			{
				component.mouseIsOver = false;
			}
			component.OnKeyDown(e);
			return;
		}
		this.scroll_rect.OnKeyDown(e);
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (!KInputManager.currentControllerIsGamepad)
		{
			e.Consumed = true;
			return;
		}
		if (this.archetypeDropDown.scrollRect.activeInHierarchy)
		{
			KScrollRect component = this.archetypeDropDown.scrollRect.GetComponent<KScrollRect>();
			Vector2 vector = component.rectTransform().InverseTransformPoint(KInputManager.GetMousePos());
			if (component.rectTransform().rect.Contains(vector))
			{
				component.mouseIsOver = true;
			}
			else
			{
				component.mouseIsOver = false;
			}
			component.OnKeyUp(e);
			return;
		}
		this.scroll_rect.OnKeyUp(e);
	}

	protected override void OnCmpEnable()
	{
		base.OnActivate();
		if (this.stats == null)
		{
			return;
		}
		this.SetAnimator();
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		this.characterNameTitle.ForceStopEditing();
	}

	private void OnArchetypeEntryClick(IListableOption skill, object data)
	{
		if (skill != null)
		{
			SkillGroup skillGroup = skill as SkillGroup;
			this.guaranteedAptitudeID = skillGroup.Id;
			this.selectedArchetypeIcon.sprite = Assets.GetSprite(skillGroup.archetypeIcon);
			this.Reshuffle(true);
			return;
		}
		this.guaranteedAptitudeID = null;
		this.selectedArchetypeIcon.sprite = this.dropdownArrowIcon;
		this.Reshuffle(true);
	}

	private int archetypeDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		if (b.Equals("Random"))
		{
			return -1;
		}
		return b.GetProperName().CompareTo(a.GetProperName());
	}

	private void archetypeDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		if (entry.entryData != null)
		{
			SkillGroup skillGroup = entry.entryData as SkillGroup;
			entry.image.sprite = Assets.GetSprite(skillGroup.archetypeIcon);
		}
	}

	private void OnModelEntryClick(IListableOption listItem, object data)
	{
		bool flag = false;
		if (listItem == null)
		{
			this.permittedModels = this.allMinionModels;
			this.selectedModelIcon.sprite = Assets.GetSprite(this.allModelSprite);
			this.Reshuffle(true);
		}
		else
		{
			CharacterContainer.MinionModelOption minionModelOption = listItem as CharacterContainer.MinionModelOption;
			if (minionModelOption != null)
			{
				flag = minionModelOption.permittedModels.Count == 1 && minionModelOption.permittedModels[0] == GameTags.Minions.Models.Bionic;
				this.permittedModels = minionModelOption.permittedModels;
				this.selectedModelIcon.sprite = minionModelOption.sprite;
				this.Reshuffle(true);
			}
		}
		this.reshuffleButton.soundPlayer.widget_sound_events()[0].OverrideAssetName = (flag ? "DupeShuffle_bionic" : "DupeShuffle");
	}

	private int modelDropDownSort(IListableOption a, IListableOption b, object targetData)
	{
		return a.GetProperName().CompareTo(b.GetProperName());
	}

	private void modelDropEntryRefreshAction(DropDownEntry entry, object targetData)
	{
		if (entry.entryData != null)
		{
			CharacterContainer.MinionModelOption minionModelOption = entry.entryData as CharacterContainer.MinionModelOption;
			entry.image.sprite = minionModelOption.sprite;
		}
	}

	public const string SHUFFLE_BUTTON_DEFAULT_SOUND_NAME_ON_USE = "DupeShuffle";

	public const string SHUFFLE_BUTTON_BIONIC_SOUND_NAME_ON_USE = "DupeShuffle_bionic";

	private static readonly Dictionary<int, string> defaultShirtIdxToDefaultOutfitID = new Dictionary<int, string>
	{
		{ 1, "StandardRed" },
		{ 2, "StandardBlue" },
		{ 3, "StandardYellow" },
		{ 4, "StandardGreen" },
		{ 5, "permit_standard_bionic_outfit" },
		{ 414842661, "permit_standard_regal_neutronium_outfit" }
	};

	[SerializeField]
	private GameObject contentBody;

	[SerializeField]
	private LocText characterName;

	[SerializeField]
	private EditableTitleBar characterNameTitle;

	[SerializeField]
	private LocText characterJob;

	[SerializeField]
	private LocText traitHeaderLabel;

	public GameObject selectedBorder;

	[SerializeField]
	private Image titleBar;

	[SerializeField]
	private Color selectedTitleColor;

	[SerializeField]
	private Color deselectedTitleColor;

	[SerializeField]
	private KButton reshuffleButton;

	private KBatchedAnimController animController;

	[SerializeField]
	private KBatchedAnimController bgAnimController;

	[SerializeField]
	private GameObject iconGroup;

	private List<GameObject> iconGroups;

	[SerializeField]
	private LocText goodTrait;

	[SerializeField]
	private LocText badTrait;

	[SerializeField]
	private GameObject aptitudeContainer;

	[SerializeField]
	private GameObject aptitudeEntry;

	[SerializeField]
	private Transform aptitudeLabel;

	[SerializeField]
	private Transform attributeLabelAptitude;

	[SerializeField]
	private Transform attributeLabelTrait;

	[SerializeField]
	private LocText expectationRight;

	private List<LocText> expectationLabels;

	[SerializeField]
	private DropDown archetypeDropDown;

	[SerializeField]
	private Image selectedArchetypeIcon;

	[SerializeField]
	private Sprite noArchetypeIcon;

	[SerializeField]
	private Sprite dropdownArrowIcon;

	private string guaranteedAptitudeID;

	private List<GameObject> aptitudeEntries;

	private List<GameObject> traitEntries;

	[SerializeField]
	private LocText description;

	[SerializeField]
	private Image selectedModelIcon;

	[SerializeField]
	private DropDown modelDropDown;

	[SerializeField]
	private HierarchyReferences outfitSelectorReferences;

	private List<Tag> permittedModels = new List<Tag>
	{
		GameTags.Minions.Models.Standard,
		GameTags.Minions.Models.Bionic
	};

	[SerializeField]
	private KToggle selectButton;

	[SerializeField]
	private KBatchedAnimController fxAnim;

	private string allModelSprite = "ui_duplicant_any_selection";

	private static Dictionary<Tag, string> portraitBGAnims = new Dictionary<Tag, string>
	{
		{
			GameTags.Minions.Models.Standard,
			"crewselect_backdrop_kanim"
		},
		{
			GameTags.Minions.Models.Bionic,
			"updated_crewSelect_bionic_backdrop_kanim"
		}
	};

	private MinionStartingStats stats;

	private CharacterSelectionController controller;

	private static List<CharacterContainer> containers;

	private KAnimFile idle_anim;

	[HideInInspector]
	public bool addMinionToIdentityList = true;

	[SerializeField]
	private Sprite enabledSpr;

	[SerializeField]
	private KScrollRect scroll_rect;

	private static readonly Dictionary<HashedString, string[]> traitIdleAnims = new Dictionary<HashedString, string[]>
	{
		{
			"anim_idle_food_kanim",
			new string[] { "Foodie" }
		},
		{
			"anim_idle_animal_lover_kanim",
			new string[] { "RanchingUp" }
		},
		{
			"anim_idle_loner_kanim",
			new string[] { "Loner" }
		},
		{
			"anim_idle_mole_hands_kanim",
			new string[] { "MoleHands" }
		},
		{
			"anim_idle_buff_kanim",
			new string[] { "StrongArm" }
		},
		{
			"anim_idle_distracted_kanim",
			new string[] { "CantResearch", "CantBuild", "CantCook", "CantDig" }
		},
		{
			"anim_idle_coaster_kanim",
			new string[] { "HappySinger" }
		}
	};

	private List<Tag> allMinionModels = new List<Tag>
	{
		GameTags.Minions.Models.Standard,
		GameTags.Minions.Models.Bionic
	};

	private static readonly HashedString[] idleAnims = new HashedString[] { "anim_idle_healthy_kanim", "anim_idle_susceptible_kanim", "anim_idle_keener_kanim", "anim_idle_fastfeet_kanim", "anim_idle_breatherdeep_kanim", "anim_idle_breathershallow_kanim" };

	public float baseCharacterScale = 0.38f;

	private List<Option<ClothingOutfitTarget>> allAvailableClothingOutfits;

	private int outfitSelectorIndex;

	private bool outfitSelectorExpanded;

	[Serializable]
	public struct ProfessionIcon
	{
		public string professionName;

		public Sprite iconImg;
	}

	private class MinionModelOption : IListableOption
	{
		public MinionModelOption(string name, List<Tag> permittedModels, Sprite sprite)
		{
			this.properName = name;
			this.permittedModels = permittedModels;
			this.sprite = sprite;
		}

		public string GetProperName()
		{
			return this.properName;
		}

		private string properName;

		public List<Tag> permittedModels;

		public Sprite sprite;
	}
}
