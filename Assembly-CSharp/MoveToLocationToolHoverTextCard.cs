using System;
using UnityEngine;

public class MoveToLocationToolHoverTextCard : HoverTextConfiguration
{
	public override void ConfigureHoverScreen()
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		if (!string.IsNullOrEmpty(this.ActionStringKey))
		{
			this.ActionName = Strings.Get(this.ActionStringKey);
		}
		instance.ClearLabels();
		instance.StartShadowBar(0f, 0f, false);
		if (this.printTitle)
		{
			this.ConfigureTitle(instance);
		}
		this.ConfigureInstructions(instance);
		instance.NewLine("Unreachable Line", 24);
		this.unreachableLine = instance.AddText("Unreachable", this.Styles_Title.Standard, true);
		instance.EndShadowBar();
	}

	public override void UpdateHoverElements(KSelectable[] selected)
	{
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		if (this.unreachableLine == null)
		{
			this.ConfigureHoverScreen();
		}
		bool flag = true;
		if (selected != null && selected.Length > 0 && selected[0] != null)
		{
			Navigator component = selected[0].GetComponent<Navigator>();
			if (component != null && component.CanReach(num))
			{
				flag = false;
			}
		}
		base.SetLineActive(this.unreachableLine.gameObject, flag);
	}

	private LocText unreachableLine;
}
