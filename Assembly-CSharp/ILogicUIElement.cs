using System;

public interface ILogicUIElement : IUniformGridObject
{
	int GetLogicUICell();

	bool IsLogicInput();
}
