using System;

namespace NodeEditorFramework
{
	public class NodeAttribute : Attribute
	{
		public NodeAttribute(bool HideNode, string ReplacedContextText, Type[] nodeCanvasTypes = null)
		{
			this.hide = HideNode;
			this.contextText = ReplacedContextText;
			Type[] array = nodeCanvasTypes;
			if (nodeCanvasTypes == null)
			{
				(array = new Type[1])[0] = typeof(NodeCanvas);
			}
			this.typeOfNodeCanvas = array;
		}

		public bool hide { get; private set; }

		public string contextText { get; private set; }

		public Type[] typeOfNodeCanvas;
	}
}
