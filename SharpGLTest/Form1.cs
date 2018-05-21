using SharpGL;
using SharpGL.Enumerations;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Core;
using SharpGL.SceneGraph.Effects;
using SharpGL.SceneGraph.Lighting;
using SharpGL.SceneGraph.Quadrics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpGLTest
{
    public partial class Form1 : Form
    {
        private ArcBallEffect objectArcBallEffect;
        const float near = 0.01f;
        const float far = 10000;
        private bool mouseDownFlag;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitEvent(sceneControl1);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

        }
        void InitEvent(SceneControl sceneControl)
        {
            Init(sceneControl.Scene);
            sceneControl.OpenGLDraw += SceneControl1_OpenGLDraw;
            sceneControl.Resize += SceneControl1_Resize;
            sceneControl.MouseWheel += SceneControl1_MouseWheel;
            sceneControl.MouseDown += SceneControl1_MouseDown;
            sceneControl.MouseUp += SceneControl1_MouseUp;
            sceneControl.MouseMove += SceneControl1_MouseMove;
            sceneControl.KeyDown += SceneControl1_KeyDown;
            this.SizeChanged += delegate
            {
                this.objectArcBallEffect.ArcBall.SetBounds(sceneControl.Width,sceneControl.Height);
            };
        }
        #region Init
        private void InitElements(Scene scene)
        {
            // var objectRoot = new SharpGL.SceneGraph.Primitives.Folder() { Name = "Root" };

            //scene.SceneContainer.AddChild(objectRoot);
            var objectRoot = scene.SceneContainer.TraverseToRootElement();

            // This implements free rotation(with translation and rotation).
            var camera = GetCamera();
            objectArcBallEffect = new ArcBallEffect(
                camera.Position.X, camera.Position.Y, camera.Position.Z,
                camera.Target.X, camera.Target.Y, camera.Target.Z,
                camera.UpVector.X, camera.UpVector.Y, camera.UpVector.Z);
            objectRoot.AddEffect(objectArcBallEffect);

            Material gray = new Material();
            gray.Emission = Color.Gray;
            gray.Diffuse = Color.Gray;

            Disk disk = new Disk() { Name = "disk1" ,Slices=100,OuterRadius=10,Material=gray};
            disk.Transformation.TranslateX= 1f;
            disk.Transformation.RotateY = 90f;
            objectRoot.AddChild(disk);
            InitLight(objectRoot);


        }

        private void InitLight(SceneElement parent)
        {
            Light light1 = new Light()
            {
                Name = "Light 1",
                On = true,
                Position = new Vertex(-9, -9, 11),
                GLCode = OpenGL.GL_LIGHT0
            };
            Light light2 = new Light()
            {
                Name = "Light 2",
                On = true,
                Position = new Vertex(9, -9, 11),
                GLCode = OpenGL.GL_LIGHT1
            };
            Light light3 = new Light()
            {
                Name = "Light 3",
                On = true,
                Position = new Vertex(0, 15, 15),
                GLCode = OpenGL.GL_LIGHT2
            };
            var folder = new SharpGL.SceneGraph.Primitives.Folder() { Name = "Lights" };

            parent.AddChild(folder);
            folder.AddChild(light1);
            folder.AddChild(light2);
            folder.AddChild(light3);
        }
        #endregion

        private LookAtCamera GetCamera()
        {
            return this.sceneControl1.Scene.CurrentCamera as LookAtCamera;
        }

        #region Control Event
        private void SceneControl1_KeyDown(object sender, KeyEventArgs e)
        {
            const float interval = 1;
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up)
            {
                this.objectArcBallEffect.ArcBall.GoUp(interval);
            }
            else if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down)
            {
                this.objectArcBallEffect.ArcBall.GoDown(interval);
            }
            else if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
            {
                this.objectArcBallEffect.ArcBall.GoLeft(interval);
            }
            else if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
            {
                this.objectArcBallEffect.ArcBall.GoRight(interval);
            }
            else if(e.KeyCode==Keys.Q)
            {
                this.objectArcBallEffect.ArcBall.GoBack((int)interval);

            }
            else if(e.KeyCode==Keys.E)
            {
                this.objectArcBallEffect.ArcBall.GoFront((int)interval);
            }
        }

        private void SceneControl1_MouseMove(object sender, MouseEventArgs e)
        {
            objectArcBallEffect.ArcBall.MouseMove(e.X, e.Y);

            //var res = GetPoint(e.X, e.Y);
           // toolStripStatusLabel1.Text = $"Pos0:{res[0]:#0.000},{res[1]:#0.000},{res[2]:#0.000}";
            
        }
        double[] GetPoint(int x,int y)
        {
            var gl = sceneControl1.OpenGL;
            int[] viewport = new int[4];
            gl.GetInteger(GetTarget.Viewport, viewport);
            double[] mtrixv = new double[16];
            double[] mtrixp = new double[16];
            gl.GetDouble(GetTarget.ModelviewMatix, mtrixv);
            gl.GetDouble(GetTarget.ProjectionMatrix, mtrixp);
            var    realy = viewport[3] - y - 1;
            double objx, objy, objz;
            objx = objy = objz = 0;
            //gl.UnProject(x,realy,0,mtrixv,mtrixp,viewport,ref objx,ref objy,ref objz);
            gl.UnProject(x, realy, 1, mtrixv, mtrixp, viewport, ref objx, ref objy, ref objz);
            return new[] { objx,objy,objz};
        }

        private void SceneControl1_MouseUp(object sender, MouseEventArgs e)
        {
            this.mouseDownFlag = false;
            objectArcBallEffect.ArcBall.MouseUp(e.X, e.Y);
        }

        private void SceneControl1_MouseDown(object sender, MouseEventArgs e)
        {
            this.mouseDownFlag = true;
            objectArcBallEffect.ArcBall.SetBounds(this.sceneControl1.Width, this.sceneControl1.Height);
            objectArcBallEffect.ArcBall.MouseDown(e.X, e.Y);
        }

        private void SceneControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            objectArcBallEffect.ArcBall.Scale -= e.Delta * 0.001f;
        }

        private void SceneControl1_Resize(object sender, EventArgs e)
        {
            this.objectArcBallEffect.ArcBall.SetBounds(this.sceneControl1.Width, this.sceneControl1.Height);
        }

        private void SceneControl1_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {

        }
        
        void Init(Scene scene)
        {
            // scene.SceneContainer.Children.Clear();
            scene.RenderBoundingVolumes = false;
            // 设置视角
            var camera = GetCamera();
            camera.Near = near;
            camera.Far = far;
            //camera.Position = new Vertex(12.5f, -1.5f, 11.5f);
            //camera.Target = new Vertex(4.5f, 7, 2.5f);
            camera.UpVector = new Vertex(0.000f, 0.000f, 1.000f);
            InitElements(scene);
        }
        #endregion
    }
}
