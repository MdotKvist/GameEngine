using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenGLHelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            // Opretter et vindue med en bestemt størrelse
            var vindue = new GameWindow(1000, 800);
            vindue.Title = "BetterCraft ikke en minecraft klon. Voksen babys hår.";

            // Beregner aspect ratio for vinduet
            float aspectRatio = vindue.Width / (float)vindue.Height;

            // Initialiserer kameraets position og retning
            Vector3 kameraPosition = new Vector3(0, 1, 5);
            Vector2 sidsteMusPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            float yaw = -90.0f;
            float pitch = 0.0f;

            // Event handler for indlæsning af vinduet
            vindue.Load += (sender, e) =>
            {
                // Sætter baggrundsfarven og aktiverer dybdetest
                GL.ClearColor(Color4.CornflowerBlue);
                GL.Enable(EnableCap.DepthTest);

                // Indstiller projektionsmatrixen
                GL.MatrixMode(MatrixMode.Projection);
                Matrix4 projektion = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio, 0.1f, 100.0f);
                GL.LoadMatrix(ref projektion);

                // Indstiller modelview matrixen
                GL.MatrixMode(MatrixMode.Modelview);
                Matrix4 view = Matrix4.LookAt(kameraPosition, kameraPosition + new Vector3(0, 0, -1), Vector3.UnitY);
                GL.LoadMatrix(ref view);
            };

            // Event handler for rendering af hvert frame
            vindue.RenderFrame += (sender, e) =>
            {
                // Rydder farve- og dybdemasken
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                // Opdaterer kameraets retning
                Vector3 front;
                front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
                front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
                front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
                Vector3 sePå = Vector3.Normalize(front);

                Matrix4 view = Matrix4.LookAt(kameraPosition, kameraPosition + sePå, Vector3.UnitY);
                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadMatrix(ref view);

                // Tegner gulvet og spilleren
                TegnGulv();
                TegnSpiller();

                vindue.SwapBuffers();
            };

            // Event handler for opdatering af hvert frame
            vindue.UpdateFrame += (sender, e) =>
            {
                // Opdaterer kameraets position baseret på tastetryk
                var tastaturState = Keyboard.GetState();
                var museState = Mouse.GetState();
                var musePos = new Vector2(museState.X, museState.Y);
                Vector2 delta = sidsteMusPos - musePos;
                sidsteMusPos = musePos;

                yaw += delta.X * 0.1f;
                pitch -= delta.Y * 0.1f;

                pitch = Math.Clamp(pitch, -90.0f, 90.0f);

                Vector3 front;
                front.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
                front.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
                front.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));

                float bevægelsesHastighed = 0.1f;
                if (tastaturState.IsKeyDown(Key.W))
                    kameraPosition += bevægelsesHastighed * Vector3.Normalize(new Vector3(front.X, 0, front.Z));
                if (tastaturState.IsKeyDown(Key.S))
                    kameraPosition -= bevægelsesHastighed * Vector3.Normalize(new Vector3(front.X, 0, front.Z));
                if (tastaturState.IsKeyDown(Key.A))
                    kameraPosition -= bevægelsesHastighed * Vector3.Normalize(Vector3.Cross(new Vector3(front.X, 0, front.Z), Vector3.UnitY));
                if (tastaturState.IsKeyDown(Key.D))
                    kameraPosition += bevægelsesHastighed * Vector3.Normalize(Vector3.Cross(new Vector3(front.X, 0, front.Z), Vector3.UnitY));
            };

            // Event handler for musebevægelse
            vindue.MouseMove += (sender, e) =>
            {
                // Opdaterer kameraets retning baseret på musens bevægelse
                if (Mouse.GetState().IsButtonDown(MouseButton.Left))
                {
                    var museState = Mouse.GetState();
                    var musePos = new Vector2(museState.X, museState.Y);
                    Vector2 delta = sidsteMusPos - musePos;
                    sidsteMusPos = musePos;

                    yaw += delta.X * 0.1f;
                    pitch -= delta.Y * 0.1f;

                    pitch = Math.Clamp(pitch, -90.0f, 90.0f);
                }
            };

            // Starter vinduets løkke
            vindue.Run();
        }

        static void TegnGulv()
        {
            // Størrelsen af gulvet
            float størrelse = 4.0f;
            float højde = -5.0f;

            // Farven på gulvet
            Color4 topFarve = new Color4(0.0f, 0.6f, 0.0f, 1.0f); // Dæmpet grøn farve
            Color4 sideFarve = new Color4(0.4f, 0.2f, 0.0f, 1.0f); // Brun farve

            // Tegn gulvet som en solid blok
            GL.Begin(PrimitiveType.Quads);

            // Tegn bunden og toppen af gulvet
            GL.Color4(topFarve); // Grøn farve for toppen
            for (float y = højde; y <= 0.0f; y += -højde)
            {
                GL.Vertex3(-størrelse, y, -størrelse);
                GL.Vertex3(størrelse, y, -størrelse);
                GL.Vertex3(størrelse, y, størrelse);
                GL.Vertex3(-størrelse, y, størrelse);
            }

            // Tegn siderne af gulvet
            GL.Color4(sideFarve); // Brun farve for siderne
            for (float x = -størrelse; x <= størrelse; x += størrelse * 2)
            {
                GL.Vertex3(x, højde, -størrelse);
                GL.Vertex3(x, 0.0f, -størrelse);
                GL.Vertex3(x, 0.0f, størrelse);
                GL.Vertex3(x, højde, størrelse);
            }

            // Tegn de to manglende sider
            for (float z = -størrelse; z <= størrelse; z += størrelse * 2)
            {
                GL.Vertex3(-størrelse, højde, z);
                GL.Vertex3(-størrelse, 0.0f, z);
                GL.Vertex3(størrelse, 0.0f, z);
                GL.Vertex3(størrelse, højde, z);
            }

            GL.End();
        }



        // Tegn spilleren
        static void TegnSpiller()
        {
            GL.Begin(PrimitiveType.Quads);
            GL.Color4(0.5f, 0.0f, 0.0f, 1.0f);

            GL.Vertex3(-0.2f, 1.0f, -0.2f);
            GL.Vertex3(0.2f, 1.0f, -0.2f);
            GL.Vertex3(0.2f, 1.0f, 0.2f);
            GL.Vertex3(-0.2f, 1.0f, 0.2f);

            GL.End();
        }
    }
}
