using System;
using System.Collections;
using System.Collections.Generic;
using Klei.AI;

public class Components
{
	public static Components.Cmps<MinionIdentity> LiveMinionIdentities = new Components.Cmps<MinionIdentity>();

	public static Components.Cmps<MinionIdentity> MinionIdentities = new Components.Cmps<MinionIdentity>();

	public static Components.Cmps<Sleepable> Sleepables = new Components.Cmps<Sleepable>();

	public static Components.Cmps<IUsable> Toilets = new Components.Cmps<IUsable>();

	public static Components.Cmps<MessStation> MessStations = new Components.Cmps<MessStation>();

	public static Components.Cmps<Pickupable> Pickupables = new Components.Cmps<Pickupable>();

	public static Components.Cmps<Brain> Brains = new Components.Cmps<Brain>();

	public static Components.Cmps<Meter> Meters = new Components.Cmps<Meter>();

	public static Components.Cmps<BuildingComplete> BuildingCompletes = new Components.Cmps<BuildingComplete>();

	public static Components.Cmps<Notifier> Notifiers = new Components.Cmps<Notifier>();

	public static Components.Cmps<OxygenBreather> OxygenBreathers = new Components.Cmps<OxygenBreather>();

	public static Components.Cmps<Fabricator> Fabricators = new Components.Cmps<Fabricator>();

	public static Components.Cmps<PlantablePlot> PlantablePlots = new Components.Cmps<PlantablePlot>();

	public static Components.Cmps<Constructable> Constructables = new Components.Cmps<Constructable>();

	public static Components.Cmps<Ladder> Ladders = new Components.Cmps<Ladder>();

	public static Components.Cmps<Door> Doors = new Components.Cmps<Door>();

	public static Components.Cmps<Light2D> Light2Ds = new Components.Cmps<Light2D>();

	public static Components.Cmps<InfraredVisualizer> InfraredVisualizers = new Components.Cmps<InfraredVisualizer>();

	public static Components.Cmps<Attackable> Attackables = new Components.Cmps<Attackable>();

	public static Components.Cmps<Workable> Workables = new Components.Cmps<Workable>();

	public static Components.Cmps<Worker> Workers = new Components.Cmps<Worker>();

	public static Components.Cmps<Edible> Edibles = new Components.Cmps<Edible>();

	public static Components.Cmps<Spawner> Spawners = new Components.Cmps<Spawner>();

	public static Components.Cmps<Diggable> Diggables = new Components.Cmps<Diggable>();

	public static Components.Cmps<ResearchCenter> ResearchCenters = new Components.Cmps<ResearchCenter>();

	public static Components.Cmps<Harvestable> Harvestables = new Components.Cmps<Harvestable>();

	public static Components.Cmps<Uprootable> Uprootables = new Components.Cmps<Uprootable>();

	public static Components.Cmps<TrashRegion> TrashRegions = new Components.Cmps<TrashRegion>();

	public static Components.Cmps<Clearable> Clearables = new Components.Cmps<Clearable>();

	public static Components.Cmps<LiquidSource> LiquidSources = new Components.Cmps<LiquidSource>();

	public static Components.Cmps<Modifiers> Modifiers = new Components.Cmps<Modifiers>();

	public static Components.Cmps<Health> Health = new Components.Cmps<Health>();

	public static Components.Cmps<FactionAlignment> FactionAlignments = new Components.Cmps<FactionAlignment>();

	public static Components.Cmps<Clinic> Clinics = new Components.Cmps<Clinic>();

	public class Cmps<T> : IEnumerable, IEnumerable<T>
	{
		public Cmps()
		{
			App.OnPreLoadScene = (global::System.Action)Delegate.Combine(App.OnPreLoadScene, new global::System.Action(this.Clear));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.Items.Count;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		public T this[int idx]
		{
			get
			{
				return this.Items[idx];
			}
		}

		private void Clear()
		{
			this.Items.Clear();
			this.OnAdd = null;
			this.OnRemove = null;
		}

		public void Add(T cmp)
		{
			this.Items.Add(cmp);
			if (this.OnAdd != null)
			{
				this.OnAdd(cmp);
			}
		}

		public void Remove(T cmp)
		{
			this.Items.Remove(cmp);
			if (this.OnRemove != null)
			{
				this.OnRemove(cmp);
			}
		}

		public void Register(Action<T> on_add, Action<T> on_remove)
		{
			this.OnAdd = (Action<T>)Delegate.Combine(this.OnAdd, on_add);
			this.OnRemove = (Action<T>)Delegate.Combine(this.OnRemove, on_remove);
			foreach (T t in this.Items)
			{
				this.OnAdd(t);
			}
		}

		public void Unregister(Action<T> on_add, Action<T> on_remove)
		{
			this.OnAdd = (Action<T>)Delegate.Remove(this.OnAdd, on_add);
			this.OnRemove = (Action<T>)Delegate.Remove(this.OnRemove, on_remove);
		}

		private List<T> Items = new List<T>();

		public Action<T> OnAdd;

		public Action<T> OnRemove;
	}
}
