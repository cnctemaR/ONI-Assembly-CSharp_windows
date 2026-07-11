using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class CargoBay : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		base.Subscribe(-1056989049, new Action<object>(this.OnLaunch));
		base.Subscribe(238242047, new Action<object>(this.OnLand));
		base.Subscribe(493375141, new Action<object>(this.OnRefreshUserMenu));
	}

	private void OnRefreshUserMenu(object data)
	{
		string text = "action_empty_contents";
		string text2 = UI.USERMENUACTIONS.EMPTYSTORAGE.NAME;
		global::System.Action action = delegate
		{
			this.storage.DropAll(false);
		};
		string text3 = UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP;
		KIconButtonMenu.ButtonInfo buttonInfo = new KIconButtonMenu.ButtonInfo(text, text2, action, global::Action.NumActions, null, null, null, text3, true);
		Game.Instance.userMenu.AddButton(base.gameObject, buttonInfo, 1f);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public void SpawnResources(object data)
	{
		SpaceDestination destination = SpacecraftManager.instance.GetDestination(SpacecraftManager.instance.savedSpacecraftDestinations[SpacecraftManager.instance.GetSpacecraftID(base.GetComponent<RocketModule>().conditionManager.GetComponent<LaunchableRocket>())]);
		int num = Grid.PosToCell(base.gameObject);
		foreach (KeyValuePair<SimHashes, float> keyValuePair in destination.GetMissionResourceResult(this.storage.RemainingCapacity(), this.storageType == CargoBay.CargoType.solids, this.storageType == CargoBay.CargoType.liquids, this.storageType == CargoBay.CargoType.gasses))
		{
			Element element = ElementLoader.FindElementByHash(keyValuePair.Key);
			if (this.storageType == CargoBay.CargoType.solids && element.IsSolid)
			{
				GameObject gameObject = Scenario.SpawnPrefab(num, 0, 0, element.tag.Name, Grid.SceneLayer.Ore);
				gameObject.GetComponent<PrimaryElement>().Mass = keyValuePair.Value;
				gameObject.GetComponent<PrimaryElement>().Temperature = ElementLoader.FindElementByHash(keyValuePair.Key).defaultValues.temperature;
				gameObject.SetActive(true);
				this.storage.Store(gameObject, false, false, true, false);
			}
			else if (this.storageType == CargoBay.CargoType.liquids && element.IsLiquid)
			{
				this.storage.AddLiquid(keyValuePair.Key, keyValuePair.Value, ElementLoader.FindElementByHash(keyValuePair.Key).defaultValues.temperature, byte.MaxValue, 0, false, true);
			}
			else if (this.storageType == CargoBay.CargoType.gasses && element.IsGas)
			{
				this.storage.AddGasChunk(keyValuePair.Key, keyValuePair.Value, ElementLoader.FindElementByHash(keyValuePair.Key).defaultValues.temperature, byte.MaxValue, 0, false, true);
			}
		}
		if (this.storageType == CargoBay.CargoType.entities)
		{
			foreach (KeyValuePair<Tag, int> keyValuePair2 in destination.GetMissionEntityResult())
			{
				GameObject prefab = Assets.GetPrefab(keyValuePair2.Key);
				if (prefab == null)
				{
					KCrashReporter.Assert(false, "Missing prefab: " + keyValuePair2.Key.Name);
				}
				else
				{
					for (int i = 0; i < keyValuePair2.Value; i++)
					{
						GameObject gameObject2 = Util.KInstantiate(prefab, base.transform.position);
						gameObject2.SetActive(true);
						this.storage.Store(gameObject2, false, false, true, false);
						Baggable component = gameObject2.GetComponent<Baggable>();
						if (component != null)
						{
							component.SetWrangled();
						}
					}
				}
			}
		}
	}

	public void OnLaunch(object data)
	{
		ConduitDispenser component = base.GetComponent<ConduitDispenser>();
		if (component != null)
		{
			component.conduitType = ConduitType.None;
		}
	}

	public void OnLand(object data)
	{
		this.SpawnResources(data);
		ConduitDispenser component = base.GetComponent<ConduitDispenser>();
		if (component != null)
		{
			CargoBay.CargoType cargoType = this.storageType;
			if (cargoType != CargoBay.CargoType.gasses)
			{
				if (cargoType != CargoBay.CargoType.liquids)
				{
					component.conduitType = ConduitType.None;
				}
				else
				{
					component.conduitType = ConduitType.Liquid;
				}
			}
			else
			{
				component.conduitType = ConduitType.Gas;
			}
		}
	}

	public Storage storage;

	public CargoBay.CargoType storageType;

	public enum CargoType
	{
		solids,
		liquids,
		gasses,
		entities
	}
}
