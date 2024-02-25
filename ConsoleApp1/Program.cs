using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Threading;

namespace OpenGLMinecraft
{
    class Program
    {
        static readonly int WorldSizeX = 20;
        static readonly int WorldSizeY = 20;
        static readonly int WorldSizeZ = 20;
        static readonly int ChunkSize = 16;
        static readonly float BlockSize = 1.0f;
        static readonly int Seed = 1234; // Ændr dette for forskellige terræner

        static Voxel[,,] worldBlocks = new Voxel[WorldSizeX * ChunkSize, WorldSizeY * ChunkSize, WorldSizeZ * ChunkSize];
        static Vector3 cameraPosition = new Vector3(0, 5, 20);
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

        class Voxel
        {
            public BlockType Type { get; set; }

            public Voxel(BlockType type)
            {
                Type = type;
            }
        }

        static void Main(string[] args)
        {
            Thread windowThread = new Thread(() =>
            {
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

                    DrawVisibleChunks();

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

                Vector2 lastMousePos = Vector2.Zero;

                window.MouseMove += (sender, e) =>
                {
                    var mouseState = Mouse.GetState();
                    var mousePos = new Vector2(mouseState.X, mouseState.Y);

                    // Check if the mouse has moved
                    if (lastMousePos != Vector2.Zero && lastMousePos != mousePos)
                    {
                        var deltaX = (mousePos.X - lastMousePos.X) * 0.1f; // Juster følsomheden her
                        var deltaY = (mousePos.Y - lastMousePos.Y) * 0.1f; // Juster følsomheden her

                        yaw += deltaX;
                        pitch -= deltaY;

                        Mouse.SetPosition(window.X + window.Width / 2, window.Y + window.Height / 2);
                    }

                    lastMousePos = mousePos;
                };

                window.Run();
            });

            windowThread.Start();

            GenerateWorld();
        }

