using System;
using System.Collections.Generic;
using UnityEngine;

public class ClusterNameDisplayScreen : KScreen
{
	public static void DestroyInstance()
	{
		ClusterNameDisplayScreen.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ClusterNameDisplayScreen.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	public void AddNewEntry(ClusterGridEntity representedObject)
	{
		if (this.GetEntry(representedObject) != null)
		{
			return;
		}
		ClusterNameDisplayScreen.Entry entry = new ClusterNameDisplayScreen.Entry();
		entry.grid_entity = representedObject;
		GameObject gameObject = Util.KInstantiateUI(this.nameAndBarsPrefab, base.gameObject, true);
		entry.display_go = gameObject;
		gameObject.name = representedObject.name + " cluster overlay";
		entry.Name = representedObject.name;
		entry.refs = gameObject.GetComponent<HierarchyReferences>();
		entry.bars_go = entry.refs.GetReference<RectTransform>("Bars").gameObject;
		this.m_entries.Add(entry);
		if (representedObject.GetComponent<KSelectable>() != null)
		{
			this.UpdateName(representedObject);
			this.UpdateBars(representedObject);
		}
	}

	private void LateUpdate()
	{
		if (App.isLoading || App.IsExiting)
		{
			return;
		}
		int num = this.m_entries.Count;
		int i = 0;
		while (i < num)
		{
			if (this.m_entries[i].grid_entity != null && ClusterMapScreen.GetRevealLevel(this.m_entries[i].grid_entity) == ClusterRevealLevel.Visible)
			{
				Transform gridEntityNameTarget = ClusterMapScreen.Instance.GetGridEntityNameTarget(this.m_entries[i].grid_entity);
				if (gridEntityNameTarget != null)
				{
					Vector3 position = gridEntityNameTarget.GetPosition();
					this.m_entries[i].display_go.GetComponent<RectTransform>().SetPositionAndRotation(position, Quaternion.identity);
					this.m_entries[i].display_go.SetActive(this.m_entries[i].grid_entity.IsVisible && this.m_entries[i].grid_entity.ShowName());
				}
				else if (this.m_entries[i].display_go.activeSelf)
				{
					this.m_entries[i].display_go.SetActive(false);
				}
				this.UpdateBars(this.m_entries[i].grid_entity);
				if (this.m_entries[i].bars_go != null)
				{
					this.m_entries[i].bars_go.GetComponentsInChildren<KCollider2D>(false, this.workingList);
					foreach (KCollider2D kcollider2D in this.workingList)
					{
						kcollider2D.MarkDirty(false);
					}
				}
				i++;
			}
			else
			{
				global::UnityEngine.Object.Destroy(this.m_entries[i].display_go);
				num--;
				this.m_entries[i] = this.m_entries[num];
			}
		}
		this.m_entries.RemoveRange(num, this.m_entries.Count - num);
	}

	public void UpdateName(ClusterGridEntity representedObject)
	{
		ClusterNameDisplayScreen.Entry entry = this.GetEntry(representedObject);
		if (entry == null)
		{
			return;
		}
		KSelectable component = representedObject.GetComponent<KSelectable>();
		entry.display_go.name = component.GetProperName() + " cluster overlay";
		LocText componentInChildren = entry.display_go.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
			componentInChildren.text = component.GetProperName();
		}
	}

	private void UpdateBars(ClusterGridEntity representedObject)
	{
		ClusterNameDisplayScreen.Entry entry = this.GetEntry(representedObject);
		if (entry == null)
		{
			return;
		}
		GenericUIProgressBar componentInChildren = entry.bars_go.GetComponentInChildren<GenericUIProgressBar>(true);
		if (entry.grid_entity.ShowProgressBar())
		{
			if (!componentInChildren.gameObject.activeSelf)
			{
				componentInChildren.gameObject.SetActive(true);
			}
			componentInChildren.SetFillPercentage(entry.grid_entity.GetProgress());
			return;
		}
		if (componentInChildren.gameObject.activeSelf)
		{
			componentInChildren.gameObject.SetActive(false);
		}
	}

	private ClusterNameDisplayScreen.Entry GetEntry(ClusterGridEntity entity)
	{
		return this.m_entries.Find((ClusterNameDisplayScreen.Entry entry) => entry.grid_entity == entity);
	}

	public static ClusterNameDisplayScreen Instance;

	public GameObject nameAndBarsPrefab;

	[SerializeField]
	private Color selectedColor;

	[SerializeField]
	private Color defaultColor;

	private List<ClusterNameDisplayScreen.Entry> m_entries = new List<ClusterNameDisplayScreen.Entry>();

	private List<KCollider2D> workingList = new List<KCollider2D>();

	private class Entry
	{
		public string Name;

		public ClusterGridEntity grid_entity;

		public GameObject display_go;

		public GameObject bars_go;

		public HierarchyReferences refs;
	}
}
