using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class DirectVolumeHeater : KMonoBehaviour, ISim33ms, ISim200ms, ISim1000ms, ISim4000ms, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.primaryElement = base.GetComponent<PrimaryElement>();
		this.structureTemperature = GameComps.StructureTemperatures.GetHandle(base.gameObject);
	}

	public void Sim33ms(float dt)
	{
		if (this.impulseFrequency == DirectVolumeHeater.TimeMode.ms33)
		{
			float num = 0f;
			num += this.AddHeatToVolume(dt);
			num += this.AddSelfHeat(dt);
			this.heatEffect.SetHeatBeingProducedValue(num);
		}
	}

	public void Sim200ms(float dt)
	{
		if (this.impulseFrequency == DirectVolumeHeater.TimeMode.ms200)
		{
			float num = 0f;
			num += this.AddHeatToVolume(dt);
			num += this.AddSelfHeat(dt);
			this.heatEffect.SetHeatBeingProducedValue(num);
		}
	}

	public void Sim1000ms(float dt)
	{
		if (this.impulseFrequency == DirectVolumeHeater.TimeMode.ms1000)
		{
			float num = 0f;
			num += this.AddHeatToVolume(dt);
			num += this.AddSelfHeat(dt);
			this.heatEffect.SetHeatBeingProducedValue(num);
		}
	}

	public void Sim4000ms(float dt)
	{
		if (this.impulseFrequency == DirectVolumeHeater.TimeMode.ms4000)
		{
			float num = 0f;
			num += this.AddHeatToVolume(dt);
			num += this.AddSelfHeat(dt);
			this.heatEffect.SetHeatBeingProducedValue(num);
		}
	}

	private float CalculateCellWeight(int dx, int dy, int maxDistance)
	{
		return 1f + (float)(maxDistance - Math.Abs(dx) - Math.Abs(dy));
	}

	private bool TestLineOfSight(int offsetCell)
	{
		int num = Grid.PosToCell(base.gameObject);
		int num2;
		int num3;
		Grid.CellToXY(offsetCell, out num2, out num3);
		int num4;
		int num5;
		Grid.CellToXY(num, out num4, out num5);
		return Grid.FastTestLineOfSightSolid(num4, num5, num2, num3);
	}

	private float AddSelfHeat(float dt)
	{
		if (!this.EnableEmission)
		{
			return 0f;
		}
		if (this.primaryElement.Temperature > this.maximumInternalTemperature)
		{
			return 0f;
		}
		float num = 8f;
		GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, 8f * dt, BUILDINGS.PREFABS.STEAMTURBINE2.HEAT_SOURCE, dt);
		return num;
	}

	private float AddHeatToVolume(float dt)
	{
		if (!this.EnableEmission)
		{
			return 0f;
		}
		int num = Grid.PosToCell(base.gameObject);
		int num2 = this.width / 2;
		int num3 = this.width % 2;
		int num4 = num2 + this.height;
		float num5 = 0f;
		float num6 = this.DTUs * dt / 1000f;
		for (int i = -num2; i < num2 + num3; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				if (Grid.IsCellOffsetValid(num, i, j))
				{
					int num7 = Grid.OffsetCell(num, i, j);
					if (!Grid.Solid[num7] && Grid.Mass[num7] != 0f && Grid.WorldIdx[num7] == Grid.WorldIdx[num] && this.TestLineOfSight(num7) && Grid.Temperature[num7] < this.maximumExternalTemperature)
					{
						num5 += this.CalculateCellWeight(i, j, num4);
					}
				}
			}
		}
		float num8 = num6;
		if (num5 > 0f)
		{
			num8 /= num5;
		}
		float num9 = 0f;
		for (int k = -num2; k < num2 + num3; k++)
		{
			for (int l = 0; l < this.height; l++)
			{
				if (Grid.IsCellOffsetValid(num, k, l))
				{
					int num10 = Grid.OffsetCell(num, k, l);
					if (!Grid.Solid[num10] && Grid.Mass[num10] != 0f && Grid.WorldIdx[num10] == Grid.WorldIdx[num] && this.TestLineOfSight(num10) && Grid.Temperature[num10] < this.maximumExternalTemperature)
					{
						float num11 = num8 * this.CalculateCellWeight(k, l, num4);
						num9 += num11;
						SimMessages.ModifyEnergy(num10, num11, 10000f, SimMessages.EnergySourceID.HeatBulb);
					}
				}
			}
		}
		return num9;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		string formattedHeatEnergy = GameUtil.GetFormattedHeatEnergy(this.DTUs, GameUtil.HeatEnergyFormatterUnit.Automatic);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.HEATGENERATED, formattedHeatEnergy), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.HEATGENERATED, formattedHeatEnergy), Descriptor.DescriptorType.Effect);
		list.Add(descriptor);
		return list;
	}

	[SerializeField]
	public int width = 12;

	[SerializeField]
	public int height = 4;

	[SerializeField]
	public float DTUs = 100000f;

	[SerializeField]
	public float maximumInternalTemperature = 773.15f;

	[SerializeField]
	public float maximumExternalTemperature = 340f;

	[SerializeField]
	public Operational operational;

	[MyCmpAdd]
	private KBatchedAnimHeatPostProcessingEffect heatEffect;

	public bool EnableEmission;

	private HandleVector<int>.Handle structureTemperature;

	private PrimaryElement primaryElement;

	[SerializeField]
	private DirectVolumeHeater.TimeMode impulseFrequency = DirectVolumeHeater.TimeMode.ms1000;

	private enum TimeMode
	{
		ms33,
		ms200,
		ms1000,
		ms4000
	}
}
