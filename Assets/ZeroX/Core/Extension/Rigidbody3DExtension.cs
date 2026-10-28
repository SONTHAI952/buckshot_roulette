using UnityEngine;

namespace ZeroX.Extensions
{
    public static class Rigidbody3DExtension
    {
        public static void SetVelocityX(this Rigidbody rigidbody, float x)
        {
            Vector3 vel = rigidbody.linearVelocity;
            vel.x = x;
            rigidbody.linearVelocity = vel;
        }

        public static void SetVelocityY(this Rigidbody rigidbody, float y)
        {
            Vector3 vel = rigidbody.linearVelocity;
            vel.y = y;
            rigidbody.linearVelocity = vel;
        }
        public static void SetVelocityZ(this Rigidbody rigidbody, float z)
        {
            Vector3 vel = rigidbody.linearVelocity;
            vel.z = z;
            rigidbody.linearVelocity = vel;
        }
        
        public static void AddForceLimitVelocity(this Rigidbody rigidbody, Vector3 force, float maxVelocity)
        {
            Vector3 vel = rigidbody.linearVelocity;
            vel += force * Time.fixedDeltaTime;
            vel = Vector3.ClampMagnitude(vel, maxVelocity);
            rigidbody.linearVelocity = vel;
        }
    }
}
