using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Dynamic.Utils
{
	internal static class ExpressionUtils
	{
		public static ReadOnlyCollection<ParameterExpression> ReturnReadOnly(IParameterProvider provider, ref object collection)
		{
			ParameterExpression parameterExpression = collection as ParameterExpression;
			if (parameterExpression != null)
			{
				Interlocked.CompareExchange(ref collection, new ReadOnlyCollection<ParameterExpression>(new ListParameterProvider(provider, parameterExpression)), parameterExpression);
			}
			return (ReadOnlyCollection<ParameterExpression>)collection;
		}

		public static ReadOnlyCollection<T> ReturnReadOnly<T>(ref IReadOnlyList<T> collection)
		{
			IReadOnlyList<T> readOnlyList = collection;
			ReadOnlyCollection<T> readOnlyCollection = readOnlyList as ReadOnlyCollection<T>;
			if (readOnlyCollection != null)
			{
				return readOnlyCollection;
			}
			Interlocked.CompareExchange<IReadOnlyList<T>>(ref collection, readOnlyList.ToReadOnly<T>(), readOnlyList);
			return (ReadOnlyCollection<T>)collection;
		}

		public static ReadOnlyCollection<Expression> ReturnReadOnly(IArgumentProvider provider, ref object collection)
		{
			Expression expression = collection as Expression;
			if (expression != null)
			{
				Interlocked.CompareExchange(ref collection, new ReadOnlyCollection<Expression>(new ListArgumentProvider(provider, expression)), expression);
			}
			return (ReadOnlyCollection<Expression>)collection;
		}

		public static T ReturnObject<T>(object collectionOrT) where T : class
		{
			T t = collectionOrT as T;
			if (t != null)
			{
				return t;
			}
			return ((ReadOnlyCollection<T>)collectionOrT)[0];
		}

		public static void ValidateArgumentTypes(MethodBase method, ExpressionType nodeKind, ref ReadOnlyCollection<Expression> arguments, string methodParamName)
		{
			ParameterInfo[] parametersForValidation = ExpressionUtils.GetParametersForValidation(method, nodeKind);
			ExpressionUtils.ValidateArgumentCount(method, nodeKind, arguments.Count, parametersForValidation);
			Expression[] array = null;
			int i = 0;
			int num = parametersForValidation.Length;
			while (i < num)
			{
				Expression expression = arguments[i];
				ParameterInfo parameterInfo = parametersForValidation[i];
				expression = ExpressionUtils.ValidateOneArgument(method, nodeKind, expression, parameterInfo, methodParamName, "arguments", i);
				if (array == null && expression != arguments[i])
				{
					array = new Expression[arguments.Count];
					for (int j = 0; j < i; j++)
					{
						array[j] = arguments[j];
					}
				}
				if (array != null)
				{
					array[i] = expression;
				}
				i++;
			}
			if (array != null)
			{
				arguments = new TrueReadOnlyCollection<Expression>(array);
			}
		}

		public static void ValidateArgumentCount(MethodBase method, ExpressionType nodeKind, int count, ParameterInfo[] pis)
		{
			if (pis.Length != count)
			{
				if (nodeKind <= ExpressionType.Invoke)
				{
					if (nodeKind != ExpressionType.Call)
					{
						if (nodeKind != ExpressionType.Invoke)
						{
							goto IL_003A;
						}
						throw Error.IncorrectNumberOfLambdaArguments();
					}
				}
				else
				{
					if (nodeKind == ExpressionType.New)
					{
						throw Error.IncorrectNumberOfConstructorArguments();
					}
					if (nodeKind != ExpressionType.Dynamic)
					{
						goto IL_003A;
					}
				}
				throw Error.IncorrectNumberOfMethodCallArguments(method, "method");
				IL_003A:
				throw ContractUtils.Unreachable;
			}
		}

		public static Expression ValidateOneArgument(MethodBase method, ExpressionType nodeKind, Expression arguments, ParameterInfo pi, string methodParamName, string argumentParamName, int index = -1)
		{
			ExpressionUtils.RequiresCanRead(arguments, argumentParamName, index);
			Type type = pi.ParameterType;
			if (type.IsByRef)
			{
				type = type.GetElementType();
			}
			TypeUtils.ValidateType(type, methodParamName, true, true);
			if (!TypeUtils.AreReferenceAssignable(type, arguments.Type) && !ExpressionUtils.TryQuote(type, ref arguments))
			{
				if (nodeKind <= ExpressionType.Invoke)
				{
					if (nodeKind != ExpressionType.Call)
					{
						if (nodeKind != ExpressionType.Invoke)
						{
							goto IL_0092;
						}
						throw Error.ExpressionTypeDoesNotMatchParameter(arguments.Type, type, argumentParamName, index);
					}
				}
				else
				{
					if (nodeKind == ExpressionType.New)
					{
						throw Error.ExpressionTypeDoesNotMatchConstructorParameter(arguments.Type, type, argumentParamName, index);
					}
					if (nodeKind != ExpressionType.Dynamic)
					{
						goto IL_0092;
					}
				}
				throw Error.ExpressionTypeDoesNotMatchMethodParameter(arguments.Type, type, method, argumentParamName, index);
				IL_0092:
				throw ContractUtils.Unreachable;
			}
			return arguments;
		}

		public static void RequiresCanRead(Expression expression, string paramName)
		{
			ExpressionUtils.RequiresCanRead(expression, paramName, -1);
		}

		public static void RequiresCanRead(Expression expression, string paramName, int idx)
		{
			ContractUtils.RequiresNotNull(expression, paramName, idx);
			ExpressionType nodeType = expression.NodeType;
			if (nodeType != ExpressionType.MemberAccess)
			{
				if (nodeType == ExpressionType.Index)
				{
					IndexExpression indexExpression = (IndexExpression)expression;
					if (indexExpression.Indexer != null && !indexExpression.Indexer.CanRead)
					{
						throw Error.ExpressionMustBeReadable(paramName, idx);
					}
				}
			}
			else
			{
				PropertyInfo propertyInfo = ((MemberExpression)expression).Member as PropertyInfo;
				if (propertyInfo != null && !propertyInfo.CanRead)
				{
					throw Error.ExpressionMustBeReadable(paramName, idx);
				}
			}
		}

		public static bool TryQuote(Type parameterType, ref Expression argument)
		{
			if (TypeUtils.IsSameOrSubclass(typeof(LambdaExpression), parameterType) && parameterType.IsInstanceOfType(argument))
			{
				argument = Expression.Quote(argument);
				return true;
			}
			return false;
		}

		internal static ParameterInfo[] GetParametersForValidation(MethodBase method, ExpressionType nodeKind)
		{
			ParameterInfo[] array = method.GetParametersCached();
			if (nodeKind == ExpressionType.Dynamic)
			{
				array = array.RemoveFirst<ParameterInfo>();
			}
			return array;
		}

		internal static bool SameElements<T>(ICollection<T> replacement, IReadOnlyList<T> current) where T : class
		{
			if (replacement == current)
			{
				return true;
			}
			if (replacement == null)
			{
				return current.Count == 0;
			}
			return ExpressionUtils.SameElementsInCollection<T>(replacement, current);
		}

		internal static bool SameElements<T>(ref IEnumerable<T> replacement, IReadOnlyList<T> current) where T : class
		{
			if (replacement == current)
			{
				return true;
			}
			if (replacement == null)
			{
				return current.Count == 0;
			}
			ICollection<T> collection = replacement as ICollection<T>;
			if (collection == null)
			{
				replacement = (collection = replacement.ToReadOnly<T>());
			}
			return ExpressionUtils.SameElementsInCollection<T>(collection, current);
		}

		private static bool SameElementsInCollection<T>(ICollection<T> replacement, IReadOnlyList<T> current) where T : class
		{
			int count = current.Count;
			if (replacement.Count != count)
			{
				return false;
			}
			if (count != 0)
			{
				int num = 0;
				using (IEnumerator<T> enumerator = replacement.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current != current[num])
						{
							return false;
						}
						num++;
					}
				}
				return true;
			}
			return true;
		}
	}
}
