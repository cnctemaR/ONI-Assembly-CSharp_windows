using System;
using UnityEngine;

public class RequiresFoundation : KGameObjectComponentManager<RequiresFoundation.Data>, IKComponentManager
{
	public HandleVector<int>.Handle Add(GameObject go)
	{
		BuildingDef def = go.GetComponent<Building>().Def;
		int num = Grid.PosToCell(go.transform.GetPosition());
		RequiresFoundation.Data data = new RequiresFoundation.Data
		{
			cell = num,
			width = def.WidthInCells,
			height = def.HeightInCells,
			buildRule = def.BuildLocationRule,
			solidPartitionerEntry = null,
			buildingPartitionerEntry = null,
			solid = true,
			go = go
		};
		HandleVector<int>.Handle h = base.Add(go, data);
		if (def.ContinuouslyCheckFoundation)
		{
			Action<object> action = delegate(object d)
			{
				this.OnSolidChanged(h);
			};
			Vector2I vector2I = Grid.CellToXY(num);
			int xoffset = BuildingDef.GetXOffset(def.WidthInCells);
			data.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Overheatable.OnSpawn", go, vector2I.x + xoffset, vector2I.y - 1, def.WidthInCells, def.HeightInCells + 1, GameScenePartitioner.Instance.solidChangedLayer, action);
			data.buildingPartitionerEntry = GameScenePartitioner.Instance.Add("Overheatable.OnSpawn", go, vector2I.x + xoffset, vector2I.y - 1, def.WidthInCells, def.HeightInCells + 1, GameScenePartitioner.Instance.objectLayers[1], action);
			base.SetData(h, data);
			this.OnSolidChanged(h);
		}
		return h;
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		RequiresFoundation.Data data = base.GetData(h);
		if (data.solidPartitionerEntry != null)
		{
			data.solidPartitionerEntry.Release();
			data.solidPartitionerEntry = null;
		}
		if (data.buildingPartitionerEntry != null)
		{
			data.buildingPartitionerEntry.Release();
			data.buildingPartitionerEntry = null;
		}
		base.SetData(h, data);
	}

	private void OnSolidChanged(HandleVector<int>.Handle h)
	{
		RequiresFoundation.Data data = base.GetData(h);
		SimCellOccupier component = data.go.GetComponent<SimCellOccupier>();
		if (component == null || component.IsReady())
		{
			Rotatable component2 = data.go.GetComponent<Rotatable>();
			Orientation orientation = ((!(component2 != null)) ? Orientation.Neutral : component2.GetOrientation());
			bool flag = BuildingDef.CheckFoundation(data.cell, orientation, data.buildRule, data.width, data.height);
			this.UpdateSolidState(flag, ref data);
			base.SetData(h, data);
		}
	}

	private void UpdateSolidState(bool is_solid, ref RequiresFoundation.Data data)
	{
		if (data.solid != is_solid)
		{
			data.solid = is_solid;
			Operational component = data.go.GetComponent<Operational>();
			if (component != null)
			{
				component.SetFlag(RequiresFoundation.solidFoundation, is_solid);
			}
			data.go.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.MissingFoundation, !is_solid, this);
		}
	}

	public static Operational.Flag solidFoundation = new Operational.Flag("solid_foundation", Operational.Flag.Type.Functional);

	public struct Data
	{
		public int cell;

		public int width;

		public int height;

		public BuildLocationRule buildRule;

		public GameScenePartitionerEntry solidPartitionerEntry;

		public GameScenePartitionerEntry buildingPartitionerEntry;

		public bool solid;

		public GameObject go;
	}
}
