using System;
using UnityEngine;

public class GlassForge : ComplexFabricator
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<GlassForge>(-2094018600, GlassForge.CheckPipesDelegate);
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

	private static readonly EventSystem.IntraObjectHandler<GlassForge> CheckPipesDelegate = new EventSystem.IntraObjectHandler<GlassForge>(delegate(GlassForge component, object data)
	{
		component.CheckPipes(data);
	});
}
