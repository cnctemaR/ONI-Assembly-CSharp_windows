using System;
using System.Collections.Generic;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>A contextual menu.</para>
	/// </summary>
	public class ContextualMenu
	{
		/// <summary>
		///   <para>Get the list of menu items.</para>
		/// </summary>
		/// <returns>
		///   <para>The list of items in the menu.</para>
		/// </returns>
		public List<ContextualMenu.MenuItem> MenuItems()
		{
			return this.menuItems;
		}

		public void AppendAction(string actionName, Action<ContextualMenu.MenuAction> action, Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags> actionStatusCallback, object userData = null)
		{
			ContextualMenu.MenuAction menuAction = new ContextualMenu.MenuAction(actionName, action, actionStatusCallback, userData);
			this.menuItems.Add(menuAction);
		}

		public void InsertAction(int atIndex, string actionName, Action<ContextualMenu.MenuAction> action, Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags> actionStatusCallback, object userData = null)
		{
			ContextualMenu.MenuAction menuAction = new ContextualMenu.MenuAction(actionName, action, actionStatusCallback, userData);
			this.menuItems.Insert(atIndex, menuAction);
		}

		/// <summary>
		///   <para>Add a separator line in the menu. The separator is added at the end of the current item list.</para>
		/// </summary>
		/// <param name="subMenuPath">The submenu path where the separator will be added. Path components are delimited by forward slashes ('/').</param>
		public void AppendSeparator(string subMenuPath = null)
		{
			if (this.menuItems.Count > 0 && !(this.menuItems[this.menuItems.Count - 1] is ContextualMenu.Separator))
			{
				ContextualMenu.Separator separator = new ContextualMenu.Separator((subMenuPath != null) ? subMenuPath : string.Empty);
				this.menuItems.Add(separator);
			}
		}

		/// <summary>
		///   <para>Add a separator line in the menu. The separator is added at the end of the specified index in the list.</para>
		/// </summary>
		/// <param name="atIndex">Index where the separator should be inserted.</param>
		/// <param name="subMenuPath">The submenu path where the separator is added. Path components are delimited by forward slashes ('/').</param>
		public void InsertSeparator(string subMenuPath, int atIndex)
		{
			if (atIndex > 0 && atIndex <= this.menuItems.Count && !(this.menuItems[atIndex - 1] is ContextualMenu.Separator))
			{
				ContextualMenu.Separator separator = new ContextualMenu.Separator((subMenuPath != null) ? subMenuPath : string.Empty);
				this.menuItems.Insert(atIndex, separator);
			}
		}

		/// <summary>
		///   <para>Remove the menu item at index.</para>
		/// </summary>
		/// <param name="index">The index of the item to remove.</param>
		public void RemoveItemAt(int index)
		{
			this.menuItems.RemoveAt(index);
		}

		/// <summary>
		///   <para>Update the status of all items by calling their status callback and remove the separators in excess. This is called just before displaying the menu.</para>
		/// </summary>
		/// <param name="e"></param>
		public void PrepareForDisplay(EventBase e)
		{
			this.m_EventInfo = new ContextualMenu.EventInfo(e);
			if (this.menuItems.Count != 0)
			{
				foreach (ContextualMenu.MenuItem menuItem in this.menuItems)
				{
					ContextualMenu.MenuAction menuAction = menuItem as ContextualMenu.MenuAction;
					if (menuAction != null)
					{
						menuAction.UpdateActionStatus(this.m_EventInfo);
					}
				}
				if (this.menuItems[this.menuItems.Count - 1] is ContextualMenu.Separator)
				{
					this.menuItems.RemoveAt(this.menuItems.Count - 1);
				}
			}
		}

		private List<ContextualMenu.MenuItem> menuItems = new List<ContextualMenu.MenuItem>();

		private ContextualMenu.EventInfo m_EventInfo;

		/// <summary>
		///   <para>A class holding information about the event that triggered the display of the contextual menu.</para>
		/// </summary>
		public class EventInfo
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			/// <param name="e"></param>
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

			/// <summary>
			///   <para>If modifier keys (Alt, Control, Shift, Windows/Command) were pressed to trigger the display of the contextual menu, this property lists the modifier keys.</para>
			/// </summary>
			public EventModifiers modifiers { get; }

			/// <summary>
			///   <para>If the triggering event was a mouse event, this property is the mouse position expressed using the global coordinate system. Otherwise this property is zero.</para>
			/// </summary>
			public Vector2 mousePosition { get; }

			/// <summary>
			///   <para>If the triggering event was a mouse event, this property is the mouse position. The position is expressed using the coordinate system of the element that received the mouse event. Otherwise this property is zero.</para>
			/// </summary>
			public Vector2 localMousePosition { get; }

			private char character { get; }

			private KeyCode keyCode { get; }
		}

		/// <summary>
		///   <para>An item in a contextual menu.</para>
		/// </summary>
		public abstract class MenuItem
		{
		}

		/// <summary>
		///   <para>A separator menu item.</para>
		/// </summary>
		public class Separator : ContextualMenu.MenuItem
		{
			/// <summary>
			///   <para>Constructor.</para>
			/// </summary>
			/// <param name="subMenuPath">The path for the submenu. Path components are delimited by forward slashes ('/').</param>
			public Separator(string subMenuPath)
			{
				this.subMenuPath = subMenuPath;
			}

			/// <summary>
			///   <para>The submenu path where the separator will be added. Path components are delimited by forward slashes ('/').</para>
			/// </summary>
			public string subMenuPath;
		}

		/// <summary>
		///   <para>A menu action item.</para>
		/// </summary>
		public class MenuAction : ContextualMenu.MenuItem
		{
			public MenuAction(string actionName, Action<ContextualMenu.MenuAction> actionCallback, Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags> actionStatusCallback, object userData = null)
			{
				this.name = actionName;
				this.actionCallback = actionCallback;
				this.actionStatusCallback = actionStatusCallback;
				this.userData = userData;
			}

			/// <summary>
			///   <para>The status of the item.</para>
			/// </summary>
			public ContextualMenu.MenuAction.StatusFlags status { get; private set; }

			/// <summary>
			///   <para>Provides information on the event that triggered the contextual menu.</para>
			/// </summary>
			public ContextualMenu.EventInfo eventInfo { get; private set; }

			/// <summary>
			///   <para>The userData object stored by the constructor.</para>
			/// </summary>
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
				this.eventInfo = eventInfo;
				this.status = ((this.actionStatusCallback == null) ? ContextualMenu.MenuAction.StatusFlags.Hidden : this.actionStatusCallback(this));
			}

			/// <summary>
			///   <para>Execute the callback associated with this item.</para>
			/// </summary>
			public void Execute()
			{
				if (this.actionCallback != null)
				{
					this.actionCallback(this);
				}
			}

			/// <summary>
			///   <para>The name of the item. The name can be prefixed by its submenu path. Path components are delimited by forward slashes ('/').</para>
			/// </summary>
			public string name;

			private Action<ContextualMenu.MenuAction> actionCallback;

			private Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags> actionStatusCallback;

			/// <summary>
			///   <para>Status of the menu item.</para>
			/// </summary>
			[Flags]
			public enum StatusFlags
			{
				/// <summary>
				///   <para>The item is displayed normally.</para>
				/// </summary>
				Normal = 0,
				/// <summary>
				///   <para>The item is disabled and is not be selectable by the user.</para>
				/// </summary>
				Disabled = 1,
				/// <summary>
				///   <para>The item is displayed with a checkmark.</para>
				/// </summary>
				Checked = 2,
				/// <summary>
				///   <para>The item is not displayed.</para>
				/// </summary>
				Hidden = 4
			}
		}
	}
}
