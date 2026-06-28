using System;

public interface IComponent
{
	void Update(float dt);

	void FixedUpdate(float dt);

	void SimUpdate(float dt);
}
