using System;
using System.Collections.Generic;
using UnityEngine;

public class OniMetrics : MonoBehaviour
{
	private static void EnsureMetrics()
	{
		if (OniMetrics.Metrics != null)
		{
			return;
		}
		OniMetrics.Metrics = new List<Dictionary<string, object>>(2);
		for (int i = 0; i < 2; i++)
		{
			OniMetrics.Metrics.Add(null);
		}
	}

	public static void LogEvent(OniMetrics.Event eventType, string key, object data)
	{
		OniMetrics.EnsureMetrics();
		if (OniMetrics.Metrics[(int)eventType] == null)
		{
			OniMetrics.Metrics[(int)eventType] = new Dictionary<string, object>();
		}
		OniMetrics.Metrics[(int)eventType][key] = data;
	}

	public static void SendEvent(OniMetrics.Event eventType, string debugName)
	{
		if (OniMetrics.Metrics[(int)eventType] == null || OniMetrics.Metrics[(int)eventType].Count == 0)
		{
			return;
		}
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(OniMetrics.Metrics[(int)eventType], debugName);
		OniMetrics.Metrics[(int)eventType].Clear();
	}

	private static List<Dictionary<string, object>> Metrics;

	public enum Event : short
	{
		NewSave,
		EndOfCycle,
		NumEvents
	}
}
