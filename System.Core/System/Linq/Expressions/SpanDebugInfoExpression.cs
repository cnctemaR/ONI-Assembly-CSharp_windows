using System;

namespace System.Linq.Expressions
{
	internal sealed class SpanDebugInfoExpression : DebugInfoExpression
	{
		internal SpanDebugInfoExpression(SymbolDocumentInfo document, int startLine, int startColumn, int endLine, int endColumn)
			: base(document)
		{
			this._startLine = startLine;
			this._startColumn = startColumn;
			this._endLine = endLine;
			this._endColumn = endColumn;
		}

		public override int StartLine
		{
			get
			{
				return this._startLine;
			}
		}

		public override int StartColumn
		{
			get
			{
				return this._startColumn;
			}
		}

		public override int EndLine
		{
			get
			{
				return this._endLine;
			}
		}

		public override int EndColumn
		{
			get
			{
				return this._endColumn;
			}
		}

		public override bool IsClear
		{
			get
			{
				return false;
			}
		}

		protected internal override Expression Accept(ExpressionVisitor visitor)
		{
			return visitor.VisitDebugInfo(this);
		}

		private readonly int _startLine;

		private readonly int _startColumn;

		private readonly int _endLine;

		private readonly int _endColumn;
	}
}
