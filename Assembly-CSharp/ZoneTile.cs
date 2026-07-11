using System;
using ProcGen;

internal class ZoneTile : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num = Grid.PosToCell(this);
		for (int i = 0; i < this.width; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				int num2 = Grid.OffsetCell(num, i, j);
				SimMessages.ModifyCellWorldZone(num2, 0);
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		int num = Grid.PosToCell(this);
		for (int i = 0; i < this.width; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				int num2 = Grid.OffsetCell(num, i, j);
				SubWorld.ZoneType subWorldZoneType = global::World.Instance.zoneRenderData.GetSubWorldZoneType(num2);
				byte b = ((subWorldZoneType != SubWorld.ZoneType.Space) ? ((byte)subWorldZoneType) : byte.MaxValue);
				SimMessages.ModifyCellWorldZone(num2, b);
			}
		}
	}

	public int width = 1;

	public int height = 1;
}
