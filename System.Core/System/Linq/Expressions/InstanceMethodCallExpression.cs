using System;
using System.Reflection;

namespace System.Linq.Expressions
{
	internal class InstanceMethodCallExpression : MethodCallExpression, IArgumentProvider
	{
		public InstanceMethodCallExpression(MethodInfo method, Expression instance)
			: base(method)
		{
			this._instance = instance;
		}

		internal override Expression GetInstance()
		{
			return this._instance;
		}

		private readonly Expression _instance;
	}
}
