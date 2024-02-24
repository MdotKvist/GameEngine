using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGLMinecraft
{
    class Program
    {
        static readonly int WorldSize = 20;
        static readonly float BlockSize = 1.0f;

        static BlockType[,,] worldBlocks = new BlockType[WorldSize, WorldSize, WorldSize];
        static Vector3 cameraPosition = new Vector3(0, 5, 20);
         Vector2 sidsteMusPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        static float yaw = -90.0f;
        static float pitch = 0.0f;

        static GameWindow window;

        enum BlockType
        {
            Air,
            Dirt,
            Grass,
            Stone
        }

        static void Main(string[] args)
        {
            GenerateWorld();

            window = new GameWindow(800, 600);
            window.Title = "Minecraft Classic";

            window.Load += (sender, e) =>
            {
                GL.ClearColor(Color4.SkyBlue);
                GL.Enable(EnableCap.DepthTest);

                GL.MatrixMode(MatrixMode.Projection);
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, window.Width / (float)window.Height, 0.1f, 1000.0f);
                GL.LoadMatrix(ref projection);

                GL.MatrixMode(MatrixMode.Modelview);
                Matrix4 view = Matrix4.LookAt(cameraPosition, cameraPosition + new Vector3(0, 0, -1), Vector3.UnitY);
                GL.LoadMatrix(ref view);
            };

            window.RenderFrame += (sender, e) =>
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                Vector3 front;
                front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
                front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
                front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
                Vector3 lookAt = Vector3.Normalize(front);

                Matrix4 view = Matrix4.LookAt(cameraPosition, cameraPosition + lookAt, Vector3.UnitY);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref view);

                DrawWorld();

                window.SwapBuffers();
            };

            window.UpdateFrame += (sender, e) =>
            {
                var keyboardState = Keyboard.GetState();

                float cameraSpeed = 0.5f;
                if (keyboardState.IsKeyDown(Key.W))
                    cameraPosition += cameraSpeed * Vector3.Normalize(new Vector3((float)Math.Cos(MathHelper.DegreesToRadians(yaw)), 0, (float)Math.Sin(MathHelper.DegreesToRadians(yaw))));
                if (keyboardState.IsKeyDown(Key.S))
                    cameraPosition -= cameraSpeed * Vector3.Normalize(new Vector3((float)Math.Cos(MathHelper.DegreesToRadians(yaw)), 0, (float)Math.Sin(MathHelper.DegreesToRadians(yaw))));
                if (keyboardState.IsKeyDown(Key.A))
                    cameraPosition -= cameraSpeed * Vector3.Normalize(Vector3.Cross(new Vector3((float)Math.Cos(MathHelper.DegreesToRadians(yaw)), 0, (float)Math.Sin(MathHelper.DegreesToRadians(yaw))), Vector3.UnitY));
                if (keyboardState.IsKeyDown(Key.D))
                    cameraPosition += cameraSpeed * Vector3.Normalize(Vector3.Cross(new Vector3((float)Math.Cos(MathHelper.DegreesToRadians(yaw)), 0, (float)Math.Sin(MathHelper.DegreesToRadians(yaw))), Vector3.UnitY));
            };

            Vector2 sidsteMusPos = Vector2.Zero;

            window.MouseMove += (sender, e) =>
            {
                var mouseState = Mouse.GetState();
                var musePos = new Vector2(mouseState.X, mouseState.Y);

                // Kontroller om musen er flyttet
                if (sidsteMusPos != Vector2.Zero && sidsteMusPos != musePos)
                {
                    var deltaX = (musePos.X - sidsteMusPos.X) * 0.1f; // Juster følsomheden her
                    var deltaY = (musePos.Y - sidsteMusPos.Y) * 0.1f; // Juster følsomheden her

                    yaw += deltaX;
                    pitch -= deltaY;

                    Mouse.SetPosition(window.X + window.Width / 2, window.Y + window.Height / 2);
                }

                sidsteMusPos = musePos;
            };


            window.Run();
        }

        static void GenerateWorld()
        {
            Random random = new Random();

            for (int x = 0; x < WorldSize; x++)
            {
                for (int z = 0; z < WorldSize; z++)
                {
                    int height = random.Next(1, WorldSize); // Random height for terrain
                    for (int y = 0; y < height; y++)
                    {
                        if (y == 0)
                            worldBlocks[x, y, z] = BlockType.Stone;
                        else if (y == height - 1)
                            worldBlocks[x, y, z] = BlockType.Grass;
                        else
                            worldBlocks[x, y, z] = BlockType.Dirt;
                    }
                }
            }
        }

        static void DrawWorld()
        {
            for (int x = 0; x < WorldSize; x++)
            {
                for (int z = 0; z < WorldSize; z++)
                {
                    for (int y = 0; y < WorldSize; y++)
                    {
                        switch (worldBlocks[x, y, z])
                        {
                            case BlockType.Dirt:
                                DrawBlock(x, y, z, Color.Brown);
                                break;
                            case BlockType.Grass:
                                DrawBlock(x, y, z, Color.Green);
                                break;
                            case BlockType.Stone:
                                DrawBlock(x, y, z, Color.Gray);
                                break;
                        }
                    }
                }
            }
        }

        static void DrawBlock(int x, int y, int z, Color color)
        {
            GL.Color3(color);
            GL.Begin(PrimitiveType.Quads);
            // Front face
            GL.Vertex3(x, y, z);
            GL.Vertex3(x + 1, y, z);
            GL.Vertex3(x + 1, y + 1, z);
            GL.Vertex3(x, y + 1, z);

            // Back face
            GL.Vertex3(x, y, z + 1);
            GL.Vertex3(x + 1, y, z + 1);
            GL.Vertex3(x + 1, y + 1, z + 1);
            GL.Vertex3(x, y + 1, z + 1);

            // Top face
            GL.Vertex3(x, y + 1, z);
            GL.Vertex3(x + 1, y + 1, z);
            GL.Vertex3(x + 1, y + 1, z + 1);
            GL.Vertex3(x, y + 1, z + 1);

            // Bottom face
            GL.Vertex3(x, y, z);
            GL.Vertex3(x + 1, y, z);
            GL.Vertex3(x + 1, y, z + 1);
            GL.Vertex3(x, y, z + 1);

            // Left face
            GL.Vertex3(x, y, z);
            GL.Vertex3(x, y, z + 1);
            GL.Vertex3(x, y + 1, z + 1);
            GL.Vertex3(x, y + 1, z);

            // Right face
            GL.Vertex3(x + 1, y, z);
            GL.Vertex3(x + 1, y, z + 1);
            GL.Vertex3(x + 1, y + 1, z + 1);
            GL.Vertex3(x + 1, y + 1, z);
            GL.End();
        }
    }
}
