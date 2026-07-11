using System;
using System.Collections.Generic;
using System.Diagnostics;
using FMOD.Studio;
using STRINGS;
using TUNING;
using UnityEngine;

public class Pickupable : Workable
{
	private Pickupable()
	{
		this.showProgressBar = false;
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
		this.shouldTransferDiseaseWithWorker = false;
	}

	public PrimaryElement PrimaryElement
	{
		get
		{
			return this.primaryElement;
		}
	}

	public Storage storage { get; set; }

	public float MinTakeAmount
	{
		get
		{
			return 0f;
		}
	}

	public bool isKinematic { get; set; }

	public bool wasAbsorbed { get; private set; }

	public int cachedCell { get; private set; }

	public bool IsEntombed
	{
		get
		{
			return this.isEntombed;
		}
		set
		{
			if (value != this.isEntombed)
			{
				this.isEntombed = value;
				if (this.isEntombed)
				{
					base.GetComponent<KPrefabID>().AddTag(GameTags.Entombed);
				}
				else
				{
					base.GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
				}
				base.Trigger(-1089732772, null);
				this.UpdateEntombedVisualizer();
			}
		}
	}

	private bool CouldBePickedUpCommon(GameObject carrier)
	{
		bool flag = this.UnreservedAmount > 0f || this.GetReservedAmount(carrier) > 0f;
		bool flag2 = this.UnreservedAmount >= this.MinTakeAmount;
		return flag && flag2;
	}

	public bool CouldBePickedUpByMinion(GameObject carrier)
	{
		bool flag = this.storage == null || !this.storage.automatable || !this.storage.automatable.GetAutomationOnly();
		return this.CouldBePickedUpCommon(carrier) && flag;
	}

	public bool CouldBePickedUpByTransferArm(GameObject carrier)
	{
		return this.CouldBePickedUpCommon(carrier);
	}

	public float GetReservedAmount(GameObject reserver)
	{
		for (int i = 0; i < this.reservations.Count; i++)
		{
			if (this.reservations[i].reserver == reserver)
			{
				return this.reservations[i].amount;
			}
		}
		return 0f;
	}

	public float UnreservedAmount
	{
		get
		{
			return this.TotalAmount - this.ReservedAmount;
		}
	}

	public float ReservedAmount { get; private set; }

	public float TotalAmount
	{
		get
		{
			return this.primaryElement.Units;
		}
		set
		{
			DebugUtil.Assert(this.primaryElement != null, "Assert!", string.Empty, string.Empty);
			this.primaryElement.Units = value;
			if (value <= 0.001f)
			{
				PrimaryElement component = base.GetComponent<PrimaryElement>();
				if (!component.KeepZeroMassObject)
				{
					base.gameObject.DeleteObject();
				}
			}
			this.NotifyChanged(Grid.PosToCell(this));
		}
	}

