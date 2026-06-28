using System;
using System.Collections.Generic;

public class FlowUtilityNetwork : UtilityNetwork
{
	public bool HasSinks
	{
		get
		{
			return this.sinks.Count > 0;
		}
	}

	public int GetActiveCount()
	{
		return this.sinks.Count;
	}

	protected override void AddItemInternal(int cell, FlowUtilityNetwork.IItem item)
	{
		if (item != null && (item.TransferType == this.transferType || item.TransferType == Vent.Transfer.NumTypes))
		{
			switch (item.EndpointType)
			{
			case Vent.Endpoint.Conduit:
				if (this.conduits.Contains(item))
				{
					return;
				}
				this.conduits.Add(item);
				item.Network = this;
				break;
			case Vent.Endpoint.Source:
				if (this.sources.Contains(item))
				{
					return;
				}
				this.sources.Add(item);
				item.Network = this;
				break;
			case Vent.Endpoint.Sink:
			case Vent.Endpoint.Consumer:
				if (this.sinks.Contains(item))
				{
					return;
				}
				this.sinks.Add(item);
				item.Network = this;
				break;
			}
		}
	}

	public override void Reset()
	{
		for (int i = 0; i < this.sinks.Count; i++)
		{
			FlowUtilityNetwork.IItem item = this.sinks[i];
			item.Network = null;
		}
		for (int j = 0; j < this.sources.Count; j++)
		{
			FlowUtilityNetwork.IItem item2 = this.sources[j];
			item2.Network = null;
		}
		for (int k = 0; k < this.conduits.Count; k++)
		{
			FlowUtilityNetwork.IItem item3 = this.conduits[k];
			item3.Network = null;
		}
	}

	public List<FlowUtilityNetwork.IItem> sources = new List<FlowUtilityNetwork.IItem>();

	public List<FlowUtilityNetwork.IItem> sinks = new List<FlowUtilityNetwork.IItem>();

	private List<FlowUtilityNetwork.IItem> conduits = new List<FlowUtilityNetwork.IItem>();

	public interface IItem
	{
		int Cell { get; }

		FlowUtilityNetwork Network { set; }

		Vent.Transfer TransferType { get; }

		Vent.Endpoint EndpointType { get; }

		int SortKey { get; }
	}

	public class NetworkItem : FlowUtilityNetwork.IItem
	{
		public NetworkItem(Vent.Transfer transfer_type, Vent.Endpoint endpoint_type, int cell, int sort_key = 1000, Action<FlowUtilityNetwork> on_set_network = null)
		{
			this.transferType = transfer_type;
			this.endpointType = endpoint_type;
			this.cell = cell;
			this.sortKey = sort_key;
			this.onSetNetwork = on_set_network;
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

		public int Cell
		{
			get
			{
				return this.cell;
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
				if (this.onSetNetwork != null)
				{
					this.onSetNetwork(value);
				}
			}
		}

		public int SortKey
		{
			get
			{
				return this.sortKey;
			}
		}

		public const int DefaultSortKey = 1000;

		private int cell;

		private int sortKey;

		private FlowUtilityNetwork network;

		private Vent.Transfer transferType;

		private Vent.Endpoint endpointType;

		private Action<FlowUtilityNetwork> onSetNetwork;
	}
}
