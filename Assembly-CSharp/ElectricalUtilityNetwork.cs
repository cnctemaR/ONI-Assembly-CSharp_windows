using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ElectricalUtilityNetwork : UtilityNetwork
{
	public override void AddItem(int cell, object item)
	{
		Wire wire = (Wire)item;
		Wire.WattageRating maxWattageRating = wire.MaxWattageRating;
		List<Wire> list = this.wireGroups[(int)maxWattageRating];
		if (list == null)
		{
			list = new List<Wire>();
			this.wireGroups[(int)maxWattageRating] = list;
		}
		list.Add(wire);
		this.timeOverloaded = Mathf.Max(this.timeOverloaded, wire.circuitOverloadTime);
	}

	public override void Reset(UtilityNetworkGridNode[] grid)
	{
		for (int i = 0; i < 5; i++)
		{
			List<Wire> list = this.wireGroups[i];
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					Wire wire = list[j];
					if (wire != null)
					{
						wire.circuitOverloadTime = this.timeOverloaded;
						int num = Grid.PosToCell(wire.transform.GetPosition());
						UtilityNetworkGridNode utilityNetworkGridNode = grid[num];
						utilityNetworkGridNode.networkIdx = -1;
						grid[num] = utilityNetworkGridNode;
					}
				}
				list.Clear();
			}
		}
		this.RemoveOverloadedNotification();
	}

	public void UpdateOverloadTime(float dt, float watts_used, List<WireUtilityNetworkLink>[] bridgeGroups)
	{
		bool flag = false;
		List<Wire> list = null;
		List<WireUtilityNetworkLink> list2 = null;
		for (int i = 0; i < 5; i++)
		{
			List<Wire> list3 = this.wireGroups[i];
			List<WireUtilityNetworkLink> list4 = bridgeGroups[i];
			Wire.WattageRating wattageRating = (Wire.WattageRating)i;
			float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(wattageRating);
			if (watts_used > maxWattageAsFloat && ((list4 != null && list4.Count > 0) || (list3 != null && list3.Count > 0)))
			{
				flag = true;
				list = list3;
				list2 = list4;
				break;
			}
		}
		if (list != null)
		{
			list.RemoveAll((Wire x) => x == null);
		}
		if (list2 != null)
		{
			list2.RemoveAll((WireUtilityNetworkLink x) => x == null);
		}
		if (flag)
		{
			this.timeOverloaded += dt;
			if (this.timeOverloaded > 6f)
			{
				this.timeOverloaded = 0f;
				if (this.targetOverloadedWire == null)
				{
					if (list2 != null && list2.Count > 0)
					{
						int num = global::UnityEngine.Random.Range(0, list2.Count);
						this.targetOverloadedWire = list2[num].gameObject;
					}
					else if (list != null && list.Count > 0)
					{
						int num2 = global::UnityEngine.Random.Range(0, list.Count);
						this.targetOverloadedWire = list[num2].gameObject;
					}
				}
				if (this.targetOverloadedWire != null)
				{
					this.targetOverloadedWire.Trigger(-794517298, new BuildingHP.DamageSourceInfo
					{
						damage = 1,
						source = BUILDINGS.DAMAGESOURCES.CIRCUIT_OVERLOADED,
						popString = UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CIRCUIT_OVERLOADED
					});
				}
				if (this.overloadedNotification == null)
				{
					this.timeOverloadNotificationDisplayed = 0f;
					string text = MISC.NOTIFICATIONS.CIRCUIT_OVERLOADED.NAME;
					NotificationType notificationType = NotificationType.BadMinor;
					HashedString invalid = HashedString.Invalid;
					Transform transform = this.targetOverloadedWire.transform;
					this.overloadedNotification = new Notification(text, notificationType, invalid, null, null, true, 0f, null, null, transform);
					Notifier notifier = Game.Instance.FindOrAdd<Notifier>();
					notifier.Add(this.overloadedNotification, string.Empty);
				}
			}
		}
		else
		{
			this.timeOverloaded = Mathf.Max(0f, this.timeOverloaded - dt * 0.95f);
			this.timeOverloadNotificationDisplayed += dt;
			if (this.timeOverloadNotificationDisplayed > 5f)
			{
				this.RemoveOverloadedNotification();
			}
		}
	}

	private void RemoveOverloadedNotification()
	{
		if (this.overloadedNotification != null)
		{
			Notifier notifier = Game.Instance.FindOrAdd<Notifier>();
			notifier.Remove(this.overloadedNotification);
			this.overloadedNotification = null;
		}
	}

	public float GetMaxSafeWattage()
	{
		for (int i = 0; i < this.wireGroups.Length; i++)
		{
			List<Wire> list = this.wireGroups[i];
			bool flag = list != null && list.Count > 0;
			if (flag)
			{
				Wire.WattageRating wattageRating = (Wire.WattageRating)i;
				return Wire.GetMaxWattageAsFloat(wattageRating);
			}
		}
		return 0f;
	}

	public override void RemoveItem(int cell, object item)
	{
		if (item.GetType() == typeof(Wire))
		{
			Wire wire = (Wire)item;
			wire.circuitOverloadTime = 0f;
		}
	}

	private Notification overloadedNotification;

	private List<Wire>[] wireGroups = new List<Wire>[5];

	private const float MIN_OVERLOAD_TIME_FOR_DAMAGE = 6f;

	private const float MIN_OVERLOAD_NOTIFICATION_DISPLAY_TIME = 5f;

	private GameObject targetOverloadedWire;

	private float timeOverloaded;

	private float timeOverloadNotificationDisplayed;
}
