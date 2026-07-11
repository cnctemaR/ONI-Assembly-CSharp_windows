using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;

public class EntombedItemManager : KMonoBehaviour, ISim33ms
{
	[OnDeserialized]
	private void OnDeserialized()
	{
		this.SpawnUncoveredObjects();
		this.PopulateEntombedItemVisualizers();
	}

	public static bool CanEntomb(Pickupable pickupable)
	{
		if (pickupable == null)
		{
			return false;
		}
		if (pickupable.storage != null)
		{
			return false;
		}
		int num = Grid.PosToCell(pickupable);
		if (!Grid.IsValidCell(num))
		{
			return false;
		}
		if (!Grid.Solid[num])
		{
			return false;
		}
		if (Grid.Objects[num, 9] != null)
		{
			return false;
		}
		PrimaryElement component = pickupable.GetComponent<PrimaryElement>();
		if (component.Element.IsSolid)
		{
			ElementChunk component2 = pickupable.GetComponent<ElementChunk>();
			if (component2 != null)
			{
				return true;
			}
		}
		return false;
	}

	public void Add(Pickupable pickupable)
	{
		this.pickupables.Add(pickupable);
	}

	public void Sim33ms(float dt)
	{
		EntombedItemVisualizer component = Game.Instance.GetComponent<EntombedItemVisualizer>();
		HashSetPool<Pickupable, EntombedItemManager>.PooledHashSet pooledHashSet = HashSetPool<Pickupable, EntombedItemManager>.Allocate();
		foreach (Pickupable pickupable in this.pickupables)
		{
			if (EntombedItemManager.CanEntomb(pickupable))
			{
				pooledHashSet.Add(pickupable);
			}
		}
		this.pickupables.Clear();
		foreach (Pickupable pickupable2 in pooledHashSet)
		{
			int num = Grid.PosToCell(pickupable2);
			PrimaryElement component2 = pickupable2.GetComponent<PrimaryElement>();
			SimHashes elementID = component2.ElementID;
			float mass = component2.Mass;
			float temperature = component2.Temperature;
			byte diseaseIdx = component2.DiseaseIdx;
			int diseaseCount = component2.DiseaseCount;
			component.AddItem(num);
			this.cells.Add(num);
			this.elementIds.Add((int)elementID);
			this.masses.Add(mass);
			this.temperatures.Add(temperature);
			this.diseaseIndices.Add(diseaseIdx);
			this.diseaseCounts.Add(diseaseCount);
			Util.KDestroyGameObject(pickupable2.gameObject);
		}
		pooledHashSet.Recycle();
	}

	public void OnSolidChanged(List<int> solid_changed_cells)
	{
		ListPool<int, EntombedItemManager>.PooledList pooledList = ListPool<int, EntombedItemManager>.Allocate();
		foreach (int num in solid_changed_cells)
		{
			if (!Grid.Solid[num])
			{
				pooledList.Add(num);
			}
		}
		ListPool<int, EntombedItemManager>.PooledList pooledList2 = ListPool<int, EntombedItemManager>.Allocate();
		for (int i = 0; i < this.cells.Count; i++)
		{
			int num2 = this.cells[i];
			foreach (int num3 in pooledList)
			{
				if (num2 == num3)
				{
					pooledList2.Add(i);
					break;
				}
			}
		}
		pooledList.Recycle();
		this.SpawnObjects(pooledList2);
		pooledList2.Recycle();
	}

	private void SpawnUncoveredObjects()
	{
		ListPool<int, EntombedItemManager>.PooledList pooledList = ListPool<int, EntombedItemManager>.Allocate();
		for (int i = 0; i < this.cells.Count; i++)
		{
			int num = this.cells[i];
			if (!Grid.Solid[num])
			{
				pooledList.Add(i);
			}
		}
		this.SpawnObjects(pooledList);
		pooledList.Recycle();
	}

	private void SpawnObjects(List<int> uncovered_item_indices)
	{
		uncovered_item_indices.Sort();
		uncovered_item_indices.Reverse();
		EntombedItemVisualizer component = Game.Instance.GetComponent<EntombedItemVisualizer>();
		foreach (int num in uncovered_item_indices)
		{
			int num2 = this.cells[num];
			int num3 = this.elementIds[num];
			float num4 = this.masses[num];
			float num5 = this.temperatures[num];
			byte b = this.diseaseIndices[num];
			int num6 = this.diseaseCounts[num];
			component.RemoveItem(num2);
			this.cells.RemoveAt(num);
			this.elementIds.RemoveAt(num);
			this.masses.RemoveAt(num);
			this.temperatures.RemoveAt(num);
			this.diseaseIndices.RemoveAt(num);
			this.diseaseCounts.RemoveAt(num);
			SimHashes simHashes = (SimHashes)num3;
			Element element = ElementLoader.FindElementByHash(simHashes);
			if (element != null)
			{
				element.substance.SpawnResource(Grid.CellToPosCCC(num2, Grid.SceneLayer.Ore), num4, num5, b, num6, false, false);
			}
		}
	}

	private void PopulateEntombedItemVisualizers()
	{
		EntombedItemVisualizer component = Game.Instance.GetComponent<EntombedItemVisualizer>();
		foreach (int num in this.cells)
		{
			component.AddItem(num);
		}
	}

	[Serialize]
	private List<int> cells = new List<int>();

	[Serialize]
	private List<int> elementIds = new List<int>();

	[Serialize]
	private List<float> masses = new List<float>();

	[Serialize]
	private List<float> temperatures = new List<float>();

	[Serialize]
	private List<byte> diseaseIndices = new List<byte>();

	[Serialize]
	private List<int> diseaseCounts = new List<int>();

	private List<Pickupable> pickupables = new List<Pickupable>();
}
