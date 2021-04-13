using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloth
{
    static class Camera
    {
        static Vector3 position = new Vector3(0.0f, -5.0f, 10f);

        static Vector3 front;
        static Vector3 right;
        static Vector3 up;

        static float pitch = 0f;
        static float yaw = -90f;
        static float rotSpeed = 0.04f;
        static float moveSpeed = 4f;

        static Camera()
        {
            front = new Vector3(0.0f, 0.0f, -1.0f);
            right = new Vector3(1.0f, 0.0f, 0.0f);
            up = -Vector3.Cross(front, right);
            MouseMove(0, 0);
        }

        public static Vector3 GetFront()
        {
            return front;
        }

        static public void move(KeyboardState input)
        {
            float time = 0.02f;

            if (input.IsKeyDown(Key.A))
            {
                position -= GetRight() * moveSpeed * time;
            }
            if (input.IsKeyDown(Key.D))
            {
                position += GetRight() * moveSpeed * time;
            }
            if (input.IsKeyDown(Key.W))
            {
                position += front * moveSpeed * time;
            }
            if (input.IsKeyDown(Key.S))
            {
                position -= front * moveSpeed * time;
            }
            if (input.IsKeyDown(Key.Space))
            {
                position += new Vector3(0f, moveSpeed * time, 0f);
            }
            if (input.IsKeyDown(Key.J))
            {
                moveSpeed = moveSpeed * (1f + 2 * time);
            }
            if (input.IsKeyDown(Key.K))
            {
                moveSpeed = moveSpeed / (1f + 2 * time);
            }
            if (input.IsKeyDown(Key.L))
            {
                position = new Vector3(position.X, 20f, position.Z);
                moveSpeed = 4f;
            }
            if (input.IsKeyDown(Key.ShiftLeft))
            {
                position -= new Vector3(0f, moveSpeed * time, 0f);
            }
        }

        static public void MouseMove(int dx, int dy)
        {
            pitch -= ((float)dy) * rotSpeed;
            yaw += ((float)dx) * rotSpeed;

            if (pitch > 89.99f)
                pitch = 89.99f;
            if (pitch < -89.99f)
                pitch = -89.99f;

            front.X = (float)(Math.Cos(MathHelper.DegreesToRadians(pitch)) * Math.Cos(MathHelper.DegreesToRadians(yaw)));
            front.Y = (float)(Math.Sin(MathHelper.DegreesToRadians(pitch)));
            front.Z = (float)(Math.Cos(MathHelper.DegreesToRadians(pitch)) * Math.Sin(MathHelper.DegreesToRadians(yaw)));
            front = Vector3.Normalize(front);
            right = Vector3.Cross(front, up);
            right.Normalize();
        }

        static public float GetYaw()
        {
            return yaw;
        }

        static public Vector3 GetRight()
        {
            return right.Normalized();
        }

        static public Matrix4 GetViewMatrix()
        {
            return Matrix4.CreateTranslation(-position) * Matrix4.LookAt(new Vector3(0.0f, 0.0f, 0.0f), front, up);
        }
    }
}
