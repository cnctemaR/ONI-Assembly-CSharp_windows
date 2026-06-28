using System;

namespace NodeEditorFramework
{
	public class NodeCanvasTypeAttribute : Attribute
	{
		public NodeCanvasTypeAttribute(string displayName)
		{
			this.Name = displayName;
		}

		public string Name;
	}
}
