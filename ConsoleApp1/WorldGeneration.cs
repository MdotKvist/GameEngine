using System;

namespace OpenGLMinecraft
{
    public class WorldGeneration
    {
        private const int ChunkSize = 10;
        private const int WorldSizeX = 10;
        private const int WorldSizeY = 10;
        private const int WorldSizeZ = 10;
        private const int Seed = 1234;

        public enum BlockType { Air, Dirt, Grass, Stone }

        public readonly Voxel[,,] WorldBlocks = new Voxel[WorldSizeX * ChunkSize, WorldSizeY * ChunkSize, WorldSizeZ * ChunkSize];


        public void GenerateWorld()
        {
            Console.WriteLine("Før random seed");
            Random random = new Random(Seed);

            for (int cx = 0; cx < WorldSizeX; cx++)
            {
                Console.WriteLine("første for");
                for (int cz = 0; cz < WorldSizeZ; cz++)
                {
                    Console.WriteLine("Andet for");
                    for (int cy = 0; cy < WorldSizeY; cy++)
                    {
                        Console.WriteLine("tredje for");
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
                                            WorldBlocks[globalX, globalY, globalZ] = new Voxel(BlockType.Stone);
                                        else if (globalY == noiseValue * WorldSizeY - 1)
                                            WorldBlocks[globalX, globalY, globalZ] = new Voxel(BlockType.Grass);
                                        else
                                            WorldBlocks[globalX, globalY, globalZ] = new Voxel(BlockType.Dirt);
                                    }
                                    else
                                    {
                                        WorldBlocks[globalX, globalY, globalZ] = new Voxel(BlockType.Air);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private double PerlinNoise(double x, double y, double z, int octaves, Random random)
        {
            PerlinNoise terrain = new PerlinNoise(Seed);
            double noiseValue = terrain.Noise(x, y, z);
            return noiseValue;
        }

        public class Voxel
        {
            public BlockType Type { get; set; }

            public Voxel(BlockType type)
            {
                Type = type;
            }
        }
    }
}
