using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class NotificationScreen : KScreen
{
	public static NotificationScreen Instance { get; private set; }

	private void OnAddNotifier(Notifier notifier)
	{
		notifier.OnAdd = (Action<Notification>)Delegate.Combine(notifier.OnAdd, new Action<Notification>(this.OnAddNotification));
		notifier.OnRemove = (Action<Notification>)Delegate.Combine(notifier.OnRemove, new Action<Notification>(this.OnRemoveNotification));
	}

	private void OnRemoveNotifier(Notifier notifier)
	{
		notifier.OnAdd = (Action<Notification>)Delegate.Remove(notifier.OnAdd, new Action<Notification>(this.OnAddNotification));
		notifier.OnRemove = (Action<Notification>)Delegate.Remove(notifier.OnRemove, new Action<Notification>(this.OnRemoveNotification));
	}

	private void OnAddNotification(Notification notification)
	{
		this.notificationTracker.OnRecieveNotification(notification);
		this.pendingNotifications.Add(notification);
	}

	private void OnRemoveNotification(Notification notification)
	{
		this.dirty = true;
		this.pendingNotifications.Remove(notification);
		NotificationScreen.Entry entry = null;
		this.entriesByMessage.TryGetValue(notification.titleText, out entry);
		if (entry == null)
		{
			return;
		}
		this.notifications.Remove(notification);
		entry.Remove(notification);
		if (entry.notifications.Count == 0)
		{
			global::UnityEngine.Object.Destroy(entry.label);
			this.entriesByMessage[notification.titleText] = null;
			this.entries.Remove(entry);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		NotificationScreen.Instance = this;
		Components.Cmps<Notifier> notifiers = Components.Notifiers;
		notifiers.OnAdd = (Action<Notifier>)Delegate.Combine(notifiers.OnAdd, new Action<Notifier>(this.OnAddNotifier));
		Components.Cmps<Notifier> notifiers2 = Components.Notifiers;
		notifiers2.OnRemove = (Action<Notifier>)Delegate.Combine(notifiers2.OnRemove, new Action<Notifier>(this.OnRemoveNotifier));
		foreach (Notifier notifier in Components.Notifiers)
		{
			this.OnAddNotifier(notifier);
		}
		this.MessagesPrefab.gameObject.SetActive(false);
		this.LabelPrefab.gameObject.SetActive(false);
		this.InitNotificationSounds();
	}

	private void OnNewMessage(object data)
	{
		Message message = (Message)data;
		this.notifier.Add(new MessageNotification(message), string.Empty);
	}

	private void ShowMessage(MessageNotification mn)
	{
		if (mn.message.OnClick != null)
		{
			mn.message.OnClick();
		}
		else
		{
			for (int i = 0; i < this.dialogPrefabs.Count; i++)
			{
				if (this.dialogPrefabs[i].CanDisplay(mn.message))
				{
					if (this.messageDialog != null)
					{
						global::UnityEngine.Object.Destroy(this.messageDialog.gameObject);
						this.messageDialog = null;
					}
					this.messageDialog = global::Util.KInstantiateUI<MessageDialogFrame>(ScreenPrefabs.Instance.MessageDialogFrame.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
					MessageDialog messageDialog = global::Util.KInstantiateUI<MessageDialog>(this.dialogPrefabs[i].gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, false);
					this.messageDialog.SetMessage(messageDialog, mn.message);
					this.messageDialog.Show(true);
					break;
				}
			}
		}
		Messenger.Instance.RemoveMessage(mn.message);
		mn.Clear();
	}

	public void OnClickNextMessage()
	{
		Notification notification2 = this.notifications.Find((Notification notification) => notification.Type == NotificationType.Messages);
		this.ShowMessage((MessageNotification)notification2);
	}

	protected override void OnCleanUp()
	{
		Components.Cmps<Notifier> notifiers = Components.Notifiers;
		notifiers.OnAdd = (Action<Notifier>)Delegate.Remove(notifiers.OnAdd, new Action<Notifier>(this.OnAddNotifier));
		Components.Cmps<Notifier> notifiers2 = Components.Notifiers;
		notifiers2.OnRemove = (Action<Notifier>)Delegate.Remove(notifiers2.OnRemove, new Action<Notifier>(this.OnRemoveNotifier));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.initTime = KTime.Instance.UnscaledGameTime;
		LocText[] array = this.LabelPrefab.GetComponentsInChildren<LocText>();
		foreach (LocText locText in array)
		{
			locText.color = this.normalColor;
		}
		array = this.MessagesPrefab.GetComponentsInChildren<LocText>();
		foreach (LocText locText2 in array)
		{
			locText2.color = this.normalColor;
		}
		base.Subscribe(Messenger.Instance.gameObject, 1558809273, new Action<object>(this.OnNewMessage));
		foreach (Message message in Messenger.Instance.Messages)
		{
			Notification notification = new MessageNotification(message);
			notification.playSound = false;
			this.notifier.Add(notification, string.Empty);
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.dirty = true;
	}

	private void AddNotification(Notification notification)
	{
		this.notifications.Add(notification);
		notification.Idx = this.notificationIncrement++;
		NotificationScreen.Entry entry = null;
		this.entriesByMessage.TryGetValue(notification.titleText, out entry);
		if (entry == null)
		{
			GameObject label;
			if (notification.Type == NotificationType.Messages)
			{
				label = global::Util.KInstantiateUI(this.MessagesPrefab, this.MessagesFolder, false);
			}
			else
			{
				label = global::Util.KInstantiateUI(this.LabelPrefab, this.LabelsFolder, false);
			}
			label.GetComponentInChildren<NotificationAnimator>().Init();
			label.gameObject.SetActive(true);
			KImage componentInChildren = label.GetComponentInChildren<KImage>(true);
			Button[] componentsInChildren = label.gameObject.GetComponentsInChildren<Button>();
			ColorBlock colors = componentsInChildren[0].colors;
			if (notification.Type == NotificationType.Bad)
			{
				colors.normalColor = this.badColorBG;
			}
			else if (notification.Type == NotificationType.Messages)
			{
				colors.normalColor = this.messageColorBG;
				Debug.Assert(notification.GetType() == typeof(MessageNotification), string.Format("Notification: \"{0}\" is not of type MessageNotification", notification.titleText));
				componentsInChildren[1].onClick.AddListener(delegate
				{
					List<Notification> list = this.notifications.FindAll((Notification n) => n.titleText == notification.titleText);
					foreach (Notification notification2 in list)
					{
						MessageNotification messageNotification = (MessageNotification)notification2;
						Messenger.Instance.RemoveMessage(messageNotification.message);
						messageNotification.Clear();
					}
				});
			}
			else if (notification.Type == NotificationType.Tutorial)
			{
				colors.normalColor = this.warningColorBG;
			}
			else
			{
				colors.normalColor = this.normalColorBG;
			}
			componentsInChildren[0].colors = colors;
			componentsInChildren[0].onClick.AddListener(delegate
			{
				this.OnClick(entry);
			});
			if (notification.ToolTip != null)
			{
				label.GetComponentInChildren<ToolTip>().OnToolTip = delegate
				{
					ToolTip componentInChildren2 = label.GetComponentInChildren<ToolTip>();
					componentInChildren2.ClearMultiStringTooltip();
					componentInChildren2.AddMultiStringTooltip(notification.ToolTip(entry.notifications, notification.tooltipData), this.TooltipTextStyle);
					return string.Empty;
				};
			}
			entry = new NotificationScreen.Entry(label);
			this.entriesByMessage[notification.titleText] = entry;
			this.entries.Add(entry);
			LocText[] componentsInChildren2 = label.GetComponentsInChildren<LocText>();
			LocText[] array = componentsInChildren2;
			int i = 0;
			while (i < array.Length)
			{
				LocText locText = array[i];
				switch (notification.Type)
				{
				case NotificationType.Bad:
					locText.color = this.badColor;
					componentInChildren.sprite = this.icon_bad;
					break;
				case NotificationType.Good:
				case NotificationType.BadMinor:
				case NotificationType.Neutral:
					goto IL_0330;
				case NotificationType.Tutorial:
					locText.color = this.warningColor;
					componentInChildren.sprite = this.icon_warning;
					break;
				case NotificationType.Messages:
					locText.color = this.messageColor;
					componentInChildren.sprite = this.icon_message;
					break;
				default:
					goto IL_0330;
				}
				IL_034E:
				componentInChildren.color = locText.color;
				string text = string.Empty;
				if (KTime.Instance.UnscaledGameTime - this.initTime > 5f && notification.playSound)
				{
					this.PlayDingSound(notification);
				}
				else
				{
					text = "too early";
				}
				if (AudioDebug.Get().debugNotificationSounds)
				{
					Debug.Log("Notification(" + notification.titleText + "):" + text);
				}
				i++;
				continue;
				IL_0330:
				locText.color = this.normalColor;
				componentInChildren.sprite = this.icon_normal;
				goto IL_034E;
			}
		}
		entry.Add(notification);
		entry.UpdateMessage(notification.titleText);
		this.dirty = true;
		this.SortNotifications();
	}

	private void SortNotifications()
	{
		this.notifications.Sort(delegate(Notification n1, Notification n2)
		{
			if (n1.Type == n2.Type)
			{
				if (n1.Idx < n2.Idx)
				{
					return -1;
				}
				if (n1.Idx > n2.Idx)
				{
					return 1;
				}
				return 0;
			}
			else
			{
				if (n1.Type < n2.Type)
				{
					return -1;
				}
				return 1;
			}
		});
		foreach (Notification notification in this.notifications)
		{
			NotificationScreen.Entry entry = null;
			this.entriesByMessage.TryGetValue(notification.titleText, out entry);
			if (entry != null)
			{
				entry.label.GetComponent<RectTransform>().SetAsLastSibling();
			}
		}
	}

	private void PlayDingSound(Notification notification)
	{
		string text;
		if (!this.notificationSounds.TryGetValue(notification.Type, out text))
		{
			text = "Notification";
		}
		float num;
		if (!this.timeOfLastNotification.TryGetValue(text, out num))
		{
			num = 0f;
		}
		float num2 = (Time.time - num) / this.soundDecayTime;
		EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound(this.notificationSounds[notification.Type], false), Vector3.zero);
		eventInstance.setParameterValue("timeSinceLast", num2);
		KFMOD.EndOneShot(eventInstance);
		this.timeOfLastNotification[text] = Time.time;
	}

	private void Update()
	{
		int i = 0;
		while (i < this.pendingNotifications.Count)
		{
			if (this.pendingNotifications[i].IsReady())
			{
				this.AddNotification(this.pendingNotifications[i]);
				this.pendingNotifications.RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
		int num = 0;
		int num2 = 0;
		for (int j = 0; j < this.notifications.Count; j++)
		{
			Notification notification = this.notifications[j];
			if (notification.Type == NotificationType.Messages)
			{
				num2++;
			}
			else
			{
				num++;
			}
			if (notification.Notifier != null)
			{
				notification.Position = notification.Notifier.transform.position;
			}
			if (notification.expires && KTime.Instance.UnscaledGameTime - notification.Time > this.lifetime)
			{
				this.dirty = true;
				if (notification.Notifier == null)
				{
					this.OnRemoveNotification(notification);
				}
				else
				{
					notification.Clear();
				}
			}
		}
	}

	private void OnClick(NotificationScreen.Entry entry)
	{
		Notification nextClickedNotification = entry.NextClickedNotification;
		Notifier notifier = nextClickedNotification.Notifier;
		base.PlaySound3D(GlobalAssets.GetSound("HUD_Click_Open", false));
		if (nextClickedNotification.customClickCallback != null)
		{
			nextClickedNotification.customClickCallback(nextClickedNotification.customClickData);
		}
		else
		{
			if (notifier != null)
			{
				SelectTool.Instance.Select(notifier.GetComponent<KSelectable>(), false);
			}
			if (nextClickedNotification.Type == NotificationType.Messages)
			{
				this.ShowMessage((MessageNotification)nextClickedNotification);
			}
			if (nextClickedNotification.hasLocation)
			{
				Vector3 position = nextClickedNotification.Position;
				position.z = -40f;
				CameraController.Instance.SetTargetPos(position, 8f, true);
			}
		}
	}

	private void PositionLocatorIcon()
	{
	}

	private void InitNotificationSounds()
	{
		this.notificationSounds[NotificationType.Good] = "Notification";
		this.notificationSounds[NotificationType.BadMinor] = "Notification";
		this.notificationSounds[NotificationType.Bad] = "Warning";
		this.notificationSounds[NotificationType.Neutral] = "Notification";
		this.notificationSounds[NotificationType.Tutorial] = "Notification";
		this.notificationSounds[NotificationType.Messages] = "Message";
	}

	public float lifetime;

	public bool dirty;

	public GameObject LabelPrefab;

	public GameObject LabelsFolder;

	public GameObject MessagesPrefab;

	public GameObject MessagesFolder;

	private MessageDialogFrame messageDialog;

	private float initTime;

	private int notificationIncrement;

	[MyCmpAdd]
	private Notifier notifier;

	[SerializeField]
	private List<MessageDialog> dialogPrefabs = new List<MessageDialog>();

	[SerializeField]
	private Color badColorBG;

	[SerializeField]
	private Color badColor = Color.red;

	[SerializeField]
	private Color normalColorBG;

	[SerializeField]
	private Color normalColor = Color.white;

	[SerializeField]
	private Color warningColorBG;

	[SerializeField]
	private Color warningColor;

	[SerializeField]
	private Color messageColorBG;

	[SerializeField]
	private Color messageColor;

	public Sprite icon_normal;

	public Sprite icon_warning;

	public Sprite icon_bad;

	public Sprite icon_message;

	private List<Notification> pendingNotifications = new List<Notification>();

	private List<Notification> notifications = new List<Notification>();

	public TextStyleSetting TooltipTextStyle;

	private Dictionary<NotificationType, string> notificationSounds = new Dictionary<NotificationType, string>();

	private Dictionary<string, float> timeOfLastNotification = new Dictionary<string, float>();

	private float soundDecayTime = 10f;

	private List<NotificationScreen.Entry> entries = new List<NotificationScreen.Entry>();

	private Dictionary<string, NotificationScreen.Entry> entriesByMessage = new Dictionary<string, NotificationScreen.Entry>();

	[MyCmpReq]
	private NotificationTracker notificationTracker;

	private class Entry
	{
		public Entry(GameObject label)
		{
			this.label = label;
		}

		public void Add(Notification notification)
		{
			this.notifications.Add(notification);
			this.UpdateMessage(notification.titleText);
		}

		public void Remove(Notification notification)
		{
			this.notifications.Remove(notification);
			this.UpdateMessage(notification.titleText);
		}

		public void UpdateMessage(string base_message)
		{
			if (Game.IsQuitting())
			{
				return;
			}
			this.message = base_message;
			if (this.notifications.Count > 1)
			{
				this.message = this.message + " (" + this.notifications.Count.ToString() + ")";
			}
			if (this.label.gameObject != null)
			{
				this.label.GetComponentInChildren<LocText>().text = this.message;
			}
		}

		public Notification NextClickedNotification
		{
			get
			{
				return this.notifications[this.clickIdx++ % this.notifications.Count];
			}
		}

		public string message;

		public int clickIdx;

		public GameObject label;

		public List<Notification> notifications = new List<Notification>();
	}
}
