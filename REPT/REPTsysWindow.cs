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

using REPT.Environment;
using REPT.Objects;
using REPT.Copied_Storage;


namespace REPT
{
    public class REPTsysWindow : GameWindow
    {
        Assembly executableAssembly;

        private int DEFAULT_WIDTH = 1080;
        private int DEFAULT_HEIGHT = 640;

        public static int CurrentBoundTexture = 0;

        public static string WriteToTitle = "REPT";

        private bool renderActive = false;
        
        
        CelestialRenderMethod CurrentCelestialRenderer // point to render world
        {
            get { return CelestialRenderWorld.CurrentCelestialRenderer; }
            set { CelestialRenderWorld.CurrentCelestialRenderer = value; }
        }
        
        public REPTsysWindow(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            //executableAssembly = Assembly.GetExecutingAssembly();
            //this.Icon = 

            InitGlEvents();
            InitRenderWorld();
            ResetCamera();
            
        }
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


        private void InitGlEvents()
        {
            KeyDown += REPT_KeyDown;
            KeyPress += REPT_KeyPress;
            MouseMove += REPT_MouseMove;
        }
        private void InitRenderWorld()
        {
            //Add error image to default render world
            RenderWorld.ErrorImage = new Texture2D("REPT.Resources.Debug.unknown_small.png");
            DefaultRenderWorld = new RenderWorld();
            DefaultRenderWorld.RegisterTexture(RenderWorld.ErrorImage);

            //Init CRO World, Default textures are automatically loaded in CRW Init!
            CelestialRenderWorld = new CelestialRenderWorld();


            CelestialRenderObject star01 = new CelestialRenderObject(CRO_Type.STAR_SMALL, 5);
            //CelestialRenderObject star02 = new CelestialRenderObject(CRO_Type.STAR_SMALL);
            CelestialRenderObject planet = new CelestialRenderObject(CRO_Type.PLANET_TERRAN, 1);

            star01.Position = new Kirali.MathR.Vector3(0, 1, 0);
            planet.Position = new Kirali.MathR.Vector3(0, -25, 0);
            //star02.Position = new Kirali.MathR.Vector3(0,  15, 0);

            CelestialRenderWorld.AddObject(star01);
            CelestialRenderWorld.AddObject(planet);
            //Cel = new Objects.Celestials.PlanetAviea();
        }



        #region User_Input

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
        private void REPT_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Exit();
                    break;
                case Key.B:
                    if (CurrentCelestialRenderer == CelestialRenderMethod.WIREFRAME) {
                        CurrentCelestialRenderer = CelestialRenderMethod.SHADING;
                    }
                    else if (CurrentCelestialRenderer == CelestialRenderMethod.SHADING)
                    {
                        CurrentCelestialRenderer = CelestialRenderMethod.FLAT;
                    }
                    else if (CurrentCelestialRenderer == CelestialRenderMethod.FLAT)
                    { CurrentCelestialRenderer = CelestialRenderMethod.WIREFRAME; }
                    break;
                case Key.Tilde:
                    if (NavMode == CameraNavigationMode.FREE)
                    {
                        NavMode = CameraNavigationMode.ORBIT;
                        ResetCamera();
                    }
                    else if(NavMode == CameraNavigationMode.ORBIT)
                    {
                        NavMode = CameraNavigationMode.FREE;
                        ResetCamera();
                    }
                    break;
                default:

