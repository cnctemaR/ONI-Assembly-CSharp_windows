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
		Game.Instance.Trigger(1174281782, this);
	}

	public virtual bool ShowHoverUI()
	{
		bool flag = false;
		global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
		if (current != null)
		{
			Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
			current.RaycastAll(new PointerEventData(current)
			{
				position = vector
			}, this.castResults);
			flag = this.castResults.Count == 0;
		}
		return flag;
	}

	protected virtual void OnActivateTool()
	{
		if (OverlayScreen.Instance != null && this.viewMode != SimViewMode.None && OverlayScreen.Instance.mode == SimViewMode.None)
		{
			OverlayScreen.Instance.ToggleOverlay(this.viewMode);
			InterfaceTool.toolActivatedViewMode = this.viewMode;
		}
		this.SetCursor(this.cursor, this.cursorOffset, CursorMode.Auto);
	}

	public void DeactivateTool(InterfaceTool new_tool = null)
	{
		this.OnDeactivateTool(new_tool);
		if ((new_tool == null || new_tool == SelectTool.Instance) && InterfaceTool.toolActivatedViewMode != SimViewMode.None)
		{
			OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
			InterfaceTool.toolActivatedViewMode = SimViewMode.None;
		}
	}

	protected virtual void OnDeactivateTool(InterfaceTool new_tool)
	{
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
		this.visualizer.transform.SetLocalPosition(cursor_pos);
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
	}

	public virtual void OnRightClickUp(Vector3 cursor_pos)
	{
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

	protected void UpdateHoverElements(List<KSelectable> hits)
	{
		HoverTextConfiguration component = base.GetComponent<HoverTextConfiguration>();
		if (component != null)
		{
			component.UpdateHoverElements(hits);
		}
	}

	public virtual void LateUpdate()
	{
		this.UpdateHoverElements(null);
	}

	public void SetLinkCursor(bool set)
	{
		this.SetCursor((!set) ? this.cursor : Assets.GetTexture("cursor_hand"), (!set) ? this.cursorOffset : Vector2.zero, CursorMode.Auto);
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

	public Vector2 cursorOffset = new Vector2(2f, 2f);

	public global::System.Action OnDeactivate;

	private static Texture2D activeCursor;

	private static SimViewMode toolActivatedViewMode;

	protected SimViewMode viewMode;

	private List<RaycastResult> castResults = new List<RaycastResult>();

	private bool isAppFocused = true;
}
