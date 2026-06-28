using System;
using System.Collections.Generic;

public class KInputManager
{
	public KInputManager()
	{
		KInput.Log("KinputManager initialized.");
	}

	public static bool isFocused { get; private set; }

	public static long lastUserActionTick { get; private set; }

	public static void SetUserActive()
	{
		if (KInputManager.isFocused)
		{
			KInputManager.lastUserActionTick = DateTime.Now.Ticks;
		}
	}

	public void AddController(KInputController controller)
	{
		this.mControllers.Add(controller);
	}

	public KInputController GetController(int controller_index)
	{
		DebugUtil.Assert(controller_index < this.mControllers.Count, "Assert!");
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

	public float timeOfLastUserAction { get; private set; }

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
		for (int i = 0; i < this.mControllers.Count; i++)
		{
			this.mControllers[i].Dispatch();
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

	protected List<KInputController> mControllers = new List<KInputController>();
}
