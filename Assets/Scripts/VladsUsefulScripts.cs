using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VladsUsefulScripts
{
    public static class Finders
    {
        public static bool TryFindObjectOfType<T>(out T val) where T : Object
        {
            if (Object.FindObjectOfType<T>() != null)
            {
                val = Object.FindObjectOfType<T>();
                return true;
            }
            else
            {
                val = null;
                return false;
            }

        }
        public static bool TryFindObjectsOfType<T>(out T[] val) where T : Object
        {
            if (Object.FindObjectsOfType<T>() != null)
            {
                val = Object.FindObjectsOfType<T>();
                return true;
            }
            else
            {
                val = null;
                return false;
            }

        }
    }
    public static class Clampers
    {
        public static Vector2 VelCalc(Vector2 velocity, float speed, float maxSpeedMult)
        {
            return new Vector2(Mathf.Clamp(velocity.x + (Input.GetAxisRaw("Horizontal") * speed), -speed * maxSpeedMult, speed * maxSpeedMult), velocity.y);
        }
        public static Vector3 VelCalc(Vector3 velocity, float speed, float maxSpeedMult)
        {
            return new Vector3(Mathf.Clamp(velocity.x, -speed * maxSpeedMult, speed * maxSpeedMult), velocity.y, Mathf.Clamp(velocity.z, -speed * maxSpeedMult, speed * maxSpeedMult));
        }
        public static Vector2 Drag(Vector2 vector, float drag)
        {
            if (vector.x > drag) vector.x -= drag;
            else if (vector.x < -drag) vector.x += drag;
            if (vector.x < drag * 1.2 && vector.x > -drag * 1.2)
            {
                vector.x = 0;
                return vector;
            }
            return vector;
        }
        public static Vector2 ClampedDrag(Vector2 vector, float drag, float min, float max)
        {
            if (vector.x > drag) vector.x -= drag;
            else if (vector.x < -drag) vector.x += drag;
            if (vector.x < drag * 1.2 && vector.x > -drag * 1.2)
            {
                vector.x = 0;
                return new Vector2(Mathf.Clamp(vector.x, min, max), vector.y);
            }
            return vector;
        }
    }
}
