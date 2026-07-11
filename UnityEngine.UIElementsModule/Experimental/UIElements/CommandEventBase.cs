using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Base class for command events.</para>
	/// </summary>
	public abstract class CommandEventBase<T> : EventBase<T>, ICommandEvent, IPropagatableEvent where T : CommandEventBase<T>, new()
	{
		protected CommandEventBase()
		{
			this.Init();
		}

		public string commandName
		{
			get
			{
				string text;
				if (this.m_CommandName == null && base.imguiEvent != null)
				{
					text = base.imguiEvent.commandName;
				}
				else
				{
					text = this.m_CommandName;
				}
				return text;
			}
			protected set
			{
				this.m_CommandName = value;
			}
		}

		protected override void Init()
		{
			base.Init();
			base.flags = EventBase.EventFlags.Bubbles | EventBase.EventFlags.Capturable | EventBase.EventFlags.Cancellable;
			this.commandName = null;
		}

		public static T GetPooled(Event systemEvent)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.imguiEvent = systemEvent;
			return pooled;
		}

		public static T GetPooled(string commandName)
		{
			T pooled = EventBase<T>.GetPooled();
			pooled.commandName = commandName;
			return pooled;
		}

		private string m_CommandName;
	}
}
