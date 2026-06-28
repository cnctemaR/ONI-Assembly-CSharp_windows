using System;
using System.Collections;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Diggable : Workable
{
	private Diggable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
	}

	public bool Reachable
	{
		get
		{
			return this.isReachable;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.progressbar_y_offset = 0.21f;
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Digging;
		if (this.choreType == null)
		{
			this.choreType = Db.Get().ChoreTypes.Dig;
		}
		this.faceTargetWhenWorking = true;
		base.Subscribe(-1432940121, new Action<object>(this.OnReachableChanged));
		this.attributeConverter = Db.Get().AttributeConverters.DiggingSpeed;
		Prioritizable.AddRef(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForDig, null);
		this.UpdateColor(this.isReachable);
		Grid.Objects[num, 7] = base.gameObject;
		this.chore = new WorkChore<Diggable>(this.choreType, this, null, true, null, null, null, true, null, true, default(Tag), null, true, true, true, PriorityScreen.PriorityClass.basic, int.MaxValue);
		base.SetWorkTime(float.PositiveInfinity);
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Diggable.OnSpawn", base.gameObject, Grid.PosToCell(this), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		this.OnSolidChanged(null);
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
		instance.StartSM();
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
		Components.Diggables.Add(this);
	}

	private IEnumerator PeriodicUnstableFallingRecheck()
	{
		yield return new WaitForSeconds(2f);
		this.OnSolidChanged(null);
		yield break;
	}

	private void OnSolidChanged(object data)
	{
		if (this == null || base.gameObject == null)
		{
			return;
		}
		if (this.unstableEntry != null)
		{
			this.unstableEntry.Release();
		}
		int num = Grid.PosToCell(this);
		int num2 = -1;
		this.UpdateColor(this.isReachable);
		bool flag = false;
		if (!Grid.Solid[num])
		{
			num2 = Diggable.GetUnstableCellAbove(num);
			if (num2 == -1)
			{
				flag = true;
			}
			else
			{
				base.StartCoroutine("PeriodicUnstableFallingRecheck");
			}
		}
		else if (Grid.Foundation[num])
		{
			flag = true;
		}
		if (flag)
		{
			if (base.worker != null)
			{
				base.Trigger(963113026, base.worker.gameObject);
			}
			Util.KDestroyGameObject(base.gameObject);
		}
		else if (num2 != -1)
		{
			Extents extents = default(Extents);
			Grid.CellToXY(num, out extents.x, out extents.y);
			extents.width = 1;
			extents.height = (num2 - num + Grid.WidthInCells - 1) / Grid.WidthInCells + 1;
			this.unstableEntry = GameScenePartitioner.Instance.Add("Diggable.OnSolidChanged", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		}
	}

	private IEnumerator DestroyWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		Util.KDestroyGameObject(base.gameObject);
		yield break;
	}

	public Element GetTargetElement()
	{
		int num = Grid.PosToCell(base.transform.position);
		return Grid.Element[num];
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		int num = Grid.PosToCell(this);
		float num2 = (float)Grid.Element[num].hardness;
		if (num2 == 255f)
		{
			return false;
		}
		Element element = ElementLoader.FindElementByHash(SimHashes.Ice);
		float num3 = num2 / (float)element.hardness;
		float num4 = Mathf.Min(Grid.Cell[num].mass, 400f) / 400f;
		float num5 = 4f * num4;
		float num6 = num5 + num3 * num5;
		float num7 = dt / num6;
		WorldDamage.Instance.ApplyDamage(num, num7, -1, -1);
		return false;
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "dig", EffectPrefabs.Instance.DigEffect);
		return anim;
	}

	public static Diggable GetDiggable(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 7];
		if (gameObject != null)
		{
			return gameObject.GetComponent<Diggable>();
		}
		return null;
	}

	public static bool IsDiggable(int cell)
	{
		if (Grid.Solid[cell])
		{
			return !Grid.Foundation[cell];
		}
		return Diggable.GetUnstableCellAbove(cell) != Grid.InvalidCell;
	}

	private static int GetUnstableCellAbove(int cell)
	{
		Vector2I vector2I = Grid.CellToXY(cell);
		UnstableGroundManager component = World.Instance.GetComponent<UnstableGroundManager>();
		List<int> cellsContainingFallingAbove = component.GetCellsContainingFallingAbove(vector2I);
		if (cellsContainingFallingAbove.Contains(cell))
		{
			return cell;
		}
		int num = Grid.CellAbove(cell);
		while (Grid.IsValidCell(num))
		{
			if (Grid.Foundation[num])
			{
				return Grid.InvalidCell;
			}
			if (Grid.Solid[num])
			{
				if (Grid.Element[num].IsUnstable)
				{
					return num;
				}
				return Grid.InvalidCell;
			}
			else
			{
				if (cellsContainingFallingAbove.Contains(num))
				{
					return num;
				}
				num = Grid.CellAbove(num);
			}
		}
		return Grid.InvalidCell;
	}

	public static bool RequiresTool(Element e)
	{
		return false;
	}

	public static bool Undiggable(Element e)
	{
		return e.id == SimHashes.Unobtanium;
	}

	private void OnReachableChanged(object data)
	{
		if (this.childRenderer == null)
		{
			this.childRenderer = base.GetComponentInChildren<MeshRenderer>();
		}
		Material material = this.childRenderer.material;
		this.isReachable = (bool)data;
		if (material.color == Game.Instance.uiColours.Dig.invalidLocation)
		{
			return;
		}
		this.UpdateColor(this.isReachable);
		if (this.isReachable)
		{
			this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, false);
		}
		else
		{
			this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, this);
			GameScheduler.Instance.Schedule("Locomotion Tutorial", 2f, delegate(object obj)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion);
			}, null, null);
		}
	}

	private void UpdateColor(bool reachable)
	{
		if (this.childRenderer != null)
		{
			Material material = this.childRenderer.material;
			if (Diggable.RequiresTool(Grid.Element[Grid.PosToCell(base.gameObject)]) || Diggable.Undiggable(Grid.Element[Grid.PosToCell(base.gameObject)]))
			{
				material.color = Game.Instance.uiColours.Dig.invalidLocation;
				return;
			}
			if (Grid.Element[Grid.PosToCell(base.gameObject)].hardness >= 150)
			{
				if (reachable)
				{
					material.color = Game.Instance.uiColours.Dig.requiresRole;
				}
				else
				{
					material.color = Game.Instance.uiColours.Dig.unreachable_requiresRole;
				}
			}
			else if (reachable)
			{
				material.color = Game.Instance.uiColours.Dig.validLocation;
			}
			else
			{
				material.color = Game.Instance.uiColours.Dig.unreachable;
			}
		}
	}

	public override float GetPercentComplete()
	{
		return Grid.Damage[Grid.PosToCell(this)];
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
		if (this.unstableEntry != null)
		{
			this.unstableEntry.Release();
		}
		GameScenePartitioner.Instance.TriggerEvent(Grid.PosToCell(this), GameScenePartitioner.Instance.digDestroyedLayer, null);
		Components.Diggables.Remove(this);
	}

	private void OnCancel()
	{
		DetailsScreen.Instance.Show(false);
		base.gameObject.Trigger(2127324410, null);
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = this.userMenu;
		string text = "icon_cancel";
		string text2 = UI.USERMENUACTIONS.CANCELDIG.NAME;
		global::System.Action action = new global::System.Action(this.OnCancel);
		string text3 = UI.USERMENUACTIONS.CANCELDIG.TOOLTIP;
		userMenu.AddButton(new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
	}

	public void SetChoreType(ChoreType chore_type)
	{
		this.choreType = chore_type;
		if (this.chore != null)
		{
			this.chore.choreType = chore_type;
		}
	}

	private GameScenePartitionerEntry partitionerEntry;

	private GameScenePartitionerEntry unstableEntry;

	private MeshRenderer childRenderer;

	private bool isReachable;

	private Element cellElementReference;

	[MyCmpAdd]
	private UserMenu userMenu;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	private ChoreType choreType;

	public Chore chore;
}
