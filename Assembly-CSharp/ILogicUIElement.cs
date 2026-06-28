using System;

public interface ILogicUIElement : IUniformGridObject
{
	int GetLogicUICell();

	LogicPortSpriteType GetLogicPortSpriteType();
}
