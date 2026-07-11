using System;
using System.Collections.Generic;
using UnityEngine;

public class KInputManager
{
	public static bool isFocused { get; private set; }

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
		KInputManager.isFocused = true;
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
		KInputManager.isFocused = focus;
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
		return Input.mousePosition;
	}

	protected List<KInputController> mControllers = new List<KInputController>();

	public static bool isMousePosLocked;

	public static Vector3 lockedMousePos;
}
