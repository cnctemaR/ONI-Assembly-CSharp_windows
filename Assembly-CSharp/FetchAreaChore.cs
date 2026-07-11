using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchAreaChore : Chore<FetchAreaChore.StatesInstance>
{
	public FetchAreaChore(Chore.Precondition.Context context)
	{
		ChoreType choreType = context.chore.choreType;
		ChoreConsumer consumer = context.consumerState.consumer;
		ChoreProvider choreProvider = context.consumerState.choreProvider;
		bool flag = false;
		PriorityScreen.PriorityClass priority_class = context.masterPriority.priority_class;
		int priority_value = context.masterPriority.priority_value;
		base..ctor(choreType, consumer, choreProvider, flag, null, null, null, priority_class, priority_value, false, true, 0, null, false, ReportManager.ReportType.WorkTime);
		this.showAvailabilityInHoverText = false;
		base.smi = new FetchAreaChore.StatesInstance(this, context);
	}

	public bool IsFetching
	{
		get
		{
			return base.smi.pickingup;
		}
	}

	public bool IsDelivering
	{
		get
		{
			return base.smi.delivering;
		}
	}

	public GameObject GetFetchTarget
	{
		get
		{
			return base.smi.sm.fetchTarget.Get(base.smi);
		}
	}

	public override void Cleanup()
	{
		base.Cleanup();
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		base.smi.Begin(context);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		base.smi.End();
		base.End(reason);
	}

	private void OnTagsChanged(object data)
	{
		if (base.smi.sm.fetchTarget.Get(base.smi) != null)
		{
			this.Fail("Tags changed");
		}
	}

	public static void GatherNearbyFetchChores(FetchChore root_chore, Chore.Precondition.Context context, int x, int y, int radius, List<Chore.Precondition.Context> succeeded_contexts, List<Chore.Precondition.Context> failed_contexts)
	{
		ListPool<ScenePartitionerEntry, FetchAreaChore>.PooledList pooledList = ListPool<ScenePartitionerEntry, FetchAreaChore>.Allocate();
		GameScenePartitioner.Instance.GatherEntries(x - radius, y - radius, radius * 2 + 1, radius * 2 + 1, GameScenePartitioner.Instance.fetchChoreLayer, pooledList);
		for (int i = 0; i < pooledList.Count; i++)
		{
			ScenePartitionerEntry scenePartitionerEntry = pooledList[i];
			FetchChore fetchChore = scenePartitionerEntry.obj as FetchChore;
			fetchChore.CollectChoresFromGlobalChoreProvider(context.consumerState, succeeded_contexts, failed_contexts, true);
		}
		pooledList.Recycle();
	}

	public class StatesInstance : GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.GameInstance
	{
		public StatesInstance(FetchAreaChore master, Chore.Precondition.Context context)
			: base(master)
		{
			this.rootContext = context;
			this.rootChore = context.chore as FetchChore;
		}

		public void Begin(Chore.Precondition.Context context)
		{
			base.sm.fetcher.Set(context.consumerState.gameObject, base.smi);
			this.chores.Clear();
			this.chores.Add(this.rootChore);
			int num;
			int num2;
			Grid.CellToXY(Grid.PosToCell(this.rootChore.destination.transform.GetPosition()), out num, out num2);
			ListPool<Chore.Precondition.Context, FetchAreaChore>.PooledList pooledList = ListPool<Chore.Precondition.Context, FetchAreaChore>.Allocate();
			ListPool<Chore.Precondition.Context, FetchAreaChore>.PooledList pooledList2 = ListPool<Chore.Precondition.Context, FetchAreaChore>.Allocate();
			if (this.rootChore.allowMultifetch)
			{
				FetchAreaChore.GatherNearbyFetchChores(this.rootChore, context, num, num2, 3, pooledList, pooledList2);
			}
			float num3 = Mathf.Max(1f, Db.Get().Attributes.CarryAmount.Lookup(context.consumerState.consumer).GetTotalValue());
			Pickupable pickupable = context.data as Pickupable;
			if (pickupable == null)
			{
				global::Debug.Assert(pooledList.Count > 0, "succeeded_contexts was empty");
				FetchChore fetchChore = (FetchChore)pooledList[0].chore;
				global::Debug.Assert(fetchChore != null, "fetch_chore was null");
				DebugUtil.LogWarningArgs(new object[]
				{
					"Missing root_fetchable for FetchAreaChore",
					fetchChore.destination,
					fetchChore.tags[0]
				});
				pickupable = fetchChore.FindFetchTarget(context.consumerState);
			}
			global::Debug.Assert(pickupable != null, "root_fetchable was null");
			List<Pickupable> list = new List<Pickupable>();
			list.Add(pickupable);
			float num4 = pickupable.UnreservedAmount;
			float minTakeAmount = pickupable.MinTakeAmount;
			int num5 = 0;
			int num6 = 0;
			Grid.CellToXY(Grid.PosToCell(pickupable.transform.GetPosition()), out num5, out num6);
			int num7 = 6;
			num5 -= num7 / 2;
			num6 -= num7 / 2;
			ListPool<ScenePartitionerEntry, FetchAreaChore>.PooledList pooledList3 = ListPool<ScenePartitionerEntry, FetchAreaChore>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(num5, num6, num7, num7, GameScenePartitioner.Instance.pickupablesLayer, pooledList3);
			Tag prefabTag = pickupable.GetComponent<KPrefabID>().PrefabTag;
			for (int i = 0; i < pooledList3.Count; i++)
			{
				ScenePartitionerEntry scenePartitionerEntry = pooledList3[i];
				if (num4 > num3)
				{
					break;
				}
				Pickupable pickupable2 = scenePartitionerEntry.obj as Pickupable;
				KPrefabID component = pickupable2.GetComponent<KPrefabID>();
				if (!(component.PrefabTag != prefabTag))
				{
					if (pickupable2.UnreservedAmount > 0f)
					{
						component.UpdateTagBits();
						if (component.HasAllTags_AssumeLaundered(ref this.rootChore.requiredTagBits))
						{
							if (!component.HasAnyTags_AssumeLaundered(ref this.rootChore.forbiddenTagBits))
							{
								if (!list.Contains(pickupable2))
								{
									if (this.rootContext.consumerState.consumer.CanReach(pickupable2))
									{
										float unreservedAmount = pickupable2.UnreservedAmount;
										list.Add(pickupable2);
										num4 += unreservedAmount;
										if (list.Count >= 10)
										{
											break;
										}
									}
								}
							}
						}
					}
				}
			}
			pooledList3.Recycle();
			num4 = Mathf.Min(num3, num4);
			if (minTakeAmount > 0f)
			{
				num4 -= num4 % minTakeAmount;
			}
			this.deliveries.Clear();
			float num8 = Mathf.Min(this.rootChore.originalAmount, num4);
			if (minTakeAmount > 0f)
			{
				num8 -= num8 % minTakeAmount;
			}
			this.deliveries.Add(new FetchAreaChore.StatesInstance.Delivery(this.rootContext, num8, new Action<FetchChore>(this.OnFetchChoreCancelled)));
			float num9 = num8;
			for (int j = 0; j < pooledList.Count; j++)
			{
				if (num9 >= num4)
				{
					break;
				}
				Chore.Precondition.Context context2 = pooledList[j];
				FetchChore fetchChore2 = context2.chore as FetchChore;
				if (fetchChore2 != this.rootChore && context2.IsSuccess() && fetchChore2.overrideTarget == null && fetchChore2.driver == null && fetchChore2.tagBits.AreEqual(ref this.rootChore.tagBits))
				{
					num8 = Mathf.Min(fetchChore2.originalAmount, num4 - num9);
					if (minTakeAmount > 0f)
					{
						num8 -= num8 % minTakeAmount;
					}
					this.chores.Add(fetchChore2);
					this.deliveries.Add(new FetchAreaChore.StatesInstance.Delivery(context2, num8, new Action<FetchChore>(this.OnFetchChoreCancelled)));
					num9 += num8;
					if (this.deliveries.Count >= 10)
					{
						break;
					}
				}
			}
			num9 = Mathf.Min(num9, num4);
			float num10 = num9;
			this.fetchables.Clear();
			for (int k = 0; k < list.Count; k++)
			{
				if (num10 <= 0f)
				{
					break;
				}
				Pickupable pickupable3 = list[k];
				num10 -= pickupable3.UnreservedAmount;
				this.fetchables.Add(pickupable3);
			}
			this.fetchAmountRequested = num9;
			this.reservations.Clear();
			pooledList.Recycle();
			pooledList2.Recycle();
		}

		public void End()
		{
			foreach (FetchAreaChore.StatesInstance.Delivery delivery in this.deliveries)
			{
				delivery.Cleanup();
			}
			this.deliveries.Clear();
		}

		public void SetupDelivery()
		{
			this.deliverables.RemoveAll((Pickupable x) => x == null || x.TotalAmount <= 0f);
			if (this.deliveries.Count > 0 && this.deliverables.Count > 0)
			{
				base.sm.deliveryDestination.Set(this.deliveries[0].destination, base.smi);
				base.sm.deliveryObject.Set(this.deliverables[0], base.smi);
				if (this.deliveries[0].destination != null)
				{
					if (this.rootContext.consumerState.hasSolidTransferArm)
					{
						if (this.rootContext.consumerState.consumer.IsWithinReach(this.deliveries[0].destination))
						{
							this.GoTo(base.sm.delivering.storing);
						}
						else
						{
							this.GoTo(base.sm.delivering.deliverfail);
						}
					}
					else
					{
						this.GoTo(base.sm.delivering.movetostorage);
					}
				}
				else
				{
					base.smi.GoTo(base.sm.delivering.deliverfail);
				}
			}
			else
			{
				this.StopSM("FetchAreaChoreComplete");
			}
		}

		public void SetupFetch()
		{
			if (this.reservations.Count > 0)
			{
				base.sm.fetchTarget.Set(this.reservations[0].pickupable, base.smi);
				base.sm.fetchResultTarget.Set(null, base.smi);
				base.sm.fetchAmount.Set(this.reservations[0].amount, base.smi);
				if (this.reservations[0].pickupable != null)
				{
					if (this.rootContext.consumerState.hasSolidTransferArm)
					{
						if (this.rootContext.consumerState.consumer.IsWithinReach(this.reservations[0].pickupable))
						{
							this.GoTo(base.sm.fetching.pickup);
						}
						else
						{
							this.GoTo(base.sm.fetching.fetchfail);
						}
					}
					else
					{
						this.GoTo(base.sm.fetching.movetopickupable);
					}
				}
				else
				{
					this.GoTo(base.sm.fetching.fetchfail);
				}
			}
			else
			{
				this.GoTo(base.sm.delivering.next);
			}
		}

		public void DeliverFail()
		{
			if (this.deliveries.Count > 0)
			{
				this.deliveries[0].Cleanup();
				this.deliveries.RemoveAt(0);
			}
			this.GoTo(base.sm.delivering.next);
		}

		public void DeliverComplete()
		{
			Pickupable pickupable = base.sm.deliveryObject.Get<Pickupable>(base.smi);
			if (pickupable == null || pickupable.TotalAmount <= 0f)
			{
				if (this.deliveries.Count > 0 && this.deliveries[0].chore.amount < PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
				{
					FetchAreaChore.StatesInstance.Delivery delivery = this.deliveries[0];
					Chore chore = delivery.chore;
					delivery.Complete(this.deliverables);
					delivery.Cleanup();
					if (this.deliveries.Count > 0 && this.deliveries[0].chore == chore)
					{
						this.deliveries.RemoveAt(0);
					}
					this.GoTo(base.sm.delivering.next);
				}
				else
				{
					base.smi.GoTo(base.sm.delivering.deliverfail);
				}
			}
			else
			{
				if (this.deliveries.Count > 0)
				{
					FetchAreaChore.StatesInstance.Delivery delivery2 = this.deliveries[0];
					Chore chore2 = delivery2.chore;
					delivery2.Complete(this.deliverables);
					delivery2.Cleanup();
					if (this.deliveries.Count > 0 && this.deliveries[0].chore == chore2)
					{
						this.deliveries.RemoveAt(0);
					}
				}
				this.GoTo(base.sm.delivering.next);
			}
		}

		public void FetchFail()
		{
			this.reservations[0].Cleanup();
			this.reservations.RemoveAt(0);
			this.GoTo(base.sm.fetching.next);
		}

		public void FetchComplete()
		{
			this.reservations[0].Cleanup();
			this.reservations.RemoveAt(0);
			this.GoTo(base.sm.fetching.next);
		}

		public void SetupDeliverables()
		{
			foreach (GameObject gameObject in base.sm.fetcher.Get<Storage>(base.smi).items)
			{
				if (!(gameObject == null))
				{
					KPrefabID component = gameObject.GetComponent<KPrefabID>();
					if (!(component == null))
					{
						Pickupable component2 = component.GetComponent<Pickupable>();
						if (component2 != null)
						{
							this.deliverables.Add(component2);
						}
					}
				}
			}
		}

		public void ReservePickupables()
		{
			ChoreConsumer choreConsumer = base.sm.fetcher.Get<ChoreConsumer>(base.smi);
			float num = this.fetchAmountRequested;
			foreach (Pickupable pickupable in this.fetchables)
			{
				if (num <= 0f)
				{
					break;
				}
				float num2 = Math.Min(num, pickupable.UnreservedAmount);
				num -= num2;
				FetchAreaChore.StatesInstance.Reservation reservation = new FetchAreaChore.StatesInstance.Reservation(choreConsumer, pickupable, num2);
				this.reservations.Add(reservation);
			}
		}

		private void OnFetchChoreCancelled(FetchChore chore)
		{
			int i = 0;
			while (i < this.deliveries.Count)
			{
				if (this.deliveries[i].chore == chore)
				{
					if (this.deliveries.Count == 1)
					{
						this.StopSM("AllDelivericesCancelled");
						break;
					}
					if (i == 0)
					{
						base.sm.currentdeliverycancelled.Trigger(this);
						break;
					}
					this.deliveries[i].Cleanup();
					this.deliveries.RemoveAt(i);
					break;
				}
				else
				{
					i++;
				}
			}
		}

		public void UnreservePickupables()
		{
			foreach (FetchAreaChore.StatesInstance.Reservation reservation in this.reservations)
			{
				reservation.Cleanup();
			}
			this.reservations.Clear();
		}

		public bool SameDestination(FetchChore fetch)
		{
			foreach (FetchChore fetchChore in this.chores)
			{
				if (fetchChore.destination == fetch.destination)
				{
					return true;
				}
			}
			return false;
		}

		private List<FetchChore> chores = new List<FetchChore>();

		private List<Pickupable> fetchables = new List<Pickupable>();

		private List<FetchAreaChore.StatesInstance.Reservation> reservations = new List<FetchAreaChore.StatesInstance.Reservation>();

		private List<Pickupable> deliverables = new List<Pickupable>();

		public List<FetchAreaChore.StatesInstance.Delivery> deliveries = new List<FetchAreaChore.StatesInstance.Delivery>();

		private FetchChore rootChore;

		private Chore.Precondition.Context rootContext;

		private float fetchAmountRequested;

		public bool delivering;

		public bool pickingup;

		public struct Delivery
		{
			public Delivery(Chore.Precondition.Context context, float amount_to_be_fetched, Action<FetchChore> on_cancelled)
			{
				this = default(FetchAreaChore.StatesInstance.Delivery);
				this.chore = context.chore as FetchChore;
				this.amount = this.chore.originalAmount;
				this.destination = this.chore.destination;
				this.chore.SetOverrideTarget(context.consumerState.consumer);
				this.onCancelled = on_cancelled;
				this.onFetchChoreCleanup = new Action<Chore>(this.OnFetchChoreCleanup);
				this.chore.FetchAreaBegin(context, amount_to_be_fetched);
				FetchChore chore = this.chore;
				chore.onCleanup = (Action<Chore>)Delegate.Combine(chore.onCleanup, this.onFetchChoreCleanup);
			}

			public Storage destination { get; private set; }

			public float amount { get; private set; }

			public FetchChore chore { get; private set; }

			public void Complete(List<Pickupable> deliverables)
			{
				using (new KProfiler.Region("FAC.Delivery.Complete", null))
				{
					if (!(this.destination == null) && !this.destination.IsEndOfLife())
					{
						FetchChore chore = this.chore;
						chore.onCleanup = (Action<Chore>)Delegate.Remove(chore.onCleanup, this.onFetchChoreCleanup);
						float num = this.amount;
						Pickupable pickupable = null;
						for (int i = 0; i < deliverables.Count; i++)
						{
							if (num <= 0f)
							{
								break;
							}
							if (deliverables[i] == null)
							{
								if (num < PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
								{
									this.destination.ForceStore(this.chore.tags[0], num);
								}
							}
							else
							{
								Pickupable pickupable2 = deliverables[i].Take(num);
								if (pickupable2 != null && pickupable2.TotalAmount > 0f)
								{
									num -= pickupable2.TotalAmount;
									this.destination.Store(pickupable2.gameObject, false, false, true, false);
									pickupable = pickupable2;
									if (pickupable2 == deliverables[i])
									{
										deliverables[i] = null;
									}
								}
							}
						}
						if (pickupable != null && this.chore.overrideTarget != null)
						{
							this.chore.FetchAreaEnd(this.chore.overrideTarget.GetComponent<ChoreDriver>(), pickupable, true);
						}
						this.chore = null;
					}
				}
			}

			private void OnFetchChoreCleanup(Chore chore)
			{
				if (this.onCancelled != null)
				{
					this.onCancelled(chore as FetchChore);
				}
			}

			public void Cleanup()
			{
				if (this.chore != null)
				{
					FetchChore chore = this.chore;
					chore.onCleanup = (Action<Chore>)Delegate.Remove(chore.onCleanup, this.onFetchChoreCleanup);
					this.chore.FetchAreaEnd(null, null, false);
				}
			}

			private Action<FetchChore> onCancelled;

			private Action<Chore> onFetchChoreCleanup;
		}

		public struct Reservation
		{
			public Reservation(ChoreConsumer consumer, Pickupable pickupable, float reservation_amount)
			{
				this = default(FetchAreaChore.StatesInstance.Reservation);
				if (reservation_amount <= 0f)
				{
					global::Debug.LogError("Invalid amount: " + reservation_amount);
				}
				this.amount = reservation_amount;
				this.pickupable = pickupable;
				this.handle = pickupable.Reserve("FetchAreaChore", consumer.gameObject, reservation_amount);
			}

			public float amount { get; private set; }

			public Pickupable pickupable { get; private set; }

			public void Cleanup()
			{
				if (this.pickupable != null)
				{
					this.pickupable.Unreserve("FetchAreaChore", this.handle);
				}
			}

			private int handle;
		}
	}

	public class States : GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.fetching;
			base.Target(this.fetcher);
			this.fetching.DefaultState(this.fetching.next).Enter("ReservePickupables", delegate(FetchAreaChore.StatesInstance smi)
			{
				smi.ReservePickupables();
			}).Exit("UnreservePickupables", delegate(FetchAreaChore.StatesInstance smi)
			{
				smi.UnreservePickupables();
			})
				.Enter("pickingup-on", delegate(FetchAreaChore.StatesInstance smi)
				{
					smi.pickingup = true;
				})
				.Exit("pickingup-off", delegate(FetchAreaChore.StatesInstance smi)
				{
					smi.pickingup = false;
				});
			this.fetching.next.Enter("SetupFetch", delegate(FetchAreaChore.StatesInstance smi)
			{
				smi.SetupFetch();
			});
			GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.ApproachSubState<Pickupable> movetopickupable = this.fetching.movetopickupable;
			StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter targetParameter = this.fetcher;
			StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter targetParameter2 = this.fetchTarget;
			GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State state = this.fetching.pickup;
			GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State state2 = this.fetching.fetchfail;
			NavTactic navTactic = NavigationTactics.ReduceTravelDistance;
			movetopickupable.InitializeStates(targetParameter, targetParameter2, state, state2, null, navTactic);
			this.fetching.pickup.DoPickup(this.fetchTarget, this.fetchResultTarget, this.fetchAmount, this.fetching.fetchcomplete, this.fetching.fetchfail);
			this.fetching.fetchcomplete.Enter(delegate(FetchAreaChore.StatesInstance smi)
			{
				smi.FetchComplete();
			});
			this.fetching.fetchfail.Enter(delegate(FetchAreaChore.StatesInstance smi)
			{
				smi.FetchFail();
			});
			this.delivering.DefaultState(this.delivering.next).OnSignal(this.currentdeliverycancelled, this.delivering.deliverfail).Enter("SetupDeliverables", delegate(FetchAreaChore.StatesInstance smi)
			{
				smi.SetupDeliverables();
			})
				.Enter("delivering-on", delegate(FetchAreaChore.StatesInstance smi)
				{
					smi.delivering = true;
				})
				.Exit("delivering-off", delegate(FetchAreaChore.StatesInstance smi)
				{
					smi.delivering = false;
				});
			this.delivering.next.Enter("SetupDelivery", delegate(FetchAreaChore.StatesInstance smi)
			{
				smi.SetupDelivery();
			});
			GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.ApproachSubState<Storage> movetostorage = this.delivering.movetostorage;
			targetParameter2 = this.fetcher;
			targetParameter = this.deliveryDestination;
			state2 = this.delivering.storing;
			state = this.delivering.deliverfail;
			navTactic = NavigationTactics.ReduceTravelDistance;
			movetostorage.InitializeStates(targetParameter2, targetParameter, state2, state, null, navTactic).Enter(delegate(FetchAreaChore.StatesInstance smi)
			{
				if (this.deliveryObject.Get(smi) != null && this.deliveryObject.Get(smi).GetComponent<MinionIdentity>() != null)
				{
					this.deliveryObject.Get(smi).transform.SetLocalPosition(Vector3.zero);
					KBatchedAnimTracker component = this.deliveryObject.Get(smi).GetComponent<KBatchedAnimTracker>();
					component.symbol = new HashedString("snapTo_chest");
					component.offset = new Vector3(0f, 0f, 1f);
				}
			});
			this.delivering.storing.DoDelivery(this.fetcher, this.deliveryDestination, this.delivering.delivercomplete, this.delivering.deliverfail);
			this.delivering.deliverfail.Enter(delegate(FetchAreaChore.StatesInstance smi)
			{
				smi.DeliverFail();
			});
			this.delivering.delivercomplete.Enter(delegate(FetchAreaChore.StatesInstance smi)
			{
				smi.DeliverComplete();
			});
		}

		public FetchAreaChore.States.FetchStates fetching;

		public FetchAreaChore.States.DeliverStates delivering;

		public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter fetcher;

		public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter fetchTarget;

		public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter fetchResultTarget;

		public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.FloatParameter fetchAmount;

		public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter deliveryDestination;

		public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter deliveryObject;

		public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.FloatParameter deliveryAmount;

		public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.Signal currentdeliverycancelled;

		public class FetchStates : GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State
		{
			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State next;

			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.ApproachSubState<Pickupable> movetopickupable;

			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State pickup;

			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State fetchfail;

			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State fetchcomplete;
		}

		public class DeliverStates : GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State
		{
			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State next;

			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.ApproachSubState<Storage> movetostorage;

			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State storing;

			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State deliverfail;

			public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State delivercomplete;
		}
	}
}
