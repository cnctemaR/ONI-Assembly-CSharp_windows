using System;
using System.Collections.Generic;
using UnityEngine;
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
			this.checkboxContainerPool = new ObjectPool<CheckboxListGroupSideScreen.CheckboxContainer>(new Func<CheckboxListGroupSideScreen.CheckboxContainer>(this.InstantiateCheckboxContainer), 0);
			this.checkboxPool = new GameObjectPool(new Func<GameObject>(this.InstantiateCheckbox), 0);
		}
		if (buildTarget == this.currentBuildTarget)
		{
			this.Refresh(null);
			return;
		}
		this.currentBuildTarget = buildTarget;
		this.descriptionLabel.enabled = !this.targets[0].Description.IsNullOrWhiteSpace();
		if (!this.targets[0].Description.IsNullOrWhiteSpace())
		{
			this.descriptionLabel.SetText(this.targets[0].Description);
		}
		foreach (ICheckboxListGroupControl checkboxListGroupControl in this.targets)
		{
			foreach (ICheckboxListGroupControl.ListGroup listGroup in checkboxListGroupControl.GetData())
			{
				CheckboxListGroupSideScreen.CheckboxContainer instance = this.checkboxContainerPool.GetInstance();
				this.InitContainer(checkboxListGroupControl, listGroup, instance);
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
					this.InitContainer(checkboxListGroupControl, listGroup, this.checkboxContainerPool.GetInstance());
				}
				CheckboxListGroupSideScreen.CheckboxContainer checkboxContainer = this.activeChecklistGroups[num - 1];
				if (listGroup.resolveTitleCallback != null)
				{
					checkboxContainer.container.GetReference<LocText>("Text").SetText(listGroup.resolveTitleCallback(listGroup.title));
				}
				int num2 = 0;
				foreach (ICheckboxListGroupControl.CheckboxItem checkboxItem in listGroup.checkboxItems)
				{
					num2++;
					checkboxContainer.checkboxUIItems[num2 - 1].GetReference<Image>("Check").enabled = checkboxItem.isOn;
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
				GameObject gameObject = checkboxContainer.checkboxUIItems[j].gameObject;
				checkboxContainer.checkboxUIItems.RemoveAt(j);
				gameObject.SetActive(false);
				gameObject.transform.SetParent(this.checkboxParent);
				this.checkboxPool.ReleaseInstance(gameObject);
			}
			checkboxContainer.container.gameObject.SetActive(false);
			this.checkboxContainerPool.ReleaseInstance(checkboxContainer);
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
		ICheckboxListGroupControl.CheckboxItem[] checkboxItems = group.checkboxItems;
		for (int i = 0; i < checkboxItems.Length; i++)
		{
			ICheckboxListGroupControl.CheckboxItem item = checkboxItems[i];
			HierarchyReferences component = this.checkboxPool.GetInstance().GetComponent<HierarchyReferences>();
			groupUI.checkboxUIItems.Add(component);
			component.transform.SetParent(groupUI.container.transform);
			component.gameObject.SetActive(true);
			component.GetReference<LocText>("Text").SetText(item.text);
			component.GetReference<Image>("Check").enabled = item.isOn;
			ToolTip reference = component.GetReference<ToolTip>("Tooltip");
			reference.SetSimpleTooltip(item.tooltip);
			reference.refreshWhileHovering = item.resolveTooltipCallback != null;
			reference.OnToolTip = delegate
			{
				if (item.resolveTooltipCallback == null)
				{
					return item.tooltip;
				}
				return item.resolveTooltipCallback(item.tooltip, target);
			};
		}
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
