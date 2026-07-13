using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using MonoMod.Utils;

namespace HarmonyLib
{
	internal class MethodCreatorConfig
	{
		internal MethodCreatorConfig(MethodBase original, MethodBase source, List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers, List<MethodInfo> finalizers, List<Infix> innerprefixes, List<Infix> innerpostfixes, bool debug)
		{
			this.original = original;
			this.source = source;
			this.prefixes = prefixes;
			this.postfixes = postfixes;
			this.transpilers = transpilers;
			this.finalizers = finalizers;
			this.innerprefixes = innerprefixes;
			this.innerpostfixes = innerpostfixes;
			this.debug = debug;
		}

		internal bool Prepare()
		{
			PatchInfo patchInfo = HarmonySharedState.GetPatchInfo(this.original) ?? new PatchInfo();
			this.patchIndex = patchInfo.VersionCount + 1;
			MethodBase methodBase = this.original;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
			defaultInterpolatedStringHandler.AppendLiteral("_Patch");
			defaultInterpolatedStringHandler.AppendFormatted<int>(this.patchIndex);
			this.patch = MethodPatcherTools.CreateDynamicMethod(methodBase, defaultInterpolatedStringHandler.ToStringAndClear(), this.debug);
			if (this.patch == null)
			{
				return false;
			}
			this.injections = this.Fixes.Union<MethodInfo>(this.InnerFixes.Select<Infix, MethodInfo>((Infix fix) => fix.OuterMethod)).ToDictionary<MethodInfo, MethodInfo, List<InjectedParameter>>((MethodInfo fix) => fix, (MethodInfo fix) => (from p in fix.GetParameters()
				select new InjectedParameter(fix, p)).ToList<InjectedParameter>());
			this.returnType = AccessTools.GetReturnedType(this.original);
			this.il = this.patch.GetILGenerator();
			this.instructions = new List<CodeInstruction>();
			return true;
		}

		internal void AddCode(CodeInstruction code)
		{
			this.instructions.Add(code);
		}

		internal void AddCodes(IEnumerable<CodeInstruction> codes)
		{
			this.instructions.AddRange(codes);
		}

		internal void AddLocal(InjectionType type, LocalBuilder local)
		{
			this.localVariables.Add(type, local);
		}

		internal void AddLocal(string name, LocalBuilder local)
		{
			this.localVariables.Add(name, local);
		}

		internal LocalBuilder GetLocal(InjectionType type)
		{
			return this.localVariables[type];
		}

		internal LocalBuilder GetLocal(string name)
		{
			return this.localVariables[name];
		}

		internal bool HasLocal(string name)
		{
			LocalBuilder localBuilder;
			return this.localVariables.TryGetValue(name, out localBuilder);
		}

		internal LocalBuilder DeclareLocal(Type type, bool isPinned = false)
		{
			return this.il.DeclareLocal(type, isPinned);
		}

		internal Label DefineLabel()
		{
			return this.il.DefineLabel();
		}

		internal MethodBase MethodBase
		{
			get
			{
				return this.source ?? this.original;
			}
		}

		internal bool OriginalIsStatic
		{
			get
			{
				return this.original.IsStatic;
			}
		}

		internal IEnumerable<MethodInfo> Fixes
		{
			get
			{
				return this.prefixes.Union<MethodInfo>(this.postfixes).Union<MethodInfo>(this.finalizers);
			}
		}

		internal IEnumerable<Infix> InnerFixes
		{
			get
			{
				return this.innerprefixes.Union<Infix>(this.innerpostfixes);
			}
		}

		internal IEnumerable<InjectedParameter> InjectionsFor(MethodInfo fix, InjectionType type = InjectionType.Unknown)
		{
			List<InjectedParameter> list;
			if (!this.injections.TryGetValue(fix, out list))
			{
				return Array.Empty<InjectedParameter>();
			}
			if (type != InjectionType.Unknown)
			{
				return list.Where<InjectedParameter>((InjectedParameter pair) => pair.injectionType == type);
			}
			return list;
		}

		internal bool AnyFixHas(InjectionType type)
		{
			return this.injections.Values.SelectMany<List<InjectedParameter>, InjectedParameter>((List<InjectedParameter> list) => list).Any<InjectedParameter>((InjectedParameter pair) => pair.injectionType == type);
		}

		internal void WithFixes(Action<MethodInfo> action)
		{
			foreach (MethodInfo methodInfo in this.Fixes)
			{
				action(methodInfo);
			}
			foreach (Infix infix in this.InnerFixes)
			{
				action(infix.OuterMethod);
			}
		}

		internal readonly MethodBase original;

		internal readonly MethodBase source;

		internal readonly List<MethodInfo> prefixes;

		internal readonly List<MethodInfo> postfixes;

		internal readonly List<MethodInfo> transpilers;

		internal readonly List<MethodInfo> finalizers;

		internal readonly List<Infix> innerprefixes;

		internal readonly List<Infix> innerpostfixes;

		internal readonly bool debug;

		internal int patchIndex;

		internal DynamicMethodDefinition patch;

		internal Dictionary<MethodInfo, List<InjectedParameter>> injections;

		internal Type returnType;

		internal ILGenerator il;

		internal List<CodeInstruction> instructions;

		internal LocalBuilder[] originalVariables;

		internal VariableState localVariables;

		internal LocalBuilder resultVariable;

		internal Label? skipOriginalLabel;

		internal LocalBuilder runOriginalVariable;

		internal LocalBuilder exceptionVariable;

		internal LocalBuilder finalizedVariable;
	}
}
