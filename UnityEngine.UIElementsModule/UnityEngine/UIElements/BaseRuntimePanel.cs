using System;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal abstract class BaseRuntimePanel : Panel, IRuntimePanel
	{
		protected BaseRuntimePanel(ScriptableObject ownerObject, EventDispatcher dispatcher = null)
			: base(ownerObject, ContextType.Player, dispatcher)
		{
		}

		internal override Shader standardWorldSpaceShader
		{
			get
			{
				return this.m_StandardWorldSpaceShader;
			}
			set
			{
				bool flag = this.m_StandardWorldSpaceShader != value;
				if (flag)
				{
					this.m_StandardWorldSpaceShader = value;
					base.InvokeStandardWorldSpaceShaderChanged();
				}
			}
		}

		internal bool drawToCameras
		{
			get
			{
				UIRRepaintUpdater uirrepaintUpdater = this.GetUpdater(VisualTreeUpdatePhase.Repaint) as UIRRepaintUpdater;
				bool flag;
				if (uirrepaintUpdater == null)
				{
					flag = false;
				}
				else
				{
					RenderChain renderChain = uirrepaintUpdater.renderChain;
					bool? flag2 = ((renderChain != null) ? new bool?(renderChain.drawInCameras) : null);
					bool flag3 = true;
					flag = (flag2.GetValueOrDefault() == flag3) & (flag2 != null);
				}
				return flag;
			}
			set
			{
				UIRRepaintUpdater uirrepaintUpdater = this.GetUpdater(VisualTreeUpdatePhase.Repaint) as UIRRepaintUpdater;
				RenderChain renderChain = ((uirrepaintUpdater != null) ? uirrepaintUpdater.renderChain : null);
				bool flag = renderChain != null;
				if (flag)
				{
					renderChain.drawInCameras = value;
				}
			}
		}

		public override void Repaint(Event e)
		{
			bool flag = this.targetTexture == null;
			if (flag)
			{
				RenderTexture active = RenderTexture.active;
				int num = ((active != null) ? active.width : Screen.width);
				int num2 = ((active != null) ? active.height : Screen.height);
				GL.Viewport(new Rect(0f, 0f, (float)num, (float)num2));
				base.clearFlags = PanelClearFlags.Depth;
				base.Repaint(e);
			}
			else
			{
				RenderTexture active2 = RenderTexture.active;
				RenderTexture.active = this.targetTexture;
				base.clearFlags = PanelClearFlags.All;
				base.Repaint(e);
				RenderTexture.active = active2;
			}
		}

		public Func<Vector2, Vector2> screenToPanelSpace
		{
			get
			{
				return this.m_ScreenToPanelSpace;
			}
			set
			{
				this.m_ScreenToPanelSpace = value ?? BaseRuntimePanel.DefaultScreenToPanelSpace;
			}
		}

		internal Vector2 ScreenToPanel(Vector2 screen)
		{
			return this.screenToPanelSpace(screen) / base.scale;
		}

		private Shader m_StandardWorldSpaceShader;

		internal RenderTexture targetTexture = null;

		internal Matrix4x4 panelToWorld = Matrix4x4.identity;

		internal static readonly Func<Vector2, Vector2> DefaultScreenToPanelSpace = (Vector2 p) => p;

		private Func<Vector2, Vector2> m_ScreenToPanelSpace = BaseRuntimePanel.DefaultScreenToPanelSpace;
	}
}
