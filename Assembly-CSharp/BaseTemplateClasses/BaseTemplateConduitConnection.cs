using System;

namespace BaseTemplateClasses
{
	[Serializable]
	public class BaseTemplateConduitConnection
	{
		public BaseTemplateConduitConnection(BaseTemplateConduitConnection.BaseTemplateConduitSystemType _systemType, UtilityConnections _connection)
		{
			this.systemType = _systemType;
			this.connection = _connection;
		}

		public BaseTemplateConduitConnection.BaseTemplateConduitSystemType systemType;

		public UtilityConnections connection;

		public enum BaseTemplateConduitSystemType
		{
			None,
			Electrical,
			Liquid,
			Gas
		}
	}
}
