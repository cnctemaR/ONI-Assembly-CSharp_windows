using System;

namespace System.ComponentModel
{
	public class RefreshEventArgs : EventArgs
	{
		public RefreshEventArgs(object componentChanged)
		{
			this.ComponentChanged = componentChanged;
			this.TypeChanged = componentChanged.GetType();
		}

		public RefreshEventArgs(Type typeChanged)
		{
			this.TypeChanged = typeChanged;
		}

		public object ComponentChanged { get; }

		public Type TypeChanged { get; }
	}
}
