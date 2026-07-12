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

	private static void OnRefreshResponseFromServer()
	{
		foreach (KleiItemsStatusRefresher.UIListener uilistener in KleiItemsStatusRefresher.listeners)
		{
			uilistener.Internal_RefreshUI();
		}
	}

	public static void Refresh()
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
