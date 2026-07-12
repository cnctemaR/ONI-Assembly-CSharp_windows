using System;
using System.Collections.Generic;
using UnityEngine;

public class RocketLaunchConditionVisualizer : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (DlcManager.FeatureClusterSpaceEnabled())
		{
			this.clusterModule = base.GetComponent<RocketModuleCluster>();
		}
		else
		{
			this.launchConditionManager = base.GetComponent<LaunchConditionManager>();
		}
		this.UpdateAllModuleData();
		base.Subscribe(1512695988, new Action<object>(this.OnAnyRocketModuleChanged));
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(1512695988, new Action<object>(this.OnAnyRocketModuleChanged));
	}

	private void OnAnyRocketModuleChanged(object obj)
	{
		this.UpdateAllModuleData();
	}

	private void UpdateAllModuleData()
	{
		if (this.moduleVisualizeData != null)
		{
			this.moduleVisualizeData = null;
		}
		bool flag = this.clusterModule != null;
		List<Ref<RocketModuleCluster>> list = null;
		List<RocketModule> list2 = null;
		if (flag)
		{
			list = new List<Ref<RocketModuleCluster>>(this.clusterModule.CraftInterface.ClusterModules);
			this.moduleVisualizeData = new RocketLaunchConditionVisualizer.RocketModuleVisualizeData[list.Count];
			list.Sort(delegate(Ref<RocketModuleCluster> a, Ref<RocketModuleCluster> b)
			{
				int y = Grid.PosToXY(a.Get().transform.GetPosition()).y;
				int y2 = Grid.PosToXY(b.Get().transform.GetPosition()).y;
				return y.CompareTo(y2);
			});
		}
		else
		{
			list2 = new List<RocketModule>(this.launchConditionManager.rocketModules);
			list2.Sort(delegate(RocketModule a, RocketModule b)
			{
				int y3 = Grid.PosToXY(a.transform.GetPosition()).y;
				int y4 = Grid.PosToXY(b.transform.GetPosition()).y;
				return y3.CompareTo(y4);
			});
			this.moduleVisualizeData = new RocketLaunchConditionVisualizer.RocketModuleVisualizeData[list2.Count];
		}
		for (int i = 0; i < this.moduleVisualizeData.Length; i++)
		{
			RocketModule rocketModule = (flag ? list[i].Get() : list2[i]);
			Building component = rocketModule.GetComponent<Building>();
			this.moduleVisualizeData[i] = new RocketLaunchConditionVisualizer.RocketModuleVisualizeData
			{
				Module = rocketModule,
				RangeMax = Mathf.FloorToInt((float)component.Def.WidthInCells / 2f),
				RangeMin = -Mathf.FloorToInt((float)(component.Def.WidthInCells - 1) / 2f)
			};
		}
	}

	public RocketLaunchConditionVisualizer.RocketModuleVisualizeData[] moduleVisualizeData;

	private LaunchConditionManager launchConditionManager;

	private RocketModuleCluster clusterModule;

	public struct RocketModuleVisualizeData
	{
		public RocketModule Module;

		public Vector2I OriginOffset;

		public int RangeMin;

		public int RangeMax;
	}
}
