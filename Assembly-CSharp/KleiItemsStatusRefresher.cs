using System;
using System.Collections.Generic;
using UnityEngine;

public static class KleiItemsStatusRefresher
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Initialize()
	{
		KleiItems.AddInventoryRefreshCallback(new KleiItems.InventoryRefreshCallback(KleiItemsStatusRefresher.OnRefreshResponseFromServer));
	}

	public static void RequestRefreshFromServer()
	{
		if (!KleiItemsStatusRefresher.Active)
		{
			return;
		}
		double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
		if (realtimeSinceStartupAsDouble - KleiItemsStatusRefresher.realtimeOfLastServerRequest < 360.0)
		{
			return;
		}
		KleiItems.AddRequestInventoryRefresh();
		KleiItemsStatusRefresher.realtimeOfLastServerRequest = realtimeSinceStartupAsDouble;
	}

	private static void OnRefreshResponseFromServer()
	{
		KleiItemsStatusRefresher.Active = false;
		KleiItemsStatusRefresher.RefreshUI();
	}

	public static void RefreshUI()
	{
		foreach (KleiItemsStatusRefresher.UIListener uilistener in KleiItemsStatusRefresher.listeners)
		{
			uilistener.Internal_RefreshUI();
		}
	}

	public static KleiItemsStatusRefresher.UIListener AddOrGetListener(Component component)
	{
		return KleiItemsStatusRefresher.AddOrGetListener(component.gameObject);
	}

	public static KleiItemsStatusRefresher.UIListener AddOrGetListener(GameObject onGameObject)
	{
		return onGameObject.AddOrGet<KleiItemsStatusRefresher.UIListener>();
	}

	public static bool Active = false;

	public const double SECONDS_PER_MINUTE = 60.0;

	public const double MINIMUM_SECONDS_BETWEEN_REFRESH_REQUESTS = 360.0;

	public static double realtimeOfLastServerRequest = -720.0;

	public static HashSet<KleiItemsStatusRefresher.UIListener> listeners = new HashSet<KleiItemsStatusRefresher.UIListener>();

	public class UIListener : MonoBehaviour
	{
		public void Internal_RefreshUI()
		{
			if (this.refreshUIFn != null)
			{
				this.refreshUIFn();
			}
		}

		public void OnRefreshUI(global::System.Action fn)
		{
			this.refreshUIFn = fn;
		}

		private void OnEnable()
		{
			KleiItemsStatusRefresher.listeners.Add(this);
		}

		private void OnDisable()
		{
			KleiItemsStatusRefresher.listeners.Remove(this);
		}

		private global::System.Action refreshUIFn;
	}
}
