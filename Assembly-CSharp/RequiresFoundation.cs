using System;
using System.Collections.Generic;
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
			Rotatable component = data.go.GetComponent<Rotatable>();
			Orientation orientation = ((component != null) ? component.GetOrientation() : Orientation.Neutral);
			int num2 = -(def.WidthInCells - 1) / 2;
			int num3 = def.WidthInCells / 2;
			List<int> list = new List<int>();
			for (int i = num2; i <= num3; i++)
			{
				CellOffset cellOffset = new CellOffset(i, -1);
				if (def.BuildLocationRule == BuildLocationRule.OnWall)
				{
					cellOffset = new CellOffset(i - 1, 0);
				}
				else if (def.BuildLocationRule == BuildLocationRule.OnCeiling || def.BuildLocationRule == BuildLocationRule.InCorner)
				{
					cellOffset = new CellOffset(i, def.HeightInCells);
				}
				CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
				int num4 = Grid.OffsetCell(num, rotatedCellOffset);
				list.Add(num4);
			}
			Vector2I vector2I = Grid.CellToXY(list[0]);
			Vector2I vector2I2 = Grid.CellToXY(list[list.Count - 1]);
			float num5 = (float)((vector2I.x > vector2I2.x) ? vector2I2.x : vector2I.x);
			float num6 = (float)((vector2I.x < vector2I2.x) ? vector2I2.x : vector2I.x);
			float num7 = (float)((vector2I.y > vector2I2.y) ? vector2I2.y : vector2I.y);
			float num8 = (float)((vector2I.y < vector2I2.y) ? vector2I2.y : vector2I.y);
			Rect rect = Rect.MinMaxRect(num5, num7, num6, num8);
			data.solidPartitionerEntry = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", go, (int)rect.x, (int)rect.y, (int)rect.width + 1, (int)rect.height + 1, GameScenePartitioner.Instance.solidChangedLayer, action);
			data.buildingPartitionerEntry = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", go, (int)rect.x, (int)rect.y, (int)rect.width + 1, (int)rect.height + 1, GameScenePartitioner.Instance.objectLayers[1], action);
			base.SetData(h, data);
			this.OnSolidChanged(h);
		}
		return h;
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		RequiresFoundation.Data data = base.GetData(h);
		GameScenePartitioner.Instance.Free(ref data.solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref data.buildingPartitionerEntry);
		base.SetData(h, data);
	}

	private void OnSolidChanged(HandleVector<int>.Handle h)
	{
		RequiresFoundation.Data data = base.GetData(h);
		SimCellOccupier component = data.go.GetComponent<SimCellOccupier>();
		if (component == null || component.IsReady())
		{
			Rotatable component2 = data.go.GetComponent<Rotatable>();
			Orientation orientation = ((component2 != null) ? component2.GetOrientation() : Orientation.Neutral);
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

	public static readonly Operational.Flag solidFoundation = new Operational.Flag("solid_foundation", Operational.Flag.Type.Functional);

	public struct Data
	{
		public int cell;

		public int width;

		public int height;

		public BuildLocationRule buildRule;

		public HandleVector<int>.Handle solidPartitionerEntry;

		public HandleVector<int>.Handle buildingPartitionerEntry;

		public bool solid;

		public GameObject go;
	}
}
