using System;
using UnityEngine;

public class PortraitTableColumn : TableColumn
{
	public PortraitTableColumn(Action<MinionIdentity, GameObject> on_load_action, Comparison<MinionIdentity> sort_comparison)
		: base(on_load_action, sort_comparison, null, null, null, 0f)
	{
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(this.prefab_portrait, parent, true);
		gameObject.GetComponent<CrewPortrait>().targetImage.enabled = true;
		return gameObject;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		return Util.KInstantiateUI(this.prefab_portrait, parent, true);
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(this.prefab_portrait, parent, true);
		gameObject.GetComponent<KButton>().onClick += delegate
		{
			parent.GetComponent<TableRow>().SelectMinion();
		};
		gameObject.GetComponent<KButton>().onDoubleClick += delegate
		{
			parent.GetComponent<TableRow>().SelectAndFocusMinion();
		};
		return gameObject;
	}

	public GameObject prefab_portrait = Assets.UIPrefabs.TableScreenWidgets.MinionPortrait;
}
