using System;
using STRINGS;
using UnityEngine;

public class ClamPoopStation : KMonoBehaviour, IPoopStation
{
	public bool IsWild
	{
		get
		{
			return !this.receptacleMonitor.Replanted;
		}
	}

	public bool IsOnPlanterBox
	{
		get
		{
			return !this.IsWild && this.receptacleMonitor.smi.ReceptacleObject != null && this.receptacleMonitor.smi.ReceptacleObject is PlantablePlot && (this.receptacleMonitor.smi.ReceptacleObject as PlantablePlot).IsOffGround;
		}
	}

	protected override void OnPrefabInit()
	{
		this.receptacleMonitor = base.GetComponent<ReceptacleMonitor>();
		this.harvestable = base.GetComponent<Harvestable>();
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		this.RegisterPoopStation();
		base.OnSpawn();
	}

	protected override void OnCleanUp()
	{
		this.UnregisterPoopStation();
		base.OnCleanUp();
	}

	public bool IsUserCompatibleWithPoopStation(KPrefabID userPrefabID)
	{
		return userPrefabID.HasAnyTags(ClamPoopStation.ALLOWED_USERS_IDS);
	}

	public GameObject GetPoopStationObject()
	{
		return base.gameObject;
	}

	public GameObject GetCurrentPoopStationUser()
	{
		return this.poopUser;
	}

	public bool IsPoopStationOperational()
	{
		return this.harvestable == null || !this.harvestable.CanBeHarvested;
	}

	public string[] GetPoopingAnimNames()
	{
		return null;
	}

	public void RegisterPoopStation()
	{
		Components.PoopStations.Add(base.gameObject.GetMyWorldId(), this);
	}

	public void UnregisterPoopStation()
	{
		Components.PoopStations.Remove(base.gameObject.GetMyWorldId(), this);
	}

	public PoopData GetPoopData()
	{
		if (!this.IsWild)
		{
			return new PoopData(false, this.receptacleMonitor.smi.ReceptacleObject.GetComponent<Storage>(), CREATURES.POOP.PLANT_POOP_STATION_WILD, Def.GetUISprite(base.gameObject, "ui", false).first);
		}
		return new PoopData(true, null, CREATURES.POOP.PLANT_POOP_STATION_WILD, Def.GetUISprite(base.gameObject, "ui", false).first);
	}

	public float GetAvailablePoopCapacity()
	{
		if (this.IsWild)
		{
			return 0f;
		}
		Storage component = this.receptacleMonitor.smi.ReceptacleObject.GetComponent<Storage>();
		return component.RemainingCapacity() / component.capacityKg;
	}

	public void PlayPoopStationAnim(string animName, KAnim.PlayMode playMode)
	{
	}

	public void ClearPoopStationUser(GameObject userRequestingClearing)
	{
		if (this.poopUser == userRequestingClearing)
		{
			this.poopUser = null;
			base.Trigger(-984476291, null);
		}
	}

	public bool AttemptToReservePoopStation(GameObject userRequestingReserve)
	{
		if (this.poopUser != null && this.poopUser != userRequestingReserve)
		{
			return false;
		}
		this.poopUser = userRequestingReserve;
		return true;
	}

	private static Tag[] ALLOWED_USERS_IDS = new Tag[] { "CrabFreshWater", "Crab", "CrabWood" };

	private ReceptacleMonitor receptacleMonitor;

	private Harvestable harvestable;

	private GameObject poopUser;
}
