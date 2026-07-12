using System;
using System.Collections.Generic;

namespace ImGuiObjectDrawer
{
	public interface IMemberDrawerProvider
	{
		int Priority { get; }

		void AppendDrawersTo(List<MemberDrawer> drawers);
	}
}
