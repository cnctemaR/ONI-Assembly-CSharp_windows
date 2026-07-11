using System;

namespace UnityEngine.UIElements
{
	public abstract class ImmediateModeElement : VisualElement
	{
		public ImmediateModeElement()
		{
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(this.OnGenerateVisualContent));
		}

		private void OnGenerateVisualContent(MeshGenerationContext mgc)
		{
			mgc.painter.DrawImmediate(new Action(this.ImmediateRepaint));
		}

		protected abstract void ImmediateRepaint();
	}
}
