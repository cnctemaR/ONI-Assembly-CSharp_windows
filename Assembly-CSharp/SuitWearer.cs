using System;
using System.Collections.Generic;
using UnityEngine;

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
			master.GetComponent<KBatchedAnimController>().HideSymbol(new HashedString("snapto_neck"), true);
		}

		public void OnPathAdvanced(object data)
		{
			this.UnreserveSuits();
			this.ReserveSuits();
		}

		public void ReserveSuits()
		{
			PathFinder.Path path = base.GetComponent<Navigator>().path;
			if (path.nodes == null)
			{
				return;
			}
			for (int i = 0; i < path.nodes.Count; i++)
			{
				if (i != path.nodes.Count - 1)
				{
					int cell = path.nodes[i].cell;
					GameObject gameObject = Grid.Objects[cell, 1];
					if (!(gameObject == null))
					{
						SuitMarker component = gameObject.GetComponent<SuitMarker>();
						if (!(component == null))
						{
							bool flag = component.DoesTraversalDirectionRequireSuit(cell, path.nodes[i + 1].cell);
							SuitWearer.Instance.Reservation reservation = new SuitWearer.Instance.Reservation
							{
								suitMarker = component,
								isForEquipping = flag
							};
							component.Reserve(this, flag);
							this.reservations.Add(reservation);
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

		private struct Reservation
		{
			public SuitMarker suitMarker;

			public bool isForEquipping;
		}
	}
}
