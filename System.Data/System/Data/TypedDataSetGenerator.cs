using System;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace System.Data
{
	[Obsolete("TypedDataSetGenerator class will be removed in a future release. Please use System.Data.Design.TypedDataSetGenerator in System.Design.dll.")]
	public class TypedDataSetGenerator
	{
		public static void Generate(DataSet dataSet, CodeNamespace codeNamespace, ICodeGenerator codeGen)
		{
			CustomDataClassGenerator.CreateDataSetClasses(dataSet, codeNamespace, codeGen, null);
		}

		public static string GenerateIdName(string name, ICodeGenerator codeGen)
		{
			return CustomDataClassGenerator.MakeSafeName(name, codeGen);
		}
	}
}
