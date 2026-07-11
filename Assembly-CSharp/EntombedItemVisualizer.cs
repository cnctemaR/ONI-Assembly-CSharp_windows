using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/EntombedItemVisualizer")]
public class EntombedItemVisualizer : KMonoBehaviour
{
	public void Clear()
	{
		this.cellEntombedCounts.Clear();
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.entombedItemPool = new ObjectPool(new Func<GameObject>(this.InstantiateEntombedObject), 32);
	}

	public bool AddItem(int cell)
	{
		bool flag = false;
		if (Grid.Objects[cell, 9] == null)
		{
			flag = true;
			EntombedItemVisualizer.Data data;
			this.cellEntombedCounts.TryGetValue(cell, out data);
			if (data.refCount == 0)
			{
				GameObject instance = this.entombedItemPool.GetInstance();
				instance.transform.SetPosition(Grid.CellToPosCCC(cell, Grid.SceneLayer.FXFront));
				instance.transform.rotation = Quaternion.Euler(0f, 0f, global::UnityEngine.Random.value * 360f);
				KBatchedAnimController component = instance.GetComponent<KBatchedAnimController>();
				int num = global::UnityEngine.Random.Range(0, EntombedItemVisualizer.EntombedVisualizerAnims.Length);
				string text = EntombedItemVisualizer.EntombedVisualizerAnims[num];
				component.initialAnim = text;
				instance.SetActive(true);
				component.Play(text, KAnim.PlayMode.Once, 1f, 0f);
				data.controller = component;
			}
			data.refCount++;
			this.cellEntombedCounts[cell] = data;
		}
		return flag;
	}

	public void RemoveItem(int cell)
	{
		EntombedItemVisualizer.Data data;
		if (this.cellEntombedCounts.TryGetValue(cell, out data))
		{
			data.refCount--;
			if (data.refCount == 0)
			{
				this.ReleaseVisualizer(cell, data);
				return;
			}
			this.cellEntombedCounts[cell] = data;
		}
	}

	public void ForceClear(int cell)
	{
		EntombedItemVisualizer.Data data;
		if (this.cellEntombedCounts.TryGetValue(cell, out data))
		{
			this.ReleaseVisualizer(cell, data);
		}
	}

	private void ReleaseVisualizer(int cell, EntombedItemVisualizer.Data data)
	{
		if (data.controller != null)
		{
			data.controller.gameObject.SetActive(false);
			this.entombedItemPool.ReleaseInstance(data.controller.gameObject);
		}
		this.cellEntombedCounts.Remove(cell);
	}

	public bool IsEntombedItem(int cell)
	{
		return this.cellEntombedCounts.ContainsKey(cell) && this.cellEntombedCounts[cell].refCount > 0;
	}

	private GameObject InstantiateEntombedObject()
	{
		GameObject gameObject = GameUtil.KInstantiate(this.entombedItemPrefab, Grid.SceneLayer.FXFront, null, 0);
		gameObject.SetActive(false);
		return gameObject;
	}

	[SerializeField]
	private GameObject entombedItemPrefab;

	private static readonly string[] EntombedVisualizerAnims = new string[] { "idle1", "idle2", "idle3", "idle4" };

	private ObjectPool entombedItemPool;

	private Dictionary<int, EntombedItemVisualizer.Data> cellEntombedCounts = new Dictionary<int, EntombedItemVisualizer.Data>();

	private struct Data
	{
		public int refCount;

		public KBatchedAnimController controller;
	}
}
