using System;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class Pickupable : Workable
{
	private Pickupable()
	{
		this.showProgressBar = false;
		base.SetOffsetTable(OffsetGroups.InvertedStandardTable);
	}

	public PrimaryElement PrimaryElement
	{
		get
		{
			return this.primaryElement;
		}
	}

	public Storage storage { get; set; }

	public bool CountableUnits
	{
		get
		{
			return this.primaryElement.CountableUnits;
		}
	}

	public float MinTakeAmount
	{
		get
		{
			return 0f;
		}
	}

	public bool isKinematic { get; set; }

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
				this.Trigger(-1089732772, null);
				this.UpdateEntombedVisualizer();
			}
		}
	}

	public bool CouldBePickedUp(GameObject carrier)
	{
		bool flag = this.UnreservedAmount > 0f || this.GetReservedAmount(carrier) > 0f;
		bool flag2 = this.storage == null || this.storage.allowItemRemoval;
		bool flag3 = this.CanStillBePickedUp();
		bool flag4 = this.UnreservedAmount >= this.MinTakeAmount;
		return flag && flag2 && flag3 && flag4;
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

	public bool CanStillBePickedUp()
	{
		return Grid.IsValidCell(Grid.PosToCell(this)) && !this.IsEntombed;
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
			DebugUtil.Assert(this.primaryElement != null, "Assert!");
			this.primaryElement.Units = value;
			if (value <= 0f)
			{
				PrimaryElement component = base.GetComponent<PrimaryElement>();
				if (!component.KeepZeroMassObject)
				{
					base.gameObject.DeleteObject();
				}
			}
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

	public void ClearReservations()
	{
		this.reservations.Clear();
		this.RefreshReservedAmount();
	}

	public void PrintReservations()
	{
		foreach (Pickupable.Reservation reservation in this.reservations)
		{
			reservation.ToString();
		}
	}

	public int Reserve(string context, GameObject reserver, float amount)
	{
		int num = this.nextTicketNumber++;
		Pickupable.Reservation reservation = new Pickupable.Reservation(reserver, amount, num);
		this.reservations.Add(reservation);
		this.RefreshReservedAmount();
		return num;
	}

	public void Unreserve(string context, int ticket)
	{
		for (int i = 0; i < this.reservations.Count; i++)
		{
			if (this.reservations[i].ticket == ticket)
			{
				this.reservations.RemoveAt(i);
				break;
			}
		}
		this.RefreshReservedAmount();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.log = new LoggerFSSF("Pickupable");
		base.gameObject.layer = Game.PickupableLayer;
		Vector3 position = this.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Use);
		this.transform.SetPosition(position);
		this.Subscribe(856640610, new Action<object>(this.OnStore));
		this.Subscribe(1188683690, new Action<object>(this.OnLanded));
		this.Subscribe(1807976145, new Action<object>(this.OnOreSizeChanged));
		this.KPrefabID.AddTag(GameTags.Pickupable);
		Components.Pickupables.Add(this);
		base.SetWorkTime(1.5f);
		Collider2D component = base.GetComponent<Collider2D>();
		if (component != null)
		{
			component.sharedMaterial = Assets.defaultPhysicsMaterial;
		}
		this.workerStatusItem = Db.Get().DuplicantStatusItems.PickingUp;
		this.Subscribe(-1432940121, new Action<object>(this.OnReachableChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			global::Debug.LogWarning(string.Concat(new object[]
			{
				"Destroying GO [",
				base.name,
				"] because it is in an invalid position [",
				this.transform.position,
				"]"
			}), null);
			base.gameObject.DeleteObject();
			return;
		}
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
		if (this.storage == null && base.GetComponent<LiquidSource>() == null)
		{
			this.RegisterListeners();
		}
		if (this.OnGetAnim == null)
		{
			this.overrideAnims = new KAnimFile[] { Assets.GetAnim("anim_equip_kanim") };
		}
		if (this.storage == null)
		{
			this.AddFaller();
		}
		this.TryToOffsetIfBuried();
		DecorProvider component2 = base.GetComponent<DecorProvider>();
		if (component2 != null && string.IsNullOrEmpty(component2.overrideName))
		{
			component2.overrideName = UI.OVERLAYS.DECOR.CLUTTER;
		}
		this.rottable = this.GetSMI<Rottable.Instance>();
		this.UpdateEntombedVisualizer();
	}

	public void RegisterListeners()
	{
		if (this.solidPartitionerEntry != null)
		{
			return;
		}
		int num = Grid.PosToCell(this);
		this.objectLayerListItem = new ObjectLayerListItem(base.gameObject, ObjectLayer.Pickupables, num);
		this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Pickupable.RegisterSolidListener", base.gameObject, num, GameScenePartitioner.Instance.solidChangedMask.mask, new Action<object>(this.OnSolidChanged));
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Pickupable.RegisterPickupable", this, num, GameScenePartitioner.Instance.pickupables.mask, null);
		CellChangeMonitor.Instance.Add(this, new Action<int, int>(this.OnCellChange), false);
	}

	public void UnregisterListeners()
	{
		if (this.objectLayerListItem != null)
		{
			this.objectLayerListItem.Clear();
			this.objectLayerListItem = null;
		}
		if (this.solidPartitionerEntry != null)
		{
			this.solidPartitionerEntry.Release();
			this.solidPartitionerEntry = null;
		}
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
		CellChangeMonitor.Instance.Remove(this, new Action<int, int>(this.OnCellChange), false);
	}

	private void OnSolidChanged(object data)
	{
		this.TryToOffsetIfBuried();
	}

	public void TryToOffsetIfBuried()
	{
		if (this.storage != null)
		{
			return;
		}
		int num = Grid.PosToCell(this);
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		Health component = base.GetComponent<Health>();
		bool flag = component == null || component.IsDead();
		if (flag && ((Grid.Solid[num] && Grid.Foundation[num]) || Grid.Cell[num].properties != 0))
		{
			for (int i = 0; i < Pickupable.displacementOffsets.Length; i++)
			{
				int num2 = Grid.OffsetCell(num, Pickupable.displacementOffsets[i]);
				if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
				{
					Vector3 vector = Grid.CellToPosCBC(num2, Grid.SceneLayer.Move);
					Collider2D component2 = base.GetComponent<Collider2D>();
					if (component2 != null)
					{
						vector.y += this.transform.position.y - component2.bounds.min.y;
					}
					this.transform.SetPosition(vector);
					num = num2;
					this.RemoveFaller();
					this.AddFaller();
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
			Health component = base.GetComponent<Health>();
			bool flag3 = component == null || component.IsDead();
			if (flag3)
			{
				this.Clearable.CancelClearing();
				flag2 = true;
			}
		}
		if (flag2 != flag && this.storage == null)
		{
			this.IsEntombed = flag2;
			KSelectable component2 = base.GetComponent<KSelectable>();
			component2.IsSelectable = !this.IsEntombed;
		}
		this.UpdateEntombedVisualizer();
		return this.IsEntombed;
	}

	private void OnCellChange(int previous_cell, int new_cell)
	{
		if (!Grid.IsValidCell(new_cell))
		{
			this.DeleteObject();
		}
		else
		{
			this.ReleaseEntombedVisualizer();
			if (this.HandleSolidCell(new_cell))
			{
				return;
			}
			this.objectLayerListItem.Update(new_cell);
			if (this.storage == null)
			{
				ObjectLayerListItem objectLayerListItem = this.objectLayerListItem.nextItem;
				while (objectLayerListItem != null)
				{
					GameObject gameObject = objectLayerListItem.gameObject;
					objectLayerListItem = objectLayerListItem.nextItem;
					Pickupable component = gameObject.GetComponent<Pickupable>();
					if (component != null)
					{
						component.TryAbsorb(this, false);
					}
				}
			}
			this.solidPartitionerEntry.UpdatePosition(new_cell);
			this.partitionerEntry.UpdatePosition(new_cell);
		}
	}

	public bool TryAbsorb(Pickupable other, bool hide_effects)
	{
		if (other != null && this.CanAbsorb(other))
		{
			Pickupable component = other.GetComponent<Pickupable>();
			if (component != null && component.CanAbsorb(this))
			{
				this.Absorb(other);
				if (!hide_effects && EffectPrefabs.Instance != null)
				{
					Vector3 position = other.transform.position;
					position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
					global::Util.KInstantiate(EffectPrefabs.Instance.OreAbsorb, position, Quaternion.identity, SceneOrganizer.Instance.GetFolder(Folder.FX), null, true, 0);
				}
				return true;
			}
		}
		return false;
	}

	protected override void OnCleanUp()
	{
		this.ReleaseEntombedVisualizer();
		this.RemoveFaller();
		if (this.storage)
		{
			this.storage.Remove(base.gameObject);
		}
		this.UnregisterListeners();
		Components.Pickupables.Remove(this);
		base.OnCleanUp();
	}

	public Pickupable Take(float amount)
	{
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

	public void Absorb(Pickupable pickupable)
	{
		if (pickupable.wasAbsorbed)
		{
			return;
		}
		this.Trigger(-2064133523, pickupable);
		this.TotalAmount += pickupable.TotalAmount;
		pickupable.Trigger(-1940207677, base.gameObject);
		pickupable.wasAbsorbed = true;
		if (SelectTool.Instance != null && SelectTool.Instance.selected != null && SelectTool.Instance.selected == pickupable.selectable)
		{
			SelectTool.Instance.Select(this.selectable, false);
		}
		pickupable.gameObject.DeleteObject();
	}

	public void OnStore(object data)
	{
		this.storage = (Storage)data;
		if (this.storage != null)
		{
			this.RemoveFaller();
			this.UnregisterListeners();
			Collider2D component = base.GetComponent<Collider2D>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
		else
		{
			this.AddFaller();
			base.GetComponent<KAnimControllerBase>().enabled = true;
			base.gameObject.transform.rotation = Quaternion.identity;
			Collider2D component2 = base.GetComponent<Collider2D>();
			if (component2 != null)
			{
				component2.enabled = true;
			}
			this.RegisterListeners();
		}
	}

	public override HashedString[] GetWorkAnims(Worker worker)
	{
		if (this.OnGetAnim == null)
		{
			return base.GetWorkAnims(worker);
		}
		return null;
	}

	public override Workable.AnimInfo GetAnim(Worker worker)
	{
		if (this.OnGetAnim != null)
		{
			return this.OnGetAnim(worker);
		}
		Workable.AnimInfo anim = base.GetAnim(worker);
		anim.smi = new MultitoolController.Instance(this, worker, "pickup", EffectPrefabs.Instance.PickupEffect);
		return anim;
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Storage component = worker.GetComponent<Storage>();
		float amount = worker.amount;
		Pickupable pickupable = this.Take(amount);
		if (pickupable != null)
		{
			component.Store(pickupable.gameObject, false, false);
			worker.workCompleteData = pickupable;
		}
	}

	public override Vector3 GetTargetPoint()
	{
		return this.transform.position;
	}

	public bool IsReachable()
	{
		return this.isReachable;
	}

	private void OnReachableChanged(object data)
	{
		this.isReachable = (bool)data;
		if (this.isReachable)
		{
			this.selectable.RemoveStatusItem(Db.Get().MiscStatusItems.PickupableUnreachable, false);
		}
		else
		{
			try
			{
				this.selectable.AddStatusItem(Db.Get().MiscStatusItems.PickupableUnreachable, this);
			}
			catch
			{
				global::Debug.Log("!", null);
			}
		}
	}

	private void AddFaller()
	{
		if (this.isKinematic || base.GetComponent<Health>() != null)
		{
			return;
		}
		if (!GameComps.Fallers.Has(base.gameObject))
		{
			GameComps.Fallers.Add(base.gameObject, Vector2.zero);
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
		this.RemoveFaller();
		if (this.storage == null)
		{
			this.AddFaller();
		}
	}

	private void OnLanded(object data)
	{
		Vector2 vector = (Vector2)data;
		float sqrMagnitude = vector.sqrMagnitude;
		if (sqrMagnitude <= 0f || SpeedControlScreen.Instance.IsPaused)
		{
			return;
		}
		if (CameraController.Instance == null)
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
			if (element.tag.ToString() == "Creature")
			{
				text = "Bodyfall_rock";
			}
			else
			{
				text = "Ore_bump_" + text;
			}
			string text2 = GlobalAssets.GetSound(text, false);
			text2 = ((text2 == null) ? GlobalAssets.GetSound("Ore_bump_rock", false) : text2);
			if (CameraController.Instance.IsAudibleSound(this.transform.position, text2))
			{
				int num = Grid.PosToCell(this.transform.position);
				bool isLiquid = Grid.Element[num].IsLiquid;
				float num2 = 0f;
				if (isLiquid)
				{
					num2 = SoundUtil.GetLiquidDepth(num);
				}
				EventInstance eventInstance = KFMOD.BeginOneShot(text2, CameraController.Instance.GetVerticallyScaledPosition(this.transform.position));
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
				int num = Grid.PosToCell(this.transform.position);
				if (Grid.Objects[num, 9] == null && Grid.Element[num].id != SimHashes.SteelDoor)
				{
					KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
					if (component != null && Game.Instance.GetComponent<EntombedItemVisualizer>().AddItem(num))
					{
						this.entombedCell = num;
						component.enabled = false;
						Collider2D component2 = base.GetComponent<Collider2D>();
						component2.enabled = false;
						this.RemoveFaller();
					}
				}
			}
		}
		else
		{
			this.ReleaseEntombedVisualizer();
		}
	}

	private void ReleaseEntombedVisualizer()
	{
		if (this.entombedCell != -1)
		{
			Game.Instance.GetComponent<EntombedItemVisualizer>().RemoveItem(this.entombedCell);
			this.entombedCell = -1;
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.enabled = true;
			Collider2D component2 = base.GetComponent<Collider2D>();
			component2.enabled = true;
			this.AddFaller();
		}
	}

	public const float WorkTime = 1.5f;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[SerializeField]
	private bool hideWhenStored;

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

	public Func<Worker, Workable.AnimInfo> OnGetAnim;

	public ObjectLayerListItem objectLayerListItem;

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

	public Rottable.Instance rottable;

	private bool isReachable;

	private bool isEntombed;

	private bool wasAbsorbed;

	private int nextTicketNumber;

	private List<Pickupable.Reservation> reservations = new List<Pickupable.Reservation>();

	private GameScenePartitionerEntry solidPartitionerEntry;

	private GameScenePartitionerEntry partitionerEntry;

	private LoggerFSSF log;

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
}
