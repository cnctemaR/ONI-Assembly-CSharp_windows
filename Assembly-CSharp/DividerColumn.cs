using System;
using UnityEngine;

public class DividerColumn : TableColumn
{
	public DividerColumn(Func<bool> revealed = null, string scrollerID = "")
	{
		Action<MinionIdentity, GameObject> action = delegate(MinionIdentity minion, GameObject widget_go)
		{
			if (revealed != null)
			{
				if (revealed())
				{
					if (!widget_go.activeSelf)
					{
						widget_go.SetActive(true);
					}
				}
				else if (widget_go.activeSelf)
				{
					widget_go.SetActive(false);
				}
			}
			else
			{
				widget_go.SetActive(true);
			}
		};
		Comparison<MinionIdentity> comparison = null;
		Action<MinionIdentity, GameObject, ToolTip> action2 = null;
		Action<MinionIdentity, GameObject, ToolTip> action3 = null;
		Func<bool> revealed2 = revealed;
		base..ctor(action, comparison, action2, action3, revealed2, false, scrollerID);
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