	private void RefreshReservedAmount()
	{
		this.ReservedAmount = 0f;
		for (int i = 0; i < this.reservations.Count; i++)
		{
			this.ReservedAmount += this.reservations[i].amount;
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void Log(string evt, string param, float value)
	{
	}

	public void ClearReservations()
	{
		this.reservations.Clear();
		this.RefreshReservedAmount();
	}

	[ContextMenu("Print Reservations")]
	public void PrintReservations()
	{
		foreach (Pickupable.Reservation reservation in this.reservations)
		{
			global::Debug.Log(reservation.ToString(), null);
		}
	}

	public int Reserve(string context, GameObject reserver, float amount)
	{
		int num = this.nextTicketNumber++;
		Pickupable.Reservation reservation = new Pickupable.Reservation(reserver, amount, num);
		this.reservations.Add(reservation);
		this.RefreshReservedAmount();
		if (this.OnReservationsChanged != null)
		{
			this.OnReservationsChanged();
		}
		return num;
	}

	public void Unreserve(string context, int ticket)
	{
		for (int i = 0; i < this.reservations.Count; i++)
		{
			if (this.reservations[i].ticket == ticket)
			{
				this.reservations.RemoveAt(i);
				this.RefreshReservedAmount();
				if (this.OnReservationsChanged != null)
				{
					this.OnReservationsChanged();
				}
				break;
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.workingPstComplete = HashedString.Invalid;
		this.workingPstFailed = HashedString.Invalid;
		this.log = new LoggerFSSF("Pickupable");
		this.workerStatusItem = Db.Get().DuplicantStatusItems.PickingUp;
		base.SetWorkTime(1.5f);
		this.targetWorkable = this;
		this.resetProgressOnStop = true;
		base.gameObject.layer = Game.PickupableLayer;
		Vector3 position = base.transform.GetPosition();
		this.UpdateCachedCell(Grid.PosToCell(position));
		base.Subscribe<Pickupable>(856640610, Pickupable.OnStoreDelegate);
		base.Subscribe<Pickupable>(1188683690, Pickupable.OnLandedDelegate);
		base.Subscribe<Pickupable>(1807976145, Pickupable.OnOreSizeChangedDelegate);
		base.Subscribe<Pickupable>(-1432940121, Pickupable.OnReachableChangedDelegate);
		base.Subscribe<Pickupable>(-778359855, Pickupable.RefreshStorageTagsDelegate);
		this.KPrefabID.AddTag(GameTags.Pickupable);
		Components.Pickupables.Add(this);
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			base.gameObject.DeleteObject();
			return;
		}
		this.UpdateCachedCell(num);
		ReachabilityMonitor.Instance instance = new ReachabilityMonitor.Instance(this);
		instance.StartSM();
		FetchableMonitor.Instance instance2 = new FetchableMonitor.Instance(this);
		instance2.StartSM();
		base.SetWorkTime(1.5f);
		this.faceTargetWhenWorking = true;
		KSelectable component = base.GetComponent<KSelectable>();
		if (component != null)
		{
			component.SetStatusIndicatorOffset(new Vector3(0f, -0.65f, 0f));
		}
		this.OnTagsChanged(null);
		this.TryToOffsetIfBuried();
		DecorProvider component2 = base.GetComponent<DecorProvider>();
		if (component2 != null && string.IsNullOrEmpty(component2.overrideName))
		{
			component2.overrideName = UI.OVERLAYS.DECOR.CLUTTER;
		}
		this.UpdateEntombedVisualizer();
		base.Subscribe<Pickupable>(-1582839653, Pickupable.OnTagsChangedDelegate);
	}

	public void RegisterListeners()
	{
		if (this.cleaningUp)
		{
			return;
		}
		if (this.solidPartitionerEntry.IsValid())
		{
			return;
		}
		int num = Grid.PosToCell(this);
		this.objectLayerListItem = new ObjectLayerListItem(base.gameObject, ObjectLayer.Pickupables, num);
		this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Pickupable.RegisterSolidListener", base.gameObject, num, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Pickupable.RegisterPickupable", this, num, GameScenePartitioner.Instance.pickupablesLayer, null);
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange), "Pickupable.RegisterListeners");
		Singleton<CellChangeMonitor>.Instance.MarkDirty(base.transform);
	}

	public void UnregisterListeners()
	{
		if (this.objectLayerListItem != null)
		{
			this.objectLayerListItem.Clear();
			this.objectLayerListItem = null;
		}
		GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
	}

	private void OnSolidChanged(object data)
	{
		this.TryToOffsetIfBuried();
	}

