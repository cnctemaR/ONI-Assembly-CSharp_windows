using System;
using UnityEngine;

public class PlantableSeed : KMonoBehaviour, IHasSortOrder
{
	public int sortOrder { get; set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
	}

	private void SimUpdate(float dt)
	{
		this.timeUntilSelfPlant -= dt;
		if (this.timeUntilSelfPlant <= 0f)
		{
			this.TryPlant();
		}
	}

	public void TryPlant()
	{
		this.timeUntilSelfPlant = Util.RandomVariance(2400f, 600f);
		int num = Grid.PosToCell(base.gameObject);
		if (this.TestSuitableGround(num, false))
		{
			Vector3 vector = Grid.CellToPosCBC(num, Grid.SceneLayer.BuildingFront);
			GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(this.PlantID), vector, Grid.SceneLayer.BuildingFront, SceneOrganizer.Instance.GetFolder(Folder.Entities), null, 0);
			gameObject.SetActive(true);
			Pickupable component = base.GetComponent<Pickupable>();
			Pickupable pickupable = component.Take(1f);
			Util.KDestroyGameObject(pickupable.gameObject);
		}
	}

	private bool TestSuitableGround(int cell, bool ignoreGround = false)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		GameObject prefab = Assets.GetPrefab(this.PlantID);
		EntombVulnerable component = prefab.GetComponent<EntombVulnerable>();
		if (component != null && !component.IsCellSafe(cell))
		{
			return false;
		}
		PressureVulnerable component2 = prefab.GetComponent<PressureVulnerable>();
		if (component2 != null && !component2.IsCellSafe(cell))
		{
			return false;
		}
		DrowningMonitor component3 = prefab.GetComponent<DrowningMonitor>();
		if (component3 != null && !component3.IsCellSafe(cell))
		{
			return false;
		}
		TemperatureVulnerable component4 = prefab.GetComponent<TemperatureVulnerable>();
		if (component4 != null && !component4.IsCellSafe(cell))
		{
			return false;
		}
		UprootedMonitor component5 = prefab.GetComponent<UprootedMonitor>();
		if (component5 != null && !component5.IsCellSafe(cell))
		{
			return false;
		}
		OccupyArea component6 = prefab.GetComponent<OccupyArea>();
		if (component6 != null && !component6.CanOccupyArea(cell, ObjectLayer.Building))
		{
			return false;
		}
		if (!ignoreGround)
		{
			int num = Grid.CellBelow(cell);
			if (Grid.Foundation[num] || (this.replantGroundTag.IsValid && !Grid.Element[num].HasTag(this.replantGroundTag)))
			{
				return false;
			}
		}
		return true;
	}

	public Tag PlantID;

	public float timeUntilSelfPlant;

	public Tag replantGroundTag;

	public string domesticatedDescription;
}
