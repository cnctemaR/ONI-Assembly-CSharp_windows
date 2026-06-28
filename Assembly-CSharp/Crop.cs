using System;
using System.Collections.Generic;
using KSerialization;
using TUNING;
using UnityEngine;

public class Crop : KMonoBehaviour
{
	public Storage PlanterStorage
	{
		get
		{
			return this.planterStorage;
		}
		set
		{
			this.planterStorage = value;
			if (this.planterStorage != null)
			{
				FertilizationMonitor.Instance smi = this.GetSMI<FertilizationMonitor.Instance>();
				if (smi != null)
				{
					smi.SetStorage(this.planterStorage);
				}
			}
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.UpdateStatus();
	}

	public void Configure(CROPS.CropVal cropval)
	{
		this.cropId = cropval.crop_id;
		this.numProduced = cropval.crop_num;
		this.maxHarvests = cropval.harvests;
		this.renewable = cropval.renewable;
	}

	public bool CanGrow()
	{
		return this.renewable || this.maxHarvests - this.timesHarvested > 0;
	}

	private void UpdateStatus()
	{
		if (this.harvestsRemainingHandle != Guid.Empty)
		{
			this.selectable.RemoveStatusItem(this.harvestsRemainingHandle);
		}
		this.harvestsRemainingHandle = this.selectable.AddStatusItem(Db.Get().CreatureStatusItems.HarvestsRemaining, this);
	}

	public int GetHarvestsRemaining()
	{
		return this.maxHarvests - this.timesHarvested;
	}

	public void SpawnFruit(object callbackParam)
	{
		if (this.cropId != null && this.cropId != string.Empty)
		{
			GameObject gameObject = Scenario.SpawnPrefab(Grid.PosToCell(base.gameObject), 0, 0, this.cropId, Grid.SceneLayer.Use, Folder.Entities);
			if (gameObject != null)
			{
				float num = 0.75f;
				gameObject.transform.SetPosition(gameObject.transform.position + new Vector3(0f, num, 0f));
				gameObject.SetActive(true);
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				component.Units = (float)this.numProduced;
				Edible component2 = gameObject.GetComponent<Edible>();
				if (component2)
				{
					ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component2.rations * 100000f, "Harvested a " + component2.name);
				}
			}
			else
			{
				Output.LogErrorWithObj(base.gameObject, new object[] { "tried to spawn an invalid crop prefab:", this.cropId });
			}
		}
		if (!this.renewable)
		{
			this.timesHarvested++;
			if (this.maxHarvests - this.timesHarvested <= 0)
			{
				this.Trigger(591871899, null);
			}
		}
	}

	[MyCmpReq]
	private KSelectable selectable;

	public string cropId = string.Empty;

	public int numProduced = 1;

	public bool renewable = true;

	public int maxHarvests = 1;

	[Serialize]
	private int timesHarvested;

	private Guid harvestsRemainingHandle = Guid.Empty;

	public string domesticatedDesc = string.Empty;

	public List<string> EffectDescription = new List<string>();

	private Storage planterStorage;
}
