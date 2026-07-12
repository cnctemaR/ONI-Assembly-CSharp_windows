using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CargoBay")]
public class CargoBayConduit : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (CargoBayConduit.connectedPortStatus == null)
		{
			CargoBayConduit.connectedPortStatus = new StatusItem("CONNECTED_ROCKET_PORT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID, true, 129022, null);
			CargoBayConduit.connectedWrongPortStatus = new StatusItem("CONNECTED_ROCKET_WRONG_PORT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, true, OverlayModes.None.ID, true, 129022, null);
			CargoBayConduit.connectedNoPortStatus = new StatusItem("CONNECTED_ROCKET_NO_PORT", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.Bad, true, OverlayModes.None.ID, true, 129022, null);
		}
		if (base.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad != null)
		{
			this.OnLaunchpadChainChanged(null);
			base.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.Subscribe(-1009905786, new Action<object>(this.OnLaunchpadChainChanged));
		}
		base.Subscribe<CargoBayConduit>(-1277991738, CargoBayConduit.OnLaunchDelegate);
		base.Subscribe<CargoBayConduit>(-887025858, CargoBayConduit.OnLandDelegate);
		this.storageType = base.GetComponent<CargoBay>().storageType;
		this.UpdateStatusItems();
	}

	protected override void OnCleanUp()
	{
		LaunchPad currentPad = base.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
		if (currentPad != null)
		{
			currentPad.Unsubscribe(-1009905786, new Action<object>(this.OnLaunchpadChainChanged));
		}
		base.OnCleanUp();
	}

	public void OnLaunch(object data)
	{
		ConduitDispenser component = base.GetComponent<ConduitDispenser>();
		if (component != null)
		{
			component.conduitType = ConduitType.None;
		}
		base.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.Unsubscribe(-1009905786, new Action<object>(this.OnLaunchpadChainChanged));
	}

	public void OnLand(object data)
	{
		ConduitDispenser component = base.GetComponent<ConduitDispenser>();
		if (component != null)
		{
			CargoBay.CargoType cargoType = this.storageType;
			if (cargoType != CargoBay.CargoType.Liquids)
			{
				if (cargoType == CargoBay.CargoType.Gasses)
				{
					component.conduitType = ConduitType.Gas;
				}
				else
				{
					component.conduitType = ConduitType.None;
				}
			}
			else
			{
				component.conduitType = ConduitType.Liquid;
			}
		}
		base.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.Subscribe(-1009905786, new Action<object>(this.OnLaunchpadChainChanged));
		this.UpdateStatusItems();
	}

	private void OnLaunchpadChainChanged(object data)
	{
		this.UpdateStatusItems();
	}

	private void UpdateStatusItems()
	{
		bool flag;
		bool flag2;
		this.HasMatchingConduitPort(out flag, out flag2);
		KSelectable component = base.GetComponent<KSelectable>();
		if (flag)
		{
			this.connectedConduitPortStatusItem = component.ReplaceStatusItem(this.connectedConduitPortStatusItem, CargoBayConduit.connectedPortStatus, this);
			return;
		}
		if (flag2)
		{
			this.connectedConduitPortStatusItem = component.ReplaceStatusItem(this.connectedConduitPortStatusItem, CargoBayConduit.connectedWrongPortStatus, this);
			return;
		}
		this.connectedConduitPortStatusItem = component.ReplaceStatusItem(this.connectedConduitPortStatusItem, CargoBayConduit.connectedNoPortStatus, this);
	}

	private void HasMatchingConduitPort(out bool hasMatch, out bool hasAny)
	{
		hasMatch = false;
		hasAny = false;
		LaunchPad currentPad = base.GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
		if (currentPad == null)
		{
			return;
		}
		ChainedBuilding.StatesInstance smi = currentPad.GetSMI<ChainedBuilding.StatesInstance>();
		if (smi == null)
		{
			return;
		}
		HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet pooledHashSet = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
		smi.GetLinkedBuildings(ref pooledHashSet);
		foreach (ChainedBuilding.StatesInstance statesInstance in pooledHashSet)
		{
			IConduitDispenser component = statesInstance.GetComponent<IConduitDispenser>();
			if (component != null)
			{
				hasAny = true;
				if (CargoBayConduit.ElementToCargoMap[component.ConduitType] == this.storageType)
				{
					hasMatch = true;
					break;
				}
			}
		}
		pooledHashSet.Recycle();
	}

	public static Dictionary<ConduitType, CargoBay.CargoType> ElementToCargoMap = new Dictionary<ConduitType, CargoBay.CargoType>
	{
		{
			ConduitType.Solid,
			CargoBay.CargoType.Solids
		},
		{
			ConduitType.Liquid,
			CargoBay.CargoType.Liquids
		},
		{
			ConduitType.Gas,
			CargoBay.CargoType.Gasses
		}
	};

	private static readonly EventSystem.IntraObjectHandler<CargoBayConduit> OnLaunchDelegate = new EventSystem.IntraObjectHandler<CargoBayConduit>(delegate(CargoBayConduit component, object data)
	{
		component.OnLaunch(data);
	});

	private static readonly EventSystem.IntraObjectHandler<CargoBayConduit> OnLandDelegate = new EventSystem.IntraObjectHandler<CargoBayConduit>(delegate(CargoBayConduit component, object data)
	{
		component.OnLand(data);
	});

	private static StatusItem connectedPortStatus;

	private static StatusItem connectedWrongPortStatus;

	private static StatusItem connectedNoPortStatus;

	private CargoBay.CargoType storageType;

	private Guid connectedConduitPortStatusItem;
}
