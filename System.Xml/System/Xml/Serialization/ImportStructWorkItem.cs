using System;

namespace System.Xml.Serialization
{
	internal class ImportStructWorkItem
	{
		internal ImportStructWorkItem(StructModel model, StructMapping mapping)
		{
			this.model = model;
			this.mapping = mapping;
		}

		internal StructModel Model
		{
			get
			{
				return this.model;
			}
		}

		internal StructMapping Mapping
		{
			get
			{
				return this.mapping;
			}
		}

		private StructModel model;

		private StructMapping mapping;
	}
}
