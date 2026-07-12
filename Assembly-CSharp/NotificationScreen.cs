using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class NotificationScreen : KScreen
{
	public static NotificationScreen Instance { get; private set; }

	public static void DestroyInstance()
	{
		NotificationScreen.Instance = null;
	}

	public void AddPendingNotification(Notification notification)
	{
		this.pendingNotifications.Add(notification);
	}

	public void RemovePendingNotification(Notification notification)
	{
		this.dirty = true;
		this.pendingNotifications.Remove(notification);
		this.RemoveNotification(notification);
	}

	public void RemoveNotification(Notification notification)
	{
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
		this.MessagesPrefab.gameObject.SetActive(false);
		this.LabelPrefab.gameObject.SetActive(false);
		this.InitNotificationSounds();
	}

	private void OnNewMessage(object data)
	{
		Message message = (Message)data;
		this.notifier.Add(new MessageNotification(message), "");
	}

	private void ShowMessage(MessageNotification mn)
	{
		mn.message.OnClick();
		if (mn.message.ShowDialog())
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.initTime = KTime.Instance.UnscaledGameTime;
		LocText[] array = this.LabelPrefab.GetComponentsInChildren<LocText>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = this.normalColor;
		}
		array = this.MessagesPrefab.GetComponentsInChildren<LocText>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = this.normalColor;
		}
		base.Subscribe(Messenger.Instance.gameObject, 1558809273, new Action<object>(this.OnNewMessage));
		foreach (Message message in Messenger.Instance.Messages)
		{
			Notification notification = new MessageNotification(message);
			notification.playSound = false;
			this.notifier.Add(notification, "");
		}
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		this.dirty = true;
	}

	public void AddNotification(Notification notification)
	{
		if (DebugHandler.NotificationsDisabled)
		{
			return;
		}
		this.notifications.Add(notification);
		NotificationScreen.Entry entry;
		this.entriesByMessage.TryGetValue(notification.titleText, out entry);
		if (entry == null)
		{
			HierarchyReferences hierarchyReferences;
			if (notification.Type == NotificationType.Messages)
			{
				hierarchyReferences = global::Util.KInstantiateUI<HierarchyReferences>(this.MessagesPrefab, this.MessagesFolder, false);
			}
			else
			{
				hierarchyReferences = global::Util.KInstantiateUI<HierarchyReferences>(this.LabelPrefab, this.LabelsFolder, false);
			}
			Button reference = hierarchyReferences.GetReference<Button>("DismissButton");
			reference.gameObject.SetActive(notification.showDismissButton);
			if (notification.showDismissButton)
			{
				reference.onClick.AddListener(delegate
				{
					NotificationScreen.Entry entry2;
					if (!this.entriesByMessage.TryGetValue(notification.titleText, out entry2))
					{
						return;
					}
					for (int i = entry2.notifications.Count - 1; i >= 0; i--)
					{
						Notification notification2 = entry2.notifications[i];
						MessageNotification messageNotification2 = notification2 as MessageNotification;
						if (messageNotification2 != null)
						{
							Messenger.Instance.RemoveMessage(messageNotification2.message);
						}
						notification2.Clear();
					}
				});
			}
			hierarchyReferences.GetReference<NotificationAnimator>("Animator").Begin(true);
			hierarchyReferences.gameObject.SetActive(true);
			if (notification.ToolTip != null)
			{
				ToolTip tooltip = hierarchyReferences.GetReference<ToolTip>("ToolTip");
				tooltip.OnToolTip = delegate
				{
					tooltip.ClearMultiStringTooltip();
					tooltip.AddMultiStringTooltip(notification.ToolTip(entry.notifications, notification.tooltipData), this.TooltipTextStyle);
					return "";
				};
			}
			KImage reference2 = hierarchyReferences.GetReference<KImage>("Icon");
			LocText reference3 = hierarchyReferences.GetReference<LocText>("Text");
			Button reference4 = hierarchyReferences.GetReference<Button>("MainButton");
			ColorBlock colors = reference4.colors;
			switch (notification.Type)
			{
			case NotificationType.Bad:
			case NotificationType.DuplicantThreatening:
				colors.normalColor = this.badColorBG;
				reference3.color = this.badColor;
				reference2.color = this.badColor;
				reference2.sprite = ((notification.Type == NotificationType.Bad) ? this.icon_bad : this.icon_threatening);
				goto IL_0300;
			case NotificationType.Tutorial:
				colors.normalColor = this.warningColorBG;
				reference3.color = this.warningColor;
				reference2.color = this.warningColor;
				reference2.sprite = this.icon_warning;
				goto IL_0300;
			case NotificationType.Messages:
			{
				colors.normalColor = this.messageColorBG;
				reference3.color = this.messageColor;
				reference2.color = this.messageColor;
				reference2.sprite = this.icon_message;
				MessageNotification messageNotification = notification as MessageNotification;
				if (messageNotification == null)
				{
					goto IL_0300;
				}
				TutorialMessage tutorialMessage = messageNotification.message as TutorialMessage;
				if (tutorialMessage != null && !string.IsNullOrEmpty(tutorialMessage.videoClipId))
				{
					reference2.sprite = this.icon_video;
					goto IL_0300;
				}
				goto IL_0300;
			}
			case NotificationType.Event:
				colors.normalColor = this.eventColorBG;
				reference3.color = this.eventColor;
				reference2.color = this.eventColor;
				reference2.sprite = this.icon_event;
				goto IL_0300;
			}
			colors.normalColor = this.normalColorBG;
			reference3.color = this.normalColor;
			reference2.color = this.normalColor;
			reference2.sprite = this.icon_normal;
			IL_0300:
			reference4.colors = colors;
			reference4.onClick.AddListener(delegate
			{
				this.OnClick(entry);
			});
			string text = "";
			if (KTime.Instance.UnscaledGameTime - this.initTime > 5f && notification.playSound)
			{
				this.PlayDingSound(notification, 0);
			}
			else
			{
				text = "too early";
			}
			if (AudioDebug.Get().debugNotificationSounds)
			{
				global::Debug.Log("Notification(" + notification.titleText + "):" + text);
			}
			entry = new NotificationScreen.Entry(hierarchyReferences.gameObject);
			this.entriesByMessage[notification.titleText] = entry;
			this.entries.Add(entry);
		}
		entry.Add(notification);
		this.dirty = true;
		this.SortNotifications();
	}

	private void SortNotifications()
	{
		this.notifications.Sort(delegate(Notification n1, Notification n2)
		{
			if (n1.Type == n2.Type)
			{
				return n1.Idx - n2.Idx;
			}
			return n1.Type - n2.Type;
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

	private void PlayDingSound(Notification notification, int count)
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
		float num2 = (notification.volume_attenuation ? ((Time.time - num) / this.soundDecayTime) : 1f);
		this.timeOfLastNotification[text] = Time.time;
		string text2;
		if (count > 1)
		{
			text2 = GlobalAssets.GetSound(text + "_AddCount", true);
			if (text2 == null)
			{
				text2 = GlobalAssets.GetSound(text, false);
			}
		}
		else
		{
			text2 = GlobalAssets.GetSound(text, false);
		}
		if (notification.playSound)
		{
			EventInstance eventInstance = KFMOD.BeginOneShot(text2, Vector3.zero, 1f);
			eventInstance.setParameterByName("timeSinceLast", num2, false);
			KFMOD.EndOneShot(eventInstance);
		}
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
			if (notification.expires && KTime.Instance.UnscaledGameTime - notification.Time > this.lifetime)
			{
				this.dirty = true;
				if (notification.Notifier == null)
				{
					this.RemovePendingNotification(notification);
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
		base.PlaySound3D(GlobalAssets.GetSound("HUD_Click_Open", false));
		if (nextClickedNotification.customClickCallback != null)
		{
			nextClickedNotification.customClickCallback(nextClickedNotification.customClickData);
		}
		else
		{
			if (nextClickedNotification.clickFocus != null)
			{
				Vector3 position = nextClickedNotification.clickFocus.GetPosition();
				position.z = -40f;
				ClusterGridEntity component = nextClickedNotification.clickFocus.GetComponent<ClusterGridEntity>();
				KSelectable component2 = nextClickedNotification.clickFocus.GetComponent<KSelectable>();
				int myWorldId = nextClickedNotification.clickFocus.gameObject.GetMyWorldId();
				if (myWorldId != -1)
				{
					CameraController.Instance.ActiveWorldStarWipe(myWorldId, position, 10f, null);
				}
				else if (DlcManager.FeatureClusterSpaceEnabled() && component != null && component.IsVisible)
				{
					ManagementMenu.Instance.OpenClusterMap();
					ClusterMapScreen.Instance.SetTargetFocusPosition(component.Location, 0.5f);
				}
				if (component2 != null)
				{
					if (DlcManager.FeatureClusterSpaceEnabled() && component != null && component.IsVisible)
					{
						ClusterMapSelectTool.Instance.Select(component2, false);
					}
					else
					{
						SelectTool.Instance.Select(component2, false);
					}
				}
			}
			else if (nextClickedNotification.Notifier != null)
			{
				SelectTool.Instance.Select(nextClickedNotification.Notifier.GetComponent<KSelectable>(), false);
			}
			if (nextClickedNotification.Type == NotificationType.Messages)
			{
				this.ShowMessage((MessageNotification)nextClickedNotification);
			}
		}
		if (nextClickedNotification.clearOnClick)
		{
			nextClickedNotification.Clear();
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
		this.notificationSounds[NotificationType.DuplicantThreatening] = "Warning_DupeThreatening";
		this.notificationSounds[NotificationType.Event] = "Message";
	}

	public Color32 BadColorBG
	{
		get
		{
			return this.badColorBG;
		}
	}

	public Sprite GetNotificationIcon(NotificationType type)
	{
		switch (type)
		{
		case NotificationType.Bad:
			return this.icon_bad;
		case NotificationType.Tutorial:
			return this.icon_warning;
		case NotificationType.Messages:
			return this.icon_message;
		case NotificationType.DuplicantThreatening:
			return this.icon_threatening;
		case NotificationType.Event:
			return this.icon_event;
		}
		return this.icon_normal;
	}

	public Color GetNotificationColour(NotificationType type)
	{
		switch (type)
		{
		case NotificationType.Bad:
			return this.badColor;
		case NotificationType.Tutorial:
			return this.warningColor;
		case NotificationType.Messages:
			return this.messageColor;
		case NotificationType.DuplicantThreatening:
			return this.badColor;
		case NotificationType.Event:
			return this.eventColor;
		}
		return this.normalColor;
	}

	public Color GetNotificationBGColour(NotificationType type)
	{
		switch (type)
		{
		case NotificationType.Bad:
			return this.badColorBG;
		case NotificationType.Tutorial:
			return this.warningColorBG;
		case NotificationType.Messages:
			return this.messageColorBG;
		case NotificationType.DuplicantThreatening:
			return this.badColorBG;
		case NotificationType.Event:
			return this.eventColorBG;
		}
		return this.normalColorBG;
	}

	public string GetNotificationSound(NotificationType type)
	{
		return this.notificationSounds[type];
	}

	public float lifetime;

	public bool dirty;

	public GameObject LabelPrefab;

	public GameObject LabelsFolder;

	public GameObject MessagesPrefab;

	public GameObject MessagesFolder;

	private MessageDialogFrame messageDialog;

	private float initTime;

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

	[SerializeField]
	private Color eventColorBG;

	[SerializeField]
	private Color eventColor;

	public Sprite icon_normal;

	public Sprite icon_warning;

	public Sprite icon_bad;

	public Sprite icon_threatening;

	public Sprite icon_message;

	public Sprite icon_video;

	public Sprite icon_event;

	private List<Notification> pendingNotifications = new List<Notification>();

	private List<Notification> notifications = new List<Notification>();

	public TextStyleSetting TooltipTextStyle;

	private Dictionary<NotificationType, string> notificationSounds = new Dictionary<NotificationType, string>();

	private Dictionary<string, float> timeOfLastNotification = new Dictionary<string, float>();

	private float soundDecayTime = 10f;

	private List<NotificationScreen.Entry> entries = new List<NotificationScreen.Entry>();

	private Dictionary<string, NotificationScreen.Entry> entriesByMessage = new Dictionary<string, NotificationScreen.Entry>();

	private class Entry
	{
		public Entry(GameObject label)
		{
			this.label = label;
		}

		public void Add(Notification notification)
		{
			this.notifications.Add(notification);
			this.UpdateMessage(notification, true);
		}

		public void Remove(Notification notification)
		{
			this.notifications.Remove(notification);
			this.UpdateMessage(notification, false);
		}

		public void UpdateMessage(Notification notification, bool playSound = true)
		{
			if (Game.IsQuitting())
			{
				return;
			}
			this.message = notification.titleText;
			if (this.notifications.Count > 1)
			{
				if (playSound && (notification.Type == NotificationType.Bad || notification.Type == NotificationType.DuplicantThreatening))
				{
					NotificationScreen.Instance.PlayDingSound(notification, this.notifications.Count);
				}
				this.message = this.message + " (" + this.notifications.Count.ToString() + ")";
			}
			if (this.label != null)
			{
				this.label.GetComponent<HierarchyReferences>().GetReference<LocText>("Text").text = this.message;
			}
		}

		public Notification NextClickedNotification
		{
			get
			{
				List<Notification> list = this.notifications;
				int num = this.clickIdx;
				this.clickIdx = num + 1;
				return list[num % this.notifications.Count];
			}
		}

		public string message;

		public int clickIdx;

		public GameObject label;

		public List<Notification> notifications = new List<Notification>();
	}
}
