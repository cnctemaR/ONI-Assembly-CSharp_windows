using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	public class DropdownMenu
	{
		public List<DropdownMenu.MenuItem> MenuItems()
		{
			return this.menuItems;
		}

		public void AppendAction(string actionName, Action<DropdownMenu.MenuAction> action, Func<DropdownMenu.MenuAction, DropdownMenu.MenuAction.StatusFlags> actionStatusCallback, object userData = null)
		{
			DropdownMenu.MenuAction menuAction = new DropdownMenu.MenuAction(actionName, action, actionStatusCallback, userData);
			this.menuItems.Add(menuAction);
		}

		public void InsertAction(int atIndex, string actionName, Action<DropdownMenu.MenuAction> action, Func<DropdownMenu.MenuAction, DropdownMenu.MenuAction.StatusFlags> actionStatusCallback, object userData = null)
		{
			DropdownMenu.MenuAction menuAction = new DropdownMenu.MenuAction(actionName, action, actionStatusCallback, userData);
			this.menuItems.Insert(atIndex, menuAction);
		}

		public void AppendSeparator(string subMenuPath = null)
		{
			if (this.menuItems.Count > 0 && !(this.menuItems[this.menuItems.Count - 1] is DropdownMenu.Separator))
			{
				DropdownMenu.Separator separator = new DropdownMenu.Separator((subMenuPath != null) ? subMenuPath : string.Empty);
				this.menuItems.Add(separator);
			}
		}

		public void InsertSeparator(string subMenuPath, int atIndex)
		{
			if (atIndex > 0 && atIndex <= this.menuItems.Count && !(this.menuItems[atIndex - 1] is DropdownMenu.Separator))
			{
				DropdownMenu.Separator separator = new DropdownMenu.Separator((subMenuPath != null) ? subMenuPath : string.Empty);
				this.menuItems.Insert(atIndex, separator);
			}
		}

		public void RemoveItemAt(int index)
		{
			this.menuItems.RemoveAt(index);
		}

		public void PrepareForDisplay(EventBase e)
		{
			this.m_EventInfo = ((e == null) ? null : new DropdownMenu.EventInfo(e));
			if (this.menuItems.Count != 0)
			{
				foreach (DropdownMenu.MenuItem menuItem in this.menuItems)
				{
					DropdownMenu.MenuAction menuAction = menuItem as DropdownMenu.MenuAction;
					if (menuAction != null)
					{
						menuAction.UpdateActionStatus(this.m_EventInfo);
					}
				}
				if (this.menuItems[this.menuItems.Count - 1] is DropdownMenu.Separator)
				{
					this.menuItems.RemoveAt(this.menuItems.Count - 1);
				}
			}
		}

		private List<DropdownMenu.MenuItem> menuItems = new List<DropdownMenu.MenuItem>();

		private DropdownMenu.EventInfo m_EventInfo;

		public class EventInfo
		{
			public EventInfo(EventBase e)
			{
				IMouseEvent mouseEvent = e as IMouseEvent;
				if (mouseEvent != null)
				{
					this.mousePosition = mouseEvent.mousePosition;
					this.localMousePosition = mouseEvent.localMousePosition;
					this.modifiers = mouseEvent.modifiers;
					this.character = '\0';
					this.keyCode = KeyCode.None;
				}
				else
				{
					IKeyboardEvent keyboardEvent = e as IKeyboardEvent;
					if (keyboardEvent != null)
					{
						this.character = keyboardEvent.character;
						this.keyCode = keyboardEvent.keyCode;
						this.modifiers = keyboardEvent.modifiers;
						this.mousePosition = Vector2.zero;
						this.localMousePosition = Vector2.zero;
					}
				}
			}

			public EventModifiers modifiers { get; }

			public Vector2 mousePosition { get; }

			public Vector2 localMousePosition { get; }

			private char character { get; }

			private KeyCode keyCode { get; }
		}

		public abstract class MenuItem
		{
		}

		public class Separator : DropdownMenu.MenuItem
		{
			public Separator(string subMenuPath)
			{
				this.subMenuPath = subMenuPath;
			}

			public string subMenuPath;
		}

		public class MenuAction : DropdownMenu.MenuItem
		{
			public MenuAction(string actionName, Action<DropdownMenu.MenuAction> actionCallback, Func<DropdownMenu.MenuAction, DropdownMenu.MenuAction.StatusFlags> actionStatusCallback, object userData = null)
			{
				this.name = actionName;
				this.actionCallback = actionCallback;
				this.actionStatusCallback = actionStatusCallback;
				this.userData = userData;
			}

			public DropdownMenu.MenuAction.StatusFlags status { get; private set; }

			public DropdownMenu.EventInfo eventInfo { get; private set; }

			public object userData { get; private set; }

			public static DropdownMenu.MenuAction.StatusFlags AlwaysEnabled(DropdownMenu.MenuAction a)
			{
				return DropdownMenu.MenuAction.StatusFlags.Normal;
			}

			public static DropdownMenu.MenuAction.StatusFlags AlwaysDisabled(DropdownMenu.MenuAction a)
			{
				return DropdownMenu.MenuAction.StatusFlags.Disabled;
			}

			public void UpdateActionStatus(DropdownMenu.EventInfo eventInfo)
			{
				this.eventInfo = eventInfo;
				DropdownMenu.MenuAction.StatusFlags? statusFlags = ((this.actionStatusCallback != null) ? new DropdownMenu.MenuAction.StatusFlags?(this.actionStatusCallback(this)) : null);
				this.status = ((statusFlags == null) ? DropdownMenu.MenuAction.StatusFlags.Hidden : statusFlags.Value);
			}

			public void Execute()
			{
				if (this.actionCallback != null)
				{
					this.actionCallback(this);
				}
			}

			public string name;

			private Action<DropdownMenu.MenuAction> actionCallback;

			private Func<DropdownMenu.MenuAction, DropdownMenu.MenuAction.StatusFlags> actionStatusCallback;

			[Flags]
			public enum StatusFlags
			{
				Normal = 0,
				Disabled = 1,
				Checked = 2,
				Hidden = 4
			}
		}
	}
}
