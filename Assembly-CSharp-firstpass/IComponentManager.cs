using System;

public interface IComponentManager
{
	void Spawn();

	void Update(float dt);

	void FixedUpdate(float dt);

	void SimUpdate(float dt);

	void CleanUp();

	void Clear();

	bool Has(object go);

	int Count { get; }
}
