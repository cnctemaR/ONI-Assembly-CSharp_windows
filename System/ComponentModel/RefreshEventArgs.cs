using System;

namespace System.ComponentModel
{
	public class RefreshEventArgs : EventArgs
	{
		public RefreshEventArgs(object componentChanged)
		{
			if (componentChanged == null)
			{
				throw new ArgumentNullException("componentChanged");
			}
			this.component = componentChanged;
			this.type = this.component.GetType();
		}

		public RefreshEventArgs(Type typeChanged)
		{
			this.type = typeChanged;
		}

		public object ComponentChanged
		{
			get
			{
				return this.component;
			}
		}

		public Type TypeChanged
		{
			get
			{
				return this.type;
			}
		}

		private object component;

		private Type type;
	}
}
