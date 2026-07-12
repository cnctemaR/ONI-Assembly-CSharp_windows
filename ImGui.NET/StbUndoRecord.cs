using System;

namespace ImGuiNET
{
	public struct StbUndoRecord
	{
		public int where;

		public int insert_length;

		public int delete_length;

		public int char_storage;
	}
}
