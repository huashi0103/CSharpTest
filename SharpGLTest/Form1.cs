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
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SharpGLTest
{
    public partial class Form1 : Form
    {
        private ArcBallEffect objectArcBallEffect;
        const float near = 0.01f;
        const float far = 10000;
        private bool mouseDownFlag;




        //string path = "data.txt";
        string path= @"D:\CODE\DisplayPoints\7125.asc";
        public Form1()
        {
            InitializeComponent();
            InitParam();
        }
        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);
            defualtToolStripMenuItem.Click += delegate
            {
                InitParam();

            };
            testFormToolStripMenuItem.Click += delegate
            {
                var frm = new FrmTest();
                frm.data = pDoc.m_ptVertexList;
                frm.Show();
            };

            InitEvent(sceneControl1);

            m_rectOld.Width = openGLControl1.Width;
            m_rectOld.Height = openGLControl1.Height;
            InitOpenGL();

            openGLControl1.MouseDown += OpenGLControl1_MouseDown;
            openGLControl1.MouseWheel += OpenGLControl1_MouseWheel;
            openGLControl1.MouseUp += OpenGLControl1_MouseUp;
            openGLControl1.MouseMove += OpenGLControl1_MouseMove;

            openGLControl1.Resized += OpenGLControl1_Resize;

            openGLControl1.OpenGLDraw += OpenGLControl1_OpenGLDraw;
        }

        private void OpenGLControl1_Resize(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl1.OpenGL;

            //  设置当前矩阵模式,对投影矩阵应用随后的矩阵操作
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            // 重置当前指定的矩阵为单位矩阵,将当前的用户坐标系的原点移到了屏幕中心
            gl.LoadIdentity();
            m_rectOld.Width = openGLControl1.Width;
            m_rectOld.Height = openGLControl1.Height;
            //这里有问题，控件大小变了以后视口和视点都没了///
            //todo

            gl.Viewport(0, 0, m_rectOld.Width, m_rectOld.Height);
            // 创建透视投影变换
            // gl.Perspective(30.0f, (double)Width / (double)Height, 5, 100.0);

            InitView();
        }

        private void OpenGLControl1_OpenGLDraw(object sender, RenderEventArgs args)
        {
    
           var gl = openGLControl1.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();
            gl.PushMatrix();
            conversion(gl);
            //drawaxis(gl);
            DrawData(gl);
            gl.PopMatrix();
            gl.Flush();
            //双缓冲
            //SwapBuffers(wglGetCurrentDC());
        }
        void mydraw(OpenGL gl)
        {
            //gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);//清除缓存将背景置
            //gl.Color(1.0, 1.0, 1.0);
            //gl.PointSize(1);
            //gl.CallList(glbase);
            //gl.CallList(baselist);
        }

        double c=Math.PI/180.0; //弧度和角度转换参数
        int du = 0, oldmy= -1,oldmx= -1; //du是视点绕y轴的角度,opengl里默认y轴是上方向
        float r=1.5f, h= 0.0f; //r是视点绕y轴的半径,h是视点高度即在y轴上的坐标
        int xTran=0, yTran= 0;
        void conversion(OpenGL gl)
        {

            float transx, transy, transz;//平移参数
            float scalex, scaley, scalez;//缩放参数
            float rotx, rotz;//旋转参数，只有考虑当前x轴和Z轴旋转	

            transx = glTransX + glCurTransX;
            transy = glTransY + glCurTransY;
            transz = glTransZ + glCurTransZ;
            gl.Translate(transx, transy, transz);//平移

            //坐标轴中心移到起始中心  保证图的缩放和旋转是以当前视角进行
            //旋转缩放后再移回世界坐标系原点
            gl.Translate(pDoc.m_ptBoxCenter.x, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z);
            rotx = glCurRotX + glRotX;//X轴旋转参数
            if(Math.Abs(rotx)>360)
                rotx =(Math.Abs(rotx)%360)*(Math.Abs(rotx)/rotx);
            gl.Rotate(rotx, 1.0, 0.0, 0.0);
            rotz = glCurRotZ + glRotZ;//Z轴旋转角度

            if (Math.Abs(rotz) > 360)
                rotx = (Math.Abs(rotz) % 360) * (Math.Abs(rotz) / rotz);
            var interval = 1.0 * (Math.Abs(rotx) * rotx) * (Math.Abs(rotz) * rotz);
            gl.Rotate(rotz, 0.0, 0.0, 1.0);

            testlabl.Text = $"{rotx},{rotz}";
            if (glScaleX < 0.001)
            {
                glScaleX = glScaleY = glScaleZ = 0.001f;
            }
            gl.Scale(glScaleX, glScaleY, glScaleZ);
            gl.Translate(-pDoc.m_ptBoxCenter.x, -pDoc.m_ptBoxCenter.y, -pDoc.m_ptBoxCenter.z);//移回去


        }
        void drawaxis(OpenGL gl)
        {
            gl.LineWidth(1.0f);
            gl.Begin(OpenGL.GL_LINES);
            //缩放
            gl.Color(1.0, 1.0, 1.0);//白色 X轴
            double delt = 50 * glScaleX;
            gl.Vertex(pDoc.m_ptBoxCenter.x - 500, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z);
            gl.Vertex(pDoc.m_ptBoxCenter.x + 500, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z);

            gl.Vertex(pDoc.m_ptBoxCenter.x - delt, pDoc.m_ptBoxCenter.y - 5, pDoc.m_ptBoxCenter.z);
            gl.Vertex(pDoc.m_ptBoxCenter.x - delt, pDoc.m_ptBoxCenter.y + 5, pDoc.m_ptBoxCenter.z);
            gl.Vertex(pDoc.m_ptBoxCenter.x + delt, pDoc.m_ptBoxCenter.y - 5, pDoc.m_ptBoxCenter.z);
            gl.Vertex(pDoc.m_ptBoxCenter.x + delt, pDoc.m_ptBoxCenter.y + 5, pDoc.m_ptBoxCenter.z);

            //
            gl.Color(1.0, 0.0, 0.0);//红色Z轴
            gl.Vertex(pDoc.m_ptBoxCenter.x, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z + 500);
            gl.Vertex(pDoc.m_ptBoxCenter.x, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z - 500);

            gl.Vertex(pDoc.m_ptBoxCenter.x - 5, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z + delt);
            gl.Vertex(pDoc.m_ptBoxCenter.x + 5, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z + delt);

            gl.Vertex(pDoc.m_ptBoxCenter.x - 5, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z - delt);
            gl.Vertex(pDoc.m_ptBoxCenter.x + 5, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z - delt);
            gl.End();

        }
        
        private void OpenGLControl1_OpenGLInitialized(object sender, EventArgs e)
        {
            //var gl = openGLControl1.OpenGL;
            ////背景色 默认为黑色
            //gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            ////gl.ClearColor(255, 0, 0, 255);
            //gl.ShadeModel(OpenGL.GL_SMOOTH);
            //gl.ClearDepth(1.0f);
            //gl.Enable(OpenGL.GL_DEPTH_TEST);
        }

        private void OpenGLControl1_MouseMove(object sender, MouseEventArgs e)
        {
            GetPoint(openGLControl1.OpenGL, e.X, e.Y);

            if (m_bMoving)
            {
                glCurTransX = (float)((e.X - glDownX) / m_rectOld.Width * 2 * pDoc.m_dbDistance);
                glCurTransY = (float)((glDownY - e.Y) / m_rectOld.Height * 2 * pDoc.m_dbDistance);

                //Invalidate(FALSE);//显示更新
            }
            if (m_bRotation)
            {
                glCurRotZ = (float)((double)(e.X - glDownX) * 360.0 / (double)m_rectOld.Width);
                glCurRotX = (float)((double)(e.Y - glDownY) * 360.0 / (double)m_rectOld.Height);
               // Invalidate(FALSE);//显示更新
            }
        }

        private void OpenGLControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_bMoving)
            {       //平移
                glCurTransX = (float)((e.X - glDownX) / m_rectOld.Width * 2 * pDoc.m_dbDistance);
                glCurTransY = (float)((glDownY - e.Y) / m_rectOld.Height * 2 * pDoc.m_dbDistance);
                glTransX += glCurTransX;
                glTransY += glCurTransY;
                glTransZ += glCurTransZ;
                glCurTransX = 0;
                glCurTransY = 0;
                glCurTransZ = 0;
                m_bMoving = false;
            }
            if (m_bRotation)
            {
                ////旋转
                glCurRotZ = (e.X - glDownX) * 360.0f /(float)m_rectOld.Width;
                glCurRotX = (e.Y - glDownY) * 360f / (float)m_rectOld.Height;
                glRotX += glCurRotX;
                glRotY += glCurRotY;
                glRotZ += glCurRotZ;
                glCurRotX = 0;
                glCurRotY = 0;
                glCurRotZ = 0;
                m_bRotation = false;

            }
           // Invalidate(FALSE);
        }

        private void OpenGLControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            switch (Control.ModifierKeys)
            {
                case Keys.Control:
                    glScaleZ += (float)e.Delta / (float)m_rectOld.Height;
                    break;
                case Keys.Shift:
                    glCurTransZ = (float)e.Delta / (float)m_rectOld.Height * 50;
                    glTransZ += glCurTransZ;
                    break;
                default:
                    glScaleX = glScaleY = glScaleZ += (float)e.Delta / (float)m_rectOld.Height;
                    break;
            }
            //Invalidate(FALSE);
        }

        private void OpenGLControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Middle)
            {//平移
                m_bMoving = true;
                glDownX = (float)e.X;
                glDownY = (float)e.Y;
                glDownZ = 0;
            }
            else
            {//旋转
                m_bRotation = true;
                glDownX = (float)e.X;
                glDownY = (float)e.Y;
                glDownZ = 0;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            tabControl1.SelectedIndex = 1;

        }

        #region scene
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

        #endregion

        float glDownX, glDownY, glDownZ; //按下时的坐标
        float glTransX, glTransY, glTransZ; //平移值
        float glRotX, glRotY, glRotZ;//旋转值
        float glScaleX, glScaleY, glScaleZ;//缩放值

        float glCurRotX, glCurRotY, glCurRotZ;
        float glCurTransX, glCurTransY, glCurTransZ;
        bool m_bScale;
        bool m_bRotation;
        bool m_bMoving;
        Point ptDown;
        bool bInitOpenGL = false;
        uint glbase;
        Rectangle m_rectOld = new Rectangle();
        DisplayDoc pDoc = new DisplayDoc();

        void InitParam()
        {  
            //放到构造函数中
            //初始化参数
            glTransX = glTransY = glTransZ = 0;
            glCurTransX = glCurTransY = glCurTransZ = 0;
            glRotX = glRotY = glRotZ = 0;
            glRotX = -30;


            glScaleX = glScaleY = glScaleZ = 1.0f;
            glCurRotX = glCurRotY = glCurRotZ = 0.0f;
            m_bScale = false;
            m_bRotation = false;
            m_bMoving = false;
            bInitOpenGL = false;
        }
        void InitOpenGL()
        {
            //加载数据:
            pDoc.LoadData(path);
            //初始化视窗
            InitView();

        }

        void GetPoint(OpenGL gl,int x, int y)
        {
            int[] viewport = new int[4];
            double[] mvmatrix = new double[16];
            double[] projmatrix = new double[16];
            gl.GetInteger(GetTarget.Viewport, viewport);
            gl.GetDouble(GetTarget.ModelviewMatix, mvmatrix);
            gl.GetDouble(GetTarget.ProjectionMatrix, projmatrix);

            /*  note viewport[3] is height of window in pixels  */
           var  realy = viewport[3] - y - 1;
           double wx = 0, wy = 0, wz = 0;
           gl.UnProject(x, realy, 0.0,
               mvmatrix, projmatrix, viewport, ref wx, ref wy, ref wz);
            double wx1 = 0, wy1= 0, wz1 = 0;
            gl.UnProject(x, realy, 1.0,
                  mvmatrix, projmatrix, viewport, ref wx1, ref wy1, ref wz1);
            toolStripStatusLabel1.Text = $"{wx:F4},{wy:F4},{wz:F4}|||{wx1:F4},{wy1:F4},{wz1:F4}";
        }
        void DrawData(OpenGL gl)
        {

           // gl.DeleteLists(glbase, 1);
            //glbase = gl.GenLists(1);
           // gl.NewList(glbase, OpenGL.GL_COMPILE);

            gl.PointSize(1.0f);
            gl.Begin(BeginMode.Points);
            foreach (var ptXYZ in pDoc.m_ptVertexList)
            {
                //gl.Color(1.0f, 0.0f, 0.0f);
                gl.Color(1.0 * (1.0 - Math.Abs(ptXYZ.z) / 60.0), 0.0, 1.0 * Math.Abs(ptXYZ.z) / 60.0);
                gl.Vertex(ptXYZ.x, ptXYZ.y, ptXYZ.z);
            }

            gl.End();
            //gl.EndList();

       
        }
        void InitView()
        {
            var gl = openGLControl1.OpenGL;
            double wid = m_rectOld.Width;
            double height = m_rectOld.Height;

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.MatrixMode(MatrixMode.Projection);
            gl.ShadeModel(ShadeModel.Smooth);
            gl.ClearDepth(1.0f);
            gl.LoadIdentity();

            ////设置视窗
            if (wid <= height)
            {
                gl.Ortho(-pDoc.m_dbDistance, pDoc.m_dbDistance,
                    -pDoc.m_dbDistance * height / wid,
                    pDoc.m_dbDistance * height / wid,
                    -pDoc.m_dbDistance, pDoc.m_dbDistance);
            }
            else
            {
                gl.Ortho(-pDoc.m_dbDistance * wid / height,
                    pDoc.m_dbDistance * wid / height,
                    -pDoc.m_dbDistance, pDoc.m_dbDistance, -pDoc.m_dbDistance, pDoc.m_dbDistance);

            }

            gl.LookAt(pDoc.m_ptBoxCenter.x, pDoc.m_ptBoxCenter.y, 0, pDoc.m_ptBoxCenter.x, pDoc.m_ptBoxCenter.y, pDoc.m_ptBoxCenter.z,  0, 1, 0);

            bInitOpenGL = true;
            gl.MatrixMode(MatrixMode.Modelview);
 
        }

        





    }


}
