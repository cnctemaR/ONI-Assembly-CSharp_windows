using System;
using System.Collections.ObjectModel;
using UnityEngine;

public class BuildQueue : KButtonMenu
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.keepMenuOpen = true;
		this.buttons = new KButtonMenu.ButtonInfo[6];
		for (int i = 0; i < 6; i++)
		{
			string text = (i + 1).ToString();
			int order_idx = i;
			this.buttons[i] = new KButtonMenu.ButtonInfo(text, global::Action.NumActions, delegate
			{
				this.CancelOrder(order_idx);
			}, null, null);
		}
	}

	public void CancelOrder(int order_idx)
	{
		if (this.fabricator != null)
		{
			if (this.fabricator.Orders.Count == 0 || order_idx >= this.fabricator.Orders.Count)
			{
				return;
			}
			this.fabricator.CancelOrder(order_idx);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
		}
	}

	private void Update()
	{
		int i = 0;
		if (this.fabricator != null)
		{
			ReadOnlyCollection<Fabricator.UserOrder> orders = this.fabricator.Orders;
			foreach (Fabricator.UserOrder userOrder in orders)
			{
				BuildQueueButton componentInChildren = this.buttonObjects[i].GetComponentInChildren<BuildQueueButton>();
				componentInChildren.SetOrder(userOrder);
				i++;
				if (i >= 6)
				{
					break;
				}
			}
			if (orders.Count > this.prevLength)
			{
				BuildQueueButton componentInChildren2 = this.buttonObjects[this.prevLength].GetComponentInChildren<BuildQueueButton>();
				SizePulse pulse = componentInChildren2.gameObject.AddComponent<SizePulse>();
				pulse.speed = 10f;
				pulse.updateWhenPaused = true;
				SizePulse pulse2 = pulse;
				pulse2.onComplete = (global::System.Action)Delegate.Combine(pulse2.onComplete, new global::System.Action(delegate
				{
					global::UnityEngine.Object.Destroy(pulse);
				}));
			}
			this.prevLength = orders.Count;
		}
		if (this.buttonObjects != null)
		{
			while (i < 6)
			{
				BuildQueueButton componentInChildren3 = this.buttonObjects[i].GetComponentInChildren<BuildQueueButton>();
				componentInChildren3.SetOrder(null);
				i++;
			}
		}
	}

	public void SetFabricator(Fabricator fabricator)
	{
		this.fabricator = fabricator;
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
		}
		this.RefreshButtons();
	}

	protected override void OnDeactivate()
	{
		for (int i = 0; i < 6; i++)
		{
			BuildQueueButton componentInChildren = this.buttonObjects[i].GetComponentInChildren<BuildQueueButton>();
			if (componentInChildren != null)
			{
				componentInChildren.SetOrder(null);
			}
		}
	}

	private Fabricator fabricator;

	private int prevLength;
}
