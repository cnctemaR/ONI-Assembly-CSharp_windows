using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Expressions.Compiler
{
	internal sealed class ParameterList : IReadOnlyList<ParameterExpression>, IReadOnlyCollection<ParameterExpression>, IEnumerable<ParameterExpression>, IEnumerable
	{
		public ParameterList(IParameterProvider provider)
		{
			this._provider = provider;
		}

		public ParameterExpression this[int index]
		{
			get
			{
				return this._provider.GetParameter(index);
			}
		}

		public int Count
		{
			get
			{
				return this._provider.ParameterCount;
			}
		}

		public IEnumerator<ParameterExpression> GetEnumerator()
		{
			int i = 0;
			int j = this._provider.ParameterCount;
			while (i < j)
			{
				yield return this._provider.GetParameter(i);
				int num = i;
				i = num + 1;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly IParameterProvider _provider;
	}
}
