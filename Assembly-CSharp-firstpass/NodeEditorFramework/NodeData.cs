using System;

namespace NodeEditorFramework
{
	public struct NodeData
	{
		public NodeData(string name, Type[] types)
		{
			this.adress = name;
			this.typeOfNodeCanvas = types;
		}

		public string adress;

		public Type[] typeOfNodeCanvas;
	}
}
