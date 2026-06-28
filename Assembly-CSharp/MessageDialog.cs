using System;

public abstract class MessageDialog : KMonoBehaviour
{
	public abstract bool CanDisplay(Message message);

	public abstract void SetMessage(Message message);

	public abstract void OnClickAction();
}
