using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

public class HijackedHeadquarters : GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>
{
	public static bool IsReadyToPrint(HijackedHeadquarters.Instance smi, int charges)
	{
		return charges >= 3;
	}

	public static bool IsOperational(HijackedHeadquarters.Instance smi)
	{
		return smi.GetComponent<Operational>().IsOperational;
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.inoperational;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.root.Enter(delegate(HijackedHeadquarters.Instance smi)
		{
			smi.UpdateMeter();
		}).EventHandler(GameHashes.BuildingActivated, delegate(HijackedHeadquarters.Instance smi, object activated)
		{
			if (((Boxed<bool>)activated).value)
			{
				StoryManager.Instance.BeginStoryEvent(Db.Get().Stories.HijackedHeadquarters);
			}
		});
		this.inoperational.PlayAnim("inactive").EventTransition(GameHashes.OperationalChanged, this.operational.passcode.idle_locked, (HijackedHeadquarters.Instance smi) => smi.GetComponent<Operational>().IsOperational);
		this.operational.DefaultState(this.operational.passcode.idle_locked).ParamTransition<int>(this.interceptCharges, this.operational.readyToPrint.pre, new StateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.Parameter<int>.Callback(HijackedHeadquarters.IsReadyToPrint)).EventTransition(GameHashes.OperationalChanged, this.inoperational, (HijackedHeadquarters.Instance smi) => !smi.GetComponent<Operational>().IsOperational)
			.Update(delegate(HijackedHeadquarters.Instance smi, float dt)
			{
				smi.UpdateMeter();
			}, UpdateRate.SIM_200ms, false);
		this.operational.passcode.idle_locked.ParamTransition<bool>(this.passcodeUnlocked, this.operational.passcode.unlocking, GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.IsTrue).PlayAnim("idle_locked", KAnim.PlayMode.Once);
		this.operational.passcode.unlocking.PlayAnim("unlocking", KAnim.PlayMode.Once).OnAnimQueueComplete(this.operational.passcode.idle_unlocked);
		this.operational.passcode.idle_unlocked.PlayAnim("idle_unlocked", KAnim.PlayMode.Loop).Enter(delegate(HijackedHeadquarters.Instance smi)
		{
			smi.AddLore();
		}).Enter(delegate(HijackedHeadquarters.Instance smi)
		{
			smi.ChangeUIDescriptionToCompleted();
		})
			.Update(delegate(HijackedHeadquarters.Instance smi, float dt)
			{
				if (Immigration.Instance.ImmigrantsAvailable)
				{
					smi.GoTo(this.operational.interceptPre);
				}
			}, UpdateRate.SIM_200ms, false);
		this.operational.interceptPre.PlayAnim("intercept_pre").OnAnimQueueComplete(this.operational.interceptLoop);
		this.operational.interceptLoop.PlayAnim("intercept_loop", KAnim.PlayMode.Loop).Update(delegate(HijackedHeadquarters.Instance smi, float dt)
		{
			if (!Immigration.Instance.ImmigrantsAvailable)
			{
				smi.GoTo(this.operational.interceptPst);
			}
		}, UpdateRate.SIM_200ms, false);
		this.operational.interceptPst.PlayAnim("intercept").OnAnimQueueComplete(this.operational.passcode.idle_unlocked);
		this.operational.readyToPrint.DefaultState(this.operational.readyToPrint.pre).EventTransition(GameHashes.PrinterceptorPrint, this.operational.readyToPrint.pst, null);
		this.operational.readyToPrint.pre.PlayAnim("print_ready_pre").OnAnimQueueComplete(this.operational.readyToPrint.loop);
		this.operational.readyToPrint.loop.QueueAnim("print_ready", false, null).QueueAnim("print_ready_loop", true, null);
		this.operational.readyToPrint.pst.PlayAnim("printing").ScheduleAction("PrinterceptorPrintDelay", 1f, delegate(HijackedHeadquarters.Instance smi)
		{
			smi.PrintSelectedEntity();
		}).Exit(delegate(HijackedHeadquarters.Instance smi)
		{
			if (!smi.sm.hasBeenCompleted.Get(smi))
			{
				smi.sm.hasBeenCompleted.Set(true, smi, false);
				smi.ShowCompletedNotification();
			}
		})
			.OnAnimQueueComplete(this.operational.passcode.idle_unlocked);
	}

