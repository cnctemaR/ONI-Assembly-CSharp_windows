using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CometDetectorSideScreen : SideScreenContent
{
	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			this.RefreshOptions();
		}
	}

	private void RefreshOptions()
	{
		int num = 0;
		this.SetRow(num++, UI.UISIDESCREENS.COMETDETECTORSIDESCREEN.COMETS, Assets.GetSprite("asteroid"), null);
		foreach (Spacecraft spacecraft in SpacecraftManager.instance.GetSpacecraft())
		{
			this.SetRow(num++, spacecraft.GetRocketName(), Assets.GetSprite("icon_category_rocketry"), spacecraft.launchConditions);
		}
		for (int i = num; i < this.rowContainer.childCount; i++)
		{
			this.rowContainer.GetChild(i).gameObject.SetActive(false);
		}
	}

	private void ClearRows()
	{
		for (int i = this.rowContainer.childCount - 1; i >= 0; i--)
		{
			Util.KDestroyGameObject(this.rowContainer.GetChild(i));
		}
		this.rows.Clear();
	}

	public override void SetTarget(GameObject target)
	{
		this.detector = target.GetSMI<CometDetector.Instance>();
		this.RefreshOptions();
	}

	private void SetRow(int idx, string name, Sprite icon, LaunchConditionManager target)
	{
		GameObject gameObject;
		if (idx < this.rowContainer.childCount)
		{
			gameObject = this.rowContainer.GetChild(idx).gameObject;
		}
		else
		{
			gameObject = Util.KInstantiateUI(this.rowPrefab, this.rowContainer.gameObject, true);
		}
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("label").text = name;
		component.GetReference<Image>("icon").sprite = icon;
		MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
		component2.ChangeState((this.detector.GetTargetCraft() == target) ? 1 : 0);
		LaunchConditionManager _target = target;
		component2.onClick = delegate
		{
			this.detector.SetTargetCraft(_target);
			this.RefreshOptions();
		};
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetSMI<CometDetector.Instance>() != null;
	}

	private CometDetector.Instance detector;

	public GameObject rowPrefab;

	public RectTransform rowContainer;

	public Dictionary<object, GameObject> rows = new Dictionary<object, GameObject>();
}
