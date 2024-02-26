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
    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                Console.WriteLine("Game run");
                game.Run();
            }
        }
    }
}
//static void Main(string[] args)
//{
//    using (var game = new GameWindow(800, 600, GraphicsMode.Default, "OpenGL Minecraft"))
//    {
//        game.Load += (sender, e) =>
//        {
//            GL.ClearColor(Color4.SkyBlue);
//            GL.Enable(EnableCap.DepthTest);
//            // Additional initialization code can be added here
//        };

//        game.Run(60.0);
//    }
//}