using System;

namespace System.Drawing.Design
{
	public interface IToolboxItemProvider
	{
		ToolboxItemCollection Items { get; }
	}
}
