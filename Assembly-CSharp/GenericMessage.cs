using System;
using KSerialization;
using UnityEngine;

public class GenericMessage : Message
{
	public GenericMessage(string _title, string _body, string _tooltip, KMonoBehaviour click_focus = null)
	{
		this.title = _title;
		this.body = _body;
		this.tooltip = _tooltip;
		this.clickFocus.Set(click_focus);
	}

	public GenericMessage()
	{
	}

	public override string GetSound()
	{
		return null;
	}

	public override string GetMessageBody()
	{
		return this.body;
	}

	public override string GetTooltip()
	{
		return this.tooltip;
	}

	public override string GetTitle()
	{
		return this.title;
	}

	public override void OnClick()
	{
		KMonoBehaviour kmonoBehaviour = this.clickFocus.Get();
		if (kmonoBehaviour == null)
		{
			return;
		}
		Transform transform = kmonoBehaviour.transform;
		if (transform == null)
		{
			return;
		}
		Vector3 position = transform.GetPosition();
		position.z = -40f;
		CameraController.Instance.SetTargetPos(position, 8f, true);
		if (transform.GetComponent<KSelectable>() != null)
		{
			SelectTool.Instance.Select(transform.GetComponent<KSelectable>(), false);
		}
	}

	[Serialize]
	private string title;

	[Serialize]
	private string tooltip;

	[Serialize]
	private string body;

	[Serialize]
	private Ref<KMonoBehaviour> clickFocus = new Ref<KMonoBehaviour>();
}
