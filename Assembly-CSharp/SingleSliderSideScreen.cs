using System;
using System.Collections.Generic;
using UnityEngine;

public class SingleSliderSideScreen : SideScreenContent
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		for (int i = 0; i < this.sliderSets.Count; i++)
		{
			this.sliderSets[i].SetupSlider(i);
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		KPrefabID component = target.GetComponent<KPrefabID>();
		ISingleSliderControl singleSliderControl = target.GetComponent<ISingleSliderControl>();
		singleSliderControl = ((singleSliderControl != null) ? singleSliderControl : target.GetSMI<ISingleSliderControl>());
		return singleSliderControl != null && !component.IsPrefabID("HydrogenGenerator".ToTag()) && !component.IsPrefabID("MethaneGenerator".ToTag()) && !component.IsPrefabID("PetroleumGenerator".ToTag()) && !component.IsPrefabID("DevGenerator".ToTag()) && !component.HasTag(GameTags.DeadReactor) && singleSliderControl.GetSliderMin(0) != singleSliderControl.GetSliderMax(0);
	}

	public override void SetTarget(GameObject new_target)
	{
		if (new_target == null)
		{
			global::Debug.LogError("Invalid gameObject received");
			return;
		}
		this.target = new_target.GetComponent<ISingleSliderControl>();
		if (this.target == null)
		{
			this.target = new_target.GetSMI<ISingleSliderControl>();
			if (this.target == null)
			{
				global::Debug.LogError("The gameObject received does not contain a ISingleSliderControl implementation");
				return;
			}
		}
		this.titleKey = this.target.SliderTitleKey;
		for (int i = 0; i < this.sliderSets.Count; i++)
		{
			this.sliderSets[i].SetTarget(this.target, i);
		}
	}

	private ISingleSliderControl target;

	public List<SliderSet> sliderSets;
}
