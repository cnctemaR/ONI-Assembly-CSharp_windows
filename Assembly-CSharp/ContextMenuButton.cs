using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContextMenuButton : KScreen, IInputHandler, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public void SetCommand(ContextMenuScreen menu_screen, PriorityCommand command)
	{
		this.command = command;
		this.menuScreen = menu_screen;
		string text = command.ToString();
		this.SetLabel(text);
		base.GetComponentInChildren<Button>().interactable = command.CanBegin();
	}

	public void SetOnClick(global::System.Action onClick)
	{
		this.button.ClearOnClick();
		this.button.onClick += onClick;
	}

	public void ExecuteTask()
	{
		if (this.command != null)
		{
			this.menuScreen.OnClick(this.command);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.mouseOver && e.TryConsume(global::Action.MouseRight))
		{
			this.menuScreen.Clear();
		}
	}

	public void SetMenuScreen(ContextMenuScreen screen)
	{
		this.menuScreen = screen;
	}

	public void MoveAction()
	{
		this.menuScreen.OrderMove();
	}

	public void SetLabel(string labelText)
	{
		if (this.label == null)
		{
			this.label = this.button.GetComponentInChildren<LocText>();
		}
		this.label.text = labelText;
	}

	private LocText label;

	[SerializeField]
	private KButton button;

	private ContextMenuScreen menuScreen;

	private PriorityCommand command;

	private global::System.Action onClick;
}
