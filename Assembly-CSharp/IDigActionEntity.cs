using System;

public interface IDigActionEntity
{
	void Dig();

	void MarkForDig(bool instantOnDebug = true);
}
