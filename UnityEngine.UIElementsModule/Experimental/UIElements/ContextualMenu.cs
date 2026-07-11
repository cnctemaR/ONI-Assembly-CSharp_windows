using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	[Obsolete("ContextualMenu has been deprecated. Use DropdownMenu instead.", true)]
	public class ContextualMenu
	{
		public List<ContextualMenu.MenuItem> MenuItems()
		{
			return new List<ContextualMenu.MenuItem>();
		}

		public void AppendAction(string actionName, Action<ContextualMenu.MenuAction> action, Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags> actionStatusCallback, object userData = null)
		{
		}

		public void InsertAction(int atIndex, string actionName, Action<ContextualMenu.MenuAction> action, Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags> actionStatusCallback, object userData = null)
		{
		}

		public void AppendSeparator(string subMenuPath = null)
		{
		}

		public void InsertSeparator(string subMenuPath, int atIndex)
		{
		}

		public void RemoveItemAt(int index)
		{
		}

		public void PrepareForDisplay(EventBase e)
		{
		}

		[Obsolete("ContextualMenu.EventInfo has been deprecated. Use DropdownMenu.EventInfo instead.", true)]
		public class EventInfo
		{
			public EventInfo(EventBase e)
			{
			}

			public EventModifiers modifiers { get; }

			public Vector2 mousePosition { get; }

			public Vector2 localMousePosition { get; }
		}

		[Obsolete("ContextualMenu.MenuItem has been deprecated. Use DropdownMenu.MenuItem instead.", true)]
		public abstract class MenuItem
		{
		}

		[Obsolete("ContextualMenu.Separator has been deprecated. Use DropdownMenu.Separator instead.", true)]
		public class Separator : ContextualMenu.MenuItem
		{
			public Separator(string subMenuPath)
			{
			}

			public string subMenuPath;
		}

		[Obsolete("ContextualMenu.MenuAction has been deprecated. Use DropdownMenu.MenuAction instead.", true)]
		public class MenuAction : ContextualMenu.MenuItem
		{
			public MenuAction(string actionName, Action<ContextualMenu.MenuAction> actionCallback, Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags> actionStatusCallback, object userData = null)
			{
			}

			public ContextualMenu.MenuAction.StatusFlags status { get; private set; }

			public ContextualMenu.EventInfo eventInfo { get; private set; }

			public object userData { get; private set; }

			public static ContextualMenu.MenuAction.StatusFlags AlwaysEnabled(ContextualMenu.MenuAction a)
			{
				return ContextualMenu.MenuAction.StatusFlags.Normal;
			}

			public static ContextualMenu.MenuAction.StatusFlags AlwaysDisabled(ContextualMenu.MenuAction a)
			{
				return ContextualMenu.MenuAction.StatusFlags.Disabled;
			}

			public void UpdateActionStatus(ContextualMenu.EventInfo eventInfo)
			{
			}

			public void Execute()
			{
			}

			public string name;

			[Obsolete("ContextualMenu.MenuAction.StatusFlags has been deprecated. Use DropdownMenu.MenuAction.StatusFlags instead.", true)]
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
