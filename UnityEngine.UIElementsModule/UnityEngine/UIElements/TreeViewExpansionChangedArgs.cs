using System;

namespace UnityEngine.UIElements
{
	public class TreeViewExpansionChangedArgs
	{
		public int id { get; set; }

		public bool isExpanded { get; set; }

		public bool isAppliedToAllChildren { get; set; }
	}
}
