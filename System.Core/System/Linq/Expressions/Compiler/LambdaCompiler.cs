using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;

namespace System.Linq.Expressions.Compiler
{
	internal sealed class LambdaCompiler : ILocalCache
	{
		private void EmitAddress(Expression node, Type type)
		{
			this.EmitAddress(node, type, LambdaCompiler.CompilationFlags.EmitExpressionStart);
		}

		private void EmitAddress(Expression node, Type type, LambdaCompiler.CompilationFlags flags)
		{
			bool flag = (flags & LambdaCompiler.CompilationFlags.EmitExpressionStartMask) == LambdaCompiler.CompilationFlags.EmitExpressionStart;
			LambdaCompiler.CompilationFlags compilationFlags = (flag ? this.EmitExpressionStart(node) : LambdaCompiler.CompilationFlags.EmitNoExpressionStart);
			ExpressionType nodeType = node.NodeType;
			if (nodeType <= ExpressionType.MemberAccess)
			{
				if (nodeType == ExpressionType.ArrayIndex)
				{
					this.AddressOf((BinaryExpression)node, type);
					goto IL_00A2;
				}
				if (nodeType == ExpressionType.Call)
				{
					this.AddressOf((MethodCallExpression)node, type);
					goto IL_00A2;
				}
				if (nodeType == ExpressionType.MemberAccess)
				{
					this.AddressOf((MemberExpression)node, type);
					goto IL_00A2;
				}
			}
			else
			{
				if (nodeType == ExpressionType.Parameter)
				{
					this.AddressOf((ParameterExpression)node, type);
					goto IL_00A2;
				}
				if (nodeType == ExpressionType.Index)
				{
					this.AddressOf((IndexExpression)node, type);
					goto IL_00A2;
				}
				if (nodeType == ExpressionType.Unbox)
				{
					this.AddressOf((UnaryExpression)node, type);
					goto IL_00A2;
				}
			}
			this.EmitExpressionAddress(node, type);
			IL_00A2:
			if (flag)
			{
				this.EmitExpressionEnd(compilationFlags);
			}
		}

		private void AddressOf(BinaryExpression node, Type type)
		{
			if (TypeUtils.AreEquivalent(type, node.Type))
			{
				this.EmitExpression(node.Left);
				this.EmitExpression(node.Right);
				this._ilg.Emit(OpCodes.Ldelema, node.Type);
				return;
			}
			this.EmitExpressionAddress(node, type);
		}

		private void AddressOf(ParameterExpression node, Type type)
		{
			if (TypeUtils.AreEquivalent(type, node.Type))
			{
				if (node.IsByRef)
				{
					this._scope.EmitGet(node);
					return;
				}
				this._scope.EmitAddressOf(node);
				return;
			}
			else
			{
				if (node.Type.IsByRef && node.Type.GetElementType() == type)
				{
					this.EmitExpression(node);
					return;
				}
				this.EmitExpressionAddress(node, type);
				return;
			}
		}

		private void AddressOf(MemberExpression node, Type type)
		{
			if (TypeUtils.AreEquivalent(type, node.Type))
			{
				Type type2 = null;
				if (node.Expression != null)
				{
					this.EmitInstance(node.Expression, out type2);
				}
				this.EmitMemberAddress(node.Member, type2);
				return;
			}
			this.EmitExpressionAddress(node, type);
		}

