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

	public override void AddItem(int cell, object generic_item)
	{
		FlowUtilityNetwork.IItem item = (FlowUtilityNetwork.IItem)generic_item;
		if (item != null)
		{
			Endpoint endpointType = item.EndpointType;
			if (endpointType != Endpoint.Source)
			{
				if (endpointType != Endpoint.Sink)
				{
					item.Network = this;
				}
				else
				{
					if (this.sinks.Contains(item))
					{
						return;
					}
					this.sinks.Add(item);
					item.Network = this;
				}
			}
			else
			{
				if (this.sources.Contains(item))
				{
					return;
				}
				this.sources.Add(item);
				item.Network = this;
			}
		}
	}

	public override void Reset(UtilityNetworkGridNode[] grid)
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

		Endpoint EndpointType { get; }

		ConduitType ConduitType { get; }

		int SortKey { get; }
	}

	public class NetworkItem : FlowUtilityNetwork.IItem
	{
		public NetworkItem(ConduitType conduit_type, Endpoint endpoint_type, int cell, int sort_key = 1000)
		{
			this.conduitType = conduit_type;
			this.endpointType = endpoint_type;
			this.cell = cell;
			this.sortKey = sort_key;
		}

		public Endpoint EndpointType
		{
			get
			{
				return this.endpointType;
			}
		}

		public ConduitType ConduitType
		{
			get
			{
				return this.conduitType;
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

		private Endpoint endpointType;

		private ConduitType conduitType;
	}
}