                    break;
            }
        }
        public void KeyPressEvent(char key)
        {
            //MOVED TO ONUPDATEFRAME
        }

        #endregion User_Input

        //RENDER WORLDS
        public static RenderWorld DefaultRenderWorld;
        public static RenderWorld CelestialRenderWorld;
        
        public static Texture2D ErrorTexture
        {
            get { return DefaultRenderWorld.TextureStorage[0]; }
        }

        public static bool TryGetTexture(int renderworld, int position, out Texture2D found)
        {
            try
            {
                switch (renderworld)
                {
                    case 0:
                        found = DefaultRenderWorld.TextureStorage[position];
                        return true;
                    case 1:
                        found = CelestialRenderWorld.TextureStorage[position];
                        return true;
                    default:
                        found = ErrorTexture;
                        return false;

                }
            }
            catch { found = ErrorTexture; return false; }
        }

        #region Camera_Function
        public static Camera MainCamera // point to active renderworld camera!
        {
            get { return CelestialRenderWorld.MainCamera; } 
            set { CelestialRenderWorld.MainCamera = value; } 
        }
        CameraNavigationMode NavMode = CameraNavigationMode.ORBIT;

        private Kirali.MathR.Vector3 CameraNav_dir       = new Kirali.MathR.Vector3(0, 0, -1);  //Z
        private Kirali.MathR.Vector3 CameraNav_thet      = new Kirali.MathR.Vector3(1, 0,  0);  //X
        private Kirali.MathR.Vector3 CameraNav_phi       = new Kirali.MathR.Vector3(0, 1,  0);  //Y
        private Kirali.MathR.Vector3 CameraNav_Position  = new Kirali.MathR.Vector3(0, -25,  0);
        private double CameraNav_Distance
        {
            get { return CameraNav_Position.Length(); }
            set { CameraNav_Position = value * CameraNav_Position.SafeNormalize(); MainCamera.position = CameraNav_Position; }
        }
        public void ResetCamera()
        {
            Kirali.MathR.Vector3 camerapos = new Kirali.MathR.Vector3(0, -32, 0);
            Kirali.MathR.Vector3 camerarot = new Kirali.MathR.Vector3(0, 0, 0);
            MainCamera = new Camera(camerapos, camerarot, Width, Height);
            MainCamera.RotateThet(-Math.PI / 2);
            if(NavMode == CameraNavigationMode.ORBIT)
                CameraNav_Distance = 55;

            CameraNav_dir       = new Kirali.MathR.Vector3(0, 0, -1);  //Z
            CameraNav_thet      = new Kirali.MathR.Vector3(1, 0,  0);  //X
            CameraNav_phi       = new Kirali.MathR.Vector3(0, 1,  0);  //Y
        }
        
        private void MainCameraRotate_thet(double rad)
        {
            MainCamera.RotateThet(rad);
            CameraNav_Position  = Kirali.MathR.Vector3.RotateU( CameraNav_Position , MainCamera.CameraX, rad);
            MainCamera.position = CameraNav_Position;
        }
        private void MainCameraRotate_phi(double rad)
        {
            MainCamera.RotatePhi(rad);
            CameraNav_Position  = Kirali.MathR.Vector3.RotateU( CameraNav_Position , MainCamera.CameraY, rad);
            MainCamera.position = CameraNav_Position;
        }
        private void MainCameraRotate_r(double rad)
        {
            MainCamera.RotateR(rad);
            CameraNav_Position  = Kirali.MathR.Vector3.RotateU( CameraNav_Position , MainCamera.CameraZ, rad);
            MainCamera.position = CameraNav_Position;
        }

        #endregion Camera_Function



        



        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();
            double speed = 0.03;
            double in_out_speed = 0.3;
            double rotSpeed = Math.PI * 0.005;

            //KEY CONTROLS
            if (input.IsKeyDown(Key.I)) {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.Move(speed, MainCamera.CameraZ);
            }
            if (input.IsKeyDown(Key.J)) 
            {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.Move(speed, -1 * MainCamera.CameraX);
            }
            if (input.IsKeyDown(Key.K)) 
            {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.Move(speed, -1 * MainCamera.CameraZ);
            }
            if (input.IsKeyDown(Key.L)) 
            {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.Move(speed, MainCamera.CameraX);
            }
            if (input.IsKeyDown(Key.U)) 
            {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.Move(speed, -1 * MainCamera.CameraY);
            }
            if (input.IsKeyDown(Key.O)) 
            {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.Move(speed, MainCamera.CameraY);
            }
            if (input.IsKeyDown(Key.W)) 
            {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.RotateThet(-rotSpeed);
            else if (NavMode == CameraNavigationMode.ORBIT)
                MainCameraRotate_thet(rotSpeed);
            }
            if (input.IsKeyDown(Key.A)) {
                    if (NavMode == CameraNavigationMode.FREE)
                MainCamera.RotatePhi(-rotSpeed);
            else if (NavMode == CameraNavigationMode.ORBIT)
                MainCameraRotate_phi(rotSpeed);
            }
                if (input.IsKeyDown(Key.S)) {
                    if (NavMode == CameraNavigationMode.FREE)
                MainCamera.RotateThet(rotSpeed);
            else if (NavMode == CameraNavigationMode.ORBIT)
                MainCameraRotate_thet(-rotSpeed);
            }
            if (input.IsKeyDown(Key.D)) 
            {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.RotatePhi(rotSpeed);
                else if (NavMode == CameraNavigationMode.ORBIT)
                    MainCameraRotate_phi(-rotSpeed);
            }
            if (input.IsKeyDown(Key.Q)) 
            {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.RotateR(rotSpeed);
                else if (NavMode == CameraNavigationMode.ORBIT)
                    MainCameraRotate_r(rotSpeed);
            }
            if (input.IsKeyDown(Key.E)) 
            {
                if (NavMode == CameraNavigationMode.FREE)
                    MainCamera.RotateR(-rotSpeed);
                else if (NavMode == CameraNavigationMode.ORBIT)
                    MainCameraRotate_r(-rotSpeed);
            }
            if (input.IsKeyDown(Key.F)) 
            {
                if (NavMode == CameraNavigationMode.ORBIT && CameraNav_Distance - in_out_speed >= 25)
                    CameraNav_Distance -= in_out_speed;
            }
            if (input.IsKeyDown(Key.R)) 
            {
                if (NavMode == CameraNavigationMode.ORBIT && CameraNav_Distance + in_out_speed <= 100)
                    CameraNav_Distance += in_out_speed;
            }


            //ANIMATE / Move objs
            if (CurrentCelestialRenderer != CelestialRenderMethod.WIREFRAME)
            {
                //Cel.Rotation.X += 0.0003; if (Cel.Rotation.X > 2 * Math.PI) { Cel.Rotation.X -= Math.PI * 2; }
            }

            base.OnUpdateFrame(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            CelestialRenderWorld.RenderAll();

            Title = "RiftEngine Planet Tools    " + WriteToTitle;
            WriteToTitle = "";

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
            ResetCamera();
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

    public enum CameraNavigationMode
    {
        ORBIT = 0,
        FREE  = 1,
        FLY   = 2
    }

    public struct DoRenderUpdate
    {
        public bool UpdateAll;
        public bool Materials;
        public bool Normals;
        public bool Positions;
    }

}
