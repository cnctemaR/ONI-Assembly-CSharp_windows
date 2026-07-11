using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class PlanterSideScreen : ReceptacleSideScreen
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<PlantablePlot>() != null;
	}

	protected override Sprite GetEntityIcon(Tag prefabTag)
	{
		GameObject prefab = Assets.GetPrefab(prefabTag);
		PlantableSeed component = prefab.GetComponent<PlantableSeed>();
		if (component != null)
		{
			return base.GetEntityIcon(new Tag(component.PlantID));
		}
		return base.GetEntityIcon(prefabTag);
	}

	protected override void SetResultDescriptions(GameObject seed_or_plant)
	{
		string text = string.Empty;
		GameObject gameObject = seed_or_plant;
		PlantableSeed component = seed_or_plant.GetComponent<PlantableSeed>();
		List<Descriptor> list = new List<Descriptor>();
		if (component != null)
		{
			list = component.GetDescriptors(component.gameObject);
			if (this.targetReceptacle.rotatable != null && this.targetReceptacle.Direction != component.direction)
			{
				if (component.direction == SingleEntityReceptacle.ReceptacleDirection.Top)
				{
					text += UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_FLOOR;
				}
				else if (component.direction == SingleEntityReceptacle.ReceptacleDirection.Side)
				{
					text += UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_WALL;
				}
				else if (component.direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
				{
					text += UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_CEILING;
				}
				text += "\n\n";
			}
			gameObject = Assets.GetPrefab(component.PlantID);
			if (!string.IsNullOrEmpty(component.domesticatedDescription))
			{
				text += component.domesticatedDescription;
			}
		}
		else
		{
			InfoDescription component2 = gameObject.GetComponent<InfoDescription>();
			if (component2)
			{
				text += component2.description;
			}
		}
		this.descriptionLabel.SetText(text);
		List<Descriptor> plantLifeCycleDescriptors = GameUtil.GetPlantLifeCycleDescriptors(gameObject);
		if (plantLifeCycleDescriptors.Count > 0)
		{
			this.HarvestDescriptorPanel.SetDescriptors(plantLifeCycleDescriptors);
			this.HarvestDescriptorPanel.gameObject.SetActive(true);
		}
		List<Descriptor> plantRequirementDescriptors = GameUtil.GetPlantRequirementDescriptors(gameObject);
		if (list.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list, 1);
			plantRequirementDescriptors.InsertRange(plantRequirementDescriptors.Count, list);
		}
		if (plantRequirementDescriptors.Count > 0)
		{
			this.RequirementsDescriptorPanel.SetDescriptors(plantRequirementDescriptors);
			this.RequirementsDescriptorPanel.gameObject.SetActive(true);
		}
		List<Descriptor> plantEffectDescriptors = GameUtil.GetPlantEffectDescriptors(gameObject);
		if (plantEffectDescriptors.Count > 0)
		{
			this.EffectsDescriptorPanel.SetDescriptors(plantEffectDescriptors);
			this.EffectsDescriptorPanel.gameObject.SetActive(true);
		}
	}

	protected override bool AdditionalCanDepositTest()
	{
		PlantablePlot plantablePlot = this.targetReceptacle as PlantablePlot;
		return plantablePlot.ValidPlant;
	}

	public DescriptorPanel RequirementsDescriptorPanel;

	public DescriptorPanel HarvestDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;
}
