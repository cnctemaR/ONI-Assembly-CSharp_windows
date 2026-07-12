using System;
using System.Collections;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/Workable/Diggable")]
public class Diggable : Workable
{
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
		this.readyForSkillWorkStatusItem = Db.Get().BuildingStatusItems.DigRequiresSkillPerk;
		this.faceTargetWhenWorking = true;
		base.Subscribe<Diggable>(-1432940121, Diggable.OnReachableChangedDelegate);
		this.attributeConverter = Db.Get().AttributeConverters.DiggingSpeed;
		this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
		this.skillExperienceSkillGroup = Db.Get().SkillGroups.Mining.Id;
		this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
		this.multitoolContext = "dig";
		this.multitoolHitEffectTag = "fx_dig_splash";
		this.workingPstComplete = null;
		this.workingPstFailed = null;
		Prioritizable.AddRef(base.gameObject);
	}

	private Diggable()
	{
		base.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		this.originalDigElement = Grid.Element[num];
		if (this.originalDigElement.hardness == 255)
		{
			this.OnCancel();
		}
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.WaitingForDig, null);
		this.UpdateColor(this.isReachable);
		Grid.Objects[num, 7] = base.gameObject;
		ChoreType choreType = Db.Get().ChoreTypes.Dig;
		if (this.choreTypeIdHash.IsValid)
		{
			choreType = Db.Get().ChoreTypes.GetByHash(this.choreTypeIdHash);
		}
		this.chore = new WorkChore<Diggable>(choreType, this, null, true, null, null, null, true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		base.SetWorkTime(float.PositiveInfinity);
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Diggable.OnSpawn", base.gameObject, Grid.PosToCell(this), GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		this.OnSolidChanged(null);
		new ReachabilityMonitor.Instance(this).StartSM();
		base.Subscribe<Diggable>(493375141, Diggable.OnRefreshUserMenuDelegate);
		this.handle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
		Components.Diggables.Add(this);
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		Workable.AnimInfo animInfo = default(Workable.AnimInfo);
		if (this.overrideAnims != null && this.overrideAnims.Length != 0)
		{
			animInfo.overrideAnims = this.overrideAnims;
		}
		if (this.multitoolContext.IsValid && this.multitoolHitEffectTag.IsValid)
		{
			animInfo.smi = new MultitoolController.Instance(this, worker, this.multitoolContext, Assets.GetPrefab(this.multitoolHitEffectTag));
		}
		return animInfo;
	}

	private static bool IsCellBuildable(int cell)
	{
		bool flag = false;
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null && gameObject.GetComponent<Constructable>() != null)
		{
			flag = true;
		}
		return flag;
	}

	private IEnumerator PeriodicUnstableFallingRecheck()
	{
		yield return SequenceUtil.WaitForSeconds(2f);
		this.OnSolidChanged(null);
		yield break;
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
		if (Grid.Element[num].hardness == 255)
		{
			this.UpdateColor(false);
			this.requiredSkillPerk = null;
			this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDigUnobtanium);
		}
		else if (Grid.Element[num].hardness >= 251)
		{
			bool flag = false;
			using (List<Chore.PreconditionInstance>.Enumerator enumerator = this.chore.GetPreconditions().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.id == ChorePreconditions.instance.HasSkillPerk.id)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDigRadioactiveMaterials);
			}
			this.requiredSkillPerk = Db.Get().SkillPerks.CanDigRadioactiveMaterials.Id;
			this.materialDisplay.sharedMaterial = this.materials[3];
		}
		else if (Grid.Element[num].hardness >= 200)
		{
			bool flag2 = false;
			using (List<Chore.PreconditionInstance>.Enumerator enumerator = this.chore.GetPreconditions().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.id == ChorePreconditions.instance.HasSkillPerk.id)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (!flag2)
			{
				this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDigSuperDuperHard);
			}
			this.requiredSkillPerk = Db.Get().SkillPerks.CanDigSuperDuperHard.Id;
			this.materialDisplay.sharedMaterial = this.materials[3];
		}
		else if (Grid.Element[num].hardness >= 150)
		{
			bool flag3 = false;
			using (List<Chore.PreconditionInstance>.Enumerator enumerator = this.chore.GetPreconditions().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.id == ChorePreconditions.instance.HasSkillPerk.id)
					{
						flag3 = true;
						break;
					}
				}
			}
			if (!flag3)
			{
				this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDigNearlyImpenetrable);
			}
			this.requiredSkillPerk = Db.Get().SkillPerks.CanDigNearlyImpenetrable.Id;
			this.materialDisplay.sharedMaterial = this.materials[2];
		}
		else if (Grid.Element[num].hardness >= 50)
		{
			bool flag4 = false;
			using (List<Chore.PreconditionInstance>.Enumerator enumerator = this.chore.GetPreconditions().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.id == ChorePreconditions.instance.HasSkillPerk.id)
					{
						flag4 = true;
						break;
					}
				}
			}
			if (!flag4)
			{
				this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanDigVeryFirm);
			}
			this.requiredSkillPerk = Db.Get().SkillPerks.CanDigVeryFirm.Id;
			this.materialDisplay.sharedMaterial = this.materials[1];
		}
		else
		{
			this.requiredSkillPerk = null;
			this.chore.GetPreconditions().Remove(this.chore.GetPreconditions().Find((Chore.PreconditionInstance o) => o.id == ChorePreconditions.instance.HasSkillPerk.id));
		}
		this.UpdateStatusItem(null);
		bool flag5 = false;
		if (!Grid.Solid[num])
		{
			num2 = Diggable.GetUnstableCellAbove(num);
			if (num2 == -1)
			{
				flag5 = true;
			}
			else
			{
				base.StartCoroutine("PeriodicUnstableFallingRecheck");
			}
		}
		else if (Grid.Foundation[num])
		{
			flag5 = true;
		}
		if (!flag5)
		{
			if (num2 != -1)
			{
				Extents extents = default(Extents);
				Grid.CellToXY(num, out extents.x, out extents.y);
				extents.width = 1;
				extents.height = (num2 - num + Grid.WidthInCells - 1) / Grid.WidthInCells + 1;
				this.unstableEntry = GameScenePartitioner.Instance.Add("Diggable.OnSolidChanged", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
			}
			return;
		}
		this.isDigComplete = true;
		if (this.chore == null || !this.chore.InProgress())
		{
			Util.KDestroyGameObject(base.gameObject);
			return;
		}
		base.GetComponentInChildren<MeshRenderer>().enabled = false;
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
		Diggable.DoDigTick(Grid.PosToCell(this), dt);
		return this.isDigComplete;
	}

	protected override void OnStopWork(Worker worker)
	{
		if (this.isDigComplete)
		{
			Util.KDestroyGameObject(base.gameObject);
		}
	}

	public override bool InstantlyFinish(Worker worker)
	{
		int num = Grid.PosToCell(this);
		if (Grid.Element[num].hardness == 255)
		{
			return false;
		}
		float approximateDigTime = Diggable.GetApproximateDigTime(num);
		worker.Work(approximateDigTime);
		return true;
	}

	public static void DoDigTick(int cell, float dt)
	{
		Diggable.DoDigTick(cell, dt, WorldDamage.DamageType.Absolute);
	}

	public static void DoDigTick(int cell, float dt, WorldDamage.DamageType damageType)
	{
		float approximateDigTime = Diggable.GetApproximateDigTime(cell);
		float num = dt / approximateDigTime;
		WorldDamage.Instance.ApplyDamage(cell, num, -1, damageType, null, null);
	}

	public static float GetApproximateDigTime(int cell)
	{
		float num = (float)Grid.Element[cell].hardness;
		if (num == 255f)
		{
			return float.MaxValue;
		}
		Element element = ElementLoader.FindElementByHash(SimHashes.Ice);
		float num2 = num / (float)element.hardness;
		float num3 = Mathf.Min(Grid.Mass[cell], 400f) / 400f;
		float num4 = 4f * num3;
		return num4 + num2 * num4;
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
		List<int> cellsContainingFallingAbove = World.Instance.GetComponent<UnstableGroundManager>().GetCellsContainingFallingAbove(vector2I);
		if (cellsContainingFallingAbove.Contains(cell))
		{
			return cell;
		}
		byte b = Grid.WorldIdx[cell];
		int num = Grid.CellAbove(cell);
		while (Grid.IsValidCell(num) && Grid.WorldIdx[num] == b)
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
			return;
		}
		component.AddStatusItem(Db.Get().BuildingStatusItems.DigUnreachable, this);
		GameScheduler.Instance.Schedule("Locomotion Tutorial", 2f, delegate(object obj)
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Locomotion, true);
		}, null, null);
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
				return;
			}
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
	}

	private void OnCancel()
	{
		if (DetailsScreen.Instance != null)
		{
			DetailsScreen.Instance.Show(false);
		}
		base.gameObject.Trigger(2127324410, null);
	}

	private void OnRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("icon_cancel", UI.USERMENUACTIONS.CANCELDIG.NAME, new global::System.Action(this.OnCancel), global::Action.NumActions, null, null, null, UI.USERMENUACTIONS.CANCELDIG.TOOLTIP, true), 1f);
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
	public Material[] materials;

	[SerializeField]
	public MeshRenderer materialDisplay;

	private bool isDigComplete;

	private static List<global::Tuple<string, Tag>> lasersForHardness = new List<global::Tuple<string, Tag>>
	{
		new global::Tuple<string, Tag>("dig", "fx_dig_splash"),
		new global::Tuple<string, Tag>("specialistdig", "fx_dig_splash")
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
