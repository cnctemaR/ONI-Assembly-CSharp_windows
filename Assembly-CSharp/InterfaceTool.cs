using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InterfaceTool : KMonoBehaviour
{
	public SimViewMode ViewMode
	{
		get
		{
			return this.viewMode;
		}
	}

	public void ActivateTool()
	{
		this.OnActivateTool();
		this.OnMouseMove(PlayerController.GetCursorPos(Input.mousePosition));
	}

	public virtual bool ShowHoverUI()
	{
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
		global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
		current.RaycastAll(new PointerEventData(current)
		{
			position = vector
		}, this.castResults);
		return this.castResults.Count == 0;
	}

	public void DeactivateTool(InterfaceTool new_tool = null)
	{
		this.OnDeactivateTool(new_tool);
	}

	protected virtual void OnActivateTool()
	{
		if (OverlayScreen.Instance != null && this.viewMode != OverlayScreen.Instance.GetMode() && this.viewMode != SimViewMode.Ignore)
		{
			Game.Instance.Trigger(1248612973, this.viewMode);
		}
		this.SetCursor(this.cursor, this.cursorOffset, CursorMode.Auto);
	}

	protected virtual void OnDeactivateTool(InterfaceTool new_tool)
	{
		if (new_tool == null)
		{
			return;
		}
		if (this.viewMode == SimViewMode.None && new_tool.viewMode == SimViewMode.Ignore)
		{
			return;
		}
		if (OverlayScreen.Instance != null && this.viewMode != new_tool.viewMode && this.viewMode != SimViewMode.Ignore)
		{
			Game.Instance.Trigger(2015652040, this.viewMode);
		}
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		this.isAppFocused = focusStatus;
	}

	public virtual string GetDeactivateSound()
	{
		return "Tile_Cancel";
	}

	public virtual void OnMouseMove(Vector3 cursor_pos)
	{
		if (this.visualizer == null || !this.isAppFocused)
		{
			return;
		}
		int num = Grid.PosToCell(cursor_pos);
		cursor_pos = Grid.CellToPosCBC(num, this.visualizerLayer);
		cursor_pos.z += InterfaceTool.DepthBias;
		this.visualizer.transform.localPosition = cursor_pos;
	}

	public virtual void OnKeyDown(KButtonEvent e)
	{
	}

	public virtual void OnKeyUp(KButtonEvent e)
	{
	}

	public virtual void OnLeftClickDown(Vector3 cursor_pos)
	{
	}

	public virtual void OnLeftClickUp(Vector3 cursor_pos)
	{
	}

	public virtual void OnRightClickDown(Vector3 cursor_pos, KButtonEvent e)
	{
		this.viewportDownPos = Camera.main.WorldToViewportPoint(cursor_pos);
	}

	public virtual void OnRightClickUp(Vector3 cursor_pos)
	{
		Vector3 vector = Camera.main.WorldToViewportPoint(cursor_pos);
		float magnitude = (vector - this.viewportDownPos).magnitude;
		if (magnitude <= 0.02f)
		{
		}
	}

	public virtual void OnFocus(bool focus)
	{
		if (this.visualizer != null)
		{
			this.visualizer.SetActive(focus);
		}
		this.hasFocus = focus;
	}

	protected Vector2 GetRegularizedPos(Vector2 input, bool minimize)
	{
		Vector3 vector = new Vector3(Grid.HalfCellSizeInMeters, Grid.HalfCellSizeInMeters, 0f);
		int num = Grid.PosToCell(input);
		return Grid.CellToPosCCC(num, Grid.SceneLayer.Background) + ((!minimize) ? vector : (-vector));
	}

	protected void SetCursor(Texture2D new_cursor, Vector2 offset, CursorMode mode)
	{
		if (new_cursor != InterfaceTool.activeCursor)
		{
			Cursor.SetCursor(new_cursor, offset, mode);
		}
	}

	public const float MaxClickDistance = 0.02f;

	public static float DepthBias = -0.15f;

	public GameObject visualizer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	public string placeSound;

	[NonSerialized]
	public bool hasFocus;

	[SerializeField]
	protected Texture2D cursor;

	protected Vector2 cursorOffset = new Vector2(8f, 8f);

	public global::System.Action OnDeactivate;

	protected HoverTextConfiguration hoverText;

	private static Texture2D activeCursor;

	protected SimViewMode viewMode = SimViewMode.Ignore;

	private List<RaycastResult> castResults = new List<RaycastResult>();

	private bool isAppFocused = true;

	protected Vector3 viewportDownPos;
}
