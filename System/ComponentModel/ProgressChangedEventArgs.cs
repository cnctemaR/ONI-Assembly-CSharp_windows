using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class ProgressChangedEventArgs : EventArgs
	{
		public ProgressChangedEventArgs(int progressPercentage, object userState)
		{
			this.progressPercentage = progressPercentage;
			this.userState = userState;
		}

		[SRDescription("Percentage progress made in operation.")]
		public int ProgressPercentage
		{
			get
			{
				return this.progressPercentage;
			}
		}

		[SRDescription("User-supplied state to identify operation.")]
		public object UserState
		{
			get
			{
				return this.userState;
			}
		}

		private readonly int progressPercentage;

		private readonly object userState;
	}
}
