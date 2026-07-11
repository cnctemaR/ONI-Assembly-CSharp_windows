using System;
using System.Collections;

namespace System.Text.RegularExpressions
{
	internal sealed class CachedCodeEntry
	{
		internal CachedCodeEntry(string key, Hashtable capnames, string[] capslist, RegexCode code, Hashtable caps, int capsize, ExclusiveReference runner, SharedReference repl)
		{
			this._key = key;
			this._capnames = capnames;
			this._capslist = capslist;
			this._code = code;
			this._caps = caps;
			this._capsize = capsize;
			this._runnerref = runner;
			this._replref = repl;
		}

		internal void AddCompiled(RegexRunnerFactory factory)
		{
			this._factory = factory;
			this._code = null;
		}

		internal string _key;

		internal RegexCode _code;

		internal Hashtable _caps;

		internal Hashtable _capnames;

		internal string[] _capslist;

		internal int _capsize;

		internal RegexRunnerFactory _factory;

		internal ExclusiveReference _runnerref;

		internal SharedReference _replref;
	}
}
