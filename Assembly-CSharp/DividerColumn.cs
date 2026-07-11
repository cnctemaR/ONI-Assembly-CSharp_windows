using System;
using UnityEngine;

public class DividerColumn : TableColumn
{
	public DividerColumn(Func<bool> revealed = null, string scrollerID = "")
		: base(delegate(IAssignableIdentity minion, GameObject widget_go)
		{
			if (revealed != null)
			{
				if (revealed())
				{
					if (!widget_go.activeSelf)
					{
						widget_go.SetActive(true);
						return;
					}
				}
				else if (widget_go.activeSelf)
				{
					widget_go.SetActive(false);
					return;
				}
			}
			else
			{
				widget_go.SetActive(true);
			}
		}, null, null, null, revealed, false, scrollerID)
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
