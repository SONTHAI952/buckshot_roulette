using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ZeroX.Extensions
{
    public static class Rigidbody2DExtension
    {
        public static void SetVelocityX(this Rigidbody2D rigidbody2D, float x)
        {
            Vector2 vel = rigidbody2D.linearVelocity;
            vel.x = x;
            rigidbody2D.linearVelocity = vel;
        }

        public static void SetVelocityY(this Rigidbody2D rigidbody2D, float y)
        {
            Vector2 vel = rigidbody2D.linearVelocity;
            vel.y = y;
            rigidbody2D.linearVelocity = vel;
        }

        public static void AddForceLimitVelocity(this Rigidbody2D rigidbody2D, Vector2 force, float maxVelocity)
        {
            Vector2 vel = rigidbody2D.linearVelocity;
            vel += force * Time.deltaTime;
            vel = Vector2.ClampMagnitude(vel, maxVelocity);
            rigidbody2D.linearVelocity = vel;
        }

        public static void AddExplosionForce(this Rigidbody2D rigidbody2D, float explosionForce,
            Vector2 explosionPosition, float explosionRadius, ForceMode2D forceMode = ForceMode2D.Impulse)
        {
            Vector2 direct = (Vector2) rigidbody2D.transform.position - explosionPosition;
            float distance = direct.magnitude;
            if (distance >= explosionRadius)
                return;
            rigidbody2D.AddForce(direct.normalized * explosionForce * (1 - distance / explosionRadius), forceMode);
        }

        public static void AddExplosionForce(this Rigidbody2D rigidbody2D, float explosionForce,
            Vector2 explosionPosition, float explosionRadius, float upwardsModifier,
            ForceMode2D forceMode = ForceMode2D.Impulse)
        {
            Vector2 direct = (Vector2) rigidbody2D.transform.position - explosionPosition;
            float distance = direct.magnitude;
            if (distance >= explosionRadius)
                return;
            rigidbody2D.AddForce(
                direct.normalized * explosionForce * (1 - distance / explosionRadius) + Vector2.up * upwardsModifier,
                forceMode);
        }


        public static void FollowTarget(this Rigidbody2D rigidbody2D, Transform target, float maxFollowSpeed,
            float brakeMulti)
        {
            Vector2 velocity = rigidbody2D.linearVelocity;
            Vector2 force = (Vector2) target.position - (Vector2) rigidbody2D.transform.position;
            float angel = Vector2.Angle(rigidbody2D.linearVelocity, force);
            bool brake = angel > 45;
            if (brake)
            {
                force = force.normalized * maxFollowSpeed + (-velocity * brakeMulti);
            }
            else
                force = force.normalized * maxFollowSpeed;


            velocity += force * Time.deltaTime;
            velocity = Vector2.ClampMagnitude(velocity, maxFollowSpeed);
            rigidbody2D.linearVelocity = velocity;
        }

        public static void FollowTarget(this Rigidbody2D rigidbody2D, Transform target, float maxFollowSpeed,
            float brakeMulti, float restrictRadius)
        {
            Vector2 velocity = rigidbody2D.linearVelocity;
            Vector2 force;
            float distanceToTarget = Vector2.Distance(rigidbody2D.transform.position, target.position);
            if (distanceToTarget < restrictRadius)
            {
                force = velocity;
            }
            else
            {
                force = (Vector2) target.position - (Vector2) rigidbody2D.transform.position;
            }

            float angel = Vector2.Angle(rigidbody2D.linearVelocity, force);
            bool brake = angel > 45;
            if (brake)
            {
                force = force.normalized * maxFollowSpeed + (-velocity * brakeMulti);
            }
            else
                force = force.normalized * maxFollowSpeed;


            velocity += force * Time.deltaTime;
            velocity = Vector2.ClampMagnitude(velocity, maxFollowSpeed);
            rigidbody2D.linearVelocity = velocity;
        }

        public static void VelocityYTowards(this Rigidbody2D rigidbody2D, float velY, float maxDelta)
        {
            Vector2 vel = rigidbody2D.linearVelocity;
            vel.y = Mathf.MoveTowards(vel.y, velY, maxDelta);
            rigidbody2D.linearVelocity = vel;
        }
    }
}