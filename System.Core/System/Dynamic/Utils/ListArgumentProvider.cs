using System;
using System.Linq.Expressions;

namespace System.Dynamic.Utils
{
	internal sealed class ListArgumentProvider : ListProvider<Expression>
	{
		internal ListArgumentProvider(IArgumentProvider provider, Expression arg0)
		{
			this._provider = provider;
			this._arg0 = arg0;
		}

		protected override Expression First
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
				return this._provider.ArgumentCount;
			}
		}

		protected override Expression GetElement(int index)
		{
			return this._provider.GetArgument(index);
		}

		private readonly IArgumentProvider _provider;

		private readonly Expression _arg0;
	}
}
