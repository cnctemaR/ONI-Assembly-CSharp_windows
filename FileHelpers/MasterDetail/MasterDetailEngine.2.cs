using System;

namespace FileHelpers.MasterDetail
{
	public sealed class MasterDetailEngine : MasterDetailEngine<object, object>
	{
		public MasterDetailEngine(Type masterType, Type detailType)
			: this(masterType, detailType, null)
		{
		}

		public MasterDetailEngine(Type masterType, Type detailType, MasterDetailSelector recordSelector)
			: base(masterType, detailType, recordSelector)
		{
		}

		public MasterDetailEngine(Type masterType, Type detailType, CommonSelector action, string selector)
			: base(masterType, detailType, action, selector)
		{
		}
	}
}
