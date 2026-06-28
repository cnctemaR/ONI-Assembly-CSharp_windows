using System;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class StructureTemperature : KMonoBehaviour, ISaveLoadableJson
{
	public float OperatingTemperature
	{
		get
		{
			return this.building.Def.OperatingTemperature * this.GetUpgradeMultiplier();
		}
	}

	public float BaseOperatingTemperature
	{
		get
		{
			return this.building.Def.OperatingTemperature;
		}
	}

	public float CurrentOperatingTemperature
	{
		get
		{
			return (!(this.operational != null) || !this.operational.IsActive) ? 0f : this.OperatingTemperature;
		}
	}

	public float Temperature
	{
		get
		{
			return base.GetComponent<PrimaryElement>().Temperature;
		}
	}

	private float ExhaustKilowatts
	{
		get
		{
			return this.building.Def.TemperatureModificationWhenActive * this.GetUpgradeMultiplier();
		}
	}

	protected override void OnPrefabInit()
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		component.getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(StructureTemperature.OnGetTemperature);
		component.setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(StructureTemperature.OnSetTemperature);
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		bool flag = this.building.Def.RequiresPower || this.OperatingTemperature > 0f || this.ExhaustKilowatts > 0f;
		base.enabled = flag;
		if (this.operational != null && flag)
		{
			this.Subscribe(824508782, new EventSystem.EventHandler(this.OnActiveChanged));
		}
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		float temperature = component.Temperature;
		if (!this.temperatureHandle.IsValid())
		{
			this.UpdateBuildingManagerRegistration(temperature);
		}
	}

	private void OnActiveChanged(object data)
	{
		this.UpdateBuildingManagerRegistration(this.Temperature);
	}

	private void UpdateBuildingManagerRegistration(float temperature)
	{
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		if (this.temperatureHandle.IsValid())
		{
			Game.Instance.buildingTemperatureManager.Unregister(ref this.temperatureHandle);
		}
		this.temperatureHandle = Game.Instance.buildingTemperatureManager.Register(this, temperature, component.Element);
	}

	protected override void OnCleanUp()
	{
		if (this.temperatureHandle.IsValid())
		{
			Game.Instance.buildingTemperatureManager.Unregister(ref this.temperatureHandle);
		}
		base.OnCleanUp();
	}

	private void SimUpdate(float dt)
	{
		if (this.operational.IsActive && this.ExhaustKilowatts > 0f)
		{
			int num = this.building.Def.WidthInCells * this.building.Def.HeightInCells;
			float num2 = this.ExhaustKilowatts * dt / (float)num;
			Extents extents = this.building.GetExtents();
			for (int i = 0; i < extents.height; i++)
			{
				int num3 = extents.y + i;
				for (int j = 0; j < extents.width; j++)
				{
					int num4 = extents.x + j;
					int num5 = num3 * Grid.WidthInCells + num4;
					float mass = Grid.Cell[num5].mass;
					float num6 = Mathf.Min(mass, 1.5f) / 1.5f;
					float num7 = num2 * num6;
					SimMessages.ModifyEnergy(num5, num7, SimMessages.EnergySourceID.StructureTemperature);
				}
			}
		}
	}

	private float GetUpgradeMultiplier()
	{
		if (this.upgradable != null)
		{
			return this.upgradable.GetTemperatureUpgradeMultiplier();
		}
		return 1f;
	}

	public Extents GetExtents()
	{
		return this.building.GetExtents();
	}

	public float Mass
	{
		get
		{
			return this.building.Def.MassForTemperatureModification;
		}
	}

	private static float OnGetTemperature(PrimaryElement primary_element)
	{
		StructureTemperature component = primary_element.GetComponent<StructureTemperature>();
		return Game.Instance.buildingTemperatureManager.GetTemperature(component.temperatureHandle);
	}

	private static void OnSetTemperature(PrimaryElement primary_element, float temperature)
	{
		StructureTemperature component = primary_element.GetComponent<StructureTemperature>();
		component.UpdateBuildingManagerRegistration(temperature);
	}

	private const float MAX_PRESSURE = 1.5f;

	[MyCmpReq]
	private Building building;

	[MyCmpGet]
	private Operational operational;

	[MyCmpGet]
	private Overheatable overheatable;

	[MyCmpGet]
	private Upgradable upgradable;

	private HandleVector<StructureTemperature>.Handle temperatureHandle = HandleVector<StructureTemperature>.InvalidHandle;
}
