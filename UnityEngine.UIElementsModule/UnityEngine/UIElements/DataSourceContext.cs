using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public readonly struct DataSourceContext
	{
		public object dataSource { get; }

		public PropertyPath dataSourcePath { get; }

		public DataSourceContext(object dataSource, in PropertyPath dataSourcePath)
		{
			this.dataSource = dataSource;
			this.dataSourcePath = dataSourcePath;
		}
	}
}
