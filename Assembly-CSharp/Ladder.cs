using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Ladder")]
public class Ladder : KMonoBehaviour, IGameObjectEffectDescriptor
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Rotatable component = base.GetComponent<Rotatable>();
		foreach (CellOffset cellOffset in this.offsets)
		{
			CellOffset cellOffset2 = cellOffset;
			if (component != null)
			{
				cellOffset2 = component.GetRotatedCellOffset(cellOffset);
			}
			int num = Grid.OffsetCell(Grid.PosToCell(this), cellOffset2);
			Grid.HasPole[num] = this.isPole;
			Grid.HasLadder[num] = !this.isPole;
		}
		base.GetComponent<KPrefabID>().AddTag(GameTags.Ladders, false);
		Components.Ladders.Add(this);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal, null);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Rotatable component = base.GetComponent<Rotatable>();
		foreach (CellOffset cellOffset in this.offsets)
		{
			CellOffset cellOffset2 = cellOffset;
			if (component != null)
			{
				cellOffset2 = component.GetRotatedCellOffset(cellOffset);
			}
			int num = Grid.OffsetCell(Grid.PosToCell(this), cellOffset2);
			if (Grid.Objects[num, 24] == null)
			{
				Grid.HasPole[num] = false;
				Grid.HasLadder[num] = false;
			}
		}
		Components.Ladders.Remove(this);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = null;
		if (this.upwardsMovementSpeedMultiplier != 1f)
		{
			list = new List<Descriptor>();
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.DUPLICANTMOVEMENTBOOST, GameUtil.GetFormattedPercent(this.upwardsMovementSpeedMultiplier * 100f - 100f, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.DUPLICANTMOVEMENTBOOST, GameUtil.GetFormattedPercent(this.upwardsMovementSpeedMultiplier * 100f - 100f, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect);
			list.Add(descriptor);
		}
		return list;
	}

	public float upwardsMovementSpeedMultiplier = 1f;

	public float downwardsMovementSpeedMultiplier = 1f;

	public bool isPole;

	public CellOffset[] offsets = new CellOffset[] { CellOffset.none };
}
