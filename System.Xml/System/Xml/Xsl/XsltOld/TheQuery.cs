using System;
using MS.Internal.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal sealed class TheQuery
	{
		internal CompiledXpathExpr CompiledQuery
		{
			get
			{
				return this._CompiledQuery;
			}
		}

		internal TheQuery(CompiledXpathExpr compiledQuery, InputScopeManager manager)
		{
			this._CompiledQuery = compiledQuery;
			this._ScopeManager = manager.Clone();
		}

		internal InputScopeManager _ScopeManager;

		private CompiledXpathExpr _CompiledQuery;
	}
}
