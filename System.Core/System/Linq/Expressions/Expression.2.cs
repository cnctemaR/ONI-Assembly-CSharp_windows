using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace System.Linq.Expressions
{
	public abstract class Expression
	{
		protected Expression(ExpressionType node_type, Type type)
		{
			this.node_type = node_type;
			this.type = type;
		}

		public ExpressionType NodeType
		{
			get
			{
				return this.node_type;
			}
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		public override string ToString()
		{
			return ExpressionPrinter.ToString(this);
		}

		private static MethodInfo GetUnaryOperator(string oper_name, Type declaring, Type param)
		{
			return Expression.GetUnaryOperator(oper_name, declaring, param, null);
		}

		private static MethodInfo GetUnaryOperator(string oper_name, Type declaring, Type param, Type ret)
		{
			MethodInfo[] methods = declaring.GetNotNullableType().GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			foreach (MethodInfo methodInfo in methods)
			{
				if (!(methodInfo.Name != oper_name))
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 1)
					{
						if (!methodInfo.IsGenericMethod)
						{
							if (Expression.IsAssignableToParameterType(param.GetNotNullableType(), parameters[0]))
							{
								if (ret == null || methodInfo.ReturnType == ret.GetNotNullableType())
								{
									return methodInfo;
								}
							}
						}
					}
				}
			}
			return null;
		}

		internal static MethodInfo GetTrueOperator(Type self)
		{
			return Expression.GetBooleanOperator("op_True", self);
		}

		internal static MethodInfo GetFalseOperator(Type self)
		{
			return Expression.GetBooleanOperator("op_False", self);
		}

		private static MethodInfo GetBooleanOperator(string op, Type self)
		{
			return Expression.GetUnaryOperator(op, self, self, typeof(bool));
		}

		private static bool IsAssignableToParameterType(Type type, ParameterInfo param)
		{
			Type type2 = param.ParameterType;
			if (type2.IsByRef)
			{
				type2 = type2.GetElementType();
			}
			return type.GetNotNullableType().IsAssignableTo(type2);
		}

		private static MethodInfo CheckUnaryMethod(MethodInfo method, Type param)
		{
			if (method.ReturnType == typeof(void))
			{
				throw new ArgumentException("Specified method must return a value", "method");
			}
			if (!method.IsStatic)
			{
				throw new ArgumentException("Method must be static", "method");
			}
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length != 1)
			{
				throw new ArgumentException("Must have only one parameters", "method");
			}
			if (!Expression.IsAssignableToParameterType(param.GetNotNullableType(), parameters[0]))
			{
				throw new InvalidOperationException("left-side argument type does not match expression type");
			}
			return method;
		}

		private static MethodInfo UnaryCoreCheck(string oper_name, Expression expression, MethodInfo method, Func<Type, bool> validator)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (method != null)
			{
				return Expression.CheckUnaryMethod(method, expression.Type);
			}
			Type notNullableType = expression.Type.GetNotNullableType();
			if (validator(notNullableType))
			{
				return null;
			}
			if (oper_name != null)
			{
				method = Expression.GetUnaryOperator(oper_name, notNullableType, expression.Type);
				if (method != null)
				{
					return method;
				}
			}
			throw new InvalidOperationException(string.Format("Operation {0} not defined for {1}", (oper_name == null) ? "is" : oper_name.Substring(3), expression.Type));
		}

		private static MethodInfo GetBinaryOperator(string oper_name, Type on_type, Expression left, Expression right)
		{
			MethodInfo[] methods = on_type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			foreach (MethodInfo methodInfo in methods)
			{
				if (!(methodInfo.Name != oper_name))
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 2)
					{
						if (!methodInfo.IsGenericMethod)
						{
							if (Expression.IsAssignableToParameterType(left.Type, parameters[0]))
							{
								if (Expression.IsAssignableToParameterType(right.Type, parameters[1]))
								{
									return methodInfo;
								}
							}
						}
					}
				}
			}
			return null;
		}

		private static MethodInfo BinaryCoreCheck(string oper_name, Expression left, Expression right, MethodInfo method)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (method != null)
			{
				if (method.ReturnType == typeof(void))
				{
					throw new ArgumentException("Specified method must return a value", "method");
				}
				if (!method.IsStatic)
				{
					throw new ArgumentException("Method must be static", "method");
				}
				ParameterInfo[] parameters = method.GetParameters();
				if (parameters.Length != 2)
				{
					throw new ArgumentException("Must have only two parameters", "method");
				}
				if (!Expression.IsAssignableToParameterType(left.Type, parameters[0]))
				{
					throw new InvalidOperationException("left-side argument type does not match left expression type");
				}
				if (!Expression.IsAssignableToParameterType(right.Type, parameters[1]))
				{
					throw new InvalidOperationException("right-side argument type does not match right expression type");
				}
				return method;
			}
			else
			{
				Type type = left.Type;
				Type type2 = right.Type;
				Type notNullableType = type.GetNotNullableType();
				Type notNullableType2 = type2.GetNotNullableType();
				if ((oper_name == "op_BitwiseOr" || oper_name == "op_BitwiseAnd") && notNullableType == typeof(bool) && notNullableType == notNullableType2 && type == type2)
				{
					return null;
				}
				if (Expression.IsNumber(notNullableType))
				{
					if (notNullableType == notNullableType2 && type == type2)
					{
						return null;
					}
					if (oper_name != null)
					{
						method = Expression.GetBinaryOperator(oper_name, notNullableType2, left, right);
						if (method != null)
						{
							return method;
						}
					}
				}
				if (oper_name != null)
				{
					method = Expression.GetBinaryOperator(oper_name, notNullableType, left, right);
					if (method != null)
					{
						return method;
					}
				}
				if (oper_name == "op_Equality" || oper_name == "op_Inequality")
				{
					if (!type.IsValueType && !type2.IsValueType)
					{
						return null;
					}
					if (type == type2 && notNullableType.IsEnum)
					{
						return null;
					}
					if (type == type2 && notNullableType == typeof(bool))
					{
						return null;
					}
				}
				if ((oper_name == "op_LeftShift" || oper_name == "op_RightShift") && Expression.IsInt(notNullableType) && notNullableType2 == typeof(int))
				{
					return null;
				}
				throw new InvalidOperationException(string.Format("Operation {0} not defined for {1} and {2}", (oper_name == null) ? "is" : oper_name.Substring(3), type, type2));
			}
		}

		private static MethodInfo BinaryBitwiseCoreCheck(string oper_name, Expression left, Expression right, MethodInfo method)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (method == null && left.Type == right.Type && Expression.IsIntOrBool(left.Type))
			{
				return null;
			}
			method = Expression.BinaryCoreCheck(oper_name, left, right, method);
			if (method == null && (left.Type == typeof(double) || left.Type == typeof(float)))
			{
				throw new InvalidOperationException("Types not supported");
			}
			return method;
		}

		private static BinaryExpression MakeSimpleBinary(ExpressionType et, Expression left, Expression right, MethodInfo method)
		{
			bool flag;
			Type type;
			if (method == null)
			{
				flag = left.Type.IsNullable();
				type = left.Type;
			}
			else
			{
				ParameterInfo[] parameters = method.GetParameters();
				ParameterInfo parameterInfo = parameters[0];
				ParameterInfo parameterInfo2 = parameters[1];
				if (Expression.IsAssignableToOperatorParameter(left, parameterInfo) && Expression.IsAssignableToOperatorParameter(right, parameterInfo2))
				{
					flag = false;
					type = method.ReturnType;
				}
				else
				{
					if (!left.Type.IsNullable() || !right.Type.IsNullable() || left.Type.GetNotNullableType() != parameterInfo.ParameterType || right.Type.GetNotNullableType() != parameterInfo2.ParameterType || method.ReturnType.IsNullable())
					{
						throw new InvalidOperationException();
					}
					flag = true;
					type = method.ReturnType.MakeNullableType();
				}
			}
			return new BinaryExpression(et, type, left, right, flag, flag, method, null);
		}

		private static bool IsAssignableToOperatorParameter(Expression expression, ParameterInfo parameter)
		{
			return expression.Type == parameter.ParameterType || (!expression.Type.IsNullable() && !parameter.ParameterType.IsNullable() && Expression.IsAssignableToParameterType(expression.Type, parameter));
		}

		private static UnaryExpression MakeSimpleUnary(ExpressionType et, Expression expression, MethodInfo method)
		{
			Type type;
			bool flag;
			if (method == null)
			{
				type = expression.Type;
				flag = type.IsNullable();
			}
			else
			{
				ParameterInfo parameterInfo = method.GetParameters()[0];
				if (Expression.IsAssignableToOperatorParameter(expression, parameterInfo))
				{
					flag = false;
					type = method.ReturnType;
				}
				else
				{
					if (!expression.Type.IsNullable() || expression.Type.GetNotNullableType() != parameterInfo.ParameterType || method.ReturnType.IsNullable())
					{
						throw new InvalidOperationException();
					}
					flag = true;
					type = method.ReturnType.MakeNullableType();
				}
			}
			return new UnaryExpression(et, expression, type, method, flag);
		}

		private static BinaryExpression MakeBoolBinary(ExpressionType et, Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			bool flag;
			Type type;
			if (method == null)
			{
				if (!left.Type.IsNullable() && !right.Type.IsNullable())
				{
					flag = false;
					liftToNull = false;
					type = typeof(bool);
				}
				else
				{
					if (!left.Type.IsNullable() || !right.Type.IsNullable())
					{
						throw new InvalidOperationException();
					}
					flag = true;
					type = ((!liftToNull) ? typeof(bool) : typeof(bool?));
				}
			}
			else
			{
				ParameterInfo[] parameters = method.GetParameters();
				ParameterInfo parameterInfo = parameters[0];
				ParameterInfo parameterInfo2 = parameters[1];
				if (Expression.IsAssignableToOperatorParameter(left, parameterInfo) && Expression.IsAssignableToOperatorParameter(right, parameterInfo2))
				{
					flag = false;
					liftToNull = false;
					type = method.ReturnType;
				}
				else
				{
					if (!left.Type.IsNullable() || !right.Type.IsNullable() || left.Type.GetNotNullableType() != parameterInfo.ParameterType || right.Type.GetNotNullableType() != parameterInfo2.ParameterType)
					{
						throw new InvalidOperationException();
					}
					flag = true;
					if (method.ReturnType == typeof(bool))
					{
						type = ((!liftToNull) ? typeof(bool) : typeof(bool?));
					}
					else
					{
						if (method.ReturnType.IsNullable())
						{
							throw new InvalidOperationException();
						}
						type = method.ReturnType.MakeNullableType();
					}
				}
			}
			return new BinaryExpression(et, type, left, right, liftToNull, flag, method, null);
		}

		public static BinaryExpression Add(Expression left, Expression right)
		{
			return Expression.Add(left, right, null);
		}

		public static BinaryExpression Add(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Addition", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.Add, left, right, method);
		}

		public static BinaryExpression AddChecked(Expression left, Expression right)
		{
			return Expression.AddChecked(left, right, null);
		}

		public static BinaryExpression AddChecked(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Addition", left, right, method);
			if (method == null && (left.Type == typeof(byte) || left.Type == typeof(sbyte)))
			{
				throw new InvalidOperationException(string.Format("AddChecked not defined for {0} and {1}", left.Type, right.Type));
			}
			return Expression.MakeSimpleBinary(ExpressionType.AddChecked, left, right, method);
		}

		public static BinaryExpression Subtract(Expression left, Expression right)
		{
			return Expression.Subtract(left, right, null);
		}

		public static BinaryExpression Subtract(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Subtraction", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.Subtract, left, right, method);
		}

		public static BinaryExpression SubtractChecked(Expression left, Expression right)
		{
			return Expression.SubtractChecked(left, right, null);
		}

		public static BinaryExpression SubtractChecked(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Subtraction", left, right, method);
			if (method == null && (left.Type == typeof(byte) || left.Type == typeof(sbyte)))
			{
				throw new InvalidOperationException(string.Format("SubtractChecked not defined for {0} and {1}", left.Type, right.Type));
			}
			return Expression.MakeSimpleBinary(ExpressionType.SubtractChecked, left, right, method);
		}

		public static BinaryExpression Modulo(Expression left, Expression right)
		{
			return Expression.Modulo(left, right, null);
		}

		public static BinaryExpression Modulo(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Modulus", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.Modulo, left, right, method);
		}

		public static BinaryExpression Multiply(Expression left, Expression right)
		{
			return Expression.Multiply(left, right, null);
		}

		public static BinaryExpression Multiply(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Multiply", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.Multiply, left, right, method);
		}

		public static BinaryExpression MultiplyChecked(Expression left, Expression right)
		{
			return Expression.MultiplyChecked(left, right, null);
		}

		public static BinaryExpression MultiplyChecked(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Multiply", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.MultiplyChecked, left, right, method);
		}

		public static BinaryExpression Divide(Expression left, Expression right)
		{
			return Expression.Divide(left, right, null);
		}

		public static BinaryExpression Divide(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Division", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.Divide, left, right, method);
		}

		public static BinaryExpression Power(Expression left, Expression right)
		{
			return Expression.Power(left, right, null);
		}

		public static BinaryExpression Power(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck(null, left, right, method);
			if (left.Type.GetNotNullableType() != typeof(double))
			{
				throw new InvalidOperationException("Power only supports double arguments");
			}
			return Expression.MakeSimpleBinary(ExpressionType.Power, left, right, method);
		}

		public static BinaryExpression And(Expression left, Expression right)
		{
			return Expression.And(left, right, null);
		}

		public static BinaryExpression And(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryBitwiseCoreCheck("op_BitwiseAnd", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.And, left, right, method);
		}

		public static BinaryExpression Or(Expression left, Expression right)
		{
			return Expression.Or(left, right, null);
		}

		public static BinaryExpression Or(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryBitwiseCoreCheck("op_BitwiseOr", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.Or, left, right, method);
		}

		public static BinaryExpression ExclusiveOr(Expression left, Expression right)
		{
			return Expression.ExclusiveOr(left, right, null);
		}

		public static BinaryExpression ExclusiveOr(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryBitwiseCoreCheck("op_ExclusiveOr", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.ExclusiveOr, left, right, method);
		}

		public static BinaryExpression LeftShift(Expression left, Expression right)
		{
			return Expression.LeftShift(left, right, null);
		}

		public static BinaryExpression LeftShift(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryBitwiseCoreCheck("op_LeftShift", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.LeftShift, left, right, method);
		}

		public static BinaryExpression RightShift(Expression left, Expression right)
		{
			return Expression.RightShift(left, right, null);
		}

		public static BinaryExpression RightShift(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_RightShift", left, right, method);
			return Expression.MakeSimpleBinary(ExpressionType.RightShift, left, right, method);
		}

		public static BinaryExpression AndAlso(Expression left, Expression right)
		{
			return Expression.AndAlso(left, right, null);
		}

		public static BinaryExpression AndAlso(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.ConditionalBinaryCheck("op_BitwiseAnd", left, right, method);
			return Expression.MakeBoolBinary(ExpressionType.AndAlso, left, right, true, method);
		}

		private static MethodInfo ConditionalBinaryCheck(string oper, Expression left, Expression right, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck(oper, left, right, method);
			if (method == null)
			{
				if (left.Type.GetNotNullableType() != typeof(bool))
				{
					throw new InvalidOperationException("Only booleans are allowed");
				}
			}
			else
			{
				Type notNullableType = left.Type.GetNotNullableType();
				if (left.Type != right.Type || method.ReturnType != notNullableType)
				{
					throw new ArgumentException("left, right and return type must match");
				}
				MethodInfo trueOperator = Expression.GetTrueOperator(notNullableType);
				MethodInfo falseOperator = Expression.GetFalseOperator(notNullableType);
				if (trueOperator == null || falseOperator == null)
				{
					throw new ArgumentException("Operators true and false are required but not defined");
				}
			}
			return method;
		}

		public static BinaryExpression OrElse(Expression left, Expression right)
		{
			return Expression.OrElse(left, right, null);
		}

		public static BinaryExpression OrElse(Expression left, Expression right, MethodInfo method)
		{
			method = Expression.ConditionalBinaryCheck("op_BitwiseOr", left, right, method);
			return Expression.MakeBoolBinary(ExpressionType.OrElse, left, right, true, method);
		}

		public static BinaryExpression Equal(Expression left, Expression right)
		{
			return Expression.Equal(left, right, false, null);
		}

		public static BinaryExpression Equal(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Equality", left, right, method);
			return Expression.MakeBoolBinary(ExpressionType.Equal, left, right, liftToNull, method);
		}

		public static BinaryExpression NotEqual(Expression left, Expression right)
		{
			return Expression.NotEqual(left, right, false, null);
		}

		public static BinaryExpression NotEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_Inequality", left, right, method);
			return Expression.MakeBoolBinary(ExpressionType.NotEqual, left, right, liftToNull, method);
		}

		public static BinaryExpression GreaterThan(Expression left, Expression right)
		{
			return Expression.GreaterThan(left, right, false, null);
		}

		public static BinaryExpression GreaterThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_GreaterThan", left, right, method);
			return Expression.MakeBoolBinary(ExpressionType.GreaterThan, left, right, liftToNull, method);
		}

		public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right)
		{
			return Expression.GreaterThanOrEqual(left, right, false, null);
		}

		public static BinaryExpression GreaterThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_GreaterThanOrEqual", left, right, method);
			return Expression.MakeBoolBinary(ExpressionType.GreaterThanOrEqual, left, right, liftToNull, method);
		}

		public static BinaryExpression LessThan(Expression left, Expression right)
		{
			return Expression.LessThan(left, right, false, null);
		}

		public static BinaryExpression LessThan(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_LessThan", left, right, method);
			return Expression.MakeBoolBinary(ExpressionType.LessThan, left, right, liftToNull, method);
		}

		public static BinaryExpression LessThanOrEqual(Expression left, Expression right)
		{
			return Expression.LessThanOrEqual(left, right, false, null);
		}

		public static BinaryExpression LessThanOrEqual(Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			method = Expression.BinaryCoreCheck("op_LessThanOrEqual", left, right, method);
			return Expression.MakeBoolBinary(ExpressionType.LessThanOrEqual, left, right, liftToNull, method);
		}

		private static void CheckArray(Expression array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (!array.Type.IsArray)
			{
				throw new ArgumentException("The array argument must be of type array");
			}
		}

		public static BinaryExpression ArrayIndex(Expression array, Expression index)
		{
			Expression.CheckArray(array);
			if (index == null)
			{
				throw new ArgumentNullException("index");
			}
			if (array.Type.GetArrayRank() != 1)
			{
				throw new ArgumentException("The array argument must be a single dimensional array");
			}
			if (index.Type != typeof(int))
			{
				throw new ArgumentException("The index must be of type int");
			}
			return new BinaryExpression(ExpressionType.ArrayIndex, array.Type.GetElementType(), array, index);
		}

		public static BinaryExpression Coalesce(Expression left, Expression right)
		{
			return Expression.Coalesce(left, right, null);
		}

		private static BinaryExpression MakeCoalesce(Expression left, Expression right)
		{
			Type type = null;
			if (left.Type.IsNullable())
			{
				Type notNullableType = left.Type.GetNotNullableType();
				if (!right.Type.IsNullable() && right.Type.IsAssignableTo(notNullableType))
				{
					type = notNullableType;
				}
			}
			if (type == null && right.Type.IsAssignableTo(left.Type))
			{
				type = left.Type;
			}
			if (type == null && left.Type.IsNullable() && left.Type.GetNotNullableType().IsAssignableTo(right.Type))
			{
				type = right.Type;
			}
			if (type == null)
			{
				throw new ArgumentException("Incompatible argument types");
			}
			return new BinaryExpression(ExpressionType.Coalesce, type, left, right, false, false, null, null);
		}

		private static BinaryExpression MakeConvertedCoalesce(Expression left, Expression right, LambdaExpression conversion)
		{
			MethodInfo invokeMethod = conversion.Type.GetInvokeMethod();
			Expression.CheckNotVoid(invokeMethod.ReturnType);
			if (invokeMethod.ReturnType != right.Type)
			{
				throw new InvalidOperationException("Conversion return type doesn't march right type");
			}
			ParameterInfo[] parameters = invokeMethod.GetParameters();
			if (parameters.Length != 1)
			{
				throw new ArgumentException("Conversion has wrong number of parameters");
			}
			if (!Expression.IsAssignableToParameterType(left.Type, parameters[0]))
			{
				throw new InvalidOperationException("Conversion argument doesn't marcht left type");
			}
			return new BinaryExpression(ExpressionType.Coalesce, right.Type, left, right, false, false, null, conversion);
		}

		public static BinaryExpression Coalesce(Expression left, Expression right, LambdaExpression conversion)
		{
			if (left == null)
			{
				throw new ArgumentNullException("left");
			}
			if (right == null)
			{
				throw new ArgumentNullException("right");
			}
			if (left.Type.IsValueType && !left.Type.IsNullable())
			{
				throw new InvalidOperationException("Left expression can never be null");
			}
			if (conversion != null)
			{
				return Expression.MakeConvertedCoalesce(left, right, conversion);
			}
			return Expression.MakeCoalesce(left, right);
		}

		public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right)
		{
			return Expression.MakeBinary(binaryType, left, right, false, null);
		}

		public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method)
		{
			return Expression.MakeBinary(binaryType, left, right, liftToNull, method, null);
		}

		public static BinaryExpression MakeBinary(ExpressionType binaryType, Expression left, Expression right, bool liftToNull, MethodInfo method, LambdaExpression conversion)
		{
			switch (binaryType)
			{
			case ExpressionType.Add:
				return Expression.Add(left, right, method);
			case ExpressionType.AddChecked:
				return Expression.AddChecked(left, right, method);
			case ExpressionType.And:
				return Expression.And(left, right, method);
			case ExpressionType.AndAlso:
				return Expression.AndAlso(left, right);
			case ExpressionType.Coalesce:
				return Expression.Coalesce(left, right, conversion);
			case ExpressionType.Divide:
				return Expression.Divide(left, right, method);
			case ExpressionType.Equal:
				return Expression.Equal(left, right, liftToNull, method);
			case ExpressionType.ExclusiveOr:
				return Expression.ExclusiveOr(left, right, method);
			case ExpressionType.GreaterThan:
				return Expression.GreaterThan(left, right, liftToNull, method);
			case ExpressionType.GreaterThanOrEqual:
				return Expression.GreaterThanOrEqual(left, right, liftToNull, method);
			case ExpressionType.LeftShift:
				return Expression.LeftShift(left, right, method);
			case ExpressionType.LessThan:
				return Expression.LessThan(left, right, liftToNull, method);
			case ExpressionType.LessThanOrEqual:
				return Expression.LessThanOrEqual(left, right, liftToNull, method);
			case ExpressionType.Modulo:
				return Expression.Modulo(left, right, method);
			case ExpressionType.Multiply:
				return Expression.Multiply(left, right, method);
			case ExpressionType.MultiplyChecked:
				return Expression.MultiplyChecked(left, right, method);
			case ExpressionType.NotEqual:
				return Expression.NotEqual(left, right, liftToNull, method);
			case ExpressionType.Or:
				return Expression.Or(left, right, method);
			case ExpressionType.OrElse:
				return Expression.OrElse(left, right);
			case ExpressionType.Power:
				return Expression.Power(left, right, method);
			case ExpressionType.RightShift:
				return Expression.RightShift(left, right, method);
			case ExpressionType.Subtract:
				return Expression.Subtract(left, right, method);
			case ExpressionType.SubtractChecked:
				return Expression.SubtractChecked(left, right, method);
			}
			throw new ArgumentException("MakeBinary expect a binary node type");
		}

		public static MethodCallExpression ArrayIndex(Expression array, params Expression[] indexes)
		{
			return Expression.ArrayIndex(array, indexes);
		}

		public static MethodCallExpression ArrayIndex(Expression array, IEnumerable<Expression> indexes)
		{
			Expression.CheckArray(array);
			if (indexes == null)
			{
				throw new ArgumentNullException("indexes");
			}
			ReadOnlyCollection<Expression> readOnlyCollection = indexes.ToReadOnlyCollection<Expression>();
			if (array.Type.GetArrayRank() != readOnlyCollection.Count)
			{
				throw new ArgumentException("The number of arguments doesn't match the rank of the array");
			}
			foreach (Expression expression in readOnlyCollection)
			{
				if (expression.Type != typeof(int))
				{
					throw new ArgumentException("The index must be of type int");
				}
			}
			return Expression.Call(array, array.Type.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy), readOnlyCollection);
		}

		public static UnaryExpression ArrayLength(Expression array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (!array.Type.IsArray)
			{
				throw new ArgumentException("The type of the expression must me Array");
			}
			if (array.Type.GetArrayRank() != 1)
			{
				throw new ArgumentException("The array must be a single dimensional array");
			}
			return new UnaryExpression(ExpressionType.ArrayLength, array, typeof(int));
		}

		public static MemberAssignment Bind(MemberInfo member, Expression expression)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			Type type = null;
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo != null && propertyInfo.GetSetMethod(true) != null)
			{
				type = propertyInfo.PropertyType;
			}
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo != null)
			{
				type = fieldInfo.FieldType;
			}
			if (type == null)
			{
				throw new ArgumentException("member");
			}
			if (!expression.Type.IsAssignableTo(type))
			{
				throw new ArgumentException("member");
			}
			return new MemberAssignment(member, expression);
		}

		public static MemberAssignment Bind(MethodInfo propertyAccessor, Expression expression)
		{
			if (propertyAccessor == null)
			{
				throw new ArgumentNullException("propertyAccessor");
			}
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			Expression.CheckNonGenericMethod(propertyAccessor);
			PropertyInfo associatedProperty = Expression.GetAssociatedProperty(propertyAccessor);
			if (associatedProperty == null)
			{
				throw new ArgumentException("propertyAccessor");
			}
			if (associatedProperty.GetSetMethod(true) == null)
			{
				throw new ArgumentException("setter");
			}
			if (!expression.Type.IsAssignableTo(associatedProperty.PropertyType))
			{
				throw new ArgumentException("member");
			}
			return new MemberAssignment(associatedProperty, expression);
		}

		public static MethodCallExpression Call(Expression instance, MethodInfo method)
		{
			return Expression.Call(instance, method, null);
		}

		public static MethodCallExpression Call(MethodInfo method, params Expression[] arguments)
		{
			return Expression.Call(null, method, arguments);
		}

		public static MethodCallExpression Call(Expression instance, MethodInfo method, params Expression[] arguments)
		{
			return Expression.Call(instance, method, arguments);
		}

		public static MethodCallExpression Call(Expression instance, MethodInfo method, IEnumerable<Expression> arguments)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (instance == null && !method.IsStatic)
			{
				throw new ArgumentNullException("instance");
			}
			if (method.IsStatic && instance != null)
			{
				throw new ArgumentException("instance");
			}
			if (!method.IsStatic && !instance.Type.IsAssignableTo(method.DeclaringType))
			{
				throw new ArgumentException("Type is not assignable to the declaring type of the method");
			}
			ReadOnlyCollection<Expression> readOnlyCollection = Expression.CheckMethodArguments(method, arguments);
			return new MethodCallExpression(instance, method, readOnlyCollection);
		}

		private static Type[] CollectTypes(IEnumerable<Expression> expressions)
		{
			return expressions.Select<Expression, Type>((Expression arg) => arg.Type).ToArray<Type>();
		}

		private static MethodInfo TryMakeGeneric(MethodInfo method, Type[] args)
		{
			if (method == null)
			{
				return null;
			}
			if (!method.IsGenericMethod && (args == null || args.Length == 0))
			{
				return method;
			}
			if (args.Length == method.GetGenericArguments().Length)
			{
				return method.MakeGenericMethod(args);
			}
			return null;
		}

		public static MethodCallExpression Call(Expression instance, string methodName, Type[] typeArguments, params Expression[] arguments)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			MethodInfo methodInfo = Expression.TryGetMethod(instance.Type, methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, Expression.CollectTypes(arguments), typeArguments);
			ReadOnlyCollection<Expression> readOnlyCollection = Expression.CheckMethodArguments(methodInfo, arguments);
			return new MethodCallExpression(instance, methodInfo, readOnlyCollection);
		}

		private static bool MethodMatch(MethodInfo method, string name, Type[] parameterTypes, Type[] argumentTypes)
		{
			if (method.Name != name)
			{
				return false;
			}
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length != parameterTypes.Length)
			{
				return false;
			}
			if (method.IsGenericMethod && method.IsGenericMethodDefinition)
			{
				MethodInfo methodInfo = Expression.TryMakeGeneric(method, argumentTypes);
				return methodInfo != null && Expression.MethodMatch(methodInfo, name, parameterTypes, argumentTypes);
			}
			if (!method.IsGenericMethod && argumentTypes != null && argumentTypes.Length > 0)
			{
				return false;
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				Type type = parameterTypes[i];
				ParameterInfo parameterInfo = parameters[i];
				if (!Expression.IsAssignableToParameterType(type, parameterInfo) && !Expression.IsExpressionOfParameter(type, parameterInfo.ParameterType))
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsExpressionOfParameter(Type type, Type ptype)
		{
			return ptype.IsGenericInstanceOf(typeof(Expression<>)) && ptype.GetFirstGenericArgument() == type;
		}

		private static MethodInfo TryGetMethod(Type type, string methodName, BindingFlags flags, Type[] parameterTypes, Type[] argumentTypes)
		{
			IEnumerable<MethodInfo> enumerable = from meth in type.GetMethods(flags)
				where Expression.MethodMatch(meth, methodName, parameterTypes, argumentTypes)
				select meth;
			if (enumerable.Count<MethodInfo>() > 1)
			{
				throw new InvalidOperationException("Too many method candidates");
			}
			MethodInfo methodInfo = Expression.TryMakeGeneric(enumerable.FirstOrDefault<MethodInfo>(), argumentTypes);
			if (methodInfo != null)
			{
				return methodInfo;
			}
			throw new InvalidOperationException("No such method");
		}

		public static MethodCallExpression Call(Type type, string methodName, Type[] typeArguments, params Expression[] arguments)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			MethodInfo methodInfo = Expression.TryGetMethod(type, methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, Expression.CollectTypes(arguments), typeArguments);
			ReadOnlyCollection<Expression> readOnlyCollection = Expression.CheckMethodArguments(methodInfo, arguments);
			return new MethodCallExpression(methodInfo, readOnlyCollection);
		}

		public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse)
		{
			if (test == null)
			{
				throw new ArgumentNullException("test");
			}
			if (ifTrue == null)
			{
				throw new ArgumentNullException("ifTrue");
			}
			if (ifFalse == null)
			{
				throw new ArgumentNullException("ifFalse");
			}
			if (test.Type != typeof(bool))
			{
				throw new ArgumentException("Test expression should be of type bool");
			}
			if (ifTrue.Type != ifFalse.Type)
			{
				throw new ArgumentException("The ifTrue and ifFalse type do not match");
			}
			return new ConditionalExpression(test, ifTrue, ifFalse);
		}

		public static ConstantExpression Constant(object value)
		{
			if (value == null)
			{
				return new ConstantExpression(null, typeof(object));
			}
			return Expression.Constant(value, value.GetType());
		}

		public static ConstantExpression Constant(object value, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (value == null)
			{
				if (type.IsValueType && !type.IsNullable())
				{
					throw new ArgumentException();
				}
			}
			else if ((!type.IsValueType || !type.IsNullable()) && !value.GetType().IsAssignableTo(type))
			{
				throw new ArgumentException();
			}
			return new ConstantExpression(value, type);
		}

		private static bool IsConvertiblePrimitive(Type type)
		{
			Type notNullableType = type.GetNotNullableType();
			return notNullableType != typeof(bool) && (notNullableType.IsEnum || notNullableType.IsPrimitive);
		}

		internal static bool IsPrimitiveConversion(Type type, Type target)
		{
			return type == target || (type.IsNullable() && target == type.GetNotNullableType()) || (target.IsNullable() && type == target.GetNotNullableType()) || (Expression.IsConvertiblePrimitive(type) && Expression.IsConvertiblePrimitive(target));
		}

		internal static bool IsReferenceConversion(Type type, Type target)
		{
			return type == target || (type == typeof(object) || target == typeof(object)) || (type.IsInterface || target.IsInterface) || (!type.IsValueType && !target.IsValueType && (type.IsAssignableTo(target) || target.IsAssignableTo(type)));
		}

		public static UnaryExpression Convert(Expression expression, Type type)
		{
			return Expression.Convert(expression, type, null);
		}

		private static MethodInfo GetUserConversionMethod(Type type, Type target)
		{
			MethodInfo methodInfo = Expression.GetUnaryOperator("op_Explicit", type, type, target);
			if (methodInfo == null)
			{
				methodInfo = Expression.GetUnaryOperator("op_Implicit", type, type, target);
			}
			if (methodInfo == null)
			{
				methodInfo = Expression.GetUnaryOperator("op_Explicit", target, type, target);
			}
			if (methodInfo == null)
			{
				methodInfo = Expression.GetUnaryOperator("op_Implicit", target, type, target);
			}
			if (methodInfo == null)
			{
				throw new InvalidOperationException();
			}
			return methodInfo;
		}

		public static UnaryExpression Convert(Expression expression, Type type, MethodInfo method)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type type2 = expression.Type;
			if (method != null)
			{
				Expression.CheckUnaryMethod(method, type2);
			}
			else if (!Expression.IsPrimitiveConversion(type2, type) && !Expression.IsReferenceConversion(type2, type))
			{
				method = Expression.GetUserConversionMethod(type2, type);
			}
			return new UnaryExpression(ExpressionType.Convert, expression, type, method, Expression.IsConvertNodeLifted(method, expression, type));
		}

		private static bool IsConvertNodeLifted(MethodInfo method, Expression operand, Type target)
		{
			if (method == null)
			{
				return operand.Type.IsNullable() || target.IsNullable();
			}
			return (operand.Type.IsNullable() && !Expression.ParameterMatch(method, operand.Type)) || (target.IsNullable() && !Expression.ReturnTypeMatch(method, target));
		}

		private static bool ParameterMatch(MethodInfo method, Type type)
		{
			return method.GetParameters()[0].ParameterType == type;
		}

		private static bool ReturnTypeMatch(MethodInfo method, Type type)
		{
			return method.ReturnType == type;
		}

		public static UnaryExpression ConvertChecked(Expression expression, Type type)
		{
			return Expression.ConvertChecked(expression, type, null);
		}

		public static UnaryExpression ConvertChecked(Expression expression, Type type, MethodInfo method)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type type2 = expression.Type;
			if (method != null)
			{
				Expression.CheckUnaryMethod(method, type2);
			}
			else
			{
				if (Expression.IsReferenceConversion(type2, type))
				{
					return Expression.Convert(expression, type, method);
				}
				if (!Expression.IsPrimitiveConversion(type2, type))
				{
					method = Expression.GetUserConversionMethod(type2, type);
				}
			}
			return new UnaryExpression(ExpressionType.ConvertChecked, expression, type, method, Expression.IsConvertNodeLifted(method, expression, type));
		}

		public static ElementInit ElementInit(MethodInfo addMethod, params Expression[] arguments)
		{
			return Expression.ElementInit(addMethod, arguments);
		}

		public static ElementInit ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
		{
			if (addMethod == null)
			{
				throw new ArgumentNullException("addMethod");
			}
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (addMethod.Name.ToLower(CultureInfo.InvariantCulture) != "add")
			{
				throw new ArgumentException("addMethod");
			}
			if (addMethod.IsStatic)
			{
				throw new ArgumentException("addMethod must be an instance method", "addMethod");
			}
			ReadOnlyCollection<Expression> readOnlyCollection = Expression.CheckMethodArguments(addMethod, arguments);
			return new ElementInit(addMethod, readOnlyCollection);
		}

		public static MemberExpression Field(Expression expression, FieldInfo field)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			if (!field.IsStatic)
			{
				if (expression == null)
				{
					throw new ArgumentNullException("expression");
				}
				if (!expression.Type.IsAssignableTo(field.DeclaringType))
				{
					throw new ArgumentException("field");
				}
			}
			else if (expression != null)
			{
				throw new ArgumentException("expression");
			}
			return new MemberExpression(expression, field, field.FieldType);
		}

		public static MemberExpression Field(Expression expression, string fieldName)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			FieldInfo field = expression.Type.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (field == null)
			{
				throw new ArgumentException(string.Format("No field named {0} on {1}", fieldName, expression.Type));
			}
			return new MemberExpression(expression, field, field.FieldType);
		}

		public static Type GetActionType(params Type[] typeArgs)
		{
			if (typeArgs == null)
			{
				throw new ArgumentNullException("typeArgs");
			}
			if (typeArgs.Length > 4)
			{
				throw new ArgumentException("No Action type of this arity");
			}
			if (typeArgs.Length == 0)
			{
				return typeof(Action);
			}
			Type type = null;
			switch (typeArgs.Length)
			{
			case 1:
				type = typeof(Action<>);
				break;
			case 2:
				type = typeof(Action<, >);
				break;
			case 3:
				type = typeof(Action<, , >);
				break;
			case 4:
				type = typeof(Action<, , , >);
				break;
			}
			return type.MakeGenericType(typeArgs);
		}

		public static Type GetFuncType(params Type[] typeArgs)
		{
			if (typeArgs == null)
			{
				throw new ArgumentNullException("typeArgs");
			}
			if (typeArgs.Length < 1 || typeArgs.Length > 5)
			{
				throw new ArgumentException("No Func type of this arity");
			}
			Type type = null;
			switch (typeArgs.Length)
			{
			case 1:
				type = typeof(Func<>);
				break;
			case 2:
				type = typeof(Func<, >);
				break;
			case 3:
				type = typeof(Func<, , >);
				break;
			case 4:
				type = typeof(Func<, , , >);
				break;
			case 5:
				type = typeof(Func<, , , , >);
				break;
			}
			return type.MakeGenericType(typeArgs);
		}

		public static InvocationExpression Invoke(Expression expression, params Expression[] arguments)
		{
			return Expression.Invoke(expression, arguments);
		}

		private static Type GetInvokableType(Type t)
		{
			if (t.IsAssignableTo(typeof(Delegate)))
			{
				return t;
			}
			return Expression.GetGenericType(t, typeof(Expression<>));
		}

		private static Type GetGenericType(Type t, Type def)
		{
			if (t == null)
			{
				return null;
			}
			if (t.IsGenericType && t.GetGenericTypeDefinition() == def)
			{
				return t;
			}
			return Expression.GetGenericType(t.BaseType, def);
		}

		public static InvocationExpression Invoke(Expression expression, IEnumerable<Expression> arguments)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			Type invokableType = Expression.GetInvokableType(expression.Type);
			if (invokableType == null)
			{
				throw new ArgumentException("The type of the expression is not invokable");
			}
			ReadOnlyCollection<Expression> readOnlyCollection = arguments.ToReadOnlyCollection<Expression>();
			Expression.CheckForNull<Expression>(readOnlyCollection, "arguments");
			MethodInfo invokeMethod = invokableType.GetInvokeMethod();
			if (invokeMethod == null)
			{
				throw new ArgumentException("expression");
			}
			if (invokeMethod.GetParameters().Length != readOnlyCollection.Count)
			{
				throw new InvalidOperationException("Arguments count doesn't match parameters length");
			}
			readOnlyCollection = Expression.CheckMethodArguments(invokeMethod, readOnlyCollection);
			return new InvocationExpression(expression, invokeMethod.ReturnType, readOnlyCollection);
		}

		private static bool CanAssign(Type target, Type source)
		{
			return !(target.IsValueType ^ source.IsValueType) && source.IsAssignableTo(target);
		}

		private static Expression CheckLambda(Type delegateType, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
		{
			if (!delegateType.IsSubclassOf(typeof(Delegate)))
			{
				throw new ArgumentException("delegateType");
			}
			MethodInfo invokeMethod = delegateType.GetInvokeMethod();
			if (invokeMethod == null)
			{
				throw new ArgumentException("delegate must contain an Invoke method", "delegateType");
			}
			ParameterInfo[] parameters2 = invokeMethod.GetParameters();
			if (parameters2.Length != parameters.Count)
			{
				throw new ArgumentException(string.Format("Different number of arguments in delegate {0}", delegateType), "delegateType");
			}
			for (int i = 0; i < parameters2.Length; i++)
			{
				ParameterExpression parameterExpression = parameters[i];
				if (parameterExpression == null)
				{
					throw new ArgumentNullException("parameters");
				}
				if (!Expression.CanAssign(parameterExpression.Type, parameters2[i].ParameterType))
				{
					throw new ArgumentException(string.Format("Can not assign a {0} to a {1}", parameters2[i].ParameterType, parameterExpression.Type));
				}
			}
			if (invokeMethod.ReturnType == typeof(void) || Expression.CanAssign(invokeMethod.ReturnType, body.Type))
			{
				return body;
			}
			if (invokeMethod.ReturnType.IsExpression())
			{
				return Expression.Quote(body);
			}
			throw new ArgumentException(string.Format("body type {0} can not be assigned to {1}", body.Type, invokeMethod.ReturnType));
		}

		public static Expression<TDelegate> Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters)
		{
			return Expression.Lambda<TDelegate>(body, parameters);
		}

		public static Expression<TDelegate> Lambda<TDelegate>(Expression body, IEnumerable<ParameterExpression> parameters)
		{
			if (body == null)
			{
				throw new ArgumentNullException("body");
			}
			ReadOnlyCollection<ParameterExpression> readOnlyCollection = parameters.ToReadOnlyCollection<ParameterExpression>();
			body = Expression.CheckLambda(typeof(TDelegate), body, readOnlyCollection);
			return new Expression<TDelegate>(body, readOnlyCollection);
		}

		public static LambdaExpression Lambda(Expression body, params ParameterExpression[] parameters)
		{
			if (body == null)
			{
				throw new ArgumentNullException("body");
			}
			if (parameters.Length > 4)
			{
				throw new ArgumentException("Too many parameters");
			}
			return Expression.Lambda(Expression.GetDelegateType(body.Type, parameters), body, parameters);
		}

		private static Type GetDelegateType(Type return_type, ParameterExpression[] parameters)
		{
			if (parameters == null)
			{
				parameters = new ParameterExpression[0];
			}
			if (return_type == typeof(void))
			{
				return Expression.GetActionType(parameters.Select<ParameterExpression, Type>((ParameterExpression p) => p.Type).ToArray<Type>());
			}
			Type[] array = new Type[parameters.Length + 1];
			for (int i = 0; i < array.Length - 1; i++)
			{
				array[i] = parameters[i].Type;
			}
			array[array.Length - 1] = return_type;
			return Expression.GetFuncType(array);
		}

		public static LambdaExpression Lambda(Type delegateType, Expression body, params ParameterExpression[] parameters)
		{
			return Expression.Lambda(delegateType, body, parameters);
		}

		private static LambdaExpression CreateExpressionOf(Type type, Expression body, ReadOnlyCollection<ParameterExpression> parameters)
		{
			return (LambdaExpression)typeof(Expression<>).MakeGenericType(new Type[] { type }).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy, null, new Type[]
			{
				typeof(Expression),
				typeof(ReadOnlyCollection<ParameterExpression>)
			}, null).Invoke(new object[] { body, parameters });
		}

		public static LambdaExpression Lambda(Type delegateType, Expression body, IEnumerable<ParameterExpression> parameters)
		{
			if (delegateType == null)
			{
				throw new ArgumentNullException("delegateType");
			}
			if (body == null)
			{
				throw new ArgumentNullException("body");
			}
			ReadOnlyCollection<ParameterExpression> readOnlyCollection = parameters.ToReadOnlyCollection<ParameterExpression>();
			body = Expression.CheckLambda(delegateType, body, readOnlyCollection);
			return Expression.CreateExpressionOf(delegateType, body, readOnlyCollection);
		}

		public static MemberListBinding ListBind(MemberInfo member, params ElementInit[] initializers)
		{
			return Expression.ListBind(member, initializers);
		}

		private static void CheckIsAssignableToIEnumerable(Type t)
		{
			if (!t.IsAssignableTo(typeof(IEnumerable)))
			{
				throw new ArgumentException(string.Format("Type {0} doesn't implemen IEnumerable", t));
			}
		}

		public static MemberListBinding ListBind(MemberInfo member, IEnumerable<ElementInit> initializers)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			if (initializers == null)
			{
				throw new ArgumentNullException("initializers");
			}
			ReadOnlyCollection<ElementInit> readOnlyCollection = initializers.ToReadOnlyCollection<ElementInit>();
			Expression.CheckForNull<ElementInit>(readOnlyCollection, "initializers");
			member.OnFieldOrProperty(delegate(FieldInfo field)
			{
				Expression.CheckIsAssignableToIEnumerable(field.FieldType);
			}, delegate(PropertyInfo prop)
			{
				Expression.CheckIsAssignableToIEnumerable(prop.PropertyType);
			});
			return new MemberListBinding(member, readOnlyCollection);
		}

		public static MemberListBinding ListBind(MethodInfo propertyAccessor, params ElementInit[] initializers)
		{
			return Expression.ListBind(propertyAccessor, initializers);
		}

		private static void CheckForNull<T>(ReadOnlyCollection<T> collection, string name) where T : class
		{
			foreach (T t in collection)
			{
				if (t == null)
				{
					throw new ArgumentNullException(name);
				}
			}
		}

		public static MemberListBinding ListBind(MethodInfo propertyAccessor, IEnumerable<ElementInit> initializers)
		{
			if (propertyAccessor == null)
			{
				throw new ArgumentNullException("propertyAccessor");
			}
			if (initializers == null)
			{
				throw new ArgumentNullException("initializers");
			}
			ReadOnlyCollection<ElementInit> readOnlyCollection = initializers.ToReadOnlyCollection<ElementInit>();
			Expression.CheckForNull<ElementInit>(readOnlyCollection, "initializers");
			PropertyInfo associatedProperty = Expression.GetAssociatedProperty(propertyAccessor);
			if (associatedProperty == null)
			{
				throw new ArgumentException("propertyAccessor");
			}
			Expression.CheckIsAssignableToIEnumerable(associatedProperty.PropertyType);
			return new MemberListBinding(associatedProperty, readOnlyCollection);
		}

		public static ListInitExpression ListInit(NewExpression newExpression, params ElementInit[] initializers)
		{
			return Expression.ListInit(newExpression, initializers);
		}

		public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<ElementInit> initializers)
		{
			ReadOnlyCollection<ElementInit> readOnlyCollection = Expression.CheckListInit<ElementInit>(newExpression, initializers);
			return new ListInitExpression(newExpression, readOnlyCollection);
		}

		public static ListInitExpression ListInit(NewExpression newExpression, params Expression[] initializers)
		{
			return Expression.ListInit(newExpression, initializers);
		}

		public static ListInitExpression ListInit(NewExpression newExpression, IEnumerable<Expression> initializers)
		{
			ReadOnlyCollection<Expression> readOnlyCollection = Expression.CheckListInit<Expression>(newExpression, initializers);
			MethodInfo addMethod = Expression.GetAddMethod(newExpression.Type, readOnlyCollection[0].Type);
			if (addMethod == null)
			{
				throw new InvalidOperationException("No suitable add method found");
			}
			return new ListInitExpression(newExpression, Expression.CreateInitializers(addMethod, readOnlyCollection));
		}

		private static ReadOnlyCollection<ElementInit> CreateInitializers(MethodInfo add_method, ReadOnlyCollection<Expression> initializers)
		{
			return initializers.Select<Expression, ElementInit>((Expression init) => Expression.ElementInit(add_method, new Expression[] { init })).ToReadOnlyCollection<ElementInit>();
		}

		private static MethodInfo GetAddMethod(Type type, Type arg)
		{
			return type.GetMethod("Add", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy, null, new Type[] { arg }, null);
		}

		public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, params Expression[] initializers)
		{
			return Expression.ListInit(newExpression, addMethod, initializers);
		}

		private static ReadOnlyCollection<T> CheckListInit<T>(NewExpression newExpression, IEnumerable<T> initializers) where T : class
		{
			if (newExpression == null)
			{
				throw new ArgumentNullException("newExpression");
			}
			if (initializers == null)
			{
				throw new ArgumentNullException("initializers");
			}
			if (!newExpression.Type.IsAssignableTo(typeof(IEnumerable)))
			{
				throw new InvalidOperationException("The type of the new expression does not implement IEnumerable");
			}
			ReadOnlyCollection<T> readOnlyCollection = initializers.ToReadOnlyCollection<T>();
			if (readOnlyCollection.Count == 0)
			{
				throw new ArgumentException("Empty initializers");
			}
			Expression.CheckForNull<T>(readOnlyCollection, "initializers");
			return readOnlyCollection;
		}

		public static ListInitExpression ListInit(NewExpression newExpression, MethodInfo addMethod, IEnumerable<Expression> initializers)
		{
			ReadOnlyCollection<Expression> readOnlyCollection = Expression.CheckListInit<Expression>(newExpression, initializers);
			if (addMethod != null)
			{
				if (addMethod.Name.ToLower(CultureInfo.InvariantCulture) != "add")
				{
					throw new ArgumentException("addMethod");
				}
				ParameterInfo[] parameters = addMethod.GetParameters();
				if (parameters.Length != 1)
				{
					throw new ArgumentException("addMethod");
				}
				foreach (Expression expression in readOnlyCollection)
				{
					if (!Expression.IsAssignableToParameterType(expression.Type, parameters[0]))
					{
						throw new InvalidOperationException("Initializer not assignable to the add method parameter type");
					}
				}
			}
			if (addMethod == null)
			{
				addMethod = Expression.GetAddMethod(newExpression.Type, readOnlyCollection[0].Type);
			}
			if (addMethod == null)
			{
				throw new InvalidOperationException("No suitable add method found");
			}
			return new ListInitExpression(newExpression, Expression.CreateInitializers(addMethod, readOnlyCollection));
		}

		public static MemberExpression MakeMemberAccess(Expression expression, MemberInfo member)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo != null)
			{
				return Expression.Field(expression, fieldInfo);
			}
			PropertyInfo propertyInfo = member as PropertyInfo;
			if (propertyInfo != null)
			{
				return Expression.Property(expression, propertyInfo);
			}
			throw new ArgumentException("Member should either be a field or a property");
		}

		public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type)
		{
			return Expression.MakeUnary(unaryType, operand, type, null);
		}

		public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type, MethodInfo method)
		{
			switch (unaryType)
			{
			case ExpressionType.Negate:
				return Expression.Negate(operand, method);
			case ExpressionType.UnaryPlus:
				return Expression.UnaryPlus(operand, method);
			case ExpressionType.NegateChecked:
				return Expression.NegateChecked(operand, method);
			default:
				if (unaryType == ExpressionType.Convert)
				{
					return Expression.Convert(operand, type, method);
				}
				if (unaryType == ExpressionType.ConvertChecked)
				{
					return Expression.ConvertChecked(operand, type, method);
				}
				if (unaryType == ExpressionType.ArrayLength)
				{
					return Expression.ArrayLength(operand);
				}
				if (unaryType == ExpressionType.Quote)
				{
					return Expression.Quote(operand);
				}
				if (unaryType != ExpressionType.TypeAs)
				{
					throw new ArgumentException("MakeUnary expect an unary operator");
				}
				return Expression.TypeAs(operand, type);
			case ExpressionType.Not:
				return Expression.Not(operand, method);
			}
		}

		public static MemberMemberBinding MemberBind(MemberInfo member, params MemberBinding[] bindings)
		{
			return Expression.MemberBind(member, bindings);
		}

		public static MemberMemberBinding MemberBind(MemberInfo member, IEnumerable<MemberBinding> bindings)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			Type type = member.OnFieldOrProperty<Type>((FieldInfo field) => field.FieldType, (PropertyInfo prop) => prop.PropertyType);
			return new MemberMemberBinding(member, Expression.CheckMemberBindings(type, bindings));
		}

		public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, params MemberBinding[] bindings)
		{
			return Expression.MemberBind(propertyAccessor, bindings);
		}

		public static MemberMemberBinding MemberBind(MethodInfo propertyAccessor, IEnumerable<MemberBinding> bindings)
		{
			if (propertyAccessor == null)
			{
				throw new ArgumentNullException("propertyAccessor");
			}
			ReadOnlyCollection<MemberBinding> readOnlyCollection = bindings.ToReadOnlyCollection<MemberBinding>();
			Expression.CheckForNull<MemberBinding>(readOnlyCollection, "bindings");
			PropertyInfo associatedProperty = Expression.GetAssociatedProperty(propertyAccessor);
			if (associatedProperty == null)
			{
				throw new ArgumentException("propertyAccessor");
			}
			return new MemberMemberBinding(associatedProperty, Expression.CheckMemberBindings(associatedProperty.PropertyType, bindings));
		}

		private static ReadOnlyCollection<MemberBinding> CheckMemberBindings(Type type, IEnumerable<MemberBinding> bindings)
		{
			if (bindings == null)
			{
				throw new ArgumentNullException("bindings");
			}
			ReadOnlyCollection<MemberBinding> readOnlyCollection = bindings.ToReadOnlyCollection<MemberBinding>();
			Expression.CheckForNull<MemberBinding>(readOnlyCollection, "bindings");
			foreach (MemberBinding memberBinding in readOnlyCollection)
			{
				if (!type.IsAssignableTo(memberBinding.Member.DeclaringType))
				{
					throw new ArgumentException("Type not assignable to member type");
				}
			}
			return readOnlyCollection;
		}

		public static MemberInitExpression MemberInit(NewExpression newExpression, params MemberBinding[] bindings)
		{
			return Expression.MemberInit(newExpression, bindings);
		}

		public static MemberInitExpression MemberInit(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
		{
			if (newExpression == null)
			{
				throw new ArgumentNullException("newExpression");
			}
			return new MemberInitExpression(newExpression, Expression.CheckMemberBindings(newExpression.Type, bindings));
		}

		public static UnaryExpression Negate(Expression expression)
		{
			return Expression.Negate(expression, null);
		}

		public static UnaryExpression Negate(Expression expression, MethodInfo method)
		{
			method = Expression.UnaryCoreCheck("op_UnaryNegation", expression, method, (Type type) => Expression.IsSignedNumber(type));
			return Expression.MakeSimpleUnary(ExpressionType.Negate, expression, method);
		}

		public static UnaryExpression NegateChecked(Expression expression)
		{
			return Expression.NegateChecked(expression, null);
		}

		public static UnaryExpression NegateChecked(Expression expression, MethodInfo method)
		{
			method = Expression.UnaryCoreCheck("op_UnaryNegation", expression, method, (Type type) => Expression.IsSignedNumber(type));
			return Expression.MakeSimpleUnary(ExpressionType.NegateChecked, expression, method);
		}

		public static NewExpression New(ConstructorInfo constructor)
		{
			if (constructor == null)
			{
				throw new ArgumentNullException("constructor");
			}
			if (constructor.GetParameters().Length > 0)
			{
				throw new ArgumentException("Constructor must be parameter less");
			}
			return new NewExpression(constructor, Enumerable.ToReadOnlyCollection<Expression>((IEnumerable<TSource>)null), null);
		}

		public static NewExpression New(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Expression.CheckNotVoid(type);
			ReadOnlyCollection<Expression> readOnlyCollection = Enumerable.ToReadOnlyCollection<Expression>((IEnumerable<TSource>)null);
			if (type.IsValueType)
			{
				return new NewExpression(type, readOnlyCollection);
			}
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor == null)
			{
				throw new ArgumentException("Type doesn't have a parameter less constructor");
			}
			return new NewExpression(constructor, readOnlyCollection, null);
		}

		public static NewExpression New(ConstructorInfo constructor, params Expression[] arguments)
		{
			return Expression.New(constructor, arguments);
		}

		public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments)
		{
			if (constructor == null)
			{
				throw new ArgumentNullException("constructor");
			}
			ReadOnlyCollection<Expression> readOnlyCollection = Expression.CheckMethodArguments(constructor, arguments);
			return new NewExpression(constructor, readOnlyCollection, null);
		}

		private static IList<Expression> CreateArgumentList(IEnumerable<Expression> arguments)
		{
			if (arguments == null)
			{
				return arguments.ToReadOnlyCollection<Expression>();
			}
			return arguments.ToList<Expression>();
		}

		private static void CheckNonGenericMethod(MethodBase method)
		{
			if (method.IsGenericMethodDefinition || method.ContainsGenericParameters)
			{
				throw new ArgumentException("Can not used open generic methods");
			}
		}

		private static ReadOnlyCollection<Expression> CheckMethodArguments(MethodBase method, IEnumerable<Expression> args)
		{
			Expression.CheckNonGenericMethod(method);
			IList<Expression> list = Expression.CreateArgumentList(args);
			ParameterInfo[] parameters = method.GetParameters();
			if (list.Count != parameters.Length)
			{
				throw new ArgumentException("The number of arguments doesn't match the number of parameters");
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				if (list[i] == null)
				{
					throw new ArgumentNullException("arguments");
				}
				if (!Expression.IsAssignableToParameterType(list[i].Type, parameters[i]))
				{
					if (!parameters[i].ParameterType.IsExpression())
					{
						throw new ArgumentException("arguments");
					}
					list[i] = Expression.Quote(list[i]);
				}
			}
			return list.ToReadOnlyCollection<Expression>();
		}

		public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, params MemberInfo[] members)
		{
			return Expression.New(constructor, arguments, members);
		}

		public static NewExpression New(ConstructorInfo constructor, IEnumerable<Expression> arguments, IEnumerable<MemberInfo> members)
		{
			if (constructor == null)
			{
				throw new ArgumentNullException("constructor");
			}
			ReadOnlyCollection<Expression> readOnlyCollection = arguments.ToReadOnlyCollection<Expression>();
			ReadOnlyCollection<MemberInfo> readOnlyCollection2 = members.ToReadOnlyCollection<MemberInfo>();
			Expression.CheckForNull<Expression>(readOnlyCollection, "arguments");
			Expression.CheckForNull<MemberInfo>(readOnlyCollection2, "members");
			readOnlyCollection = Expression.CheckMethodArguments(constructor, arguments);
			if (readOnlyCollection.Count != readOnlyCollection2.Count)
			{
				throw new ArgumentException("Arguments count does not match members count");
			}
			for (int i = 0; i < readOnlyCollection2.Count; i++)
			{
				MemberInfo memberInfo = readOnlyCollection2[i];
				MemberTypes memberType = memberInfo.MemberType;
				Type type;
				if (memberType != MemberTypes.Field)
				{
					if (memberType != MemberTypes.Method)
					{
						if (memberType != MemberTypes.Property)
						{
							throw new ArgumentException("Member type not allowed");
						}
						PropertyInfo propertyInfo = memberInfo as PropertyInfo;
						if (propertyInfo.GetGetMethod(true) == null)
						{
							throw new ArgumentException("Property must have a getter");
						}
						type = (memberInfo as PropertyInfo).PropertyType;
					}
					else
					{
						type = (memberInfo as MethodInfo).ReturnType;
					}
				}
				else
				{
					type = (memberInfo as FieldInfo).FieldType;
				}
				if (!readOnlyCollection[i].Type.IsAssignableTo(type))
				{
					throw new ArgumentException("Argument type not assignable to member type");
				}
			}
			return new NewExpression(constructor, readOnlyCollection, readOnlyCollection2);
		}

		public static NewArrayExpression NewArrayBounds(Type type, params Expression[] bounds)
		{
			return Expression.NewArrayBounds(type, bounds);
		}

		public static NewArrayExpression NewArrayBounds(Type type, IEnumerable<Expression> bounds)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (bounds == null)
			{
				throw new ArgumentNullException("bounds");
			}
			Expression.CheckNotVoid(type);
			ReadOnlyCollection<Expression> readOnlyCollection = bounds.ToReadOnlyCollection<Expression>();
			foreach (Expression expression in readOnlyCollection)
			{
				if (!Expression.IsInt(expression.Type))
				{
					throw new ArgumentException("The bounds collection can only contain expression of integers types");
				}
			}
			return new NewArrayExpression(ExpressionType.NewArrayBounds, type.MakeArrayType(readOnlyCollection.Count), readOnlyCollection);
		}

		public static NewArrayExpression NewArrayInit(Type type, params Expression[] initializers)
		{
			return Expression.NewArrayInit(type, initializers);
		}

		public static NewArrayExpression NewArrayInit(Type type, IEnumerable<Expression> initializers)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (initializers == null)
			{
				throw new ArgumentNullException("initializers");
			}
			Expression.CheckNotVoid(type);
			ReadOnlyCollection<Expression> readOnlyCollection = initializers.ToReadOnlyCollection<Expression>();
			foreach (Expression expression in readOnlyCollection)
			{
				if (expression == null)
				{
					throw new ArgumentNullException("initializers");
				}
				if (!expression.Type.IsAssignableTo(type))
				{
					throw new InvalidOperationException(string.Format("{0} IsAssignableTo {1}, expression [ {2} ] : {3}", new object[] { expression.Type, type, expression.NodeType, expression }));
				}
			}
			return new NewArrayExpression(ExpressionType.NewArrayInit, type.MakeArrayType(), readOnlyCollection);
		}

		public static UnaryExpression Not(Expression expression)
		{
			return Expression.Not(expression, null);
		}

		public static UnaryExpression Not(Expression expression, MethodInfo method)
		{
			Func<Type, bool> func = (Type type) => Expression.IsIntOrBool(type);
			method = Expression.UnaryCoreCheck("op_LogicalNot", expression, method, func);
			if (method == null)
			{
				method = Expression.UnaryCoreCheck("op_OnesComplement", expression, method, func);
			}
			return Expression.MakeSimpleUnary(ExpressionType.Not, expression, method);
		}

		private static void CheckNotVoid(Type type)
		{
			if (type == typeof(void))
			{
				throw new ArgumentException("Type can't be void");
			}
		}

		public static ParameterExpression Parameter(Type type, string name)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Expression.CheckNotVoid(type);
			return new ParameterExpression(type, name);
		}

		public static MemberExpression Property(Expression expression, MethodInfo propertyAccessor)
		{
			if (propertyAccessor == null)
			{
				throw new ArgumentNullException("propertyAccessor");
			}
			Expression.CheckNonGenericMethod(propertyAccessor);
			if (!propertyAccessor.IsStatic)
			{
				if (expression == null)
				{
					throw new ArgumentNullException("expression");
				}
				if (!expression.Type.IsAssignableTo(propertyAccessor.DeclaringType))
				{
					throw new ArgumentException("expression");
				}
			}
			PropertyInfo associatedProperty = Expression.GetAssociatedProperty(propertyAccessor);
			if (associatedProperty == null)
			{
				throw new ArgumentException(string.Format("Method {0} has no associated property", propertyAccessor));
			}
			return new MemberExpression(expression, associatedProperty, associatedProperty.PropertyType);
		}

		private static PropertyInfo GetAssociatedProperty(MethodInfo method)
		{
			if (method == null)
			{
				return null;
			}
			foreach (PropertyInfo propertyInfo in method.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
			{
				if (method.Equals(propertyInfo.GetGetMethod(true)))
				{
					return propertyInfo;
				}
				if (method.Equals(propertyInfo.GetSetMethod(true)))
				{
					return propertyInfo;
				}
			}
			return null;
		}

		public static MemberExpression Property(Expression expression, PropertyInfo property)
		{
			if (property == null)
			{
				throw new ArgumentNullException("property");
			}
			MethodInfo getMethod = property.GetGetMethod(true);
			if (getMethod == null)
			{
				throw new ArgumentException("getter");
			}
			if (!getMethod.IsStatic)
			{
				if (expression == null)
				{
					throw new ArgumentNullException("expression");
				}
				if (!expression.Type.IsAssignableTo(property.DeclaringType))
				{
					throw new ArgumentException("expression");
				}
			}
			else if (expression != null)
			{
				throw new ArgumentException("expression");
			}
			return new MemberExpression(expression, property, property.PropertyType);
		}

		public static MemberExpression Property(Expression expression, string propertyName)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			PropertyInfo property = expression.Type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (property == null)
			{
				throw new ArgumentException(string.Format("No property named {0} on {1}", propertyName, expression.Type));
			}
			return new MemberExpression(expression, property, property.PropertyType);
		}

		public static MemberExpression PropertyOrField(Expression expression, string propertyOrFieldName)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (propertyOrFieldName == null)
			{
				throw new ArgumentNullException("propertyOrFieldName");
			}
			PropertyInfo property = expression.Type.GetProperty(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (property != null)
			{
				return new MemberExpression(expression, property, property.PropertyType);
			}
			FieldInfo field = expression.Type.GetField(propertyOrFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (field != null)
			{
				return new MemberExpression(expression, field, field.FieldType);
			}
			throw new ArgumentException(string.Format("No field or property named {0} on {1}", propertyOrFieldName, expression.Type));
		}

		public static UnaryExpression Quote(Expression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			return new UnaryExpression(ExpressionType.Quote, expression, expression.GetType());
		}

		public static UnaryExpression TypeAs(Expression expression, Type type)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type.IsValueType && !type.IsNullable())
			{
				throw new ArgumentException("TypeAs expect a reference or a nullable type");
			}
			return new UnaryExpression(ExpressionType.TypeAs, expression, type);
		}

		public static TypeBinaryExpression TypeIs(Expression expression, Type type)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Expression.CheckNotVoid(type);
			return new TypeBinaryExpression(ExpressionType.TypeIs, expression, type, typeof(bool));
		}

		public static UnaryExpression UnaryPlus(Expression expression)
		{
			return Expression.UnaryPlus(expression, null);
		}

		public static UnaryExpression UnaryPlus(Expression expression, MethodInfo method)
		{
			method = Expression.UnaryCoreCheck("op_UnaryPlus", expression, method, (Type type) => Expression.IsNumber(type));
			return Expression.MakeSimpleUnary(ExpressionType.UnaryPlus, expression, method);
		}

		private static bool IsInt(Type t)
		{
			return t == typeof(byte) || t == typeof(sbyte) || t == typeof(short) || t == typeof(ushort) || t == typeof(int) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong);
		}

		private static bool IsIntOrBool(Type t)
		{
			return Expression.IsInt(t) || t == typeof(bool);
		}

		private static bool IsNumber(Type t)
		{
			return Expression.IsInt(t) || t == typeof(float) || t == typeof(double);
		}

		private static bool IsSignedNumber(Type t)
		{
			return Expression.IsNumber(t) && !Expression.IsUnsigned(t);
		}

		internal static bool IsUnsigned(Type t)
		{
			if (t.IsPointer)
			{
				return Expression.IsUnsigned(t.GetElementType());
			}
			return t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong) || t == typeof(byte);
		}

		internal virtual void Emit(EmitContext ec)
		{
			throw new NotImplementedException(string.Format("Emit method is not implemented in expression type {0}", base.GetType()));
		}

		internal const BindingFlags PublicInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

		internal const BindingFlags NonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		internal const BindingFlags PublicStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

		internal const BindingFlags AllInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		internal const BindingFlags AllStatic = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		internal const BindingFlags All = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

		private ExpressionType node_type;

		private Type type;
	}
}
