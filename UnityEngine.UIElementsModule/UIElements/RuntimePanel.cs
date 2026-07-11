using System;

namespace UnityEngine.UIElements
{
	internal class RuntimePanel : Panel
	{
		public RuntimePanel(ScriptableObject ownerObject, EventDispatcher dispatcher = null)
			: base(ownerObject, ContextType.Player, dispatcher)
		{
		}

		public override void Repaint(Event e)
		{
			bool flag = this.targetTexture == null;
			if (flag)
			{
				base.clearFlags = PanelClearFlags.Depth;
				base.Repaint(e);
			}
			else
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = this.targetTexture;
				base.clearFlags = PanelClearFlags.All;
				base.Repaint(e);
				RenderTexture.active = active;
			}
		}

		internal RenderTexture targetTexture = null;
	}
}
