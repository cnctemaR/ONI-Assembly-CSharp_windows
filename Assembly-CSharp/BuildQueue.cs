using System;
using System.Collections.Generic;
using STRINGS;
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
			if (this.fabricator.NumOrders == 0 || order_idx >= this.fabricator.NumOrders)
			{
				return;
			}
			this.fabricator.CancelOrder(order_idx);
		}
	}

	private void Update()
	{
		this.allocatedMaterials.Clear();
		int i = 0;
		if (this.fabricator != null)
		{
			List<IBuildQueueOrder> orders = this.fabricator.Orders;
			foreach (IBuildQueueOrder buildQueueOrder in orders)
			{
				BuildQueueButton componentInChildren = this.buttonObjects[i].GetComponentInChildren<BuildQueueButton>();
				componentInChildren.SetOrder(buildQueueOrder);
				bool flag = true;
				string text = string.Empty;
				foreach (KeyValuePair<Tag, float> keyValuePair in buildQueueOrder.GetMaterialRequirements())
				{
					float num = keyValuePair.Value - WorldInventory.Instance.GetAmount(keyValuePair.Key);
					for (int j = 0; j < this.availableMaterialStorages.Count; j++)
					{
						num -= this.availableMaterialStorages[j].GetAmountAvailable(keyValuePair.Key);
					}
					if (this.allocatedMaterials.ContainsKey(keyValuePair.Key))
					{
						num += this.allocatedMaterials[keyValuePair.Key];
					}
					if (num > 0f)
					{
						flag = false;
						text += string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.QUEUED_MISSING_INGREDIENTS_TOOLTIP, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), keyValuePair.Key.ProperName());
					}
					if (this.allocatedMaterials.ContainsKey(keyValuePair.Key))
					{
						Dictionary<Tag, float> dictionary;
						Tag key;
						(dictionary = this.allocatedMaterials)[key = keyValuePair.Key] = dictionary[key] + keyValuePair.Value;
					}
					else
					{
						this.allocatedMaterials.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
				componentInChildren.SetAvailability(buildQueueOrder.Result.ProperName(), flag, text);
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

	public void SetFabricator(IHasBuildQueue fabricator)
	{
		this.availableMaterialStorages.Clear();
		this.fabricator = fabricator;
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(true);
		}
		this.RefreshButtons();
	}

	public void AddAvailableMaterialStorage(Storage storage)
	{
		if (!this.availableMaterialStorages.Contains(storage))
		{
			this.availableMaterialStorages.Add(storage);
		}
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

	private IHasBuildQueue fabricator;

	private int prevLength;

	private Dictionary<Tag, float> allocatedMaterials = new Dictionary<Tag, float>();

	public List<Storage> availableMaterialStorages = new List<Storage>();
}
