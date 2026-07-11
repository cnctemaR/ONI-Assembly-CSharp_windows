using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using Unity;

namespace System.Linq.Expressions
{
	[DebuggerTypeProxy(typeof(Expression.DebugInfoExpressionProxy))]
	public class DebugInfoExpression : Expression
	{
		internal DebugInfoExpression(SymbolDocumentInfo document)
		{
			this.Document = document;
		}

		public sealed override Type Type
		{
			get
			{
				return typeof(void);
			}
		}

		public sealed override ExpressionType NodeType
		{
			get
			{
				return ExpressionType.DebugInfo;
			}
		}

		[ExcludeFromCodeCoverage]
		public virtual int StartLine
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		[ExcludeFromCodeCoverage]
		public virtual int StartColumn
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		[ExcludeFromCodeCoverage]
		public virtual int EndLine
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		[ExcludeFromCodeCoverage]
		public virtual int EndColumn
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		public SymbolDocumentInfo Document { get; }

		[ExcludeFromCodeCoverage]
		public virtual bool IsClear
		{
			get
			{
				throw ContractUtils.Unreachable;
			}
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitDebugInfo(this);
		}

		internal DebugInfoExpression()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
