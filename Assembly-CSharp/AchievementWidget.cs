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

[AddComponentMenu("KMonoBehaviour/scripts/AchievementWidget")]
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
		Canvas canvas = base.GetComponent<Canvas>();
		if (canvas == null)
		{
			canvas = base.gameObject.AddComponent<Canvas>();
		}
		yield return SequenceUtil.WaitForSecondsRealtime(startDelay);
		KScrollRect component = base.transform.parent.parent.GetComponent<KScrollRect>();
		float num = 1.1f;
		float num2 = 1f + base.transform.localPosition.y * num / component.content.rect.height;
		component.SetSmoothAutoScrollTarget(num2);
		yield return SequenceUtil.WaitForSecondsRealtime(0.5f);
		canvas.overrideSorting = true;
		canvas.sortingOrder = 30;
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
		HierarchyReferences component2 = base.GetComponent<HierarchyReferences>();
		component2.GetReference<Image>("iconBG").color = this.color_dark_red;
		component2.GetReference<Image>("iconBorder").color = this.color_gold;
		component2.GetReference<Image>("icon").color = this.color_gold;
		bool colorChanged = false;
		EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("AchievementUnlocked", false), Vector3.zero, 1f);
		int num3 = Mathf.RoundToInt(MathUtil.Clamp(1f, 7f, startDelay - startDelay % 1f / 1f)) - 1;
		eventInstance.setParameterByName("num_achievements", (float)num3, false);
		KFMOD.EndOneShot(eventInstance);
		for (float i = 0f; i < 1.2f; i += Time.unscaledDeltaTime)
		{
			icon.transform.localScale = Vector3.one * this.flourish_iconScaleCurve.Evaluate(i);
			this.sheenTransform.anchoredPosition = new Vector2(this.flourish_sheenPositionCurve.Evaluate(i), this.sheenTransform.anchoredPosition.y);
			if (i > 1f && !colorChanged)
			{
				colorChanged = true;
				KBatchedAnimController[] array = this.sparks;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].Play("spark", KAnim.PlayMode.Once, 1f, 0f);
				}
				this.SetAchievedNow();
			}
			yield return SequenceUtil.WaitForNextFrame;
		}
		icon.transform.localScale = Vector3.one;
		canvas.overrideSorting = false;
		for (float i = 0f; i < 0.6f; i += Time.unscaledDeltaTime)
		{
			yield return SequenceUtil.WaitForNextFrame;
		}
		base.transform.localScale = Vector3.one;
		yield break;
	}

	public void SetAchievedNow()
	{
		base.GetComponent<MultiToggle>().ChangeState(1);
		HierarchyReferences component = base.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = this.color_dark_red;
		component.GetReference<Image>("iconBorder").color = this.color_gold;
		component.GetReference<Image>("icon").color = this.color_gold;
		LocText[] componentsInChildren = base.GetComponentsInChildren<LocText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = Color.white;
		}
		this.ConfigureToolTip(base.GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.ACHIEVED_THIS_COLONY_TOOLTIP);
	}

	public void SetAchievedBefore()
	{
		base.GetComponent<MultiToggle>().ChangeState(1);
		HierarchyReferences component = base.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = this.color_dark_red;
		component.GetReference<Image>("iconBorder").color = this.color_gold;
		component.GetReference<Image>("icon").color = this.color_gold;
		LocText[] componentsInChildren = base.GetComponentsInChildren<LocText>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].color = Color.white;
		}
		this.ConfigureToolTip(base.GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.ACHIEVED_OTHER_COLONY_TOOLTIP);
	}

	public void SetNeverAchieved()
	{
		base.GetComponent<MultiToggle>().ChangeState(2);
		HierarchyReferences component = base.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = this.color_dark_grey;
		component.GetReference<Image>("iconBorder").color = this.color_grey;
		component.GetReference<Image>("icon").color = this.color_grey;
		foreach (LocText locText in base.GetComponentsInChildren<LocText>())
		{
			locText.color = new Color(locText.color.r, locText.color.g, locText.color.b, 0.6f);
		}
		this.ConfigureToolTip(base.GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.NOT_ACHIEVED_EVER);
	}

	public void SetNotAchieved()
	{
		base.GetComponent<MultiToggle>().ChangeState(2);
		HierarchyReferences component = base.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = this.color_dark_grey;
		component.GetReference<Image>("iconBorder").color = this.color_grey;
		component.GetReference<Image>("icon").color = this.color_grey;
		foreach (LocText locText in base.GetComponentsInChildren<LocText>())
		{
			locText.color = new Color(locText.color.r, locText.color.g, locText.color.b, 0.6f);
		}
		this.ConfigureToolTip(base.GetComponent<ToolTip>(), COLONY_ACHIEVEMENTS.NOT_ACHIEVED_THIS_COLONY);
	}

	public void SetFailed()
	{
		base.GetComponent<MultiToggle>().ChangeState(2);
		HierarchyReferences component = base.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("iconBG").color = this.color_dark_grey;
		component.GetReference<Image>("iconBG").SetAlpha(0.5f);
		component.GetReference<Image>("iconBorder").color = this.color_grey;
		component.GetReference<Image>("iconBorder").SetAlpha(0.5f);
		component.GetReference<Image>("icon").color = this.color_grey;
		component.GetReference<Image>("icon").SetAlpha(0.5f);
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
		if (this.dlcAchievement)
		{
			tooltip.AddMultiStringTooltip(COLONY_ACHIEVEMENTS.DLC.EXPANSION1, null);
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
			else if (colonyAchievementRequirement is SurviveARocketWithMinimumMorale)
			{
				this.ShowRocketMoraleRequirement(achievement.success, colonyAchievementRequirement);
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
		refs.GetReference<LocText>("Desc").SetText(str);
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
		refs.GetReference<Image>("Icon").gameObject.SetActive(show);
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
			else
			{
				this.ShowIcon(false, nextRequirementWidget);
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
		this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ARM_PERFORMANCE, succeed ? dupesVsSolidTransferArmFetch.numCycles : dupesVsSolidTransferArmFetch.currentCycleCount, dupesVsSolidTransferArmFetch.numCycles), hierarchyReferences);
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
			else
			{
				this.ShowIcon(false, hierarchyReferences);
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
		else
		{
			this.ShowIcon(false, hierarchyReferences);
		}
		this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXOSUIT_CYCLES, succeed ? dupesCompleteChoreInExoSuitForCycles.numCycles : dupesCompleteChoreInExoSuitForCycles.currentCycleStreak, dupesCompleteChoreInExoSuitForCycles.numCycles), hierarchyReferences);
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
		this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.GENERATE_POWER, GameUtil.GetFormattedRoundedJoules(productionAmount), GameUtil.GetFormattedRoundedJoules(produceXEngeryWithoutUsingYList.amountToProduce * 1000f)), hierarchyReferences);
		if (succeed)
		{
			this.SetIcon(this.statusSuccessIcon, Color.green, hierarchyReferences);
		}
		else
		{
			this.ShowIcon(false, hierarchyReferences);
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
			return;
		}
		foreach (object obj in Components.MinionAssignablesProxy)
		{
			GameObject targetGameObject = ((MinionAssignablesProxy)obj).GetTargetGameObject();
			if (targetGameObject != null && !targetGameObject.HasTag(GameTags.Dead))
			{
				AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup(targetGameObject.GetComponent<MinionModifiers>());
				if (attributeInstance != null)
				{
					HierarchyReferences nextRequirementWidget = this.GetNextRequirementWidget();
					if (attributeInstance.GetTotalValue() >= (float)minimumMorale.minimumMorale)
					{
						this.SetIcon(this.statusSuccessIcon, Color.green, nextRequirementWidget);
					}
					else
					{
						this.ShowIcon(false, nextRequirementWidget);
					}
					this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.MORALE, targetGameObject.GetProperName(), attributeInstance.GetTotalDisplayValue()), nextRequirementWidget);
				}
			}
		}
	}

	private void ShowRocketMoraleRequirement(bool success, ColonyAchievementRequirement req)
	{
		SurviveARocketWithMinimumMorale surviveARocketWithMinimumMorale = req as SurviveARocketWithMinimumMorale;
		if (surviveARocketWithMinimumMorale == null)
		{
			return;
		}
		if (success)
		{
			this.ShowRequirement(success, req);
			return;
		}
		foreach (KeyValuePair<int, int> keyValuePair in SaveGame.Instance.GetComponent<ColonyAchievementTracker>().cyclesRocketDupeMoraleAboveRequirement)
		{
			WorldContainer world = ClusterManager.Instance.GetWorld(keyValuePair.Key);
			if (world != null)
			{
				HierarchyReferences nextRequirementWidget = this.GetNextRequirementWidget();
				this.SetDescription(string.Format(COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.SURVIVE_SPACE, new object[]
				{
					surviveARocketWithMinimumMorale.minimumMorale,
					keyValuePair.Value,
					surviveARocketWithMinimumMorale.numberOfCycles,
					world.GetProperName()
				}), nextRequirementWidget);
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

	public bool dlcAchievement;
}
