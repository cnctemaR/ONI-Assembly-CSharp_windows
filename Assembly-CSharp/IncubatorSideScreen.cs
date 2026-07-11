using System;
using UnityEngine;

public class IncubatorSideScreen : ReceptacleSideScreen
{
	protected override void OnPrefabInit()
	{
		this.hideUndiscoveredEntities = true;
		base.OnPrefabInit();
	}

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
		IncubationMonitor.Def def = prefab.GetDef<IncubationMonitor.Def>();
		string text = "ui";
		if (def != null)
		{
			GameObject prefab2 = Assets.GetPrefab(def.spawnedCreature);
			if (prefab2)
			{
				CreatureBrain component = prefab2.GetComponent<CreatureBrain>();
				if (component && !string.IsNullOrEmpty(component.symbolPrefix))
				{
					text = component.symbolPrefix + text;
				}
			}
		}
		KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
		return Def.GetUISpriteFromMultiObjectAnim(component2.AnimFiles[0], text, false);
	}

	public DescriptorPanel RequirementsDescriptorPanel;

	public DescriptorPanel HarvestDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;
}
