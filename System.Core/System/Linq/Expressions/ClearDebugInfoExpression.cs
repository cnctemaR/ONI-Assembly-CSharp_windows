using System;

namespace System.Linq.Expressions
{
	internal sealed class ClearDebugInfoExpression : DebugInfoExpression
	{
		internal ClearDebugInfoExpression(SymbolDocumentInfo document)
			: base(document)
		{
		}

		public override bool IsClear
		{
			get
			{
				return true;
			}
		}

		public override int StartLine
		{
			get
			{
				return 16707566;
			}
		}

		public override int StartColumn
		{
			get
			{
				return 0;
			}
		}

		public override int EndLine
		{
			get
			{
				return 16707566;
			}
		}

		public override int EndColumn
		{
			get
			{
				return 0;
			}
		}
	}
}
