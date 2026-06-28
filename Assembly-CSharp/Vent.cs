using System;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class Vent : KMonoBehaviour, IDisconnectable, FlowUtilityNetwork.IItem
{
	public int Cell
	{
		get
		{
			return this.cell;
		}
	}

	public Vent.Transfer TransferType
	{
		get
		{
			return this.transferType;
		}
	}

	public Vent.Endpoint EndpointType
	{
		get
		{
			return this.endpointType;
		}
	}

	public FlowUtilityNetwork Network
	{
		get
		{
			return this.network;
		}
		set
		{
			this.network = value;
		}
	}

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

	public IUtilityNetworkMgr GetConduitManager()
	{
		return (this.transferType != Vent.Transfer.Gas) ? Game.Instance.liquidConduitSystem : Game.Instance.gasConduitSystem;
	}

	public ConduitFlow GetConduitFlowManager()
	{
		return (this.transferType != Vent.Transfer.Gas) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;
	}

	protected override void OnSpawn()
	{
		Building component = base.GetComponent<Building>();
		CellOffset cellOffset = ((this.endpointType != Vent.Endpoint.Source) ? component.GetUtilityInputOffset() : component.GetUtilityOutputOffset());
		CellOffset rotatedOffset = component.GetRotatedOffset(this.DynamicOffset);
		cellOffset += rotatedOffset;
		this.cell = Grid.PosToCell(this.transform.position);
		this.cell = Grid.OffsetCell(this.cell, cellOffset);
		if (this.endpointType == Vent.Endpoint.Conduit)
		{
			base.enabled = false;
		}
		else
		{
			this.smi = new Vent.StatesInstance(this);
			this.smi.StartSM();
		}
		if (this.connected)
		{
			IUtilityNetworkMgr conduitManager = this.GetConduitManager();
			conduitManager.AddToNetworks(this.cell, this);
		}
		this.initialized = true;
		this.Trigger(-1305509372, null);
		this.Subscribe(-1041684577, new EventSystem.EventHandler(this.OnConnectionsChanged));
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[this.cell, (int)component.Def.ReplacementLayer] == null)
		{
			IUtilityNetworkMgr conduitManager = this.GetConduitManager();
			conduitManager.RemoveFromNetworks(this.cell, this);
			conduitManager.ConduitFlowManager.EmptyConduit(Grid.PosToCell(this.transform.position));
		}
	}

	public Vent.State GetEndPointState()
	{
		Vent.State state = Vent.State.Invalid;
		switch (this.endpointType)
		{
		case Vent.Endpoint.Source:
			state = ((!this.IsConnected()) ? Vent.State.Blocked : Vent.State.Ready);
			break;
		case Vent.Endpoint.Sink:
		case Vent.Endpoint.Consumer:
		{
			state = Vent.State.Ready;
			int num = this.cell;
			if (!this.IsValidOutputCell(num))
			{
				state = ((!Grid.Solid[num]) ? Vent.State.OverPressure : Vent.State.Blocked);
			}
			break;
		}
		}
		return state;
	}

	public bool IsConnected()
	{
		return this.network != null && this.network.HasSinks;
	}

	public bool IsBlocked
	{
		get
		{
			return this.GetEndPointState() != Vent.State.Ready;
		}
	}

	private bool GasPressureRangeValid(int cell)
	{
		return Grid.Cell[cell].mass < 2f;
	}

	private bool LiquidPressureRangeValid(int cell)
	{
		return Grid.Cell[cell].mass < 1000f;
	}

	private bool IsValidOutputCell(int output_cell)
	{
		bool flag = false;
		if ((this.structure == null || !this.structure.IsEntombed()) && !Grid.Solid[output_cell])
		{
			Vent.Transfer transfer = this.transferType;
			if (transfer != Vent.Transfer.Gas)
			{
				if (transfer == Vent.Transfer.Liquid)
				{
					flag = this.LiquidPressureRangeValid(output_cell);
				}
			}
			else
			{
				flag = this.GasPressureRangeValid(output_cell);
			}
		}
		return flag;
	}

	public void Connect()
	{
		if (!this.connected && this.initialized)
		{
			this.connected = true;
			int num = Grid.PosToCell(this.transform.position);
			if (this.cachedConnections != (UtilityConnections)0)
			{
				IUtilityNetworkMgr conduitManager = this.GetConduitManager();
				conduitManager.SetConnections(this.cachedConnections, num, true);
				conduitManager.AddToNetworks(num, this);
			}
			EventSystem.Trigger(base.gameObject, -2094018600, true);
		}
	}

	public void Disconnect()
	{
		if (this.connected && this.initialized)
		{
			this.connected = false;
			int num = Grid.PosToCell(this.transform.position);
			BuildingComplete component = base.GetComponent<BuildingComplete>();
			if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
			{
				IUtilityNetworkMgr conduitManager = this.GetConduitManager();
				this.cachedConnections = conduitManager.GetConnections(num, true);
				conduitManager.RemoveFromNetworks(num, this);
			}
			EventSystem.Trigger(base.gameObject, -2094018600, false);
		}
	}

	private void OnConnectionsChanged(object data)
	{
		UtilityConnections utilityConnections = (UtilityConnections)((int)data);
		this.cachedConnections = utilityConnections;
		if (this.connected && utilityConnections != (UtilityConnections)0)
		{
			int num = Grid.PosToCell(this.transform.position);
			IUtilityNetworkMgr conduitManager = this.GetConduitManager();
			conduitManager.SetConnections(this.cachedConnections, num, true);
			conduitManager.AddToNetworks(num, this);
		}
	}

	[Serialize]
	private UtilityConnections cachedConnections;

	[Serialize]
	public Vent.Transfer transferType;

	[Serialize]
	public Vent.Endpoint endpointType;

	[NonSerialized]
	public FlowUtilityNetwork network;

	[NonSerialized]
	public bool showConnectivityIcons = true;

	[MyCmpGet]
	[NonSerialized]
	public Structure structure;

	private int sortKey;

	private bool connected = true;

	private bool initialized;

	private Vent.StatesInstance smi;

	private int cell = -1;

	public CellOffset DynamicOffset = CellOffset.none;

	public class StatesInstance : GameStateMachine<Vent.States, Vent.StatesInstance, Vent>.GameInstance
	{
		public StatesInstance(Vent master)
			: base(master)
		{
		}

		public bool NeedsExhaust()
		{
			return base.master.GetComponent<Exhaust>() != null && base.master.GetEndPointState() != Vent.State.Ready && base.master.endpointType == Vent.Endpoint.Source;
		}

		public bool Blocked()
		{
			return base.master.GetEndPointState() == Vent.State.Blocked && base.master.endpointType != Vent.Endpoint.Source;
		}

		public bool OverPressure()
		{
			return base.master.GetComponent<Exhaust>() != null && base.master.GetEndPointState() == Vent.State.OverPressure && base.master.endpointType != Vent.Endpoint.Source;
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
			if (base.master.transferType == Vent.Transfer.Gas)
			{
				return gas_status_item;
			}
			return liquid_status_item;
		}
	}

	public class States : GameStateMachine<Vent.States, Vent.StatesInstance, Vent>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.idle;
			this.idle.Update("CheckTransitions", delegate(Vent.StatesInstance smi)
			{
				smi.CheckTransitions();
			});
			this.blocked.ToggleStatusItem((Vent.StatesInstance smi) => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentObstructed, Db.Get().BuildingStatusItems.LiquidVentObstructed), null);
			this.overPressure.ToggleStatusItem((Vent.StatesInstance smi) => smi.SelectStatusItem(Db.Get().BuildingStatusItems.GasVentOverPressure, Db.Get().BuildingStatusItems.LiquidVentOverPressure), null);
		}

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent>.State idle;

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent>.State blocked;

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent>.State overPressure;

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent>.State needExhaust;

		public GameStateMachine<Vent.States, Vent.StatesInstance, Vent>.State venting;
	}

	public enum Transfer
	{
		Gas,
		Liquid,
		NumTypes
	}

	public enum Endpoint
	{
		Conduit,
		Source,
		Sink,
		Consumer
	}

	public enum State
	{
		Invalid,
		Ready,
		Blocked,
		OverPressure
	}
}
