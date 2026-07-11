using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Harmony
{
	public class Patches
	{
		public ReadOnlyCollection<string> Owners
		{
			get
			{
				HashSet<string> hashSet = new HashSet<string>();
				hashSet.UnionWith(this.Prefixes.Select<Patch, string>((Patch p) => p.owner));
				hashSet.UnionWith(this.Postfixes.Select<Patch, string>((Patch p) => p.owner));
				hashSet.UnionWith(this.Transpilers.Select<Patch, string>((Patch p) => p.owner));
				return hashSet.ToList<string>().AsReadOnly();
			}
		}

		public Patches(Patch[] prefixes, Patch[] postfixes, Patch[] transpilers)
		{
			bool flag = prefixes == null;
			if (flag)
			{
				prefixes = new Patch[0];
			}
			bool flag2 = postfixes == null;
			if (flag2)
			{
				postfixes = new Patch[0];
			}
			bool flag3 = transpilers == null;
			if (flag3)
			{
				transpilers = new Patch[0];
			}
			this.Prefixes = prefixes.ToList<Patch>().AsReadOnly();
			this.Postfixes = postfixes.ToList<Patch>().AsReadOnly();
			this.Transpilers = transpilers.ToList<Patch>().AsReadOnly();
		}

		public readonly ReadOnlyCollection<Patch> Prefixes;

		public readonly ReadOnlyCollection<Patch> Postfixes;

		public readonly ReadOnlyCollection<Patch> Transpilers;
	}
}
