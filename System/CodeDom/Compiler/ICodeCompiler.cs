using System;

namespace System.CodeDom.Compiler
{
	public interface ICodeCompiler
	{
		CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit);

		CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] batch);

		CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName);

		CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] batch);

		CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source);

		CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] batch);
	}
}
