using System;

namespace UnityEngine
{
	public sealed class ContextMenu : Attribute
	{
		public ContextMenu(string name)
		{
			this.m_ItemName = name;
		}

		public string menuItem
		{
			get
			{
				return this.m_ItemName;
			}
		}

		private string m_ItemName;
	}
}
