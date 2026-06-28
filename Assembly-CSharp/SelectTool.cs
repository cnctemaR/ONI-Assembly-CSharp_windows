using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SelectTool : InterfaceTool
{
	public bool IncludeRegions { get; protected set; }

	protected override void OnPrefabInit()
	{
		this.defaultLayerMask = 1 | LayerMask.GetMask(new string[] { "World", "Pickupable", "Place", "PlaceWithDepth", "BlockSelection", "Construction", "Selection" });
		this.layerMask = this.defaultLayerMask;
		this.selectMarker = global::Util.KInstantiateUI<SelectMarker>(EntityPrefabs.Instance.SelectMarker, GameScreenManager.Instance.worldSpaceCanvas, false);
		this.selectMarker.gameObject.SetActive(false);
		SelectTool.Instance = this;
	}

	public bool IsManualControlActive()
	{
		return base.enabled && this.selected != null && this.selected.GetSMI<ManualControlMonitor.Instance>() != null && this.selected.GetSMI<ManualControlMonitor.Instance>().IsControlled();
	}

	public void Activate()
	{
		if (!this.IsManualControlActive())
		{
			PlayerController.Instance.ActivateTool(this);
			this.Select(null, false);
		}
	}

	public void SetLayerMask(int mask)
	{
		this.layerMask = mask;
	}

	public void ClearLayerMask()
	{
		this.layerMask = this.defaultLayerMask;
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		this.ClearHover();
		this.Select(null, false);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
	}

	private void LateUpdate()
	{
		bool flag = false;
		this.cell_new = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (!Grid.IsValidCell(this.cell_new))
		{
			return;
		}
		if (!HoverTextScreen.Instance.IsVisible)
		{
			this.hoverScreenUpdate.Prime();
		}
		else if (this.hoverScreenUpdate.tick())
		{
			flag = true;
		}
		if (!this.hasFocus && this.hoverOverride == null)
		{
			this.ClearHover();
		}
		else
		{
			KSelectable[] selectablesUnderCursor = this.GetSelectablesUnderCursor(this.IncludeRegions);
			KSelectable objectUnderCursor = this.GetObjectUnderCursor<KSelectable>(false, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, null);
			bool flag2 = false;
			if (this.cell_old != this.cell_new)
			{
				flag2 = true;
			}
			if (flag || selectablesUnderCursor != this.selectablesOnCell || flag2)
			{
				if (this.hoverText == null)
				{
					this.hoverText = base.gameObject.GetComponent<HoverTextConfiguration>();
					this.hoverText.ConfigureHoverScreen();
				}
				if (flag2 || selectablesUnderCursor != this.selectablesOnCell)
				{
					this.hoverText.UpdateHoverElements(selectablesUnderCursor);
				}
				else
				{
					this.hoverText.UpdateHoverElements(selectablesUnderCursor);
				}
			}
			if (objectUnderCursor != null && objectUnderCursor != this.hover)
			{
				this.ClearHover();
				if (selectablesUnderCursor != null && selectablesUnderCursor != this.selectablesOnCell)
				{
					if (this.hover != objectUnderCursor)
					{
						this.hover = objectUnderCursor;
						Game.Instance.Trigger(2095258329, objectUnderCursor.gameObject);
						if (objectUnderCursor != null)
						{
							objectUnderCursor.Hover(!this.playedSoundThisFrame);
							this.playedSoundThisFrame = true;
						}
					}
					this.selectablesOnCell = selectablesUnderCursor;
				}
			}
			if (selectablesUnderCursor == null)
			{
				this.ClearHover();
			}
		}
		this.cell_old = this.cell_new;
		this.playedSoundThisFrame = false;
	}

	private void GetObjectUnderCursor2D<T>(List<SelectTool.Intersection> intersections, Func<T, bool> condition, int layer_mask) where T : MonoBehaviour
	{
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.position.z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Vector2 vector3 = new Vector2(vector2.x, vector2.y);
		if (this.hoverOverride != null)
		{
			intersections.Add(new SelectTool.Intersection
			{
				component = this.hoverOverride,
				distance = -100f
			});
		}
		Game.Instance.statusItemRenderer.GetIntersections(vector3, intersections);
		int num = Physics2D.OverlapPointNonAlloc(vector3, this.overlaps, layer_mask);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = this.overlaps[i].gameObject;
			T t = gameObject.GetComponent<T>();
			if (t == null)
			{
				t = gameObject.GetComponentInParent<T>();
			}
			if (t != null && (condition == null || condition(t)))
			{
				float num2 = gameObject.transform.position.z - vector2.z;
				bool flag = false;
				for (int j = 0; j < intersections.Count; j++)
				{
					SelectTool.Intersection intersection = intersections[j];
					if (intersection.component.gameObject == t.gameObject)
					{
						intersection.distance = Mathf.Min(intersection.distance, num2);
						intersections[j] = intersection;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					intersections.Add(new SelectTool.Intersection
					{
						component = t,
						distance = num2
					});
				}
			}
		}
	}

	private KSelectable[] GetSelectablesUnderCursor(bool includeRegions = false)
	{
		this.hits.Clear();
		if (this.hoverOverride != null)
		{
			this.hits.Add(this.hoverOverride);
		}
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.position.z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Vector2 vector3 = new Vector2(vector2.x, vector2.y);
		int num = Physics2D.OverlapPointNonAlloc(vector3, this.allSelectableOverlaps);
		Game.Instance.statusItemRenderer.GetIntersections(vector3, this.hits);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = this.allSelectableOverlaps[i].gameObject;
			KSelectable kselectable = gameObject.GetComponent<KSelectable>();
			if (kselectable == null)
			{
				kselectable = gameObject.GetComponentInParent<KSelectable>();
			}
			if (kselectable != null && kselectable.isActiveAndEnabled && !this.hits.Contains(kselectable) && kselectable.IsSelectable)
			{
				this.hits.Add(kselectable);
			}
		}
		if (includeRegions)
		{
			Region region = RegionInterfaceScreen.Instance.RegionUnderCursor();
			if (region != null)
			{
				this.hits.Add(region.GetComponent<KSelectable>());
			}
		}
		return this.hits.ToArray();
	}

	private void GetRegionsUnderCursor<T>(List<SelectTool.Intersection> intersections, Func<T, bool> condition, int layer_mask) where T : MonoBehaviour
	{
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.position.z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Region intersectionRegion = Game.Instance.RegionManager.GetIntersectionRegion(vector2);
		if (intersectionRegion != null)
		{
			T t = (T)((object)intersectionRegion.GetComponent<T>());
			if (t != null && (condition == null || condition(t)))
			{
				intersections.Add(new SelectTool.Intersection
				{
					component = t,
					distance = float.MaxValue
				});
			}
		}
	}

	private T GetObjectUnderCursor<T>(bool cycleSelection, Func<T, bool> condition = null, Component previous_selection = null) where T : MonoBehaviour
	{
		this.intersections.Clear();
		this.GetObjectUnderCursor2D<T>(this.intersections, condition, this.layerMask);
		this.GetRegionsUnderCursor<T>(this.intersections, condition, this.layerMask);
		this.intersections.RemoveAll((SelectTool.Intersection intersection) => !intersection.component);
		if (this.intersections.Count <= 0)
		{
			this.prevIntersectionGroup.Clear();
			return (T)((object)null);
		}
		this.curIntersectionGroup.Clear();
		foreach (SelectTool.Intersection intersection2 in this.intersections)
		{
			this.curIntersectionGroup.Add(intersection2.component);
		}
		if (!this.prevIntersectionGroup.Equals(this.curIntersectionGroup))
		{
			this.hitCycleCount = 0;
			this.prevIntersectionGroup = this.curIntersectionGroup;
		}
		this.intersections.Sort((SelectTool.Intersection a, SelectTool.Intersection b) => (a.distance == b.distance) ? a.component.GetInstanceID().CompareTo(b.component.GetInstanceID()) : a.distance.CompareTo(b.distance));
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
				num = ++this.hitCycleCount % this.intersections.Count;
			}
		}
		return this.intersections[num].component as T;
	}

	private void ClearHover()
	{
		if (this.hover != null)
		{
			KSelectable kselectable = this.hover;
			this.hover = null;
			kselectable.Unhover();
			Game.Instance.Trigger(-1201923725, null);
		}
	}

	public void SetHoverOverride(KSelectable hover_override)
	{
		this.hoverOverride = hover_override;
	}

	public void Focus(Vector3 pos, KSelectable selectable, Vector3 offset)
	{
		if (selectable != null)
		{
			pos = selectable.transform.position;
		}
		pos.z = -40f;
		pos += offset;
		CameraController.Instance.SetTargetPos(pos, 8f);
	}

	public void SelectAndFocus(Vector3 pos, KSelectable selectable, Vector3 offset)
	{
		this.Focus(pos, selectable, offset);
		this.Select(selectable, false);
	}

	public void SelectAndFocus(Vector3 pos, KSelectable selectable)
	{
		this.SelectAndFocus(pos, selectable, Vector3.zero);
	}

	public void Select(KSelectable new_selected, bool skipSound = false)
	{
		this.ClearContextMenu();
		if (new_selected == this.previousSelection)
		{
			return;
		}
		this.previousSelection = new_selected;
		if (this.selected != null)
		{
			this.selected.Unselect();
		}
		GameObject gameObject = null;
		if (new_selected != null)
		{
			if (new_selected == this.hover)
			{
				this.ClearHover();
			}
			new_selected.Select();
			gameObject = new_selected.gameObject;
			this.selectMarker.SetTargetTransform(gameObject.transform);
			this.selectMarker.gameObject.SetActive(!new_selected.DisableSelectMarker);
			SelectToolHoverTextCard component = base.GetComponent<SelectToolHoverTextCard>();
			if (component != null)
			{
				int num = component.currentSelectedSelectableIndex;
				int recentNumberOfDisplayedSelectables = component.recentNumberOfDisplayedSelectables;
				if (recentNumberOfDisplayedSelectables != 0)
				{
					num = (num + 1) % recentNumberOfDisplayedSelectables;
					if (!skipSound)
					{
						if (recentNumberOfDisplayedSelectables == 1)
						{
							KFMOD.PlayOneShot(GlobalAssets.GetSound("Select_empty", false));
						}
						else
						{
							EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Select_full", false), Vector3.zero);
							eventInstance.setParameterValue("selection", (float)num);
							SoundEvent.EndOneShot(eventInstance);
						}
						this.playedSoundThisFrame = true;
					}
				}
			}
		}
		else
		{
			this.selectMarker.gameObject.SetActive(false);
		}
		this.selected = new_selected;
		Game.Instance.Trigger(-1503271301, gameObject);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		KSelectable objectUnderCursor = this.GetObjectUnderCursor<KSelectable>(true, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, this.selected);
		this.selectedCell = Grid.PosToCell(cursor_pos);
		this.Select(objectUnderCursor, false);
	}

	private void ClearContextMenu()
	{
	}

	public override void OnRightClickDown(Vector3 cursor_pos, KButtonEvent e)
	{
		base.OnRightClickDown(cursor_pos, e);
	}

	public override void OnRightClickUp(Vector3 cursor_pos)
	{
		base.OnRightClickUp(cursor_pos);
	}

	public int GetSelectedCell()
	{
		return this.selectedCell;
	}

	private const int MAX_OVERLAPS = 16;

	public KSelectable selected;

	public KSelectable hover;

	private KSelectable[] selectablesOnCell;

	protected int cell_new;

	protected int cell_old;

	private int selectedCell;

	private KSelectable hoverOverride;

	public static SelectTool Instance;

	protected int defaultLayerMask;

	protected int layerMask;

	private HoverTextScreen.HoverTextUpdateTimer hoverScreenUpdate;

	protected SelectMarker selectMarker;

	private Collider2D[] overlaps = new Collider2D[16];

	private List<KSelectable> hits = new List<KSelectable>();

	private Collider2D[] allSelectableOverlaps = new Collider2D[16];

	private int hitCycleCount;

	private List<SelectTool.Intersection> intersections = new List<SelectTool.Intersection>();

	private HashSet<Component> prevIntersectionGroup = new HashSet<Component>();

	private HashSet<Component> curIntersectionGroup = new HashSet<Component>();

	private KSelectable previousSelection;

	private bool playedSoundThisFrame;

	public struct Intersection
	{
		public MonoBehaviour component;

		public float distance;
	}
}
