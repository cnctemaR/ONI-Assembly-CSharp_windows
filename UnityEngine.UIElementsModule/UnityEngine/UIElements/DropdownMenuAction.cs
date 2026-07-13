using System;

namespace UnityEngine.UIElements
{
	public class DropdownMenuAction : DropdownMenuItem
	{
		public string name { get; }

		public DropdownMenuAction.Status status { get; private set; }

		public DropdownMenuEventInfo eventInfo { get; private set; }

		public object userData { get; private set; }

		internal VisualElement content { get; }

		public static DropdownMenuAction.Status AlwaysEnabled(DropdownMenuAction a)
		{
			return DropdownMenuAction.Status.Normal;
		}

		public static DropdownMenuAction.Status AlwaysDisabled(DropdownMenuAction a)
		{
			return DropdownMenuAction.Status.Disabled;
		}

		public DropdownMenuAction(string actionName, Action<DropdownMenuAction> actionCallback, Func<DropdownMenuAction, DropdownMenuAction.Status> actionStatusCallback, object userData = null)
		{
			this.name = actionName;
			this.actionCallback = actionCallback;
			this.actionStatusCallback = actionStatusCallback;
			this.userData = userData;
		}

		public void UpdateActionStatus(DropdownMenuEventInfo eventInfo)
		{
			this.eventInfo = eventInfo;
			Func<DropdownMenuAction, DropdownMenuAction.Status> func = this.actionStatusCallback;
			this.status = ((func != null) ? func(this) : DropdownMenuAction.Status.Hidden);
		}

		public void Execute()
		{
			Action<DropdownMenuAction> action = this.actionCallback;
			if (action != null)
			{
				action(this);
			}
		}

		private readonly Action<DropdownMenuAction> actionCallback;

		private readonly Func<DropdownMenuAction, DropdownMenuAction.Status> actionStatusCallback;

		[Flags]
		public enum Status
		{
			None = 0,
			Normal = 1,
			Disabled = 2,
			Checked = 4,
			Hidden = 8
		}
	}
}
