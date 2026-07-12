using System;

namespace ImGuiObjectDrawer
{
	public readonly struct MemberDrawContext
	{
		public MemberDrawContext(bool hide_default_values, bool default_open)
		{
			this.hide_default_values = hide_default_values;
			this.default_open = default_open;
		}

		public readonly bool hide_default_values;

		public readonly bool default_open;
	}
}