	public void TryToOffsetIfBuried()
	{
		if (this.KPrefabID.HasTag(GameTags.Stored) || this.KPrefabID.HasTag(GameTags.Equipped))
		{
			return;
		}
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		DeathMonitor.Instance smi = base.gameObject.GetSMI<DeathMonitor.Instance>();
		bool flag = smi == null || smi.IsDead();
		if (flag && ((Grid.Solid[num] && Grid.Foundation[num]) || Grid.Properties[num] != 0))
		{
			for (int i = 0; i < Pickupable.displacementOffsets.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, Pickupable.displacementOffsets[i]);
				if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
				{
					Vector3 vector = Grid.CellToPosCBC(num2, Grid.SceneLayer.Move);
					KCollider2D component = base.GetComponent<KCollider2D>();
					if (component != null)
					{
						vector.y += base.transform.GetPosition().y - component.bounds.min.y;
					}
					base.transform.SetPosition(vector);
					num = num2;
					this.RemoveFaller();
					this.AddFaller(Vector2.zero);
					break;
				}
			}
		}
		this.HandleSolidCell(num);
	}

	private bool HandleSolidCell(int cell)
	{
		bool flag = this.IsEntombed;
		bool flag2 = false;
		if (Grid.IsValidCell(cell) && Grid.Solid[cell])
		{
			DeathMonitor.Instance smi = base.gameObject.GetSMI<DeathMonitor.Instance>();
			bool flag3 = smi == null || smi.IsDead();
			if (flag3)
			{
				this.Clearable.CancelClearing();
				flag2 = true;
			}
		}
		if (flag2 != flag && !this.KPrefabID.HasTag(GameTags.Stored))
		{
			this.IsEntombed = flag2;
			KSelectable component = base.GetComponent<KSelectable>();
			component.IsSelectable = !this.IsEntombed;
		}
		this.UpdateEntombedVisualizer();
		return this.IsEntombed;
	}

	private void OnCellChange()
	{
		Vector3 position = base.transform.GetPosition();
		int num = Grid.PosToCell(position);
		if (!Grid.IsValidCell(num))
		{
			Vector2 vector = new Vector2(-0.1f * (float)Grid.WidthInCells, 1.1f * (float)Grid.WidthInCells);
			Vector2 vector2 = new Vector2(-0.1f * (float)Grid.HeightInCells, 1.1f * (float)Grid.HeightInCells);
			if (position.x < vector.x || vector.y < position.x || position.y < vector2.x || vector2.y < position.y)
			{
				this.DeleteObject();
			}
		}
		else
		{
			bool flag = false;
			this.ReleaseEntombedVisualizerAndAddFaller(true);
			if (this.HandleSolidCell(num))
			{
				return;
			}
			this.objectLayerListItem.Update(num);
			if (!this.KPrefabID.HasTag(GameTags.Stored))
			{
				int num2 = Grid.CellBelow(num);
				if (Grid.IsValidCell(num2) && Grid.Solid[num2])
				{
					ObjectLayerListItem objectLayerListItem = this.objectLayerListItem.nextItem;
					while (objectLayerListItem != null)
					{
						GameObject gameObject = objectLayerListItem.gameObject;
						objectLayerListItem = objectLayerListItem.nextItem;
						Pickupable component = gameObject.GetComponent<Pickupable>();
						if (component != null && !flag)
						{
							flag = component.TryAbsorb(this, false, false);
						}
					}
				}
			}
			GameScenePartitioner.Instance.UpdatePosition(this.solidPartitionerEntry, num);
			GameScenePartitioner.Instance.UpdatePosition(this.partitionerEntry, num);
			if (!flag)
			{
				this.NotifyChanged(num);
			}
			if (Grid.IsValidCell(this.cachedCell) && num != this.cachedCell)
			{
				this.NotifyChanged(this.cachedCell);
			}
			this.UpdateCachedCell(num);
		}
	}

	private void OnTagsChanged(object data)
	{
		if (!this.KPrefabID.HasTag(GameTags.Stored) && !this.KPrefabID.HasTag(GameTags.Equipped))
		{
			this.RegisterListeners();
			this.AddFaller(Vector2.zero);
		}
		else
		{
			this.UnregisterListeners();
			this.RemoveFaller();
		}
	}

	private void NotifyChanged(int new_cell)
	{
		GameScenePartitioner.Instance.TriggerEvent(new_cell, GameScenePartitioner.Instance.pickupablesChangedLayer, this);
	}

	public bool TryAbsorb(Pickupable other, bool hide_effects, bool allow_cross_storage = false)
	{
		if (other == null)
		{
			return false;
		}
		if (other.wasAbsorbed)
		{
			return false;
		}
		if (this.wasAbsorbed)
		{
			return false;
		}
		if (!other.CanAbsorb(this))
		{
			return false;
		}
		if (!allow_cross_storage && this.storage == null != (other.storage == null))
		{
			return false;
		}
		this.Absorb(other);
		if (!hide_effects && EffectPrefabs.Instance != null)
		{
			Vector3 position = base.transform.GetPosition();
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
			GameObject gameObject = global::Util.KInstantiate(Assets.GetPrefab(EffectConfigs.OreAbsorbId), position, Quaternion.identity, null, null, true, 0);
			gameObject.SetActive(true);
		}
		return true;
	}

	protected override void OnCleanUp()
	{
		this.cleaningUp = true;
		this.ReleaseEntombedVisualizerAndAddFaller(false);
		this.RemoveFaller();
		if (this.storage)
		{
			this.storage.Remove(base.gameObject);
		}
		this.UnregisterListeners();
		Components.Pickupables.Remove(this);
		if (this.reservations.Count > 0)
		{
			this.reservations.Clear();
			if (this.OnReservationsChanged != null)
			{
				this.OnReservationsChanged();
			}
		}
		if (Grid.IsValidCell(this.cachedCell))
		{
			this.NotifyChanged(this.cachedCell);
		}
		base.OnCleanUp();
	}

	public Pickupable Take(float amount)
	{
		if (amount <= 0f)
		{
			return null;
		}
		if (this.OnTake == null)
		{
			if (this.storage != null)
			{
				this.storage.Remove(base.gameObject);
			}
			return this;
		}
		if (amount >= this.TotalAmount && this.storage != null)
		{
			this.storage.Remove(base.gameObject);
		}
		float num = Math.Min(this.TotalAmount, amount);
		if (num <= 0f)
		{
			return null;
		}
		return this.OnTake(num);
	}

	private void Absorb(Pickupable pickupable)
	{
		base.Trigger(-2064133523, pickupable);
		pickupable.Trigger(-1940207677, base.gameObject);
		pickupable.wasAbsorbed = true;
		KSelectable component = base.GetComponent<KSelectable>();
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == pickupable.GetComponent<KSelectable>())
		{
			SelectTool.Instance.Select(component, false);
		}
		pickupable.gameObject.DeleteObject();
		this.NotifyChanged(Grid.PosToCell(this));
	}

	private void RefreshStorageTags(object data = null)
	{
		bool flag = data is Storage || (data != null && (bool)data);
		if (flag)
		{
			this.KPrefabID.AddTag(GameTags.Stored);
			bool flag2 = this.storage == null || !this.storage.allowItemRemoval;
			if (flag2)
			{
				this.KPrefabID.AddTag(GameTags.StoredPrivate);
			}
			else
			{
				this.KPrefabID.RemoveTag(GameTags.StoredPrivate);
			}
		}
		else
		{
			this.KPrefabID.RemoveTag(GameTags.Stored);
			this.KPrefabID.RemoveTag(GameTags.StoredPrivate);
		}
	}

	public void OnStore(object data)
	{
		this.storage = data as Storage;
		bool flag = data is Storage || (data != null && (bool)data);
		SaveLoadRoot component = base.GetComponent<SaveLoadRoot>();
		if (this.carryAnimOverride != null && this.lastCarrier != null)
		{
			this.lastCarrier.RemoveAnimOverrides(this.carryAnimOverride);
			this.lastCarrier = null;
		}
		if (flag)
		{
			int cachedCell = this.cachedCell;
			this.RefreshStorageTags(data);
			if (this.storage != null)
			{
				if (this.carryAnimOverride != null && this.storage.GetComponent<Navigator>() != null)
				{
					this.lastCarrier = this.storage.GetComponent<KBatchedAnimController>();
					if (this.lastCarrier != null)
					{
						this.lastCarrier.AddAnimOverrides(this.carryAnimOverride, 0f);
					}
				}
				this.UpdateCachedCell(Grid.PosToCell(this.storage));
			}
			this.NotifyChanged(cachedCell);
			if (component != null)
			{
				component.SetRegistered(false);
			}
		}
		else
		{
			if (component != null)
			{
				component.SetRegistered(true);
			}
			this.RemovedFromStorage();
		}
	}

	private void RemovedFromStorage()
	{
		this.storage = null;
		this.UpdateCachedCell(Grid.PosToCell(this));
		this.RefreshStorageTags(null);
		this.AddFaller(Vector2.zero);
		KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
		component.enabled = true;
		base.gameObject.transform.rotation = Quaternion.identity;
		this.RegisterListeners();
		component.GetBatchInstanceData().ClearOverrideTransformMatrix();
	}

	private void UpdateCachedCell(int cell)
	{
		this.cachedCell = cell;
		this.GetOffsets(this.cachedCell);
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		if (this.useGunforPickup && worker.usesMultiTool)
		{
			Workable.AnimInfo anim = base.GetAnim(worker);
			anim.smi = new MultitoolController.Instance(this, worker, "pickup", Assets.GetPrefab(EffectConfigs.OreAbsorbId));
			return anim;
		}
		return base.GetAnim(worker);
	}

	public override void AwardExperience(float work_dt, MinionResume resume)
	{
		resume.AddExperienceIfRole("Hauler", work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
		resume.AddExperienceIfRole(MaterialsManager.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_VERY_SLOW);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage component = worker.GetComponent<Storage>();
		Pickupable.PickupableStartWorkInfo pickupableStartWorkInfo = (Pickupable.PickupableStartWorkInfo)worker.startWorkInfo;
		float amount = pickupableStartWorkInfo.amount;
		if (this != null)
		{
			Pickupable pickupable = this.Take(amount);
			if (pickupable != null)
			{
				component.Store(pickupable.gameObject, false, false, true, false);
				worker.workCompleteData = pickupable;
				pickupableStartWorkInfo.setResultCb(pickupable.gameObject);
			}
			else
			{
				pickupableStartWorkInfo.setResultCb(null);
			}
		}
		else
		{
			pickupableStartWorkInfo.setResultCb(null);
		}
	}

	public override Vector3 GetTargetPoint()
	{
		return base.transform.GetPosition();
	}

	public bool IsReachable()
	{
		return this.isReachable;
	}

	private void OnReachableChanged(object data)
	{
		this.isReachable = (bool)data;
		KSelectable component = base.GetComponent<KSelectable>();
		if (this.isReachable)
		{
			component.RemoveStatusItem(Db.Get().MiscStatusItems.PickupableUnreachable, false);
		}
		else
		{
			component.AddStatusItem(Db.Get().MiscStatusItems.PickupableUnreachable, this);
		}
	}

	private void AddFaller(Vector2 initial_velocity)
	{
		if (this.isKinematic || base.GetComponent<Health>() != null)
		{
			return;
		}
		if (!GameComps.Fallers.Has(base.gameObject))
		{
			GameComps.Fallers.Add(base.gameObject, initial_velocity);
		}
	}

	private void RemoveFaller()
	{
		if (base.GetComponent<Health>() != null)
		{
			return;
		}
		if (GameComps.Fallers.Has(base.gameObject))
		{
			GameComps.Fallers.Remove(base.gameObject);
		}
	}

	private void OnOreSizeChanged(object data)
	{
		Vector3 vector = Vector3.zero;
		HandleVector<int>.Handle handle = GameComps.Gravities.GetHandle(base.gameObject);
		if (handle.IsValid())
		{
			vector = GameComps.Gravities.GetData(handle).velocity;
		}
		this.RemoveFaller();
		if (!this.KPrefabID.HasTag(GameTags.Stored))
		{
			this.AddFaller(vector);
		}
	}

	private void OnLanded(object data)
	{
		if (CameraController.Instance == null)
		{
			return;
		}
		Vector3 position = base.transform.GetPosition();
		Vector2I vector2I = Grid.PosToXY(position);
		if (vector2I.x < 0 || Grid.WidthInCells <= vector2I.x || vector2I.y < 0 || Grid.HeightInCells <= vector2I.y)
		{
			this.DeleteObject();
			return;
		}
		Vector2 vector = (Vector2)data;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude <= 0.2f || SpeedControlScreen.Instance.IsPaused)
		{
			return;
		}
		Element element = this.primaryElement.Element;
		if (element.substance != null)
		{
			string text = element.substance.GetOreBumpSound();
			if (text == null)
			{
				if (element.HasTag(GameTags.RefinedMetal))
				{
					text = "RefinedMetal";
				}
				else if (element.HasTag(GameTags.Metal))
				{
					text = "RawMetal";
				}
				else
				{
					text = "Rock";
				}
			}
			if (element.tag.ToString() == "Creature" && !base.gameObject.HasTag(GameTags.Seed))
			{
				text = "Bodyfall_rock";
			}
			else
			{
				text = "Ore_bump_" + text;
			}
			string text2 = GlobalAssets.GetSound(text, true);
			text2 = ((text2 == null) ? GlobalAssets.GetSound("Ore_bump_rock", false) : text2);
			if (CameraController.Instance.IsAudibleSound(base.transform.GetPosition(), text2))
			{
				int num = Grid.PosToCell(position);
				bool isLiquid = Grid.Element[num].IsLiquid;
				float num2 = 0f;
				if (isLiquid)
				{
					num2 = SoundUtil.GetLiquidDepth(num);
				}
				FMOD.Studio.EventInstance eventInstance = KFMOD.BeginOneShot(text2, CameraController.Instance.GetVerticallyScaledPosition(base.transform.GetPosition()));
				eventInstance.setParameterValue("velocity", vector.magnitude);
				eventInstance.setParameterValue("liquidDepth", num2);
				KFMOD.EndOneShot(eventInstance);
			}
		}
	}

	private void UpdateEntombedVisualizer()
	{
		if (this.IsEntombed)
		{
			if (this.entombedCell == -1)
			{
				int num = Grid.PosToCell(this);
				if (EntombedItemManager.CanEntomb(this))
				{
					SaveGame.Instance.entombedItemManager.Add(this);
				}
				if (Grid.Objects[num, 1] == null)
				{
					KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
					if (component != null && Game.Instance.GetComponent<EntombedItemVisualizer>().AddItem(num))
					{
						this.entombedCell = num;
						component.enabled = false;
						this.RemoveFaller();
					}
				}
			}
		}
		else
		{
			this.ReleaseEntombedVisualizerAndAddFaller(true);
		}
	}

	private void ReleaseEntombedVisualizerAndAddFaller(bool add_faller_if_necessary)
	{
		if (this.entombedCell != -1)
		{
			Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(this.entombedCell);
			this.entombedCell = -1;
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.enabled = true;
			if (add_faller_if_necessary)
			{
				this.AddFaller(Vector2.zero);
			}
		}
	}

	[MyCmpReq]
	private PrimaryElement primaryElement;

	public const float WorkTime = 1.5f;

	[MyCmpReq]
	[NonSerialized]
	public KPrefabID KPrefabID;

	[MyCmpAdd]
	[NonSerialized]
	public Clearable Clearable;

	[MyCmpAdd]
	[NonSerialized]
	public Prioritizable prioritizable;

	public Func<Pickupable, bool> CanAbsorb = (Pickupable other) => false;

	public Func<float, Pickupable> OnTake;

	public global::System.Action OnReservationsChanged;

	public ObjectLayerListItem objectLayerListItem;

	public Workable targetWorkable;

	public KAnimFile carryAnimOverride;

	private KBatchedAnimController lastCarrier;

	public bool useGunforPickup = true;

	private static CellOffset[] displacementOffsets = new CellOffset[]
	{
		new CellOffset(0, 1),
		new CellOffset(0, -1),
		new CellOffset(1, 0),
		new CellOffset(-1, 0),
		new CellOffset(1, 1),
		new CellOffset(1, -1),
		new CellOffset(-1, 1),
		new CellOffset(-1, -1)
	};

	private bool isReachable;

	private bool isEntombed;

	private bool cleaningUp;

	public bool trackOnPickup = true;

	private int nextTicketNumber;

	private List<Pickupable.Reservation> reservations = new List<Pickupable.Reservation>();

	private HandleVector<int>.Handle solidPartitionerEntry;

	private HandleVector<int>.Handle partitionerEntry;

	private LoggerFSSF log;

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnStore(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnLandedDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnLanded(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnOreSizeChangedDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnOreSizeChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnReachableChanged(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> RefreshStorageTagsDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.RefreshStorageTags(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Pickupable> OnTagsChangedDelegate = new EventSystem.IntraObjectHandler<Pickupable>(delegate(Pickupable component, object data)
	{
		component.OnTagsChanged(data);
	});

	private int entombedCell = -1;

	private struct Reservation
	{
		public Reservation(GameObject reserver, float amount, int ticket)
		{
			this.reserver = reserver;
			this.amount = amount;
			this.ticket = ticket;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.reserver.name,
				", ",
				this.amount,
				", ",
				this.ticket
			});
		}

		public GameObject reserver;

		public float amount;

		public int ticket;
	}

	public class PickupableStartWorkInfo : Worker.StartWorkInfo
	{
		public PickupableStartWorkInfo(Pickupable pickupable, float amount, Action<GameObject> set_result_cb)
			: base(pickupable.targetWorkable)
		{
			this.originalPickupable = pickupable;
			this.amount = amount;
			this.setResultCb = set_result_cb;
		}

		public float amount { get; private set; }

		public Pickupable originalPickupable { get; private set; }

		public Action<GameObject> setResultCb { get; private set; }
	}
}
