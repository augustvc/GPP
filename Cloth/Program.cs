using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Cloth
{
    class Game : GameWindow
    {
        static void Main(string[] args)
        {
            using (Game game = new Game(1400, 1000, "Clothy cloth cloth"))
            {
                game.Run(100.0, 60.0);
            }
        }

        static int VBO;
        static int VAO;

        static Shader shader;
        static Texture clothTexture;

        protected override void OnLoad(EventArgs e)
        {
            shader = new Shader("shader.vert", "shader.frag");
            shader.Use();

            clothTexture = new Texture("Green.png");
            clothTexture.Use();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.MirroredRepeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.MirroredRepeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindVertexArray(VAO);

            int aPosLocation = shader.GetAttribLocation("aPosition");
            int aTexCoordLocation = shader.GetAttribLocation("aTexCoord");
            int aNormalLocation = shader.GetAttribLocation("aNormal");

            GL.VertexAttribPointer(aPosLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            GL.VertexAttribPointer(aTexCoordLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));
            GL.VertexAttribPointer(aNormalLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 5 * sizeof(float));

            GL.EnableVertexAttribArray(aPosLocation);
            GL.EnableVertexAttribArray(aTexCoordLocation);
            GL.EnableVertexAttribArray(aNormalLocation);

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(75.0f),
                (float)Width / (float)Height, 0.03f, 5000f);

            shader.SetMatrix4("view", Camera.GetViewMatrix());
            shader.SetMatrix4("projection", projection);

            float[] vertices = Cloth.exportVertices();

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            GL.DrawArrays(PrimitiveType.Triangles, 0, vertices.Length / 8);

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }

        bool started = false;

        int cd = 0;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();
            if(input.IsKeyDown(Key.Escape))
            {
                Exit();
            }
            Camera.move(input);

            Vector3 blowDirection = new Vector3(0, 0, 0);
            if(input.IsKeyDown(Key.B))
            {
                blowDirection = Camera.GetFront();
            }

            if(input.IsKeyDown(Key.Enter) && cd < 1)
            {
                started = !started;
                cd = 100;
            }
            cd--;
            if(input.IsKeyDown(Key.G) && cd < 1)
            {
                Cloth.Integrate(0.01f, blowDirection);
                cd = 100;
            } else if(started)
                Cloth.Integrate(0.01f, blowDirection);
            base.OnUpdateFrame(e);
        }

        bool wasFocused = false;

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            if (Focused) // check to see if the window is focused  
            {
                if (wasFocused)
                {
                    MouseState mouseState = Mouse.GetCursorState();
                    if (mouseState.X - X != Width / 2f || mouseState.Y - Y != Height / 2f)
                    {
                        int dx = mouseState.X - middleX;
                        int dy = mouseState.Y - middleY;
                        Camera.MouseMove(dx, dy);
                    }
                }
                Mouse.SetPosition(X + Width / 2f, Y + Height / 2f);
                wasFocused = true;
            } else
            {
                wasFocused = false;
            }
            base.OnMouseMove(e);
        }

        private int middleX, middleY;

        public void SetMiddle(float newMiddleX, float newMiddleY)
        {
            middleX = (int)newMiddleX;
            middleY = (int)newMiddleY;
        }

        protected override void OnMove(EventArgs e)
        {
            SetMiddle(X + Width / 2f, Y + Height / 2f);
            base.OnMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            SetMiddle(X + Width / 2f, Y + Height / 2f);
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            shader.Dispose();
            GL.DeleteBuffer(VBO);
            GL.DeleteVertexArray(VAO);
            base.OnUnload(e);
        }

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
        }
    }
}
