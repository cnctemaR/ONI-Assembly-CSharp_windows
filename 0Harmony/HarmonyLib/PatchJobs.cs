using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HarmonyLib
{
	internal class PatchJobs<T>
	{
		internal PatchJobs<T>.Job GetJob(MethodBase method)
		{
			if (method == null)
			{
				return null;
			}
			PatchJobs<T>.Job job;
			if (!this.state.TryGetValue(method, out job))
			{
				job = new PatchJobs<T>.Job
				{
					original = method
				};
				this.state[method] = job;
			}
			return job;
		}

		internal List<PatchJobs<T>.Job> GetJobs()
		{
			return this.state.Values.Where<PatchJobs<T>.Job>((PatchJobs<T>.Job job) => job.prefixes.Count + job.postfixes.Count + job.transpilers.Count + job.finalizers.Count + job.innerprefixes.Count + job.innerpostfixes.Count > 0).ToList<PatchJobs<T>.Job>();
		}

		internal List<T> GetReplacements()
		{
			return this.state.Values.Select<PatchJobs<T>.Job, T>((PatchJobs<T>.Job job) => job.replacement).ToList<T>();
		}

		internal Dictionary<MethodBase, PatchJobs<T>.Job> state = new Dictionary<MethodBase, PatchJobs<T>.Job>();

		internal class Job
		{
			internal void AddPatch(AttributePatch patch)
			{
				HarmonyPatchType? type = patch.type;
				if (type != null)
				{
					switch (type.GetValueOrDefault())
					{
					case HarmonyPatchType.Prefix:
						this.prefixes.Add(patch.info);
						return;
					case HarmonyPatchType.Postfix:
						this.postfixes.Add(patch.info);
						return;
					case HarmonyPatchType.Transpiler:
						this.transpilers.Add(patch.info);
						return;
					case HarmonyPatchType.Finalizer:
						this.finalizers.Add(patch.info);
						return;
					case HarmonyPatchType.ReversePatch:
						break;
					case HarmonyPatchType.InnerPrefix:
						this.innerprefixes.Add(patch.info);
						return;
					case HarmonyPatchType.InnerPostfix:
						this.innerpostfixes.Add(patch.info);
						break;
					default:
						return;
					}
				}
			}

			internal MethodBase original;

			internal T replacement;

			internal List<HarmonyMethod> prefixes = new List<HarmonyMethod>();

			internal List<HarmonyMethod> postfixes = new List<HarmonyMethod>();

			internal List<HarmonyMethod> transpilers = new List<HarmonyMethod>();

			internal List<HarmonyMethod> finalizers = new List<HarmonyMethod>();

			internal List<HarmonyMethod> innerprefixes = new List<HarmonyMethod>();

			internal List<HarmonyMethod> innerpostfixes = new List<HarmonyMethod>();
		}
	}
}
