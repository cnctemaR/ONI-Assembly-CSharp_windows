using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harmony
{
	[Serializable]
	public class PatchInfo
	{
		public PatchInfo()
		{
			this.prefixes = new Patch[0];
			this.postfixes = new Patch[0];
			this.transpilers = new Patch[0];
		}

		public void AddPrefix(MethodInfo patch, string owner, int priority, string[] before, string[] after)
		{
			List<Patch> list = this.prefixes.ToList<Patch>();
			list.Add(new Patch(patch, this.prefixes.Count<Patch>() + 1, owner, priority, before, after));
			this.prefixes = list.ToArray();
		}

		public void RemovePrefix(string owner)
		{
			bool flag = owner == "*";
			if (flag)
			{
				this.prefixes = new Patch[0];
			}
			else
			{
				this.prefixes = this.prefixes.Where<Patch>((Patch patch) => patch.owner != owner).ToArray<Patch>();
			}
		}

		public void AddPostfix(MethodInfo patch, string owner, int priority, string[] before, string[] after)
		{
			List<Patch> list = this.postfixes.ToList<Patch>();
			list.Add(new Patch(patch, this.postfixes.Count<Patch>() + 1, owner, priority, before, after));
			this.postfixes = list.ToArray();
		}

		public void RemovePostfix(string owner)
		{
			bool flag = owner == "*";
			if (flag)
			{
				this.postfixes = new Patch[0];
			}
			else
			{
				this.postfixes = this.postfixes.Where<Patch>((Patch patch) => patch.owner != owner).ToArray<Patch>();
			}
		}

		public void AddTranspiler(MethodInfo patch, string owner, int priority, string[] before, string[] after)
		{
			List<Patch> list = this.transpilers.ToList<Patch>();
			list.Add(new Patch(patch, this.transpilers.Count<Patch>() + 1, owner, priority, before, after));
			this.transpilers = list.ToArray();
		}

		public void RemoveTranspiler(string owner)
		{
			bool flag = owner == "*";
			if (flag)
			{
				this.transpilers = new Patch[0];
			}
			else
			{
				this.transpilers = this.transpilers.Where<Patch>((Patch patch) => patch.owner != owner).ToArray<Patch>();
			}
		}

		public void RemovePatch(MethodInfo patch)
		{
			this.prefixes = this.prefixes.Where<Patch>((Patch p) => p.patch != patch).ToArray<Patch>();
			this.postfixes = this.postfixes.Where<Patch>((Patch p) => p.patch != patch).ToArray<Patch>();
			this.transpilers = this.transpilers.Where<Patch>((Patch p) => p.patch != patch).ToArray<Patch>();
		}

		public Patch[] prefixes;

		public Patch[] postfixes;

		public Patch[] transpilers;
	}
}
