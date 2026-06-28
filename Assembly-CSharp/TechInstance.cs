using System;

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

	public TechInstance.SaveData Save()
	{
		string[] array = new string[this.progressInventory.PointsByTypeID.Count];
		this.progressInventory.PointsByTypeID.Keys.CopyTo(array, 0);
		float[] array2 = new float[this.progressInventory.PointsByTypeID.Count];
		this.progressInventory.PointsByTypeID.Values.CopyTo(array2, 0);
		return new TechInstance.SaveData
		{
			techId = this.tech.Id,
			complete = this.complete,
			inventoryIDs = array,
			inventoryValues = array2
		};
	}

	public void Load(TechInstance.SaveData save_data)
	{
		this.complete = save_data.complete;
		for (int i = 0; i < save_data.inventoryIDs.Length; i++)
		{
			this.progressInventory.AddResearchPoints(save_data.inventoryIDs[i], save_data.inventoryValues[i]);
		}
	}

	public Tech tech;

	private bool complete;

	public ResearchPointInventory progressInventory = new ResearchPointInventory();

	public struct SaveData
	{
		public string techId;

		public bool complete;

		public string[] inventoryIDs;

		public float[] inventoryValues;
	}
}
