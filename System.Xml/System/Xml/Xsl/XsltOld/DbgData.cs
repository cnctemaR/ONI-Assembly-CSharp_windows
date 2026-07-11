using System;
using System.Xml.XPath;

namespace System.Xml.Xsl.XsltOld
{
	internal class DbgData
	{
		public XPathNavigator StyleSheet
		{
			get
			{
				return this.styleSheet;
			}
		}

		public VariableAction[] Variables
		{
			get
			{
				return this.variables;
			}
		}

		public DbgData(Compiler compiler)
		{
			DbgCompiler dbgCompiler = (DbgCompiler)compiler;
			this.styleSheet = dbgCompiler.Input.Navigator.Clone();
			this.variables = dbgCompiler.LocalVariables;
			dbgCompiler.Debugger.OnInstructionCompile(this.StyleSheet);
		}

		internal void ReplaceVariables(VariableAction[] vars)
		{
			this.variables = vars;
		}

		private DbgData()
		{
			this.styleSheet = null;
			this.variables = new VariableAction[0];
		}

		public static DbgData Empty
		{
			get
			{
				return DbgData.s_nullDbgData;
			}
		}

		private XPathNavigator styleSheet;

		private VariableAction[] variables;

		private static DbgData s_nullDbgData = new DbgData();
	}
}