	public StateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.IntParameter interceptCharges;

	public StateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.BoolParameter passcodeUnlocked;

	public StateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.BoolParameter hasBeenCompleted;

	public const int MAX_INTERCEPT_CHARGES = 3;

	public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State inoperational;

	public HijackedHeadquarters.OperationalStates operational;

	public class Def : StateMachine.BaseDef
	{
	}

	public class OperationalStates : GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State
	{
		public HijackedHeadquarters.PasscodeStates passcode;

		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State interceptPre;

		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State interceptLoop;

		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State interceptPst;

		public HijackedHeadquarters.ReadyToPrintStates readyToPrint;

		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State printing;
	}

	public class PasscodeStates : GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State
	{
		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State idle_locked;

		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State unlocking;

		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State idle_unlocked;
	}

	public class ReadyToPrintStates : GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State
	{
		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State pre;

		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State loop;

		public GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.State pst;
	}

	public new class Instance : GameStateMachine<HijackedHeadquarters, HijackedHeadquarters.Instance, IStateMachineTarget, HijackedHeadquarters.Def>.GameInstance, IUserControlledCapacity
	{
		float IUserControlledCapacity.UserMaxCapacity
		{
			get
			{
				return this.userMaxCapacity;
			}
			set
			{
				this.userMaxCapacity = value;
				this.ApplyMaxCapacity();
			}
		}

		float IUserControlledCapacity.AmountStored
		{
			get
			{
				return this.m_storage.MassStored();
			}
		}

		float IUserControlledCapacity.MinCapacity
		{
			get
			{
				return 0f;
			}
		}

		float IUserControlledCapacity.MaxCapacity
		{
			get
			{
				return 500f;
			}
		}

		bool IUserControlledCapacity.WholeValues
		{
			get
			{
				return true;
			}
		}

		LocString IUserControlledCapacity.CapacityUnits
		{
			get
			{
				return DatabankHelper.NAME_PLURAL;
			}
		}

		bool IUserControlledCapacity.ControlEnabled()
		{
			return base.smi.sm.passcodeUnlocked.Get(base.smi);
		}

		public void ApplyMaxCapacity()
		{
			this.m_storage.capacityKg = this.userMaxCapacity;
			this.m_storage.GetComponent<ManualDeliveryKG>().AbortDelivery("Switching to new delivery request");
			this.m_storage.GetComponent<ManualDeliveryKG>().capacity = this.userMaxCapacity;
			this.m_storage.GetComponent<ManualDeliveryKG>().refillMass = this.userMaxCapacity;
			this.m_storage.GetComponent<ManualDeliveryKG>().FillToCapacity = true;
			this.m_storage.Trigger(-945020481, this);
			if (this.m_storage.MassStored() > this.userMaxCapacity)
			{
				this.m_storage.DropSome(DatabankHelper.ID, this.m_storage.MassStored() - this.userMaxCapacity, false, false, default(Vector3), true, false);
			}
		}

		public Instance(IStateMachineTarget master, HijackedHeadquarters.Def def)
			: base(master, def)
		{
			this.m_progressMeter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			HijackedHeadquarters.Instance.PrinterceptorInstance = base.smi.master.gameObject;
		}

		public void ChangeUIDescriptionToCompleted()
		{
			BuildingComplete component = base.gameObject.GetComponent<BuildingComplete>();
			base.gameObject.GetComponent<KSelectable>().SetName(BUILDINGS.PREFABS.HIJACKEDHEADQUARTERS_COMPLETED.NAME);
			component.SetDescriptionFlavour(BUILDINGS.PREFABS.HIJACKEDHEADQUARTERS_COMPLETED.EFFECT);
			component.SetDescription(BUILDINGS.PREFABS.HIJACKEDHEADQUARTERS_COMPLETED.DESC);
		}

		public void AddLore()
		{
			if (StoryManager.Instance.IsStoryComplete(Db.Get().Stories.HijackedHeadquarters) && base.smi.master.GetComponent<LoreBearer>() == null)
			{
				LoreBearerUtil.AddLoreTo(base.smi.master.gameObject, LoreBearerUtil.UnlockSpecificEntryThenNext("story_trait_hijackheadquarters_complete", UI.USERMENUACTIONS.READLORE.SEARCH_OBJECT_SUCCESS.SEARCH6, new Action<InfoDialogScreen>(LoreBearerUtil.UnlockNextEmail), true));
			}
		}

		public void Intercept()
		{
			base.smi.sm.interceptCharges.Delta(1, base.smi);
			ImmigrantScreen.instance.ClearRejectedShuffleState();
			Immigration.Instance.EndImmigration();
			if (base.smi.sm.interceptCharges.Get(base.smi) >= 3)
			{
				base.smi.GoTo(base.smi.sm.operational.readyToPrint);
			}
			SelectTool.Instance.Select(null, true);
		}

		public void ActivatePrintInterface()
		{
			SelectTool.Instance.Select(null, true);
			PrinterceptorScreen.Instance.SetTarget(this);
			PrinterceptorScreen.Instance.Show(true);
		}

		public void UnlockPrinterceptor()
		{
			base.GetComponent<BuildingEnabledButton>().IsEnabled = true;
			base.smi.sm.passcodeUnlocked.Set(true, base.smi, false);
		}

		public void PrintSelectedEntity()
		{
			base.smi.sm.interceptCharges.Set(0, base.smi, false);
			GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(PrinterceptorScreen.Instance.selectedEntityTag), Grid.CellToPosCCC(Grid.PosToCell(base.gameObject), Grid.SceneLayer.Creatures) + Vector3.up * 1.5f, Quaternion.identity, null, null, true, 0);
			base.smi.master.GetComponent<Storage>().ConsumeIgnoringDisease(DatabankHelper.ID, (float)HijackedHeadquartersConfig.GetDataBankCost(PrinterceptorScreen.Instance.selectedEntityTag, base.smi.printCounts.ContainsKey(PrinterceptorScreen.Instance.selectedEntityTag) ? base.smi.printCounts[PrinterceptorScreen.Instance.selectedEntityTag] : 0));
			gameObject.SetActive(true);
			if (!base.smi.printCounts.ContainsKey(PrinterceptorScreen.Instance.selectedEntityTag))
			{
				base.smi.printCounts[PrinterceptorScreen.Instance.selectedEntityTag] = 0;
			}
			Dictionary<Tag, int> dictionary = base.smi.printCounts;
			Tag selectedEntityTag = PrinterceptorScreen.Instance.selectedEntityTag;
			int num = dictionary[selectedEntityTag];
			dictionary[selectedEntityTag] = num + 1;
		}

