using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class KInputHandler
{
	public KInputHandler(object obj, KInputController controller)
		: this(obj)
	{
		this.mController = controller;
	}

	public KInputHandler(object obj)
	{
		MethodInfo method = obj.GetType().GetMethod("OnKeyDown");
		if (method != null)
		{
			Action<KButtonEvent> action = (Action<KButtonEvent>)Delegate.CreateDelegate(typeof(Action<KButtonEvent>), obj, method);
			this.mOnKeyDownDelegates.Add(action);
		}
		MethodInfo method2 = obj.GetType().GetMethod("OnKeyUp");
		if (method2 != null)
		{
			Action<KButtonEvent> action2 = (Action<KButtonEvent>)Delegate.CreateDelegate(typeof(Action<KButtonEvent>), obj, method2);
			this.mOnKeyUpDelegates.Add(action2);
		}
	}

	private void SetController(KInputController controller)
	{
		this.mController = controller;
		if (this.mChildren != null)
		{
			foreach (KInputHandler.HandlerInfo handlerInfo in this.mChildren)
			{
				handlerInfo.handler.SetController(controller);
			}
		}
	}

	public void AddInputHandler(KInputHandler handler, int priority)
	{
		if (this.mChildren == null)
		{
			this.mChildren = new List<KInputHandler.HandlerInfo>();
		}
		handler.SetController(this.mController);
		this.mChildren.Add(new KInputHandler.HandlerInfo
		{
			priority = priority,
			handler = handler
		});
		this.mChildren.Sort((KInputHandler.HandlerInfo a, KInputHandler.HandlerInfo b) => b.priority.CompareTo(a.priority));
	}

	public void RemoveInputHandler(KInputHandler handler)
	{
		if (this.mChildren != null)
		{
			for (int i = 0; i < this.mChildren.Count; i++)
			{
				if (this.mChildren[i].handler == handler)
				{
					this.mChildren.RemoveAt(i);
					break;
				}
			}
		}
	}

	public void PushInputHandler(KInputHandler handler)
	{
		if (this.mChildren == null)
		{
			this.mChildren = new List<KInputHandler.HandlerInfo>();
		}
		handler.SetController(this.mController);
		this.mChildren.Insert(0, new KInputHandler.HandlerInfo
		{
			priority = int.MaxValue,
			handler = handler
		});
	}

	public void PopInputHandler()
	{
		if (this.mChildren != null)
		{
			this.mChildren.RemoveAt(0);
		}
	}

	public void HandleEvent(KInputEvent e)
	{
		if (e.Type == InputEventType.KeyDown)
		{
			this.HandleKeyDown((KButtonEvent)e);
		}
		else if (e.Type == InputEventType.KeyUp)
		{
			this.HandleKeyUp((KButtonEvent)e);
		}
	}

	public void HandleKeyDown(KButtonEvent e)
	{
		foreach (Action<KButtonEvent> action in this.mOnKeyDownDelegates)
		{
			action(e);
		}
		if (!e.Consumed && this.mChildren != null)
		{
			foreach (KInputHandler.HandlerInfo handlerInfo in this.mChildren)
			{
				handlerInfo.handler.HandleKeyDown(e);
				if (e.Consumed)
				{
					break;
				}
			}
		}
	}

	public void HandleKeyUp(KButtonEvent e)
	{
		foreach (Action<KButtonEvent> action in this.mOnKeyUpDelegates)
		{
			action(e);
		}
		if (!e.Consumed && this.mChildren != null)
		{
			foreach (KInputHandler.HandlerInfo handlerInfo in this.mChildren)
			{
				handlerInfo.handler.HandleKeyUp(e);
				if (e.Consumed)
				{
					break;
				}
			}
		}
	}

	public static KInputHandler GetInputHandler(IInputHandler handler)
	{
		if (handler.inputHandler == null)
		{
			handler.inputHandler = new KInputHandler(handler);
		}
		return handler.inputHandler;
	}

	public static void Add(IInputHandler parent, GameObject child)
	{
		foreach (Component component in child.GetComponents<Component>())
		{
			IInputHandler inputHandler = component as IInputHandler;
			if (inputHandler != null)
			{
				KInputHandler.Add(parent, inputHandler, 0);
			}
		}
	}

	public static void Add(IInputHandler parent, IInputHandler child, int priority = 0)
	{
		KInputHandler inputHandler = KInputHandler.GetInputHandler(parent);
		KInputHandler inputHandler2 = KInputHandler.GetInputHandler(child);
		inputHandler.AddInputHandler(inputHandler2, priority);
	}

	public static void Push(IInputHandler parent, IInputHandler child)
	{
		KInputHandler inputHandler = KInputHandler.GetInputHandler(parent);
		KInputHandler inputHandler2 = KInputHandler.GetInputHandler(child);
		inputHandler.PushInputHandler(inputHandler2);
	}

	public static void Remove(IInputHandler parent, IInputHandler child)
	{
		KInputHandler inputHandler = KInputHandler.GetInputHandler(parent);
		KInputHandler inputHandler2 = KInputHandler.GetInputHandler(child);
		inputHandler.RemoveInputHandler(inputHandler2);
	}

	public bool IsActive(global::Action action)
	{
		return this.mController != null && this.mController.IsActive(action);
	}

	public float GetAxis(Axis axis)
	{
		if (this.mController != null)
		{
			return this.mController.GetAxis(axis);
		}
		return 0f;
	}

	public bool IsGamepad()
	{
		return this.mController != null && this.mController.IsGamepad;
	}

	private List<Action<KButtonEvent>> mOnKeyDownDelegates = new List<Action<KButtonEvent>>();

	private List<Action<KButtonEvent>> mOnKeyUpDelegates = new List<Action<KButtonEvent>>();

	private List<KInputHandler.HandlerInfo> mChildren;

	private KInputController mController;

	private struct HandlerInfo
	{
		public int priority;

		public KInputHandler handler;
	}

	public delegate void KButtonEventHandler(KButtonEvent e);

	public delegate void KCancelInputHandler();
}
