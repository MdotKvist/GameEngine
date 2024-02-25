using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using VoxelType = OpenGLMinecraft.Voxel; // Brug denne aliaserklæring

namespace OpenGLMinecraft
{
    public class Renderer
    {
        public static readonly int WorldSizeX = 20;
        public static readonly int WorldSizeY = 20;
        public static readonly int WorldSizeZ = 20;
        public static readonly int ChunkSize = 16;
        public static readonly float BlockSize = 1.0f;
        public static readonly int Seed = 1234;

        public class RenderVoxel // Ændret navn på klassen for at undgå konflikt
        {
            public BlockType Type { get; set; }
            public RenderVoxel(BlockType type) { Type = type; }
        }

        static VoxelType[,,] worldBlocks = new VoxelType[WorldSizeX * ChunkSize, WorldSizeY * ChunkSize, WorldSizeZ * ChunkSize];
        static Vector3 cameraPosition = new Vector3(0, 5, 20);

        public static void DrawBlock(VoxelType voxel, int x, int y, int z)
        {
            // Implement block drawing here
            GL.PushMatrix();
            GL.Translate(x, y, z);

            switch (voxel.Type)
            {
                case BlockType.Air:
                    return;
                case BlockType.Dirt:
                    GL.Color3(0.55f, 0.27f, 0.07f);
                    break;
                case BlockType.Grass:
                    GL.Color3(0.0f, 0.5f, 0.0f);
                    break;
                case BlockType.Stone:
                    GL.Color3(0.5f, 0.5f, 0.5f);
                    break;
                default:
                    GL.Color3(1.0f, 1.0f, 1.0f);
                    break;
            }

            // Tegn en kubisk blok
            GL.Begin(PrimitiveType.Quads);
            // Front face
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(0, 1, 0);
            // Back face
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(1, 0, 1);
            GL.Vertex3(1, 1, 1);
            GL.Vertex3(0, 1, 1);
            // Left face
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0, 0, 1);
            GL.Vertex3(0, 1, 1);
            GL.Vertex3(0, 1, 0);
            // Right face
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 0, 1);
            GL.Vertex3(1, 1, 1);
            GL.Vertex3(1, 1, 0);
            // Top face
            GL.Vertex3(0, 1, 0);
            GL.Vertex3(1, 1, 0);
            GL.Vertex3(1, 1, 1);
            GL.Vertex3(0, 1, 1);
            // Bottom face
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(1, 0, 0);
            GL.Vertex3(1, 0, 1);
            GL.Vertex3(0, 0, 1);
            GL.End();
        }

        public static void DrawChunk(int cx, int cy, int cz)
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
                        if (voxel != null && voxel.Type != BlockType.Air)
                        {
                            DrawBlock(voxel, globalX, globalY, globalZ);
                        }
                    }
                }
            }
        }

        public void DrawWorld(World world)
        {
            // Draw the world here
            for (int x = 0; x < WorldSizeX; x++)
            {
                for (int y = 0; y < WorldSizeY; y++)
                {
                    for (int z = 0; z < WorldSizeZ; z++)
                    {
                        VoxelType voxel = world.GetVoxel(x, y, z);
                        if (voxel != null)
                        {
                            DrawBlock(voxel, x, y, z);
                        }
                    }
                }
            }
        }
    }
}








//// Tegn en kubisk blok
//GL.Begin(PrimitiveType.Quads);
//        // Front face
//        GL.Vertex3(0, 0, 0);
//        GL.Vertex3(1, 0, 0);
//        GL.Vertex3(1, 1, 0);
//        GL.Vertex3(0, 1, 0);
//        // Back face
//        GL.Vertex3(0, 0, 1);
//        GL.Vertex3(1, 0, 1);
//        GL.Vertex3(1, 1, 1);
//        GL.Vertex3(0, 1, 1);
//        // Left face
//        GL.Vertex3(0, 0, 0);
//        GL.Vertex3(0, 0, 1);
//        GL.Vertex3(0, 1, 1);
//        GL.Vertex3(0, 1, 0);
//        // Right face
//        GL.Vertex3(1, 0, 0);
//        GL.Vertex3(1, 0, 1);
//        GL.Vertex3(1, 1, 1);
//        GL.Vertex3(1, 1, 0);
//        // Top face
//        GL.Vertex3(0, 1, 0);
//        GL.Vertex3(1, 1, 0);
//        GL.Vertex3(1, 1, 1);
//        GL.Vertex3(0, 1, 1);
//        // Bottom face
//        GL.Vertex3(0, 0, 0);
//        GL.Vertex3(1, 0, 0);
//        GL.Vertex3(1, 0, 1);
//        GL.Vertex3(0, 0, 1);
//        GL.End();
//    }