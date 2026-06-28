using System;
using System.Collections.Generic;
using Klei;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class ElementChunk : SimTemperatureTransfer, ISaveLoadableJson
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-2064133523, new EventSystem.EventHandler(this.OnAbsorb));
		this.Subscribe(-1697596308, new EventSystem.EventHandler(this.OnStorageChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Vector3 position = this.transform.position;
		position.z = Grid.GetLayerZ(Grid.SceneLayer.Use);
		this.transform.SetPosition(position);
		PrimaryElement component = base.GetComponent<PrimaryElement>();
		Element element = component.Element;
		if (this.ShowElementStatus)
		{
			KSelectable component2 = base.GetComponent<KSelectable>();
			Func<Element> func = () => element;
			component2.AddStatusItem(Db.Get().MiscStatusItems.ElementalCategory, func);
			component2.AddStatusItem(Db.Get().MiscStatusItems.OreMass, this);
			component2.AddStatusItem(Db.Get().MiscStatusItems.OreTemp, this);
		}
	}

	private static string OnResourceMeltedTooltip(List<Notification> notifications, object data)
	{
		return "Resources melted:" + notifications.ReduceMessages(true);
	}

	private void OnAbsorb(object data)
	{
		GameObject gameObject = data as GameObject;
		if (gameObject != null)
		{
			PrimaryElement component = base.GetComponent<PrimaryElement>();
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			if (component2 != null)
			{
				if (component.Mass > 0f)
				{
					float num = SimUtil.CalculateFinalTemperature(component.Mass, component.Temperature, component2.Mass, component2.Temperature);
					component.Temperature = num;
				}
				else
				{
					component.Temperature = component2.Temperature;
				}
				Debug.Assert(component.Temperature > 0f || component.Mass == 0f, "OnAbsorb resulted in a temperature of 0", base.gameObject);
				if (CameraController.Instance != null)
				{
					string sound = GlobalAssets.GetSound("Ore_absorb", false);
					if (sound != null && CameraController.Instance.IsAudibleSound(gameObject.transform.position, sound))
					{
						base.PlaySound3D(sound);
					}
				}
			}
		}
	}

	private void OnStorageChanged(object data)
	{
		base.enabled = base.GetComponent<Pickupable>().storage == null;
	}

	public void AddMeltedNotification()
	{
		Notifier notifier = base.gameObject.AddComponent<Notifier>();
		Func<List<Notification>, object, string> func = new Func<List<Notification>, object, string>(ElementChunk.OnResourceMeltedTooltip);
		this.resourceMelted = new Notification(MISC.NOTIFICATIONS.RESOURCEMELTED.NAME, NotificationType.BadMinor, null, func, null, true, 0f, null, null, null);
		notifier.Add(this.resourceMelted, string.Empty);
	}

	public bool ShowElementStatus = true;

	private Notification resourceMelted;
}
