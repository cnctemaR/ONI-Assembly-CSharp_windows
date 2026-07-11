using System;

namespace System.Xml
{
	internal enum ElementProperties : uint
	{
		DEFAULT,
		URI_PARENT,
		BOOL_PARENT,
		NAME_PARENT = 4U,
		EMPTY = 8U,
		NO_ENTITIES = 16U,
		HEAD = 32U,
		BLOCK_WS = 64U,
		HAS_NS = 128U
	}
}