		public override void StartSM()
		{
			base.StartSM();
			this.UpdateStatusItems();
			this.UpdateMeter();
			StoryManager.Instance.ForceCreateStory(Db.Get().Stories.HijackedHeadquarters, base.gameObject.GetMyWorldId());
			this.onBuildingSelectHandle = base.Subscribe(-1503271301, new Action<object>(this.OnBuildingSelect));
			StoryManager.Instance.DiscoverStoryEvent(Db.Get().Stories.HijackedHeadquarters);
			if (StoryManager.Instance.IsStoryComplete(Db.Get().Stories.HijackedHeadquarters))
			{
				base.smi.AddLore();
			}
			this.m_storage.capacityKg = this.userMaxCapacity;
			this.ApplyMaxCapacity();
		}

		public override void StopSM(string reason)
		{
			base.Unsubscribe(ref this.onBuildingSelectHandle);
			base.StopSM(reason);
		}

		private void OnBuildingSelect(object obj)
		{
			if (!((Boxed<bool>)obj).value)
			{
				return;
			}
			if (!this.m_introPopupSeen)
			{
				this.ShowIntroNotification();
			}
			if (this.m_endNotification != null)
			{
				this.m_endNotification.customClickCallback(this.m_endNotification.customClickData);
			}
		}

		private void UpdateStatusItems()
		{
			base.gameObject.GetComponent<KSelectable>();
		}

