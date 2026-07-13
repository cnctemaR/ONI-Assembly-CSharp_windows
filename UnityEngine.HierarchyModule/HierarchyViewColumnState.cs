using System;

namespace Unity.Hierarchy
{
	[Serializable]
	internal sealed class HierarchyViewColumnState
	{
		public override string ToString()
		{
			return string.Format("{0} Visible:{1} Index:{2} Width:{3}", new object[] { this.ColumnId, this.Visible, this.Index, this.Width });
		}

		public string ColumnId;

		public bool Visible;

		public float Width;

		public int Index = -1;
	}
}
