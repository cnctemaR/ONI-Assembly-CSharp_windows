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
			mouse_pos.z = -Camera.main.transform.position.z - Grid.CellSizeInMeters;
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
		HoverTextScreen.Instance.ClearLabels();
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

	public void OnKeyDown(KButtonEvent e)
	{
		if (this.activeTool == null || !this.activeTool.enabled)
		{
			return;
		}
		List<RaycastResult> list = new List<RaycastResult>();
		PointerEventData pointerEventData = new PointerEventData(global::UnityEngine.EventSystems.EventSystem.current);
		pointerEventData.position = Input.mousePosition;
		global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
		if (current == null)
		{
			return;
		}
		current.RaycastAll(pointerEventData, list);
		if (list.Count > 0)
		{
			return;
		}
		if (e.TryConsume(global::Action.MouseLeft))
		{
			this.activeTool.OnLeftClickDown(this.GetCursorPos());
		}
		else if (e.IsAction(global::Action.MouseRight))
		{
			this.activeTool.OnRightClickDown(this.GetCursorPos(), e);
		}
		else
		{
			this.activeTool.OnKeyDown(e);
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (this.activeTool == null || !this.activeTool.enabled)
		{
			return;
		}
		if (!this.activeTool.hasFocus)
		{
			return;
		}
		if (e.TryConsume(global::Action.MouseLeft))
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

	public InterfaceTool[] tools;

	private InterfaceTool activeTool;

	private bool DebugHidingCursor;

	private Vector3 prevMousePos = new Vector3(float.PositiveInfinity, 0f, 0f);
}
