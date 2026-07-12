using System;
using System.Collections.Generic;
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
		this.preview = new StampToolPreview(this, new IStampToolPreviewPlugin[]
		{
			new StampToolPreview_Placers(this.PlacerPrefab),
			new StampToolPreview_Area(),
			new StampToolPreview_SolidLiquidGas(),
			new StampToolPreview_Prefabs()
		});
	}

	private void Update()
	{
		this.preview.Refresh(Grid.PosToCell(this.GetCursorPos()));
	}

	public void Activate(TemplateContainer template, bool SelectAffected = false, bool DeactivateOnStamp = false)
	{
		this.selectAffected = SelectAffected;
		this.deactivateOnStamp = DeactivateOnStamp;
		if (this.stampTemplate == template || template == null || template.cells == null)
		{
			return;
		}
		this.stampTemplate = template;
		PlayerController.Instance.ActivateTool(this);
		base.StartCoroutine(this.preview.Setup(template));
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
			this.preview.OnPlace();
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
			foreach (GameObject gameObject2 in list)
			{
				if (gameObject2 != null)
				{
					Util.KDestroyGameObject(gameObject2);
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
		if (base.gameObject.activeSelf)
		{
			return;
		}
		this.preview.Cleanup();
		this.stampTemplate = null;
	}

	public static StampTool Instance;

	private StampToolPreview preview;

	public TemplateContainer stampTemplate;

	public GameObject PlacerPrefab;

	private bool ready = true;

	private bool selectAffected;

	private bool deactivateOnStamp;
}
