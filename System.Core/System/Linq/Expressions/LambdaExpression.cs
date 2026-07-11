using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Linq.Expressions.Compiler;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.LambdaExpressionProxy))]
	public abstract class LambdaExpression : Expression, IParameterProvider
	{
		internal LambdaExpression(Expression body)
		{
			this._body = body;
		}

		public sealed override Type Type
		{
			get
			{
				return this.TypeCore;
			}
		}

		internal abstract Type TypeCore { get; }

		internal abstract Type PublicType { get; }

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.Lambda;
			}
		}

		public ReadOnlyCollection<ParameterExpression> Parameters
		{
			get
			{
				return this.GetOrMakeParameters();
			}
		}

		public string Name
		{
			get
			{
				return this.NameCore;
			}
		}

		internal virtual string NameCore
		{
			get
			{
				return null;
			}
		}

		public Expression Body
		{
			get
			{
				return this._body;
			}
		}

		public Type ReturnType
		{
			get
			{
				return this.Type.GetInvokeMethod().ReturnType;
			}
		}

		public bool TailCall
		{
			get
			{
				return this.TailCallCore;
			}
		}

		internal virtual bool TailCallCore
		{
			get
			{
				return false;
			}
		}

		[ExcludeFromCodeCoverage]
		internal virtual ReadOnlyCollection<ParameterExpression> GetOrMakeParameters()
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		ParameterExpression IParameterProvider.GetParameter(int index)
		{
			return this.GetParameter(index);
		}

		[ExcludeFromCodeCoverage]
		internal virtual ParameterExpression GetParameter(int index)
		{
			throw ContractUtils.Unreachable;
		}

		[ExcludeFromCodeCoverage]
		int IParameterProvider.ParameterCount
		{
			get
			{
				return this.ParameterCount;
			}
		}

		[ExcludeFromCodeCoverage]
		internal virtual int ParameterCount
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		public Delegate Compile()
		{
			return this.Compile(false);
		}

		public Delegate Compile(bool preferInterpretation)
		{
			return LambdaCompiler.Compile(this);
		}

		public void CompileToMethod(MethodBuilder method)
		{
			ContractUtils.RequiresNotNull(method, "method");
			ContractUtils.Requires(method.IsStatic, "method");
			if (method.DeclaringType as TypeBuilder == null)
			{
				throw Error.MethodBuilderDoesNotHaveTypeBuilder();
			}
			LambdaCompiler.Compile(this, method);
		}

		internal abstract LambdaExpression Accept(StackSpiller spiller);

		public Delegate Compile(DebugInfoGenerator debugInfoGenerator)
		{
			return this.Compile();
		}

		public void CompileToMethod(MethodBuilder method, DebugInfoGenerator debugInfoGenerator)
		{
			this.CompileToMethod(method);
		}

		internal LambdaExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly Expression _body;
	}
}
