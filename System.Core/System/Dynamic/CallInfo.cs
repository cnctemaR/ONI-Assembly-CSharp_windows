using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Linq.Expressions;

namespace System.Dynamic
{
	public sealed class CallInfo
	{
		public CallInfo(int argCount, params string[] argNames)
			: this(argCount, argNames)
		{
		}

		public CallInfo(int argCount, IEnumerable<string> argNames)
		{
			ContractUtils.RequiresNotNull(argNames, "argNames");
			ReadOnlyCollection<string> readOnlyCollection = argNames.ToReadOnly<string>();
			if (argCount < readOnlyCollection.Count)
			{
				throw Error.ArgCntMustBeGreaterThanNameCnt();
			}
			ContractUtils.RequiresNotNullItems<string>(readOnlyCollection, "argNames");
			this.ArgumentCount = argCount;
			this.ArgumentNames = readOnlyCollection;
		}

		public int ArgumentCount { get; }

		public ReadOnlyCollection<string> ArgumentNames { get; }

		public override int GetHashCode()
		{
			return this.ArgumentCount ^ this.ArgumentNames.ListHashCode<string>();
		}

		public override bool Equals(object obj)
		{
			CallInfo callInfo = obj as CallInfo;
			return callInfo != null && this.ArgumentCount == callInfo.ArgumentCount && this.ArgumentNames.ListEquals<string>(callInfo.ArgumentNames);
		}
	}
}
