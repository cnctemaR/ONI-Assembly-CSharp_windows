using System;
using System.Collections.Generic;
using UnityEngine;

public class TechInstance
{
	public TechInstance(Tech tech)
	{
		this.tech = tech;
	}

	public bool IsComplete()
	{
		return this.complete;
	}

	public void Purchased()
	{
		if (!this.complete)
		{
			this.complete = true;
		}
	}

	public void UnlockPOITech(string tech_id)
	{
		TechItem techItem = Db.Get().TechItems.Get(tech_id);
		if (techItem == null || !techItem.isPOIUnlock)
		{
			return;
		}
		if (!this.UnlockedPOITechIds.Contains(tech_id))
		{
			this.UnlockedPOITechIds.Add(tech_id);
			BuildingDef buildingDef = Assets.GetBuildingDef(techItem.Id);
			if (buildingDef != null)
			{
				Game.Instance.Trigger(-107300940, buildingDef);
			}
		}
	}

	public float GetTotalPercentageComplete()
	{
		float num = 0f;
		int num2 = 0;
		foreach (string text in this.progressInventory.PointsByTypeID.Keys)
		{
			if (this.tech.RequiresResearchType(text))
			{
				num += this.PercentageCompleteResearchType(text);
				num2++;
			}
		}
		return num / (float)num2;
	}

	public float PercentageCompleteResearchType(string type)
	{
		if (!this.tech.RequiresResearchType(type))
		{
			return 1f;
		}
		return Mathf.Clamp01(this.progressInventory.PointsByTypeID[type] / this.tech.costsByResearchTypeID[type]);
	}

	public TechInstance.SaveData Save()
	{
		string[] array = new string[this.progressInventory.PointsByTypeID.Count];
		this.progressInventory.PointsByTypeID.Keys.CopyTo(array, 0);
		float[] array2 = new float[this.progressInventory.PointsByTypeID.Count];
		this.progressInventory.PointsByTypeID.Values.CopyTo(array2, 0);
		string[] array3 = this.UnlockedPOITechIds.ToArray();
		return new TechInstance.SaveData
		{
			techId = this.tech.Id,
			complete = this.complete,
			inventoryIDs = array,
			inventoryValues = array2,
			unlockedPOIIDs = array3
		};
	}

	public void Load(TechInstance.SaveData save_data)
	{
		this.complete = save_data.complete;
		for (int i = 0; i < save_data.inventoryIDs.Length; i++)
		{
			this.progressInventory.AddResearchPoints(save_data.inventoryIDs[i], save_data.inventoryValues[i]);
		}
		if (save_data.unlockedPOIIDs != null)
		{
			this.UnlockedPOITechIds = new List<string>(save_data.unlockedPOIIDs);
		}
	}

	public Tech tech;

	private bool complete;

	public ResearchPointInventory progressInventory = new ResearchPointInventory();

	public List<string> UnlockedPOITechIds = new List<string>();

	public struct SaveData
	{
		public string techId;

		public bool complete;

		public string[] inventoryIDs;

		public float[] inventoryValues;

		public string[] unlockedPOIIDs;
	}
}
