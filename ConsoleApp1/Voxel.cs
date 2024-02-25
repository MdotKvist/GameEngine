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

    public enum BlockType { Air, Dirt, Grass, Stone }
    public class Voxel
	{
		public BlockType Type { get; set; }

		public Voxel(BlockType type)
		{
			Type = type;
		}
	}

	
}