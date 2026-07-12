using System;
using System.Collections.Generic;
using TemplateClasses;
using UnityEngine;

public class StampTool : InterfaceTool
{
	public static void DestroyInstance()
	{
		StampTool.Instance = null;
	}

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
		return PlayerController.GetCursorPos(KInputManager.GetMousePos());
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
		int num = Grid.OffsetCell(Grid.PosToCell(pos), Mathf.FloorToInt(-this.stampTemplate.info.size.X / 2f), 0);
		int num2 = Grid.OffsetCell(Grid.PosToCell(pos), Mathf.FloorToInt(this.stampTemplate.info.size.X / 2f), 0);
		int num3 = Grid.OffsetCell(Grid.PosToCell(pos), 0, 1 + Mathf.FloorToInt(-this.stampTemplate.info.size.Y / 2f));
		int num4 = Grid.OffsetCell(Grid.PosToCell(pos), 0, 1 + Mathf.FloorToInt(this.stampTemplate.info.size.Y / 2f));
		if (!Grid.IsValidBuildingCell(num) || !Grid.IsValidBuildingCell(num2) || !Grid.IsValidBuildingCell(num4) || !Grid.IsValidBuildingCell(num3))
		{
			return;
		}
		this.ready = false;
		bool pauseOnComplete = SpeedControlScreen.Instance.IsPaused;
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(true);
		}
		if (this.stampTemplate.cells != null)
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.stampTemplate.cells.Count; i++)
			{
				for (int j = 0; j < 34; j++)
				{
					GameObject gameObject = Grid.Objects[Grid.XYToCell((int)(pos.x + (float)this.stampTemplate.cells[i].location_x), (int)(pos.y + (float)this.stampTemplate.cells[i].location_y)), j];
					if (gameObject != null && !list.Contains(gameObject))
					{
						list.Add(gameObject);
					}
				}
			}
			if (list != null)
			{
				foreach (GameObject gameObject2 in list)
				{
					if (gameObject2 != null)
					{
						Util.KDestroyGameObject(gameObject2);
					}
				}
			}
		}
		TemplateLoader.Stamp(this.stampTemplate, pos, delegate
		{
			this.CompleteStamp(pauseOnComplete);
		});
		if (this.selectAffected)
		{
			DebugBaseTemplateButton.Instance.ClearSelection();
			if (this.stampTemplate.cells != null)
			{
				for (int k = 0; k < this.stampTemplate.cells.Count; k++)
				{
					DebugBaseTemplateButton.Instance.AddToSelection(Grid.XYToCell((int)(pos.x + (float)this.stampTemplate.cells[k].location_x), (int)(pos.y + (float)this.stampTemplate.cells[k].location_y)));
				}
			}
		}
		if (this.deactivateOnStamp)
		{
			base.DeactivateTool(null);
		}
	}

	private void CompleteStamp(bool pause)
	{
		if (pause)
		{
			SpeedControlScreen.Instance.Pause(true, false);
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
		if (this.stampTemplate.cells != null)
		{
			foreach (Cell cell in this.stampTemplate.cells)
			{
				if (this.placementCell != Grid.InvalidCell)
				{
					int num = Grid.OffsetCell(this.placementCell, new CellOffset(cell.location_x, cell.location_y));
					if (Grid.IsValidCell(num))
					{
						list.Add(num);
					}
				}
				if (new_placement_cell != Grid.InvalidCell)
				{
					int num2 = Grid.OffsetCell(new_placement_cell, new CellOffset(cell.location_x, cell.location_y));
					if (Grid.IsValidCell(num2))
					{
						list2.Add(num2);
					}
				}
			}
		}
		this.placementCell = new_placement_cell;
		foreach (int num3 in list)
		{
			if (!list2.Contains(num3))
			{
				GameObject gameObject = Grid.Objects[num3, 6];
				if (gameObject != null)
				{
					gameObject.DeleteObject();
				}
			}
		}
		foreach (int num4 in list2)
		{
			if (!list.Contains(num4) && Grid.Objects[num4, 6] == null)
			{
				GameObject gameObject2 = Util.KInstantiate(this.PlacerPrefab, null, null);
				Grid.Objects[num4, 6] = gameObject2;
				Vector3 vector = Grid.CellToPosCBC(num4, this.visualizerLayer);
				float num5 = -0.15f;
				vector.z += num5;
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
