using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class PlanterSideScreen : ReceptacleSideScreen
{
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

	protected override string GetResultDescription(GameObject seed_or_plant)
	{
		string text = string.Empty;
		GameObject gameObject = seed_or_plant;
		PlantableSeed component = seed_or_plant.GetComponent<PlantableSeed>();
		if (component != null)
		{
			gameObject = Assets.GetPrefab(component.PlantID);
			if (!string.IsNullOrEmpty(component.domesticatedDescription))
			{
				text += component.domesticatedDescription;
				text += "\n\n";
			}
		}
		else
		{
			InfoDescription component2 = gameObject.GetComponent<InfoDescription>();
			if (component2)
			{
				text += component2.description;
				text += "\n\n";
			}
		}
		Crop component3 = gameObject.GetComponent<Crop>();
		string cropRequirements = this.GetCropRequirements(component3);
		if (!string.IsNullOrEmpty(cropRequirements))
		{
			text += UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTREQUIREMENTS;
			text += cropRequirements;
			text += "\n";
		}
		string cropEffects = this.GetCropEffects(gameObject);
		if (!string.IsNullOrEmpty(cropEffects))
		{
			text += UI.UISIDESCREENS.PLANTERSIDESCREEN.PLANTEFFECTS;
			text += cropEffects;
		}
		return text;
	}

	private string GetCropRequirements(Crop crop)
	{
		string text = string.Empty;
		if (crop != null)
		{
			string crop_id = crop.cropId;
			CROPS.CropVal cropVal = CROPS.CROP_TYPES.Find((CROPS.CropVal m) => m.crop_id == crop_id);
			if (cropVal.regrow_duration != cropVal.crop_duration)
			{
				text += string.Format(UI.LISTENTRYSTRING, string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.INITIALGROWTHTIME, GameUtil.GetFormattedCycles(cropVal.crop_duration)));
				text += string.Format(UI.LISTENTRYSTRING, string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.REGROWTHTIME, GameUtil.GetFormattedCycles(cropVal.regrow_duration)));
			}
			else
			{
				text += string.Format(UI.LISTENTRYSTRING, string.Format(UI.UISIDESCREENS.PLANTERSIDESCREEN.GROWTHTIME, GameUtil.GetFormattedCycles(cropVal.crop_duration)));
			}
			if (crop.EffectDescription != null && crop.EffectDescription.Count > 0)
			{
				for (int i = 0; i < crop.EffectDescription.Count; i++)
				{
					text += string.Format(UI.LISTENTRYSTRING, crop.EffectDescription[i]);
				}
			}
		}
		return text;
	}

	private string GetCropEffects(GameObject plant_prefab)
	{
		return GameUtil.GetGameObjectEffectsString(plant_prefab);
	}
}
