using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements
{
	public class DropdownMenu
	{
		internal int Count
		{
			get
			{
				return this.m_MenuItems.Count;
			}
		}

		internal bool repaintPanelBeforeDisplay { get; set; }

		public bool allowDuplicateNames { get; set; }

		public List<DropdownMenuItem> MenuItems()
		{
			return this.m_MenuItems;
		}

		public void AppendAction(string actionName, Action<DropdownMenuAction> action, Func<DropdownMenuAction, DropdownMenuAction.Status> actionStatusCallback, object userData = null)
		{
			DropdownMenuAction dropdownMenuAction = new DropdownMenuAction(actionName, action, actionStatusCallback, userData);
			this.m_MenuItems.Add(dropdownMenuAction);
		}

		public void AppendAction(string actionName, Action<DropdownMenuAction> action, DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal)
		{
			bool flag = status == DropdownMenuAction.Status.Normal;
			if (flag)
			{
				this.AppendAction(actionName, action, new Func<DropdownMenuAction, DropdownMenuAction.Status>(DropdownMenuAction.AlwaysEnabled), null);
			}
			else
			{
				bool flag2 = status == DropdownMenuAction.Status.Disabled;
				if (flag2)
				{
					this.AppendAction(actionName, action, new Func<DropdownMenuAction, DropdownMenuAction.Status>(DropdownMenuAction.AlwaysDisabled), null);
				}
				else
				{
					this.AppendAction(actionName, action, (DropdownMenuAction e) => status, null);
				}
			}
		}

		public void InsertAction(int atIndex, string actionName, Action<DropdownMenuAction> action, Func<DropdownMenuAction, DropdownMenuAction.Status> actionStatusCallback, object userData = null)
		{
			DropdownMenuAction dropdownMenuAction = new DropdownMenuAction(actionName, action, actionStatusCallback, userData);
			this.m_MenuItems.Insert(atIndex, dropdownMenuAction);
		}

		public void InsertAction(int atIndex, string actionName, Action<DropdownMenuAction> action, DropdownMenuAction.Status status = DropdownMenuAction.Status.Normal)
		{
			bool flag = status == DropdownMenuAction.Status.Normal;
			if (flag)
			{
				this.InsertAction(atIndex, actionName, action, new Func<DropdownMenuAction, DropdownMenuAction.Status>(DropdownMenuAction.AlwaysEnabled), null);
			}
			else
			{
				bool flag2 = status == DropdownMenuAction.Status.Disabled;
				if (flag2)
				{
					this.InsertAction(atIndex, actionName, action, new Func<DropdownMenuAction, DropdownMenuAction.Status>(DropdownMenuAction.AlwaysDisabled), null);
				}
				else
				{
					this.InsertAction(atIndex, actionName, action, (DropdownMenuAction e) => status, null);
				}
			}
		}

		public void AppendSeparator(string subMenuPath = null)
		{
			if (subMenuPath == null)
			{
				subMenuPath = string.Empty;
			}
			bool flag = this.m_MenuItems.FindIndex(delegate(DropdownMenuItem item)
			{
				DropdownMenuAction dropdownMenuAction = item as DropdownMenuAction;
				return dropdownMenuAction != null && dropdownMenuAction.name.StartsWith(subMenuPath);
			}) == -1;
			bool flag2;
			if (this.m_MenuItems.Count > 0)
			{
				List<DropdownMenuItem> menuItems = this.m_MenuItems;
				if (menuItems[menuItems.Count - 1] is DropdownMenuSeparator)
				{
					List<DropdownMenuItem> menuItems2 = this.m_MenuItems;
					if (((DropdownMenuSeparator)menuItems2[menuItems2.Count - 1]).subMenuPath == subMenuPath)
					{
						goto IL_0094;
					}
				}
				flag2 = !flag;
				goto IL_0095;
			}
			IL_0094:
			flag2 = false;
			IL_0095:
			bool flag3 = flag2;
			if (flag3)
			{
				DropdownMenuSeparator dropdownMenuSeparator = new DropdownMenuSeparator(subMenuPath);
				this.m_MenuItems.Add(dropdownMenuSeparator);
			}
		}

		public void InsertSeparator(string subMenuPath, int atIndex)
		{
			bool flag = atIndex > 0 && atIndex <= this.m_MenuItems.Count && !(this.m_MenuItems[atIndex - 1] is DropdownMenuSeparator);
			if (flag)
			{
				DropdownMenuSeparator dropdownMenuSeparator = new DropdownMenuSeparator(subMenuPath ?? string.Empty);
				this.m_MenuItems.Insert(atIndex, dropdownMenuSeparator);
			}
		}

		public void RemoveItemAt(int index)
		{
			this.m_MenuItems.RemoveAt(index);
		}

		public void ClearItems()
		{
			this.m_MenuItems.Clear();
		}

		public void PrepareForDisplay(EventBase e)
		{
			this.m_DropdownMenuEventInfo = ((e != null) ? new DropdownMenuEventInfo(e) : null);
			bool flag = this.m_MenuItems.Count == 0;
			if (!flag)
			{
				foreach (DropdownMenuItem dropdownMenuItem in this.m_MenuItems)
				{
					DropdownMenuAction dropdownMenuAction = dropdownMenuItem as DropdownMenuAction;
					bool flag2 = dropdownMenuAction != null;
					if (flag2)
					{
						dropdownMenuAction.UpdateActionStatus(this.m_DropdownMenuEventInfo);
					}
				}
				bool flag3 = this.m_MenuItems[this.m_MenuItems.Count - 1] is DropdownMenuSeparator;
				if (flag3)
				{
					this.m_MenuItems.RemoveAt(this.m_MenuItems.Count - 1);
				}
			}
		}

		private List<DropdownMenuItem> m_MenuItems = new List<DropdownMenuItem>();

		private DropdownMenuEventInfo m_DropdownMenuEventInfo;
	}
}
