using System;
using UnityEngine.UIElements.UIR;

namespace UnityEngine.UIElements
{
	internal abstract class AtlasBase
	{
		public virtual bool TryGetAtlas(VisualElement ctx, Texture2D src, out TextureId atlas, out RectInt atlasRect)
		{
			atlas = TextureId.invalid;
			atlasRect = default(RectInt);
			return false;
		}

		public virtual void ReturnAtlas(VisualElement ctx, Texture2D src, TextureId atlas)
		{
		}

		public virtual void Reset()
		{
		}

		protected virtual void OnAssignedToPanel(IPanel panel)
		{
		}

		protected virtual void OnRemovedFromPanel(IPanel panel)
		{
		}

		protected virtual void OnUpdateDynamicTextures(IPanel panel)
		{
		}

		internal void InvokeAssignedToPanel(IPanel panel)
		{
			this.OnAssignedToPanel(panel);
		}

		internal void InvokeRemovedFromPanel(IPanel panel)
		{
			this.OnRemovedFromPanel(panel);
		}

		internal void InvokeUpdateDynamicTextures(IPanel panel)
		{
			this.OnUpdateDynamicTextures(panel);
		}

		protected static void RepaintTexturedElements(IPanel panel)
		{
			Panel panel2 = panel as Panel;
			UIRRepaintUpdater uirrepaintUpdater = ((panel2 != null) ? panel2.GetUpdater(VisualTreeUpdatePhase.Repaint) : null) as UIRRepaintUpdater;
			if (uirrepaintUpdater != null)
			{
				RenderTreeManager renderTreeManager = uirrepaintUpdater.renderTreeManager;
				if (renderTreeManager != null)
				{
					renderTreeManager.RepaintTexturedElements();
				}
			}
		}

		protected TextureId AllocateDynamicTexture()
		{
			return this.textureRegistry.AllocAndAcquireDynamic();
		}

		protected void FreeDynamicTexture(TextureId id)
		{
			this.textureRegistry.Release(id);
		}

		protected void SetDynamicTexture(TextureId id, Texture texture)
		{
			this.textureRegistry.UpdateDynamic(id, texture);
		}

		internal TextureRegistry textureRegistry = TextureRegistry.instance;
	}
}
