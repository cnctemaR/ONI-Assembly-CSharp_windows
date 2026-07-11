using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony.ILCopying
{
	public class MethodCopier
	{
		public MethodCopier(MethodBase fromMethod, ILGenerator toILGenerator, LocalBuilder[] existingVariables = null)
		{
			bool flag = fromMethod == null;
			if (flag)
			{
				throw new ArgumentNullException("Method cannot be null");
			}
			this.reader = new MethodBodyReader(fromMethod, toILGenerator);
			this.reader.DeclareVariables(existingVariables);
			this.reader.ReadInstructions();
		}

		public void AddTranspiler(MethodInfo transpiler)
		{
			this.transpilers.Add(transpiler);
		}

		public void Finalize(List<Label> endLabels, List<ExceptionBlock> endBlocks)
		{
			this.reader.FinalizeILCodes(this.transpilers, endLabels, endBlocks);
		}

		private readonly MethodBodyReader reader;

		private readonly List<MethodInfo> transpilers = new List<MethodInfo>();
	}
}
