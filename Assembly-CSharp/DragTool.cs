using System;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class DragTool : InterfaceTool
{
	public bool Dragging
	{
		get
		{
			return this.dragging;
		}
	}

	protected virtual DragTool.Mode GetMode()
	{
		return this.mode;
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		this.dragging = false;
		this.SetMode(this.mode);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		KScreenManager.Instance.SetEventSystemEnabled(true);
		if (this.areaVisualizerText != Guid.Empty)
		{
			NameDisplayScreen.Instance.RemoveWorldText(this.areaVisualizerText);
			this.areaVisualizerText = Guid.Empty;
		}
		base.OnDeactivateTool(new_tool);
	}

	protected override void OnPrefabInit()
	{
		Game.Instance.Subscribe(1634669191, new Action<object>(this.OnTutorialOpened));
		base.OnPrefabInit();
		if (this.visualizer != null)
		{
			this.visualizer = global::Util.KInstantiate(this.visualizer, null, null);
		}
		if (this.areaVisualizer != null)
		{
			this.areaVisualizer = global::Util.KInstantiate(this.areaVisualizer, null, null);
			this.areaVisualizer.SetActive(false);
			this.areaVisualizerSpriteRenderer = this.areaVisualizer.GetComponent<SpriteRenderer>();
			this.areaVisualizer.transform.SetParent(base.transform);
			this.areaVisualizer.GetComponent<Renderer>().material.color = this.areaColour;
		}
	}

	protected override void OnCmpEnable()
	{
		this.dragging = false;
	}

	protected override void OnCmpDisable()
	{
		if (this.visualizer != null)
		{
			this.visualizer.SetActive(false);
		}
		if (this.areaVisualizer != null)
		{
			this.areaVisualizer.SetActive(false);
		}
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		cursor_pos = this.ClampPositionToWorld(cursor_pos, ClusterManager.Instance.activeWorld);
		this.dragging = true;
		this.downPos = cursor_pos;
		this.previousCursorPos = cursor_pos;
		if (!KInputManager.currentControllerIsGamepad)
		{
			KScreenManager.Instance.SetEventSystemEnabled(false);
		}
		this.hasFocus = true;
		if (this.areaVisualizerTextPrefab != null)
		{
			this.areaVisualizerText = NameDisplayScreen.Instance.AddWorldText("", this.areaVisualizerTextPrefab);
			NameDisplayScreen.Instance.GetWorldText(this.areaVisualizerText).GetComponent<LocText>().color = this.areaColour;
		}
		DragTool.Mode mode = this.GetMode();
		if (mode == DragTool.Mode.Brush)
		{
			if (this.visualizer != null)
			{
				this.AddDragPoint(cursor_pos);
				return;
			}
		}
		else if (mode == DragTool.Mode.Box)
		{
			if (this.visualizer != null)
			{
				this.visualizer.SetActive(false);
			}
			if (this.areaVisualizer != null)
			{
				this.areaVisualizer.SetActive(true);
				this.areaVisualizer.transform.SetPosition(cursor_pos);
				this.areaVisualizerSpriteRenderer.size = new Vector2(0.01f, 0.01f);
			}
		}
	}

	public void CancelDragging()
	{
		KScreenManager.Instance.SetEventSystemEnabled(true);
		this.dragAxis = DragTool.DragAxis.Invalid;
		if (!this.dragging)
		{
			return;
		}
		this.dragging = false;
		if (this.areaVisualizerText != Guid.Empty)
		{
			NameDisplayScreen.Instance.RemoveWorldText(this.areaVisualizerText);
			this.areaVisualizerText = Guid.Empty;
		}
		if (this.GetMode() == DragTool.Mode.Box && this.areaVisualizer != null)
		{
			this.areaVisualizer.SetActive(false);
		}
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		cursor_pos = this.ClampPositionToWorld(cursor_pos, ClusterManager.Instance.activeWorld);
		KScreenManager.Instance.SetEventSystemEnabled(true);
		this.dragAxis = DragTool.DragAxis.Invalid;
		if (!this.dragging)
		{
			return;
		}
		this.dragging = false;
		if (this.areaVisualizerText != Guid.Empty)
		{
			NameDisplayScreen.Instance.RemoveWorldText(this.areaVisualizerText);
			this.areaVisualizerText = Guid.Empty;
		}
		if (this.GetMode() == DragTool.Mode.Box && this.areaVisualizer != null)
		{
			this.areaVisualizer.SetActive(false);
			int num;
			int num2;
			Grid.PosToXY(this.downPos, out num, out num2);
			int num3 = num;
			int num4 = num2;
			int num5;
			int num6;
			Grid.PosToXY(cursor_pos, out num5, out num6);
			if (num5 < num)
			{
				global::Util.Swap<int>(ref num, ref num5);
			}
			if (num6 < num2)
			{
				global::Util.Swap<int>(ref num2, ref num6);
			}
			for (int i = num2; i <= num6; i++)
			{
				for (int j = num; j <= num5; j++)
				{
					int num7 = Grid.XYToCell(j, i);
					if (Grid.IsValidCell(num7) && Grid.IsVisible(num7))
					{
						int num8 = i - num4;
						int num9 = j - num3;
						num8 = Mathf.Abs(num8);
						num9 = Mathf.Abs(num9);
						this.OnDragTool(num7, num8 + num9);
					}
				}
			}
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound(this.GetConfirmSound(), false));
			this.OnDragComplete(this.downPos, cursor_pos);
		}
	}

	protected virtual string GetConfirmSound()
	{
		return "Tile_Confirm";
	}

	protected virtual string GetDragSound()
	{
		return "Tile_Drag";
	}

	public override string GetDeactivateSound()
	{
		return "Tile_Cancel";
	}

	protected Vector3 ClampPositionToWorld(Vector3 position, WorldContainer world)
	{
		position.x = Mathf.Clamp(position.x, world.minimumBounds.x, world.maximumBounds.x);
		position.y = Mathf.Clamp(position.y, world.minimumBounds.y, world.maximumBounds.y);
		return position;
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		cursorPos = this.ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
		if (this.dragging)
		{
			if (Input.GetKey((KeyCode)Global.Instance.GetInputManager().GetDefaultController().GetInputForAction(global::Action.DragStraight)))
			{
				Vector3 vector = cursorPos - this.downPos;
				if ((this.canChangeDragAxis || this.dragAxis == DragTool.DragAxis.Invalid) && vector.sqrMagnitude > 0.707f)
				{
					if (Mathf.Abs(vector.x) < Mathf.Abs(vector.y))
					{
						this.dragAxis = DragTool.DragAxis.Vertical;
					}
					else
					{
						this.dragAxis = DragTool.DragAxis.Horizontal;
					}
				}
			}
			else
			{
				this.dragAxis = DragTool.DragAxis.Invalid;
			}
			DragTool.DragAxis dragAxis = this.dragAxis;
			if (dragAxis != DragTool.DragAxis.Horizontal)
			{
				if (dragAxis == DragTool.DragAxis.Vertical)
				{
					cursorPos.x = this.downPos.x;
				}
			}
			else
			{
				cursorPos.y = this.downPos.y;
			}
		}
		base.OnMouseMove(cursorPos);
		if (!this.dragging)
		{
			return;
		}
		DragTool.Mode mode = this.GetMode();
		if (mode != DragTool.Mode.Brush)
		{
			if (mode == DragTool.Mode.Box)
			{
				Vector2 vector2 = Vector3.Max(this.downPos, cursorPos);
				Vector2 vector3 = Vector3.Min(this.downPos, cursorPos);
				vector2 = base.GetWorldRestrictedPosition(vector2);
				vector3 = base.GetWorldRestrictedPosition(vector3);
				vector2 = base.GetRegularizedPos(vector2, false);
				vector3 = base.GetRegularizedPos(vector3, true);
				Vector2 vector4 = vector2 - vector3;
				Vector2 vector5 = (vector2 + vector3) * 0.5f;
				this.areaVisualizer.transform.SetPosition(new Vector2(vector5.x, vector5.y));
				int num = (int)(vector2.x - vector3.x + (vector2.y - vector3.y) - 1f);
				if (this.areaVisualizerSpriteRenderer.size != vector4)
				{
					string sound = GlobalAssets.GetSound(this.GetDragSound(), false);
					if (sound != null)
					{
						Vector3 position = this.areaVisualizer.transform.GetPosition();
						position.z = 0f;
						EventInstance eventInstance = SoundEvent.BeginOneShot(sound, position, 1f, false);
						eventInstance.setParameterByName("tileCount", (float)num, false);
						SoundEvent.EndOneShot(eventInstance);
					}
				}
				this.areaVisualizerSpriteRenderer.size = vector4;
				if (this.areaVisualizerText != Guid.Empty)
				{
					Vector2I vector2I = new Vector2I(Mathf.RoundToInt(vector4.x), Mathf.RoundToInt(vector4.y));
					LocText component = NameDisplayScreen.Instance.GetWorldText(this.areaVisualizerText).GetComponent<LocText>();
					component.text = string.Format(UI.TOOLS.TOOL_AREA_FMT, vector2I.x, vector2I.y, vector2I.x * vector2I.y);
					Vector2 vector6 = vector5;
					component.transform.SetPosition(vector6);
				}
			}
		}
		else
		{
			this.AddDragPoints(cursorPos, this.previousCursorPos);
			if (this.areaVisualizerText != Guid.Empty)
			{
				int dragLength = this.GetDragLength();
				LocText component2 = NameDisplayScreen.Instance.GetWorldText(this.areaVisualizerText).GetComponent<LocText>();
				component2.text = string.Format(UI.TOOLS.TOOL_LENGTH_FMT, dragLength);
				Vector3 vector7 = Grid.CellToPos(Grid.PosToCell(cursorPos));
				vector7 += new Vector3(0f, 1f, 0f);
				component2.transform.SetPosition(vector7);
			}
		}
		this.previousCursorPos = cursorPos;
	}

	protected virtual void OnDragTool(int cell, int distFromOrigin)
	{
	}

	protected virtual void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp)
	{
	}

	protected virtual int GetDragLength()
	{
		return 0;
	}

	private void AddDragPoint(Vector3 cursorPos)
	{
		cursorPos = this.ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
		int num = Grid.PosToCell(cursorPos);
		if (Grid.IsValidCell(num) && Grid.IsVisible(num))
		{
			this.OnDragTool(num, 0);
		}
	}

	private void AddDragPoints(Vector3 cursorPos, Vector3 previousCursorPos)
	{
		cursorPos = this.ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
		Vector3 vector = cursorPos - previousCursorPos;
		float magnitude = vector.magnitude;
		float num = Grid.CellSizeInMeters * 0.25f;
		int num2 = 1 + (int)(magnitude / num);
		vector.Normalize();
		for (int i = 0; i < num2; i++)
		{
			Vector3 vector2 = previousCursorPos + vector * ((float)i * num);
			this.AddDragPoint(vector2);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.interceptNumberKeysForPriority)
		{
			this.HandlePriortyKeysDown(e);
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (this.interceptNumberKeysForPriority)
		{
			this.HandlePriorityKeysUp(e);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private void HandlePriortyKeysDown(KButtonEvent e)
	{
		global::Action action = e.GetAction();
		if (global::Action.Plan1 > action || action > global::Action.Plan10 || !e.TryConsume(action))
		{
			return;
		}
		int num = action - global::Action.Plan1 + 1;
		if (num <= 9)
		{
			ToolMenu.Instance.PriorityScreen.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, num), true);
			return;
		}
		ToolMenu.Instance.PriorityScreen.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.topPriority, 1), true);
	}

	private void HandlePriorityKeysUp(KButtonEvent e)
	{
		global::Action action = e.GetAction();
		if (global::Action.Plan1 <= action && action <= global::Action.Plan10)
		{
			e.TryConsume(action);
		}
	}

	protected void SetMode(DragTool.Mode newMode)
	{
		this.mode = newMode;
		DragTool.Mode mode = this.mode;
		if (mode == DragTool.Mode.Brush)
		{
			if (this.areaVisualizer != null)
			{
				this.areaVisualizer.SetActive(false);
			}
			if (this.visualizer != null)
			{
				this.visualizer.SetActive(true);
			}
			base.SetCursor(this.cursor, this.cursorOffset, CursorMode.Auto);
			return;
		}
		if (mode != DragTool.Mode.Box)
		{
			return;
		}
		if (this.visualizer != null)
		{
			this.visualizer.SetActive(true);
		}
		this.mode = DragTool.Mode.Box;
		base.SetCursor(this.boxCursor, this.cursorOffset, CursorMode.Auto);
	}

	public override void OnFocus(bool focus)
	{
		DragTool.Mode mode = this.GetMode();
		if (mode != DragTool.Mode.Brush)
		{
			if (mode == DragTool.Mode.Box)
			{
				if (this.visualizer != null && !this.dragging)
				{
					this.visualizer.SetActive(focus);
				}
				this.hasFocus = focus || this.dragging;
			}
		}
		else
		{
			if (this.visualizer != null)
			{
				this.visualizer.SetActive(focus);
			}
			this.hasFocus = focus;
		}
		base.OnFocus(focus);
	}

	private void OnTutorialOpened(object data)
	{
		this.dragging = false;
	}

	public override bool ShowHoverUI()
	{
		return this.dragging || base.ShowHoverUI();
	}

	public override void LateUpdate()
	{
		base.LateUpdate();
	}

	[SerializeField]
	private Texture2D boxCursor;

	[SerializeField]
	private GameObject areaVisualizer;

	[SerializeField]
	private GameObject areaVisualizerTextPrefab;

	[SerializeField]
	private Color32 areaColour = new Color(1f, 1f, 1f, 0.5f);

	protected SpriteRenderer areaVisualizerSpriteRenderer;

	protected Guid areaVisualizerText;

	protected Vector3 placementPivot;

	protected bool interceptNumberKeysForPriority;

	private bool dragging;

	private Vector3 previousCursorPos;

	private DragTool.Mode mode = DragTool.Mode.Box;

	private DragTool.DragAxis dragAxis = DragTool.DragAxis.Invalid;

	protected bool canChangeDragAxis = true;

	protected Vector3 downPos;

	private enum DragAxis
	{
		Invalid = -1,
		None,
		Horizontal,
		Vertical
	}

	public enum Mode
	{
		Brush,
		Box
	}
}
