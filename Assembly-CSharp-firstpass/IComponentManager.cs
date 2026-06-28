using System;

public interface IComponentManager
{
	void Spawn();

	void RenderEveryTick(float dt);

	void FixedUpdate(float dt);

	void Sim200ms(float dt);

	void CleanUp();

	void Clear();

	bool Has(object go);

	int Count { get; }
}
