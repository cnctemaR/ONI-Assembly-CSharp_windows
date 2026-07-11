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
			this.navigator = master.GetComponent<Navigator>();
			this.navigator.SetFlags(PathFinder.PotentialPath.Flags.PerformSuitChecks);
			KBatchedAnimController component = master.GetComponent<KBatchedAnimController>();
			component.SetSymbolVisiblity("snapto_neck", false);
		}

		public void OnPathAdvanced(object data)
		{
			if (this.navigator.CurrentNavType == NavType.Hover && (byte)(this.navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) == 0)
			{
				this.navigator.SetCurrentNavType(NavType.Floor);
			}
			this.UnreserveSuits();
			this.ReserveSuits();
		}

		public void ReserveSuits()
		{
			PathFinder.Path path = this.navigator.path;
			if (path.nodes == null)
			{
				return;
			}
			bool flag = (byte)(this.navigator.flags & PathFinder.PotentialPath.Flags.HasAtmoSuit) != 0;
			bool flag2 = (byte)(this.navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) != 0;
			for (int i = 0; i < path.nodes.Count - 1; i++)
			{
				int cell = path.nodes[i].cell;
				Pathfinding.INavigationFeature navigationFeature = Pathfinding.Instance.GetNavigationFeature(cell);
				if (navigationFeature != null)
				{
					SuitMarker suitMarker = navigationFeature as SuitMarker;
					if (!(suitMarker == null))
					{
						bool flag3 = (byte)(suitMarker.PathFlag & PathFinder.PotentialPath.Flags.HasAtmoSuit) != 0;
						bool flag4 = (byte)(suitMarker.PathFlag & PathFinder.PotentialPath.Flags.HasJetPack) != 0;
						bool flag5 = flag2 || flag;
						bool flag6 = flag3 == flag && flag4 == flag2;
						bool flag7 = suitMarker.DoesTraversalDirectionRequireSuit(cell, path.nodes[i + 1].cell);
						if (flag7 && !flag5)
						{
							SuitWearer.Instance.Reservation reservation = new SuitWearer.Instance.Reservation
							{
								suitMarker = suitMarker,
								isForEquipping = flag7
							};
							suitMarker.Reserve(this, flag7);
							this.reservations.Add(reservation);
							if (flag3)
							{
								flag = true;
							}
							if (flag4)
							{
								flag2 = true;
							}
						}
						else if (!flag7 && flag6 && suitMarker.IsUnequipAvailableForSuitWearer(this))
						{
							SuitWearer.Instance.Reservation reservation2 = new SuitWearer.Instance.Reservation
							{
								suitMarker = suitMarker,
								isForEquipping = flag7
							};
							suitMarker.Reserve(this, flag7);
							this.reservations.Add(reservation2);
							if (flag3)
							{
								flag = false;
							}
							if (flag4)
							{
								flag2 = false;
							}
						}
					}
				}
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

		private Navigator navigator;

		private struct Reservation
		{
			public SuitMarker suitMarker;

			public bool isForEquipping;
		}
	}
}
