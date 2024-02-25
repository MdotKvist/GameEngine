using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Threading;
using System.Drawing.Imaging;

namespace OpenGLMinecraft
{
	class Camera
	{
		private Vector3 position;
		private Vector3 front;
		private Vector3 up;
		private Vector3 right;
		private float yaw;
		private float pitch;
		private float speed;
		private float sensitivity;

		public Camera(Vector3 startPosition)
		{
			position = startPosition;
			front = -Vector3.UnitZ;
			up = Vector3.UnitY;
			yaw = -90.0f;
			pitch = 0.0f;
			speed = 0.5f;
			sensitivity = 0.1f;
		}

		public Matrix4 GetViewMatrix()
		{
			return Matrix4.LookAt(position, position + front, up);
		}

		public void UpdateCameraDirection(float mouseX, float mouseY)
		{
			yaw += mouseX * sensitivity;
			pitch -= mouseY * sensitivity;

			if (pitch > 89.0f)
				pitch = 89.0f;
			if (pitch < -89.0f)
				pitch = -89.0f;

			Vector3 frontDirection;
			frontDirection.X = (float)Math.Cos(MathHelper.DegreesToRadians(yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(pitch));
			frontDirection.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
			frontDirection.Z = (float)Math.Sin(MathHelper.DegreesToRadians(yaw)) * (float)Math.Cos(MathHelper.DegreesToRadians(pitch));
			front = Vector3.Normalize(frontDirection);

			right = Vector3.Normalize(Vector3.Cross(front, up));
		}
	}
}
