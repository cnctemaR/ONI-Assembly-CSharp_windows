using System;
using UnityEngine;

public class GlassForge : Refinery
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-2094018600, new Action<object>(this.CheckPipes));
	}

	private void CheckPipes(object data)
	{
		KSelectable component = base.GetComponent<KSelectable>();
		int num = Grid.OffsetCell(Grid.PosToCell(this), GlassForgeConfig.outPipeOffset);
		GameObject gameObject = Grid.Objects[num, 16];
		if (gameObject != null)
		{
			PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
			if (component2.Element.highTemp > ElementLoader.FindElementByHash(SimHashes.MoltenGlass).lowTemp)
			{
				component.RemoveStatusItem(this.statusHandle, false);
			}
			else
			{
				this.statusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.PipeMayMelt, null);
			}
		}
		else
		{
			component.RemoveStatusItem(this.statusHandle, false);
		}
	}

	private Guid statusHandle;
}