		private void EmitMemberAddress(MemberInfo member, Type objectType)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo != null && !fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
			{
				this._ilg.EmitFieldAddress(fieldInfo);
				return;
			}
			this.EmitMemberGet(member, objectType);
			LocalBuilder local = this.GetLocal(LambdaCompiler.GetMemberType(member));
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldloca, local);
		}

		private void AddressOf(MethodCallExpression node, Type type)
		{
			if (!node.Method.IsStatic && node.Object.Type.IsArray && node.Method == node.Object.Type.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public))
			{
				MethodInfo method = node.Object.Type.GetMethod("Address", BindingFlags.Instance | BindingFlags.Public);
				this.EmitMethodCall(node.Object, method, node);
				return;
			}
			this.EmitExpressionAddress(node, type);
		}

		private void AddressOf(IndexExpression node, Type type)
		{
			if (!TypeUtils.AreEquivalent(type, node.Type) || node.Indexer != null)
			{
				this.EmitExpressionAddress(node, type);
				return;
			}
			if (node.ArgumentCount == 1)
			{
				this.EmitExpression(node.Object);
				this.EmitExpression(node.GetArgument(0));
				this._ilg.Emit(OpCodes.Ldelema, node.Type);
				return;
			}
			MethodInfo method = node.Object.Type.GetMethod("Address", BindingFlags.Instance | BindingFlags.Public);
			this.EmitMethodCall(node.Object, method, node);
		}

		private void AddressOf(UnaryExpression node, Type type)
		{
			this.EmitExpression(node.Operand);
			this._ilg.Emit(OpCodes.Unbox, type);
		}

		private void EmitExpressionAddress(Expression node, Type type)
		{
			this.EmitExpression(node, LambdaCompiler.CompilationFlags.EmitNoExpressionStart | LambdaCompiler.CompilationFlags.EmitAsNoTail);
			LocalBuilder local = this.GetLocal(type);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldloca, local);
		}

		private LambdaCompiler.WriteBack EmitAddressWriteBack(Expression node, Type type)
		{
			LambdaCompiler.CompilationFlags compilationFlags = this.EmitExpressionStart(node);
			LambdaCompiler.WriteBack writeBack = null;
			if (TypeUtils.AreEquivalent(type, node.Type))
			{
				ExpressionType nodeType = node.NodeType;
				if (nodeType != ExpressionType.MemberAccess)
				{
					if (nodeType == ExpressionType.Index)
					{
						writeBack = this.AddressOfWriteBack((IndexExpression)node);
					}
				}
				else
				{
					writeBack = this.AddressOfWriteBack((MemberExpression)node);
				}
			}
			if (writeBack == null)
			{
				this.EmitAddress(node, type, LambdaCompiler.CompilationFlags.EmitNoExpressionStart | LambdaCompiler.CompilationFlags.EmitAsNoTail);
			}
			this.EmitExpressionEnd(compilationFlags);
			return writeBack;
		}

		private LambdaCompiler.WriteBack AddressOfWriteBack(MemberExpression node)
		{
			PropertyInfo propertyInfo = node.Member as PropertyInfo;
			if (propertyInfo == null || !propertyInfo.CanWrite)
			{
				return null;
			}
			return this.AddressOfWriteBackCore(node);
		}

		private LambdaCompiler.WriteBack AddressOfWriteBackCore(MemberExpression node)
		{
			LocalBuilder instanceLocal = null;
			Type type = null;
			if (node.Expression != null)
			{
				this.EmitInstance(node.Expression, out type);
				this._ilg.Emit(OpCodes.Dup);
				this._ilg.Emit(OpCodes.Stloc, instanceLocal = this.GetInstanceLocal(type));
			}
			PropertyInfo pi = (PropertyInfo)node.Member;
			this.EmitCall(type, pi.GetGetMethod(true));
			LocalBuilder valueLocal = this.GetLocal(node.Type);
			this._ilg.Emit(OpCodes.Stloc, valueLocal);
			this._ilg.Emit(OpCodes.Ldloca, valueLocal);
			return delegate(LambdaCompiler @this)
			{
				if (instanceLocal != null)
				{
					@this._ilg.Emit(OpCodes.Ldloc, instanceLocal);
					@this.FreeLocal(instanceLocal);
				}
				@this._ilg.Emit(OpCodes.Ldloc, valueLocal);
				@this.FreeLocal(valueLocal);
				LocalBuilder instanceLocal2 = instanceLocal;
				@this.EmitCall((instanceLocal2 != null) ? instanceLocal2.LocalType : null, pi.GetSetMethod(true));
			};
		}

		private LambdaCompiler.WriteBack AddressOfWriteBack(IndexExpression node)
		{
			if (node.Indexer == null || !node.Indexer.CanWrite)
			{
				return null;
			}
			return this.AddressOfWriteBackCore(node);
		}

		private LambdaCompiler.WriteBack AddressOfWriteBackCore(IndexExpression node)
		{
			LocalBuilder instanceLocal = null;
			Type type = null;
			if (node.Object != null)
			{
				this.EmitInstance(node.Object, out type);
				this._ilg.Emit(OpCodes.Dup);
				this._ilg.Emit(OpCodes.Stloc, instanceLocal = this.GetInstanceLocal(type));
			}
			int argumentCount = node.ArgumentCount;
			LocalBuilder[] args = new LocalBuilder[argumentCount];
			for (int i = 0; i < argumentCount; i++)
			{
				Expression argument = node.GetArgument(i);
				this.EmitExpression(argument);
				LocalBuilder local = this.GetLocal(argument.Type);
				this._ilg.Emit(OpCodes.Dup);
				this._ilg.Emit(OpCodes.Stloc, local);
				args[i] = local;
			}
			this.EmitGetIndexCall(node, type);
			LocalBuilder valueLocal = this.GetLocal(node.Type);
			this._ilg.Emit(OpCodes.Stloc, valueLocal);
			this._ilg.Emit(OpCodes.Ldloca, valueLocal);
			return delegate(LambdaCompiler @this)
			{
				if (instanceLocal != null)
				{
					@this._ilg.Emit(OpCodes.Ldloc, instanceLocal);
					@this.FreeLocal(instanceLocal);
				}
				foreach (LocalBuilder localBuilder in args)
				{
					@this._ilg.Emit(OpCodes.Ldloc, localBuilder);
					@this.FreeLocal(localBuilder);
				}
				@this._ilg.Emit(OpCodes.Ldloc, valueLocal);
				@this.FreeLocal(valueLocal);
				IndexExpression node2 = node;
				LocalBuilder instanceLocal2 = instanceLocal;
				@this.EmitSetIndexCall(node2, (instanceLocal2 != null) ? instanceLocal2.LocalType : null);
			};
		}

		private LocalBuilder GetInstanceLocal(Type type)
		{
			Type type2 = (type.IsValueType ? type.MakeByRefType() : type);
			return this.GetLocal(type2);
		}

		private void EmitBinaryExpression(Expression expr)
		{
			this.EmitBinaryExpression(expr, LambdaCompiler.CompilationFlags.EmitAsNoTail);
		}

		private void EmitBinaryExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			BinaryExpression binaryExpression = (BinaryExpression)expr;
			if (binaryExpression.Method != null)
			{
				this.EmitBinaryMethod(binaryExpression, flags);
				return;
			}
			if ((binaryExpression.NodeType == ExpressionType.Equal || binaryExpression.NodeType == ExpressionType.NotEqual) && (binaryExpression.Type == typeof(bool) || binaryExpression.Type == typeof(bool?)))
			{
				if (ConstantCheck.IsNull(binaryExpression.Left) && !ConstantCheck.IsNull(binaryExpression.Right) && binaryExpression.Right.Type.IsNullableType())
				{
					this.EmitNullEquality(binaryExpression.NodeType, binaryExpression.Right, binaryExpression.IsLiftedToNull);
					return;
				}
				if (ConstantCheck.IsNull(binaryExpression.Right) && !ConstantCheck.IsNull(binaryExpression.Left) && binaryExpression.Left.Type.IsNullableType())
				{
					this.EmitNullEquality(binaryExpression.NodeType, binaryExpression.Left, binaryExpression.IsLiftedToNull);
					return;
				}
				this.EmitExpression(LambdaCompiler.GetEqualityOperand(binaryExpression.Left));
				this.EmitExpression(LambdaCompiler.GetEqualityOperand(binaryExpression.Right));
			}
			else
			{
				this.EmitExpression(binaryExpression.Left);
				this.EmitExpression(binaryExpression.Right);
			}
			this.EmitBinaryOperator(binaryExpression.NodeType, binaryExpression.Left.Type, binaryExpression.Right.Type, binaryExpression.Type, binaryExpression.IsLiftedToNull);
		}

		private void EmitNullEquality(ExpressionType op, Expression e, bool isLiftedToNull)
		{
			if (isLiftedToNull)
			{
				this.EmitExpressionAsVoid(e);
				this._ilg.EmitDefault(typeof(bool?), this);
				return;
			}
			this.EmitAddress(e, e.Type);
			this._ilg.EmitHasValue(e.Type);
			if (op == ExpressionType.Equal)
			{
				this._ilg.Emit(OpCodes.Ldc_I4_0);
				this._ilg.Emit(OpCodes.Ceq);
			}
		}

		private void EmitBinaryMethod(BinaryExpression b, LambdaCompiler.CompilationFlags flags)
		{
			if (b.IsLifted)
			{
				ParameterExpression parameterExpression = Expression.Variable(b.Left.Type.GetNonNullableType(), null);
				ParameterExpression parameterExpression2 = Expression.Variable(b.Right.Type.GetNonNullableType(), null);
				MethodCallExpression methodCallExpression = Expression.Call(null, b.Method, parameterExpression, parameterExpression2);
				Type type;
				if (b.IsLiftedToNull)
				{
					type = methodCallExpression.Type.GetNullableType();
				}
				else
				{
					type = typeof(bool);
				}
				this.EmitLift(b.NodeType, type, methodCallExpression, new ParameterExpression[] { parameterExpression, parameterExpression2 }, new Expression[] { b.Left, b.Right });
				return;
			}
			this.EmitMethodCallExpression(Expression.Call(null, b.Method, b.Left, b.Right), flags);
		}

		private void EmitBinaryOperator(ExpressionType op, Type leftType, Type rightType, Type resultType, bool liftedToNull)
		{
			if (op == ExpressionType.ArrayIndex)
			{
				this.EmitGetArrayElement(leftType);
				return;
			}
			if (leftType.IsNullableType() || rightType.IsNullableType())
			{
				this.EmitLiftedBinaryOp(op, leftType, rightType, resultType, liftedToNull);
				return;
			}
			this.EmitUnliftedBinaryOp(op, leftType, rightType);
		}

		private void EmitUnliftedBinaryOp(ExpressionType op, Type leftType, Type rightType)
		{
			switch (op)
			{
			case ExpressionType.Add:
				this._ilg.Emit(OpCodes.Add);
				goto IL_034E;
			case ExpressionType.AddChecked:
				this._ilg.Emit(leftType.IsFloatingPoint() ? OpCodes.Add : (leftType.IsUnsigned() ? OpCodes.Add_Ovf_Un : OpCodes.Add_Ovf));
				goto IL_034E;
			case ExpressionType.And:
			case ExpressionType.AndAlso:
				this._ilg.Emit(OpCodes.And);
				return;
			case ExpressionType.ArrayLength:
			case ExpressionType.ArrayIndex:
			case ExpressionType.Call:
			case ExpressionType.Coalesce:
			case ExpressionType.Conditional:
			case ExpressionType.Constant:
			case ExpressionType.Convert:
			case ExpressionType.ConvertChecked:
			case ExpressionType.Invoke:
			case ExpressionType.Lambda:
			case ExpressionType.ListInit:
			case ExpressionType.MemberAccess:
			case ExpressionType.MemberInit:
				goto IL_034E;
			case ExpressionType.Divide:
				this._ilg.Emit(leftType.IsUnsigned() ? OpCodes.Div_Un : OpCodes.Div);
				goto IL_034E;
			case ExpressionType.Equal:
				break;
			case ExpressionType.ExclusiveOr:
				goto IL_02FD;
			case ExpressionType.GreaterThan:
				this._ilg.Emit(leftType.IsUnsigned() ? OpCodes.Cgt_Un : OpCodes.Cgt);
				return;
			case ExpressionType.GreaterThanOrEqual:
				this._ilg.Emit((leftType.IsUnsigned() || leftType.IsFloatingPoint()) ? OpCodes.Clt_Un : OpCodes.Clt);
				this._ilg.Emit(OpCodes.Ldc_I4_0);
				this._ilg.Emit(OpCodes.Ceq);
				return;
			case ExpressionType.LeftShift:
				this.EmitShiftMask(leftType);
				this._ilg.Emit(OpCodes.Shl);
				goto IL_034E;
			case ExpressionType.LessThan:
				this._ilg.Emit(leftType.IsUnsigned() ? OpCodes.Clt_Un : OpCodes.Clt);
				return;
			case ExpressionType.LessThanOrEqual:
				this._ilg.Emit((leftType.IsUnsigned() || leftType.IsFloatingPoint()) ? OpCodes.Cgt_Un : OpCodes.Cgt);
				this._ilg.Emit(OpCodes.Ldc_I4_0);
				this._ilg.Emit(OpCodes.Ceq);
				return;
			case ExpressionType.Modulo:
				this._ilg.Emit(leftType.IsUnsigned() ? OpCodes.Rem_Un : OpCodes.Rem);
				return;
			case ExpressionType.Multiply:
				this._ilg.Emit(OpCodes.Mul);
				goto IL_034E;
			case ExpressionType.MultiplyChecked:
				this._ilg.Emit(leftType.IsFloatingPoint() ? OpCodes.Mul : (leftType.IsUnsigned() ? OpCodes.Mul_Ovf_Un : OpCodes.Mul_Ovf));
				goto IL_034E;
			default:
				switch (op)
				{
				case ExpressionType.NotEqual:
					if (leftType.GetTypeCode() == TypeCode.Boolean)
					{
						goto IL_02FD;
					}
					this._ilg.Emit(OpCodes.Ceq);
					this._ilg.Emit(OpCodes.Ldc_I4_0);
					break;
				case ExpressionType.Or:
				case ExpressionType.OrElse:
					this._ilg.Emit(OpCodes.Or);
					return;
				case ExpressionType.Parameter:
				case ExpressionType.Power:
				case ExpressionType.Quote:
					goto IL_034E;
				case ExpressionType.RightShift:
					this.EmitShiftMask(leftType);
					this._ilg.Emit(leftType.IsUnsigned() ? OpCodes.Shr_Un : OpCodes.Shr);
					return;
				case ExpressionType.Subtract:
					this._ilg.Emit(OpCodes.Sub);
					goto IL_034E;
				case ExpressionType.SubtractChecked:
					if (leftType.IsUnsigned())
					{
						this._ilg.Emit(OpCodes.Sub_Ovf_Un);
						return;
					}
					this._ilg.Emit(leftType.IsFloatingPoint() ? OpCodes.Sub : OpCodes.Sub_Ovf);
					goto IL_034E;
				default:
					goto IL_034E;
				}
				break;
			}
			this._ilg.Emit(OpCodes.Ceq);
			return;
			IL_02FD:
			this._ilg.Emit(OpCodes.Xor);
			return;
			IL_034E:
			this.EmitConvertArithmeticResult(op, leftType);
		}

		private void EmitShiftMask(Type leftType)
		{
			int num = (leftType.IsInteger64() ? 63 : 31);
			this._ilg.EmitPrimitive(num);
			this._ilg.Emit(OpCodes.And);
		}

		private void EmitConvertArithmeticResult(ExpressionType op, Type resultType)
		{
			switch (resultType.GetTypeCode())
			{
			case TypeCode.SByte:
				this._ilg.Emit(LambdaCompiler.IsChecked(op) ? OpCodes.Conv_Ovf_I1 : OpCodes.Conv_I1);
				return;
			case TypeCode.Byte:
				this._ilg.Emit(LambdaCompiler.IsChecked(op) ? OpCodes.Conv_Ovf_U1 : OpCodes.Conv_U1);
				return;
			case TypeCode.Int16:
				this._ilg.Emit(LambdaCompiler.IsChecked(op) ? OpCodes.Conv_Ovf_I2 : OpCodes.Conv_I2);
				return;
			case TypeCode.UInt16:
				this._ilg.Emit(LambdaCompiler.IsChecked(op) ? OpCodes.Conv_Ovf_U2 : OpCodes.Conv_U2);
				return;
			default:
				return;
			}
		}

		private void EmitLiftedBinaryOp(ExpressionType op, Type leftType, Type rightType, Type resultType, bool liftedToNull)
		{
			if (op > ExpressionType.And)
			{
				switch (op)
				{
				case ExpressionType.Divide:
				case ExpressionType.ExclusiveOr:
				case ExpressionType.LeftShift:
				case ExpressionType.Modulo:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
					goto IL_00CF;
				case ExpressionType.Equal:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.NotEqual:
					if (liftedToNull)
					{
						this.EmitLiftedToNullRelational(op, leftType);
						return;
					}
					this.EmitLiftedRelational(op, leftType);
					break;
				case ExpressionType.Invoke:
				case ExpressionType.Lambda:
				case ExpressionType.ListInit:
				case ExpressionType.MemberAccess:
				case ExpressionType.MemberInit:
				case ExpressionType.Negate:
				case ExpressionType.UnaryPlus:
				case ExpressionType.NegateChecked:
				case ExpressionType.New:
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
				case ExpressionType.Not:
					break;
				case ExpressionType.Or:
					if (leftType == typeof(bool?))
					{
						this.EmitLiftedBooleanOr();
						return;
					}
					this.EmitLiftedBinaryArithmetic(op, leftType, rightType, resultType);
					return;
				default:
					if (op - ExpressionType.RightShift > 2)
					{
						return;
					}
					goto IL_00CF;
				}
				return;
			}
			if (op > ExpressionType.AddChecked)
			{
				if (op != ExpressionType.And)
				{
					return;
				}
				if (leftType == typeof(bool?))
				{
					this.EmitLiftedBooleanAnd();
					return;
				}
				this.EmitLiftedBinaryArithmetic(op, leftType, rightType, resultType);
				return;
			}
			IL_00CF:
			this.EmitLiftedBinaryArithmetic(op, leftType, rightType, resultType);
		}

		private void EmitLiftedRelational(ExpressionType op, Type type)
		{
			bool flag = op == ExpressionType.NotEqual;
			if (flag)
			{
				op = ExpressionType.Equal;
			}
			LocalBuilder local = this.GetLocal(type);
			LocalBuilder local2 = this.GetLocal(type);
			this._ilg.Emit(OpCodes.Stloc, local2);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitGetValueOrDefault(type);
			this._ilg.Emit(OpCodes.Ldloca, local2);
			this._ilg.EmitGetValueOrDefault(type);
			Type nonNullableType = type.GetNonNullableType();
			this.EmitUnliftedBinaryOp(op, nonNullableType, nonNullableType);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitHasValue(type);
			this._ilg.Emit(OpCodes.Ldloca, local2);
			this._ilg.EmitHasValue(type);
			this.FreeLocal(local);
			this.FreeLocal(local2);
			this._ilg.Emit((op == ExpressionType.Equal) ? OpCodes.Ceq : OpCodes.And);
			this._ilg.Emit(OpCodes.And);
			if (flag)
			{
				this._ilg.Emit(OpCodes.Ldc_I4_0);
				this._ilg.Emit(OpCodes.Ceq);
			}
		}

		private void EmitLiftedToNullRelational(ExpressionType op, Type type)
		{
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			LocalBuilder local = this.GetLocal(type);
			LocalBuilder local2 = this.GetLocal(type);
			this._ilg.Emit(OpCodes.Stloc, local2);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitHasValue(type);
			this._ilg.Emit(OpCodes.Ldloca, local2);
			this._ilg.EmitHasValue(type);
			this._ilg.Emit(OpCodes.And);
			this._ilg.Emit(OpCodes.Brtrue_S, label);
			this._ilg.EmitDefault(typeof(bool?), this);
			this._ilg.Emit(OpCodes.Br_S, label2);
			this._ilg.MarkLabel(label);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitGetValueOrDefault(type);
			this._ilg.Emit(OpCodes.Ldloca, local2);
			this._ilg.EmitGetValueOrDefault(type);
			this.FreeLocal(local);
			this.FreeLocal(local2);
			Type nonNullableType = type.GetNonNullableType();
			this.EmitUnliftedBinaryOp(op, nonNullableType, nonNullableType);
			this._ilg.Emit(OpCodes.Newobj, CachedReflectionInfo.Nullable_Boolean_Ctor);
			this._ilg.MarkLabel(label2);
		}

		private void EmitLiftedBinaryArithmetic(ExpressionType op, Type leftType, Type rightType, Type resultType)
		{
			bool flag = leftType.IsNullableType();
			bool flag2 = rightType.IsNullableType();
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			LocalBuilder local = this.GetLocal(leftType);
			LocalBuilder local2 = this.GetLocal(rightType);
			LocalBuilder local3 = this.GetLocal(resultType);
			this._ilg.Emit(OpCodes.Stloc, local2);
			this._ilg.Emit(OpCodes.Stloc, local);
			if (flag)
			{
				this._ilg.Emit(OpCodes.Ldloca, local);
				this._ilg.EmitHasValue(leftType);
			}
			if (flag2)
			{
				this._ilg.Emit(OpCodes.Ldloca, local2);
				this._ilg.EmitHasValue(rightType);
				if (flag)
				{
					this._ilg.Emit(OpCodes.And);
				}
			}
			this._ilg.Emit(OpCodes.Brfalse_S, label);
			if (flag)
			{
				this._ilg.Emit(OpCodes.Ldloca, local);
				this._ilg.EmitGetValueOrDefault(leftType);
			}
			else
			{
				this._ilg.Emit(OpCodes.Ldloc, local);
			}
			if (flag2)
			{
				this._ilg.Emit(OpCodes.Ldloca, local2);
				this._ilg.EmitGetValueOrDefault(rightType);
			}
			else
			{
				this._ilg.Emit(OpCodes.Ldloc, local2);
			}
			this.FreeLocal(local);
			this.FreeLocal(local2);
			this.EmitBinaryOperator(op, leftType.GetNonNullableType(), rightType.GetNonNullableType(), resultType.GetNonNullableType(), false);
			ConstructorInfo constructor = resultType.GetConstructor(new Type[] { resultType.GetNonNullableType() });
			this._ilg.Emit(OpCodes.Newobj, constructor);
			this._ilg.Emit(OpCodes.Stloc, local3);
			this._ilg.Emit(OpCodes.Br_S, label2);
			this._ilg.MarkLabel(label);
			this._ilg.Emit(OpCodes.Ldloca, local3);
			this._ilg.Emit(OpCodes.Initobj, resultType);
			this._ilg.MarkLabel(label2);
			this._ilg.Emit(OpCodes.Ldloc, local3);
			this.FreeLocal(local3);
		}

		private void EmitLiftedBooleanAnd()
		{
			Type typeFromHandle = typeof(bool?);
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			LocalBuilder local = this.GetLocal(typeFromHandle);
			LocalBuilder local2 = this.GetLocal(typeFromHandle);
			this._ilg.Emit(OpCodes.Stloc, local2);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitGetValueOrDefault(typeFromHandle);
			this._ilg.Emit(OpCodes.Brtrue_S, label);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitHasValue(typeFromHandle);
			this._ilg.Emit(OpCodes.Ldloca, local2);
			this._ilg.EmitGetValueOrDefault(typeFromHandle);
			this._ilg.Emit(OpCodes.Or);
			this._ilg.Emit(OpCodes.Brfalse_S, label);
			this._ilg.Emit(OpCodes.Ldloc, local);
			this.FreeLocal(local);
			this._ilg.Emit(OpCodes.Br_S, label2);
			this._ilg.MarkLabel(label);
			this._ilg.Emit(OpCodes.Ldloc, local2);
			this.FreeLocal(local2);
			this._ilg.MarkLabel(label2);
		}

		private void EmitLiftedBooleanOr()
		{
			Type typeFromHandle = typeof(bool?);
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			LocalBuilder local = this.GetLocal(typeFromHandle);
			LocalBuilder local2 = this.GetLocal(typeFromHandle);
			this._ilg.Emit(OpCodes.Stloc, local2);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitGetValueOrDefault(typeFromHandle);
			this._ilg.Emit(OpCodes.Brtrue_S, label);
			this._ilg.Emit(OpCodes.Ldloca, local2);
			this._ilg.EmitGetValueOrDefault(typeFromHandle);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitHasValue(typeFromHandle);
			this._ilg.Emit(OpCodes.Or);
			this._ilg.Emit(OpCodes.Brfalse_S, label);
			this._ilg.Emit(OpCodes.Ldloc, local2);
			this.FreeLocal(local2);
			this._ilg.Emit(OpCodes.Br_S, label2);
			this._ilg.MarkLabel(label);
			this._ilg.Emit(OpCodes.Ldloc, local);
			this.FreeLocal(local);
			this._ilg.MarkLabel(label2);
		}

		private LabelInfo EnsureLabel(LabelTarget node)
		{
			LabelInfo labelInfo;
			if (!this._labelInfo.TryGetValue(node, out labelInfo))
			{
				this._labelInfo.Add(node, labelInfo = new LabelInfo(this._ilg, node, false));
			}
			return labelInfo;
		}

		private LabelInfo ReferenceLabel(LabelTarget node)
		{
			LabelInfo labelInfo = this.EnsureLabel(node);
			labelInfo.Reference(this._labelBlock);
			return labelInfo;
		}

		private LabelInfo DefineLabel(LabelTarget node)
		{
			if (node == null)
			{
				return new LabelInfo(this._ilg, null, false);
			}
			LabelInfo labelInfo = this.EnsureLabel(node);
			labelInfo.Define(this._labelBlock);
			return labelInfo;
		}

		private void PushLabelBlock(LabelScopeKind type)
		{
			this._labelBlock = new LabelScopeInfo(this._labelBlock, type);
		}

		private void PopLabelBlock(LabelScopeKind kind)
		{
			this._labelBlock = this._labelBlock.Parent;
		}

		private void EmitLabelExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			LabelExpression labelExpression = (LabelExpression)expr;
			LabelInfo labelInfo = null;
			if (this._labelBlock.Kind == LabelScopeKind.Block)
			{
				this._labelBlock.TryGetLabelInfo(labelExpression.Target, out labelInfo);
				if (labelInfo == null && this._labelBlock.Parent.Kind == LabelScopeKind.Switch)
				{
					this._labelBlock.Parent.TryGetLabelInfo(labelExpression.Target, out labelInfo);
				}
			}
			if (labelInfo == null)
			{
				labelInfo = this.DefineLabel(labelExpression.Target);
			}
			if (labelExpression.DefaultValue != null)
			{
				if (labelExpression.Target.Type == typeof(void))
				{
					this.EmitExpressionAsVoid(labelExpression.DefaultValue, flags);
				}
				else
				{
					flags = LambdaCompiler.UpdateEmitExpressionStartFlag(flags, LambdaCompiler.CompilationFlags.EmitExpressionStart);
					this.EmitExpression(labelExpression.DefaultValue, flags);
				}
			}
			labelInfo.Mark();
		}

		private void EmitGotoExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			GotoExpression gotoExpression = (GotoExpression)expr;
			LabelInfo labelInfo = this.ReferenceLabel(gotoExpression.Target);
			LambdaCompiler.CompilationFlags compilationFlags = flags & LambdaCompiler.CompilationFlags.EmitAsTailCallMask;
			if (compilationFlags != LambdaCompiler.CompilationFlags.EmitAsNoTail)
			{
				compilationFlags = (labelInfo.CanReturn ? LambdaCompiler.CompilationFlags.EmitAsTail : LambdaCompiler.CompilationFlags.EmitAsNoTail);
				flags = LambdaCompiler.UpdateEmitAsTailCallFlag(flags, compilationFlags);
			}
			if (gotoExpression.Value != null)
			{
				if (gotoExpression.Target.Type == typeof(void))
				{
					this.EmitExpressionAsVoid(gotoExpression.Value, flags);
				}
				else
				{
					flags = LambdaCompiler.UpdateEmitExpressionStartFlag(flags, LambdaCompiler.CompilationFlags.EmitExpressionStart);
					this.EmitExpression(gotoExpression.Value, flags);
				}
			}
			labelInfo.EmitJump();
			this.EmitUnreachable(gotoExpression, flags);
		}

		private void EmitUnreachable(Expression node, LambdaCompiler.CompilationFlags flags)
		{
			if (node.Type != typeof(void) && (flags & LambdaCompiler.CompilationFlags.EmitAsVoidType) == (LambdaCompiler.CompilationFlags)0)
			{
				this._ilg.EmitDefault(node.Type, this);
			}
		}

		private bool TryPushLabelBlock(Expression node)
		{
			ExpressionType nodeType = node.NodeType;
			if (nodeType <= ExpressionType.Convert)
			{
				if (nodeType == ExpressionType.Conditional)
				{
					goto IL_015F;
				}
				if (nodeType == ExpressionType.Convert)
				{
					if (!(node.Type != typeof(void)))
					{
						this.PushLabelBlock(LabelScopeKind.Statement);
						return true;
					}
				}
			}
			else if (nodeType != ExpressionType.Block)
			{
				switch (nodeType)
				{
				case ExpressionType.Goto:
				case ExpressionType.Loop:
					goto IL_015F;
				case ExpressionType.Label:
					if (this._labelBlock.Kind == LabelScopeKind.Block)
					{
						LabelTarget target = ((LabelExpression)node).Target;
						if (this._labelBlock.ContainsTarget(target))
						{
							return false;
						}
						if (this._labelBlock.Parent.Kind == LabelScopeKind.Switch && this._labelBlock.Parent.ContainsTarget(target))
						{
							return false;
						}
					}
					this.PushLabelBlock(LabelScopeKind.Statement);
					return true;
				case ExpressionType.Switch:
				{
					this.PushLabelBlock(LabelScopeKind.Switch);
					SwitchExpression switchExpression = (SwitchExpression)node;
					foreach (SwitchCase switchCase in switchExpression.Cases)
					{
						this.DefineBlockLabels(switchCase.Body);
					}
					this.DefineBlockLabels(switchExpression.DefaultBody);
					return true;
				}
				}
			}
			else if (!(node is SpilledExpressionBlock))
			{
				this.PushLabelBlock(LabelScopeKind.Block);
				if (this._labelBlock.Parent.Kind != LabelScopeKind.Switch)
				{
					this.DefineBlockLabels(node);
				}
				return true;
			}
			if (this._labelBlock.Kind != LabelScopeKind.Expression)
			{
				this.PushLabelBlock(LabelScopeKind.Expression);
				return true;
			}
			return false;
			IL_015F:
			this.PushLabelBlock(LabelScopeKind.Statement);
			return true;
		}

		private void DefineBlockLabels(Expression node)
		{
			BlockExpression blockExpression = node as BlockExpression;
			if (blockExpression == null || blockExpression is SpilledExpressionBlock)
			{
				return;
			}
			int i = 0;
			int expressionCount = blockExpression.ExpressionCount;
			while (i < expressionCount)
			{
				LabelExpression labelExpression = blockExpression.GetExpression(i) as LabelExpression;
				if (labelExpression != null)
				{
					this.DefineLabel(labelExpression.Target);
				}
				i++;
			}
		}

		private void AddReturnLabel(LambdaExpression lambda)
		{
			Expression expression = lambda.Body;
			ExpressionType nodeType;
			for (;;)
			{
				nodeType = expression.NodeType;
				if (nodeType != ExpressionType.Block)
				{
					break;
				}
				BlockExpression blockExpression = (BlockExpression)expression;
				if (blockExpression.ExpressionCount == 0)
				{
					return;
				}
				for (int i = blockExpression.ExpressionCount - 1; i >= 0; i--)
				{
					expression = blockExpression.GetExpression(i);
					if (LambdaCompiler.Significant(expression))
					{
						break;
					}
				}
			}
			if (nodeType != ExpressionType.Label)
			{
				return;
			}
			LabelTarget target = ((LabelExpression)expression).Target;
			this._labelInfo.Add(target, new LabelInfo(this._ilg, target, TypeUtils.AreReferenceAssignable(lambda.ReturnType, target.Type)));
		}

		private static LambdaCompiler.CompilationFlags UpdateEmitAsTailCallFlag(LambdaCompiler.CompilationFlags flags, LambdaCompiler.CompilationFlags newValue)
		{
			LambdaCompiler.CompilationFlags compilationFlags = flags & LambdaCompiler.CompilationFlags.EmitAsTailCallMask;
			return (flags ^ compilationFlags) | newValue;
		}

		private static LambdaCompiler.CompilationFlags UpdateEmitExpressionStartFlag(LambdaCompiler.CompilationFlags flags, LambdaCompiler.CompilationFlags newValue)
		{
			LambdaCompiler.CompilationFlags compilationFlags = flags & LambdaCompiler.CompilationFlags.EmitExpressionStartMask;
			return (flags ^ compilationFlags) | newValue;
		}

		private static LambdaCompiler.CompilationFlags UpdateEmitAsTypeFlag(LambdaCompiler.CompilationFlags flags, LambdaCompiler.CompilationFlags newValue)
		{
			LambdaCompiler.CompilationFlags compilationFlags = flags & LambdaCompiler.CompilationFlags.EmitAsTypeMask;
			return (flags ^ compilationFlags) | newValue;
		}

		internal void EmitExpression(Expression node)
		{
			this.EmitExpression(node, LambdaCompiler.CompilationFlags.EmitExpressionStart | LambdaCompiler.CompilationFlags.EmitAsNoTail);
		}

		private void EmitExpressionAsVoid(Expression node)
		{
			this.EmitExpressionAsVoid(node, LambdaCompiler.CompilationFlags.EmitAsNoTail);
		}

		private void EmitExpressionAsVoid(Expression node, LambdaCompiler.CompilationFlags flags)
		{
			LambdaCompiler.CompilationFlags compilationFlags = this.EmitExpressionStart(node);
			ExpressionType nodeType = node.NodeType;
			if (nodeType <= ExpressionType.Assign)
			{
				if (nodeType == ExpressionType.Constant || nodeType == ExpressionType.Parameter)
				{
					goto IL_00D5;
				}
				if (nodeType == ExpressionType.Assign)
				{
					this.EmitAssign((AssignBinaryExpression)node, LambdaCompiler.CompilationFlags.EmitAsVoidType);
					goto IL_00D5;
				}
			}
			else if (nodeType <= ExpressionType.Default)
			{
				if (nodeType == ExpressionType.Block)
				{
					this.Emit((BlockExpression)node, LambdaCompiler.UpdateEmitAsTypeFlag(flags, LambdaCompiler.CompilationFlags.EmitAsVoidType));
					goto IL_00D5;
				}
				if (nodeType == ExpressionType.Default)
				{
					goto IL_00D5;
				}
			}
			else
			{
				if (nodeType == ExpressionType.Goto)
				{
					this.EmitGotoExpression(node, LambdaCompiler.UpdateEmitAsTypeFlag(flags, LambdaCompiler.CompilationFlags.EmitAsVoidType));
					goto IL_00D5;
				}
				if (nodeType == ExpressionType.Throw)
				{
					this.EmitThrow((UnaryExpression)node, LambdaCompiler.CompilationFlags.EmitAsVoidType);
					goto IL_00D5;
				}
			}
			if (node.Type == typeof(void))
			{
				this.EmitExpression(node, LambdaCompiler.UpdateEmitExpressionStartFlag(flags, LambdaCompiler.CompilationFlags.EmitNoExpressionStart));
			}
			else
			{
				this.EmitExpression(node, LambdaCompiler.CompilationFlags.EmitNoExpressionStart | LambdaCompiler.CompilationFlags.EmitAsNoTail);
				this._ilg.Emit(OpCodes.Pop);
			}
			IL_00D5:
			this.EmitExpressionEnd(compilationFlags);
		}

		private void EmitExpressionAsType(Expression node, Type type, LambdaCompiler.CompilationFlags flags)
		{
			if (type == typeof(void))
			{
				this.EmitExpressionAsVoid(node, flags);
				return;
			}
			if (!TypeUtils.AreEquivalent(node.Type, type))
			{
				this.EmitExpression(node);
				this._ilg.Emit(OpCodes.Castclass, type);
				return;
			}
			this.EmitExpression(node, LambdaCompiler.UpdateEmitExpressionStartFlag(flags, LambdaCompiler.CompilationFlags.EmitExpressionStart));
		}

		private LambdaCompiler.CompilationFlags EmitExpressionStart(Expression node)
		{
			if (this.TryPushLabelBlock(node))
			{
				return LambdaCompiler.CompilationFlags.EmitExpressionStart;
			}
			return LambdaCompiler.CompilationFlags.EmitNoExpressionStart;
		}

		private void EmitExpressionEnd(LambdaCompiler.CompilationFlags flags)
		{
			if ((flags & LambdaCompiler.CompilationFlags.EmitExpressionStartMask) == LambdaCompiler.CompilationFlags.EmitExpressionStart)
			{
				this.PopLabelBlock(this._labelBlock.Kind);
			}
		}

		private void EmitInvocationExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			InvocationExpression invocationExpression = (InvocationExpression)expr;
			if (invocationExpression.LambdaOperand != null)
			{
				this.EmitInlinedInvoke(invocationExpression, flags);
				return;
			}
			expr = invocationExpression.Expression;
			this.EmitMethodCall(expr, expr.Type.GetInvokeMethod(), invocationExpression, LambdaCompiler.CompilationFlags.EmitExpressionStart | LambdaCompiler.CompilationFlags.EmitAsNoTail);
		}

		private void EmitInlinedInvoke(InvocationExpression invoke, LambdaCompiler.CompilationFlags flags)
		{
			LambdaExpression lambdaOperand = invoke.LambdaOperand;
			List<LambdaCompiler.WriteBack> list = this.EmitArguments(lambdaOperand.Type.GetInvokeMethod(), invoke);
			LambdaCompiler lambdaCompiler = new LambdaCompiler(this, lambdaOperand, invoke);
			if (list != null)
			{
				flags = LambdaCompiler.UpdateEmitAsTailCallFlag(flags, LambdaCompiler.CompilationFlags.EmitAsNoTail);
			}
			lambdaCompiler.EmitLambdaBody(this._scope, true, flags);
			this.EmitWriteBack(list);
		}

		private void EmitIndexExpression(Expression expr)
		{
			IndexExpression indexExpression = (IndexExpression)expr;
			Type type = null;
			if (indexExpression.Object != null)
			{
				this.EmitInstance(indexExpression.Object, out type);
			}
			int i = 0;
			int argumentCount = indexExpression.ArgumentCount;
			while (i < argumentCount)
			{
				Expression argument = indexExpression.GetArgument(i);
				this.EmitExpression(argument);
				i++;
			}
			this.EmitGetIndexCall(indexExpression, type);
		}

		private void EmitIndexAssignment(AssignBinaryExpression node, LambdaCompiler.CompilationFlags flags)
		{
			IndexExpression indexExpression = (IndexExpression)node.Left;
			LambdaCompiler.CompilationFlags compilationFlags = flags & LambdaCompiler.CompilationFlags.EmitAsTypeMask;
			Type type = null;
			if (indexExpression.Object != null)
			{
				this.EmitInstance(indexExpression.Object, out type);
			}
			int i = 0;
			int argumentCount = indexExpression.ArgumentCount;
			while (i < argumentCount)
			{
				Expression argument = indexExpression.GetArgument(i);
				this.EmitExpression(argument);
				i++;
			}
			this.EmitExpression(node.Right);
			LocalBuilder localBuilder = null;
			if (compilationFlags != LambdaCompiler.CompilationFlags.EmitAsVoidType)
			{
				this._ilg.Emit(OpCodes.Dup);
				this._ilg.Emit(OpCodes.Stloc, localBuilder = this.GetLocal(node.Type));
			}
			this.EmitSetIndexCall(indexExpression, type);
			if (compilationFlags != LambdaCompiler.CompilationFlags.EmitAsVoidType)
			{
				this._ilg.Emit(OpCodes.Ldloc, localBuilder);
				this.FreeLocal(localBuilder);
			}
		}

		private void EmitGetIndexCall(IndexExpression node, Type objectType)
		{
			if (node.Indexer != null)
			{
				MethodInfo getMethod = node.Indexer.GetGetMethod(true);
				this.EmitCall(objectType, getMethod);
				return;
			}
			this.EmitGetArrayElement(objectType);
		}

		private void EmitGetArrayElement(Type arrayType)
		{
			if (arrayType.IsSZArray)
			{
				this._ilg.EmitLoadElement(arrayType.GetElementType());
				return;
			}
			this._ilg.Emit(OpCodes.Call, arrayType.GetMethod("Get", BindingFlags.Instance | BindingFlags.Public));
		}

		private void EmitSetIndexCall(IndexExpression node, Type objectType)
		{
			if (node.Indexer != null)
			{
				MethodInfo setMethod = node.Indexer.GetSetMethod(true);
				this.EmitCall(objectType, setMethod);
				return;
			}
			this.EmitSetArrayElement(objectType);
		}

		private void EmitSetArrayElement(Type arrayType)
		{
			if (arrayType.IsSZArray)
			{
				this._ilg.EmitStoreElement(arrayType.GetElementType());
				return;
			}
			this._ilg.Emit(OpCodes.Call, arrayType.GetMethod("Set", BindingFlags.Instance | BindingFlags.Public));
		}

		private void EmitMethodCallExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			MethodCallExpression methodCallExpression = (MethodCallExpression)expr;
			this.EmitMethodCall(methodCallExpression.Object, methodCallExpression.Method, methodCallExpression, flags);
		}

		private void EmitMethodCallExpression(Expression expr)
		{
			this.EmitMethodCallExpression(expr, LambdaCompiler.CompilationFlags.EmitAsNoTail);
		}

		private void EmitMethodCall(Expression obj, MethodInfo method, IArgumentProvider methodCallExpr)
		{
			this.EmitMethodCall(obj, method, methodCallExpr, LambdaCompiler.CompilationFlags.EmitAsNoTail);
		}

		private void EmitMethodCall(Expression obj, MethodInfo method, IArgumentProvider methodCallExpr, LambdaCompiler.CompilationFlags flags)
		{
			Type type = null;
			if (!method.IsStatic)
			{
				this.EmitInstance(obj, out type);
			}
			if (obj != null && obj.Type.IsValueType)
			{
				this.EmitMethodCall(method, methodCallExpr, type);
				return;
			}
			this.EmitMethodCall(method, methodCallExpr, type, flags);
		}

		private void EmitMethodCall(MethodInfo mi, IArgumentProvider args, Type objectType)
		{
			this.EmitMethodCall(mi, args, objectType, LambdaCompiler.CompilationFlags.EmitAsNoTail);
		}

		private void EmitMethodCall(MethodInfo mi, IArgumentProvider args, Type objectType, LambdaCompiler.CompilationFlags flags)
		{
			List<LambdaCompiler.WriteBack> list = this.EmitArguments(mi, args);
			OpCode opCode = (LambdaCompiler.UseVirtual(mi) ? OpCodes.Callvirt : OpCodes.Call);
			if (opCode == OpCodes.Callvirt && objectType.IsValueType)
			{
				this._ilg.Emit(OpCodes.Constrained, objectType);
			}
			if ((flags & LambdaCompiler.CompilationFlags.EmitAsTailCallMask) == LambdaCompiler.CompilationFlags.EmitAsTail && !LambdaCompiler.MethodHasByRefParameter(mi))
			{
				this._ilg.Emit(OpCodes.Tailcall);
			}
			if (mi.CallingConvention == CallingConventions.VarArgs)
			{
				int argumentCount = args.ArgumentCount;
				Type[] array = new Type[argumentCount];
				for (int i = 0; i < argumentCount; i++)
				{
					array[i] = args.GetArgument(i).Type;
				}
				this._ilg.EmitCall(opCode, mi, array);
			}
			else
			{
				this._ilg.Emit(opCode, mi);
			}
			this.EmitWriteBack(list);
		}

		private static bool MethodHasByRefParameter(MethodInfo mi)
		{
			ParameterInfo[] parametersCached = mi.GetParametersCached();
			for (int i = 0; i < parametersCached.Length; i++)
			{
				if (parametersCached[i].IsByRefParameter())
				{
					return true;
				}
			}
			return false;
		}

		private void EmitCall(Type objectType, MethodInfo method)
		{
			if (method.CallingConvention == CallingConventions.VarArgs)
			{
				throw Error.UnexpectedVarArgsCall(method);
			}
			OpCode opCode = (LambdaCompiler.UseVirtual(method) ? OpCodes.Callvirt : OpCodes.Call);
			if (opCode == OpCodes.Callvirt && objectType.IsValueType)
			{
				this._ilg.Emit(OpCodes.Constrained, objectType);
			}
			this._ilg.Emit(opCode, method);
		}

		private static bool UseVirtual(MethodInfo mi)
		{
			return !mi.IsStatic && !mi.DeclaringType.IsValueType;
		}

		private List<LambdaCompiler.WriteBack> EmitArguments(MethodBase method, IArgumentProvider args)
		{
			return this.EmitArguments(method, args, 0);
		}

		private List<LambdaCompiler.WriteBack> EmitArguments(MethodBase method, IArgumentProvider args, int skipParameters)
		{
			ParameterInfo[] parametersCached = method.GetParametersCached();
			List<LambdaCompiler.WriteBack> list = null;
			int i = skipParameters;
			int num = parametersCached.Length;
			while (i < num)
			{
				ParameterInfo parameterInfo = parametersCached[i];
				Expression argument = args.GetArgument(i - skipParameters);
				Type type = parameterInfo.ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
					LambdaCompiler.WriteBack writeBack = this.EmitAddressWriteBack(argument, type);
					if (writeBack != null)
					{
						if (list == null)
						{
							list = new List<LambdaCompiler.WriteBack>();
						}
						list.Add(writeBack);
					}
				}
				else
				{
					this.EmitExpression(argument);
				}
				i++;
			}
			return list;
		}

		private void EmitWriteBack(List<LambdaCompiler.WriteBack> writeBacks)
		{
			if (writeBacks != null)
			{
				foreach (LambdaCompiler.WriteBack writeBack in writeBacks)
				{
					writeBack(this);
				}
			}
		}

		private void EmitConstantExpression(Expression expr)
		{
			ConstantExpression constantExpression = (ConstantExpression)expr;
			this.EmitConstant(constantExpression.Value, constantExpression.Type);
		}

		private void EmitConstant(object value)
		{
			this.EmitConstant(value, value.GetType());
		}

		private void EmitConstant(object value, Type type)
		{
			if (!this._ilg.TryEmitConstant(value, type, this))
			{
				this._boundConstants.EmitConstant(this, value, type);
			}
		}

		private void EmitDynamicExpression(Expression expr)
		{
			if (!(this._method is DynamicMethod))
			{
				throw Error.CannotCompileDynamic();
			}
			IDynamicExpression dynamicExpression = (IDynamicExpression)expr;
			object obj = dynamicExpression.CreateCallSite();
			Type type = obj.GetType();
			MethodInfo invokeMethod = dynamicExpression.DelegateType.GetInvokeMethod();
			this.EmitConstant(obj, type);
			this._ilg.Emit(OpCodes.Dup);
			LocalBuilder local = this.GetLocal(type);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldfld, type.GetField("Target"));
			this._ilg.Emit(OpCodes.Ldloc, local);
			this.FreeLocal(local);
			List<LambdaCompiler.WriteBack> list = this.EmitArguments(invokeMethod, dynamicExpression, 1);
			this._ilg.Emit(OpCodes.Callvirt, invokeMethod);
			this.EmitWriteBack(list);
		}

		private void EmitNewExpression(Expression expr)
		{
			NewExpression newExpression = (NewExpression)expr;
			if (!(newExpression.Constructor != null))
			{
				LocalBuilder local = this.GetLocal(newExpression.Type);
				this._ilg.Emit(OpCodes.Ldloca, local);
				this._ilg.Emit(OpCodes.Initobj, newExpression.Type);
				this._ilg.Emit(OpCodes.Ldloc, local);
				this.FreeLocal(local);
				return;
			}
			if (newExpression.Constructor.DeclaringType.IsAbstract)
			{
				throw Error.NonAbstractConstructorRequired();
			}
			List<LambdaCompiler.WriteBack> list = this.EmitArguments(newExpression.Constructor, newExpression);
			this._ilg.Emit(OpCodes.Newobj, newExpression.Constructor);
			this.EmitWriteBack(list);
		}

		private void EmitTypeBinaryExpression(Expression expr)
		{
			TypeBinaryExpression typeBinaryExpression = (TypeBinaryExpression)expr;
			if (typeBinaryExpression.NodeType == ExpressionType.TypeEqual)
			{
				this.EmitExpression(typeBinaryExpression.ReduceTypeEqual());
				return;
			}
			Type type = typeBinaryExpression.Expression.Type;
			AnalyzeTypeIsResult analyzeTypeIsResult = ConstantCheck.AnalyzeTypeIs(typeBinaryExpression);
			if (analyzeTypeIsResult == AnalyzeTypeIsResult.KnownTrue || analyzeTypeIsResult == AnalyzeTypeIsResult.KnownFalse)
			{
				this.EmitExpressionAsVoid(typeBinaryExpression.Expression);
				this._ilg.EmitPrimitive(analyzeTypeIsResult == AnalyzeTypeIsResult.KnownTrue);
				return;
			}
			if (analyzeTypeIsResult != AnalyzeTypeIsResult.KnownAssignable)
			{
				this.EmitExpression(typeBinaryExpression.Expression);
				if (type.IsValueType)
				{
					this._ilg.Emit(OpCodes.Box, type);
				}
				this._ilg.Emit(OpCodes.Isinst, typeBinaryExpression.TypeOperand);
				this._ilg.Emit(OpCodes.Ldnull);
				this._ilg.Emit(OpCodes.Cgt_Un);
				return;
			}
			if (type.IsNullableType())
			{
				this.EmitAddress(typeBinaryExpression.Expression, type);
				this._ilg.EmitHasValue(type);
				return;
			}
			this.EmitExpression(typeBinaryExpression.Expression);
			this._ilg.Emit(OpCodes.Ldnull);
			this._ilg.Emit(OpCodes.Cgt_Un);
		}

		private void EmitVariableAssignment(AssignBinaryExpression node, LambdaCompiler.CompilationFlags flags)
		{
			ParameterExpression parameterExpression = (ParameterExpression)node.Left;
			int num = (int)(flags & LambdaCompiler.CompilationFlags.EmitAsTypeMask);
			if (node.IsByRef)
			{
				this.EmitAddress(node.Right, node.Right.Type);
			}
			else
			{
				this.EmitExpression(node.Right);
			}
			if (num != 32)
			{
				this._ilg.Emit(OpCodes.Dup);
			}
			if (parameterExpression.IsByRef)
			{
				LocalBuilder local = this.GetLocal(parameterExpression.Type);
				this._ilg.Emit(OpCodes.Stloc, local);
				this._scope.EmitGet(parameterExpression);
				this._ilg.Emit(OpCodes.Ldloc, local);
				this.FreeLocal(local);
				this._ilg.EmitStoreValueIndirect(parameterExpression.Type);
				return;
			}
			this._scope.EmitSet(parameterExpression);
		}

		private void EmitAssignBinaryExpression(Expression expr)
		{
			this.EmitAssign((AssignBinaryExpression)expr, LambdaCompiler.CompilationFlags.EmitAsDefaultType);
		}

		private void EmitAssign(AssignBinaryExpression node, LambdaCompiler.CompilationFlags emitAs)
		{
			ExpressionType nodeType = node.Left.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				this.EmitMemberAssignment(node, emitAs);
				return;
			}
			if (nodeType == ExpressionType.Parameter)
			{
				this.EmitVariableAssignment(node, emitAs);
				return;
			}
			if (nodeType == ExpressionType.Index)
			{
				this.EmitIndexAssignment(node, emitAs);
				return;
			}
			throw ContractUtils.Unreachable;
		}

		private void EmitParameterExpression(Expression expr)
		{
			ParameterExpression parameterExpression = (ParameterExpression)expr;
			this._scope.EmitGet(parameterExpression);
			if (parameterExpression.IsByRef)
			{
				this._ilg.EmitLoadValueIndirect(parameterExpression.Type);
			}
		}

		private void EmitLambdaExpression(Expression expr)
		{
			LambdaExpression lambdaExpression = (LambdaExpression)expr;
			this.EmitDelegateConstruction(lambdaExpression);
		}

		private void EmitRuntimeVariablesExpression(Expression expr)
		{
			RuntimeVariablesExpression runtimeVariablesExpression = (RuntimeVariablesExpression)expr;
			this._scope.EmitVariableAccess(this, runtimeVariablesExpression.Variables);
		}

		private void EmitMemberAssignment(AssignBinaryExpression node, LambdaCompiler.CompilationFlags flags)
		{
			MemberExpression memberExpression = (MemberExpression)node.Left;
			MemberInfo member = memberExpression.Member;
			Type type = null;
			if (memberExpression.Expression != null)
			{
				this.EmitInstance(memberExpression.Expression, out type);
			}
			this.EmitExpression(node.Right);
			LocalBuilder localBuilder = null;
			LambdaCompiler.CompilationFlags compilationFlags = flags & LambdaCompiler.CompilationFlags.EmitAsTypeMask;
			if (compilationFlags != LambdaCompiler.CompilationFlags.EmitAsVoidType)
			{
				this._ilg.Emit(OpCodes.Dup);
				this._ilg.Emit(OpCodes.Stloc, localBuilder = this.GetLocal(node.Type));
			}
			if (member is FieldInfo)
			{
				this._ilg.EmitFieldSet((FieldInfo)member);
			}
			else
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				this.EmitCall(type, propertyInfo.GetSetMethod(true));
			}
			if (compilationFlags != LambdaCompiler.CompilationFlags.EmitAsVoidType)
			{
				this._ilg.Emit(OpCodes.Ldloc, localBuilder);
				this.FreeLocal(localBuilder);
			}
		}

		private void EmitMemberExpression(Expression expr)
		{
			MemberExpression memberExpression = (MemberExpression)expr;
			Type type = null;
			if (memberExpression.Expression != null)
			{
				this.EmitInstance(memberExpression.Expression, out type);
			}
			this.EmitMemberGet(memberExpression.Member, type);
		}

		private void EmitMemberGet(MemberInfo member, Type objectType)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if (fieldInfo == null)
			{
				PropertyInfo propertyInfo = (PropertyInfo)member;
				this.EmitCall(objectType, propertyInfo.GetGetMethod(true));
				return;
			}
			if (fieldInfo.IsLiteral)
			{
				this.EmitConstant(fieldInfo.GetRawConstantValue(), fieldInfo.FieldType);
				return;
			}
			this._ilg.EmitFieldGet(fieldInfo);
		}

		private void EmitInstance(Expression instance, out Type type)
		{
			type = instance.Type;
			if (type.IsByRef)
			{
				type = type.GetElementType();
				this.EmitExpression(instance);
				return;
			}
			if (type.IsValueType)
			{
				this.EmitAddress(instance, type);
				return;
			}
			this.EmitExpression(instance);
		}

		private void EmitNewArrayExpression(Expression expr)
		{
			NewArrayExpression newArrayExpression = (NewArrayExpression)expr;
			ReadOnlyCollection<Expression> expressions = newArrayExpression.Expressions;
			int count = expressions.Count;
			if (newArrayExpression.NodeType == ExpressionType.NewArrayInit)
			{
				Type elementType = newArrayExpression.Type.GetElementType();
				this._ilg.EmitArray(elementType, count);
				for (int i = 0; i < count; i++)
				{
					this._ilg.Emit(OpCodes.Dup);
					this._ilg.EmitPrimitive(i);
					this.EmitExpression(expressions[i]);
					this._ilg.EmitStoreElement(elementType);
				}
				return;
			}
			for (int j = 0; j < count; j++)
			{
				Expression expression = expressions[j];
				this.EmitExpression(expression);
				this._ilg.EmitConvertToType(expression.Type, typeof(int), true, this);
			}
			this._ilg.EmitArray(newArrayExpression.Type);
		}

		private void EmitDebugInfoExpression(Expression expr)
		{
		}

		private void EmitListInitExpression(Expression expr)
		{
			this.EmitListInit((ListInitExpression)expr);
		}

		private void EmitMemberInitExpression(Expression expr)
		{
			this.EmitMemberInit((MemberInitExpression)expr);
		}

		private void EmitBinding(MemberBinding binding, Type objectType)
		{
			switch (binding.BindingType)
			{
			case MemberBindingType.Assignment:
				this.EmitMemberAssignment((MemberAssignment)binding, objectType);
				return;
			case MemberBindingType.MemberBinding:
				this.EmitMemberMemberBinding((MemberMemberBinding)binding);
				return;
			case MemberBindingType.ListBinding:
				this.EmitMemberListBinding((MemberListBinding)binding);
				return;
			default:
				return;
			}
		}

		private void EmitMemberAssignment(MemberAssignment binding, Type objectType)
		{
			this.EmitExpression(binding.Expression);
			FieldInfo fieldInfo;
			if ((fieldInfo = binding.Member as FieldInfo) != null)
			{
				this._ilg.Emit(OpCodes.Stfld, fieldInfo);
				return;
			}
			this.EmitCall(objectType, (binding.Member as PropertyInfo).GetSetMethod(true));
		}

		private void EmitMemberMemberBinding(MemberMemberBinding binding)
		{
			Type memberType = LambdaCompiler.GetMemberType(binding.Member);
			if (binding.Member is PropertyInfo && memberType.IsValueType)
			{
				throw Error.CannotAutoInitializeValueTypeMemberThroughProperty(binding.Member);
			}
			if (memberType.IsValueType)
			{
				this.EmitMemberAddress(binding.Member, binding.Member.DeclaringType);
			}
			else
			{
				this.EmitMemberGet(binding.Member, binding.Member.DeclaringType);
			}
			this.EmitMemberInit(binding.Bindings, false, memberType);
		}

		private void EmitMemberListBinding(MemberListBinding binding)
		{
			Type memberType = LambdaCompiler.GetMemberType(binding.Member);
			if (binding.Member is PropertyInfo && memberType.IsValueType)
			{
				throw Error.CannotAutoInitializeValueTypeElementThroughProperty(binding.Member);
			}
			if (memberType.IsValueType)
			{
				this.EmitMemberAddress(binding.Member, binding.Member.DeclaringType);
			}
			else
			{
				this.EmitMemberGet(binding.Member, binding.Member.DeclaringType);
			}
			this.EmitListInit(binding.Initializers, false, memberType);
		}

		private void EmitMemberInit(MemberInitExpression init)
		{
			this.EmitExpression(init.NewExpression);
			LocalBuilder localBuilder = null;
			if (init.NewExpression.Type.IsValueType && init.Bindings.Count > 0)
			{
				localBuilder = this.GetLocal(init.NewExpression.Type);
				this._ilg.Emit(OpCodes.Stloc, localBuilder);
				this._ilg.Emit(OpCodes.Ldloca, localBuilder);
			}
			this.EmitMemberInit(init.Bindings, localBuilder == null, init.NewExpression.Type);
			if (localBuilder != null)
			{
				this._ilg.Emit(OpCodes.Ldloc, localBuilder);
				this.FreeLocal(localBuilder);
			}
		}

		private void EmitMemberInit(ReadOnlyCollection<MemberBinding> bindings, bool keepOnStack, Type objectType)
		{
			int count = bindings.Count;
			if (count == 0)
			{
				if (!keepOnStack)
				{
					this._ilg.Emit(OpCodes.Pop);
					return;
				}
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					if (keepOnStack || i < count - 1)
					{
						this._ilg.Emit(OpCodes.Dup);
					}
					this.EmitBinding(bindings[i], objectType);
				}
			}
		}

		private void EmitListInit(ListInitExpression init)
		{
			this.EmitExpression(init.NewExpression);
			LocalBuilder localBuilder = null;
			if (init.NewExpression.Type.IsValueType)
			{
				localBuilder = this.GetLocal(init.NewExpression.Type);
				this._ilg.Emit(OpCodes.Stloc, localBuilder);
				this._ilg.Emit(OpCodes.Ldloca, localBuilder);
			}
			this.EmitListInit(init.Initializers, localBuilder == null, init.NewExpression.Type);
			if (localBuilder != null)
			{
				this._ilg.Emit(OpCodes.Ldloc, localBuilder);
				this.FreeLocal(localBuilder);
			}
		}

		private void EmitListInit(ReadOnlyCollection<ElementInit> initializers, bool keepOnStack, Type objectType)
		{
			int count = initializers.Count;
			if (count == 0)
			{
				if (!keepOnStack)
				{
					this._ilg.Emit(OpCodes.Pop);
					return;
				}
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					if (keepOnStack || i < count - 1)
					{
						this._ilg.Emit(OpCodes.Dup);
					}
					this.EmitMethodCall(initializers[i].AddMethod, initializers[i], objectType);
					if (initializers[i].AddMethod.ReturnType != typeof(void))
					{
						this._ilg.Emit(OpCodes.Pop);
					}
				}
			}
		}

		private static Type GetMemberType(MemberInfo member)
		{
			FieldInfo fieldInfo;
			if ((fieldInfo = member as FieldInfo) == null)
			{
				return (member as PropertyInfo).PropertyType;
			}
			return fieldInfo.FieldType;
		}

		private void EmitLift(ExpressionType nodeType, Type resultType, MethodCallExpression mc, ParameterExpression[] paramList, Expression[] argList)
		{
			switch (nodeType)
			{
			case ExpressionType.Equal:
				goto IL_02B5;
			case ExpressionType.ExclusiveOr:
			case ExpressionType.GreaterThan:
			case ExpressionType.GreaterThanOrEqual:
			case ExpressionType.Invoke:
			case ExpressionType.Lambda:
			case ExpressionType.LeftShift:
			case ExpressionType.LessThan:
			case ExpressionType.LessThanOrEqual:
				break;
			default:
				if (nodeType == ExpressionType.NotEqual)
				{
					goto IL_02B5;
				}
				break;
			}
			IL_0035:
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			LocalBuilder local = this.GetLocal(typeof(bool));
			int i = 0;
			int num = paramList.Length;
			while (i < num)
			{
				ParameterExpression parameterExpression = paramList[i];
				Expression expression = argList[i];
				if (expression.Type.IsNullableType())
				{
					this._scope.AddLocal(this, parameterExpression);
					this.EmitAddress(expression, expression.Type);
					this._ilg.Emit(OpCodes.Dup);
					this._ilg.EmitHasValue(expression.Type);
					this._ilg.Emit(OpCodes.Ldc_I4_0);
					this._ilg.Emit(OpCodes.Ceq);
					this._ilg.Emit(OpCodes.Stloc, local);
					this._ilg.EmitGetValueOrDefault(expression.Type);
					this._scope.EmitSet(parameterExpression);
				}
				else
				{
					this._scope.AddLocal(this, parameterExpression);
					this.EmitExpression(expression);
					if (!expression.Type.IsValueType)
					{
						this._ilg.Emit(OpCodes.Dup);
						this._ilg.Emit(OpCodes.Ldnull);
						this._ilg.Emit(OpCodes.Ceq);
						this._ilg.Emit(OpCodes.Stloc, local);
					}
					this._scope.EmitSet(parameterExpression);
				}
				this._ilg.Emit(OpCodes.Ldloc, local);
				this._ilg.Emit(OpCodes.Brtrue, label2);
				i++;
			}
			this.EmitMethodCallExpression(mc);
			if (resultType.IsNullableType() && !TypeUtils.AreEquivalent(resultType, mc.Type))
			{
				ConstructorInfo constructor = resultType.GetConstructor(new Type[] { mc.Type });
				this._ilg.Emit(OpCodes.Newobj, constructor);
			}
			this._ilg.Emit(OpCodes.Br_S, label);
			this._ilg.MarkLabel(label2);
			if (TypeUtils.AreEquivalent(resultType, mc.Type.GetNullableType()))
			{
				if (resultType.IsValueType)
				{
					LocalBuilder local2 = this.GetLocal(resultType);
					this._ilg.Emit(OpCodes.Ldloca, local2);
					this._ilg.Emit(OpCodes.Initobj, resultType);
					this._ilg.Emit(OpCodes.Ldloc, local2);
					this.FreeLocal(local2);
				}
				else
				{
					this._ilg.Emit(OpCodes.Ldnull);
				}
			}
			else
			{
				this._ilg.Emit(OpCodes.Ldc_I4_0);
			}
			this._ilg.MarkLabel(label);
			this.FreeLocal(local);
			return;
			IL_02B5:
			if (!TypeUtils.AreEquivalent(resultType, mc.Type.GetNullableType()))
			{
				Label label3 = this._ilg.DefineLabel();
				Label label4 = this._ilg.DefineLabel();
				Label label5 = this._ilg.DefineLabel();
				LocalBuilder local3 = this.GetLocal(typeof(bool));
				LocalBuilder local4 = this.GetLocal(typeof(bool));
				this._ilg.Emit(OpCodes.Ldc_I4_0);
				this._ilg.Emit(OpCodes.Stloc, local3);
				this._ilg.Emit(OpCodes.Ldc_I4_1);
				this._ilg.Emit(OpCodes.Stloc, local4);
				int j = 0;
				int num2 = paramList.Length;
				while (j < num2)
				{
					ParameterExpression parameterExpression2 = paramList[j];
					Expression expression2 = argList[j];
					this._scope.AddLocal(this, parameterExpression2);
					if (expression2.Type.IsNullableType())
					{
						this.EmitAddress(expression2, expression2.Type);
						this._ilg.Emit(OpCodes.Dup);
						this._ilg.EmitHasValue(expression2.Type);
						this._ilg.Emit(OpCodes.Ldc_I4_0);
						this._ilg.Emit(OpCodes.Ceq);
						this._ilg.Emit(OpCodes.Dup);
						this._ilg.Emit(OpCodes.Ldloc, local3);
						this._ilg.Emit(OpCodes.Or);
						this._ilg.Emit(OpCodes.Stloc, local3);
						this._ilg.Emit(OpCodes.Ldloc, local4);
						this._ilg.Emit(OpCodes.And);
						this._ilg.Emit(OpCodes.Stloc, local4);
						this._ilg.EmitGetValueOrDefault(expression2.Type);
					}
					else
					{
						this.EmitExpression(expression2);
						if (!expression2.Type.IsValueType)
						{
							this._ilg.Emit(OpCodes.Dup);
							this._ilg.Emit(OpCodes.Ldnull);
							this._ilg.Emit(OpCodes.Ceq);
							this._ilg.Emit(OpCodes.Dup);
							this._ilg.Emit(OpCodes.Ldloc, local3);
							this._ilg.Emit(OpCodes.Or);
							this._ilg.Emit(OpCodes.Stloc, local3);
							this._ilg.Emit(OpCodes.Ldloc, local4);
							this._ilg.Emit(OpCodes.And);
							this._ilg.Emit(OpCodes.Stloc, local4);
						}
						else
						{
							this._ilg.Emit(OpCodes.Ldc_I4_0);
							this._ilg.Emit(OpCodes.Stloc, local4);
						}
					}
					this._scope.EmitSet(parameterExpression2);
					j++;
				}
				this._ilg.Emit(OpCodes.Ldloc, local4);
				this._ilg.Emit(OpCodes.Brtrue, label4);
				this._ilg.Emit(OpCodes.Ldloc, local3);
				this._ilg.Emit(OpCodes.Brtrue, label5);
				this.EmitMethodCallExpression(mc);
				if (resultType.IsNullableType() && !TypeUtils.AreEquivalent(resultType, mc.Type))
				{
					ConstructorInfo constructor2 = resultType.GetConstructor(new Type[] { mc.Type });
					this._ilg.Emit(OpCodes.Newobj, constructor2);
				}
				this._ilg.Emit(OpCodes.Br_S, label3);
				this._ilg.MarkLabel(label4);
				this._ilg.EmitPrimitive(nodeType == ExpressionType.Equal);
				this._ilg.Emit(OpCodes.Br_S, label3);
				this._ilg.MarkLabel(label5);
				this._ilg.EmitPrimitive(nodeType == ExpressionType.NotEqual);
				this._ilg.MarkLabel(label3);
				this.FreeLocal(local3);
				this.FreeLocal(local4);
				return;
			}
			goto IL_0035;
		}

		private void EmitExpression(Expression node, LambdaCompiler.CompilationFlags flags)
		{
			if (!this._guard.TryEnterOnCurrentStack())
			{
				this._guard.RunOnEmptyStack<LambdaCompiler, Expression, LambdaCompiler.CompilationFlags>(delegate(LambdaCompiler @this, Expression n, LambdaCompiler.CompilationFlags f)
				{
					@this.EmitExpression(n, f);
				}, this, node, flags);
				return;
			}
			bool flag = (flags & LambdaCompiler.CompilationFlags.EmitExpressionStartMask) == LambdaCompiler.CompilationFlags.EmitExpressionStart;
			LambdaCompiler.CompilationFlags compilationFlags = (flag ? this.EmitExpressionStart(node) : LambdaCompiler.CompilationFlags.EmitNoExpressionStart);
			flags &= LambdaCompiler.CompilationFlags.EmitAsTailCallMask;
			switch (node.NodeType)
			{
			case ExpressionType.Add:
			case ExpressionType.AddChecked:
			case ExpressionType.And:
			case ExpressionType.ArrayIndex:
			case ExpressionType.Divide:
			case ExpressionType.Equal:
			case ExpressionType.ExclusiveOr:
			case ExpressionType.GreaterThan:
			case ExpressionType.GreaterThanOrEqual:
			case ExpressionType.LeftShift:
			case ExpressionType.LessThan:
			case ExpressionType.LessThanOrEqual:
			case ExpressionType.Modulo:
			case ExpressionType.Multiply:
			case ExpressionType.MultiplyChecked:
			case ExpressionType.NotEqual:
			case ExpressionType.Or:
			case ExpressionType.Power:
			case ExpressionType.RightShift:
			case ExpressionType.Subtract:
			case ExpressionType.SubtractChecked:
				this.EmitBinaryExpression(node, flags);
				break;
			case ExpressionType.AndAlso:
				this.EmitAndAlsoBinaryExpression(node, flags);
				break;
			case ExpressionType.ArrayLength:
			case ExpressionType.Negate:
			case ExpressionType.UnaryPlus:
			case ExpressionType.NegateChecked:
			case ExpressionType.Not:
			case ExpressionType.TypeAs:
			case ExpressionType.Decrement:
			case ExpressionType.Increment:
			case ExpressionType.OnesComplement:
			case ExpressionType.IsTrue:
			case ExpressionType.IsFalse:
				this.EmitUnaryExpression(node, flags);
				break;
			case ExpressionType.Call:
				this.EmitMethodCallExpression(node, flags);
				break;
			case ExpressionType.Coalesce:
				this.EmitCoalesceBinaryExpression(node);
				break;
			case ExpressionType.Conditional:
				this.EmitConditionalExpression(node, flags);
				break;
			case ExpressionType.Constant:
				this.EmitConstantExpression(node);
				break;
			case ExpressionType.Convert:
			case ExpressionType.ConvertChecked:
				this.EmitConvertUnaryExpression(node, flags);
				break;
			case ExpressionType.Invoke:
				this.EmitInvocationExpression(node, flags);
				break;
			case ExpressionType.Lambda:
				this.EmitLambdaExpression(node);
				break;
			case ExpressionType.ListInit:
				this.EmitListInitExpression(node);
				break;
			case ExpressionType.MemberAccess:
				this.EmitMemberExpression(node);
				break;
			case ExpressionType.MemberInit:
				this.EmitMemberInitExpression(node);
				break;
			case ExpressionType.New:
				this.EmitNewExpression(node);
				break;
			case ExpressionType.NewArrayInit:
			case ExpressionType.NewArrayBounds:
				this.EmitNewArrayExpression(node);
				break;
			case ExpressionType.OrElse:
				this.EmitOrElseBinaryExpression(node, flags);
				break;
			case ExpressionType.Parameter:
				this.EmitParameterExpression(node);
				break;
			case ExpressionType.Quote:
				this.EmitQuoteUnaryExpression(node);
				break;
			case ExpressionType.TypeIs:
			case ExpressionType.TypeEqual:
				this.EmitTypeBinaryExpression(node);
				break;
			case ExpressionType.Assign:
				this.EmitAssignBinaryExpression(node);
				break;
			case ExpressionType.Block:
				this.EmitBlockExpression(node, flags);
				break;
			case ExpressionType.DebugInfo:
				this.EmitDebugInfoExpression(node);
				break;
			case ExpressionType.Dynamic:
				this.EmitDynamicExpression(node);
				break;
			case ExpressionType.Default:
				this.EmitDefaultExpression(node);
				break;
			case ExpressionType.Goto:
				this.EmitGotoExpression(node, flags);
				break;
			case ExpressionType.Index:
				this.EmitIndexExpression(node);
				break;
			case ExpressionType.Label:
				this.EmitLabelExpression(node, flags);
				break;
			case ExpressionType.RuntimeVariables:
				this.EmitRuntimeVariablesExpression(node);
				break;
			case ExpressionType.Loop:
				this.EmitLoopExpression(node);
				break;
			case ExpressionType.Switch:
				this.EmitSwitchExpression(node, flags);
				break;
			case ExpressionType.Throw:
				this.EmitThrowUnaryExpression(node);
				break;
			case ExpressionType.Try:
				this.EmitTryExpression(node);
				break;
			case ExpressionType.Unbox:
				this.EmitUnboxUnaryExpression(node);
				break;
			}
			if (flag)
			{
				this.EmitExpressionEnd(compilationFlags);
			}
		}

		private static bool IsChecked(ExpressionType op)
		{
			if (op <= ExpressionType.MultiplyChecked)
			{
				if (op != ExpressionType.AddChecked && op != ExpressionType.ConvertChecked && op != ExpressionType.MultiplyChecked)
				{
					return false;
				}
			}
			else if (op != ExpressionType.NegateChecked && op != ExpressionType.SubtractChecked && op - ExpressionType.AddAssignChecked > 2)
			{
				return false;
			}
			return true;
		}

		internal void EmitConstantArray<T>(T[] array)
		{
			if (this._method is DynamicMethod)
			{
				this.EmitConstant(array, typeof(T[]));
				return;
			}
			if (this._typeBuilder != null)
			{
				FieldBuilder fieldBuilder = this.CreateStaticField("ConstantArray", typeof(T[]));
				Label label = this._ilg.DefineLabel();
				this._ilg.Emit(OpCodes.Ldsfld, fieldBuilder);
				this._ilg.Emit(OpCodes.Ldnull);
				this._ilg.Emit(OpCodes.Bne_Un, label);
				this._ilg.EmitArray<T>(array, this);
				this._ilg.Emit(OpCodes.Stsfld, fieldBuilder);
				this._ilg.MarkLabel(label);
				this._ilg.Emit(OpCodes.Ldsfld, fieldBuilder);
				return;
			}
			this._ilg.EmitArray<T>(array, this);
		}

		private void EmitClosureCreation(LambdaCompiler inner)
		{
			bool needsClosure = inner._scope.NeedsClosure;
			bool flag = inner._boundConstants.Count > 0;
			if (!needsClosure && !flag)
			{
				this._ilg.EmitNull();
				return;
			}
			if (flag)
			{
				this._boundConstants.EmitConstant(this, inner._boundConstants.ToArray(), typeof(object[]));
			}
			else
			{
				this._ilg.EmitNull();
			}
			if (needsClosure)
			{
				this._scope.EmitGet(this._scope.NearestHoistedLocals.SelfVariable);
			}
			else
			{
				this._ilg.EmitNull();
			}
			this._ilg.EmitNew(CachedReflectionInfo.Closure_ObjectArray_ObjectArray);
		}

		private void EmitDelegateConstruction(LambdaCompiler inner)
		{
			Type type = inner._lambda.Type;
			DynamicMethod dynamicMethod = inner._method as DynamicMethod;
			if (dynamicMethod != null)
			{
				this._boundConstants.EmitConstant(this, dynamicMethod, typeof(MethodInfo));
				this._ilg.EmitType(type);
				this.EmitClosureCreation(inner);
				this._ilg.Emit(OpCodes.Callvirt, CachedReflectionInfo.MethodInfo_CreateDelegate_Type_Object);
				this._ilg.Emit(OpCodes.Castclass, type);
				return;
			}
			this.EmitClosureCreation(inner);
			this._ilg.Emit(OpCodes.Ldftn, inner._method);
			this._ilg.Emit(OpCodes.Newobj, (ConstructorInfo)type.GetMember(".ctor")[0]);
		}

		private void EmitDelegateConstruction(LambdaExpression lambda)
		{
			LambdaCompiler lambdaCompiler;
			if (this._method is DynamicMethod)
			{
				lambdaCompiler = new LambdaCompiler(this._tree, lambda);
			}
			else
			{
				string text = (string.IsNullOrEmpty(lambda.Name) ? LambdaCompiler.GetUniqueMethodName() : lambda.Name);
				MethodBuilder methodBuilder = this._typeBuilder.DefineMethod(text, MethodAttributes.Private | MethodAttributes.Static);
				lambdaCompiler = new LambdaCompiler(this._tree, lambda, methodBuilder);
			}
			lambdaCompiler.EmitLambdaBody(this._scope, false, LambdaCompiler.CompilationFlags.EmitAsNoTail);
			this.EmitDelegateConstruction(lambdaCompiler);
		}

		private static Type[] GetParameterTypes(LambdaExpression lambda, Type firstType)
		{
			int parameterCount = lambda.ParameterCount;
			Type[] array;
			int num;
			if (firstType != null)
			{
				array = new Type[parameterCount + 1];
				array[0] = firstType;
				num = 1;
			}
			else
			{
				array = new Type[parameterCount];
				num = 0;
			}
			int i = 0;
			while (i < parameterCount)
			{
				ParameterExpression parameter = lambda.GetParameter(i);
				array[num] = (parameter.IsByRef ? parameter.Type.MakeByRefType() : parameter.Type);
				i++;
				num++;
			}
			return array;
		}

		private static string GetUniqueMethodName()
		{
			return "<ExpressionCompilerImplementationDetails>{" + Interlocked.Increment(ref LambdaCompiler.s_counter) + "}lambda_method";
		}

		private void EmitLambdaBody()
		{
			LambdaCompiler.CompilationFlags compilationFlags = (this._lambda.TailCall ? LambdaCompiler.CompilationFlags.EmitAsTail : LambdaCompiler.CompilationFlags.EmitAsNoTail);
			this.EmitLambdaBody(null, false, compilationFlags);
		}

		private void EmitLambdaBody(CompilerScope parent, bool inlined, LambdaCompiler.CompilationFlags flags)
		{
			this._scope.Enter(this, parent);
			if (inlined)
			{
				for (int i = this._lambda.ParameterCount - 1; i >= 0; i--)
				{
					this._scope.EmitSet(this._lambda.GetParameter(i));
				}
			}
			flags = LambdaCompiler.UpdateEmitExpressionStartFlag(flags, LambdaCompiler.CompilationFlags.EmitExpressionStart);
			if (this._lambda.ReturnType == typeof(void))
			{
				this.EmitExpressionAsVoid(this._lambda.Body, flags);
			}
			else
			{
				this.EmitExpression(this._lambda.Body, flags);
			}
			if (!inlined)
			{
				this._ilg.Emit(OpCodes.Ret);
			}
			this._scope.Exit();
			foreach (LabelInfo labelInfo in this._labelInfo.Values)
			{
				labelInfo.ValidateFinish();
			}
		}

		private void EmitConditionalExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			ConditionalExpression conditionalExpression = (ConditionalExpression)expr;
			Label label = this._ilg.DefineLabel();
			this.EmitExpressionAndBranch(false, conditionalExpression.Test, label);
			this.EmitExpressionAsType(conditionalExpression.IfTrue, conditionalExpression.Type, flags);
			if (LambdaCompiler.NotEmpty(conditionalExpression.IfFalse))
			{
				Label label2 = this._ilg.DefineLabel();
				if ((flags & LambdaCompiler.CompilationFlags.EmitAsTailCallMask) == LambdaCompiler.CompilationFlags.EmitAsTail)
				{
					this._ilg.Emit(OpCodes.Ret);
				}
				else
				{
					this._ilg.Emit(OpCodes.Br, label2);
				}
				this._ilg.MarkLabel(label);
				this.EmitExpressionAsType(conditionalExpression.IfFalse, conditionalExpression.Type, flags);
				this._ilg.MarkLabel(label2);
				return;
			}
			this._ilg.MarkLabel(label);
		}

		private static bool NotEmpty(Expression node)
		{
			DefaultExpression defaultExpression = node as DefaultExpression;
			return defaultExpression == null || defaultExpression.Type != typeof(void);
		}

		private static bool Significant(Expression node)
		{
			BlockExpression blockExpression = node as BlockExpression;
			if (blockExpression != null)
			{
				for (int i = 0; i < blockExpression.ExpressionCount; i++)
				{
					if (LambdaCompiler.Significant(blockExpression.GetExpression(i)))
					{
						return true;
					}
				}
				return false;
			}
			return LambdaCompiler.NotEmpty(node) && !(node is DebugInfoExpression);
		}

		private void EmitCoalesceBinaryExpression(Expression expr)
		{
			BinaryExpression binaryExpression = (BinaryExpression)expr;
			if (binaryExpression.Left.Type.IsNullableType())
			{
				this.EmitNullableCoalesce(binaryExpression);
				return;
			}
			if (binaryExpression.Conversion != null)
			{
				this.EmitLambdaReferenceCoalesce(binaryExpression);
				return;
			}
			this.EmitReferenceCoalesceWithoutConversion(binaryExpression);
		}

		private void EmitNullableCoalesce(BinaryExpression b)
		{
			LocalBuilder local = this.GetLocal(b.Left.Type);
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			this.EmitExpression(b.Left);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitHasValue(b.Left.Type);
			this._ilg.Emit(OpCodes.Brfalse, label);
			Type nonNullableType = b.Left.Type.GetNonNullableType();
			if (b.Conversion != null)
			{
				Expression parameter = b.Conversion.GetParameter(0);
				this.EmitLambdaExpression(b.Conversion);
				if (!parameter.Type.IsAssignableFrom(b.Left.Type))
				{
					this._ilg.Emit(OpCodes.Ldloca, local);
					this._ilg.EmitGetValueOrDefault(b.Left.Type);
				}
				else
				{
					this._ilg.Emit(OpCodes.Ldloc, local);
				}
				this._ilg.Emit(OpCodes.Callvirt, b.Conversion.Type.GetInvokeMethod());
			}
			else if (TypeUtils.AreEquivalent(b.Type, b.Left.Type))
			{
				this._ilg.Emit(OpCodes.Ldloc, local);
			}
			else
			{
				this._ilg.Emit(OpCodes.Ldloca, local);
				this._ilg.EmitGetValueOrDefault(b.Left.Type);
				if (!TypeUtils.AreEquivalent(b.Type, nonNullableType))
				{
					this._ilg.EmitConvertToType(nonNullableType, b.Type, true, this);
				}
			}
			this.FreeLocal(local);
			this._ilg.Emit(OpCodes.Br, label2);
			this._ilg.MarkLabel(label);
			this.EmitExpression(b.Right);
			if (!TypeUtils.AreEquivalent(b.Right.Type, b.Type))
			{
				this._ilg.EmitConvertToType(b.Right.Type, b.Type, true, this);
			}
			this._ilg.MarkLabel(label2);
		}

		private void EmitLambdaReferenceCoalesce(BinaryExpression b)
		{
			LocalBuilder local = this.GetLocal(b.Left.Type);
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			this.EmitExpression(b.Left);
			this._ilg.Emit(OpCodes.Dup);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Brtrue, label2);
			this.EmitExpression(b.Right);
			this._ilg.Emit(OpCodes.Br, label);
			this._ilg.MarkLabel(label2);
			this.EmitLambdaExpression(b.Conversion);
			this._ilg.Emit(OpCodes.Ldloc, local);
			this.FreeLocal(local);
			this._ilg.Emit(OpCodes.Callvirt, b.Conversion.Type.GetInvokeMethod());
			this._ilg.MarkLabel(label);
		}

		private void EmitReferenceCoalesceWithoutConversion(BinaryExpression b)
		{
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			this.EmitExpression(b.Left);
			this._ilg.Emit(OpCodes.Dup);
			this._ilg.Emit(OpCodes.Brtrue, label2);
			this._ilg.Emit(OpCodes.Pop);
			this.EmitExpression(b.Right);
			if (!TypeUtils.AreEquivalent(b.Right.Type, b.Type))
			{
				if (b.Right.Type.IsValueType)
				{
					this._ilg.Emit(OpCodes.Box, b.Right.Type);
				}
				this._ilg.Emit(OpCodes.Castclass, b.Type);
			}
			this._ilg.Emit(OpCodes.Br_S, label);
			this._ilg.MarkLabel(label2);
			if (!TypeUtils.AreEquivalent(b.Left.Type, b.Type))
			{
				this._ilg.Emit(OpCodes.Castclass, b.Type);
			}
			this._ilg.MarkLabel(label);
		}

		private void EmitLiftedAndAlso(BinaryExpression b)
		{
			Type typeFromHandle = typeof(bool?);
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			Label label3 = this._ilg.DefineLabel();
			this.EmitExpression(b.Left);
			LocalBuilder local = this.GetLocal(typeFromHandle);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitHasValue(typeFromHandle);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitGetValueOrDefault(typeFromHandle);
			this._ilg.Emit(OpCodes.Not);
			this._ilg.Emit(OpCodes.And);
			this._ilg.Emit(OpCodes.Brtrue, label);
			this.EmitExpression(b.Right);
			LocalBuilder local2 = this.GetLocal(typeFromHandle);
			this._ilg.Emit(OpCodes.Stloc, local2);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitGetValueOrDefault(typeFromHandle);
			this._ilg.Emit(OpCodes.Brtrue_S, label2);
			this._ilg.Emit(OpCodes.Ldloca, local2);
			this._ilg.EmitGetValueOrDefault(typeFromHandle);
			this._ilg.Emit(OpCodes.Brtrue_S, label);
			this._ilg.MarkLabel(label2);
			this._ilg.Emit(OpCodes.Ldloc, local2);
			this.FreeLocal(local2);
			this._ilg.Emit(OpCodes.Br_S, label3);
			this._ilg.MarkLabel(label);
			this._ilg.Emit(OpCodes.Ldloc, local);
			this.FreeLocal(local);
			this._ilg.MarkLabel(label3);
		}

		private void EmitMethodAndAlso(BinaryExpression b, LambdaCompiler.CompilationFlags flags)
		{
			Label label = this._ilg.DefineLabel();
			this.EmitExpression(b.Left);
			this._ilg.Emit(OpCodes.Dup);
			MethodInfo booleanOperator = TypeUtils.GetBooleanOperator(b.Method.DeclaringType, "op_False");
			this._ilg.Emit(OpCodes.Call, booleanOperator);
			this._ilg.Emit(OpCodes.Brtrue, label);
			this.EmitExpression(b.Right);
			if ((flags & LambdaCompiler.CompilationFlags.EmitAsTailCallMask) == LambdaCompiler.CompilationFlags.EmitAsTail)
			{
				this._ilg.Emit(OpCodes.Tailcall);
			}
			this._ilg.Emit(OpCodes.Call, b.Method);
			this._ilg.MarkLabel(label);
		}

		private void EmitUnliftedAndAlso(BinaryExpression b)
		{
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			this.EmitExpressionAndBranch(false, b.Left, label);
			this.EmitExpression(b.Right);
			this._ilg.Emit(OpCodes.Br, label2);
			this._ilg.MarkLabel(label);
			this._ilg.Emit(OpCodes.Ldc_I4_0);
			this._ilg.MarkLabel(label2);
		}

		private void EmitAndAlsoBinaryExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			BinaryExpression binaryExpression = (BinaryExpression)expr;
			if (binaryExpression.Method != null)
			{
				if (binaryExpression.IsLiftedLogical)
				{
					this.EmitExpression(binaryExpression.ReduceUserdefinedLifted());
					return;
				}
				this.EmitMethodAndAlso(binaryExpression, flags);
				return;
			}
			else
			{
				if (binaryExpression.Left.Type == typeof(bool?))
				{
					this.EmitLiftedAndAlso(binaryExpression);
					return;
				}
				this.EmitUnliftedAndAlso(binaryExpression);
				return;
			}
		}

		private void EmitLiftedOrElse(BinaryExpression b)
		{
			Type typeFromHandle = typeof(bool?);
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			LocalBuilder local = this.GetLocal(typeFromHandle);
			this.EmitExpression(b.Left);
			this._ilg.Emit(OpCodes.Stloc, local);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitGetValueOrDefault(typeFromHandle);
			this._ilg.Emit(OpCodes.Brtrue, label);
			this.EmitExpression(b.Right);
			LocalBuilder local2 = this.GetLocal(typeFromHandle);
			this._ilg.Emit(OpCodes.Stloc, local2);
			this._ilg.Emit(OpCodes.Ldloca, local2);
			this._ilg.EmitGetValueOrDefault(typeFromHandle);
			this._ilg.Emit(OpCodes.Ldloca, local);
			this._ilg.EmitHasValue(typeFromHandle);
			this._ilg.Emit(OpCodes.Or);
			this._ilg.Emit(OpCodes.Brfalse_S, label);
			this._ilg.Emit(OpCodes.Ldloc, local2);
			this.FreeLocal(local2);
			this._ilg.Emit(OpCodes.Br_S, label2);
			this._ilg.MarkLabel(label);
			this._ilg.Emit(OpCodes.Ldloc, local);
			this.FreeLocal(local);
			this._ilg.MarkLabel(label2);
		}

		private void EmitUnliftedOrElse(BinaryExpression b)
		{
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			this.EmitExpressionAndBranch(false, b.Left, label);
			this._ilg.Emit(OpCodes.Ldc_I4_1);
			this._ilg.Emit(OpCodes.Br, label2);
			this._ilg.MarkLabel(label);
			this.EmitExpression(b.Right);
			this._ilg.MarkLabel(label2);
		}

		private void EmitMethodOrElse(BinaryExpression b, LambdaCompiler.CompilationFlags flags)
		{
			Label label = this._ilg.DefineLabel();
			this.EmitExpression(b.Left);
			this._ilg.Emit(OpCodes.Dup);
			MethodInfo booleanOperator = TypeUtils.GetBooleanOperator(b.Method.DeclaringType, "op_True");
			this._ilg.Emit(OpCodes.Call, booleanOperator);
			this._ilg.Emit(OpCodes.Brtrue, label);
			this.EmitExpression(b.Right);
			if ((flags & LambdaCompiler.CompilationFlags.EmitAsTailCallMask) == LambdaCompiler.CompilationFlags.EmitAsTail)
			{
				this._ilg.Emit(OpCodes.Tailcall);
			}
			this._ilg.Emit(OpCodes.Call, b.Method);
			this._ilg.MarkLabel(label);
		}

		private void EmitOrElseBinaryExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			BinaryExpression binaryExpression = (BinaryExpression)expr;
			if (binaryExpression.Method != null)
			{
				if (binaryExpression.IsLiftedLogical)
				{
					this.EmitExpression(binaryExpression.ReduceUserdefinedLifted());
					return;
				}
				this.EmitMethodOrElse(binaryExpression, flags);
				return;
			}
			else
			{
				if (binaryExpression.Left.Type == typeof(bool?))
				{
					this.EmitLiftedOrElse(binaryExpression);
					return;
				}
				this.EmitUnliftedOrElse(binaryExpression);
				return;
			}
		}

		private void EmitExpressionAndBranch(bool branchValue, Expression node, Label label)
		{
			LambdaCompiler.CompilationFlags compilationFlags = this.EmitExpressionStart(node);
			ExpressionType nodeType = node.NodeType;
			if (nodeType <= ExpressionType.Equal)
			{
				if (nodeType != ExpressionType.AndAlso)
				{
					if (nodeType != ExpressionType.Equal)
					{
						goto IL_007F;
					}
					goto IL_006F;
				}
			}
			else
			{
				switch (nodeType)
				{
				case ExpressionType.Not:
					this.EmitBranchNot(branchValue, (UnaryExpression)node, label);
					goto IL_0093;
				case ExpressionType.NotEqual:
					goto IL_006F;
				case ExpressionType.Or:
					goto IL_007F;
				case ExpressionType.OrElse:
					break;
				default:
					if (nodeType != ExpressionType.Block)
					{
						goto IL_007F;
					}
					this.EmitBranchBlock(branchValue, (BlockExpression)node, label);
					goto IL_0093;
				}
			}
			this.EmitBranchLogical(branchValue, (BinaryExpression)node, label);
			goto IL_0093;
			IL_006F:
			this.EmitBranchComparison(branchValue, (BinaryExpression)node, label);
			goto IL_0093;
			IL_007F:
			this.EmitExpression(node, LambdaCompiler.CompilationFlags.EmitNoExpressionStart | LambdaCompiler.CompilationFlags.EmitAsNoTail);
			this.EmitBranchOp(branchValue, label);
			IL_0093:
			this.EmitExpressionEnd(compilationFlags);
		}

		private void EmitBranchOp(bool branch, Label label)
		{
			this._ilg.Emit(branch ? OpCodes.Brtrue : OpCodes.Brfalse, label);
		}

		private void EmitBranchNot(bool branch, UnaryExpression node, Label label)
		{
			if (node.Method != null)
			{
				this.EmitExpression(node, LambdaCompiler.CompilationFlags.EmitNoExpressionStart | LambdaCompiler.CompilationFlags.EmitAsNoTail);
				this.EmitBranchOp(branch, label);
				return;
			}
			this.EmitExpressionAndBranch(!branch, node.Operand, label);
		}

		private void EmitBranchComparison(bool branch, BinaryExpression node, Label label)
		{
			bool flag = branch == (node.NodeType == ExpressionType.Equal);
			if (node.Method != null)
			{
				this.EmitBinaryMethod(node, LambdaCompiler.CompilationFlags.EmitAsNoTail);
				this.EmitBranchOp(branch, label);
				return;
			}
			if (ConstantCheck.IsNull(node.Left))
			{
				if (node.Right.Type.IsNullableType())
				{
					this.EmitAddress(node.Right, node.Right.Type);
					this._ilg.EmitHasValue(node.Right.Type);
				}
				else
				{
					this.EmitExpression(LambdaCompiler.GetEqualityOperand(node.Right));
				}
				this.EmitBranchOp(!flag, label);
				return;
			}
			if (ConstantCheck.IsNull(node.Right))
			{
				if (node.Left.Type.IsNullableType())
				{
					this.EmitAddress(node.Left, node.Left.Type);
					this._ilg.EmitHasValue(node.Left.Type);
				}
				else
				{
					this.EmitExpression(LambdaCompiler.GetEqualityOperand(node.Left));
				}
				this.EmitBranchOp(!flag, label);
				return;
			}
			if (node.Left.Type.IsNullableType() || node.Right.Type.IsNullableType())
			{
				this.EmitBinaryExpression(node);
				this.EmitBranchOp(branch, label);
				return;
			}
			this.EmitExpression(LambdaCompiler.GetEqualityOperand(node.Left));
			this.EmitExpression(LambdaCompiler.GetEqualityOperand(node.Right));
			this._ilg.Emit(flag ? OpCodes.Beq : OpCodes.Bne_Un, label);
		}

		private static Expression GetEqualityOperand(Expression expression)
		{
			if (expression.NodeType == ExpressionType.Convert)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;
				if (TypeUtils.AreReferenceAssignable(unaryExpression.Type, unaryExpression.Operand.Type))
				{
					return unaryExpression.Operand;
				}
			}
			return expression;
		}

		private void EmitBranchLogical(bool branch, BinaryExpression node, Label label)
		{
			if (node.Method != null || node.IsLifted)
			{
				this.EmitExpression(node);
				this.EmitBranchOp(branch, label);
				return;
			}
			bool flag = node.NodeType == ExpressionType.AndAlso;
			if (branch == flag)
			{
				this.EmitBranchAnd(branch, node, label);
				return;
			}
			this.EmitBranchOr(branch, node, label);
		}

		private void EmitBranchAnd(bool branch, BinaryExpression node, Label label)
		{
			Label label2 = this._ilg.DefineLabel();
			this.EmitExpressionAndBranch(!branch, node.Left, label2);
			this.EmitExpressionAndBranch(branch, node.Right, label);
			this._ilg.MarkLabel(label2);
		}

		private void EmitBranchOr(bool branch, BinaryExpression node, Label label)
		{
			this.EmitExpressionAndBranch(branch, node.Left, label);
			this.EmitExpressionAndBranch(branch, node.Right, label);
		}

		private void EmitBranchBlock(bool branch, BlockExpression node, Label label)
		{
			this.EnterScope(node);
			int expressionCount = node.ExpressionCount;
			for (int i = 0; i < expressionCount - 1; i++)
			{
				this.EmitExpressionAsVoid(node.GetExpression(i));
			}
			this.EmitExpressionAndBranch(branch, node.GetExpression(expressionCount - 1), label);
			this.ExitScope(node);
		}

		private void EmitBlockExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			this.Emit((BlockExpression)expr, LambdaCompiler.UpdateEmitAsTypeFlag(flags, LambdaCompiler.CompilationFlags.EmitAsDefaultType));
		}

		private void Emit(BlockExpression node, LambdaCompiler.CompilationFlags flags)
		{
			int expressionCount = node.ExpressionCount;
			if (expressionCount == 0)
			{
				return;
			}
			this.EnterScope(node);
			LambdaCompiler.CompilationFlags compilationFlags = flags & LambdaCompiler.CompilationFlags.EmitAsTypeMask;
			LambdaCompiler.CompilationFlags compilationFlags2 = flags & LambdaCompiler.CompilationFlags.EmitAsTailCallMask;
			for (int i = 0; i < expressionCount - 1; i++)
			{
				Expression expression = node.GetExpression(i);
				Expression expression2 = node.GetExpression(i + 1);
				LambdaCompiler.CompilationFlags compilationFlags3;
				if (compilationFlags2 != LambdaCompiler.CompilationFlags.EmitAsNoTail)
				{
					GotoExpression gotoExpression = expression2 as GotoExpression;
					if (gotoExpression != null && (gotoExpression.Value == null || !LambdaCompiler.Significant(gotoExpression.Value)) && this.ReferenceLabel(gotoExpression.Target).CanReturn)
					{
						compilationFlags3 = LambdaCompiler.CompilationFlags.EmitAsTail;
					}
					else
					{
						compilationFlags3 = LambdaCompiler.CompilationFlags.EmitAsMiddle;
					}
				}
				else
				{
					compilationFlags3 = LambdaCompiler.CompilationFlags.EmitAsNoTail;
				}
				flags = LambdaCompiler.UpdateEmitAsTailCallFlag(flags, compilationFlags3);
				this.EmitExpressionAsVoid(expression, flags);
			}
			if (compilationFlags == LambdaCompiler.CompilationFlags.EmitAsVoidType || node.Type == typeof(void))
			{
				this.EmitExpressionAsVoid(node.GetExpression(expressionCount - 1), compilationFlags2);
			}
			else
			{
				this.EmitExpressionAsType(node.GetExpression(expressionCount - 1), node.Type, compilationFlags2);
			}
			this.ExitScope(node);
		}

		private void EnterScope(object node)
		{
			if (LambdaCompiler.HasVariables(node) && (this._scope.MergedScopes == null || !this._scope.MergedScopes.Contains(node)))
			{
				CompilerScope compilerScope;
				if (!this._tree.Scopes.TryGetValue(node, out compilerScope))
				{
					compilerScope = new CompilerScope(node, false)
					{
						NeedsClosure = this._scope.NeedsClosure
					};
				}
				this._scope = compilerScope.Enter(this, this._scope);
			}
		}

		private static bool HasVariables(object node)
		{
			BlockExpression blockExpression = node as BlockExpression;
			if (blockExpression != null)
			{
				return blockExpression.Variables.Count > 0;
			}
			return ((CatchBlock)node).Variable != null;
		}

		private void ExitScope(object node)
		{
			if (this._scope.Node == node)
			{
				this._scope = this._scope.Exit();
			}
		}

		private void EmitDefaultExpression(Expression expr)
		{
			DefaultExpression defaultExpression = (DefaultExpression)expr;
			if (defaultExpression.Type != typeof(void))
			{
				this._ilg.EmitDefault(defaultExpression.Type, this);
			}
		}

		private void EmitLoopExpression(Expression expr)
		{
			LoopExpression loopExpression = (LoopExpression)expr;
			this.PushLabelBlock(LabelScopeKind.Statement);
			LabelInfo labelInfo = this.DefineLabel(loopExpression.BreakLabel);
			LabelInfo labelInfo2 = this.DefineLabel(loopExpression.ContinueLabel);
			labelInfo2.MarkWithEmptyStack();
			this.EmitExpressionAsVoid(loopExpression.Body);
			this._ilg.Emit(OpCodes.Br, labelInfo2.Label);
			this.PopLabelBlock(LabelScopeKind.Statement);
			labelInfo.MarkWithEmptyStack();
		}

		private void EmitSwitchExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			SwitchExpression switchExpression = (SwitchExpression)expr;
			if (switchExpression.Cases.Count == 0)
			{
				this.EmitExpressionAsVoid(switchExpression.SwitchValue);
				if (switchExpression.DefaultBody != null)
				{
					this.EmitExpressionAsType(switchExpression.DefaultBody, switchExpression.Type, flags);
				}
				return;
			}
			if (this.TryEmitSwitchInstruction(switchExpression, flags))
			{
				return;
			}
			if (this.TryEmitHashtableSwitch(switchExpression, flags))
			{
				return;
			}
			ParameterExpression parameterExpression = Expression.Parameter(switchExpression.SwitchValue.Type, "switchValue");
			ParameterExpression parameterExpression2 = Expression.Parameter(LambdaCompiler.GetTestValueType(switchExpression), "testValue");
			this._scope.AddLocal(this, parameterExpression);
			this._scope.AddLocal(this, parameterExpression2);
			this.EmitExpression(switchExpression.SwitchValue);
			this._scope.EmitSet(parameterExpression);
			Label[] array = new Label[switchExpression.Cases.Count];
			bool[] array2 = new bool[switchExpression.Cases.Count];
			int i = 0;
			int count = switchExpression.Cases.Count;
			while (i < count)
			{
				this.DefineSwitchCaseLabel(switchExpression.Cases[i], out array[i], out array2[i]);
				foreach (Expression expression in switchExpression.Cases[i].TestValues)
				{
					this.EmitExpression(expression);
					this._scope.EmitSet(parameterExpression2);
					this.EmitExpressionAndBranch(true, Expression.Equal(parameterExpression, parameterExpression2, false, switchExpression.Comparison), array[i]);
				}
				i++;
			}
			Label label = this._ilg.DefineLabel();
			Label label2 = ((switchExpression.DefaultBody == null) ? label : this._ilg.DefineLabel());
			this.EmitSwitchCases(switchExpression, array, array2, label2, label, flags);
		}

		private static Type GetTestValueType(SwitchExpression node)
		{
			if (node.Comparison == null)
			{
				return node.Cases[0].TestValues[0].Type;
			}
			Type type = node.Comparison.GetParametersCached()[1].ParameterType.GetNonRefType();
			if (node.IsLifted)
			{
				type = type.GetNullableType();
			}
			return type;
		}

		private static bool FitsInBucket(List<LambdaCompiler.SwitchLabel> buckets, decimal key, int count)
		{
			decimal num = key - buckets[0].Key + 1m;
			return !(num > 2147483647m) && (buckets.Count + count) * 2 > num;
		}

		private static void MergeBuckets(List<List<LambdaCompiler.SwitchLabel>> buckets)
		{
			while (buckets.Count > 1)
			{
				List<LambdaCompiler.SwitchLabel> list = buckets[buckets.Count - 2];
				List<LambdaCompiler.SwitchLabel> list2 = buckets[buckets.Count - 1];
				if (!LambdaCompiler.FitsInBucket(list, list2[list2.Count - 1].Key, list2.Count))
				{
					return;
				}
				list.AddRange(list2);
				buckets.RemoveAt(buckets.Count - 1);
			}
		}

		private static void AddToBuckets(List<List<LambdaCompiler.SwitchLabel>> buckets, LambdaCompiler.SwitchLabel key)
		{
			if (buckets.Count > 0)
			{
				List<LambdaCompiler.SwitchLabel> list = buckets[buckets.Count - 1];
				if (LambdaCompiler.FitsInBucket(list, key.Key, 1))
				{
					list.Add(key);
					LambdaCompiler.MergeBuckets(buckets);
					return;
				}
			}
			buckets.Add(new List<LambdaCompiler.SwitchLabel> { key });
		}

		private static bool CanOptimizeSwitchType(Type valueType)
		{
			TypeCode typeCode = valueType.GetTypeCode();
			return typeCode - TypeCode.Char <= 8;
		}

		private bool TryEmitSwitchInstruction(SwitchExpression node, LambdaCompiler.CompilationFlags flags)
		{
			if (node.Comparison != null)
			{
				return false;
			}
			Type type = node.SwitchValue.Type;
			if (!LambdaCompiler.CanOptimizeSwitchType(type) || !TypeUtils.AreEquivalent(type, node.Cases[0].TestValues[0].Type))
			{
				return false;
			}
			if (!node.Cases.All<SwitchCase>((SwitchCase c) => c.TestValues.All<Expression>((Expression t) => t is ConstantExpression)))
			{
				return false;
			}
			Label[] array = new Label[node.Cases.Count];
			bool[] array2 = new bool[node.Cases.Count];
			HashSet<decimal> hashSet = new HashSet<decimal>();
			List<LambdaCompiler.SwitchLabel> list = new List<LambdaCompiler.SwitchLabel>();
			for (int i = 0; i < node.Cases.Count; i++)
			{
				this.DefineSwitchCaseLabel(node.Cases[i], out array[i], out array2[i]);
				foreach (Expression expression in node.Cases[i].TestValues)
				{
					ConstantExpression constantExpression = (ConstantExpression)expression;
					decimal num = LambdaCompiler.ConvertSwitchValue(constantExpression.Value);
					if (hashSet.Add(num))
					{
						list.Add(new LambdaCompiler.SwitchLabel(num, constantExpression.Value, array[i]));
					}
				}
			}
			list.Sort((LambdaCompiler.SwitchLabel x, LambdaCompiler.SwitchLabel y) => Math.Sign(x.Key - y.Key));
			List<List<LambdaCompiler.SwitchLabel>> list2 = new List<List<LambdaCompiler.SwitchLabel>>();
			foreach (LambdaCompiler.SwitchLabel switchLabel in list)
			{
				LambdaCompiler.AddToBuckets(list2, switchLabel);
			}
			LocalBuilder local = this.GetLocal(node.SwitchValue.Type);
			this.EmitExpression(node.SwitchValue);
			this._ilg.Emit(OpCodes.Stloc, local);
			Label label = this._ilg.DefineLabel();
			Label label2 = ((node.DefaultBody == null) ? label : this._ilg.DefineLabel());
			LambdaCompiler.SwitchInfo switchInfo = new LambdaCompiler.SwitchInfo(node, local, label2);
			this.EmitSwitchBuckets(switchInfo, list2, 0, list2.Count - 1);
			this.EmitSwitchCases(node, array, array2, label2, label, flags);
			this.FreeLocal(local);
			return true;
		}

		private static decimal ConvertSwitchValue(object value)
		{
			if (value is char)
			{
				return (int)((char)value);
			}
			return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
		}

		private void DefineSwitchCaseLabel(SwitchCase @case, out Label label, out bool isGoto)
		{
			GotoExpression gotoExpression = @case.Body as GotoExpression;
			if (gotoExpression != null && gotoExpression.Value == null)
			{
				LabelInfo labelInfo = this.ReferenceLabel(gotoExpression.Target);
				if (labelInfo.CanBranch)
				{
					label = labelInfo.Label;
					isGoto = true;
					return;
				}
			}
			label = this._ilg.DefineLabel();
			isGoto = false;
		}

		private void EmitSwitchCases(SwitchExpression node, Label[] labels, bool[] isGoto, Label @default, Label end, LambdaCompiler.CompilationFlags flags)
		{
			this._ilg.Emit(OpCodes.Br, @default);
			int i = 0;
			int count = node.Cases.Count;
			while (i < count)
			{
				if (!isGoto[i])
				{
					this._ilg.MarkLabel(labels[i]);
					this.EmitExpressionAsType(node.Cases[i].Body, node.Type, flags);
					if (node.DefaultBody != null || i < count - 1)
					{
						if ((flags & LambdaCompiler.CompilationFlags.EmitAsTailCallMask) == LambdaCompiler.CompilationFlags.EmitAsTail)
						{
							this._ilg.Emit(OpCodes.Ret);
						}
						else
						{
							this._ilg.Emit(OpCodes.Br, end);
						}
					}
				}
				i++;
			}
			if (node.DefaultBody != null)
			{
				this._ilg.MarkLabel(@default);
				this.EmitExpressionAsType(node.DefaultBody, node.Type, flags);
			}
			this._ilg.MarkLabel(end);
		}

		private void EmitSwitchBuckets(LambdaCompiler.SwitchInfo info, List<List<LambdaCompiler.SwitchLabel>> buckets, int first, int last)
		{
			while (first != last)
			{
				int num = (int)(((long)first + (long)last + 1L) / 2L);
				if (first == num - 1)
				{
					this.EmitSwitchBucket(info, buckets[first]);
				}
				else
				{
					Label label = this._ilg.DefineLabel();
					this._ilg.Emit(OpCodes.Ldloc, info.Value);
					this.EmitConstant(buckets[num - 1].Last<LambdaCompiler.SwitchLabel>().Constant);
					this._ilg.Emit(info.IsUnsigned ? OpCodes.Bgt_Un : OpCodes.Bgt, label);
					this.EmitSwitchBuckets(info, buckets, first, num - 1);
					this._ilg.MarkLabel(label);
				}
				first = num;
			}
			this.EmitSwitchBucket(info, buckets[first]);
		}

		private void EmitSwitchBucket(LambdaCompiler.SwitchInfo info, List<LambdaCompiler.SwitchLabel> bucket)
		{
			if (bucket.Count == 1)
			{
				this._ilg.Emit(OpCodes.Ldloc, info.Value);
				this.EmitConstant(bucket[0].Constant);
				this._ilg.Emit(OpCodes.Beq, bucket[0].Label);
				return;
			}
			Label? label = null;
			if (info.Is64BitSwitch)
			{
				label = new Label?(this._ilg.DefineLabel());
				this._ilg.Emit(OpCodes.Ldloc, info.Value);
				this.EmitConstant(bucket.Last<LambdaCompiler.SwitchLabel>().Constant);
				this._ilg.Emit(info.IsUnsigned ? OpCodes.Bgt_Un : OpCodes.Bgt, label.Value);
				this._ilg.Emit(OpCodes.Ldloc, info.Value);
				this.EmitConstant(bucket[0].Constant);
				this._ilg.Emit(info.IsUnsigned ? OpCodes.Blt_Un : OpCodes.Blt, label.Value);
			}
			this._ilg.Emit(OpCodes.Ldloc, info.Value);
			decimal num = bucket[0].Key;
			if (num != 0m)
			{
				this.EmitConstant(bucket[0].Constant);
				this._ilg.Emit(OpCodes.Sub);
			}
			if (info.Is64BitSwitch)
			{
				this._ilg.Emit(OpCodes.Conv_I4);
			}
			Label[] array = new Label[(int)(bucket[bucket.Count - 1].Key - bucket[0].Key + 1m)];
			int num2 = 0;
			foreach (LambdaCompiler.SwitchLabel switchLabel in bucket)
			{
				for (;;)
				{
					decimal num3 = num;
					num = num3 + 1m;
					if (!(num3 != switchLabel.Key))
					{
						break;
					}
					array[num2++] = info.Default;
				}
				array[num2++] = switchLabel.Label;
			}
			this._ilg.Emit(OpCodes.Switch, array);
			if (info.Is64BitSwitch)
			{
				this._ilg.MarkLabel(label.Value);
			}
		}

		private bool TryEmitHashtableSwitch(SwitchExpression node, LambdaCompiler.CompilationFlags flags)
		{
			if (node.Comparison != CachedReflectionInfo.String_op_Equality_String_String && node.Comparison != CachedReflectionInfo.String_Equals_String_String)
			{
				return false;
			}
			int num = 0;
			foreach (SwitchCase switchCase in node.Cases)
			{
				using (IEnumerator<Expression> enumerator2 = switchCase.TestValues.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (!(enumerator2.Current is ConstantExpression))
						{
							return false;
						}
						num++;
					}
				}
			}
			if (num < 7)
			{
				return false;
			}
			List<ElementInit> list = new List<ElementInit>(num);
			ArrayBuilder<SwitchCase> arrayBuilder = new ArrayBuilder<SwitchCase>(node.Cases.Count);
			int num2 = -1;
			MethodInfo dictionaryOfStringInt32_Add_String_Int = CachedReflectionInfo.DictionaryOfStringInt32_Add_String_Int32;
			int i = 0;
			int count = node.Cases.Count;
			while (i < count)
			{
				foreach (Expression expression in node.Cases[i].TestValues)
				{
					ConstantExpression constantExpression = (ConstantExpression)expression;
					if (constantExpression.Value != null)
					{
						list.Add(Expression.ElementInit(dictionaryOfStringInt32_Add_String_Int, new TrueReadOnlyCollection<Expression>(new Expression[]
						{
							constantExpression,
							Utils.Constant(i)
						})));
					}
					else
					{
						num2 = i;
					}
				}
				arrayBuilder.UncheckedAdd(Expression.SwitchCase(node.Cases[i].Body, new TrueReadOnlyCollection<Expression>(new Expression[] { Utils.Constant(i) })));
				i++;
			}
			MemberExpression memberExpression = this.CreateLazyInitializedField<Dictionary<string, int>>("dictionarySwitch");
			Expression expression2 = Expression.Condition(Expression.Equal(memberExpression, Expression.Constant(null, memberExpression.Type)), Expression.Assign(memberExpression, Expression.ListInit(Expression.New(CachedReflectionInfo.DictionaryOfStringInt32_Ctor_Int32, new TrueReadOnlyCollection<Expression>(new Expression[] { Utils.Constant(list.Count) })), list)), memberExpression);
			ParameterExpression parameterExpression = Expression.Variable(typeof(string), "switchValue");
			ParameterExpression parameterExpression2 = Expression.Variable(typeof(int), "switchIndex");
			BlockExpression blockExpression = Expression.Block(new TrueReadOnlyCollection<ParameterExpression>(new ParameterExpression[] { parameterExpression2, parameterExpression }), new TrueReadOnlyCollection<Expression>(new Expression[]
			{
				Expression.Assign(parameterExpression, node.SwitchValue),
				Expression.IfThenElse(Expression.Equal(parameterExpression, Expression.Constant(null, typeof(string))), Expression.Assign(parameterExpression2, Utils.Constant(num2)), Expression.IfThenElse(Expression.Call(expression2, "TryGetValue", null, new Expression[] { parameterExpression, parameterExpression2 }), Utils.Empty, Expression.Assign(parameterExpression2, Utils.Constant(-1)))),
				Expression.Switch(node.Type, parameterExpression2, node.DefaultBody, null, arrayBuilder.ToReadOnly<SwitchCase>())
			}));
			this.EmitExpression(blockExpression, flags);
			return true;
		}

		private void CheckRethrow()
		{
			for (LabelScopeInfo labelScopeInfo = this._labelBlock; labelScopeInfo != null; labelScopeInfo = labelScopeInfo.Parent)
			{
				if (labelScopeInfo.Kind == LabelScopeKind.Catch)
				{
					return;
				}
				if (labelScopeInfo.Kind == LabelScopeKind.Finally)
				{
					break;
				}
			}
			throw Error.RethrowRequiresCatch();
		}

		private void CheckTry()
		{
			for (LabelScopeInfo labelScopeInfo = this._labelBlock; labelScopeInfo != null; labelScopeInfo = labelScopeInfo.Parent)
			{
				if (labelScopeInfo.Kind == LabelScopeKind.Filter)
				{
					throw Error.TryNotAllowedInFilter();
				}
			}
		}

		private void EmitSaveExceptionOrPop(CatchBlock cb)
		{
			if (cb.Variable != null)
			{
				this._scope.EmitSet(cb.Variable);
				return;
			}
			this._ilg.Emit(OpCodes.Pop);
		}

		private void EmitTryExpression(Expression expr)
		{
			TryExpression tryExpression = (TryExpression)expr;
			this.CheckTry();
			this.PushLabelBlock(LabelScopeKind.Try);
			this._ilg.BeginExceptionBlock();
			this.EmitExpression(tryExpression.Body);
			Type type = tryExpression.Type;
			LocalBuilder localBuilder = null;
			if (type != typeof(void))
			{
				localBuilder = this.GetLocal(type);
				this._ilg.Emit(OpCodes.Stloc, localBuilder);
			}
			foreach (CatchBlock catchBlock in tryExpression.Handlers)
			{
				this.PushLabelBlock(LabelScopeKind.Catch);
				if (catchBlock.Filter == null)
				{
					this._ilg.BeginCatchBlock(catchBlock.Test);
				}
				else
				{
					this._ilg.BeginExceptFilterBlock();
				}
				this.EnterScope(catchBlock);
				this.EmitCatchStart(catchBlock);
				this.EmitExpression(catchBlock.Body);
				if (type != typeof(void))
				{
					this._ilg.Emit(OpCodes.Stloc, localBuilder);
				}
				this.ExitScope(catchBlock);
				this.PopLabelBlock(LabelScopeKind.Catch);
			}
			if (tryExpression.Finally != null || tryExpression.Fault != null)
			{
				this.PushLabelBlock(LabelScopeKind.Finally);
				if (tryExpression.Finally != null)
				{
					this._ilg.BeginFinallyBlock();
				}
				else
				{
					this._ilg.BeginFaultBlock();
				}
				this.EmitExpressionAsVoid(tryExpression.Finally ?? tryExpression.Fault);
				this._ilg.EndExceptionBlock();
				this.PopLabelBlock(LabelScopeKind.Finally);
			}
			else
			{
				this._ilg.EndExceptionBlock();
			}
			if (type != typeof(void))
			{
				this._ilg.Emit(OpCodes.Ldloc, localBuilder);
				this.FreeLocal(localBuilder);
			}
			this.PopLabelBlock(LabelScopeKind.Try);
		}

		private void EmitCatchStart(CatchBlock cb)
		{
			if (cb.Filter == null)
			{
				this.EmitSaveExceptionOrPop(cb);
				return;
			}
			Label label = this._ilg.DefineLabel();
			Label label2 = this._ilg.DefineLabel();
			this._ilg.Emit(OpCodes.Isinst, cb.Test);
			this._ilg.Emit(OpCodes.Dup);
			this._ilg.Emit(OpCodes.Brtrue, label2);
			this._ilg.Emit(OpCodes.Pop);
			this._ilg.Emit(OpCodes.Ldc_I4_0);
			this._ilg.Emit(OpCodes.Br, label);
			this._ilg.MarkLabel(label2);
			this.EmitSaveExceptionOrPop(cb);
			this.PushLabelBlock(LabelScopeKind.Filter);
			this.EmitExpression(cb.Filter);
			this.PopLabelBlock(LabelScopeKind.Filter);
			this._ilg.MarkLabel(label);
			this._ilg.BeginCatchBlock(null);
			this._ilg.Emit(OpCodes.Pop);
		}

		private void EmitQuoteUnaryExpression(Expression expr)
		{
			this.EmitQuote((UnaryExpression)expr);
		}

		private void EmitQuote(UnaryExpression quote)
		{
			this.EmitConstant(quote.Operand, quote.Type);
			if (this._scope.NearestHoistedLocals != null)
			{
				this.EmitConstant(this._scope.NearestHoistedLocals, typeof(object));
				this._scope.EmitGet(this._scope.NearestHoistedLocals.SelfVariable);
				this._ilg.Emit(OpCodes.Call, CachedReflectionInfo.RuntimeOps_Quote);
				this._ilg.Emit(OpCodes.Castclass, quote.Type);
			}
		}

		private void EmitThrowUnaryExpression(Expression expr)
		{
			this.EmitThrow((UnaryExpression)expr, LambdaCompiler.CompilationFlags.EmitAsDefaultType);
		}

		private void EmitThrow(UnaryExpression expr, LambdaCompiler.CompilationFlags flags)
		{
			if (expr.Operand == null)
			{
				this.CheckRethrow();
				this._ilg.Emit(OpCodes.Rethrow);
			}
			else
			{
				this.EmitExpression(expr.Operand);
				this._ilg.Emit(OpCodes.Throw);
			}
			this.EmitUnreachable(expr, flags);
		}

		private void EmitUnaryExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			this.EmitUnary((UnaryExpression)expr, flags);
		}

		private void EmitUnary(UnaryExpression node, LambdaCompiler.CompilationFlags flags)
		{
			if (node.Method != null)
			{
				this.EmitUnaryMethod(node, flags);
				return;
			}
			if (node.NodeType != ExpressionType.NegateChecked || !node.Operand.Type.IsInteger())
			{
				this.EmitExpression(node.Operand);
				this.EmitUnaryOperator(node.NodeType, node.Operand.Type, node.Type);
				return;
			}
			Type type = node.Type;
			if (type.IsNullableType())
			{
				Label label = this._ilg.DefineLabel();
				Label label2 = this._ilg.DefineLabel();
				this.EmitExpression(node.Operand);
				LocalBuilder local = this.GetLocal(type);
				this._ilg.Emit(OpCodes.Stloc, local);
				this._ilg.Emit(OpCodes.Ldloca, local);
				this._ilg.EmitGetValueOrDefault(type);
				this._ilg.Emit(OpCodes.Brfalse_S, label);
				Type nonNullableType = type.GetNonNullableType();
				this._ilg.EmitDefault(nonNullableType, null);
				this._ilg.Emit(OpCodes.Ldloca, local);
				this._ilg.EmitGetValueOrDefault(type);
				this.EmitBinaryOperator(ExpressionType.SubtractChecked, nonNullableType, nonNullableType, nonNullableType, false);
				this._ilg.Emit(OpCodes.Newobj, type.GetConstructor(new Type[] { nonNullableType }));
				this._ilg.Emit(OpCodes.Br_S, label2);
				this._ilg.MarkLabel(label);
				this._ilg.Emit(OpCodes.Ldloc, local);
				this.FreeLocal(local);
				this._ilg.MarkLabel(label2);
				return;
			}
			this._ilg.EmitDefault(type, null);
			this.EmitExpression(node.Operand);
			this.EmitBinaryOperator(ExpressionType.SubtractChecked, type, type, type, false);
		}

		private void EmitUnaryOperator(ExpressionType op, Type operandType, Type resultType)
		{
			bool flag = operandType.IsNullableType();
			if (op == ExpressionType.ArrayLength)
			{
				this._ilg.Emit(OpCodes.Ldlen);
				return;
			}
			if (!flag)
			{
				if (op <= ExpressionType.TypeAs)
				{
					switch (op)
					{
					case ExpressionType.Negate:
					case ExpressionType.NegateChecked:
						this._ilg.Emit(OpCodes.Neg);
						return;
					case ExpressionType.UnaryPlus:
						return;
					case ExpressionType.New:
					case ExpressionType.NewArrayInit:
					case ExpressionType.NewArrayBounds:
						goto IL_02EB;
					case ExpressionType.Not:
						if (operandType == typeof(bool))
						{
							this._ilg.Emit(OpCodes.Ldc_I4_0);
							this._ilg.Emit(OpCodes.Ceq);
							return;
						}
						break;
					default:
						if (op != ExpressionType.TypeAs)
						{
							goto IL_02EB;
						}
						if (operandType != resultType)
						{
							if (operandType.IsValueType)
							{
								this._ilg.Emit(OpCodes.Box, operandType);
							}
							this._ilg.Emit(OpCodes.Isinst, resultType);
							if (resultType.IsNullableType())
							{
								this._ilg.Emit(OpCodes.Unbox_Any, resultType);
							}
						}
						return;
					}
				}
				else
				{
					if (op == ExpressionType.Decrement)
					{
						this.EmitConstantOne(resultType);
						this._ilg.Emit(OpCodes.Sub);
						goto IL_02EB;
					}
					if (op == ExpressionType.Increment)
					{
						this.EmitConstantOne(resultType);
						this._ilg.Emit(OpCodes.Add);
						goto IL_02EB;
					}
					switch (op)
					{
					case ExpressionType.OnesComplement:
						break;
					case ExpressionType.IsTrue:
						this._ilg.Emit(OpCodes.Ldc_I4_1);
						this._ilg.Emit(OpCodes.Ceq);
						return;
					case ExpressionType.IsFalse:
						this._ilg.Emit(OpCodes.Ldc_I4_0);
						this._ilg.Emit(OpCodes.Ceq);
						return;
					default:
						goto IL_02EB;
					}
				}
				this._ilg.Emit(OpCodes.Not);
				if (!operandType.IsUnsigned())
				{
					return;
				}
				IL_02EB:
				this.EmitConvertArithmeticResult(op, resultType);
				return;
			}
			if (op == ExpressionType.UnaryPlus)
			{
				return;
			}
			if (op != ExpressionType.TypeAs)
			{
				Label label = this._ilg.DefineLabel();
				Label label2 = this._ilg.DefineLabel();
				LocalBuilder local = this.GetLocal(operandType);
				this._ilg.Emit(OpCodes.Stloc, local);
				this._ilg.Emit(OpCodes.Ldloca, local);
				this._ilg.EmitHasValue(operandType);
				this._ilg.Emit(OpCodes.Brfalse_S, label);
				this._ilg.Emit(OpCodes.Ldloca, local);
				this._ilg.EmitGetValueOrDefault(operandType);
				Type nonNullableType = resultType.GetNonNullableType();
				this.EmitUnaryOperator(op, nonNullableType, nonNullableType);
				ConstructorInfo constructor = resultType.GetConstructor(new Type[] { nonNullableType });
				this._ilg.Emit(OpCodes.Newobj, constructor);
				this._ilg.Emit(OpCodes.Br_S, label2);
				this._ilg.MarkLabel(label);
				this._ilg.Emit(OpCodes.Ldloc, local);
				this.FreeLocal(local);
				this._ilg.MarkLabel(label2);
				return;
			}
			if (operandType != resultType)
			{
				this._ilg.Emit(OpCodes.Box, operandType);
				this._ilg.Emit(OpCodes.Isinst, resultType);
				if (resultType.IsNullableType())
				{
					this._ilg.Emit(OpCodes.Unbox_Any, resultType);
				}
			}
		}

		private void EmitConstantOne(Type type)
		{
			switch (type.GetTypeCode())
			{
			case TypeCode.Int64:
			case TypeCode.UInt64:
				this._ilg.Emit(OpCodes.Ldc_I4_1);
				this._ilg.Emit(OpCodes.Conv_I8);
				return;
			case TypeCode.Single:
				this._ilg.Emit(OpCodes.Ldc_R4, 1f);
				return;
			case TypeCode.Double:
				this._ilg.Emit(OpCodes.Ldc_R8, 1.0);
				return;
			default:
				this._ilg.Emit(OpCodes.Ldc_I4_1);
				return;
			}
		}

		private void EmitUnboxUnaryExpression(Expression expr)
		{
			UnaryExpression unaryExpression = (UnaryExpression)expr;
			this.EmitExpression(unaryExpression.Operand);
			this._ilg.Emit(OpCodes.Unbox_Any, unaryExpression.Type);
		}

		private void EmitConvertUnaryExpression(Expression expr, LambdaCompiler.CompilationFlags flags)
		{
			this.EmitConvert((UnaryExpression)expr, flags);
		}

		private void EmitConvert(UnaryExpression node, LambdaCompiler.CompilationFlags flags)
		{
			if (node.Method != null)
			{
				if (!node.IsLifted || (node.Type.IsValueType && node.Operand.Type.IsValueType))
				{
					this.EmitUnaryMethod(node, flags);
					return;
				}
				Type type = node.Method.GetParametersCached()[0].ParameterType;
				if (type.IsByRef)
				{
					type = type.GetElementType();
				}
				UnaryExpression unaryExpression = Expression.Convert(node.Operand, type);
				node = Expression.Convert(Expression.Call(node.Method, unaryExpression), node.Type);
			}
			if (node.Type == typeof(void))
			{
				this.EmitExpressionAsVoid(node.Operand, flags);
				return;
			}
			if (TypeUtils.AreEquivalent(node.Operand.Type, node.Type))
			{
				this.EmitExpression(node.Operand, flags);
				return;
			}
			this.EmitExpression(node.Operand);
			this._ilg.EmitConvertToType(node.Operand.Type, node.Type, node.NodeType == ExpressionType.ConvertChecked, this);
		}

		private void EmitUnaryMethod(UnaryExpression node, LambdaCompiler.CompilationFlags flags)
		{
			if (node.IsLifted)
			{
				ParameterExpression parameterExpression = Expression.Variable(node.Operand.Type.GetNonNullableType(), null);
				MethodCallExpression methodCallExpression = Expression.Call(node.Method, parameterExpression);
				Type nullableType = methodCallExpression.Type.GetNullableType();
				this.EmitLift(node.NodeType, nullableType, methodCallExpression, new ParameterExpression[] { parameterExpression }, new Expression[] { node.Operand });
				this._ilg.EmitConvertToType(nullableType, node.Type, false, this);
				return;
			}
			this.EmitMethodCallExpression(Expression.Call(node.Method, node.Operand), flags);
		}

		private LambdaCompiler(AnalyzedTree tree, LambdaExpression lambda)
		{
			Type[] parameterTypes = LambdaCompiler.GetParameterTypes(lambda, typeof(Closure));
			DynamicMethod dynamicMethod = new DynamicMethod(lambda.Name ?? "lambda_method", lambda.ReturnType, parameterTypes, true);
			this._tree = tree;
			this._lambda = lambda;
			this._method = dynamicMethod;
			this._ilg = dynamicMethod.GetILGenerator();
			this._hasClosureArgument = true;
			this._scope = tree.Scopes[lambda];
			this._boundConstants = tree.Constants[lambda];
			this.InitializeMethod();
		}

		private LambdaCompiler(AnalyzedTree tree, LambdaExpression lambda, MethodBuilder method)
		{
			CompilerScope compilerScope = tree.Scopes[lambda];
			bool needsClosure = compilerScope.NeedsClosure;
			Type[] parameterTypes = LambdaCompiler.GetParameterTypes(lambda, needsClosure ? typeof(Closure) : null);
			method.SetReturnType(lambda.ReturnType);
			method.SetParameters(parameterTypes);
			ReadOnlyCollection<ParameterExpression> parameters = lambda.Parameters;
			int num = (needsClosure ? 2 : 1);
			int i = 0;
			int count = parameters.Count;
			while (i < count)
			{
				method.DefineParameter(i + num, ParameterAttributes.None, parameters[i].Name);
				i++;
			}
			this._tree = tree;
			this._lambda = lambda;
			this._typeBuilder = (TypeBuilder)method.DeclaringType;
			this._method = method;
			this._hasClosureArgument = needsClosure;
			this._ilg = method.GetILGenerator();
			this._scope = compilerScope;
			this._boundConstants = tree.Constants[lambda];
			this.InitializeMethod();
		}

		private LambdaCompiler(LambdaCompiler parent, LambdaExpression lambda, InvocationExpression invocation)
		{
			this._tree = parent._tree;
			this._lambda = lambda;
			this._method = parent._method;
			this._ilg = parent._ilg;
			this._hasClosureArgument = parent._hasClosureArgument;
			this._typeBuilder = parent._typeBuilder;
			this._scope = this._tree.Scopes[invocation];
			this._boundConstants = parent._boundConstants;
		}

		private void InitializeMethod()
		{
			this.AddReturnLabel(this._lambda);
			this._boundConstants.EmitCacheConstants(this);
		}

		internal ILGenerator IL
		{
			get
			{
				return this._ilg;
			}
		}

		internal IParameterProvider Parameters
		{
			get
			{
				return this._lambda;
			}
		}

		internal bool CanEmitBoundConstants
		{
			get
			{
				return this._method is DynamicMethod;
			}
		}

		internal static Delegate Compile(LambdaExpression lambda)
		{
			LambdaCompiler lambdaCompiler = new LambdaCompiler(LambdaCompiler.AnalyzeLambda(ref lambda), lambda);
			lambdaCompiler.EmitLambdaBody();
			return lambdaCompiler.CreateDelegate();
		}

		internal static void Compile(LambdaExpression lambda, MethodBuilder method)
		{
			new LambdaCompiler(LambdaCompiler.AnalyzeLambda(ref lambda), lambda, method).EmitLambdaBody();
		}

		private static AnalyzedTree AnalyzeLambda(ref LambdaExpression lambda)
		{
			lambda = StackSpiller.AnalyzeLambda(lambda);
			return VariableBinder.Bind(lambda);
		}

		public LocalBuilder GetLocal(Type type)
		{
			return this._freeLocals.TryPop(type) ?? this._ilg.DeclareLocal(type);
		}

		public void FreeLocal(LocalBuilder local)
		{
			this._freeLocals.Push(local.LocalType, local);
		}

		internal int GetLambdaArgument(int index)
		{
			return index + (this._hasClosureArgument ? 1 : 0) + (this._method.IsStatic ? 0 : 1);
		}

		internal void EmitLambdaArgument(int index)
		{
			this._ilg.EmitLoadArg(this.GetLambdaArgument(index));
		}

		internal void EmitClosureArgument()
		{
			this._ilg.EmitLoadArg(0);
		}

		private Delegate CreateDelegate()
		{
			return this._method.CreateDelegate(this._lambda.Type, new Closure(this._boundConstants.ToArray(), null));
		}

		private FieldBuilder CreateStaticField(string name, Type type)
		{
			return this._typeBuilder.DefineField(string.Concat(new object[]
			{
				"<ExpressionCompilerImplementationDetails>{",
				Interlocked.Increment(ref LambdaCompiler.s_counter),
				"}",
				name
			}), type, FieldAttributes.Private | FieldAttributes.Static);
		}

		private MemberExpression CreateLazyInitializedField<T>(string name)
		{
			if (this._method is DynamicMethod)
			{
				return Expression.Field(Expression.Constant(new StrongBox<T>(default(T))), "Value");
			}
			return Expression.Field(null, this.CreateStaticField(name, typeof(T)));
		}

		private readonly StackGuard _guard = new StackGuard();

		private static int s_counter;

		private readonly AnalyzedTree _tree;

		private readonly ILGenerator _ilg;

		private readonly TypeBuilder _typeBuilder;

		private readonly MethodInfo _method;

		private LabelScopeInfo _labelBlock = new LabelScopeInfo(null, LabelScopeKind.Lambda);

		private readonly Dictionary<LabelTarget, LabelInfo> _labelInfo = new Dictionary<LabelTarget, LabelInfo>();

		private CompilerScope _scope;

		private readonly LambdaExpression _lambda;

		private readonly bool _hasClosureArgument;

		private readonly BoundConstants _boundConstants;

		private readonly KeyedStack<Type, LocalBuilder> _freeLocals = new KeyedStack<Type, LocalBuilder>();

		[Flags]
		internal enum CompilationFlags
		{
			EmitExpressionStart = 1,
			EmitNoExpressionStart = 2,
			EmitAsDefaultType = 16,
			EmitAsVoidType = 32,
			EmitAsTail = 256,
			EmitAsMiddle = 512,
			EmitAsNoTail = 1024,
			EmitExpressionStartMask = 15,
			EmitAsTypeMask = 240,
			EmitAsTailCallMask = 3840
		}

		private sealed class SwitchLabel
		{
			internal SwitchLabel(decimal key, object constant, Label label)
			{
				this.Key = key;
				this.Constant = constant;
				this.Label = label;
			}

			internal readonly decimal Key;

			internal readonly Label Label;

			internal readonly object Constant;
		}

		private sealed class SwitchInfo
		{
			internal SwitchInfo(SwitchExpression node, LocalBuilder value, Label @default)
			{
				this.Node = node;
				this.Value = value;
				this.Default = @default;
				this.Type = this.Node.SwitchValue.Type;
				this.IsUnsigned = this.Type.IsUnsigned();
				TypeCode typeCode = this.Type.GetTypeCode();
				this.Is64BitSwitch = typeCode == TypeCode.UInt64 || typeCode == TypeCode.Int64;
			}

			internal readonly SwitchExpression Node;

			internal readonly LocalBuilder Value;

			internal readonly Label Default;

			internal readonly Type Type;

			internal readonly bool IsUnsigned;

			internal readonly bool Is64BitSwitch;
		}

		private delegate void WriteBack(LambdaCompiler compiler);
	}
}
