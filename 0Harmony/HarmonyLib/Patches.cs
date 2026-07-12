using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HarmonyLib
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
				hashSet.UnionWith(this.Finalizers.Select<Patch, string>((Patch p) => p.owner));
				return hashSet.ToList<string>().AsReadOnly();
			}
		}

		public Patches(Patch[] prefixes, Patch[] postfixes, Patch[] transpilers, Patch[] finalizers)
		{
			if (prefixes == null)
			{
				prefixes = new Patch[0];
			}
			if (postfixes == null)
			{
				postfixes = new Patch[0];
			}
			if (transpilers == null)
			{
				transpilers = new Patch[0];
			}
			if (finalizers == null)
			{
				finalizers = new Patch[0];
			}
			this.Prefixes = prefixes.ToList<Patch>().AsReadOnly();
			this.Postfixes = postfixes.ToList<Patch>().AsReadOnly();
			this.Transpilers = transpilers.ToList<Patch>().AsReadOnly();
			this.Finalizers = finalizers.ToList<Patch>().AsReadOnly();
		}

		public readonly ReadOnlyCollection<Patch> Prefixes;

		public readonly ReadOnlyCollection<Patch> Postfixes;

		public readonly ReadOnlyCollection<Patch> Transpilers;

		public readonly ReadOnlyCollection<Patch> Finalizers;
	}
}
