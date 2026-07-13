using System;
using System.Collections.Generic;
using System.Linq;
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
			validFoundation = true,
			operationalFlag = RequiresFoundation.solidFoundation,
			go = go
		};
		if (data.buildRule == BuildLocationRule.OnBackWall)
		{
			data.operationalFlag = RequiresFoundation.backwallFoundation;
			data.noFoundationStatusItem = Db.Get().BuildingStatusItems.MissingFoundationBackwall;
		}
		else
		{
			data.operationalFlag = RequiresFoundation.solidFoundation;
			data.noFoundationStatusItem = Db.Get().BuildingStatusItems.MissingFoundation;
		}
		HandleVector<int>.Handle h = base.Add(go, data);
		if (def.ContinuouslyCheckFoundation)
		{
			Rotatable component = data.go.GetComponent<Rotatable>();
			Orientation orientation = ((component != null) ? component.GetOrientation() : Orientation.Neutral);
			int num2 = -(def.WidthInCells - 1) / 2;
			int num3 = def.WidthInCells / 2;
			CellOffset cellOffset = new CellOffset(num2, -1);
			CellOffset cellOffset2 = new CellOffset(num3, -1);
			BuildLocationRule buildRule = data.buildRule;
			switch (buildRule)
			{
			case BuildLocationRule.OnCeiling:
			case BuildLocationRule.InCorner:
				cellOffset.y = def.HeightInCells;
				cellOffset2.y = def.HeightInCells;
				break;
			case BuildLocationRule.OnWall:
				cellOffset = new CellOffset(num2 - 1, 0);
				cellOffset2 = new CellOffset(num2 - 1, def.HeightInCells);
				break;
			default:
				if (buildRule != BuildLocationRule.WallFloor)
				{
					if (buildRule == BuildLocationRule.OnBackWall)
					{
						cellOffset = new CellOffset(num2, 0);
						cellOffset2 = new CellOffset(num3, def.HeightInCells - 1);
					}
				}
				else
				{
					cellOffset = new CellOffset(num2 - 1, -1);
					cellOffset2 = new CellOffset(num3, def.HeightInCells - 1);
				}
				break;
			}
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(cellOffset, orientation);
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(cellOffset2, orientation);
			int num4 = Grid.OffsetCell(num, rotatedCellOffset);
			int num5 = Grid.OffsetCell(num, rotatedCellOffset2);
			Vector2I vector2I = Grid.CellToXY(num4);
			Vector2I vector2I2 = Grid.CellToXY(num5);
			float num6 = (float)Mathf.Min(vector2I.x, vector2I2.x);
			float num7 = (float)Mathf.Max(vector2I.x, vector2I2.x);
			float num8 = (float)Mathf.Min(vector2I.y, vector2I2.y);
			float num9 = (float)Mathf.Max(vector2I.y, vector2I2.y);
			Rect rect = Rect.MinMaxRect(num6, num8, num7, num9);
			if (data.buildRule == BuildLocationRule.OnBackWall)
			{
				data.changeCallback = delegate(object d)
				{
					this.OnFoundationChanged(h);
				};
				data.partitionerEntry1 = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", go, (int)rect.x, (int)rect.y, (int)rect.width + 1, (int)rect.height + 1, GameScenePartitioner.Instance.objectLayers[2], data.changeCallback);
			}
			else
			{
				data.changeCallback = delegate(object d)
				{
					this.OnFoundationChanged(h);
				};
				data.partitionerEntry1 = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", go, (int)rect.x, (int)rect.y, (int)rect.width + 1, (int)rect.height + 1, GameScenePartitioner.Instance.solidChangedLayer, data.changeCallback);
				data.partitionerEntry2 = GameScenePartitioner.Instance.Add("RequiresFoundation.Add", go, (int)rect.x, (int)rect.y, (int)rect.width + 1, (int)rect.height + 1, GameScenePartitioner.Instance.objectLayers[1], data.changeCallback);
			}
			if (def.BuildLocationRule == BuildLocationRule.BuildingAttachPoint || def.BuildLocationRule == BuildLocationRule.OnFloorOrBuildingAttachPoint)
			{
				AttachableBuilding component2 = data.go.GetComponent<AttachableBuilding>();
				component2.onAttachmentNetworkChanged = (Action<object>)Delegate.Combine(component2.onAttachmentNetworkChanged, data.changeCallback);
			}
			base.SetData(h, data);
			data.changeCallback(h);
			data = base.GetData(h);
			this.UpdateValidFoundationState(data.validFoundation, ref data, true);
		}
		return h;
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		RequiresFoundation.Data data = base.GetData(h);
		GameScenePartitioner.Instance.Free(ref data.partitionerEntry1);
		GameScenePartitioner.Instance.Free(ref data.partitionerEntry2);
		AttachableBuilding component = data.go.GetComponent<AttachableBuilding>();
		if (!component.IsNullOrDestroyed())
		{
			AttachableBuilding attachableBuilding = component;
			attachableBuilding.onAttachmentNetworkChanged = (Action<object>)Delegate.Remove(attachableBuilding.onAttachmentNetworkChanged, data.changeCallback);
		}
		base.SetData(h, data);
	}

	private void OnFoundationChanged(HandleVector<int>.Handle h)
	{
		RequiresFoundation.Data data = base.GetData(h);
		SimCellOccupier component = data.go.GetComponent<SimCellOccupier>();
		if (component == null || component.IsReady())
		{
			Rotatable component2 = data.go.GetComponent<Rotatable>();
			Orientation orientation = ((component2 != null) ? component2.GetOrientation() : Orientation.Neutral);
			bool flag = BuildingDef.CheckFoundation(data.cell, orientation, data.buildRule, data.width, data.height, default(Tag));
			if (!flag && (data.buildRule == BuildLocationRule.BuildingAttachPoint || data.buildRule == BuildLocationRule.OnFloorOrBuildingAttachPoint))
			{
				List<GameObject> list = new List<GameObject>();
				AttachableBuilding.GetAttachedBelow(data.go.GetComponent<AttachableBuilding>(), ref list);
				if (list.Count > 0)
				{
					Operational component3 = list.Last<GameObject>().GetComponent<Operational>();
					if (component3 != null && component3.GetFlag(data.operationalFlag))
					{
						flag = true;
					}
				}
			}
			this.UpdateValidFoundationState(flag, ref data, false);
			base.SetData(h, data);
		}
	}

	private void UpdateValidFoundationState(bool is_validFoundation, ref RequiresFoundation.Data data, bool forceUpdate = false)
	{
		if (data.validFoundation != is_validFoundation || forceUpdate)
		{
			data.validFoundation = is_validFoundation;
			Operational component = data.go.GetComponent<Operational>();
			if (component != null)
			{
				component.SetFlag(data.operationalFlag, is_validFoundation);
			}
			AttachableBuilding component2 = data.go.GetComponent<AttachableBuilding>();
			if (component2 != null)
			{
				List<GameObject> list = new List<GameObject>();
				AttachableBuilding.GetAttachedAbove(component2, ref list);
				AttachableBuilding.NotifyBuildingsNetworkChanged(list, null);
			}
			data.go.GetComponent<KSelectable>().ToggleStatusItem(data.noFoundationStatusItem, !is_validFoundation, this);
		}
	}

	public static readonly Operational.Flag solidFoundation = new Operational.Flag("solid_foundation", Operational.Flag.Type.Functional);

	public static readonly Operational.Flag backwallFoundation = new Operational.Flag("backwall_foundation", Operational.Flag.Type.Functional);

	public struct Data
	{
		public int cell;

		public int width;

		public int height;

		public BuildLocationRule buildRule;

		public HandleVector<int>.Handle partitionerEntry1;

		public HandleVector<int>.Handle partitionerEntry2;

		public bool validFoundation;

		public Operational.Flag operationalFlag;

		public GameObject go;

		public StatusItem noFoundationStatusItem;

		public Action<object> changeCallback;
	}
}
