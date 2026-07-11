using System;
using KSerialization;
using ProcGen;
using UnityEngine;

public class DiggerMonitor : GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.loop;
		this.loop.EventTransition(GameHashes.BeginMeteorBombardment, (DiggerMonitor.Instance smi) => Game.Instance, this.dig, (DiggerMonitor.Instance smi) => smi.CanTunnel());
		this.dig.ToggleBehaviour(GameTags.Creatures.Tunnel, (DiggerMonitor.Instance smi) => true, null).GoTo(this.loop);
	}

	public GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>.State loop;

	public GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>.State dig;

	public class Def : StateMachine.BaseDef
	{
		public int depthToDig { get; set; }
	}

	public new class Instance : GameStateMachine<DiggerMonitor, DiggerMonitor.Instance, IStateMachineTarget, DiggerMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, DiggerMonitor.Def def)
			: base(master, def)
		{
			global::World instance = global::World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Combine(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
			this.OnDestinationReachedDelegate = new Action<object>(this.OnDestinationReached);
			master.Subscribe(387220196, this.OnDestinationReachedDelegate);
			master.Subscribe(-766531887, this.OnDestinationReachedDelegate);
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			global::World instance = global::World.Instance;
			instance.OnSolidChanged = (Action<int>)Delegate.Remove(instance.OnSolidChanged, new Action<int>(this.OnSolidChanged));
			base.master.Unsubscribe(387220196, this.OnDestinationReachedDelegate);
			base.master.Unsubscribe(-766531887, this.OnDestinationReachedDelegate);
		}

		private void OnDestinationReached(object data)
		{
			this.CheckInSolid();
		}

		private void CheckInSolid()
		{
			Navigator component = base.gameObject.GetComponent<Navigator>();
			if (component == null)
			{
				return;
			}
			int num = Grid.PosToCell(base.gameObject);
			if (component.CurrentNavType != NavType.Solid && Grid.IsSolidCell(num))
			{
				component.SetCurrentNavType(NavType.Solid);
				return;
			}
			if (component.CurrentNavType == NavType.Solid && !Grid.IsSolidCell(num))
			{
				component.SetCurrentNavType(NavType.Floor);
				base.gameObject.AddTag(GameTags.Creatures.Falling);
			}
		}

		private void OnSolidChanged(int cell)
		{
			this.CheckInSolid();
		}

		public bool CanTunnel()
		{
			int num = Grid.PosToCell(this);
			if (global::World.Instance.zoneRenderData.GetSubWorldZoneType(num) == SubWorld.ZoneType.Space)
			{
				int num2 = num;
				while (Grid.IsValidCell(num2) && !Grid.Solid[num2])
				{
					num2 = Grid.CellAbove(num2);
				}
				if (!Grid.IsValidCell(num2))
				{
					return this.FoundValidDigCell();
				}
			}
			return false;
		}

		private bool FoundValidDigCell()
		{
			int num = base.smi.def.depthToDig;
			int num2 = Grid.PosToCell(base.smi.master.gameObject);
			this.lastDigCell = num2;
			int num3 = Grid.CellBelow(num2);
			while (this.IsValidDigCell(num3, null) && num > 0)
			{
				num3 = Grid.CellBelow(num3);
				num--;
			}
			if (num > 0)
			{
				num3 = GameUtil.FloodFillFind<object>(new Func<int, object, bool>(this.IsValidDigCell), null, num2, base.smi.def.depthToDig, false, true);
			}
			this.lastDigCell = num3;
			return this.lastDigCell != -1;
		}

		private bool IsValidDigCell(int cell, object arg = null)
		{
			if (Grid.IsValidCell(cell) && Grid.Solid[cell])
			{
				if (!Grid.HasDoor[cell] && !Grid.Foundation[cell])
				{
					byte b = Grid.ElementIdx[cell];
					Element element = ElementLoader.elements[(int)b];
					return Grid.Element[cell].hardness < 150 && !element.HasTag(GameTags.RefinedMetal);
				}
				GameObject gameObject = Grid.Objects[cell, 1];
				if (gameObject != null)
				{
					PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
					return Grid.Element[cell].hardness < 150 && !component.Element.HasTag(GameTags.RefinedMetal);
				}
			}
			return false;
		}

		[Serialize]
		public int lastDigCell = -1;

		private Action<object> OnDestinationReachedDelegate;
	}
}
