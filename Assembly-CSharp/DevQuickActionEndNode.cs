using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class DevQuickActionEndNode : DevQuickActionNode
{
	protected void Awake()
	{
		this.button = base.GetComponent<Button>();
		this.button.onClick.AddListener(new UnityAction(this.ButtonClicked));
	}

	private void ButtonClicked()
	{
		global::System.Action onNodeInteractedWith = this.OnNodeInteractedWith;
		if (onNodeInteractedWith == null)
		{
			return;
		}
		onNodeInteractedWith();
	}

	public void Setup(string name, DevQuickActionNode parentNode, global::System.Action clickCB)
	{
		this.label.SetText(name);
		this.parentNode = parentNode;
		this.OnNodeInteractedWith = clickCB;
	}

	private Button button;
}
