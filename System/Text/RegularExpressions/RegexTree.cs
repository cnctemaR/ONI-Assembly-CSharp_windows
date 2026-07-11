using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	internal sealed class RegexTree
	{
		internal RegexTree(RegexNode root, Hashtable caps, int[] capnumlist, int captop, Hashtable capnames, string[] capslist, RegexOptions opts)
		{
			this._root = root;
			this._caps = caps;
			this._capnumlist = capnumlist;
			this._capnames = capnames;
			this._capslist = capslist;
			this._captop = captop;
			this._options = opts;
		}

		internal RegexNode _root;

		internal Hashtable _caps;

		internal int[] _capnumlist;

		internal Hashtable _capnames;

		internal string[] _capslist;

		internal RegexOptions _options;

		internal int _captop;
	}
}
