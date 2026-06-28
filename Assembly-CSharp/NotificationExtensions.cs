using System;
using System.Collections.Generic;

public static class NotificationExtensions
{
	public static string ReduceMessages(this List<Notification> notifications, bool countNames = true)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (Notification notification in notifications)
		{
			int num = 0;
			if (!dictionary.TryGetValue(notification.NotifierName, out num))
			{
				dictionary[notification.NotifierName] = 0;
			}
			dictionary[notification.NotifierName] = num + 1;
		}
		string text = string.Empty;
		foreach (KeyValuePair<string, int> keyValuePair in dictionary)
		{
			if (countNames)
			{
				string text2 = text;
				text = string.Concat(new object[] { text2, "\n", keyValuePair.Key, "(", keyValuePair.Value, ")" });
			}
			else
			{
				text = text + "\n" + keyValuePair.Key;
			}
		}
		return text;
	}
}
