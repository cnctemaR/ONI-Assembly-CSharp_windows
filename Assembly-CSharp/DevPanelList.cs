using System;
using System.Collections.Generic;

public class DevPanelList
{
	public DevPanel AddPanelFor<T>() where T : DevTool, new()
	{
		return this.AddPanelFor(new T());
	}

	public DevPanel AddPanelFor(DevTool devTool)
	{
		DevPanel devPanel = new DevPanel(devTool, this);
		this.activePanels.Add(devPanel);
		return devPanel;
	}

	public Option<T> GetDevTool<T>() where T : DevTool
	{
		foreach (DevPanel devPanel in this.activePanels)
		{
			T t = devPanel.GetCurrentDevTool() as T;
			if (t != null)
			{
				return t;
			}
		}
		return Option.None;
	}

	public T AddOrGetDevTool<T>() where T : DevTool, new()
	{
		bool flag;
		T t;
		this.GetDevTool<T>().Deconstruct(out flag, out t);
		bool flag2 = flag;
		T t2 = t;
		if (!flag2)
		{
			t2 = new T();
			this.AddPanelFor(t2);
		}
		return t2;
	}

	public void ClosePanel(DevPanel panel)
	{
		if (this.activePanels.Remove(panel))
		{
			panel.Internal_Uninit();
		}
	}

	public void Render()
	{
		if (this.activePanels.Count == 0)
		{
			return;
		}
		using (ListPool<DevPanel, DevPanelList>.PooledList pooledList = ListPool<DevPanel, DevPanelList>.Allocate())
		{
			for (int i = 0; i < this.activePanels.Count; i++)
			{
				DevPanel devPanel = this.activePanels[i];
				devPanel.RenderPanel();
				if (devPanel.isRequestingToClose)
				{
					pooledList.Add(devPanel);
				}
			}
			foreach (DevPanel devPanel2 in pooledList)
			{
				this.ClosePanel(devPanel2);
			}
		}
	}

	public void Internal_InitPanelId(Type initialDevToolType, out string panelId, out uint idPostfixNumber)
	{
		idPostfixNumber = this.Internal_GetUniqueIdPostfix(initialDevToolType);
		panelId = initialDevToolType.Name + idPostfixNumber.ToString();
	}

	public uint Internal_GetUniqueIdPostfix(Type initialDevToolType)
	{
		uint num3;
		using (HashSetPool<uint, DevPanelList>.PooledHashSet pooledHashSet = HashSetPool<uint, DevPanelList>.Allocate())
		{
			foreach (DevPanel devPanel in this.activePanels)
			{
				if (!(devPanel.initialDevToolType != initialDevToolType))
				{
					pooledHashSet.Add(devPanel.idPostfixNumber);
				}
			}
			for (uint num = 0U; num < 100U; num += 1U)
			{
				if (!pooledHashSet.Contains(num))
				{
					return num;
				}
			}
			Debug.Assert(false, "Something went wrong, this should only assert if there's over 100 of the same type of debug window");
			uint num2 = this.fallbackUniqueIdPostfixNumber;
			this.fallbackUniqueIdPostfixNumber = num2 + 1U;
			num3 = num2;
		}
		return num3;
	}

	private List<DevPanel> activePanels = new List<DevPanel>();

	private uint fallbackUniqueIdPostfixNumber = 300U;
}
