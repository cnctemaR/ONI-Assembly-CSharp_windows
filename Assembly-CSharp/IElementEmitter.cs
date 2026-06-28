using System;

public interface IElementEmitter
{
	SimHashes Element { get; }

	float AverageEmitRate { get; }
}
