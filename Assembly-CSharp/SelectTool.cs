using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using UnityEngine;

public class SelectTool : InterfaceTool
{
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
		ToolMenu.Instance.PriorityScreen.ResetPriority();
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

	public int GetDefaultLayerMask()
	{
		return this.defaultLayerMask;
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		this.ClearHover();
		this.Select(null, false);
	}

	private void OnApplicationFocus(bool app_has_focus)
	{
		this.appHasFocus = app_has_focus;
	}

	public override void LateUpdate()
	{
		if (!this.appHasFocus)
		{
			return;
		}
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		this.hits.Clear();
		this.GetSelectablesUnderCursor(this.hits);
		KSelectable objectUnderCursor = this.GetObjectUnderCursor<KSelectable>(false, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, null);
		base.UpdateHoverElements(this.hits);
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

	private void GetObjectUnderCursor2D<T>(List<SelectTool.Intersection> intersections, Func<T, bool> condition, int layer_mask) where T : MonoBehaviour
	{
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.GetPosition().z);
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
		int num = Grid.PosToCell(vector2);
		if (Grid.IsValidCell(num) && (Grid.Visible[num] != 0 || DebugPaintElementScreen.Instance.gameObject.activeSelf))
		{
			Game.Instance.statusItemRenderer.GetIntersections(vector3, intersections);
			List<ScenePartitionerEntry> list = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			int num2 = 0;
			int num3 = 0;
			Grid.CellToXY(num, out num2, out num3);
			GameScenePartitioner.Instance.GatherEntries(num2, num3, 1, 1, GameScenePartitioner.Instance.collisionLayer, list);
			foreach (ScenePartitionerEntry scenePartitionerEntry in list)
			{
				KCollider2D kcollider2D = scenePartitionerEntry.obj as KCollider2D;
				if (!(kcollider2D == null))
				{
					if (kcollider2D.Intersects(new Vector2(vector2.x, vector2.y)))
					{
						T t = kcollider2D.GetComponent<T>();
						if (t == null)
						{
							t = kcollider2D.GetComponentInParent<T>();
						}
						if (!(t == null))
						{
							if (((1 << t.gameObject.layer) & layer_mask) != 0)
							{
								if (!(t == null) && (condition == null || condition(t)))
								{
									float num4 = t.transform.GetPosition().z - vector2.z;
									bool flag = false;
									for (int i = 0; i < intersections.Count; i++)
									{
										SelectTool.Intersection intersection = intersections[i];
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
										intersections.Add(new SelectTool.Intersection
										{
											component = t,
											distance = num4
										});
									}
								}
							}
						}
					}
				}
			}
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.Free(list);
		}
	}

	public void GetSelectablesUnderCursor(List<KSelectable> hits)
	{
		if (this.hoverOverride != null)
		{
			hits.Add(this.hoverOverride);
		}
		Camera main = Camera.main;
		Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -main.transform.GetPosition().z);
		Vector3 vector2 = main.ScreenToWorldPoint(vector);
		Vector2 vector3 = new Vector2(vector2.x, vector2.y);
		int num = Grid.PosToCell(vector2);
		if (!Grid.IsValidCell(num) || (Grid.Visible[num] == 0 && !DebugPaintElementScreen.Instance.gameObject.activeSelf))
		{
			return;
		}
		Game.Instance.statusItemRenderer.GetIntersections(vector3, hits);
		List<ScenePartitionerEntry> list = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
		list.OrderBy<ScenePartitionerEntry, float>((ScenePartitionerEntry x) => (x.obj as Transform).GetPosition().z);
		GameScenePartitioner.Instance.GatherEntries((int)vector3.x, (int)vector3.y, 1, 1, GameScenePartitioner.Instance.collisionLayer, list);
		foreach (ScenePartitionerEntry scenePartitionerEntry in list)
		{
			KCollider2D kcollider2D = scenePartitionerEntry.obj as KCollider2D;
			if (!(kcollider2D == null))
			{
				if (kcollider2D.Intersects(new Vector2(vector3.x, vector3.y)))
				{
					KSelectable kselectable = kcollider2D.GetComponent<KSelectable>();
					if (kselectable == null)
					{
						kselectable = kcollider2D.GetComponentInParent<KSelectable>();
					}
					if (!(kselectable == null))
					{
						if (kselectable.isActiveAndEnabled)
						{
							if (!hits.Contains(kselectable))
							{
								if (kselectable.IsSelectable)
								{
									hits.Add(kselectable);
								}
							}
						}
					}
				}
			}
		}
		ListPool<ScenePartitionerEntry, GameScenePartitioner>.Free(list);
	}

	private T GetObjectUnderCursor<T>(bool cycleSelection, Func<T, bool> condition = null, Component previous_selection = null) where T : MonoBehaviour
	{
		this.intersections.Clear();
		this.GetObjectUnderCursor2D<T>(this.intersections, condition, this.layerMask);
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
			pos = selectable.transform.GetPosition();
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

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		KSelectable objectUnderCursor = this.GetObjectUnderCursor<KSelectable>(true, (KSelectable s) => s.GetComponent<KSelectable>().IsSelectable, this.selected);
		this.selectedCell = Grid.PosToCell(cursor_pos);
		this.Select(objectUnderCursor, false);
	}

	public int GetSelectedCell()
	{
		return this.selectedCell;
	}

	public KSelectable selected;

	public KSelectable hover;

	protected int cell_new;

	private int selectedCell;

	private KSelectable hoverOverride;

	public static SelectTool Instance;

	protected int defaultLayerMask;

	protected int layerMask;

	protected SelectMarker selectMarker;

	private bool appHasFocus = true;

	private List<KSelectable> hits = new List<KSelectable>();

	private int hitCycleCount;

	private List<SelectTool.Intersection> intersections = new List<SelectTool.Intersection>();

	private HashSet<Component> prevIntersectionGroup = new HashSet<Component>();

	private HashSet<Component> curIntersectionGroup = new HashSet<Component>();

	private KSelectable delayedNextSelection;

	private bool delayedSkipSound;

	private KSelectable previousSelection;

	private bool playedSoundThisFrame;

	public struct Intersection
	{
		public MonoBehaviour component;

		public float distance;
	}
}
