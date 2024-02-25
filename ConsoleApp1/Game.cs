using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGLMinecraft
{
    class Game : GameWindow
    {
        // Definér kameraets position
        Vector3 cameraPosition = new Vector3(0, 0, 0);
        bool isPaused = false;

        public Game(int width, int height, string title)
            : base(width, height, GraphicsMode.Default, title) { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.ClearColor(Color4.SkyBlue);
            GL.Enable(EnableCap.DepthTest);

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Width / (float)Height, 0.1f, 1000.0f);
            GL.LoadMatrix(ref projection);

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 view = Matrix4.LookAt(cameraPosition, cameraPosition + new Vector3(0, 0, -1), Vector3.UnitY);
            GL.LoadMatrix(ref view);

            // Load texture and other initialization here
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var keyboardState = Keyboard.GetState();

            // Toggle pausing when Escape is pressed
            if (keyboardState.IsKeyDown(Key.Escape))
            {
                TogglePause();
            }

            // Update game logic when not paused
            if (!isPaused)
            {
                UpdateGameLogic();
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render game content here
            RenderGameContent();

            SwapBuffers();
        }

        void TogglePause()
        {
            isPaused = !isPaused;
            if (isPaused)
            {
                // Perform actions when the game is paused
                Console.WriteLine("Game paused.");
            }
            else
            {
                // Perform actions when the game is resumed
                Console.WriteLine("Game resumed.");
            }
        }

        void UpdateGameLogic()
        {
            // Update game logic here
        }

        void RenderGameContent()
        {
            // Render game content here
        }
    }
}
