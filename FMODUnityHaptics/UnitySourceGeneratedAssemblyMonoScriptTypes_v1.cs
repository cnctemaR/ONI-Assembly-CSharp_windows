using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[CompilerGenerated]
[EditorBrowsable(EditorBrowsableState.Never)]
[GeneratedCode("Unity.MonoScriptGenerator.MonoScriptInfoGenerator", null)]
internal class UnitySourceGeneratedAssemblyMonoScriptTypes_v1
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static UnitySourceGeneratedAssemblyMonoScriptTypes_v1.MonoScriptData Get()
	{
		UnitySourceGeneratedAssemblyMonoScriptTypes_v1.MonoScriptData monoScriptData = default(UnitySourceGeneratedAssemblyMonoScriptTypes_v1.MonoScriptData);
		monoScriptData.FilePathsData = new byte[0];
		monoScriptData.TypesData = new byte[0];
		monoScriptData.TotalFiles = 0;
		monoScriptData.TotalTypes = 0;
		monoScriptData.IsEditorOnly = false;
		return monoScriptData;
	}

	private struct MonoScriptData
	{
		public byte[] FilePathsData;

		public byte[] TypesData;

		public int TotalTypes;

		public int TotalFiles;

		public bool IsEditorOnly;
	}
}
