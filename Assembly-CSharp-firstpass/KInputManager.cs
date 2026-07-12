using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KInputManager
{
	public static bool isFocused
	{
		get
		{
			return KInputManager.hasFocus && !KInputManager.devToolFocus;
		}
	}

	public static long lastUserActionTicks { get; private set; }

	public static void SetUserActive()
	{
		if (KInputManager.isFocused)
		{
			KInputManager.lastUserActionTicks = DateTime.Now.Ticks;
		}
	}

	public KInputManager()
	{
		KInputManager.lastUserActionTicks = DateTime.Now.Ticks;
		KInputManager.hasFocus = true;
	}

	public void AddController(KInputController controller)
	{
		this.mControllers.Add(controller);
	}

	public KInputController GetController(int controller_index)
	{
		DebugUtil.Assert(controller_index < this.mControllers.Count);
		return this.mControllers[controller_index];
	}

	public int GetControllerCount()
	{
		return this.mControllers.Count;
	}

	public KInputController GetDefaultController()
	{
		return this.GetController(0);
	}

	public virtual void Update()
	{
		if (KInputManager.isFocused)
		{
			for (int i = 0; i < this.mControllers.Count; i++)
			{
				this.mControllers[i].Update();
			}
			this.Dispatch();
		}
	}

	public virtual void Dispatch()
	{
		if (KInputManager.isFocused)
		{
			for (int i = 0; i < this.mControllers.Count; i++)
			{
				this.mControllers[i].Dispatch();
			}
		}
	}

	public virtual void OnApplicationFocus(bool focus)
	{
		KInputManager.hasFocus = focus;
		KInputManager.SetUserActive();
		if (!KInputManager.isFocused)
		{
			foreach (KInputController kinputController in this.mControllers)
			{
				kinputController.HandleCancelInput();
			}
		}
	}

	public static Vector3 GetMousePos()
	{
		if (KInputManager.isMousePosLocked)
		{
			return KInputManager.lockedMousePos;
		}
		if (KInputManager.currentControllerIsGamepad)
		{
			return KInputManager.virtualCursorPos;
		}
		return Input.mousePosition;
	}

	protected List<KInputController> mControllers = new List<KInputController>();

	private static bool hasFocus = false;

	public static bool devToolFocus = false;

	public static SteamInputInterpreter steamInputInterpreter = new SteamInputInterpreter();

	public static Vector3F virtualCursorPos;

	public static bool currentControllerIsGamepad;

	public static KInputController prevController;

	public static KInputController currentController;

	public static UnityEvent InputChange = new UnityEvent();

	public static bool isMousePosLocked;

	public static Vector3 lockedMousePos;
}
