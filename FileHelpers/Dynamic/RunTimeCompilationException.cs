using System;
using System.CodeDom.Compiler;

namespace FileHelpers.Dynamic
{
	[Serializable]
	public sealed class RunTimeCompilationException : FileHelpersException
	{
		internal RunTimeCompilationException(string message, string sourceCode, CompilerErrorCollection errors)
			: base(message)
		{
			this.mSourceCode = sourceCode;
			this.mCompilerErrors = errors;
		}

		public string SourceCode
		{
			get
			{
				return this.mSourceCode;
			}
		}

		public CompilerErrorCollection CompilerErrors
		{
			get
			{
				return this.mCompilerErrors;
			}
		}

		private readonly string mSourceCode;

		private readonly CompilerErrorCollection mCompilerErrors;
	}
}
