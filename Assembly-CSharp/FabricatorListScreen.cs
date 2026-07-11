using System;
using System.Collections.Generic;

public class FabricatorListScreen : KToggleMenu
{
	private void Refresh()
	{
		List<KToggleMenu.ToggleInfo> list = new List<KToggleMenu.ToggleInfo>();
		foreach (Fabricator fabricator in Components.Fabricators.Items)
		{
			KSelectable component = fabricator.GetComponent<KSelectable>();
			list.Add(new KToggleMenu.ToggleInfo(component.GetName(), fabricator, global::Action.NumActions));
		}
		base.Setup(list);
	}

	protected override void OnSpawn()
	{
		base.onSelect += this.OnClickFabricator;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.Refresh();
	}

	private void OnClickFabricator(KToggleMenu.ToggleInfo toggle_info)
	{
		Fabricator fabricator = (Fabricator)toggle_info.userData;
		SelectTool.Instance.Select(fabricator.GetComponent<KSelectable>(), false);
	}
}
