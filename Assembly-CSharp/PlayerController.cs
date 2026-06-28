using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : KMonoBehaviour, IInputHandler
{
	public KInputHandler inputHandler { get; set; }

	public InterfaceTool ActiveTool
	{
		get
		{
			return this.activeTool;
		}
	}

	public static PlayerController Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		PlayerController.Instance = this;
		for (int i = 0; i < this.tools.Length; i++)
		{
			GameObject gameObject = Util.KInstantiate(this.tools[i].gameObject, base.gameObject, null);
			this.tools[i] = gameObject.GetComponent<InterfaceTool>();
			this.tools[i].gameObject.SetActive(true);
			this.tools[i].gameObject.SetActive(false);
		}
	}

	protected override void OnSpawn()
	{
		this.ActivateTool(this.tools[0]);
	}

	private Vector3 GetCursorPos()
	{
		return PlayerController.GetCursorPos(Input.mousePosition);
	}

	public static Vector3 GetCursorPos(Vector3 mouse_pos)
	{
		Ray ray = Camera.main.ScreenPointToRay(mouse_pos);
		RaycastHit raycastHit;
		Vector3 vector;
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity, Game.BlockSelectionLayerMask))
		{
			vector = raycastHit.point;
		}
		else
		{
			mouse_pos.z = -Camera.main.transform.GetPosition().z - Grid.CellSizeInMeters;
			vector = Camera.main.ScreenToWorldPoint(mouse_pos);
		}
		float num = vector.x;
		float num2 = vector.y;
		num = Mathf.Max(num, 0f);
		num = Mathf.Min(num, Grid.WidthInMeters);
		num2 = Mathf.Max(num2, 0f);
		num2 = Mathf.Min(num2, Grid.HeightInMeters);
		vector.x = num;
		vector.y = num2;
		return vector;
	}

	private void UpdateHover()
	{
		global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
		if (current != null)
		{
			this.activeTool.OnFocus(!current.IsPointerOverGameObject());
		}
	}

	private void Update()
	{
		this.UpdateDrag();
		if (this.activeTool && this.activeTool.enabled)
		{
			this.UpdateHover();
			Vector3 cursorPos = this.GetCursorPos();
			if (cursorPos != this.prevMousePos)
			{
				this.prevMousePos = cursorPos;
				this.activeTool.OnMouseMove(cursorPos);
			}
		}
		if (Input.GetKeyDown(KeyCode.F12) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
		{
			this.DebugHidingCursor = !this.DebugHidingCursor;
			Cursor.visible = !this.DebugHidingCursor;
			HoverTextScreen.Instance.Show(!this.DebugHidingCursor);
		}
	}

	private void LateUpdate()
	{
		if (this.queueStopDrag)
		{
			this.queueStopDrag = false;
			this.dragging = false;
			this.dragAction = global::Action.Invalid;
			this.dragDelta = Vector3.zero;
			this.worldDragDelta = Vector3.zero;
		}
	}

	public void ActivateTool(InterfaceTool tool)
	{
		if (this.activeTool == tool)
		{
			return;
		}
		this.DeactivateTool(tool);
		this.activeTool = tool;
		this.activeTool.enabled = true;
		this.activeTool.gameObject.SetActive(true);
		this.activeTool.ActivateTool();
		this.UpdateHover();
	}

	public void ToolDeactivated(InterfaceTool tool)
	{
		if (this.activeTool == tool && this.activeTool != null)
		{
			this.DeactivateTool(null);
		}
		if (this.activeTool == null)
		{
			this.ActivateTool(SelectTool.Instance);
		}
	}

	private void DeactivateTool(InterfaceTool new_tool = null)
	{
		if (this.activeTool != null)
		{
			this.activeTool.enabled = false;
			this.activeTool.gameObject.SetActive(false);
			InterfaceTool interfaceTool = this.activeTool;
			this.activeTool = null;
			interfaceTool.DeactivateTool(new_tool);
		}
	}

	public bool IsUsingDefaultTool()
	{
		return this.activeTool == this.tools[0];
	}

	private void StartDrag(global::Action action)
	{
		if (this.dragAction == global::Action.Invalid)
		{
			this.dragAction = action;
			this.startDragPos = Input.mousePosition;
			this.startDragTime = Time.unscaledTime;
		}
	}

	private void UpdateDrag()
	{
		this.dragDelta = Vector2.zero;
		Vector3 mousePosition = Input.mousePosition;
		if (!this.dragging && this.dragAction != global::Action.Invalid && ((mousePosition - this.startDragPos).magnitude > 6f || Time.unscaledTime - this.startDragTime > 0.3f))
		{
			this.dragging = true;
		}
		if (this.dragging)
		{
			this.dragDelta = mousePosition - this.startDragPos;
			this.worldDragDelta = Camera.main.ScreenToWorldPoint(mousePosition) - Camera.main.ScreenToWorldPoint(this.startDragPos);
			this.startDragPos = mousePosition;
		}
	}

	private void StopDrag(global::Action action)
	{
		if (this.dragAction == action)
		{
			this.queueStopDrag = true;
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.ToggleScreenshotMode))
		{
			DebugHandler.ToggleScreenshotMode();
			return;
		}
		if (this.activeTool == null || !this.activeTool.enabled)
		{
			return;
		}
		List<RaycastResult> list = new List<RaycastResult>();
		PointerEventData pointerEventData = new PointerEventData(global::UnityEngine.EventSystems.EventSystem.current);
		pointerEventData.position = Input.mousePosition;
		global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
		if (current != null)
		{
			current.RaycastAll(pointerEventData, list);
			if (list.Count > 0)
			{
				return;
			}
		}
		if (e.TryConsume(global::Action.MouseLeft) || e.TryConsume(global::Action.ShiftMouseLeft))
		{
			this.StartDrag(global::Action.MouseLeft);
			this.activeTool.OnLeftClickDown(this.GetCursorPos());
		}
		else if (e.IsAction(global::Action.MouseRight))
		{
			this.StartDrag(global::Action.MouseRight);
			this.activeTool.OnRightClickDown(this.GetCursorPos(), e);
		}
		else if (e.IsAction(global::Action.MouseMiddle))
		{
			this.StartDrag(global::Action.MouseMiddle);
		}
		else
		{
			this.activeTool.OnKeyDown(e);
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (e.IsAction(global::Action.MouseLeft) || e.IsAction(global::Action.ShiftMouseLeft))
		{
			this.StopDrag(global::Action.MouseLeft);
		}
		else if (e.IsAction(global::Action.MouseRight))
		{
			this.StopDrag(global::Action.MouseRight);
		}
		else if (e.IsAction(global::Action.MouseMiddle))
		{
			this.StopDrag(global::Action.MouseMiddle);
		}
		if (this.activeTool == null || !this.activeTool.enabled)
		{
			return;
		}
		if (!this.activeTool.hasFocus)
		{
			return;
		}
		if (e.TryConsume(global::Action.MouseLeft) || e.TryConsume(global::Action.ShiftMouseLeft))
		{
			this.activeTool.OnLeftClickUp(this.GetCursorPos());
		}
		else if (e.IsAction(global::Action.MouseRight))
		{
			this.activeTool.OnRightClickUp(this.GetCursorPos());
		}
		else
		{
			this.activeTool.OnKeyUp(e);
		}
	}

	public bool ConsumeIfNotDragging(KButtonEvent e, global::Action action)
	{
		return (this.dragAction != action || !this.dragging) && e.TryConsume(action);
	}

	public bool IsDragging()
	{
		return this.dragAction != global::Action.Invalid;
	}

	public Vector3 GetDragDelta()
	{
		return this.dragDelta;
	}

	public Vector3 GetWorldDragDelta()
	{
		return this.worldDragDelta;
	}

	public InterfaceTool[] tools;

	private InterfaceTool activeTool;

	private bool DebugHidingCursor;

	private Vector3 prevMousePos = new Vector3(float.PositiveInfinity, 0f, 0f);

	private const float MIN_DRAG_DIST = 6f;

	private const float MIN_DRAG_TIME = 0.3f;

	private global::Action dragAction;

	private bool dragging;

	private bool queueStopDrag;

	private Vector3 startDragPos;

	private float startDragTime;

	private Vector3 dragDelta;

	private Vector3 worldDragDelta;
}
