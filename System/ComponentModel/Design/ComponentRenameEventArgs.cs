using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class ComponentRenameEventArgs : EventArgs
	{
		public ComponentRenameEventArgs(object component, string oldName, string newName)
		{
			this.component = component;
			this.oldName = oldName;
			this.newName = newName;
		}

		public object Component
		{
			get
			{
				return this.component;
			}
		}

		public virtual string NewName
		{
			get
			{
				return this.newName;
			}
		}

		public virtual string OldName
		{
			get
			{
				return this.oldName;
			}
		}

		private object component;

		private string oldName;

		private string newName;
	}
}
