using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Notification
{
	public Notification(string title, NotificationType type, HashedString group, Func<List<Notification>, object, string> tooltip = null, object tooltip_data = null, bool expires = true, float delay = 0f, Notification.ClickCallback custom_click_callback = null, object custom_click_data = null)
	{
		this.titleText = title;
		this.Group = group;
		this.Type = type;
		this.ToolTip = tooltip;
		this.tooltipData = tooltip_data;
		this.expires = expires;
		this.Delay = delay;
		this.customClickCallback = custom_click_callback;
		this.customClickData = custom_click_data;
	}

	public NotificationType Type { get; set; }

	public Notifier Notifier { get; set; }

	public float Time { get; set; }

	public float GameTime { get; set; }

	public float Delay { get; set; }

	public int Idx { get; set; }

	public Func<List<Notification>, object, string> ToolTip { get; set; }

	public bool IsReady()
	{
		return global::UnityEngine.Time.time >= this.GameTime + this.Delay;
	}

	public string titleText { get; private set; }

	public string NotifierName
	{
		get
		{
			return this.notifierName;
		}
		set
		{
			this.notifierName = value;
			this.titleText = this.ReplaceTags(this.titleText);
		}
	}

	public void Clear()
	{
		if (this.Notifier != null)
		{
			this.Notifier.Remove(this);
		}
	}

	private string ReplaceTags(string text)
	{
		DebugUtil.Assert(text != null);
		int num = text.IndexOf('{');
		int num2 = text.IndexOf('}');
		if (0 <= num && num < num2)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num3 = 0;
			while (0 <= num)
			{
				string text2 = text.Substring(num3, num - num3);
				stringBuilder.Append(text2);
				num2 = text.IndexOf('}', num);
				if (num >= num2)
				{
					break;
				}
				string text3 = text.Substring(num + 1, num2 - num - 1);
				string tagDescription = this.GetTagDescription(text3);
				stringBuilder.Append(tagDescription);
				num3 = num2 + 1;
				num = text.IndexOf('{', num2);
			}
			stringBuilder.Append(text.Substring(num3, text.Length - num3));
			return stringBuilder.ToString();
		}
		return text;
	}

	private string GetTagDescription(string tag)
	{
		string text;
		if (tag == "NotifierName")
		{
			text = this.notifierName;
		}
		else
		{
			text = "UNKNOWN TAG: " + tag;
		}
		return text;
	}

	public HashedString Group;

	public Vector3 Position;

	public object tooltipData;

	public bool hasLocation = true;

	public bool expires = true;

	public bool playSound = true;

	public Notification.ClickCallback customClickCallback;

	public object customClickData;

	private string notifierName;

	public delegate void ClickCallback(object data);
}
