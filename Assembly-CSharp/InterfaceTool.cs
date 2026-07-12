using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/scripts/InterfaceTool")]
public class InterfaceTool : KMonoBehaviour
{
	public HashedString ViewMode
	{
		get
		{
			return this.viewMode;
		}
	}

	public virtual string[] DlcIDs
	{
		get
		{
			return DlcManager.AVAILABLE_ALL_VERSIONS;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.hoverTextConfiguration = base.GetComponent<HoverTextConfiguration>();
	}

	public void ActivateTool()
	{
		this.OnActivateTool();
		this.OnMouseMove(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
		Game.Instance.Trigger(1174281782, this);
	}

	public virtual bool ShowHoverUI()
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
		if (OverlayScreen.Instance == null || !ClusterManager.Instance.IsPositionInActiveWorld(vector) || vector.x < 0f || vector.x > Grid.WidthInMeters || vector.y < 0f || vector.y > Grid.HeightInMeters)
		{
			return false;
		}
		bool flag = false;
		global::UnityEngine.EventSystems.EventSystem current = global::UnityEngine.EventSystems.EventSystem.current;
		if (current != null)
		{
			Vector3 vector2 = new Vector3(KInputManager.GetMousePos().x, KInputManager.GetMousePos().y, 0f);
			current.RaycastAll(new PointerEventData(current)
			{
				position = vector2
			}, this.castResults);
			flag = this.castResults.Count == 0;
		}
		return flag;
	}

	protected virtual void OnActivateTool()
	{
		if (OverlayScreen.Instance != null && this.viewMode != OverlayModes.None.ID && OverlayScreen.Instance.mode != this.viewMode)
		{
			OverlayScreen.Instance.ToggleOverlay(this.viewMode, true);
			InterfaceTool.toolActivatedViewMode = this.viewMode;
		}
		this.SetCursor(this.cursor, this.cursorOffset, CursorMode.Auto);
	}

	public void DeactivateTool(InterfaceTool new_tool = null)
	{
		this.OnDeactivateTool(new_tool);
		if ((new_tool == null || new_tool == SelectTool.Instance) && InterfaceTool.toolActivatedViewMode != OverlayModes.None.ID && InterfaceTool.toolActivatedViewMode == SimDebugView.Instance.GetMode())
		{
			OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID, true);
			InterfaceTool.toolActivatedViewMode = OverlayModes.None.ID;
		}
	}

