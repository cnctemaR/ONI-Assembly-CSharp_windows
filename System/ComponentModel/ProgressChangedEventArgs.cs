using System;

namespace System.ComponentModel
{
	public class ProgressChangedEventArgs : EventArgs
	{
		public ProgressChangedEventArgs(int progressPercentage, object userState)
		{
			this.progress = progressPercentage;
			this.state = userState;
		}

		public int ProgressPercentage
		{
			get
			{
				return this.progress;
			}
		}

		public object UserState
		{
			get
			{
				return this.state;
			}
		}

		private int progress;

		private object state;
	}
}
