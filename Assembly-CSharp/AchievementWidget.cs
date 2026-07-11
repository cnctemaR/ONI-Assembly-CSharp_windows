using System;
using System.Collections;
using System.Collections.Generic;
using Database;
using FMOD.Studio;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AchievementWidget : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		MultiToggle component = base.GetComponent<MultiToggle>();
		component.onClick = (global::System.Action)Delegate.Combine(component.onClick, new global::System.Action(delegate
		{
			this.ExpandAchievement();
		}));
	}

	private void Update()
	{
	}

	private void ExpandAchievement()
	{
		if (SaveGame.Instance != null)
		{
			this.progressParent.gameObject.SetActive(!this.progressParent.gameObject.activeSelf);
		}
	}

	public void ActivateNewlyAchievedFlourish(float delay = 1f)
	{
		base.StartCoroutine(this.Flourish(delay));
	}

	private IEnumerator Flourish(float startDelay)
	{
		this.SetNeverAchieved();
		if (base.GetComponent<Canvas>() == null)
		{
			Canvas canvas = base.gameObject.AddComponent<Canvas>();
			canvas.sortingOrder = 1;
		}
		base.GetComponent<Canvas>().overrideSorting = true;
		yield return new WaitForSecondsRealtime(startDelay);
		KScrollRect scrollRect = base.transform.parent.parent.GetComponent<KScrollRect>();
		float scrollTarget = 1f + base.transform.localPosition.y / scrollRect.content.rect.height;
		scrollRect.SetSmoothAutoScrollTarget(scrollTarget);
		GameObject icon = base.GetComponent<HierarchyReferences>().GetReference<Image>("icon").transform.parent.gameObject;
		foreach (KBatchedAnimController kbatchedAnimController in this.sparks)
		{
			if (kbatchedAnimController.transform.parent != icon.transform.parent)
			{
				kbatchedAnimController.GetComponent<KBatchedAnimController>().TintColour = new Color(1f, 0.86f, 0.56f, 1f);
				kbatchedAnimController.transform.SetParent(icon.transform.parent);
				kbatchedAnimController.transform.SetSiblingIndex(icon.transform.GetSiblingIndex());
				kbatchedAnimController.GetComponent<KBatchedAnimCanvasRenderer>().compare = CompareFunction.Always;
			}
		}
		HierarchyReferences refs = base.GetComponent<HierarchyReferences>();
		refs.GetReference<Image>("iconBG").color = this.color_dark_red;
		refs.GetReference<Image>("iconBorder").color = this.color_gold;
		refs.GetReference<Image>("icon").color = this.color_gold;
		bool colorChanged = false;
		EventInstance achievementUnlockedSound = KFMOD.BeginOneShot(GlobalAssets.GetSound("AchievementUnlocked", false), Vector3.zero);
		int pitchParamValue = Mathf.RoundToInt(MathUtil.Clamp(1f, 7f, startDelay - startDelay % 1f / 1f)) - 1;
		achievementUnlockedSound.setParameterValue("num_achievements", (float)pitchParamValue);
		global::Debug.Log("parameter: " + pitchParamValue);
		KFMOD.EndOneShot(achievementUnlockedSound);
		for (float i = 0f; i < 1.2f; i += Time.unscaledDeltaTime)
		{
			icon.transform.localScale = Vector3.one * this.flourish_iconScaleCurve.Evaluate(i);
			this.sheenTransform.anchoredPosition = new Vector2(this.flourish_sheenPositionCurve.Evaluate(i), this.sheenTransform.anchoredPosition.y);
			if (i > 1f && !colorChanged)
			{
				colorChanged = true;
				foreach (KBatchedAnimController kbatchedAnimController2 in this.sparks)
				{
					kbatchedAnimController2.Play("spark", KAnim.PlayMode.Once, 1f, 0f);
				}
				this.SetAchievedNow();
			}
			yield return 0;
		}
		icon.transform.localScale = Vector3.one;
		for (float j = 0f; j < 0.3f; j += Time.unscaledDeltaTime)
		{
			yield return 0;
		}
		base.GetComponent<Canvas>().overrideSorting = false;
		base.transform.localScale = Vector3.one;
		yield break;
	}

	public void SetAchievedNow()
	{
		MultiToggle component = base.GetComponent<MultiToggle>();
		component.ChangeState(1);
		HierarchyReferences component2 = base.GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = this.color_dark_red;
		component2.GetReference<Image>("iconBorder").color = this.color_gold;
		component2.GetReference<Image>("icon").color = this.color_gold;
		foreach (LocText locText in base.GetComponentsInChildren<LocText>())
		{
			locText.color = Color.white;
		}
		this.ConfigureToolTip(base.GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.ACHIEVED_THIS_COLONY_TOOLTIP);
	}

	public void SetAchievedBefore()
	{
		MultiToggle component = base.GetComponent<MultiToggle>();
		component.ChangeState(1);
		HierarchyReferences component2 = base.GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = this.color_dark_red;
		component2.GetReference<Image>("iconBorder").color = this.color_gold;
		component2.GetReference<Image>("icon").color = this.color_gold;
		foreach (LocText locText in base.GetComponentsInChildren<LocText>())
		{
			locText.color = Color.white;
		}
		this.ConfigureToolTip(base.GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.ACHIEVED_OTHER_COLONY_TOOLTIP);
	}

	public void SetNeverAchieved()
	{
		MultiToggle component = base.GetComponent<MultiToggle>();
		component.ChangeState(2);
		HierarchyReferences component2 = base.GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = this.color_dark_grey;
		component2.GetReference<Image>("iconBorder").color = this.color_grey;
		component2.GetReference<Image>("icon").color = this.color_grey;
		foreach (LocText locText in base.GetComponentsInChildren<LocText>())
		{
			locText.color = new Color(locText.color.r, locText.color.g, locText.color.b, 0.6f);
		}
		this.ConfigureToolTip(base.GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.NOT_ACHIEVED_EVER);
	}

	public void SetNotAchieved()
	{
		MultiToggle component = base.GetComponent<MultiToggle>();
		component.ChangeState(2);
		HierarchyReferences component2 = base.GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = this.color_dark_grey;
		component2.GetReference<Image>("iconBorder").color = this.color_grey;
		component2.GetReference<Image>("icon").color = this.color_grey;
		foreach (LocText locText in base.GetComponentsInChildren<LocText>())
		{
			locText.color = new Color(locText.color.r, locText.color.g, locText.color.b, 0.6f);
		}
		this.ConfigureToolTip(base.GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.NOT_ACHIEVED_THIS_COLONY);
	}

	public void SetFailed()
	{
		MultiToggle component = base.GetComponent<MultiToggle>();
		component.ChangeState(2);
		HierarchyReferences component2 = base.GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = this.color_dark_grey;
		component2.GetReference<Image>("iconBG").SetAlpha(0.5f);
		component2.GetReference<Image>("iconBorder").color = this.color_grey;
		component2.GetReference<Image>("iconBorder").SetAlpha(0.5f);
		component2.GetReference<Image>("icon").color = this.color_grey;
		component2.GetReference<Image>("icon").SetAlpha(0.5f);
		foreach (LocText locText in base.GetComponentsInChildren<LocText>())
		{
			locText.color = new Color(locText.color.r, locText.color.g, locText.color.b, 0.25f);
		}
		this.ConfigureToolTip(base.GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.FAILED_THIS_COLONY);
	}

	private void ConfigureToolTip(ToolTip tooltip, string status)
	{
		tooltip.ClearMultiStringTooltip();
		tooltip.AddMultiStringTooltip(status, null);
		if (SaveGame.Instance != null && !this.progressParent.gameObject.activeSelf)
		{
			tooltip.AddMultiStringTooltip(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXPAND_TOOLTIP, null);
		}
	}

	public void ShowProgress(ColonyAchievementStatus achievement)
	{
		if (this.progressParent == null)
		{
			return;
		}
		this.numRequirementsDisplayed = 0;
		for (int i = 0; i < achievement.Requirements.Count; i++)
		{
			ColonyAchievementRequirement colonyAchievementRequirement = achievement.Requirements[i];
			if (colonyAchievementRequirement is CritterTypesWithTraits)
			{
				this.ShowCritterChecklist(colonyAchievementRequirement);
			}
			else if (colonyAchievementRequirement is DupesCompleteChoreInExoSuitForCycles)
			{
				this.ShowDupesInExoSuitsRequirement(achievement.success, colonyAchievementRequirement);
			}
			else if (colonyAchievementRequirement is DupesVsSolidTransferArmFetch)
			{
				this.ShowArmsOutPeformingDupesRequirement(achievement.success, colonyAchievementRequirement);
			}
			else if (colonyAchievementRequirement is ProduceXEngeryWithoutUsingYList)
			{
				this.ShowEngeryWithoutUsing(achievement.success, colonyAchievementRequirement);
			}
			else if (colonyAchievementRequirement is MinimumMorale)
			{
				this.ShowMinimumMoraleRequirement(achievement.success, colonyAchievementRequirement);
			}
			else
			{
				this.ShowRequirement(achievement.success, colonyAchievementRequirement);
			}
		}
	}

	private HierarchyReferences GetNextRequirementWidget()
	{
		GameObject gameObject;
		if (this.progressParent.childCount <= this.numRequirementsDisplayed)
		{
			gameObject = global::Util.KInstantiateUI(this.requirementPrefab, this.progressParent.gameObject, true);
		}
		else
		{
			gameObject = this.progressParent.GetChild(this.numRequirementsDisplayed).gameObject;
			gameObject.SetActive(true);
		}
		this.numRequirementsDisplayed++;
		return gameObject.GetComponent<HierarchyReferences>();
	}

	private void SetDescription(string str, HierarchyReferences refs)
	{
		LocText reference = refs.GetReference<LocText>("Desc");
		reference.SetText(str);
	}

	private void SetIcon(Sprite sprite, Color color, HierarchyReferences refs)
	{
		Image reference = refs.GetReference<Image>("Icon");
		reference.sprite = sprite;
		reference.color = color;
		reference.gameObject.SetActive(true);
	}

	private void ShowIcon(bool show, HierarchyReferences refs)
	{
		Image reference = refs.GetReference<Image>("Icon");
		reference.gameObject.SetActive(show);
	}

	private void ShowRequirement(bool succeed, ColonyAchievementRequirement req)
	{
		HierarchyReferences nextRequirementWidget = this.GetNextRequirementWidget();
		bool flag = req.Success() || succeed;
		bool flag2 = req.Fail();
		if (flag && !flag2)
		{
			this.SetIcon(this.statusSuccessIcon, Color.green, nextRequirementWidget);
		}
		else if (flag2)
		{
			this.SetIcon(this.statusFailureIcon, Color.red, nextRequirementWidget);
		}
		else
		{
			this.ShowIcon(false, nextRequirementWidget);
		}
		this.SetDescription(req.GetProgress(flag), nextRequirementWidget);
	}

	private void ShowCritterChecklist(ColonyAchievementRequirement req)
	{
		CritterTypesWithTraits critterTypesWithTraits = req as CritterTypesWithTraits;
		if (req == null)
		{
			return;
		}
		foreach (KeyValuePair<Tag, bool> keyValuePair in critterTypesWithTraits.critterTypesToCheck)
		{
			HierarchyReferences nextRequirementWidget = this.GetNextRequirementWidget();
			if (keyValuePair.Value)
			{
				this.SetIcon(this.statusSuccessIcon, Color.green, nextRequirementWidget);
			}
			this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TAME_A_CRITTER, keyValuePair.Key.Name.ProperName()), nextRequirementWidget);
		}
	}

	private void ShowArmsOutPeformingDupesRequirement(bool succeed, ColonyAchievementRequirement req)
	{
		DupesVsSolidTransferArmFetch dupesVsSolidTransferArmFetch = req as DupesVsSolidTransferArmFetch;
		if (dupesVsSolidTransferArmFetch == null)
		{
			return;
		}
		HierarchyReferences hierarchyReferences = this.GetNextRequirementWidget();
		if (succeed)
		{
			this.SetIcon(this.statusSuccessIcon, Color.green, hierarchyReferences);
		}
		this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ARM_PERFORMANCE, (!succeed) ? dupesVsSolidTransferArmFetch.currentCycleCount : dupesVsSolidTransferArmFetch.numCycles, dupesVsSolidTransferArmFetch.numCycles), hierarchyReferences);
		if (!succeed)
		{
			Dictionary<int, int> fetchDupeChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchDupeChoreDeliveries;
			Dictionary<int, int> fetchAutomatedChoreDeliveries = SaveGame.Instance.GetComponent<ColonyAchievementTracker>().fetchAutomatedChoreDeliveries;
			int num = 0;
			fetchDupeChoreDeliveries.TryGetValue(GameClock.Instance.GetCycle(), out num);
			int num2 = 0;
			fetchAutomatedChoreDeliveries.TryGetValue(GameClock.Instance.GetCycle(), out num2);
			hierarchyReferences = this.GetNextRequirementWidget();
			if ((float)num < (float)num2 * dupesVsSolidTransferArmFetch.percentage)
			{
				this.SetIcon(this.statusSuccessIcon, Color.green, hierarchyReferences);
			}
			this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ARM_VS_DUPE_FETCHES, "SolidTransferArm", num2, num), hierarchyReferences);
		}
	}

	private void ShowDupesInExoSuitsRequirement(bool succeed, ColonyAchievementRequirement req)
	{
		DupesCompleteChoreInExoSuitForCycles dupesCompleteChoreInExoSuitForCycles = req as DupesCompleteChoreInExoSuitForCycles;
		if (dupesCompleteChoreInExoSuitForCycles == null)
		{
			return;
		}
		HierarchyReferences hierarchyReferences = this.GetNextRequirementWidget();
		if (succeed)
		{
			this.SetIcon(this.statusSuccessIcon, Color.green, hierarchyReferences);
		}
		this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXOSUIT_CYCLES, (!succeed) ? dupesCompleteChoreInExoSuitForCycles.currentCycleStreak : dupesCompleteChoreInExoSuitForCycles.numCycles, dupesCompleteChoreInExoSuitForCycles.numCycles), hierarchyReferences);
		if (!succeed)
		{
			hierarchyReferences = this.GetNextRequirementWidget();
			int num = dupesCompleteChoreInExoSuitForCycles.GetNumberOfDupesForCycle(GameClock.Instance.GetCycle());
			if (num >= Components.LiveMinionIdentities.Count)
			{
				num = Components.LiveMinionIdentities.Count;
				this.SetIcon(this.statusSuccessIcon, Color.green, hierarchyReferences);
			}
			this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXOSUIT_THIS_CYCLE, num, Components.LiveMinionIdentities.Count), hierarchyReferences);
		}
	}

	private void ShowEngeryWithoutUsing(bool succeed, ColonyAchievementRequirement req)
	{
		ProduceXEngeryWithoutUsingYList produceXEngeryWithoutUsingYList = req as ProduceXEngeryWithoutUsingYList;
		if (req == null)
		{
			return;
		}
		HierarchyReferences hierarchyReferences = this.GetNextRequirementWidget();
		float productionAmount = produceXEngeryWithoutUsingYList.GetProductionAmount(succeed);
		this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.GENERATE_POWER, GameUtil.GetFormattedRoundedJoules(productionAmount), GameUtil.GetFormattedRoundedJoules(produceXEngeryWithoutUsingYList.amountToProduce)), hierarchyReferences);
		if (succeed)
		{
			this.SetIcon(this.statusSuccessIcon, Color.green, hierarchyReferences);
		}
		foreach (Tag tag in produceXEngeryWithoutUsingYList.disallowedBuildings)
		{
			hierarchyReferences = this.GetNextRequirementWidget();
			if (Game.Instance.savedInfo.powerCreatedbyGeneratorType.ContainsKey(tag))
			{
				this.SetIcon(this.statusFailureIcon, Color.red, hierarchyReferences);
			}
			else
			{
				this.SetIcon(this.statusSuccessIcon, Color.green, hierarchyReferences);
			}
			BuildingDef buildingDef = Assets.GetBuildingDef(tag.Name);
			this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.NO_BUILDING, buildingDef.Name), hierarchyReferences);
		}
	}

	private void ShowMinimumMoraleRequirement(bool success, ColonyAchievementRequirement req)
	{
		MinimumMorale minimumMorale = req as MinimumMorale;
		if (minimumMorale == null)
		{
			return;
		}
		if (success)
		{
			this.ShowRequirement(success, req);
		}
		else
		{
			IEnumerator enumerator = Components.MinionAssignablesProxy.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					MinionAssignablesProxy minionAssignablesProxy = (MinionAssignablesProxy)obj;
					GameObject targetGameObject = minionAssignablesProxy.GetTargetGameObject();
					if (targetGameObject != null)
					{
						if (!targetGameObject.HasTag(GameTags.Dead))
						{
							AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(targetGameObject.GetComponent<MinionModifiers>());
							if (attributeInstance != null)
							{
								HierarchyReferences nextRequirementWidget = this.GetNextRequirementWidget();
								if (attributeInstance.GetTotalValue() >= (float)minimumMorale.minimumMorale)
								{
									this.SetIcon(this.statusSuccessIcon, Color.green, nextRequirementWidget);
								}
								this.SetDescription(string.Format("{0} morale: {1}", targetGameObject.GetProperName(), attributeInstance.GetTotalDisplayValue()), nextRequirementWidget);
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}

	private Color color_dark_red = new Color(0.28235295f, 0.16078432f, 0.14901961f);

	private Color color_gold = new Color(1f, 0.63529414f, 0.28627452f);

	private Color color_dark_grey = new Color(0.21568628f, 0.21568628f, 0.21568628f);

	private Color color_grey = new Color(0.6901961f, 0.6901961f, 0.6901961f);

	[SerializeField]
	private RectTransform sheenTransform;

	public AnimationCurve flourish_iconScaleCurve;

	public AnimationCurve flourish_sheenPositionCurve;

	public KBatchedAnimController[] sparks;

	[SerializeField]
	private RectTransform progressParent;

	[SerializeField]
	private GameObject requirementPrefab;

	[SerializeField]
	private Sprite statusSuccessIcon;

	[SerializeField]
	private Sprite statusFailureIcon;

	private int numRequirementsDisplayed;
}
