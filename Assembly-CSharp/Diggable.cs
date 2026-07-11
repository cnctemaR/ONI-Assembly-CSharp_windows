using System;
using System.Collections;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using TUNING;
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
		this.workerStatusItem = Db.Get().DuplicantStatusItems.Digging;
		this.readyForRoleWorkStatusItem = Db.Get().BuildingStatusItems.DigRequiresRolePerk;
		this.faceTargetWhenWorking = true;
		base.Subscribe<Diggable>(-1432940121, Diggable.OnReachableChangedDelegate);
		this.attributeConverter = Db.Get().AttributeConverters.DiggingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		this.multitoolContext = "dig";
		this.multitoolHitEffectTag = "fx_dig_splash";
		this.workingPstComplete = HashedString.Invalid;
		this.workingPstFailed = HashedString.Invalid;
		Prioritizable.AddRef(base.gameObject);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		this.originalDigElement = Grid.Element[num];
		KSelectable component = base.GetComponent<KSelectable>();
		component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForDig, null);
		this.UpdateColor(this.isReachable);
		Grid.Objects[num, 7] = base.gameObject;
		ChoreType choreType = Db.Get().ChoreTypes.Dig;
		if (this.choreTypeIdHash.IsValid)
		{
			choreType = Db.Get().ChoreTypes.GetByHash(this.choreTypeIdHash);
		}
		this.chore = new WorkChore<Diggable>(choreType, this, null, this.choreTags, true, null, null, null, true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 0, false);
		base.SetWorkTime(float.PositiveInfinity);
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Diggable.OnSpawn", base.gameObject, Grid.PosToCell(this), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		this.OnSolidChanged(null);
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
		instance.StartSM();
		base.Subscribe<Diggable>(493375141, Diggable.OnRefreshUserMenuDelegate);
		this.handle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
		Components.Diggables.Add(this);
		Diggable.UpdateBuildableDiggables(num);
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo animInfo = default(Workable.AnimInfo);
		if (this.overrideAnims != null && this.overrideAnims.Length > 0)
		{
			animInfo.overrideAnims = this.overrideAnims;
		}
		if (this.multitoolContext.IsValid && this.multitoolHitEffectTag.IsValid)
		{
			animInfo.smi = new MultitoolController.Instance(this, worker, this.multitoolContext, Assets.GetPrefab(this.multitoolHitEffectTag));
		}
		return animInfo;
	}

	public static void UpdateBuildableDiggables(int cell)
	{
		Queue<GameUtil.FloodFillInfo> floodFillNext = GameUtil.FloodFillNext;
		floodFillNext.Clear();
		floodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = Grid.CellLeft(cell),
			depth = 0
		});
		floodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = Grid.CellRight(cell),
			depth = 0
		});
		floodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = Grid.CellAbove(cell),
			depth = 0
		});
		floodFillNext.Enqueue(new GameUtil.FloodFillInfo
		{
			cell = Grid.CellBelow(cell),
			depth = 0
		});
		Diggable.UpdateBuildableDiggables(floodFillNext);
		floodFillNext.Clear();
	}

	public static void UpdateBuildableDiggables(Queue<GameUtil.FloodFillInfo> queue)
	{
		List<Diggable> adjacentDiggables = new List<Diggable>();
		bool any_buildables = false;
		GameUtil.FloodFillConditional(queue, delegate(int visited_cell)
		{
			bool flag = false;
			if (Diggable.IsCellBuildable(visited_cell))
			{
				flag = true;
				any_buildables = true;
			}
			else
			{
				GameObject gameObject = Grid.Objects[visited_cell, 7];
				if (gameObject != null)
				{
					flag = true;
					adjacentDiggables.Add(gameObject.GetComponent<Diggable>());
				}
			}
			return flag;
		}, GameUtil.FloodFillVisited, null, 10000);
		GameUtil.FloodFillVisited.Clear();
		if (any_buildables)
		{
			foreach (Diggable diggable in adjacentDiggables)
			{
				if (!(diggable == null))
				{
					if (Array.IndexOf<Tag>(diggable.choreTags, GameTags.ChoreTypes.Building) < 0)
					{
						Array.Resize<Tag>(ref diggable.choreTags, diggable.choreTags.Length + 1);
						diggable.choreTags[diggable.choreTags.Length - 1] = GameTags.ChoreTypes.Building;
					}
				}
			}
		}
		else
		{
			foreach (Diggable diggable2 in adjacentDiggables)
			{
				if (!(diggable2 == null))
				{
					int num = Array.IndexOf<Tag>(diggable2.choreTags, GameTags.ChoreTypes.Building);
					if (num >= 0)
					{
						diggable2.choreTags[num] = diggable2.choreTags[diggable2.choreTags.Length - 1];
						Array.Resize<Tag>(ref diggable2.choreTags, diggable2.choreTags.Length - 1);
					}
				}
			}
		}
		queue.Clear();
	}

	private static bool IsCellBuildable(int cell)
	{
		bool flag = false;
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null)
		{
			Constructable component = gameObject.GetComponent<Constructable>();
			if (component != null)
			{
				flag = true;
			}
		}
		return flag;
	}

	private IEnumerator PeriodicUnstableFallingRecheck()
	{
		yield return new WaitForSeconds(2f);
		this.OnSolidChanged(null);
		yield break;
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole(JuniorMiner.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
		resume.AddExperienceIfRole(Miner.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
		resume.AddExperienceIfRole(SeniorMiner.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
	}

	private void OnSolidChanged(object data)
	{
		if (this == null || base.gameObject == null)
		{
			return;
		}
		GameScenePartitioner.Instance.Free(ref this.unstableEntry);
		int num = Grid.PosToCell(this);
		int num2 = -1;
		this.UpdateColor(this.isReachable);
		if (Grid.Element[num].hardness >= 150)
		{
			bool flag = false;
			foreach (Chore.PreconditionInstance preconditionInstance in this.chore.GetPreconditions())
			{
				if (preconditionInstance.id == ChorePreconditions.instance.HasRolePerk.id)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.chore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanDigNearlyImpenetrable);
			}
			this.requiredRolePerk = RoleManager.rolePerks.CanDigNearlyImpenetrable.id;
			this.materialDisplay.sharedMaterial = this.materials[2];
		}
		else if (Grid.Element[num].hardness >= 50)
		{
			bool flag2 = false;
			foreach (Chore.PreconditionInstance preconditionInstance2 in this.chore.GetPreconditions())
			{
				if (preconditionInstance2.id == ChorePreconditions.instance.HasRolePerk.id)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				this.chore.AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanDigVeryFirm);
			}
			this.requiredRolePerk = RoleManager.rolePerks.CanDigVeryFirm.id;
			this.materialDisplay.sharedMaterial = this.materials[1];
		}
		else
		{
			this.requiredRolePerk = HashedString.Invalid;
			this.chore.GetPreconditions().Remove(this.chore.GetPreconditions().Find((Chore.PreconditionInstance o) => o.id == ChorePreconditions.instance.HasRolePerk.id));
		}
		this.UpdateStatusItem(null);
		bool flag3 = false;
		if (!Grid.Solid[num])
		{
			num2 = Diggable.GetUnstableCellAbove(num);
			if (num2 == -1)
			{
				flag3 = true;
			}
			else
			{
				base.StartCoroutine("PeriodicUnstableFallingRecheck");
			}
		}
		else if (Grid.Foundation[num])
		{
			flag3 = true;
		}
		if (flag3)
		{
			this.isDigComplete = true;
			if (this.chore == null || !this.chore.InProgress())
			{
				Util.KDestroyGameObject(base.gameObject);
			}
			else
			{
				base.GetComponentInChildren<MeshRenderer>().enabled = false;
			}
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

	public Element GetTargetElement()
	{
		int num = Grid.PosToCell(base.transform.GetPosition());
		return Grid.Element[num];
	}

	public override string GetConversationTopic()
	{
		return this.originalDigElement.tag.Name;
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		int num = Grid.PosToCell(this);
		Diggable.DoDigTick(num, dt);
		return this.isDigComplete;
	}

	protected override void OnStopWork(Worker worker)
	{
		if (this.isDigComplete)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	public static void DoDigTick(int cell, float dt)
	{
		float num = (float)Grid.Element[cell].hardness;
		if (num == 255f)
		{
			return;
		}
		Element element = ElementLoader.FindElementByHash(SimHashes.Ice);
		float num2 = num / (float)element.hardness;
		float num3 = Mathf.Min(Grid.Mass[cell], 400f) / 400f;
		float num4 = 4f * num3;
		float num5 = num4 + num2 * num4;
		float num6 = dt / num5;
		WorldDamage.Instance.ApplyDamage(cell, num6, -1, -1, null, null);
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
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.isReachable)
		{
			component.RemoveStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, false);
		}
		else
		{
			component.AddStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, this);
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
			if (Grid.Element[Grid.PosToCell(base.gameObject)].hardness >= 50)
			{
				if (reachable)
				{
					material.color = Game.Instance.uiColours.Dig.validLocation;
				}
				else
				{
					material.color = Game.Instance.uiColours.Dig.unreachable;
				}
				this.multitoolContext = Diggable.lasersForHardness[1].first;
				this.multitoolHitEffectTag = Diggable.lasersForHardness[1].second;
			}
			else
			{
				if (reachable)
				{
					material.color = Game.Instance.uiColours.Dig.validLocation;
				}
				else
				{
					material.color = Game.Instance.uiColours.Dig.unreachable;
				}
				this.multitoolContext = Diggable.lasersForHardness[0].first;
				this.multitoolHitEffectTag = Diggable.lasersForHardness[0].second;
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
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		GameScenePartitioner.Instance.Free(ref this.unstableEntry);
		Game.Instance.Unsubscribe(this.handle);
		int num = Grid.PosToCell(this);
		GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.digDestroyedLayer, null);
		Components.Diggables.Remove(this);
		Diggable.UpdateBuildableDiggables(num);
	}

	private void OnCancel()
	{
		DetailsScreen.Instance.Show(false);
		base.gameObject.Trigger(2127324410, null);
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string text = "icon_cancel";
		string text2 = UI.USERMENUACTIONS.CANCELDIG.NAME;
		global::System.Action action = new global::System.Action(this.OnCancel);
		string text3 = UI.USERMENUACTIONS.CANCELDIG.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true), 1f);
	}

	private HandleVector<int>.Handle partitionerEntry;

	private HandleVector<int>.Handle unstableEntry;

	private MeshRenderer childRenderer;

	private bool isReachable;

	private Element originalDigElement;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	[SerializeField]
	public HashedString choreTypeIdHash;

	[SerializeField]
	public Tag[] choreTags;

	[SerializeField]
	public Material[] materials;

	[SerializeField]
	public MeshRenderer materialDisplay;

	private bool isDigComplete;

	private static List<Tuple<string, Tag>> lasersForHardness = new List<Tuple<string, Tag>>
	{
		new Tuple<string, Tag>("dig", "fx_dig_splash"),
		new Tuple<string, Tag>("specialistdig", "fx_dig_splash")
	};

	private int handle;

	private static readonly EventSystem.IntraObjectHandler<Diggable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Diggable>(delegate(Diggable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Diggable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Diggable>(delegate(Diggable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public Chore chore;
}
