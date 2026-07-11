using System;
using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
	internal sealed class ListParameterProvider : ListProvider<ParameterExpression>
	{
		internal ListParameterProvider(IParameterProvider provider, ParameterExpression arg0)
		{
			this._provider = provider;
			this._arg0 = arg0;
		}

		protected override ParameterExpression First
		{
			get
			{
				return this._arg0;
			}
		}

		protected override int ElementCount
		{
			get
			{
				return this._provider.ParameterCount;
			}
		}

		protected override ParameterExpression GetElement(int index)
		{
			return this._provider.GetParameter(index);
		}

		private readonly IParameterProvider _provider;

		private readonly ParameterExpression _arg0;
	}
}
