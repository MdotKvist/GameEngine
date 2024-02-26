using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace OpenGLMinecraft
{
    public class Chunk
    {
        public Vector3 Position { get; private set; }

        public Chunk(Vector3 position)
        {
            Position = position;
        }

        public void Render()
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color3(Color.Red);
            GL.Vertex3(Position.X, Position.Y, Position.Z);
            GL.Vertex3(Position.X + 16, Position.Y, Position.Z);
            GL.Vertex3(Position.X + 16, Position.Y, Position.Z + 16);
            GL.Vertex3(Position.X, Position.Y, Position.Z + 16);
            GL.End();
        }
    }
}
