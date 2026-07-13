using System;

namespace UnityEngine.LowLevelPhysics2D
{
	internal interface IPhysicsJoint
	{
		bool Destroy(int ownerKey = 0);

		bool isValid { get; }

		PhysicsWorld world { get; }

		PhysicsJoint.JointType jointType { get; }

		PhysicsBody bodyA { get; }

		PhysicsBody bodyB { get; }

		PhysicsTransform localAnchorA { get; set; }

		PhysicsTransform localAnchorB { get; set; }

		float forceThreshold { get; set; }

		float torqueThreshold { get; set; }

		bool collideConnected { get; set; }

		float tuningFrequency { get; set; }

		float tuningDamping { get; set; }

		float drawScale { get; set; }

		void WakeBodies();

		Vector2 currentConstraintForce { get; }

		float currentConstraintTorque { get; }

		float currentLinearSeparationError { get; }

		float currentAngularSeparationError { get; }

		int SetOwner(Object owner);

		Object GetOwner();

		bool isOwned { get; }

		object callbackTarget { get; set; }

		PhysicsUserData userData { get; set; }

		void Draw();
	}
}
