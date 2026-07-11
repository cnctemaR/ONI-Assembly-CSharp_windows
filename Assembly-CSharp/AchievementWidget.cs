using System;
using System.Collections;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class AchievementWidget : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private void Update()
	{
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
		int pitchParamValue = Mathf.RoundToInt(MathUtil.Clamp(startDelay - startDelay % 1f / 1f, 1f, 3f)) - 1;
		achievementUnlockedSound.setParameterValue("num_achievements", (float)pitchParamValue);
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
		base.GetComponent<ToolTip>().SetSimpleTooltip(COLONY_ACHIEVEMENTS.ACHIEVED_THIS_COLONY_TOOLTIP);
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
		base.GetComponent<ToolTip>().SetSimpleTooltip(COLONY_ACHIEVEMENTS.ACHIEVED_OTHER_COLONY_TOOLTIP);
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
		base.GetComponent<ToolTip>().SetSimpleTooltip(COLONY_ACHIEVEMENTS.NOT_ACHIEVED_EVER);
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
		base.GetComponent<ToolTip>().SetSimpleTooltip(COLONY_ACHIEVEMENTS.NOT_ACHIEVED_THIS_COLONY);
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
}
