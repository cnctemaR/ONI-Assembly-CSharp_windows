using System;

public class WireBuildTool : BaseUtilityBuildTool
{
	protected override void OnPrefabInit()
	{
		WireBuildTool.Instance = this;
		base.OnPrefabInit();
		this.viewMode = SimViewMode.PowerMap;
	}

	protected override void ApplyPathToConduitSystem()
	{
		if (this.path.Count >= 2)
		{
			for (int i = 1; i < this.path.Count; i++)
			{
				int cell = this.path[i - 1].cell;
				int cell2 = this.path[i].cell;
				UtilityConnections direction = base.GetDirection(cell, this.path[i].cell);
				UtilityConnections oppositeDirection = base.GetOppositeDirection(direction);
				this.conduitMgr.AddConnection(direction, cell, false);
				this.conduitMgr.AddConnection(oppositeDirection, cell2, false);
			}
		}
	}

	public static WireBuildTool Instance;
}
