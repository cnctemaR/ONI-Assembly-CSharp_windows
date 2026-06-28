using System;
using UnityEngine;

public class MessageDialogFrame : KScreen
{
	public override float GetSortKey()
	{
		return 9999f;
	}

	protected override void OnActivate()
	{
		this.closeButton.onClick += this.OnClickClose;
		this.nextMessageButton.onClick += this.OnClickNextMessage;
		base.Subscribe(Messenger.Instance.gameObject, -599791736, new Action<object>(this.OnMessagesChanged));
		this.OnMessagesChanged(null);
	}

	protected override void OnDeactivate()
	{
		base.Unsubscribe(Messenger.Instance.gameObject, -599791736, new Action<object>(this.OnMessagesChanged));
	}

	private void OnClickClose()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnClickNextMessage()
	{
		global::UnityEngine.Object.Destroy(base.gameObject);
		NotificationScreen.Instance.OnClickNextMessage();
	}

	private void OnMessagesChanged(object data)
	{
		this.nextMessageButton.gameObject.SetActive(Messenger.Instance.Count != 0);
	}

	public void SetMessage(MessageDialog dialog, Message message)
	{
		this.title.text = message.GetTitle().ToUpper();
		dialog.GetComponent<RectTransform>().SetParent(this.body.GetComponent<RectTransform>());
		RectTransform component = dialog.GetComponent<RectTransform>();
		component.offsetMin = Vector2.zero;
		component.offsetMax = Vector2.zero;
		dialog.transform.localPosition = Vector3.zero;
		dialog.SetMessage(message);
		dialog.OnClickAction();
	}

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KToggle nextMessageButton;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private RectTransform body;
}
