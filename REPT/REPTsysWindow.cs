using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Security.Cryptography;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

using Kirali.MathR;
using Kirali.Light;
using Kirali.Framework;
using Kirali.Celestials;
using Kirali.Environment.Shaders;
using Kirali.Environment.Render.Primatives;
using Kirali.Storage;
using Kirali.REGS;

using REPT.Objects;
using REPT.Copied_Storage;


namespace REPT
{
    public class REPTsysWindow : GameWindow
    {
        Assembly executableAssembly;

        private bool renderActive = false;
        private int DEFAULT_WIDTH = 1080;
        private int DEFAULT_HEIGHT = 640;

        private int resolutionDivision = 2;

        public DoRenderUpdate updateRenderTags = new DoRenderUpdate();

        public REPTsysWindow(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            executableAssembly = Assembly.GetExecutingAssembly();
            //this.Icon = 
            this.KeyDown   += REPT_KeyDown;
            this.KeyPress  += REPT_KeyPress;
            this.MouseMove += REPT_MouseMove;
            

            int dres = 8;
            //Kirali.MathR.Vector3 camerapos = new Kirali.MathR.Vector3(0, -12, 7);
            //Kirali.MathR.Vector3 camerarot = new Kirali.MathR.Vector3(0, 0, 0);
            Kirali.MathR.Vector3 camerapos = new Kirali.MathR.Vector3(0, -32, 0);
            Kirali.MathR.Vector3 camerarot = new Kirali.MathR.Vector3(0, 0, 0);
            MainCamera = new Camera(camerapos, camerarot, 1920 / dres, 1080 / dres);
            MainCamera.RotateThet(-Math.PI / 2);

            Cel = new Objects.Celestials.PlanetAviea();
            //Cel = new CelestialRenderObject();

        }

        private void REPT_MouseMove(object sender, MouseMoveEventArgs e)
        {
            /*
            if (lockCursor)
            {
                double sensitivity = Math.PI * 0.0003;
                //mouseX = e.X;
                //mouseY = e.Y;

                if (correctiveMotion)
                {
                    correctiveMotion = false;
                }
                else
                {
                    //Mouse.SetPosition(Width / 2, Height / 2);
                    correctiveMotion = true;
                }
                MainCamera.rotation.Skew(e.YDelta * sensitivity, e.XDelta * sensitivity, 0);
            }
            */
        }

        private void REPT_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressEvent(e.KeyChar);
        }

        public void KeyPressEvent(char key)
        {
            double speed = 0.4;
            double rotSpeed = Math.PI * 0.02;

            switch (key)
            {
                case 'i':
                    MainCamera.Move(speed, MainCamera.CameraZ);
                    break;
                case 'j':
                    MainCamera.Move(speed, -1 * MainCamera.CameraX);
                    break;
                case 'k':
                    MainCamera.Move(speed, -1 * MainCamera.CameraZ);
                    break;
                case 'l':
                    MainCamera.Move(speed, MainCamera.CameraX);
                    break;
                case 'u':
                    MainCamera.Move(speed, -1 * MainCamera.CameraY);
                    break;
                case 'o':
                    MainCamera.Move(speed, MainCamera.CameraY);
                    break;
                case 'w':
                    MainCamera.RotateThet(-rotSpeed);
                    break;
                case 'a':
                    MainCamera.RotatePhi(-rotSpeed);
                    break;
                case 's':
                    MainCamera.RotateThet(rotSpeed);
                    break;
                case 'd':
                    MainCamera.RotatePhi(rotSpeed);
                    break;
                case 'q':
                    MainCamera.RotateR(rotSpeed);
                    break;
                case 'e':
                    MainCamera.RotateR(-rotSpeed);
                    break;
                default:

                    break;
            }
        }

        private void REPT_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Exit();
                    break;
                case Key.B:
                    if (CurrentCelestialRenderer == CelestialRenderMethod.WIREFRAME) {
                        Cel.ReloadTextures();
                        CurrentCelestialRenderer = CelestialRenderMethod.SHADING;
                    }
                    else { CurrentCelestialRenderer = CelestialRenderMethod.WIREFRAME; Cel.UnloadTextures(); }
                    break;
                default:

                    break;
            }
        }

        //renderinfo declaration
        private static CelestialRenderObject Cel;

        private static Camera MainCamera;

        
        protected override void OnLoad(EventArgs e)
        {
            //currentState = GameState.LOADING;
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);


            GL.Enable(EnableCap.Blend);
            GL.BlendFunc((BlendingFactor)BlendingFactorSrc.SrcAlpha, (BlendingFactor)BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthClamp);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Enable(EnableCap.Texture2D);

            base.OnLoad(e);
        }

        private int frameLoop = 1;
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState keyInput = Keyboard.GetState();
            if(CurrentCelestialRenderer != CelestialRenderMethod.WIREFRAME)
            {
                Cel.Rotation.X += 0.0003; if (Cel.Rotation.X > 2 * Math.PI) { Cel.Rotation.X -= Math.PI * 2; }
            }
            

            base.OnUpdateFrame(e);
        }

        CelestialRenderMethod CurrentCelestialRenderer = CelestialRenderMethod.SHADING;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            int trict;
            Cel.Render(MainCamera, CurrentCelestialRenderer, out trict);

            Title = "RiftEngine Planet Tools     Rendered " + trict + " triangles.";

            Context.SwapBuffers();
            base.OnRenderFrame(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            double scale = 1.0;
            if (((double)Width / DEFAULT_WIDTH) > ((double)Height / DEFAULT_HEIGHT))
                scale *= ((double)Height / DEFAULT_HEIGHT);
            else scale *= ((double)Width / DEFAULT_WIDTH);

            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            //shader.Dispose();
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            //GL.DeleteBuffer(VertexBufferObject);
            base.OnUnload(e);
        }
    }

    public enum CelestialRenderMethod
    {
        FLAT       = 0,
        SHADING    = 1,
        REALISTIC  = 2,
        WIREFRAME  = 3
    }

    public struct DoRenderUpdate
    {
        public bool UpdateAll;
        public bool Materials;
        public bool Normals;
        public bool Positions;
    }

}