	public virtual void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = null;
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
		cursor_pos = Grid.CellToPosCBC(Grid.PosToCell(cursor_pos), this.visualizerLayer);
		cursor_pos.z += -0.15f;
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
		return Grid.CellToPosCCC(Grid.PosToCell(input), Grid.SceneLayer.Background) + (minimize ? (-vector) : vector);
	}

	protected Vector2 GetWorldRestrictedPosition(Vector2 input)
	{
		input.x = Mathf.Clamp(input.x, ClusterManager.Instance.activeWorld.minimumBounds.x, ClusterManager.Instance.activeWorld.maximumBounds.x);
		input.y = Mathf.Clamp(input.y, ClusterManager.Instance.activeWorld.minimumBounds.y, ClusterManager.Instance.activeWorld.maximumBounds.y);
		return input;
	}

	protected void SetCursor(Texture2D new_cursor, Vector2 offset, CursorMode mode)
	{
		if (new_cursor != InterfaceTool.activeCursor && new_cursor != null)
		{
			InterfaceTool.activeCursor = new_cursor;
			try
			{
				Cursor.SetCursor(new_cursor, offset, mode);
				if (PlayerController.Instance.vim != null)
				{
					PlayerController.Instance.vim.SetCursor(new_cursor);
				}
			}
			catch (Exception ex)
			{
				string text = string.Format("SetCursor Failed new_cursor={0} offset={1} mode={2}", new_cursor, offset, mode);
				KCrashReporter.ReportErrorDevNotification("SetCursor Failed", ex.StackTrace, text);
			}
		}
	}

	protected void UpdateHoverElements(List<KSelectable> hits)
	{
		if (this.hoverTextConfiguration != null)
		{
			this.hoverTextConfiguration.UpdateHoverElements(hits);
		}
	}

	public virtual void LateUpdate()
	{
		if (!this.populateHitsList)
		{
			this.UpdateHoverElements(null);
			return;
		}
		if (!this.isAppFocused)
		{
			return;
		}
		if (!Grid.IsValidCell(Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()))))
		{
			return;
		}
		this.hits.Clear();
		this.GetSelectablesUnderCursor(this.hits);
		KSelectable objectUnderCursor = this.GetObjectUnderCursor<KSelectable>(false, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, null);
		this.UpdateHoverElements(this.hits);
		if (!this.hasFocus && this.hoverOverride == null)
		{
			this.ClearHover();
		}
		else if (objectUnderCursor != this.hover)
		{
			this.ClearHover();
			this.hover = objectUnderCursor;
			if (objectUnderCursor != null)
			{
				Game.Instance.Trigger(2095258329, objectUnderCursor.gameObject);
				objectUnderCursor.Hover(!this.playedSoundThisFrame);
				this.playedSoundThisFrame = true;
			}
		}
		this.playedSoundThisFrame = false;
	}

	public void GetSelectablesUnderCursor(List<KSelectable> hits)
	{
		if (this.hoverOverride != null)
		{
			hits.Add(this.hoverOverride);
		}
		Camera main = Camera.main;
		Vector3 vector = new Vector3(KInputManager.GetMousePos().x, KInputManager.GetMousePos().y, -main.transform.GetPosition().z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Vector2 vector3 = new Vector2(vector2.x, vector2.y);
		int num = Grid.PosToCell(vector2);
		if (!Grid.IsValidCell(num) || !Grid.IsVisible(num))
		{
			return;
		}
		Game.Instance.statusItemRenderer.GetIntersections(vector3, hits);
		ListPool<ScenePartitionerEntry, SelectTool>.PooledList pooledList = ListPool<ScenePartitionerEntry, SelectTool>.Allocate();
		GameScenePartitioner.Instance.GatherEntries((int)vector3.x, (int)vector3.y, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
		pooledList.Sort((ScenePartitionerEntry x, ScenePartitionerEntry y) => this.SortHoverCards(x, y));
		foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList)
		{
			KCollider2D kcollider2D = scenePartitionerEntry.obj as KCollider2D;
			if (!(kcollider2D == null) && kcollider2D.Intersects(new Vector2(vector3.x, vector3.y)))
			{
				KSelectable kselectable = kcollider2D.GetComponent<KSelectable>();
				if (kselectable == null)
				{
					kselectable = kcollider2D.GetComponentInParent<KSelectable>();
				}
				if (!(kselectable == null) && kselectable.isActiveAndEnabled && !hits.Contains(kselectable) && kselectable.IsSelectable)
				{
					hits.Add(kselectable);
				}
			}
		}
		pooledList.Recycle();
	}

	public void SetLinkCursor(bool set)
	{
		this.SetCursor(set ? Assets.GetTexture("cursor_hand") : this.cursor, set ? Vector2.zero : this.cursorOffset, CursorMode.Auto);
	}

	protected T GetObjectUnderCursor<T>(bool cycleSelection, Func<T, bool> condition = null, Component previous_selection = null) where T : MonoBehaviour
	{
		this.intersections.Clear();
		this.GetObjectUnderCursor2D<T>(this.intersections, condition, this.layerMask);
		this.intersections.RemoveAll(new Predicate<InterfaceTool.Intersection>(InterfaceTool.is_component_null));
		if (this.intersections.Count <= 0)
		{
			this.prevIntersectionGroup.Clear();
			return default(T);
		}
		this.curIntersectionGroup.Clear();
		foreach (InterfaceTool.Intersection intersection in this.intersections)
		{
			this.curIntersectionGroup.Add(intersection.component);
		}
		if (!this.prevIntersectionGroup.Equals(this.curIntersectionGroup))
		{
			this.hitCycleCount = 0;
			this.prevIntersectionGroup = this.curIntersectionGroup;
		}
		this.intersections.Sort((InterfaceTool.Intersection a, InterfaceTool.Intersection b) => this.SortSelectables(a.component as KMonoBehaviour, b.component as KMonoBehaviour));
		int num = 0;
		if (cycleSelection)
		{
			num = this.hitCycleCount % this.intersections.Count;
			if (this.intersections[num].component != previous_selection || previous_selection == null)
			{
				num = 0;
				this.hitCycleCount = 0;
			}
			else
			{
				int num2 = this.hitCycleCount + 1;
				this.hitCycleCount = num2;
				num = num2 % this.intersections.Count;
			}
		}
		return this.intersections[num].component as T;
	}

	private void GetObjectUnderCursor2D<T>(List<InterfaceTool.Intersection> intersections, Func<T, bool> condition, int layer_mask) where T : MonoBehaviour
	{
		Camera main = Camera.main;
		Vector3 vector = new Vector3(KInputManager.GetMousePos().x, KInputManager.GetMousePos().y, -main.transform.GetPosition().z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Vector2 vector3 = new Vector2(vector2.x, vector2.y);
		if (this.hoverOverride != null)
		{
			intersections.Add(new InterfaceTool.Intersection
			{
				component = this.hoverOverride,
				distance = -100f
			});
		}
		int num = Grid.PosToCell(vector2);
		if (Grid.IsValidCell(num) && Grid.IsVisible(num))
		{
			Game.Instance.statusItemRenderer.GetIntersections(vector3, intersections);
			ListPool<ScenePartitionerEntry, SelectTool>.PooledList pooledList = ListPool<ScenePartitionerEntry, SelectTool>.Allocate();
			int num2 = 0;
			int num3 = 0;
			Grid.CellToXY(num, out num2, out num3);
			GameScenePartitioner.Instance.GatherEntries(num2, num3, 1, 1, GameScenePartitioner.Instance.collisionLayer, pooledList);
			foreach (ScenePartitionerEntry scenePartitionerEntry in pooledList)
			{
				KCollider2D kcollider2D = scenePartitionerEntry.obj as KCollider2D;
				if (!(kcollider2D == null) && kcollider2D.Intersects(new Vector2(vector2.x, vector2.y)))
				{
					T t = kcollider2D.GetComponent<T>();
					if (t == null)
					{
						t = kcollider2D.GetComponentInParent<T>();
					}
					if (!(t == null) && ((1 << t.gameObject.layer) & layer_mask) != 0 && !(t == null) && (condition == null || condition(t)))
					{
						float num4 = t.transform.GetPosition().z - vector2.z;
						bool flag = false;
						for (int i = 0; i < intersections.Count; i++)
						{
							InterfaceTool.Intersection intersection = intersections[i];
							if (intersection.component.gameObject == t.gameObject)
							{
								intersection.distance = Mathf.Min(intersection.distance, num4);
								intersections[i] = intersection;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							intersections.Add(new InterfaceTool.Intersection
							{
								component = t,
								distance = num4
							});
						}
					}
				}
			}
			pooledList.Recycle();
		}
	}

	private int SortSelectables(KMonoBehaviour x, KMonoBehaviour y)
	{
		if (x == null && y == null)
		{
			return 0;
		}
		if (x == null)
		{
			return -1;
		}
		if (y == null)
		{
			return 1;
		}
		int num = x.transform.GetPosition().z.CompareTo(y.transform.GetPosition().z);
		if (num != 0)
		{
			return num;
		}
		return x.GetInstanceID().CompareTo(y.GetInstanceID());
	}

	public void SetHoverOverride(KSelectable hover_override)
	{
		this.hoverOverride = hover_override;
	}

	private int SortHoverCards(ScenePartitionerEntry x, ScenePartitionerEntry y)
	{
		KMonoBehaviour kmonoBehaviour = x.obj as KMonoBehaviour;
		KMonoBehaviour kmonoBehaviour2 = y.obj as KMonoBehaviour;
		return this.SortSelectables(kmonoBehaviour, kmonoBehaviour2);
	}

	private static bool is_component_null(InterfaceTool.Intersection intersection)
	{
		return !intersection.component;
	}

	protected void ClearHover()
	{
		if (this.hover != null)
		{
			KSelectable kselectable = this.hover;
			this.hover = null;
			kselectable.Unhover();
			Game.Instance.Trigger(-1201923725, null);
		}
	}

	public const float MaxClickDistance = 0.02f;

	public const float DepthBias = -0.15f;

	public GameObject visualizer;

	public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;

	public string placeSound;

	protected bool populateHitsList;

	[NonSerialized]
	public bool hasFocus;

	[SerializeField]
	protected Texture2D cursor;

	public Vector2 cursorOffset = new Vector2(2f, 2f);

	public global::System.Action OnDeactivate;

	private static Texture2D activeCursor = null;

	private static HashedString toolActivatedViewMode = OverlayModes.None.ID;

	protected HashedString viewMode = OverlayModes.None.ID;

	private HoverTextConfiguration hoverTextConfiguration;

	private KSelectable hoverOverride;

	public KSelectable hover;

	protected int layerMask;

	protected SelectMarker selectMarker;

	private List<RaycastResult> castResults = new List<RaycastResult>();

	private bool isAppFocused = true;

	private List<KSelectable> hits = new List<KSelectable>();

	protected bool playedSoundThisFrame;

	private List<InterfaceTool.Intersection> intersections = new List<InterfaceTool.Intersection>();

	private HashSet<Component> prevIntersectionGroup = new HashSet<Component>();

	private HashSet<Component> curIntersectionGroup = new HashSet<Component>();

	private int hitCycleCount;

	public struct Intersection
	{
		public MonoBehaviour component;

		public float distance;
	}
}
