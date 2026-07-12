using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace System.Linq.Expressions.Compiler
{
	internal sealed class CompilerScope
	{
		internal CompilerScope(object node, bool isMethod)
		{
			this.Node = node;
			this.IsMethod = isMethod;
			IReadOnlyList<ParameterExpression> variables = CompilerScope.GetVariables(node);
			this.Definitions = new Dictionary<ParameterExpression, VariableStorageKind>(variables.Count);
			foreach (ParameterExpression parameterExpression in variables)
			{
				this.Definitions.Add(parameterExpression, VariableStorageKind.Local);
			}
		}

		internal HoistedLocals NearestHoistedLocals
		{
			get
			{
				return this._hoistedLocals ?? this._closureHoistedLocals;
			}
		}

		internal CompilerScope Enter(LambdaCompiler lc, CompilerScope parent)
		{
			this.SetParent(lc, parent);
			this.AllocateLocals(lc);
			if (this.IsMethod && this._closureHoistedLocals != null)
			{
				this.EmitClosureAccess(lc, this._closureHoistedLocals);
			}
			this.EmitNewHoistedLocals(lc);
			if (this.IsMethod)
			{
				this.EmitCachedVariables();
			}
			return this;
		}

		internal CompilerScope Exit()
		{
			if (!this.IsMethod)
			{
				foreach (CompilerScope.Storage storage in this._locals.Values)
				{
					storage.FreeLocal();
				}
			}
			CompilerScope parent = this._parent;
			this._parent = null;
			this._hoistedLocals = null;
			this._closureHoistedLocals = null;
			this._locals.Clear();
			return parent;
		}

		internal void EmitVariableAccess(LambdaCompiler lc, ReadOnlyCollection<ParameterExpression> vars)
		{
			if (this.NearestHoistedLocals != null && vars.Count > 0)
			{
				global::System.Collections.Generic.ArrayBuilder<long> arrayBuilder = new global::System.Collections.Generic.ArrayBuilder<long>(vars.Count);
				foreach (ParameterExpression parameterExpression in vars)
				{
					ulong num = 0UL;
					HoistedLocals hoistedLocals = this.NearestHoistedLocals;
					while (!hoistedLocals.Indexes.ContainsKey(parameterExpression))
					{
						num += 1UL;
						hoistedLocals = hoistedLocals.Parent;
					}
					ulong num2 = (num << 32) | (ulong)hoistedLocals.Indexes[parameterExpression];
					arrayBuilder.UncheckedAdd((long)num2);
				}
				this.EmitGet(this.NearestHoistedLocals.SelfVariable);
				lc.EmitConstantArray<long>(arrayBuilder.ToArray());
				lc.IL.Emit(OpCodes.Call, CachedReflectionInfo.RuntimeOps_CreateRuntimeVariables_ObjectArray_Int64Array);
				return;
			}
			lc.IL.Emit(OpCodes.Call, CachedReflectionInfo.RuntimeOps_CreateRuntimeVariables);
		}

		internal void AddLocal(LambdaCompiler gen, ParameterExpression variable)
		{
			this._locals.Add(variable, new CompilerScope.LocalStorage(gen, variable));
		}

		internal void EmitGet(ParameterExpression variable)
		{
			this.ResolveVariable(variable).EmitLoad();
		}

		internal void EmitSet(ParameterExpression variable)
		{
			this.ResolveVariable(variable).EmitStore();
		}

		internal void EmitAddressOf(ParameterExpression variable)
		{
			this.ResolveVariable(variable).EmitAddress();
		}

		private CompilerScope.Storage ResolveVariable(ParameterExpression variable)
		{
			return this.ResolveVariable(variable, this.NearestHoistedLocals);
		}

		private CompilerScope.Storage ResolveVariable(ParameterExpression variable, HoistedLocals hoistedLocals)
		{
			for (CompilerScope compilerScope = this; compilerScope != null; compilerScope = compilerScope._parent)
			{
				CompilerScope.Storage storage;
				if (compilerScope._locals.TryGetValue(variable, out storage))
				{
					return storage;
				}
				if (compilerScope.IsMethod)
				{
					break;
				}
			}
			for (HoistedLocals hoistedLocals2 = hoistedLocals; hoistedLocals2 != null; hoistedLocals2 = hoistedLocals2.Parent)
			{
				int num;
				if (hoistedLocals2.Indexes.TryGetValue(variable, out num))
				{
					return new CompilerScope.ElementBoxStorage(this.ResolveVariable(hoistedLocals2.SelfVariable, hoistedLocals), num, variable);
				}
			}
			throw Error.UndefinedVariable(variable.Name, variable.Type, this.CurrentLambdaName);
		}

		private void SetParent(LambdaCompiler lc, CompilerScope parent)
		{
			this._parent = parent;
			if (this.NeedsClosure && this._parent != null)
			{
				this._closureHoistedLocals = this._parent.NearestHoistedLocals;
			}
			ReadOnlyCollection<ParameterExpression> readOnlyCollection = (from p in this.GetVariables()
				where this.Definitions[p] == VariableStorageKind.Hoisted
				select p).ToReadOnly<ParameterExpression>();
			if (readOnlyCollection.Count > 0)
			{
				this._hoistedLocals = new HoistedLocals(this._closureHoistedLocals, readOnlyCollection);
				this.AddLocal(lc, this._hoistedLocals.SelfVariable);
			}
		}

		private void EmitNewHoistedLocals(LambdaCompiler lc)
		{
			if (this._hoistedLocals == null)
			{
				return;
			}
			lc.IL.EmitPrimitive(this._hoistedLocals.Variables.Count);
			lc.IL.Emit(OpCodes.Newarr, typeof(object));
			int num = 0;
			foreach (ParameterExpression parameterExpression in this._hoistedLocals.Variables)
			{
				lc.IL.Emit(OpCodes.Dup);
				lc.IL.EmitPrimitive(num++);
				Type type = typeof(StrongBox<>).MakeGenericType(new Type[] { parameterExpression.Type });
				int num2;
				if (this.IsMethod && (num2 = lc.Parameters.IndexOf(parameterExpression)) >= 0)
				{
					lc.EmitLambdaArgument(num2);
					lc.IL.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { parameterExpression.Type }));
				}
				else if (parameterExpression == this._hoistedLocals.ParentVariable)
				{
					this.ResolveVariable(parameterExpression, this._closureHoistedLocals).EmitLoad();
					lc.IL.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { parameterExpression.Type }));
				}
				else
				{
					lc.IL.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
				}
				if (this.ShouldCache(parameterExpression))
				{
					lc.IL.Emit(OpCodes.Dup);
					this.CacheBoxToLocal(lc, parameterExpression);
				}
				lc.IL.Emit(OpCodes.Stelem_Ref);
			}
			this.EmitSet(this._hoistedLocals.SelfVariable);
		}

		private void EmitCachedVariables()
		{
			if (this.ReferenceCount == null)
			{
				return;
			}
			foreach (KeyValuePair<ParameterExpression, int> keyValuePair in this.ReferenceCount)
			{
				if (this.ShouldCache(keyValuePair.Key, keyValuePair.Value))
				{
					CompilerScope.ElementBoxStorage elementBoxStorage = this.ResolveVariable(keyValuePair.Key) as CompilerScope.ElementBoxStorage;
					if (elementBoxStorage != null)
					{
						elementBoxStorage.EmitLoadBox();
						this.CacheBoxToLocal(elementBoxStorage.Compiler, keyValuePair.Key);
					}
				}
			}
		}

		private bool ShouldCache(ParameterExpression v, int refCount)
		{
			return refCount > 2 && !this._locals.ContainsKey(v);
		}

		private bool ShouldCache(ParameterExpression v)
		{
			int num;
			return this.ReferenceCount != null && this.ReferenceCount.TryGetValue(v, out num) && this.ShouldCache(v, num);
		}

		private void CacheBoxToLocal(LambdaCompiler lc, ParameterExpression v)
		{
			CompilerScope.LocalBoxStorage localBoxStorage = new CompilerScope.LocalBoxStorage(lc, v);
			localBoxStorage.EmitStoreBox();
			this._locals.Add(v, localBoxStorage);
		}

		private void EmitClosureAccess(LambdaCompiler lc, HoistedLocals locals)
		{
			if (locals == null)
			{
				return;
			}
			this.EmitClosureToVariable(lc, locals);
			while ((locals = locals.Parent) != null)
			{
				ParameterExpression selfVariable = locals.SelfVariable;
				CompilerScope.LocalStorage localStorage = new CompilerScope.LocalStorage(lc, selfVariable);
				localStorage.EmitStore(this.ResolveVariable(selfVariable));
				this._locals.Add(selfVariable, localStorage);
			}
		}

		private void EmitClosureToVariable(LambdaCompiler lc, HoistedLocals locals)
		{
			lc.EmitClosureArgument();
			lc.IL.Emit(OpCodes.Ldfld, CachedReflectionInfo.Closure_Locals);
			this.AddLocal(lc, locals.SelfVariable);
			this.EmitSet(locals.SelfVariable);
		}

		private void AllocateLocals(LambdaCompiler lc)
		{
			foreach (ParameterExpression parameterExpression in this.GetVariables())
			{
				if (this.Definitions[parameterExpression] == VariableStorageKind.Local)
				{
					CompilerScope.Storage storage;
					if (this.IsMethod && lc.Parameters.Contains(parameterExpression))
					{
						storage = new CompilerScope.ArgumentStorage(lc, parameterExpression);
					}
					else
					{
						storage = new CompilerScope.LocalStorage(lc, parameterExpression);
					}
					this._locals.Add(parameterExpression, storage);
				}
			}
		}

		private IEnumerable<ParameterExpression> GetVariables()
		{
			if (this.MergedScopes != null)
			{
				return this.GetVariablesIncludingMerged();
			}
			return CompilerScope.GetVariables(this.Node);
		}

		private IEnumerable<ParameterExpression> GetVariablesIncludingMerged()
		{
			foreach (ParameterExpression parameterExpression in CompilerScope.GetVariables(this.Node))
			{
				yield return parameterExpression;
			}
			IEnumerator<ParameterExpression> enumerator = null;
			foreach (BlockExpression blockExpression in this.MergedScopes)
			{
				foreach (ParameterExpression parameterExpression2 in blockExpression.Variables)
				{
					yield return parameterExpression2;
				}
				enumerator = null;
			}
			HashSet<BlockExpression>.Enumerator enumerator2 = default(HashSet<BlockExpression>.Enumerator);
			yield break;
			yield break;
		}

		private static IReadOnlyList<ParameterExpression> GetVariables(object scope)
		{
			LambdaExpression lambdaExpression = scope as LambdaExpression;
			if (lambdaExpression != null)
			{
				return new ParameterList(lambdaExpression);
			}
			BlockExpression blockExpression = scope as BlockExpression;
			if (blockExpression != null)
			{
				return blockExpression.Variables;
			}
			return new ParameterExpression[] { ((CatchBlock)scope).Variable };
		}

		private string CurrentLambdaName
		{
			get
			{
				for (CompilerScope compilerScope = this; compilerScope != null; compilerScope = compilerScope._parent)
				{
					LambdaExpression lambdaExpression = compilerScope.Node as LambdaExpression;
					if (lambdaExpression != null)
					{
						return lambdaExpression.Name;
					}
				}
				throw ContractUtils.Unreachable;
			}
		}

		private CompilerScope _parent;

		internal readonly object Node;

		internal readonly bool IsMethod;

		internal bool NeedsClosure;

		internal readonly Dictionary<ParameterExpression, VariableStorageKind> Definitions = new Dictionary<ParameterExpression, VariableStorageKind>();

		internal Dictionary<ParameterExpression, int> ReferenceCount;

		internal HashSet<BlockExpression> MergedScopes;

		private HoistedLocals _hoistedLocals;

		private HoistedLocals _closureHoistedLocals;

		private readonly Dictionary<ParameterExpression, CompilerScope.Storage> _locals = new Dictionary<ParameterExpression, CompilerScope.Storage>();

		private abstract class Storage
		{
			internal Storage(LambdaCompiler compiler, ParameterExpression variable)
			{
				this.Compiler = compiler;
				this.Variable = variable;
			}

			internal abstract void EmitLoad();

			internal abstract void EmitAddress();

			internal abstract void EmitStore();

			internal virtual void EmitStore(CompilerScope.Storage value)
			{
				value.EmitLoad();
				this.EmitStore();
			}

			internal virtual void FreeLocal()
			{
			}

			internal readonly LambdaCompiler Compiler;

			internal readonly ParameterExpression Variable;
		}

		private sealed class LocalStorage : CompilerScope.Storage
		{
			internal LocalStorage(LambdaCompiler compiler, ParameterExpression variable)
				: base(compiler, variable)
			{
				this._local = compiler.GetLocal(variable.IsByRef ? variable.Type.MakeByRefType() : variable.Type);
			}

			internal override void EmitLoad()
			{
				this.Compiler.IL.Emit(OpCodes.Ldloc, this._local);
			}

			internal override void EmitStore()
			{
				this.Compiler.IL.Emit(OpCodes.Stloc, this._local);
			}

			internal override void EmitAddress()
			{
				this.Compiler.IL.Emit(OpCodes.Ldloca, this._local);
			}

			internal override void FreeLocal()
			{
				this.Compiler.FreeLocal(this._local);
			}

			private readonly LocalBuilder _local;
		}

		private sealed class ArgumentStorage : CompilerScope.Storage
		{
			internal ArgumentStorage(LambdaCompiler compiler, ParameterExpression p)
				: base(compiler, p)
			{
				this._argument = compiler.GetLambdaArgument(compiler.Parameters.IndexOf(p));
			}

			internal override void EmitLoad()
			{
				this.Compiler.IL.EmitLoadArg(this._argument);
			}

			internal override void EmitStore()
			{
				this.Compiler.IL.EmitStoreArg(this._argument);
			}

			internal override void EmitAddress()
			{
				this.Compiler.IL.EmitLoadArgAddress(this._argument);
			}

			private readonly int _argument;
		}

		private sealed class ElementBoxStorage : CompilerScope.Storage
		{
			internal ElementBoxStorage(CompilerScope.Storage array, int index, ParameterExpression variable)
				: base(array.Compiler, variable)
			{
				this._array = array;
				this._index = index;
				this._boxType = typeof(StrongBox<>).MakeGenericType(new Type[] { variable.Type });
				this._boxValueField = this._boxType.GetField("Value");
			}

			internal override void EmitLoad()
			{
				this.EmitLoadBox();
				this.Compiler.IL.Emit(OpCodes.Ldfld, this._boxValueField);
			}

			internal override void EmitStore()
			{
				LocalBuilder local = this.Compiler.GetLocal(this.Variable.Type);
				this.Compiler.IL.Emit(OpCodes.Stloc, local);
				this.EmitLoadBox();
				this.Compiler.IL.Emit(OpCodes.Ldloc, local);
				this.Compiler.FreeLocal(local);
				this.Compiler.IL.Emit(OpCodes.Stfld, this._boxValueField);
			}

			internal override void EmitStore(CompilerScope.Storage value)
			{
				this.EmitLoadBox();
				value.EmitLoad();
				this.Compiler.IL.Emit(OpCodes.Stfld, this._boxValueField);
			}

			internal override void EmitAddress()
			{
				this.EmitLoadBox();
				this.Compiler.IL.Emit(OpCodes.Ldflda, this._boxValueField);
			}

			internal void EmitLoadBox()
			{
				this._array.EmitLoad();
				this.Compiler.IL.EmitPrimitive(this._index);
				this.Compiler.IL.Emit(OpCodes.Ldelem_Ref);
				this.Compiler.IL.Emit(OpCodes.Castclass, this._boxType);
			}

			private readonly int _index;

			private readonly CompilerScope.Storage _array;

			private readonly Type _boxType;

			private readonly FieldInfo _boxValueField;
		}

		private sealed class LocalBoxStorage : CompilerScope.Storage
		{
			internal LocalBoxStorage(LambdaCompiler compiler, ParameterExpression variable)
				: base(compiler, variable)
			{
				Type type = typeof(StrongBox<>).MakeGenericType(new Type[] { variable.Type });
				this._boxValueField = type.GetField("Value");
				this._boxLocal = compiler.GetLocal(type);
			}

			internal override void EmitLoad()
			{
				this.Compiler.IL.Emit(OpCodes.Ldloc, this._boxLocal);
				this.Compiler.IL.Emit(OpCodes.Ldfld, this._boxValueField);
			}

			internal override void EmitAddress()
			{
				this.Compiler.IL.Emit(OpCodes.Ldloc, this._boxLocal);
				this.Compiler.IL.Emit(OpCodes.Ldflda, this._boxValueField);
			}

			internal override void EmitStore()
			{
				LocalBuilder local = this.Compiler.GetLocal(this.Variable.Type);
				this.Compiler.IL.Emit(OpCodes.Stloc, local);
				this.Compiler.IL.Emit(OpCodes.Ldloc, this._boxLocal);
				this.Compiler.IL.Emit(OpCodes.Ldloc, local);
				this.Compiler.FreeLocal(local);
				this.Compiler.IL.Emit(OpCodes.Stfld, this._boxValueField);
			}

			internal override void EmitStore(CompilerScope.Storage value)
			{
				this.Compiler.IL.Emit(OpCodes.Ldloc, this._boxLocal);
				value.EmitLoad();
				this.Compiler.IL.Emit(OpCodes.Stfld, this._boxValueField);
			}

			internal void EmitStoreBox()
			{
				this.Compiler.IL.Emit(OpCodes.Stloc, this._boxLocal);
			}

			internal override void FreeLocal()
			{
				this.Compiler.FreeLocal(this._boxLocal);
			}

			private readonly LocalBuilder _boxLocal;

			private readonly FieldInfo _boxValueField;
		}
	}
}
