using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class Vent : KMonoBehaviour, IEffectDescriptor
{
	public int SortKey
	{
		get
		{
			return this.sortKey;
		}
		set
		{
			this.sortKey = value;
		}
	}

	protected override void OnSpawn()
	{
		Building component = base.GetComponent<Building>();
		this.cell = component.GetUtilityOutputCell();
		this.smi = new Vent.StatesInstance(this);
		this.smi.StartSM();
		if (this.connected)
		{
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
			networkManager.AddToNetworks(this.cell, this, true);
		}
	}

	protected override void OnCleanUp()
	{
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[this.cell, (int)component.Def.ReplacementLayer] == null)
		{
			IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
			networkManager.RemoveFromNetworks(this.cell, this, true);
		}
		base.OnCleanUp();
	}

	public Vent.State GetEndPointState()
	{
		Vent.State state = Vent.State.Invalid;
		Endpoint endpoint = this.endpointType;
		if (endpoint != Endpoint.Source)
		{
			if (endpoint == Endpoint.Sink)
			{
				state = Vent.State.Ready;
				int num = this.cell;
				if (!this.IsValidOutputCell(num))
				{
					state = ((!Grid.Solid[num]) ? Vent.State.OverPressure : Vent.State.Blocked);
				}
			}
		}
		else
		{
			state = ((!this.IsConnected()) ? Vent.State.Blocked : Vent.State.Ready);
		}
		return state;
	}

	public bool IsConnected()
	{
		IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitType);
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(this.cell);
		return networkForCell != null && (networkForCell as FlowUtilityNetwork).HasSinks;
	}

	public bool IsBlocked
	{
		get
		{
			return this.GetEndPointState() != Vent.State.Ready;
		}
	}

	private bool IsValidOutputCell(int output_cell)
	{
		bool flag = false;
		if ((this.structure == null || !this.structure.IsEntombed()) && !Grid.Solid[output_cell])
		{
			flag = Grid.Cell[output_cell].mass < this.overpressureMass;
		}
		return flag;
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		string formattedMass = GameUtil.GetFormattedMass(this.overpressureMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}");
		return new List<Descriptor>
		{
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.OVER_PRESSURE_MASS, formattedMass), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.OVER_PRESSURE_MASS, formattedMass), Descriptor.DescriptorType.Effect, false)
		};
	}

	private int cell = -1;

	private int sortKey;

	private bool connected = true;

	private Vent.StatesInstance smi;

	[SerializeField]
	public ConduitType conduitType = ConduitType.Gas;

	[SerializeField]
	public Endpoint endpointType;

	[SerializeField]
	public float overpressureMass = 1f;

	[NonSerialized]
	public bool showConnectivityIcons = true;

	[MyCmpGet]
	[NonSerialized]
	public Structure structure;

	public enum State
	{
		Invalid,
		Ready,
		Blocked,
		OverPressure
	}

	public class StatesInstance : GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.GameInstance
	{
		public StatesInstance(Vent master)
			: base(master)
		{
			this.exhaust = master.GetComponent<Exhaust>();
		}

		public bool NeedsExhaust()
		{
			return this.exhaust != null && base.master.GetEndPointState() != Vent.State.Ready && base.master.endpointType == Endpoint.Source;
		}

		public bool Blocked()
		{
			return base.master.GetEndPointState() == Vent.State.Blocked && base.master.endpointType != Endpoint.Source;
		}

		public bool OverPressure()
		{
			return this.exhaust != null && base.master.GetEndPointState() == Vent.State.OverPressure && base.master.endpointType != Endpoint.Source;
		}

		public void CheckTransitions()
		{
			if (this.NeedsExhaust())
			{
				base.smi.GoTo(base.sm.needExhaust);
			}
			else if (this.Blocked())
			{
				base.smi.GoTo(base.sm.blocked);
			}
			else if (this.OverPressure())
			{
				base.smi.GoTo(base.sm.overPressure);
			}
			else
			{
				base.smi.GoTo(base.sm.idle);
			}
		}

		public StatusItem SelectStatusItem(StatusItem gas_status_item, StatusItem liquid_status_item)
		{
			return (base.master.conduitType != ConduitType.Gas) ? liquid_status_item : gas_status_item;
		}

		private Exhaust exhaust;
	}

	public class States : GameStateMachine<Vent.States, Vent.StatesInstance, Vent>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.root.Update("CheckTransitions", delegate(Vent.StatesInstance smi)
			{
				smi.CheckTransitions();
			});
			this.blocked.ToggleStatusItem((Vent.StatesInstance smi) => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentObstructed, Db.Get().BuildingStatusItems.LiquidVentObstructed), null);
			this.overPressure.ToggleStatusItem((Vent.StatesInstance smi) => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentOverPressure, Db.Get().BuildingStatusItems.LiquidVentOverPressure), null);
		}

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State idle;

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State blocked;

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State overPressure;

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State needExhaust;

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent, object>.State venting;
	}
}
