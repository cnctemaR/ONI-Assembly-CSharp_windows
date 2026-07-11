using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class KScreenManager : KMonoBehaviour, IInputHandler
{
	public static KScreenManager Instance { get; private set; }

	public string handlerName
	{
		get
		{
			return base.gameObject.name;
		}
	}

	public KInputHandler inputHandler { get; set; }

	private void OnApplicationQuit()
	{
		KScreenManager.quitting = true;
	}

	public void DisableInput(bool disable)
	{
		KScreenManager.inputDisabled = disable;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		KScreenManager.Instance = this;
	}

	protected override void OnCleanUp()
	{
		KScreenManager.Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.evSys = global::UnityEngine.EventSystems.EventSystem.current;
	}

	protected override void OnCmpDisable()
	{
		if (KScreenManager.quitting)
		{
			for (int i = this.screenStack.Count - 1; i >= 0; i--)
			{
				KScreen kscreen = this.screenStack[i];
				kscreen.Deactivate();
			}
		}
	}

	public GameObject ActivateScreen(GameObject screen, GameObject parent)
	{
		KScreenManager.AddExistingChild(parent, screen);
		KScreen component = screen.GetComponent<KScreen>();
		component.Activate();
		return screen;
	}

	public KScreen InstantiateScreen(GameObject screenPrefab, GameObject parent)
	{
		GameObject gameObject = KScreenManager.AddChild(parent, screenPrefab);
		return gameObject.GetComponent<KScreen>();
	}

	public KScreen StartScreen(GameObject screenPrefab, GameObject parent)
	{
		GameObject gameObject = KScreenManager.AddChild(parent, screenPrefab);
		KScreen component = gameObject.GetComponent<KScreen>();
		component.Activate();
		return component;
	}

	public void PushScreen(KScreen screen)
	{
		this.screenStack.Add(screen);
		this.RefreshStack();
	}

	public void RefreshStack()
	{
		this.screenStack = (from x in this.screenStack
			where x != null
			orderby x.GetSortKey()
			select x).ToList<KScreen>();
	}

	public KScreen PopScreen(KScreen screen)
	{
		KScreen kscreen = null;
		int num = this.screenStack.IndexOf(screen);
		if (num >= 0)
		{
			kscreen = this.screenStack[num];
			this.screenStack.RemoveAt(num);
		}
		this.screenStack = (from x in this.screenStack
			where x != null
			orderby x.GetSortKey()
			select x).ToList<KScreen>();
		return kscreen;
	}

	public KScreen PopScreen()
	{
		KScreen kscreen = this.screenStack[this.screenStack.Count - 1];
		this.screenStack.RemoveAt(this.screenStack.Count - 1);
		return kscreen;
	}

	public string DebugScreenStack()
	{
		string text = string.Empty;
		foreach (KScreen kscreen in this.screenStack)
		{
			text = text + kscreen.name + "\n";
		}
		return text;
	}

	private void Update()
	{
		bool flag = true;
		for (int i = this.screenStack.Count - 1; i >= 0; i--)
		{
			KScreen kscreen = this.screenStack[i];
			if (kscreen != null && kscreen.isActiveAndEnabled)
			{
				kscreen.ScreenUpdate(flag);
			}
			if (flag && kscreen.IsModal())
			{
				flag = false;
			}
		}
	}

	public void OnKeyDown(KButtonEvent e)
	{
		if (KScreenManager.inputDisabled)
		{
			return;
		}
		for (int i = this.screenStack.Count - 1; i >= 0; i--)
		{
			KScreen kscreen = this.screenStack[i];
			if (kscreen != null && kscreen.isActiveAndEnabled)
			{
				kscreen.OnKeyDown(e);
				if (e.Consumed || kscreen.IsModal())
				{
					this.lastConsumedEvent = e;
					this.lastConsumedEventScreen = kscreen;
					break;
				}
			}
		}
	}

	public void OnKeyUp(KButtonEvent e)
	{
		if (KScreenManager.inputDisabled)
		{
			return;
		}
		for (int i = this.screenStack.Count - 1; i >= 0; i--)
		{
			KScreen kscreen = this.screenStack[i];
			if (kscreen != null && kscreen.isActiveAndEnabled)
			{
				kscreen.OnKeyUp(e);
				if (e.Consumed || kscreen.IsModal())
				{
					this.lastConsumedEvent = e;
					this.lastConsumedEventScreen = kscreen;
					break;
				}
			}
		}
	}

	public void SetEventSystemEnabled(bool state)
	{
		if (this.evSys == null)
		{
			this.evSys = global::UnityEngine.EventSystems.EventSystem.current;
			if (this.evSys == null)
			{
				global::Debug.LogWarning("Cannot enable/disable null UI event system");
				return;
			}
		}
		if (this.evSys.enabled != state)
		{
			this.evSys.enabled = state;
		}
	}

	public void SetNavigationEventsEnabled(bool state)
	{
		if (this.evSys == null)
		{
			return;
		}
		this.evSys.sendNavigationEvents = state;
	}

	public static GameObject AddExistingChild(GameObject parent, GameObject go)
	{
		if (go != null && parent != null)
		{
			go.transform.SetParent(parent.transform, false);
			go.layer = parent.layer;
		}
		return go;
	}

	public static GameObject AddChild(GameObject parent, GameObject prefab)
	{
		return Util.KInstantiateUI(prefab, parent, false);
	}

	private static bool quitting;

	private static bool inputDisabled;

	private List<KScreen> screenStack = new List<KScreen>();

	private global::UnityEngine.EventSystems.EventSystem evSys;

	private KButtonEvent lastConsumedEvent;

	private KScreen lastConsumedEventScreen;
}
