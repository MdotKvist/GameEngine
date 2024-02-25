using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Text;

namespace OpenGLMinecraft
{
    public class Chunk
    {
        public enum BlockType { Air, Dirt, Grass, Stone }

        public class Voxel
        {
            public BlockType Type { get; set; }
            public Voxel(BlockType type) { Type = type; }
        }

        public static readonly int ChunkSize = 16;
        public Voxel[,,] blocks = new Voxel[ChunkSize, ChunkSize, ChunkSize];

        public Chunk()
        {
            InitializeBlocks();
        }

        public void InitializeBlocks()
        {
            // Initialiser dine blokke her. Dette kunne være baseret på en eller anden form for kortgenerering eller hentet fra en fil.
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        // Eksempel: Sæt alle blokke til at være af typen Air som standard
                        blocks[x, y, z] = new Voxel(BlockType.Air);
                    }
                }
            }
            // Tilføj yderligere logik for at definere bloktyper baseret på deres position, f.eks. for at skabe et simpelt landskab
        }

        public void Draw()
        {
            // Tegn alle blokke i denne chunk. Dette vil involvere at iterere over hver blok og tegne den,
            // hvis den er af en type, der kræver det (ikke luft), og potentielt kun hvis den er synlig.
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        Voxel voxel = blocks[x, y, z];
                        if (voxel.Type != BlockType.Air)
                        {
                            // Antag her, at du har en metode til at tegne en enkelt blok givet dens type og position.
                            // Du skal muligvis tilpasse dette til din specifikke rendering logik.
                            DrawVoxel(x, y, z, voxel.Type);
                        }
                    }
                }
            }
        }

        public void DrawVoxel(int x, int y, int z, BlockType type)
        {
            // Sæt farven baseret på bloktypen
            Color color = GetColorForVoxelType(type);
            GL.Color3(color);

            // Beregn den faktiske position baseret på blokkens indeks og blokstørrelsen
            float blockSize = 1.0f; // Antag at hver blok er 1 enhed stor
            float posX = x * blockSize;
            float posY = y * blockSize;
            float posZ = z * blockSize;

            // Start tegning af en kube ved at tegne dens overflader
            GL.Begin(PrimitiveType.Quads);

            // Top
            GL.Vertex3(posX, posY + blockSize, posZ);
            GL.Vertex3(posX + blockSize, posY + blockSize, posZ);
            GL.Vertex3(posX + blockSize, posY + blockSize, posZ + blockSize);
            GL.Vertex3(posX, posY + blockSize, posZ + blockSize);

            // Bund
            GL.Vertex3(posX, posY, posZ);
            GL.Vertex3(posX + blockSize, posY, posZ);
            GL.Vertex3(posX + blockSize, posY, posZ + blockSize);
            GL.Vertex3(posX, posY, posZ + blockSize);

            // Foran
            GL.Vertex3(posX, posY, posZ);
            GL.Vertex3(posX + blockSize, posY, posZ);
            GL.Vertex3(posX + blockSize, posY + blockSize, posZ);
            GL.Vertex3(posX, posY + blockSize, posZ);

            // Bag
            GL.Vertex3(posX, posY, posZ + blockSize);
            GL.Vertex3(posX + blockSize, posY, posZ + blockSize);
            GL.Vertex3(posX + blockSize, posY + blockSize, posZ + blockSize);
            GL.Vertex3(posX, posY + blockSize, posZ + blockSize);

            // Venstre
            GL.Vertex3(posX, posY, posZ);
            GL.Vertex3(posX, posY, posZ + blockSize);
            GL.Vertex3(posX, posY + blockSize, posZ + blockSize);
            GL.Vertex3(posX, posY + blockSize, posZ);

            // Højre
            GL.Vertex3(posX + blockSize, posY, posZ);
            GL.Vertex3(posX + blockSize, posY, posZ + blockSize);
            GL.Vertex3(posX + blockSize, posY + blockSize, posZ + blockSize);
            GL.Vertex3(posX + blockSize, posY + blockSize, posZ);

            GL.End();
        }

        public Color GetColorForVoxelType(BlockType type)
        {
            switch (type)
            {
                case BlockType.Dirt:
                    return Color.Brown;
                case BlockType.Grass:
                    return Color.Green;
                case BlockType.Stone:
                    return Color.Gray;
                default:
                    return Color.White;
            }
        }
    }
}
