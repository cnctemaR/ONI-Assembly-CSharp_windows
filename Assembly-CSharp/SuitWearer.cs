using System;
using System.Collections.Generic;

public class SuitWearer : GameStateMachine<SuitWearer, SuitWearer.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.EventHandler(GameHashes.PathAdvanced, delegate(SuitWearer.Instance smi, object data)
		{
			smi.OnPathAdvanced(data);
		}).DoNothing();
		this.suit.DoNothing();
		this.nosuit.DoNothing();
	}

	public GameStateMachine<SuitWearer, SuitWearer.Instance, IStateMachineTarget, object>.State suit;

	public GameStateMachine<SuitWearer, SuitWearer.Instance, IStateMachineTarget, object>.State nosuit;

	public new class Instance : GameStateMachine<SuitWearer, SuitWearer.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			master.GetComponent<Navigator>().SetFlags(PathFinder.PotentialPath.Flags.PerformSuitChecks);
			master.GetComponent<KBatchedAnimController>().SetSymbolVisiblity("snapto_neck", false);
		}

		public void OnPathAdvanced(object data)
		{
			this.UnreserveSuits();
			this.ReserveSuits();
		}

		public void ReserveSuits()
		{
			Navigator component = base.GetComponent<Navigator>();
			PathFinder.Path path = component.path;
			if (path.nodes == null)
			{
				return;
			}
			bool flag = (byte)(component.flags & PathFinder.PotentialPath.Flags.HasSuit) != 0;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < path.nodes.Count - 1; i++)
			{
				int cell = path.nodes[i].cell;
				Pathfinding.INavigationFeature navigationFeature = Pathfinding.Instance.GetNavigationFeature(cell);
				if (navigationFeature != null)
				{
					SuitMarker suitMarker = navigationFeature as SuitMarker;
					if (!(suitMarker == null))
					{
						num++;
						bool flag2 = suitMarker.DoesTraversalDirectionRequireSuit(cell, path.nodes[i + 1].cell);
						if (flag2 && !flag)
						{
							SuitWearer.Instance.Reservation reservation = new SuitWearer.Instance.Reservation
							{
								suitMarker = suitMarker,
								isForEquipping = flag2
							};
							suitMarker.Reserve(this, flag2);
							this.reservations.Add(reservation);
							flag = true;
							num2++;
						}
						else if (!flag2 && flag && suitMarker.IsUnequipAvailableForSuitWearer(this))
						{
							SuitWearer.Instance.Reservation reservation2 = new SuitWearer.Instance.Reservation
							{
								suitMarker = suitMarker,
								isForEquipping = flag2
							};
							suitMarker.Reserve(this, flag2);
							this.reservations.Add(reservation2);
							flag = false;
							num2++;
						}
					}
				}
			}
			if (num > 1)
			{
			}
		}

		public void UnreserveSuits()
		{
			foreach (SuitWearer.Instance.Reservation reservation in this.reservations)
			{
				if (!(reservation.suitMarker == null))
				{
					reservation.suitMarker.Unreserve(this, reservation.isForEquipping);
				}
			}
			this.reservations.Clear();
		}

		private List<SuitWearer.Instance.Reservation> reservations = new List<SuitWearer.Instance.Reservation>();

		private struct Reservation
		{
			public SuitMarker suitMarker;

			public bool isForEquipping;
		}
	}
}
