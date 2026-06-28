using System;

public interface IComponentManager
{
	void Update(float dt);

	void FixedUpdate(float dt);

	int Count { get; }
}
