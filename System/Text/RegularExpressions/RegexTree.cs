using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexTree
	{
		internal RegexTree(RegexNode root, Hashtable caps, int[] capNumList, int capTop, Hashtable capNames, string[] capsList, RegexOptions options)
		{
			this.Root = root;
			this.Caps = caps;
			this.CapNumList = capNumList;
			this.CapTop = capTop;
			this.CapNames = capNames;
			this.CapsList = capsList;
			this.Options = options;
		}

		public readonly RegexNode Root;

		public readonly Hashtable Caps;

		public readonly int[] CapNumList;

		public readonly int CapTop;

		public readonly Hashtable CapNames;

		public readonly string[] CapsList;

		public readonly RegexOptions Options;
	}
}
