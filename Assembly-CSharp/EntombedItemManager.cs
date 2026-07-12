using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntombedItemManager")]
public class EntombedItemManager : KMonoBehaviour, ISim33ms
{
	[OnDeserialized]
	private void OnDeserialized()
	{
		this.SpawnUncoveredObjects();
		this.AddMassToWorldIfPossible();
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
		return Grid.IsValidCell(num) && Grid.Solid[num] && !(Grid.Objects[num, 9] != null) && (pickupable.GetComponent<PrimaryElement>().Element.IsSolid && pickupable.GetComponent<ElementChunk>() != null);
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
			Element element = Grid.Element[num];
			if (elementID == element.id && mass > 0.010000001f && Grid.Mass[num] + mass < element.maxMass)
			{
				SimMessages.AddRemoveSubstance(num, ElementLoader.FindElementByHash(elementID).idx, CellEventLogger.Instance.ElementConsumerSimUpdate, mass, temperature, diseaseIdx, diseaseCount, true, -1);
			}
			else
			{
				component.AddItem(num);
				this.cells.Add(num);
				this.elementIds.Add((int)elementID);
				this.masses.Add(mass);
				this.temperatures.Add(temperature);
				this.diseaseIndices.Add(diseaseIdx);
				this.diseaseCounts.Add(diseaseCount);
			}
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

	private void AddMassToWorldIfPossible()
	{
		ListPool<int, EntombedItemManager>.PooledList pooledList = ListPool<int, EntombedItemManager>.Allocate();
		for (int i = 0; i < this.cells.Count; i++)
		{
			int num = this.cells[i];
			if (Grid.Solid[num] && Grid.Element[num].id == (SimHashes)this.elementIds[i])
			{
				pooledList.Add(i);
			}
		}
		pooledList.Sort();
		pooledList.Reverse();
		foreach (int num2 in pooledList)
		{
			EntombedItemManager.Item item = this.GetItem(num2);
			this.RemoveItem(num2);
			if (item.mass > 1E-45f)
			{
				SimMessages.AddRemoveSubstance(item.cell, ElementLoader.FindElementByHash((SimHashes)item.elementId).idx, CellEventLogger.Instance.ElementConsumerSimUpdate, item.mass, item.temperature, item.diseaseIdx, item.diseaseCount, false, -1);
			}
		}
		pooledList.Recycle();
	}

	private void RemoveItem(int item_idx)
	{
		this.cells.RemoveAt(item_idx);
		this.elementIds.RemoveAt(item_idx);
		this.masses.RemoveAt(item_idx);
		this.temperatures.RemoveAt(item_idx);
		this.diseaseIndices.RemoveAt(item_idx);
		this.diseaseCounts.RemoveAt(item_idx);
	}

	private EntombedItemManager.Item GetItem(int item_idx)
	{
		return new EntombedItemManager.Item
		{
			cell = this.cells[item_idx],
			elementId = this.elementIds[item_idx],
			mass = this.masses[item_idx],
			temperature = this.temperatures[item_idx],
			diseaseIdx = this.diseaseIndices[item_idx],
			diseaseCount = this.diseaseCounts[item_idx]
		};
	}

	private void SpawnObjects(List<int> uncovered_item_indices)
	{
		uncovered_item_indices.Sort();
		uncovered_item_indices.Reverse();
		EntombedItemVisualizer component = Game.Instance.GetComponent<EntombedItemVisualizer>();
		foreach (int num in uncovered_item_indices)
		{
			EntombedItemManager.Item item = this.GetItem(num);
			component.RemoveItem(item.cell);
			this.RemoveItem(num);
			Element element = ElementLoader.FindElementByHash((SimHashes)item.elementId);
			if (element != null)
			{
				element.substance.SpawnResource(Grid.CellToPosCCC(item.cell, Grid.SceneLayer.Ore), item.mass, item.temperature, item.diseaseIdx, item.diseaseCount, false, false, false);
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

	private struct Item
	{
		public int cell;

		public int elementId;

		public float mass;

		public float temperature;

		public byte diseaseIdx;

		public int diseaseCount;
	}
}
