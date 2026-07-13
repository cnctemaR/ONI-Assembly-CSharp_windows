using System;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public interface IDataSourceProvider
	{
		object dataSource { get; }

		PropertyPath dataSourcePath { get; }
	}
}
