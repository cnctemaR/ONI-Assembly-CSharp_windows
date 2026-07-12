using System;

public class ScannerModule : GameStateMachine<ScannerModule, ScannerModule.Instance, IStateMachineTarget, ScannerModule.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Enter(delegate(ScannerModule.Instance smi)
		{
			smi.SetFogOfWarAllowed();
		}).EventHandler(GameHashes.RocketLaunched, delegate(ScannerModule.Instance smi)
		{
			smi.Scan();
		}).EventHandler(GameHashes.ClusterLocationChanged, (ScannerModule.Instance smi) => smi.GetComponent<RocketModuleCluster>().CraftInterface, delegate(ScannerModule.Instance smi)
		{
			smi.Scan();
		})
			.EventHandler(GameHashes.RocketModuleChanged, (ScannerModule.Instance smi) => smi.GetComponent<RocketModuleCluster>().CraftInterface, delegate(ScannerModule.Instance smi)
			{
				smi.SetFogOfWarAllowed();
			})
			.Exit(delegate(ScannerModule.Instance smi)
			{
				smi.SetFogOfWarAllowed();
			});
	}

	public class Def : StateMachine.BaseDef
	{
		public int scanRadius = 1;
	}

	public new class Instance : GameStateMachine<ScannerModule, ScannerModule.Instance, IStateMachineTarget, ScannerModule.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ScannerModule.Def def)
			: base(master, def)
		{
		}

		public void Scan()
		{
			Clustercraft component = base.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
			if (component.Status == Clustercraft.CraftStatus.InFlight)
			{
				ClusterFogOfWarManager.Instance smi = SaveGame.Instance.GetSMI<ClusterFogOfWarManager.Instance>();
				AxialI location = component.Location;
				smi.RevealLocation(location, base.def.scanRadius);
				foreach (ClusterGridEntity clusterGridEntity in ClusterGrid.Instance.GetNotVisibleEntitiesAtAdjacentCell(location))
				{
					smi.RevealLocation(clusterGridEntity.Location, 0);
				}
			}
		}

		public void SetFogOfWarAllowed()
		{
			CraftModuleInterface craftInterface = base.GetComponent<RocketModuleCluster>().CraftInterface;
			if (craftInterface.HasClusterDestinationSelector())
			{
				bool flag = false;
				ClusterDestinationSelector clusterDestinationSelector = craftInterface.GetClusterDestinationSelector();
				bool canNavigateFogOfWar = clusterDestinationSelector.canNavigateFogOfWar;
				foreach (Ref<RocketModuleCluster> @ref in craftInterface.ClusterModules)
				{
					RocketModuleCluster rocketModuleCluster = @ref.Get();
					if (((rocketModuleCluster != null) ? rocketModuleCluster.GetSMI<ScannerModule.Instance>() : null) != null)
					{
						flag = true;
						break;
					}
				}
				clusterDestinationSelector.canNavigateFogOfWar = flag;
				if (canNavigateFogOfWar && !flag)
				{
					ClusterTraveler component = craftInterface.GetComponent<ClusterTraveler>();
					if (component != null)
					{
						component.RevalidatePath(true);
					}
				}
				craftInterface.GetComponent<Clustercraft>().Trigger(-688990705, null);
			}
		}
	}
}
