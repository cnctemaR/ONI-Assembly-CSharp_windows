using System;

namespace ImGuiNET
{
	public struct ImGuiTableSortSpecs
	{
		public unsafe ImGuiTableColumnSortSpecs* Specs;

		public int SpecsCount;

		public byte SpecsDirty;
	}
}
