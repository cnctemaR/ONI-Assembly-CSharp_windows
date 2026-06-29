using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugTool : DragTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DebugTool.Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	public void Activate(DebugTool.Type type)
	{
		this.type = type;
		this.Activate();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		PlayerController.Instance.ToolDeactivated(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (Grid.IsValidCell(cell))
		{
			switch (this.type)
			{
			case DebugTool.Type.Dig:
				SimMessages.Dig(cell, -1);
				break;
			case DebugTool.Type.Heat:
				SimMessages.ModifyEnergy(cell, 10000f, 10000f, SimMessages.EnergySourceID.DebugHeat);
				break;
			case DebugTool.Type.Cool:
				SimMessages.ModifyEnergy(cell, -10000f, 10000f, SimMessages.EnergySourceID.DebugCool);
				break;
			case DebugTool.Type.ReplaceSubstance:
				this.DoReplaceSubstance(cell);
				break;
			case DebugTool.Type.FillReplaceSubstance:
			{
				GameUtil.FloodFillNext.Clear();
				GameUtil.FloodFillVisited.Clear();
				SimHashes elem_hash = Grid.Element[cell].id;
				GameUtil.FloodFillConditional(cell, delegate(int check_cell)
				{
					bool flag = false;
					if (Grid.Element[check_cell].id == elem_hash)
					{
						flag = true;
						this.DoReplaceSubstance(check_cell);
					}
					return flag;
				}, GameUtil.FloodFillVisited, null);
				break;
			}
			case DebugTool.Type.AddPressure:
				SimMessages.ModifyMass(cell, 10000f, byte.MaxValue, 0, CellEventLogger.Instance.DebugToolModifyMass, 293f, SimHashes.Oxygen);
				break;
			case DebugTool.Type.RemovePressure:
				SimMessages.ModifyMass(cell, -10000f, byte.MaxValue, 0, CellEventLogger.Instance.DebugToolModifyMass, 0f, SimHashes.Oxygen);
				break;
			case DebugTool.Type.Clear:
				this.ClearCell(cell);
				break;
			case DebugTool.Type.AddSelection:
				DebugBaseTemplateButton.Instance.AddToSelection(cell);
				break;
			case DebugTool.Type.RemoveSelection:
				DebugBaseTemplateButton.Instance.RemoveFromSelection(cell);
				break;
			case DebugTool.Type.Deconstruct:
				this.DeconstructCell(cell);
				break;
			case DebugTool.Type.Destroy:
				this.DestroyCell(cell);
				break;
			case DebugTool.Type.Sample:
				DebugPaintElementScreen.Instance.SampleCell(cell);
				break;
			}
		}
	}

	public void DoReplaceSubstance(int cell)
	{
		Element element = ((!DebugPaintElementScreen.Instance.paintElement.isOn) ? ElementLoader.elements[(int)Grid.ElementIdx[cell]] : ElementLoader.FindElementByHash(DebugPaintElementScreen.Instance.element));
		if (element == null)
		{
			element = ElementLoader.FindElementByHash(SimHashes.Vacuum);
		}
		byte b = ((!DebugPaintElementScreen.Instance.paintDisease.isOn) ? Grid.DiseaseIdx[cell] : DebugPaintElementScreen.Instance.diseaseIdx);
		float num = ((!DebugPaintElementScreen.Instance.paintTemperature.isOn) ? Grid.Temperature[cell] : DebugPaintElementScreen.Instance.temperature);
		float num2 = ((!DebugPaintElementScreen.Instance.paintMass.isOn) ? Grid.Mass[cell] : DebugPaintElementScreen.Instance.mass);
		int num3 = ((!DebugPaintElementScreen.Instance.paintDiseaseCount.isOn) ? Grid.DiseaseCount[cell] : DebugPaintElementScreen.Instance.diseaseCount);
		if (num == -1f)
		{
			num = element.defaultValues.temperature;
		}
		if (num2 == -1f)
		{
			num2 = element.defaultValues.mass;
		}
		if (DebugPaintElementScreen.Instance.affectCells.isOn)
		{
			SimMessages.ReplaceElement(cell, element.id, CellEventLogger.Instance.DebugTool, num2, num, b, num3, -1);
			if (DebugPaintElementScreen.Instance.set_prevent_fow_reveal)
			{
				Grid.Visible[cell] = 0;
				Grid.PreventFogOfWarReveal[cell] = true;
			}
			else if (DebugPaintElementScreen.Instance.set_allow_fow_reveal && Grid.PreventFogOfWarReveal[cell])
			{
				Grid.PreventFogOfWarReveal[cell] = false;
			}
		}
		if (DebugPaintElementScreen.Instance.affectBuildings.isOn)
		{
			foreach (GameObject gameObject in new List<GameObject>
			{
				Grid.Objects[cell, 1],
				Grid.Objects[cell, 2],
				Grid.Objects[cell, 9],
				Grid.Objects[cell, 16],
				Grid.Objects[cell, 12],
				Grid.Objects[cell, 16],
				Grid.Objects[cell, 24]
			})
			{
				if (gameObject != null)
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					if (num > 0f)
					{
						component.Temperature = num;
					}
					if (num3 > 0 && b != 255)
					{
						component.ModifyDiseaseCount(int.MinValue, "DebugTool.DoReplaceSubstance");
						component.AddDisease(b, num3, "DebugTool.DoReplaceSubstance");
					}
				}
			}
		}
	}

	public void DeconstructCell(int cell)
	{
		bool instantBuildMode = DebugHandler.InstantBuildMode;
		DebugHandler.InstantBuildMode = true;
		DeconstructTool.Instance.DeconstructCell(cell);
		if (!instantBuildMode)
		{
			DebugHandler.InstantBuildMode = false;
		}
	}

	public void DestroyCell(int cell)
	{
		foreach (GameObject gameObject in new List<GameObject>
		{
			Grid.Objects[cell, 2],
			Grid.Objects[cell, 1],
			Grid.Objects[cell, 12],
			Grid.Objects[cell, 16],
			Grid.Objects[cell, 0],
			Grid.Objects[cell, 24]
		})
		{
			if (gameObject != null)
			{
				global::UnityEngine.Object.Destroy(gameObject);
			}
		}
		this.ClearCell(cell);
		if (ElementLoader.elements[(int)Grid.ElementIdx[cell]].id == SimHashes.Void)
		{
			SimMessages.ReplaceElement(cell, SimHashes.Void, CellEventLogger.Instance.DebugTool, 0f, 0f, byte.MaxValue, 0, -1);
		}
		else
		{
			SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 0f, 0f, byte.MaxValue, 0, -1);
		}
	}

	public void ClearCell(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		List<ScenePartitionerEntry> list = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(vector2I.x, vector2I.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, list);
		for (int i = 0; i < list.Count; i++)
		{
			ScenePartitionerEntry scenePartitionerEntry = list[i];
			Pickupable pickupable = scenePartitionerEntry.obj as Pickupable;
			if (pickupable != null && pickupable.GetComponent<MinionBrain>() == null)
			{
				Util.KDestroyGameObject(pickupable.gameObject);
			}
		}
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.Free(list);
	}

	public static DebugTool Instance;

	public DebugTool.Type type;

	public enum Type
	{
		Dig,
		Heat,
		Cool,
		ReplaceSubstance,
		FillReplaceSubstance,
		AddPressure,
		RemovePressure,
		PaintPlant,
		Clear,
		AddSelection,
		RemoveSelection,
		Deconstruct,
		Destroy,
		Sample
	}
}
