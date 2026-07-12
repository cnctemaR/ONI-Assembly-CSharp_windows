using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/MinimumOperatingTemperature")]
public class MinimumOperatingTemperature : KMonoBehaviour, ISim200ms, IGameObjectEffectDescriptor
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.TestTemperature(true);
	}

	public void Sim200ms(float dt)
	{
		this.TestTemperature(false);
	}

	private void TestTemperature(bool force)
	{
		bool flag;
		if (this.primaryElement.Temperature < this.minimumTemperature)
		{
			flag = false;
		}
		else
		{
			flag = true;
			for (int i = 0; i < this.building.PlacementCells.Length; i++)
			{
				int num = this.building.PlacementCells[i];
				float num2 = Grid.Temperature[num];
				float num3 = Grid.Mass[num];
				if ((num2 != 0f || num3 != 0f) && num2 < this.minimumTemperature)
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			this.lastOffTime = Time.time;
		}
		if ((flag != this.isWarm && !flag) || (flag != this.isWarm && flag && Time.time > this.lastOffTime + 5f) || force)
		{
			this.isWarm = flag;
			this.operational.SetFlag(MinimumOperatingTemperature.warmEnoughFlag, this.isWarm);
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.TooCold, !this.isWarm, this);
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = new Descriptor(string.Format(UI.BUILDINGEFFECTS.MINIMUM_TEMP, GameUtil.GetFormattedTemperature(this.minimumTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.MINIMUM_TEMP, GameUtil.GetFormattedTemperature(this.minimumTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Effect, false);
		list.Add(descriptor);
		return list;
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	public float minimumTemperature = 275.15f;

	private const float TURN_ON_DELAY = 5f;

	private float lastOffTime;

	public static readonly Operational.Flag warmEnoughFlag = new Operational.Flag("warm_enough", Operational.Flag.Type.Functional);

	private bool isWarm;

	private HandleVector<int>.Handle partitionerEntry;
}
