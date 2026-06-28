using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	public sealed class MethodBody
	{
		internal MethodBody()
		{
		}

		public IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get
			{
				return Array.AsReadOnly<ExceptionHandlingClause>(this.clauses);
			}
		}

		public IList<LocalVariableInfo> LocalVariables
		{
			get
			{
				return Array.AsReadOnly<LocalVariableInfo>(this.locals);
			}
		}

		public bool InitLocals
		{
			get
			{
				return this.init_locals;
			}
		}

		public int LocalSignatureMetadataToken
		{
			get
			{
				return this.sig_token;
			}
		}

		public int MaxStackSize
		{
			get
			{
				return this.max_stack;
			}
		}

		public byte[] GetILAsByteArray()
		{
			return this.il;
		}

		private ExceptionHandlingClause[] clauses;

		private LocalVariableInfo[] locals;

		private byte[] il;

		private bool init_locals;

		private int sig_token;

		private int max_stack;
	}
}
