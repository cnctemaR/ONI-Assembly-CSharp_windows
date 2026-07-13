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
				hashSet.UnionWith(this.InnerPrefixes.Select<Patch, string>((Patch p) => p.owner));
				hashSet.UnionWith(this.InnerPostfixes.Select<Patch, string>((Patch p) => p.owner));
				return hashSet.ToList<string>().AsReadOnly();
			}
		}

		public Patches(Patch[] prefixes, Patch[] postfixes, Patch[] transpilers, Patch[] finalizers, Patch[] innerprefixes, Patch[] innerpostfixes)
		{
			if (prefixes == null)
			{
				prefixes = Array.Empty<Patch>();
			}
			if (postfixes == null)
			{
				postfixes = Array.Empty<Patch>();
			}
			if (transpilers == null)
			{
				transpilers = Array.Empty<Patch>();
			}
			if (finalizers == null)
			{
				finalizers = Array.Empty<Patch>();
			}
			if (innerprefixes == null)
			{
				innerprefixes = Array.Empty<Patch>();
			}
			if (innerpostfixes == null)
			{
				innerpostfixes = Array.Empty<Patch>();
			}
			this.Prefixes = prefixes.ToList<Patch>().AsReadOnly();
			this.Postfixes = postfixes.ToList<Patch>().AsReadOnly();
			this.Transpilers = transpilers.ToList<Patch>().AsReadOnly();
			this.Finalizers = finalizers.ToList<Patch>().AsReadOnly();
			this.InnerPrefixes = innerprefixes.ToList<Patch>().AsReadOnly();
			this.InnerPostfixes = innerpostfixes.ToList<Patch>().AsReadOnly();
		}

		public readonly ReadOnlyCollection<Patch> Prefixes;

		public readonly ReadOnlyCollection<Patch> Postfixes;

		public readonly ReadOnlyCollection<Patch> Transpilers;

		public readonly ReadOnlyCollection<Patch> Finalizers;

		public readonly ReadOnlyCollection<Patch> InnerPrefixes;

		public readonly ReadOnlyCollection<Patch> InnerPostfixes;
	}
}
