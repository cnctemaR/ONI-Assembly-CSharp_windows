using System;
using UnityEngine;

public class IncubatorSideScreen : ReceptacleSideScreen
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<EggIncubator>() != null;
	}

	protected override void SetResultDescriptions(GameObject go)
	{
		string text = string.Empty;
		InfoDescription component = go.GetComponent<InfoDescription>();
		if (component)
		{
			text += component.description;
		}
		this.descriptionLabel.SetText(text);
	}

	protected override Sprite GetEntityIcon(Tag prefabTag)
	{
		GameObject prefab = Assets.GetPrefab(prefabTag);
		return Def.GetUISprite(prefab, "ui", false).first;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		EggIncubator incubator = target.GetComponent<EggIncubator>();
		this.continuousToggle.ChangeState((!incubator.autoReplaceEntity) ? 1 : 0);
		this.continuousToggle.onClick = delegate
		{
			incubator.autoReplaceEntity = !incubator.autoReplaceEntity;
			this.continuousToggle.ChangeState((!incubator.autoReplaceEntity) ? 1 : 0);
		};
	}

	public DescriptorPanel RequirementsDescriptorPanel;

	public DescriptorPanel HarvestDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	public MultiToggle continuousToggle;
}