        static void GenerateWorld()
        {
            Random random = new Random(Seed);

            for (int cx = 0; cx < WorldSizeX; cx++)
            {
                for (int cz = 0; cz < WorldSizeZ; cz++)
                {
                    for (int cy = 0; cy < WorldSizeY; cy++)
                    {
                        for (int x = 0; x < ChunkSize; x++)
                        {
                            for (int z = 0; z < ChunkSize; z++)
                            {
                                for (int y = 0; y < ChunkSize; y++)
                                {
                                    int globalX = cx * ChunkSize + x;
                                    int globalY = cy * ChunkSize + y;
                                    int globalZ = cz * ChunkSize + z;

                                    double noiseValue = PerlinNoise(globalX * 0.1, globalY * 0.1, globalZ * 0.1, 4, random);
                                    if (globalY < noiseValue * WorldSizeY)
                                    {
                                        if (globalY == 0)
                                            worldBlocks[globalX, globalY, globalZ] = new Voxel(BlockType.Stone);
                                        else if (globalY == noiseValue * WorldSizeY - 1)
                                            worldBlocks[globalX, globalY, globalZ] = new Voxel(BlockType.Grass);
                                        else
                                            worldBlocks[globalX, globalY, globalZ] = new Voxel(BlockType.Dirt);
                                    }
                                    else
                                    {
                                        worldBlocks[globalX, globalY, globalZ] = new Voxel(BlockType.Air);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static double PerlinNoise(double x, double y, double z, int octaves, Random random)
        {
            double total = 0;
            double frequency = 1;
            double amplitude = 1;
            double maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
            for (int i = 0; i < octaves; i++)
            {
                total += Noise(x * frequency, y * frequency, z * frequency, random) * amplitude;

                maxValue += amplitude;

                amplitude *= 0.5;
                frequency *= 2;
            }

            return total / maxValue;
        }

        static double Noise(double x, double y, double z, Random random)
        {
            int X = (int)Math.Floor(x) & 255;                  // FIND UNIT CUBE THAT
            int Y = (int)Math.Floor(y) & 255;                  // CONTAINS POINT.
            int Z = (int)Math.Floor(z) & 255;
            x -= Math.Floor(x);                                // FIND RELATIVE X,Y,Z
            y -= Math.Floor(y);                                // OF POINT IN CUBE.
            z -= Math.Floor(z);
            double u = Fade(x);                                // COMPUTE FADE CURVES
            double v = Fade(y);                                // FOR EACH OF X,Y,Z.
            double w = Fade(z);
            int A = p[X] + Y, AA = p[A] + Z, AB = p[A + 1] + Z;      // HASH COORDINATES OF
            int B = p[X + 1] + Y, BA = p[B] + Z, BB = p[B + 1] + Z;      // THE 8 CUBE CORNERS,

            return Lerp(w, Lerp(v, Lerp(u, Grad(p[AA], x, y, z),
                                           Grad(p[BA], x - 1, y, z)),
                                   Lerp(u, Grad(p[AB], x, y - 1, z),
                                           Grad(p[BB], x - 1, y - 1, z))),
                           Lerp(v, Lerp(u, Grad(p[AA + 1], x, y, z - 1),
                                           Grad(p[BA + 1], x - 1, y, z - 1)),
                                   Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1),
                                           Grad(p[BB + 1], x - 1, y - 1, z - 1))));
        }

        static double Fade(double t) { return t * t * t * (t * (t * 6 - 15) + 10); }
        static double Lerp(double t, double a, double b) { return a + t * (b - a); }
        static double Grad(int hash, double x, double y, double z)
        {
            int h = hash & 15;                      // CONVERT LO 4 BITS OF HASH CODE
            double u = h < 8 ? x : y,                 // INTO 12 GRADIENT DIRECTIONS.
                   v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        static readonly int[] p = new int[512];
        static Program()
        {
            int[] permutation = { 151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9, 129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180 };
            for (int i = 0; i < 256; i++)
                p[256 + i] = p[i] = permutation[i];
        }

        static void DrawVisibleChunks()
        {
            int chunkX = (int)Math.Floor(cameraPosition.X / ChunkSize);
            int chunkY = (int)Math.Floor(cameraPosition.Y / ChunkSize);
            int chunkZ = (int)Math.Floor(cameraPosition.Z / ChunkSize);

            int renderDistance = 5; // Juster dette for hvor mange chunks du vil have synlige omkring spilleren

            for (int cx = chunkX - renderDistance; cx <= chunkX + renderDistance; cx++)
            {
                for (int cy = chunkY - renderDistance; cy <= chunkY + renderDistance; cy++)
                {
                    for (int cz = chunkZ - renderDistance; cz <= chunkZ + renderDistance; cz++)
                    {
                        if (cx >= 0 && cy >= 0 && cz >= 0 && cx < WorldSizeX && cy < WorldSizeY && cz < WorldSizeZ)
                        {
                            DrawChunk(cx, cy, cz);
                        }
                    }
                }
            }
        }

        static void DrawChunk(int cx, int cy, int cz)
        {
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int z = 0; z < ChunkSize; z++)
                {
                    for (int y = 0; y < ChunkSize; y++)
                    {
                        int globalX = cx * ChunkSize + x;
                        int globalY = cy * ChunkSize + y;
                        int globalZ = cz * ChunkSize + z;

                        var voxel = worldBlocks[globalX, globalY, globalZ];
                        if (voxel != null)
                        {
                            switch (voxel.Type)
                            {
                                case BlockType.Dirt:
                                    DrawBlock(globalX, globalY, globalZ, Color.Brown);
                                    break;
                                case BlockType.Grass:
                                    DrawBlock(globalX, globalY, globalZ, Color.Green);
                                    break;
                                case BlockType.Stone:
                                    DrawBlock(globalX, globalY, globalZ, Color.Gray);
                                    break;
                            }
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
