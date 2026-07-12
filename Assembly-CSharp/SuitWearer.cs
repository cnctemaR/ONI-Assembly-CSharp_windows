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
			this.prefabInstanceID = this.navigator.GetComponent<KPrefabID>().InstanceID;
		}

		public void OnPathAdvanced(object data)
		{
			if (this.navigator.CurrentNavType == NavType.Hover && (this.navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) <= PathFinder.PotentialPath.Flags.None)
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
			bool flag = (this.navigator.flags & PathFinder.PotentialPath.Flags.HasAtmoSuit) > PathFinder.PotentialPath.Flags.None;
			bool flag2 = (this.navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) > PathFinder.PotentialPath.Flags.None;
			bool flag3 = (this.navigator.flags & PathFinder.PotentialPath.Flags.HasOxygenMask) > PathFinder.PotentialPath.Flags.None;
			bool flag4 = (this.navigator.flags & PathFinder.PotentialPath.Flags.HasLeadSuit) > PathFinder.PotentialPath.Flags.None;
			for (int i = 0; i < path.nodes.Count - 1; i++)
			{
				int cell = path.nodes[i].cell;
				Grid.SuitMarker.Flags flags = (Grid.SuitMarker.Flags)0;
				PathFinder.PotentialPath.Flags flags2 = PathFinder.PotentialPath.Flags.None;
				if (Grid.TryGetSuitMarkerFlags(cell, out flags, out flags2))
				{
					bool flag5 = (flags2 & PathFinder.PotentialPath.Flags.HasAtmoSuit) > PathFinder.PotentialPath.Flags.None;
					bool flag6 = (flags2 & PathFinder.PotentialPath.Flags.HasJetPack) > PathFinder.PotentialPath.Flags.None;
					bool flag7 = (flags2 & PathFinder.PotentialPath.Flags.HasOxygenMask) > PathFinder.PotentialPath.Flags.None;
					bool flag8 = (flags2 & PathFinder.PotentialPath.Flags.HasLeadSuit) > PathFinder.PotentialPath.Flags.None;
					bool flag9 = flag2 || flag || flag3 || flag4;
					bool flag10 = flag5 == flag && flag6 == flag2 && flag7 == flag3 && flag8 == flag4;
					bool flag11 = SuitMarker.DoesTraversalDirectionRequireSuit(cell, path.nodes[i + 1].cell, flags);
					if (flag11 && !flag9)
					{
						Grid.ReserveSuit(cell, this.prefabInstanceID, true);
						this.suitReservations.Add(cell);
						if (flag5)
						{
							flag = true;
						}
						if (flag6)
						{
							flag2 = true;
						}
						if (flag7)
						{
							flag3 = true;
						}
					}
					else if (!flag11 && flag10 && Grid.HasEmptyLocker(cell, this.prefabInstanceID))
					{
						Grid.ReserveEmptyLocker(cell, this.prefabInstanceID, true);
						this.emptyLockerReservations.Add(cell);
						if (flag5)
						{
							flag = false;
						}
						if (flag6)
						{
							flag2 = false;
						}
						if (flag7)
						{
							flag3 = false;
						}
					}
				}
			}
		}

		public void UnreserveSuits()
		{
			foreach (int num in this.suitReservations)
			{
				if (Grid.HasSuitMarker[num])
				{
					Grid.ReserveSuit(num, this.prefabInstanceID, false);
				}
			}
			this.suitReservations.Clear();
			foreach (int num2 in this.emptyLockerReservations)
			{
				if (Grid.HasSuitMarker[num2])
				{
					Grid.ReserveEmptyLocker(num2, this.prefabInstanceID, false);
				}
			}
			this.emptyLockerReservations.Clear();
		}

		private List<int> suitReservations = new List<int>();

		private List<int> emptyLockerReservations = new List<int>();

		private Navigator navigator;

		private int prefabInstanceID;
	}
}
