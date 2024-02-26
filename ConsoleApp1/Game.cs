using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenGLMinecraft
{
    public class Game : GameWindow // Ændret navnet fra 'Game' til 'OpenGLMinecraft'
    {
        private const int BlockSize = 1; // Ændret 'float' til 'int' fordi blokstørrelsen er i hele enheder
        private const int RenderDistance = 2 * BlockSize; // Flyttet RenderDistance definition før WorldSize
        private const int WorldSizeX = RenderDistance * 2;
        private const int WorldSizeY = 8; // Ændret WorldSizeY til 8
        private const int WorldSizeZ = RenderDistance * 2;
        private const int ChunkSize = 16; // Flyttet ChunkSize definition før brugen i UpdateLoadedChunks()

        private WorldGeneration worldGeneration;
        private Vector3 playerPosition;
        private Dictionary<Vector3, Chunk> loadedChunks;

        public Game() : base(1280, 780, GraphicsMode.Default, "Minecraft Classic")
        {
            Console.WriteLine("Vindue åbnet");
            worldGeneration = new WorldGeneration();
            Console.WriteLine("World Generation objekt");

            playerPosition = new Vector3(0, 0, 0);
            loadedChunks = new Dictionary<Vector3, Chunk>();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Console.WriteLine("On load");

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            Console.WriteLine("CullFace");

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 100.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            worldGeneration.GenerateWorld();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            playerPosition += new Vector3(0.1f, 0, 0);
            UpdateLoadedChunks();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            Console.WriteLine("Render frame");

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Console.WriteLine("GL CLEAR");

            Matrix4 modelview = Matrix4.LookAt(playerPosition + new Vector3(0, 0, RenderDistance * 2), playerPosition, Vector3.UnitY);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);

            RenderScene();
            Console.WriteLine("Rendering scene");
            SwapBuffers();
            Console.WriteLine("SwapBuffers");
        }

        private void RenderScene()
        {
            Console.WriteLine("Rendering scene");

            GL.ClearColor(Color4.SkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(-WorldSizeX / 2.0f, -WorldSizeY / 2.0f, -WorldSizeZ / 2.0f);

            Console.WriteLine("Rendering loaded chunks");
            foreach (var chunk in loadedChunks.Values)
            {
                Console.WriteLine($"Rendering chunk at position {chunk.Position}");
                chunk.Render();
            }

            GL.Flush();
        }

        private void UpdateLoadedChunks()
        {
            int playerChunkX = (int)Math.Floor(playerPosition.X / ChunkSize);
            int playerChunkY = (int)Math.Floor(playerPosition.Y / ChunkSize);
            int playerChunkZ = (int)Math.Floor(playerPosition.Z / ChunkSize);

            int renderDistanceChunks = RenderDistance / ChunkSize;

            loadedChunks.Clear();
            for (int x = playerChunkX - renderDistanceChunks; x <= playerChunkX + renderDistanceChunks; x++)
            {
                for (int y = playerChunkY - renderDistanceChunks; y <= playerChunkY + renderDistanceChunks; y++)
                {
                    for (int z = playerChunkZ - renderDistanceChunks; z <= playerChunkZ + renderDistanceChunks; z++)
                    {
                        Vector3 chunkPosition = new Vector3(x * ChunkSize, y * ChunkSize, z * ChunkSize);
                        float distanceToChunk = Vector3.Distance(playerPosition, chunkPosition + new Vector3(ChunkSize / 2));

                        if (distanceToChunk <= RenderDistance)
                        {
                            if (!loadedChunks.ContainsKey(chunkPosition))
                            {
                                loadedChunks.Add(chunkPosition, new Chunk(chunkPosition));
                                Console.WriteLine($"Chunk at position {chunkPosition} loaded. Distance to player: {distanceToChunk}");
                            }
                            else
                            {
                                Console.WriteLine($"Chunk at position {chunkPosition} already loaded. Distance to player: {distanceToChunk}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Chunk at position {chunkPosition} outside render distance. Distance to player: {distanceToChunk}");
                        }
                    }
                }
            }
        }
    }
}
