using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[ComVisible(true)]
	[StructLayout(LayoutKind.Sequential)]
	public class MethodBody
	{
		protected MethodBody()
		{
		}

		public virtual IList<ExceptionHandlingClause> ExceptionHandlingClauses
		{
			get
			{
				return Array.AsReadOnly<ExceptionHandlingClause>(this.clauses);
			}
		}

		public virtual IList<LocalVariableInfo> LocalVariables
		{
			get
			{
				return Array.AsReadOnly<LocalVariableInfo>(this.locals);
			}
		}

		public virtual bool InitLocals
		{
			get
			{
				return this.init_locals;
			}
		}

		public virtual int LocalSignatureMetadataToken
		{
			get
			{
				return this.sig_token;
			}
		}

		public virtual int MaxStackSize
		{
			get
			{
				return this.max_stack;
			}
		}

		public virtual byte[] GetILAsByteArray()
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
