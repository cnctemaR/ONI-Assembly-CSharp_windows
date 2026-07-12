using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

public class StorageTile : GameStateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.idle;
		this.root.PlayAnim("on").EventHandler(GameHashes.OnStorageChange, new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State.Callback(StorageTile.OnStorageChanged)).EventHandler(GameHashes.StorageTileTargetItemChanged, new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State.Callback(StorageTile.RefreshContentVisuals));
		this.idle.Enter(new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State.Callback(StorageTile.RefreshContentVisuals)).EventTransition(GameHashes.OnStorageChange, this.awaitingDelivery, new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.Transition.ConditionCallback(StorageTile.IsAwaitingDelivery)).EventTransition(GameHashes.StorageTileTargetItemChanged, this.change, new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.Transition.ConditionCallback(StorageTile.IsAwaitingForSettingChange));
		this.change.Enter(new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State.Callback(StorageTile.RefreshContentVisuals)).EventTransition(GameHashes.StorageTileTargetItemChanged, this.idle, new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.Transition.ConditionCallback(StorageTile.NoLongerAwaitingForSettingChange)).DefaultState(this.change.awaitingSettingsChange);
		this.change.awaitingSettingsChange.Enter(new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State.Callback(StorageTile.StartWorkChore)).Exit(new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State.Callback(StorageTile.CancelWorkChore)).ToggleStatusItem(Db.Get().BuildingStatusItems.ChangeStorageTileTarget, null)
			.WorkableCompleteTransition((StorageTile.Instance smi) => smi.GetWorkable(), this.change.complete);
		this.change.complete.Enter(new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State.Callback(StorageTile.ApplySettings)).Enter(new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State.Callback(StorageTile.DropUndesiredItems)).EnterTransition(this.idle, new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.Transition.ConditionCallback(StorageTile.HasAnyDesiredItemStored))
			.EnterTransition(this.awaitingDelivery, new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.Transition.ConditionCallback(StorageTile.IsAwaitingDelivery));
		this.awaitingDelivery.Enter(new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State.Callback(StorageTile.RefreshContentVisuals)).EventTransition(GameHashes.OnStorageChange, this.idle, new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.Transition.ConditionCallback(StorageTile.HasAnyDesiredItemStored)).EventTransition(GameHashes.StorageTileTargetItemChanged, this.change, new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.Transition.ConditionCallback(StorageTile.IsAwaitingForSettingChange));
	}

	public static void DropUndesiredItems(StorageTile.Instance smi)
	{
		smi.DropUndesiredItems();
	}

	public static void ApplySettings(StorageTile.Instance smi)
	{
		smi.ApplySettings();
	}

	public static void StartWorkChore(StorageTile.Instance smi)
	{
		smi.StartChangeSettingChore();
	}

	public static void CancelWorkChore(StorageTile.Instance smi)
	{
		smi.CanceChangeSettingChore();
	}

	public static void RefreshContentVisuals(StorageTile.Instance smi)
	{
		smi.UpdateContentSymbol();
	}

	public static bool IsAwaitingForSettingChange(StorageTile.Instance smi)
	{
		return smi.IsPendingChange;
	}

	public static bool NoLongerAwaitingForSettingChange(StorageTile.Instance smi)
	{
		return !smi.IsPendingChange;
	}

	public static bool HasAnyDesiredItemStored(StorageTile.Instance smi)
	{
		return smi.HasAnyDesiredContents;
	}

	public static void OnStorageChanged(StorageTile.Instance smi)
	{
		smi.PlayDoorAnimation();
		StorageTile.RefreshContentVisuals(smi);
	}

	public static bool IsAwaitingDelivery(StorageTile.Instance smi)
	{
		return !smi.IsPendingChange && !smi.HasAnyDesiredContents;
	}

	public const string METER_TARGET = "meter_target";

	public const string METER_ANIMATION = "meter";

	public static HashedString DOOR_SYMBOL_NAME = new HashedString("storage_door");

	public static HashedString ITEM_SYMBOL_TARGET = new HashedString("meter_target_object");

	public static HashedString ITEM_SYMBOL_NAME = new HashedString("object");

	public const string ITEM_SYMBOL_ANIMATION = "meter_object";

	public static HashedString ITEM_PREVIEW_SYMBOL_TARGET = new HashedString("meter_target_object_ui");

	public static HashedString ITEM_PREVIEW_SYMBOL_NAME = new HashedString("object_ui");

	public const string ITEM_PREVIEW_SYMBOL_ANIMATION = "meter_object_ui";

	public static HashedString ITEM_PREVIEW_BACKGROUND_SYMBOL_NAME = new HashedString("placeholder");

	public const string DEFAULT_ANIMATION_NAME = "on";

	public const string STORAGE_CHANGE_ANIMATION_NAME = "door";

	public const string SYMBOL_ANIMATION_NAME_AWAITING_DELIVERY = "ui";

	public static Tag INVALID_TAG = GameTags.Void;

	private StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.TagParameter TargetItemTag = new StateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.TagParameter(StorageTile.INVALID_TAG);

	public GameStateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State idle;

	public StorageTile.SettingsChangeStates change;

	public GameStateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State awaitingDelivery;

	public class SpecificItemTagSizeInstruction
	{
		public SpecificItemTagSizeInstruction(Tag tag, float size)
		{
			this.tag = tag;
			this.sizeMultiplier = size;
		}

		public Tag tag;

		public float sizeMultiplier;
	}

	public class Def : StateMachine.BaseDef
	{
		public StorageTile.SpecificItemTagSizeInstruction GetSizeInstructionForObject(GameObject obj)
		{
			if (this.specialItemCases == null)
			{
				return null;
			}
			KPrefabID component = obj.GetComponent<KPrefabID>();
			foreach (StorageTile.SpecificItemTagSizeInstruction specificItemTagSizeInstruction in this.specialItemCases)
			{
				if (component.HasTag(specificItemTagSizeInstruction.tag))
				{
					return specificItemTagSizeInstruction;
				}
			}
			return null;
		}

		public float MaxCapacity;

		public StorageTile.SpecificItemTagSizeInstruction[] specialItemCases;
	}

	public class SettingsChangeStates : GameStateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State
	{
		public GameStateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State awaitingSettingsChange;

		public GameStateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.State complete;
	}

	public new class Instance : GameStateMachine<StorageTile, StorageTile.Instance, IStateMachineTarget, StorageTile.Def>.GameInstance, IUserControlledCapacity
	{
		public Tag TargetTag
		{
			get
			{
				return base.smi.sm.TargetItemTag.Get(base.smi);
			}
		}

		public bool HasContents
		{
			get
			{
				return this.storage.MassStored() > 0f;
			}
		}

		public bool HasAnyDesiredContents
		{
			get
			{
				if (!(this.TargetTag == StorageTile.INVALID_TAG))
				{
					return this.AmountOfDesiredContentStored > 0f;
				}
				return !this.HasContents;
			}
		}

		public float AmountOfDesiredContentStored
		{
			get
			{
				if (!(this.TargetTag == StorageTile.INVALID_TAG))
				{
					return this.storage.GetMassAvailable(this.TargetTag);
				}
				return 0f;
			}
		}

		public bool IsPendingChange
		{
			get
			{
				return this.GetTreeFilterableCurrentTag() != this.TargetTag;
			}
		}

		public float UserMaxCapacity
		{
			get
			{
				return Mathf.Min(this.userMaxCapacity, this.storage.capacityKg);
			}
			set
			{
				this.userMaxCapacity = value;
				this.filteredStorage.FilterChanged();
				this.RefreshAmountMeter();
			}
		}

		public float AmountStored
		{
			get
			{
				return this.storage.MassStored();
			}
		}

		public float MinCapacity
		{
			get
			{
				return 0f;
			}
		}

		public float MaxCapacity
		{
			get
			{
				return base.def.MaxCapacity;
			}
		}

		public bool WholeValues
		{
			get
			{
				return false;
			}
		}

		public LocString CapacityUnits
		{
			get
			{
				return GameUtil.GetCurrentMassUnit(false);
			}
		}

		private Tag GetTreeFilterableCurrentTag()
		{
			if (this.treeFilterable.GetTags() != null && this.treeFilterable.GetTags().Count != 0)
			{
				return this.treeFilterable.GetTags().GetRandom<Tag>();
			}
			return StorageTile.INVALID_TAG;
		}

		public StorageTileSwitchItemWorkable GetWorkable()
		{
			return base.smi.gameObject.GetComponent<StorageTileSwitchItemWorkable>();
		}

		public Instance(IStateMachineTarget master, StorageTile.Def def)
			: base(master, def)
		{
			this.itemSymbol = this.CreateSymbolOverrideCapsule(StorageTile.ITEM_SYMBOL_TARGET, StorageTile.ITEM_SYMBOL_NAME, "meter_object");
			this.itemSymbol.usingNewSymbolOverrideSystem = true;
			this.itemSymbolOverrideController = SymbolOverrideControllerUtil.AddToPrefab(this.itemSymbol.gameObject);
			this.itemPreviewSymbol = this.CreateSymbolOverrideCapsule(StorageTile.ITEM_PREVIEW_SYMBOL_TARGET, StorageTile.ITEM_PREVIEW_SYMBOL_NAME, "meter_object_ui");
			this.defaultItemSymbolScale = this.itemSymbol.transform.localScale.x;
			this.defaultItemLocalPosition = this.itemSymbol.transform.localPosition;
			this.doorSymbol = this.CreateEmptyKAnimController(StorageTile.DOOR_SYMBOL_NAME.ToString());
			this.doorSymbol.initialAnim = "on";
			foreach (KAnim.Build.Symbol symbol in this.doorSymbol.AnimFiles[0].GetData().build.symbols)
			{
				this.doorSymbol.SetSymbolVisiblity(symbol.hash, symbol.hash == StorageTile.DOOR_SYMBOL_NAME);
			}
			this.doorSymbol.transform.SetParent(this.animController.transform, false);
			this.doorSymbol.transform.SetLocalPosition(-Vector3.forward * 0.05f);
			this.doorSymbol.onAnimComplete += this.OnDoorAnimationCompleted;
			this.doorSymbol.gameObject.SetActive(true);
			this.animController.SetSymbolVisiblity(StorageTile.DOOR_SYMBOL_NAME, false);
			this.doorAnimLink = new KAnimLink(this.animController, this.doorSymbol);
			this.amountMeter = new MeterController(this.animController, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			ChoreType choreType = Db.Get().ChoreTypes.Get(this.choreTypeID);
			this.filteredStorage = new FilteredStorage(this.storage, null, this, false, choreType);
			base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
			base.Subscribe(1606648047, new Action<object>(this.OnObjectReplaced));
		}

		public override void StartSM()
		{
			base.StartSM();
			this.filteredStorage.FilterChanged();
		}

		private void OnObjectReplaced(object data)
		{
			Constructable.ReplaceCallbackParameters replaceCallbackParameters = (Constructable.ReplaceCallbackParameters)data;
			List<GameObject> list = new List<GameObject>();
			this.storage.DropAll(false, false, default(Vector3), true, list);
			if (replaceCallbackParameters.Worker != null)
			{
				foreach (GameObject gameObject in list)
				{
					gameObject.GetComponent<Pickupable>().Trigger(580035959, replaceCallbackParameters.Worker);
				}
			}
		}

		private void OnDoorAnimationCompleted(HashedString animName)
		{
			if (animName == "door")
			{
				this.doorSymbol.Play("on", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		private KBatchedAnimController CreateEmptyKAnimController(string name)
		{
			GameObject gameObject = new GameObject(base.gameObject.name + "-" + name);
			gameObject.SetActive(false);
			KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
			kbatchedAnimController.AnimFiles = new KAnimFile[] { Assets.GetAnim("storagetile_kanim") };
			kbatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingFront;
			return kbatchedAnimController;
		}

		private KBatchedAnimController CreateSymbolOverrideCapsule(HashedString symbolTarget, HashedString symbolName, string animationName)
		{
			KBatchedAnimController kbatchedAnimController = this.CreateEmptyKAnimController(symbolTarget.ToString());
			kbatchedAnimController.initialAnim = animationName;
			bool flag;
			Matrix4x4 symbolTransform = this.animController.GetSymbolTransform(symbolTarget, out flag);
			bool flag2;
			Matrix2x3 symbolLocalTransform = this.animController.GetSymbolLocalTransform(symbolTarget, out flag2);
			Vector3 vector = symbolTransform.GetColumn(3);
			Vector3 vector2 = Vector3.one * symbolLocalTransform.m00;
			kbatchedAnimController.transform.SetParent(base.transform, false);
			kbatchedAnimController.transform.SetPosition(vector);
			Vector3 localPosition = kbatchedAnimController.transform.localPosition;
			localPosition.z = -0.0025f;
			kbatchedAnimController.transform.localPosition = localPosition;
			kbatchedAnimController.transform.localScale = vector2;
			kbatchedAnimController.gameObject.SetActive(false);
			this.animController.SetSymbolVisiblity(symbolTarget, false);
			return kbatchedAnimController;
		}

		private void OnCopySettings(object sourceOBJ)
		{
			if (sourceOBJ != null)
			{
				StorageTile.Instance smi = ((GameObject)sourceOBJ).GetSMI<StorageTile.Instance>();
				if (smi != null)
				{
					this.SetTargetItem(smi.TargetTag);
					this.UserMaxCapacity = smi.UserMaxCapacity;
				}
			}
		}

		public void RefreshAmountMeter()
		{
			float num = ((this.UserMaxCapacity == 0f) ? 0f : Mathf.Clamp(this.AmountOfDesiredContentStored / this.UserMaxCapacity, 0f, 1f));
			this.amountMeter.SetPositionPercent(num);
		}

		public void PlayDoorAnimation()
		{
			this.doorSymbol.Play("door", KAnim.PlayMode.Once, 1f, 0f);
		}

		public void SetTargetItem(Tag tag)
		{
			base.sm.TargetItemTag.Set(tag, this, false);
			base.gameObject.Trigger(-2076953849, null);
		}

		public void ApplySettings()
		{
			Tag treeFilterableCurrentTag = this.GetTreeFilterableCurrentTag();
			this.treeFilterable.RemoveTagFromFilter(treeFilterableCurrentTag);
		}

		public void DropUndesiredItems()
		{
			Vector3 vector = Grid.CellToPos(this.GetWorkable().LastCellWorkerUsed) + Vector3.right * Grid.CellSizeInMeters * 0.5f + Vector3.up * Grid.CellSizeInMeters * 0.5f;
			vector.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			if (this.TargetTag != StorageTile.INVALID_TAG)
			{
				this.treeFilterable.AddTagToFilter(this.TargetTag);
				GameObject[] array = this.storage.DropUnlessHasTag(this.TargetTag);
				if (array != null)
				{
					GameObject[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i].transform.SetPosition(vector);
					}
				}
			}
			else
			{
				this.storage.DropAll(vector, false, false, default(Vector3), true, null);
			}
			this.storage.DropUnlessHasTag(this.TargetTag);
		}

		public void UpdateContentSymbol()
		{
			this.RefreshAmountMeter();
			bool flag = this.TargetTag == StorageTile.INVALID_TAG;
			if (flag && !this.HasContents)
			{
				this.itemSymbol.gameObject.SetActive(false);
				this.itemPreviewSymbol.gameObject.SetActive(false);
				this.animController.SetSymbolVisiblity(StorageTile.ITEM_PREVIEW_BACKGROUND_SYMBOL_NAME, false);
				return;
			}
			bool flag2 = !flag && (this.IsPendingChange || !this.HasAnyDesiredContents);
			string text = "";
			GameObject gameObject = ((this.TargetTag == StorageTile.INVALID_TAG) ? Assets.GetPrefab(this.storage.items[0].PrefabID()) : Assets.GetPrefab(this.TargetTag));
			KAnimFile animFileFromPrefabWithTag = global::Def.GetAnimFileFromPrefabWithTag(gameObject, flag2 ? "ui" : "", out text);
			this.animController.SetSymbolVisiblity(StorageTile.ITEM_PREVIEW_BACKGROUND_SYMBOL_NAME, flag2);
			this.itemPreviewSymbol.gameObject.SetActive(flag2);
			this.itemSymbol.gameObject.SetActive(!flag2);
			if (flag2)
			{
				this.itemPreviewSymbol.SwapAnims(new KAnimFile[] { animFileFromPrefabWithTag });
				this.itemPreviewSymbol.Play(text, KAnim.PlayMode.Once, 1f, 0f);
				return;
			}
			if (gameObject.HasTag(GameTags.Egg))
			{
				string text2 = text;
				if (!string.IsNullOrEmpty(text2))
				{
					this.itemSymbolOverrideController.ApplySymbolOverridesByAffix(animFileFromPrefabWithTag, text2, null, 0);
				}
				text = gameObject.GetComponent<KBatchedAnimController>().initialAnim;
			}
			else
			{
				this.itemSymbolOverrideController.RemoveAllSymbolOverrides(0);
				text = gameObject.GetComponent<KBatchedAnimController>().initialAnim;
			}
			this.itemSymbol.SwapAnims(new KAnimFile[] { animFileFromPrefabWithTag });
			this.itemSymbol.Play(text, KAnim.PlayMode.Once, 1f, 0f);
			StorageTile.SpecificItemTagSizeInstruction sizeInstructionForObject = base.def.GetSizeInstructionForObject(gameObject);
			this.itemSymbol.transform.localScale = Vector3.one * ((sizeInstructionForObject != null) ? sizeInstructionForObject.sizeMultiplier : this.defaultItemSymbolScale);
			KCollider2D component = gameObject.GetComponent<KCollider2D>();
			Vector3 vector = this.defaultItemLocalPosition;
			vector.y += ((component == null || component is KCircleCollider2D) ? 0f : (-component.offset.y * 0.5f));
			this.itemSymbol.transform.localPosition = vector;
		}

		private void AbortChore()
		{
			if (this.chore != null)
			{
				this.chore.Cancel("Change settings Chore aborted");
				this.chore = null;
			}
		}

		public void StartChangeSettingChore()
		{
			this.AbortChore();
			this.chore = new WorkChore<StorageTileSwitchItemWorkable>(Db.Get().ChoreTypes.Toggle, this.GetWorkable(), null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		}

		public void CanceChangeSettingChore()
		{
			this.AbortChore();
		}

		[Serialize]
		private float userMaxCapacity = float.PositiveInfinity;

		[MyCmpGet]
		private Storage storage;

		[MyCmpGet]
		private KBatchedAnimController animController;

		[MyCmpGet]
		private TreeFilterable treeFilterable;

		private FilteredStorage filteredStorage;

		private Chore chore;

		private MeterController amountMeter;

		private KBatchedAnimController doorSymbol;

		private KBatchedAnimController itemSymbol;

		private SymbolOverrideController itemSymbolOverrideController;

		private KBatchedAnimController itemPreviewSymbol;

		private KAnimLink doorAnimLink;

		private string choreTypeID = Db.Get().ChoreTypes.StorageFetch.Id;

		private float defaultItemSymbolScale = -1f;

		private Vector3 defaultItemLocalPosition = Vector3.zero;
	}
}
