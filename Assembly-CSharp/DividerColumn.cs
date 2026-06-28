using System;
using UnityEngine;

public class DividerColumn : TableColumn
{
	public DividerColumn()
		: base(null, null, null, null)
	{
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		return Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Spacer, parent, true);
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		return Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Spacer, parent, true);
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		return Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Spacer, parent, true);
	}
}
