using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

public class StatusItemGroup
{
	public StatusItemGroup(GameObject go)
	{
		this.gameObject = go;
	}

	public IEnumerator<StatusItemGroup.Entry> GetEnumerator()
	{
		return this.items.GetEnumerator();
	}

	public GameObject gameObject { get; private set; }

	public void SetOffset(Vector3 offset)
	{
		this.offset = offset;
		Game.Instance.SetStatusItemOffset(this.gameObject.transform, offset);
	}

	public StatusItemGroup.Entry GetStatusItem(StatusItemCategory category)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i].category == category)
			{
				return this.items[i];
			}
		}
		return default(StatusItemGroup.Entry);
	}

	public Guid SetStatusItem(StatusItemCategory category, StatusItem item, object data = null)
	{
		this.Log("Set", item);
		if (item != null && item.allowMultiples)
		{
			throw new ArgumentException(item.Name + " allows multiple instances of itself to be active so you must access it via its handle");
		}
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i].category == category)
			{
				if (this.items[i].item == item)
				{
					return this.items[i].id;
				}
				this.RemoveStatusItem(this.items[i].id);
			}
		}
		if (item != null)
		{
			return this.AddStatusItem(item, data, category);
		}
		return Guid.Empty;
	}

	public void SetStatusItem(Guid guid, StatusItemCategory category, StatusItem new_item, object data = null)
	{
		this.Log("Set", new_item);
		this.RemoveStatusItem(guid);
		if (new_item != null)
		{
			this.AddStatusItem(new_item, data, category);
		}
	}

	public bool HasStatusItem(StatusItem status_item)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i].item.Id == status_item.Id)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasStatusItemID(StatusItem status_item)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i].item.Id == status_item.Id)
			{
				return true;
			}
		}
		return false;
	}

	public Guid AddStatusItem(StatusItem item, object data = null, StatusItemCategory category = null)
	{
		this.Log("Add", item);
		if (this.gameObject == null || (!item.allowMultiples && this.HasStatusItem(item)))
		{
			return Guid.Empty;
		}
		if (!item.allowMultiples)
		{
			foreach (StatusItemGroup.Entry entry in this.items)
			{
				if (entry.item.Id == item.Id)
				{
					throw new ArgumentException("Tried to add " + item.Id + " multiples times which is not permitted.");
				}
			}
		}
		StatusItemGroup.Entry entry2 = new StatusItemGroup.Entry(item, category, data);
		if (item.shouldNotify)
		{
			entry2.notification = new Notification(item.notificationText, item.notificationType, null, new Func<List<Notification>, object, string>(StatusItemGroup.OnToolTip), item, false, item.notificationDelay, item.notificationClickCallback, data, item.soundPath);
			this.gameObject.GetComponent<Notifier>().Add(entry2.notification, string.Empty);
		}
		if (item.ShouldShowIcon())
		{
			Game.Instance.AddStatusItem(this.gameObject.transform, item);
			Game.Instance.SetStatusItemOffset(this.gameObject.transform, this.offset);
		}
		this.items.Add(entry2);
		if (item.effect != null)
		{
			Effects component = this.gameObject.GetComponent<Effects>();
			component.Add(item.effect, false);
		}
		if (this.OnAddStatusItem != null)
		{
			this.OnAddStatusItem(entry2, category);
		}
		return entry2.id;
	}

	public Guid RemoveStatusItem(StatusItem status_item)
	{
		this.Log("Remove", status_item);
		if (status_item.allowMultiples)
		{
			throw new ArgumentException(status_item.Name + " allows multiple instances of itself to be active so it must be released via an instance handle");
		}
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i].item.Id == status_item.Id)
			{
				this.RemoveStatusItem(this.items[i].id);
				break;
			}
		}
		return Guid.Empty;
	}

	public Guid RemoveStatusItem(Guid guid)
	{
		if (guid == Guid.Empty)
		{
			return guid;
		}
		for (int i = 0; i < this.items.Count; i++)
		{
			StatusItemGroup.Entry entry = this.items[i];
			if (entry.id == guid)
			{
				StatusItemGroup.Entry entry2 = this.items[i];
				this.items.RemoveAt(i);
				if (entry2.notification != null)
				{
					this.gameObject.GetComponent<Notifier>().Remove(entry2.notification);
				}
				if (entry.item.ShouldShowIcon())
				{
					Game.Instance.RemoveStatusItem(this.gameObject.transform, entry2.item);
				}
				if (entry2.item.effect != null)
				{
					Effects component = this.gameObject.GetComponent<Effects>();
					component.Remove(entry2.item.effect);
				}
				if (this.OnRemoveStatusItem != null)
				{
					this.OnRemoveStatusItem(entry2);
				}
				break;
			}
		}
		return Guid.Empty;
	}

	private static string OnToolTip(List<Notification> notifications, object data)
	{
		StatusItem statusItem = (StatusItem)data;
		string text = statusItem.notificationTooltipText + "\n";
		foreach (Notification notification in notifications)
		{
			text = text + "\n" + notification.Notifier.GetComponent<KSelectable>().GetName();
		}
		return text;
	}

	public void Destroy()
	{
		if (Game.IsQuitting())
		{
			return;
		}
		while (this.items.Count > 0)
		{
			this.RemoveStatusItem(this.items[0].id);
		}
	}

	private void Log(string action, StatusItem item)
	{
		if (item != null)
		{
			string id = item.Id;
		}
	}

	public global::Logger GetLog()
	{
		return this.log;
	}

	private List<StatusItemGroup.Entry> items = new List<StatusItemGroup.Entry>();

	public Action<StatusItemGroup.Entry, StatusItemCategory> OnAddStatusItem;

	public Action<StatusItemGroup.Entry> OnRemoveStatusItem;

	private Vector3 offset = new Vector3(0f, 0f, 0f);

	private LoggerFSS log = new LoggerFSS("StatusItemGroup");

	public struct Entry : IComparable<StatusItemGroup.Entry>, IEquatable<StatusItemGroup.Entry>
	{
		public Entry(StatusItem item, StatusItemCategory category, object data)
		{
			this.id = Guid.NewGuid();
			this.item = item;
			this.data = data;
			this.category = category;
			this.notification = null;
		}

		public string GetName()
		{
			return this.item.GetName(this.data);
		}

		public void ShowToolTip(ToolTip tooltip_widget, TextStyleSetting property_style)
		{
			this.item.ShowToolTip(tooltip_widget, this.data, property_style);
		}

		public void SetIcon(Image image)
		{
			this.item.SetIcon(image, this.data);
		}

		public int CompareTo(StatusItemGroup.Entry other)
		{
			return this.id.CompareTo(other.id);
		}

		public bool Equals(StatusItemGroup.Entry other)
		{
			return this.id == other.id;
		}

		public Guid id;

		public StatusItem item;

		public object data;

		public Notification notification;

		public StatusItemCategory category;
	}
}
