using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class CheckboxListGroupSideScreen : SideScreenContent
{
	private CheckboxListGroupSideScreen.CheckboxContainer InstantiateCheckboxContainer()
	{
		return new CheckboxListGroupSideScreen.CheckboxContainer(Util.KInstantiateUI(this.checkboxGroupPrefab, this.groupParent.gameObject, true).GetComponent<HierarchyReferences>());
	}

	private GameObject InstantiateCheckbox()
	{
		return Util.KInstantiateUI(this.checkboxPrefab, this.checkboxParent.gameObject, false);
	}

	protected override void OnSpawn()
	{
		this.checkboxPrefab.SetActive(false);
		this.checkboxGroupPrefab.SetActive(false);
		base.OnSpawn();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		ICheckboxListGroupControl[] components = target.GetComponents<ICheckboxListGroupControl>();
		if (components != null)
		{
			ICheckboxListGroupControl[] array = components;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].SidescreenEnabled())
				{
					return true;
				}
			}
		}
		using (List<ICheckboxListGroupControl>.Enumerator enumerator = target.GetAllSMI<ICheckboxListGroupControl>().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.SidescreenEnabled())
				{
					return true;
				}
			}
		}
		return false;
	}

	public override int GetSideScreenSortOrder()
	{
		if (this.targets == null)
		{
			return 20;
		}
		return this.targets[0].CheckboxSideScreenSortOrder();
	}

	public override void SetTarget(GameObject target)
	{
		if (target == null)
		{
			global::Debug.LogError("Invalid gameObject received");
			return;
		}
		this.targets = target.GetAllSMI<ICheckboxListGroupControl>();
		this.targets.AddRange(target.GetComponents<ICheckboxListGroupControl>());
		this.Rebuild(target);
		this.uiRefreshSubHandle = this.currentBuildTarget.Subscribe(1980521255, new Action<object>(this.Refresh));
	}

	public override void ClearTarget()
	{
		if (this.uiRefreshSubHandle != -1 && this.currentBuildTarget != null)
		{
			this.currentBuildTarget.Unsubscribe(this.uiRefreshSubHandle);
			this.uiRefreshSubHandle = -1;
		}
		this.ReleaseContainers(this.activeChecklistGroups.Count);
	}

	public override string GetTitle()
	{
		if (this.targets != null && this.targets.Count > 0 && this.targets[0] != null)
		{
			return this.targets[0].Title;
		}
		return base.GetTitle();
	}

	private void Rebuild(GameObject buildTarget)
	{
		if (this.checkboxContainerPool == null)
		{
			this.checkboxContainerPool = new ObjectPool<CheckboxListGroupSideScreen.CheckboxContainer>(new Func<CheckboxListGroupSideScreen.CheckboxContainer>(this.InstantiateCheckboxContainer), null, null, null, false, 10, 10000);
			this.checkboxPool = new GameObjectPool(new Func<GameObject>(this.InstantiateCheckbox), delegate(GameObject _)
			{
			}, 0);
		}
		this.descriptionLabel.enabled = !this.targets[0].Description.IsNullOrWhiteSpace();
		if (!this.targets[0].Description.IsNullOrWhiteSpace())
		{
			this.descriptionLabel.SetText(this.targets[0].Description);
		}
		if (buildTarget == this.currentBuildTarget)
		{
			this.Refresh(null);
			return;
		}
		this.currentBuildTarget = buildTarget;
		foreach (ICheckboxListGroupControl checkboxListGroupControl in this.targets)
		{
			foreach (ICheckboxListGroupControl.ListGroup listGroup in checkboxListGroupControl.GetData())
			{
				CheckboxListGroupSideScreen.CheckboxContainer checkboxContainer = this.checkboxContainerPool.Get();
				this.InitContainer(checkboxListGroupControl, listGroup, checkboxContainer);
			}
		}
	}

	[ContextMenu("Force refresh")]
	private void Test()
	{
		this.Refresh(null);
	}

	private void Refresh(object data = null)
	{
		int num = 0;
		foreach (ICheckboxListGroupControl checkboxListGroupControl in this.targets)
		{
			foreach (ICheckboxListGroupControl.ListGroup listGroup in checkboxListGroupControl.GetData())
			{
				if (++num > this.activeChecklistGroups.Count)
				{
					this.InitContainer(checkboxListGroupControl, listGroup, this.checkboxContainerPool.Get());
				}
				CheckboxListGroupSideScreen.CheckboxContainer checkboxContainer = this.activeChecklistGroups[num - 1];
				if (listGroup.resolveTitleCallback != null)
				{
					checkboxContainer.container.GetReference<LocText>("Text").SetText(listGroup.resolveTitleCallback(listGroup.title));
				}
				for (int j = 0; j < listGroup.checkboxItems.Length; j++)
				{
					ICheckboxListGroupControl.CheckboxItem checkboxItem = listGroup.checkboxItems[j];
					if (checkboxContainer.checkboxUIItems.Count <= j)
					{
						this.CreateSingleCheckBoxForGroupUI(checkboxContainer);
					}
					HierarchyReferences hierarchyReferences = checkboxContainer.checkboxUIItems[j];
					this.SetCheckboxData(hierarchyReferences, checkboxItem, checkboxListGroupControl);
				}
				while (checkboxContainer.checkboxUIItems.Count > listGroup.checkboxItems.Length)
				{
					HierarchyReferences hierarchyReferences2 = checkboxContainer.checkboxUIItems[checkboxContainer.checkboxUIItems.Count - 1];
					this.RemoveSingleCheckboxFromContainer(hierarchyReferences2, checkboxContainer);
				}
			}
		}
		this.ReleaseContainers(this.activeChecklistGroups.Count - num);
	}

	private void ReleaseContainers(int count)
	{
		int count2 = this.activeChecklistGroups.Count;
		for (int i = 1; i <= count; i++)
		{
			int num = count2 - i;
			CheckboxListGroupSideScreen.CheckboxContainer checkboxContainer = this.activeChecklistGroups[num];
			this.activeChecklistGroups.RemoveAt(num);
			for (int j = checkboxContainer.checkboxUIItems.Count - 1; j >= 0; j--)
			{
				HierarchyReferences hierarchyReferences = checkboxContainer.checkboxUIItems[j];
				this.RemoveSingleCheckboxFromContainer(hierarchyReferences, checkboxContainer);
			}
			checkboxContainer.container.gameObject.SetActive(false);
			this.checkboxContainerPool.Release(checkboxContainer);
		}
	}

	private void InitContainer(ICheckboxListGroupControl target, ICheckboxListGroupControl.ListGroup group, CheckboxListGroupSideScreen.CheckboxContainer groupUI)
	{
		this.activeChecklistGroups.Add(groupUI);
		groupUI.container.gameObject.SetActive(true);
		string text = group.title;
		if (group.resolveTitleCallback != null)
		{
			text = group.resolveTitleCallback(text);
		}
		groupUI.container.GetReference<LocText>("Text").SetText(text);
		foreach (ICheckboxListGroupControl.CheckboxItem checkboxItem in group.checkboxItems)
		{
			this.CreateSingleCheckBoxForGroupUI(checkboxItem, target, groupUI);
		}
	}

	public void RemoveSingleCheckboxFromContainer(HierarchyReferences checkbox, CheckboxListGroupSideScreen.CheckboxContainer container)
	{
		container.checkboxUIItems.Remove(checkbox);
		checkbox.gameObject.SetActive(false);
		checkbox.transform.SetParent(this.checkboxParent);
		this.checkboxPool.ReleaseInstance(checkbox.gameObject);
	}

	public HierarchyReferences CreateSingleCheckBoxForGroupUI(CheckboxListGroupSideScreen.CheckboxContainer container)
	{
		HierarchyReferences component = this.checkboxPool.GetInstance().GetComponent<HierarchyReferences>();
		component.gameObject.SetActive(true);
		container.checkboxUIItems.Add(component);
		component.transform.SetParent(container.container.transform);
		return component;
	}

	public HierarchyReferences CreateSingleCheckBoxForGroupUI(ICheckboxListGroupControl.CheckboxItem data, ICheckboxListGroupControl target, CheckboxListGroupSideScreen.CheckboxContainer container)
	{
		HierarchyReferences hierarchyReferences = this.CreateSingleCheckBoxForGroupUI(container);
		this.SetCheckboxData(hierarchyReferences, data, target);
		return hierarchyReferences;
	}

	public void SetCheckboxData(HierarchyReferences checkboxUI, ICheckboxListGroupControl.CheckboxItem data, ICheckboxListGroupControl target)
	{
		LocText reference = checkboxUI.GetReference<LocText>("Text");
		reference.SetText(data.text);
		reference.SetLinkOverrideAction(data.overrideLinkActions);
		checkboxUI.GetReference<Image>("Check").enabled = data.isOn;
		ToolTip reference2 = checkboxUI.GetReference<ToolTip>("Tooltip");
		reference2.SetSimpleTooltip(data.tooltip);
		reference2.refreshWhileHovering = data.resolveTooltipCallback != null;
		reference2.OnToolTip = delegate
		{
			if (data.resolveTooltipCallback == null)
			{
				return data.tooltip;
			}
			return data.resolveTooltipCallback(data.tooltip, target);
		};
	}

	public const int DefaultCheckboxListSideScreenSortOrder = 20;

	private ObjectPool<CheckboxListGroupSideScreen.CheckboxContainer> checkboxContainerPool;

	private GameObjectPool checkboxPool;

	[SerializeField]
	private GameObject checkboxGroupPrefab;

	[SerializeField]
	private GameObject checkboxPrefab;

	[SerializeField]
	private RectTransform groupParent;

	[SerializeField]
	private RectTransform checkboxParent;

	[SerializeField]
	private LocText descriptionLabel;

	private List<ICheckboxListGroupControl> targets;

	private GameObject currentBuildTarget;

	private int uiRefreshSubHandle = -1;

	private List<CheckboxListGroupSideScreen.CheckboxContainer> activeChecklistGroups = new List<CheckboxListGroupSideScreen.CheckboxContainer>();

	public class CheckboxContainer
	{
		public CheckboxContainer(HierarchyReferences container)
		{
			this.container = container;
		}

		public HierarchyReferences container;

		public List<HierarchyReferences> checkboxUIItems = new List<HierarchyReferences>();
	}
}
