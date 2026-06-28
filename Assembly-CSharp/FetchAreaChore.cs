using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class FetchAreaChore : Chore<FetchAreaChore.StatesInstance>
{
	public FetchAreaChore(Chore.Precondition.Context context)
		: base(context.chore.choreType, context.consumer, context.consumer.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.basic, int.MaxValue, false, true, 0)
	{
		this.smi = new FetchAreaChore.StatesInstance(this, context);
	}

	public override void Begin(Chore.Precondition.Context context)
	{
		this.smi.Begin(context);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		this.smi.End();
		base.End(reason);
	}

	public override void Cleanup()
	{
		base.Cleanup();
	}

	public override string GetReportName()
	{
		if (this.smi.deliveries.Count > 0 && this.smi.deliveries[0].destination != null)
		{
			string text = DUPLICANTS.CHORES.FETCH.REPORT_NAME;
			return text.Replace("{0}", this.smi.deliveries[0].destination.GetProperName());
		}
		return base.GetReportName();
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
			base.sm.fetcher.Set(context.consumer.gameObject, base.smi);
			this.chores.Clear();
			this.chores.Add(this.rootChore);
			int num = 0;
			int num2 = 0;
			Grid.CellToXY(Grid.PosToCell(this.rootChore.destination.transform.position), out num, out num2);
			int num3 = 6;
			num -= num3 / 2;
			num2 -= num3 / 2;
			List<ScenePartitionerEntry> list = GameScenePartitioner.Instance.ReserveList();
			List<Chore.Precondition.Context> list2 = new List<Chore.Precondition.Context>();
			if (this.rootChore.allowMultifetch)
			{
				GameScenePartitioner.Instance.GatherEntries(num, num2, num3, num3, GameScenePartitioner.Instance.fetchChoreLayer, list);
				for (int i = 0; i < list.Count; i++)
				{
					ScenePartitionerEntry scenePartitionerEntry = list[i];
					Chore chore = scenePartitionerEntry.obj as Chore;
					chore.CollectChores(context.consumer, list2, true);
				}
				GameScenePartitioner.Instance.ReleaseList(list);
			}
			float num4 = 600f;
			AttributeConverterInstance attributeConverterInstance = Db.Get().AttributeConverters.CarryAmount.Lookup(context.consumer);
			num4 += attributeConverterInstance.Evaluate();
			Pickupable pickupable = context.data as Pickupable;
			List<Pickupable> list3 = new List<Pickupable>();
			list3.Add(pickupable);
			float num5 = pickupable.UnreservedAmount;
			float minTakeAmount = pickupable.MinTakeAmount;
			int num6 = 0;
			int num7 = 0;
			Grid.CellToXY(Grid.PosToCell(pickupable.transform.position), out num6, out num7);
			int num8 = 6;
			num6 -= num8 / 2;
			num7 -= num8 / 2;
			List<ScenePartitionerEntry> list4 = GameScenePartitioner.Instance.ReserveList();
			GameScenePartitioner.Instance.GatherEntries(num6, num7, num8, num8, GameScenePartitioner.Instance.pickupablesLayer, list4);
			Tag prefabTag = pickupable.GetComponent<KPrefabID>().PrefabTag;
			for (int j = 0; j < list4.Count; j++)
			{
				ScenePartitionerEntry scenePartitionerEntry2 = list4[j];
				if (num5 > num4)
				{
					break;
				}
				Pickupable pickupable2 = scenePartitionerEntry2.obj as Pickupable;
				Tag prefabTag2 = pickupable2.GetComponent<KPrefabID>().PrefabTag;
				if (!list3.Contains(pickupable2) && prefabTag2 == prefabTag && pickupable2.UnreservedAmount > 0f)
				{
					float unreservedAmount = pickupable2.UnreservedAmount;
					list3.Add(pickupable2);
					num5 += unreservedAmount;
				}
			}
			GameScenePartitioner.Instance.ReleaseList(list4);
			num5 = Mathf.Min(num4, num5);
			if (minTakeAmount > 0f)
			{
				num5 -= num5 % minTakeAmount;
			}
			this.deliveries.Clear();
			float num9 = Mathf.Min(this.rootChore.originalAmount, num5);
			if (minTakeAmount > 0f)
			{
				num9 -= num9 % minTakeAmount;
			}
			this.deliveries.Add(new FetchAreaChore.StatesInstance.Delivery(this.rootContext, num9, new Action<FetchChore>(this.OnFetchChoreCancelled)));
			float num10 = num9;
			for (int k = 0; k < list2.Count; k++)
			{
				if (num10 >= num5)
				{
					break;
				}
				Chore.Precondition.Context context2 = list2[k];
				FetchChore fetchChore = context2.chore as FetchChore;
				if (fetchChore != this.rootChore && context2.IsSuccess() && fetchChore.overrideTarget == null && fetchChore.driver == null && fetchChore.tagBits.AreEqual(this.rootChore.tagBits))
				{
					num9 = Mathf.Min(fetchChore.originalAmount, num5 - num10);
					if (minTakeAmount > 0f)
					{
						num9 -= num9 % minTakeAmount;
					}
					this.chores.Add(fetchChore);
					this.deliveries.Add(new FetchAreaChore.StatesInstance.Delivery(context2, num9, new Action<FetchChore>(this.OnFetchChoreCancelled)));
					num10 += num9;
				}
			}
			num10 = Mathf.Min(num10, num5);
			float num11 = num10;
			this.fetchables.Clear();
			for (int l = 0; l < list3.Count; l++)
			{
				if (num11 <= 0f)
				{
					break;
				}
				Pickupable pickupable3 = list3[l];
				num11 -= pickupable3.UnreservedAmount;
				this.fetchables.Add(pickupable3);
			}
			this.fetchAmountRequested = num10;
			this.reservations.Clear();
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
				this.GoTo(base.sm.delivering.movetostorage);
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
				this.GoTo(base.sm.fetching.movetopickupable);
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
				global::Debug.LogWarning("How did the thing that I am holding disappear?", null);
				base.smi.GoTo(base.sm.delivering.deliverfail);
			}
			else
			{
				if (this.deliveries.Count > 0)
				{
					FetchAreaChore.StatesInstance.Delivery delivery = this.deliveries[0];
					Chore chore = delivery.chore;
					delivery.Complete(this.deliverables);
					delivery.Cleanup();
					if (this.deliveries.Count > 0 && this.deliveries[0].chore == chore)
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
			foreach (GameObject gameObject in base.sm.fetcher.Get<Storage>(base.smi))
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

		private List<FetchChore> chores = new List<FetchChore>();

		private List<Pickupable> fetchables = new List<Pickupable>();

		private List<FetchAreaChore.StatesInstance.Reservation> reservations = new List<FetchAreaChore.StatesInstance.Reservation>();

		private List<Pickupable> deliverables = new List<Pickupable>();

		public List<FetchAreaChore.StatesInstance.Delivery> deliveries = new List<FetchAreaChore.StatesInstance.Delivery>();

		private FetchChore rootChore;

		private Chore.Precondition.Context rootContext;

		private float fetchAmountRequested;

		public struct Delivery
		{
			public Delivery(Chore.Precondition.Context context, float amount_to_be_fetched, Action<FetchChore> on_cancelled)
			{
				this = default(FetchAreaChore.StatesInstance.Delivery);
				this.chore = context.chore as FetchChore;
				this.amount = this.chore.originalAmount;
				this.destination = this.chore.destination;
				this.chore.SetOverrideTarget(context.consumer);
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
							if (!(deliverables[i] == null))
							{
								Pickupable pickupable2 = deliverables[i].Take(num);
								if (pickupable2 != null && pickupable2.TotalAmount > 0f)
								{
									num -= pickupable2.TotalAmount;
									this.destination.Store(pickupable2.gameObject, false, false, true);
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
					global::Debug.LogError("Invalid amount: " + reservation_amount, null);
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
					component.symbol = new HashedString("snapTo_pivot");
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