		public void UpdateMeter()
		{
			float num = (float)base.smi.sm.interceptCharges.Get(base.smi) / 3f;
			this.m_progressMeter.SetPositionPercent(Mathf.Clamp01(num));
		}

		public void ShowIntroNotification()
		{
			this.m_introPopupSeen = true;
			EventInfoScreen.ShowPopup(EventInfoDataHelper.GenerateStoryTraitData(CODEX.STORY_TRAITS.HIJACK_HEADQUARTERS.BEGIN_POPUP.NAME, CODEX.STORY_TRAITS.HIJACK_HEADQUARTERS.BEGIN_POPUP.DESCRIPTION, CODEX.STORY_TRAITS.CLOSE_BUTTON, "printerceptordiscovered_kanim", EventInfoDataHelper.PopupType.BEGIN, null, null, null));
		}

		public void ShowCompletedNotification()
		{
			this.eventInfo = EventInfoDataHelper.GenerateStoryTraitData(CODEX.STORY_TRAITS.HIJACK_HEADQUARTERS.END_POPUP.NAME, CODEX.STORY_TRAITS.HIJACK_HEADQUARTERS.END_POPUP.DESCRIPTION, CODEX.STORY_TRAITS.HIJACK_HEADQUARTERS.END_POPUP.BUTTON, "printerceptorprintready_kanim", EventInfoDataHelper.PopupType.COMPLETE, null, null, null);
			this.m_endNotification = EventInfoScreen.CreateNotification(this.eventInfo, new Notification.ClickCallback(this.CompleteStory));
			base.gameObject.AddOrGet<Notifier>().Add(this.m_endNotification, "");
			base.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.AttentionRequired, base.smi);
		}

		public void ClearEndNotification()
		{
			base.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.AttentionRequired, false);
			if (this.m_endNotification != null)
			{
				base.gameObject.AddOrGet<Notifier>().Remove(this.m_endNotification);
			}
			this.m_endNotification = null;
		}

		public void CompleteStory(object _)
		{
			if (this.m_endNotification != null)
			{
				base.gameObject.AddOrGet<Notifier>().Remove(this.m_endNotification);
			}
			this.UpdateStatusItems();
			this.ClearEndNotification();
			Vector3 vector = Grid.CellToPosCCC(Grid.OffsetCell(Grid.PosToCell(base.smi), new CellOffset(0, 2)), Grid.SceneLayer.Ore);
			StoryManager.Instance.CompleteStoryEvent(Db.Get().Stories.HijackedHeadquarters, base.gameObject.GetComponent<MonoBehaviour>(), new FocusTargetSequence.Data
			{
				WorldId = base.smi.GetMyWorldId(),
				OrthographicSize = 6f,
				TargetSize = 6f,
				Target = vector,
				PopupData = this.eventInfo,
				CompleteCB = new global::System.Action(this.OnStorySequenceComplete),
				CanCompleteCB = null
			});
			this.AddLore();
		}

		private void OnStorySequenceComplete()
		{
			Vector3 vector = Grid.CellToPosCCC(Grid.OffsetCell(Grid.PosToCell(base.smi), new CellOffset(-1, 1)), Grid.SceneLayer.Ore);
			StoryManager.Instance.CompleteStoryEvent(Db.Get().Stories.HijackedHeadquarters, vector);
			this.eventInfo = null;
		}

		protected override void OnCleanUp()
		{
			if (this.m_endNotification != null)
			{
				base.gameObject.AddOrGet<Notifier>().Remove(this.m_endNotification);
			}
		}

		[MyCmpGet]
		private Storage m_storage;

		[Serialize]
		private bool m_introPopupSeen;

		private EventInfoData eventInfo;

		private Notification m_endNotification;

		private MeterController m_progressMeter;

		[Serialize]
		public Dictionary<Tag, int> printCounts = new Dictionary<Tag, int>();

		public static GameObject PrinterceptorInstance;

		private int onBuildingSelectHandle = -1;

		[Serialize]
		public float userMaxCapacity = 500f;
	}
}
