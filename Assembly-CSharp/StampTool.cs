using System;
using System.Collections.Generic;
using TemplateClasses;
using UnityEngine;

public class StampTool : InterfaceTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		StampTool.Instance = this;
	}

	public void Activate(TemplateContainer template, bool SelectAffected = false, bool DeactivateOnStamp = false)
	{
		this.stampTemplate = template;
		PlayerController.Instance.ActivateTool(this);
		this.selectAffected = SelectAffected;
		this.deactivateOnStamp = DeactivateOnStamp;
	}

	private void Update()
	{
		this.RefreshPreview(Grid.PosToCell(this.GetCursorPos()));
	}

	private Vector3 GetCursorPos()
	{
		return PlayerController.GetCursorPos(Input.mousePosition);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		this.Stamp(cursor_pos);
	}

	private void Stamp(Vector2 pos)
	{
		if (!this.ready)
		{
			return;
		}
		this.ready = false;
		bool pauseOnComplete = SpeedControlScreen.Instance.IsPaused;
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(true);
		}
		List<GameObject> objects_to_destroy = new List<GameObject>();
		for (int i = 0; i < this.stampTemplate.cells.Count; i++)
		{
			for (int j = 0; j < 20; j++)
			{
				GameObject gameObject = Grid.Objects[Grid.XYToCell((int)(pos.x + (float)this.stampTemplate.cells[i].location_x), (int)(pos.y + (float)this.stampTemplate.cells[i].location_y)), j];
				if (gameObject != null && !objects_to_destroy.Contains(gameObject))
				{
					objects_to_destroy.Add(gameObject);
				}
			}
		}
		TemplateLoader.Stamp(this.stampTemplate, pos, delegate
		{
			this.CompleteStamp(pauseOnComplete, objects_to_destroy);
		});
		if (this.selectAffected)
		{
			DebugBaseTemplateButton.Instance.ClearSelection();
			for (int k = 0; k < this.stampTemplate.cells.Count; k++)
			{
				DebugBaseTemplateButton.Instance.AddToSelection(Grid.XYToCell((int)(pos.x + (float)this.stampTemplate.cells[k].location_x), (int)(pos.y + (float)this.stampTemplate.cells[k].location_y)));
			}
		}
		if (this.deactivateOnStamp)
		{
			base.DeactivateTool(null);
		}
	}

	private void CompleteStamp(bool pause, List<GameObject> objects_to_destroy = null)
	{
		if (objects_to_destroy != null)
		{
			foreach (GameObject gameObject in objects_to_destroy)
			{
				if (gameObject != null)
				{
					Util.KDestroyGameObject(gameObject);
				}
			}
		}
		if (pause)
		{
			SpeedControlScreen.Instance.Pause(true);
		}
		this.ready = true;
		this.OnDeactivateTool(null);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		this.RefreshPreview(Grid.InvalidCell);
	}

	public void RefreshPreview(int new_placement_cell)
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		foreach (Cell cell in this.stampTemplate.cells)
		{
			if (this.placementCell != Grid.InvalidCell)
			{
				list.Add(Grid.OffsetCell(this.placementCell, new CellOffset(cell.location_x, cell.location_y)));
			}
			if (new_placement_cell != Grid.InvalidCell)
			{
				list2.Add(Grid.OffsetCell(new_placement_cell, new CellOffset(cell.location_x, cell.location_y)));
			}
		}
		this.placementCell = new_placement_cell;
		foreach (int num in list)
		{
			if (!list2.Contains(num))
			{
				GameObject gameObject = Grid.Objects[num, 6];
				if (gameObject != null)
				{
					gameObject.DeleteObject();
				}
			}
		}
		foreach (int num2 in list2)
		{
			if (!list.Contains(num2) && Grid.Objects[num2, 6] == null)
			{
				GameObject gameObject2 = Util.KInstantiate(this.PlacerPrefab, SceneOrganizer.Instance.GetFolder(Folder.Placers), null);
				Grid.Objects[num2, 6] = gameObject2;
				Vector3 vector = Grid.CellToPosCBC(num2, this.visualizerLayer);
				float depthBias = InterfaceTool.DepthBias;
				vector.z += depthBias;
				gameObject2.transform.SetPosition(vector);
			}
		}
	}

	public static StampTool Instance;

	public TemplateContainer stampTemplate;

	public GameObject PlacerPrefab;

	private bool ready = true;

	private int placementCell = Grid.InvalidCell;

	private bool selectAffected;

	private bool deactivateOnStamp;
}
