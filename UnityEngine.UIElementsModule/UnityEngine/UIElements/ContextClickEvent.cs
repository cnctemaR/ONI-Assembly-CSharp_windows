using System;

namespace UnityEngine.UIElements
{
	public class ContextClickEvent : MouseEventBase<ContextClickEvent>
	{
		static ContextClickEvent()
		{
			EventBase<ContextClickEvent>.SetCreateFunction(() => new ContextClickEvent());
		}

		public ContextClickEvent()
		{
			this.LocalInit();
		}

		protected override void Init()
		{
			base.Init();
			this.LocalInit();
		}

		private void LocalInit()
		{
		}
	}
}
