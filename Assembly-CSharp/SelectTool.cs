using System;
using System.Collections;
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

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
		this.Select(null, false);
	}

	public void SetLayerMask(int mask)
	{
		this.layerMask = mask;
		this.ClearHover();
		this.LateUpdate();
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

	private void LateUpdate()
	{
		this.cell_new = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (Grid.IsValidCell(this.cell_new))
		{
			if (!HoverTextScreen.Instance.IsVisible)
			{
				this.hoverScreenUpdate.Prime();
			}
			if (!this.hasFocus && this.hoverOverride == null)
			{
				this.ClearHover();
			}
			else
			{
				this.hits.Clear();
				this.GetSelectablesUnderCursor(this.hits, this.IncludeRegions);
				KSelectable objectUnderCursor = this.GetObjectUnderCursor<KSelectable>(false, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, null);
				if (this.hoverText == null)
				{
					this.hoverText = base.gameObject.GetComponent<HoverTextConfiguration>();
					this.hoverText.ConfigureHoverScreen();
				}
				this.hoverText.UpdateHoverElements(this.hits);
				if (objectUnderCursor != null && objectUnderCursor != this.hover)
				{
					this.ClearHover();
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
				}
			}
			this.cell_old = this.cell_new;
			this.playedSoundThisFrame = false;
		}
	}

	private void GetObjectUnderCursor2D<T>(List<SelectTool.Intersection> intersections, Func<T, bool> condition, int layer_mask) where T : MonoBehaviour
	{
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.position.z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Vector2 vector3 = new Vector2(vector2.x, vector2.y);
		int num = 0;
		if (this.hoverOverride != null)
		{
			intersections.Add(new SelectTool.Intersection
			{
				component = this.hoverOverride,
				distance = -100f
			});
		}
		if (Grid.Visible[Grid.PosToCell(vector2)] != 0 || DebugPaintElementScreen.Instance.gameObject.activeSelf)
		{
			num = Physics2D.OverlapPointNonAlloc(vector3, this.overlaps, layer_mask);
			Game.Instance.statusItemRenderer.GetIntersections(vector3, intersections);
		}
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

	public void GetSelectablesUnderCursor(List<KSelectable> hits, bool includeRegions = false)
	{
		if (this.hoverOverride != null)
		{
			hits.Add(this.hoverOverride);
		}
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.position.z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Vector2 vector3 = new Vector2(vector2.x, vector2.y);
		int num = Grid.PosToCell(vector2);
		if (Grid.IsValidCell(num) && (Grid.Visible[num] != 0 || DebugPaintElementScreen.Instance.gameObject.activeSelf))
		{
			int num2 = Physics2D.OverlapPointNonAlloc(vector3, this.allSelectableOverlaps);
			Game.Instance.statusItemRenderer.GetIntersections(vector3, hits);
			for (int i = 0; i < num2; i++)
			{
				GameObject gameObject = this.allSelectableOverlaps[i].gameObject;
				KSelectable kselectable = gameObject.GetComponent<KSelectable>();
				if (kselectable == null)
				{
					kselectable = gameObject.GetComponentInParent<KSelectable>();
				}
				if (kselectable != null && kselectable.isActiveAndEnabled && !hits.Contains(kselectable) && kselectable.IsSelectable)
				{
					hits.Add(kselectable);
				}
			}
			if (includeRegions)
			{
				Region region = RegionInterfaceScreen.Instance.RegionUnderCursor();
				if (region != null)
				{
					hits.Add(region.GetComponent<KSelectable>());
				}
			}
		}
	}

	private void GetRegionsUnderCursor<T>(List<SelectTool.Intersection> intersections, Func<T, bool> condition, int layer_mask) where T : MonoBehaviour
	{
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.position.z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Region intersectionRegion = Game.Instance.RegionManager.GetIntersectionRegion(vector2);
		if (intersectionRegion != null)
		{
			T component = intersectionRegion.GetComponent<T>();
			if (component != null && (condition == null || condition(component)))
			{
				intersections.Add(new SelectTool.Intersection
				{
					component = component,
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
		T t;
		if (this.intersections.Count <= 0)
		{
			this.prevIntersectionGroup.Clear();
			t = (T)((object)null);
		}
		else
		{
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
			t = this.intersections[num].component as T;
		}
		return t;
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
		CameraController.Instance.SetTargetPos(pos, 8f, true);
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

	public void SelectNextFrame(KSelectable new_selected, bool skipSound = false)
	{
		this.delayedNextSelection = new_selected;
		this.delayedSkipSound = skipSound;
		base.StartCoroutine(this.DoSelectNextFrame());
	}

	private IEnumerator DoSelectNextFrame()
	{
		yield return null;
		this.Select(this.delayedNextSelection, this.delayedSkipSound);
		this.delayedNextSelection = null;
		yield break;
	}

	public void Select(KSelectable new_selected, bool skipSound = false)
	{
		if (!(new_selected == this.previousSelection))
		{
			this.previousSelection = new_selected;
			if (this.selected != null)
			{
				this.selected.Unselect();
			}
			GameObject gameObject = null;
			if (new_selected != null)
			{
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
				if (new_selected == this.hover)
				{
					this.ClearHover();
				}
				new_selected.Select();
				gameObject = new_selected.gameObject;
				this.selectMarker.SetTargetTransform(gameObject.transform);
				this.selectMarker.gameObject.SetActive(!new_selected.DisableSelectMarker);
			}
			else if (this.selectMarker != null)
			{
				this.selectMarker.gameObject.SetActive(false);
			}
			this.selected = new_selected;
			Game.Instance.Trigger(-1503271301, gameObject);
		}
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		KSelectable objectUnderCursor = this.GetObjectUnderCursor<KSelectable>(true, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, this.selected);
		this.selectedCell = Grid.PosToCell(cursor_pos);
		this.Select(objectUnderCursor, false);
	}

	public override void OnRightClickUp(Vector3 cursor_pos)
	{
		base.OnRightClickUp(cursor_pos);
	}

	public int GetSelectedCell()
	{
		return this.selectedCell;
	}

	public KSelectable selected;

	public KSelectable hover;

	protected int cell_new;

	protected int cell_old;

	private int selectedCell;

	private KSelectable hoverOverride;

	public static SelectTool Instance;

	protected int defaultLayerMask;

	protected int layerMask;

	private HoverTextScreen.HoverTextUpdateTimer hoverScreenUpdate;

	protected SelectMarker selectMarker;

	private List<KSelectable> hits = new List<KSelectable>();

	private const int MAX_OVERLAPS = 16;

	private Collider2D[] overlaps = new Collider2D[16];

	private Collider2D[] allSelectableOverlaps = new Collider2D[16];

	private int hitCycleCount = 0;

	private List<SelectTool.Intersection> intersections = new List<SelectTool.Intersection>();

	private HashSet<Component> prevIntersectionGroup = new HashSet<Component>();

	private HashSet<Component> curIntersectionGroup = new HashSet<Component>();

	private KSelectable delayedNextSelection;

	private bool delayedSkipSound;

	private KSelectable previousSelection = null;

	private bool playedSoundThisFrame = false;

	public struct Intersection
	{
		public MonoBehaviour component;

		public float distance;
	}
}
