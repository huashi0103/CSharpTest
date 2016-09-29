using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using Autodesk.AutoCAD.BoundaryRepresentation;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Color = Autodesk.AutoCAD.Colors.Color;
using Exception = System.Exception;

[assembly: ExtensionApplication(typeof(cadObjArx.CADExetensionCls))]
[assembly: CommandClass(typeof(cadObjArx.CADCommandS))]

namespace cadObjArx
{   
    public class CADExetensionCls : IExtensionApplication
    {
        public void Initialize()
        {

            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.SetImpliedSelection(new ObjectId[0]);
            ed.WriteMessage("\n加载cadObjArx\n");
           // load();
            Application.Idle += new EventHandler(Application_Idle);
           
        }

        void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
      
            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //if (doc.LockMode() == DocumentLockMode.NotLocked)
            //{
            //    doc.LockDocument(DocumentLockMode.Write, "", "", false);
            //}
            //doc.SendStringToExecute("InitT\n", false, false, false);
   
        }

        public void Terminate()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            doc.LockDocument(DocumentLockMode.NotLocked, "", "", false);

        } 
        private void load()
        {
            try
            {
                AcadMenuGroups menugroups = (AcadMenuGroups)Application.MenuGroups; 
                AcadToolbar toolbar = menugroups.Item(0).Toolbars.Add("Test");
                int i = toolbar.Count;
                AcadToolbarItem item = toolbar.AddToolbarButton(0, "ENTER", "", Convert.ToChar(13).ToString());
                string path  = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                item.SetBitmaps(path + "\\Enter16x16.bmp", path + "\\Enter32x32.bmp");

                item = toolbar.AddToolbarButton(1, "测试", "测试", "SB\n");
                item.SetBitmaps(path + "\\设置16x16.bmp", path + "\\设置32x32.bmp");

                item = toolbar.AddToolbarButton(2, "ESC", "撤销", Convert.ToChar(27).ToString());
                item.SetBitmaps(path + "\\ESC16x16.bmp", path + "\\ESC32x32.bmp");

                //item = toolbar.AddToolbarButton(3, "NESC", "撤销选中", "NESC\n");
               
                item = toolbar.AddToolbarButton(3, "NESC", "撤销选中", "AddArea\n");
            
                toolbar.Dock(AcToolbarDockStatus.acToolbarDockTop);
            }
            catch (Exception ex)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(ex.Message);              
            }


        }
    }

    public class CADCommandS
    {

        [CommandMethod("InitT",CommandFlags.Session)]
        public void Init()
        {

            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            string path = ((AcadApplication) Application.AcadApplication).ActiveDocument.FullName;
            ed.WriteMessage("测试init{0}:{1}\n",DateTime.Now.ToString(),path);
            
            
            loadlinetype();
            LoadLayer();
        }

        [CommandMethod("NESC", CommandFlags.Transparent)]
        public void ESC()
        {
            SetForegroundWindow(Application.MainWindow.Handle);
            keybd_event(0x1B, 0, 0, 0);
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("测试=======\n");
        }

        //添加图层
        private void LoadLayer()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            using (Transaction tr = acDoc.TransactionManager.StartTransaction())
            {
                //指定点图层
                LayerTable lt = (LayerTable)tr.GetObject(acDoc.Database.LayerTableId, OpenMode.ForRead);
                if (!lt.Has("test"))//判断是否存在
                {//不存在添加
                    var LayerID = ObjectId.Null;
                    LayerTableRecord ltr = new LayerTableRecord();
                    ltr.Name = "test";
                    ltr.Color = Autodesk.AutoCAD.Colors.Color.FromColorIndex(
                        Autodesk.AutoCAD.Colors.ColorMethod.ByColor, 130);
                    lt.UpgradeOpen();
                    LayerID = lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);
                }
                tr.Commit();
            }
        }

        private bool isBCAD = false;
        /// <summary>
        /// 列出所有对象
        /// </summary>
        [CommandMethod("LE",CommandFlags.Transparent)]
        public  void ListEntities()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acadEd = Application.DocumentManager.MdiActiveDocument.Editor;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                //列举组
                var dicgroup = acTrans.GetObject(acCurDb.GroupDictionaryId, OpenMode.ForRead) as DBDictionary;
                foreach (var dic in dicgroup)
                {
                    var group = acTrans.GetObject(dic.Value, OpenMode.ForRead) as Group;
                    if (group == null) continue;
                    acadEd.WriteMessage(dic.Key+":"+group.Name+"\n");

                }
                //列举实体对象
                BlockTable acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                int nCnt = 0;
                acDoc.Editor.WriteMessage("\nModel space objects: ");
                foreach (ObjectId acObjId in acBlkTblRec)
                {

                    acDoc.Editor.WriteMessage("\n" + acObjId.ObjectClass.DxfName + ":" + acObjId.Handle.Value.ToString());
                    nCnt = nCnt + 1;
                }
                if (nCnt == 0)
                {
                    acDoc.Editor.WriteMessage("\nNo objects found.");
                }
                else
                {
                    acDoc.Editor.WriteMessage("\nTotal {0} objects.", nCnt);
                }

            }
        }

        private int _id = 0;
        /// <summary>
        /// 生成闭合区域，
        /// </summary>
        [CommandMethod("SetArea", CommandFlags.Transparent)]
        public void SetArea()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acadEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptPointOptions ptOpt = new PromptPointOptions("选择点:\n");
            PromptPointResult ptRel = null;
            ObjectId[] ids = null;

            ptRel = acadEd.GetPoint(ptOpt);
            Database acCurDb = acDoc.Database;
            //该方法必须是ObjectArx2011以后的版本才支持，以前的版本可以用-bo命令加SelectImplied方法得到的选择集来实现该功能
            var bound = acadEd.TraceBoundary(ptRel.Value, true);
   
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                foreach (var dbObj in bound)
                {
                    var line = dbObj as Polyline;
                    if (acCurDb != null) line.Color = Color.FromColor(System.Drawing.Color.Red);
                    BlockTable bt = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    var id = btr.AppendEntity(line);//添加实体
                    acTrans.AddNewlyCreatedDBObject(line, true);//保存到cad数据库
                    //下面为实体添加扩展属性,这里扩展属性添加到实体的扩展字典中，也可以直接添加到实体的XData的属性中
                    line.CreateExtensionDictionary();//为实体创建扩展字典
                    var dic = acTrans.GetObject(line.ExtensionDictionary, OpenMode.ForWrite) as DBDictionary;//获取实体的扩展字典，DBDictionary，可以用通过SetAt和GetAt设置和获取
                    var xrecord=new Xrecord();//创建Xrecord对象。里边保存具体的扩展属性对象
                    xrecord.Data = new ResultBuffer(new TypedValue((int) DxfCode.Text, "测试1"));//添加属性对象都保存为ResultBuffer对象对应不同的DxfCode码
                    dic.SetAt("exta", xrecord);//添加到字典中，继承自DBObject的对象都可以作为value添加到字典中
                    acTrans.AddNewlyCreatedDBObject(xrecord, true);//因为添加的扩展属性也是DBObject对象,要添加到cad的数据库中
                    //这里添加一个cad的DataTable对象到字典
                    var dt = new DataTable();
                    dt.TableName = "polyattri";
                    dt.AppendColumn(CellType.Integer, "line_id");
                    var lineid = new DataCell();
                    lineid.SetInteger(_id);
                    var row = new DataCellCollection();
                    row.Add(lineid);
                    dt.AppendRow(row, true);
                    dic.UpgradeOpen();
                    dic.SetAt("attr", dt);
                    acTrans.AddNewlyCreatedDBObject(dt, true);
                    _id++;
                    acadEd.WriteMessage(line.ObjectId.ToString());
                }
                acTrans.Commit();
            }

        }
        //导入线型，根据线型文件和线名来加载线型，后面可以通过设置当前的环境变量来改变当前线型
        //路径如果为文件名，cad会自动在主程序的相对路径下搜索，如果是自定线型需要指定绝对路径，
        //如果线型文件关联SHX文件，记得要一起不然会加载失败
        private void loadlinetype()
        {
            var lineTypeFile = "acadiso.lin";
            string lineTypeName = "acad_iso03w100";
            ObjectId idRet = ObjectId.Null;
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acadEd = Application.DocumentManager.MdiActiveDocument.Editor;
            Database db = acDoc.Database;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {

                LinetypeTable ltt = (LinetypeTable)trans.GetObject(db.LinetypeTableId, OpenMode.ForWrite);
                LinetypeTableRecord lttr = new LinetypeTableRecord();

                if (ltt.Has(lineTypeName))
                {
                    idRet = ltt[lineTypeName];
                }
                else
                {
                    try
                    {
                        db.LoadLineTypeFile(lineTypeName, lineTypeFile);//加载线型
                        idRet = ltt[lineTypeName];
                    }
                    catch (System.Exception ex)
                    {
                        acadEd.WriteMessage(ex.Message);
                    }
                    finally
                    {
                        trans.Commit();
                    }
                }
            }
        }
        //设置线型
        [CommandMethod("SLT")]
        public void SetLineType()
        {
            string test = Application.GetSystemVariable("CELTYPE").ToString();
            Application.SetSystemVariable("CELTYPE", "acad_iso03w100");//通过改变环境变量来设置线型,CELTYPE对应当前线型
        }
        //删除选中//这里实现先选中后删除的操作
         [CommandMethod("DelS")]
        public void DelSelect()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acadEd = Application.DocumentManager.MdiActiveDocument.Editor;

            SelectionSet acSSet;
            var selectionRes = acadEd.SelectImplied();//对应选中的选择集
            Database acCurDb = acDoc.Database;
            if (selectionRes.Status == PromptStatus.OK)
            {
                acSSet = selectionRes.Value;

                using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
                {
                    foreach (var v in acSSet.GetObjectIds())
                    {
                        DBObject dbObj = acTrans.GetObject(v, OpenMode.ForWrite);
                        dbObj.Erase(true);
                    }
                    acTrans.Commit();
                }

            }
            else
            {
                acDoc.Editor.WriteMessage(selectionRes.Status.ToString());
            }
        }
               
        //选中线病获取上面设置的扩展属性，上面怎么添加的这边就怎么解析Xrecord对象
        [CommandMethod("SLL")]
        public void SelectLine()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Polyline line;
            if (!SelectEntity(out line, "多段线")) return;
            if (line == null) return;
            if (line != null)
            {
                using (var tr = Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
                {
                    var xdata = line.XData;
                    if (xdata != null)
                    {
                        var iter = xdata.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            var value = (TypedValue)iter.Current;
                            ed.WriteMessage(value.TypeCode.ToString() +":"+ value.Value + "\n");
                        }
                    }
                    //var dic = tr.GetObject(line.ExtensionDictionary, OpenMode.ForRead) as DBDictionary;
                    //ObjectId id = dic.GetAt("attr");
                    //var dt = tr.GetObject(id, OpenMode.ForRead) as DataTable;
                    //int index = dt.GetColumnIndexAtName("line_id");
                    //var att = dt.GetCellAt(0, index).Value;
                    //ed.WriteMessage(att + "\n");
                    //id = dic.GetAt("exta");
                    //var xrec = tr.GetObject(id, OpenMode.ForRead) as Xrecord;
                    //foreach (var xdata in xrec.Data)
                    //{
                    //    ed.WriteMessage(xdata.Value.ToString() + "\n");
                    //}
                }
                // ed.WriteMessage("\n{0},{1}", line.StartPoint.X, line.StartPoint.Y);
                //ed.WriteMessage("\n{0},{1}", line.EndPoint.X, line.EndPoint.Y);
            }
        }
        [CommandMethod("SLLT")]
        public void SelectPolyline2d()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Polyline2d line;
            if (!SelectEntity(out line, "二维多段线")) return;
            if (line == null) return;
            if (line != null)
            {
                using (var tr = Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
                {
                    var xdata = line.XData;
                    if (xdata != null)
                    {
                        var iter = xdata.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            var value = (TypedValue)iter.Current;
                            ed.WriteMessage(value.TypeCode.ToString() + ":" + value.Value + "\n");
                        }
                    }
                }
            }
        }

        //添加文字
        [CommandMethod("SLP")]
        public void SelectPoint()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var sel = ed.GetPoint("选择点");
            var db = Application.DocumentManager.MdiActiveDocument.Database;
            using (var tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord     btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                DBText txt = new DBText();
                txt.Position = sel.Value;
                txt.Height = 0.5;
                txt.TextString = "测试";
                
                btr.AppendEntity(txt);
                tr.AddNewlyCreatedDBObject(txt, true);
                tr.Commit();
            }
        }

        //选中所有的多段线
        [CommandMethod("SLPS")]
        public void SelectPolyline()
        {
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            //添加过滤条件，可以添加类型/图层条件不同的dxfcode对应不同的对象
            var tvs = new TypedValue[] { new TypedValue((int)DxfCode.Start, "LWPOLYLINE") };//LWPOLYLINE 对应Polyline对象，这里的类型字段可以用前面的列举对象来查看···，
            var sf = new SelectionFilter(tvs);
            var sel = ed.SelectAll(sf);
            if (sel.Value == null) return;
            using (var tr=Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
            {
                var ids = sel.Value.GetObjectIds();
                foreach (var obj in ids)
                {
                    var line = tr.GetObject(obj, OpenMode.ForRead) as Polyline;
                    ed.WriteMessage(line.ObjectId.ToString());
                }
                tr.Commit();
            }
            
        }

        //跳转到某点，用当前视图的大小来设置view的大小，也可以直接指定大小
        [CommandMethod("FLYTOP")]
        public void FlytoPoint()
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var db = Application.DocumentManager.MdiActiveDocument.Database;
            var sel = ed.SelectPrevious();//ed.SelectLast();
            using (Transaction acTrans = db.TransactionManager.StartTransaction())
            {
                if(sel.Value==null)return;
                if(sel.Value.GetObjectIds().Length<1)return;
                var ent=acTrans.GetObject(sel.Value.GetObjectIds()[0], OpenMode.ForRead) as Entity;
                if (ent != null)
                {
                    ViewTableRecord view = new ViewTableRecord();
                    Extents3d ext = ent.GeometricExtents;
                    ext.TransformBy(ed.CurrentUserCoordinateSystem.Inverse());

                    Vector3d vec1 = new Vector3d(new double[] { 10, 10, 10 });
                    ext.MaxPoint.Add(vec1);
                    ext.MinPoint.Subtract(vec1);
                    view.CenterPoint = new Point2d((ext.MaxPoint.X + ext.MinPoint.X) / 2,
                        (ext.MaxPoint.Y + ext.MinPoint.Y) / 2);
                    view.Height = ed.GetCurrentView().Height;
                    view.Width = ed.GetCurrentView().Height;
                    ed.SetCurrentView(view);
                    Application.DocumentManager.MdiActiveDocument.TransactionManager.FlushGraphics();
                }
                acTrans.Commit();
            }

        }
        //选择一个实体对象，取消或者选中返回
        private bool SelectEntity<T>(out T obj, string entityName,bool allclass=false) where T : Entity
        {
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var db = Application.DocumentManager.MdiActiveDocument.Database;
            PromptEntityOptions peo = new PromptEntityOptions(string.Format("\n请选择{0}:", entityName));
            peo.SetRejectMessage(string.Format("\n必须是{0}!", entityName));
            peo.AddAllowedClass(typeof(T), allclass);
            obj = null;
            while (true)
            {
                var selection = ed.GetEntity(peo);
                
                if (selection.Status == PromptStatus.OK)
                {
                    using (var tr = db.TransactionManager.StartTransaction())
                    {
                        obj = tr.GetObject(selection.ObjectId, OpenMode.ForRead) as T;
                        tr.Commit();
                        return true;
                    }
                }
                if (selection.Status == PromptStatus.Cancel)
                {
                    return false;
                }
            }

        }

        //隐藏图层
        [CommandMethod("CTL")]
        public void LayerEnable()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            using (Transaction trs = db.TransactionManager.StartTransaction())
            {
                  LayerTable lt = trs.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
                var layer = trs.GetObject(lt["test"],  OpenMode.ForWrite) as LayerTableRecord;
                layer.IsOff = layer.IsOff ? false : true;

                trs.Commit();
            }
        }

        [CommandMethod("AddArea")]
        public void AddArea()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acadEd = Application.DocumentManager.MdiActiveDocument.Editor;

            PromptPointOptions ptOpt = new PromptPointOptions("选择点:\n");
            PromptPointResult ptRel = null;
            ObjectId[] ids = null;

            ptRel = acadEd.GetPoint(ptOpt);
            Database acCurDb = acDoc.Database;
            //该方法必须是ObjectArx2011以后的版本才支持，以前的版本可以用-bo命令加SelectImplied方法得到的选择集来实现该功能
            var bound = acadEd.TraceBoundary(ptRel.Value, true);

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                foreach (var dbObj in bound)
                {
                    var line = dbObj as Polyline;
                    
                    if (acCurDb != null) line.Color = Color.FromColor(System.Drawing.Color.Red);
                    BlockTable bt = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    var id = btr.AppendEntity(line);//添加实体
                    acTrans.AddNewlyCreatedDBObject(line, true);//保存到cad数据库
                }
                acTrans.Commit();
            }
        }

        [CommandMethod("GMA")]
        public void GetMaxArea()
        {
            var acDoc = Application.DocumentManager.MdiActiveDocument;
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var option = new PromptSelectionOptions();
            option.MessageForAdding = "框选区域";
            var sft = new SelectionFilter(new TypedValue[] { new TypedValue((int)DxfCode.Start, "LWPOLYLINE") });
            var result = ed.GetSelection(option,sft);
            if(result.Status==PromptStatus.Cancel)return;
            if (result.Value == null) return;
            using (var tr = Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
            {
                var reg=new Region();
                var extent = new Extents3d();
                var regColl=new DBObjectCollection();
                foreach (var id in result.Value.GetObjectIds())
                {
                    var ent = tr.GetObject(id, OpenMode.ForRead) as Polyline;
                    if (ent.Closed)
                    {
                        regColl.Add(ent);   
                    }
                    
                }
                if (regColl.Count <= 0) return;
                foreach (var redb in Region.CreateFromCurves(regColl))
                {
                    var r1 = redb as Region;
                    reg.BooleanOperation(BooleanOperationType.BoolUnite, r1);
                }

                var db = Application.DocumentManager.MdiActiveDocument.Database;
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                reg.Color=Color.FromColor(System.Drawing.Color.Magenta);
                LayerTable lt = (LayerTable)tr.GetObject(acDoc.Database.LayerTableId, OpenMode.ForRead);
                reg.Layer = "test";
                btr.AppendEntity(reg);
                tr.AddNewlyCreatedDBObject(reg,true);
                tr.Commit();
                 
            }

        }

        private List<List<Point3d>> getlines(List<Point3d> pts)
        {
            List<List<Point3d>> ptss = new List<List<Point3d>>();
            foreach (var p in pts)
            {
                if (ptss.Count > 0)
                {
                    bool flag = true;
                    foreach (var li in ptss)
                    {
                        if (Math.Abs(li[0].Y-p.Y)<0.5)
                        {
                            li.Add(p);
                            flag = false;
                        }
                    }
                    if (flag)
                    {
                        List<Point3d> templist = new List<Point3d>();
                        templist.Add(p);
                        ptss.Add(templist);
                    }
                }
                else
                {
                    List<Point3d> templist = new List<Point3d>();
                    templist.Add(p);
                    ptss.Add(templist);
                }
            }
            for (int i=0;i<ptss.Count;i++)
            {
                ptss[i] = ptss[i].OrderBy(p => p.X).ToList<Point3d>();
                
            }
            return ptss;
        }
        
        //选择某个图层下特定名称的所有块
         [CommandMethod("SB")]
        public void SelectBlock()
        {

            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            var db = Application.DocumentManager.MdiActiveDocument.Database;
            SelectionFilter filter = new SelectionFilter(new TypedValue[]{new TypedValue((int)DxfCode.Start,"INSERT"),
                   new TypedValue((int)DxfCode.LayerName,"监测")});

            var selection = ed.SelectAll(filter);
            if (selection.Status == PromptStatus.OK)
            {
                using (var tr = db.TransactionManager.StartTransaction())
                {
                    List<Point3d> pts = new List<Point3d>();
                    foreach(var id in selection.Value.GetObjectIds())
                    {
                        var obj = tr.GetObject(id, OpenMode.ForWrite) as BlockReference;
                        if (obj == null||obj.Name.ToLower()!="p") continue;

                        ed.WriteMessage(String.Format("{0},{1},{2:#0.00}\n",obj.Position.X,obj.Position.Y,obj.Position.Z));
                        //obj.Color = Color.FromColor(System.Drawing.Color.Green);
                        Point3d pt = new Point3d(obj.Position.X, obj.Position.Y, obj.Position.Z);
                        pts.Add(pt);
                    }
                    BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    List<List<Point3d>> ptss = getlines(pts);
                    foreach (var list in ptss)
                    {
                        if (list.Count < 2) continue;
                        for(int i=0;i<list.Count-1;i++)
                        {
                            Polyline pl = new Polyline(4);
                            pl.AddVertexAt(0, new Point2d(list[i].X, list[i].Y),0,0,0);
                            pl.AddVertexAt(1, new Point2d(list[i].X, list[i].Y-4), 0, 0, 0);
                            pl.AddVertexAt(2, new Point2d(list[i+1].X, list[i+1].Y-4), 0, 0, 0);
                            pl.AddVertexAt(3, new Point2d(list[i+1].X, list[i+1].Y), 0, 0, 0);
                            pl.Closed = true;
                            pl.Layer = "监测";
                            pl.Color = Color.FromColor(System.Drawing.Color.Green);
                            var id = btr.AppendEntity(pl);//添加实体
                            tr.AddNewlyCreatedDBObject(pl, true);//保存到cad数据库

                            ObjectIdCollection aObjIdColl=new ObjectIdCollection();
                            aObjIdColl.Add(pl.Id);

                            Hatch acHatch=new Hatch();
                            btr.AppendEntity(acHatch);//添加实体
                            tr.AddNewlyCreatedDBObject(acHatch, true);//保存到cad数据库
                            acHatch.SetHatchPattern(HatchPatternType.PreDefined,"ANSI31");
                            acHatch.Associative=true;
                            acHatch.AppendLoop(HatchLoopTypes.Outermost, aObjIdColl);
                            acHatch.EvaluateHatch(true);

                        }
                        
                    }
                    tr.Commit();
                    return;
                }
            }
            if (selection.Status == PromptStatus.Cancel)
            {
                return;
            }
            
        }

        //列举cass编辑后的cad中所有宗地
         [CommandMethod("LISTZD")]
         public void ListAll()
         {
             var ed = Application.DocumentManager.MdiActiveDocument.Editor;
             List<ZDinfo> ZDS = new List<ZDinfo>();

             //添加过滤条件，可以添加类型/图层条件不同的dxfcode对应不同的对象
             var tvszd = new TypedValue[] { new TypedValue((int)DxfCode.Start, "POLYLINE"),
                                                                new TypedValue((int)DxfCode.LayerName,"JZD")};
             var tvshouse = new TypedValue[] { new TypedValue((int)DxfCode.Start, "LWPOLYLINE"),
                                                                new TypedValue((int)DxfCode.LayerName,"JMD")};
             var sf = new SelectionFilter(tvszd);
             var sel = ed.SelectAll(sf);
             if (sel.Value == null) return;
             using (var tr = Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
             {
                 var ids = sel.Value.GetObjectIds();
                 foreach (var obj in ids)
                 {
                     try
                     {
                         //获取宗地
                         var line = tr.GetObject(obj, OpenMode.ForRead) as Polyline2d;
                         ZDinfo zd = getZD(line);
                         //获取宗地内部的所有闭合多段线
                         if (zd == null) continue;
                         var collection=new Point3dCollection();
                         var iter=line.GetEnumerator();
                         while(iter.MoveNext())
                         {
                             ObjectId id = (ObjectId)iter.Current;
                             Vertex2d vtx = tr.GetObject(id, OpenMode.ForRead) as Vertex2d;
                             collection.Add(vtx.Position);
                         }
                        var result = ed.SelectCrossingPolygon(collection, new SelectionFilter(tvshouse));
                        if (result.Status == PromptStatus.OK)
                        {
                            //获取宗地内部的房屋信息
                            //var regColl = new DBObjectCollection();
                            foreach (var id in result.Value.GetObjectIds())
                            {
                                var pline = tr.GetObject(id, OpenMode.ForRead) as Polyline;
                                if (pline == null) continue;
                                //var newline = ent.Clone() as Polyline;
                                //newline.Elevation = 0;
                                //if (newline.Closed)
                                //{
                                //    regColl.Add(newline);
                                //}
                                var house = gethouse(pline);
                                if (house != null)
                                {
                                    zd.Houses.Add(house);
                                }
                            }
                            //用区域内的闭合多边形生成面域可以用来判断有几幢
                            //if (regColl.Count <= 0) continue;
                            //var redbs = Autodesk.AutoCAD.DatabaseServices.Region.CreateFromCurves(regColl);
                            //zd.PartCount = redbs.Count;
                        }
                        string data = String.Format("{0}:{1},{2}", zd.Owner, line.StartPoint.X, line.StartPoint.Y);
                        ed.WriteMessage(data + "\n");
                        //Log(data);
                        zd.Group = zd.Houses.Count.ToString();
                        ZDS.Add(zd);
                     }
                     catch (Exception ex)
                     {
                         Application.ShowAlertDialog(ex.Message);
                         continue;
                     }
                 }
                 tr.Commit();
             }
             Tools.Sort(ZDS);
             FormToolsForCass frm = new FormToolsForCass(ZDS);
             Application.ShowModalDialog(frm);


         }
        //测试记录
         void Log(string data)
         {
             using (StreamWriter sw = new StreamWriter(@"D:\test1.txt", true))
             {
                 sw.WriteLine(data);
             }
         }
        ZDinfo getZD(Polyline2d line )
        {
            try
            {
                ZDinfo zd = new ZDinfo();
                var xzd = line.XData;
                if (xzd == null) return null;
                var bufs = xzd.AsArray();
                zd.Code = bufs[6].Value.ToString() + bufs[2].Value.ToString();
                zd.Owner = bufs[3].Value.ToString();
                zd.Area = line.Area;
                return zd;
            }
            catch { return null; }
        }
        House gethouse(Polyline line)
        {
            if (!line.Closed) return null;
            var xzd = line.XData;
            if (xzd == null) return null;
            var bufs = xzd.AsArray();
            House house = new House();
            house.Area = line.Area;
            string type=bufs[1].Value.ToString();
            switch(type)
            {
                case "141161":
                    house.Structure="混合";
                    if (bufs.Length > 2)
                    {
                        house.FloorCount = int.Parse(bufs[2].Value.ToString());
                    }
                    break;
                case "141121":
                    house.Structure="砖木";
                    if (bufs.Length > 2)
                    {
                        house.FloorCount = int.Parse(bufs[2].Value.ToString());
                    }
                    break;
                case "143130":
                    house.Isbalcony=true;
                    break;
                default:
                    return null;
                    //house.Structure = type;
                    //if (bufs.Length > 2)
                    //{
                    //    house.FloorCount = int.Parse(bufs[2].Value.ToString());
                    //}
                    //break;
            }

            return house;
        }
        [CommandMethod("SELGP")]
         public void SelectGroup()
         {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Editor acadEd = Application.DocumentManager.MdiActiveDocument.Editor;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                var dicgroup = acTrans.GetObject(acCurDb.GroupDictionaryId, OpenMode.ForRead) as DBDictionary;
                foreach (var dic in dicgroup)
                {

                    var group = acTrans.GetObject(dic.Value, OpenMode.ForRead) as Group;
                    if (group == null) continue;
                    //acadEd.WriteMessage(dic.Key + ":" + group.Name + "\n");
                    foreach (var zdid in group.GetAllEntityIds())
                    {
                        var zd = acTrans.GetObject(zdid, OpenMode.ForRead);
                        if (zd.GetType() == typeof(Polyline2d))
                        {
                            var zdinfo = getZD(zd as Polyline2d);
                            acDoc.Editor.WriteMessage("\n" + zdinfo.Owner + "--" + zdinfo.Code);
                        }
                        else if (zd.GetType() ==typeof(BlockReference))
                        {
                            var table = acTrans.GetObject((zd as BlockReference).BlockTableRecord, OpenMode.ForRead) as BlockTableRecord;
                            foreach (var id in table)
                            {
                                var line = acTrans.GetObject(id, OpenMode.ForRead) as Polyline;
                                if (line == null) continue;
                                var xzd = line.XData;
                                if (xzd == null) continue;
                                var bufs = xzd.AsArray();
                                acDoc.Editor.WriteMessage("\n" + bufs[0].TypeCode+":"+bufs[0].Value + "--" + bufs[1].TypeCode+":"+ bufs[1].Value);
                            }
                        }

                    }
                    acDoc.Editor.WriteMessage("\n=================");
                }

            }
        }

        [CommandMethod("listp")]
        public void listPolyline()
        {
            Polyline2d line;
            
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            if (SelectEntity(out line, "多段线"))
            {
                var collection = new Point3dCollection();
                //for (int i = 0; i < line.NumberOfVertices; i++)
                //{
                //    collection.Add(line.GetPoint3dAt(i));
                //}
                using (var tr = Application.DocumentManager.MdiActiveDocument.TransactionManager.StartTransaction())
                {
                    var iter = line.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        ObjectId id = (ObjectId)iter.Current;
                        Vertex2d vtx = tr.GetObject(id, OpenMode.ForRead) as Vertex2d;

                        collection.Add(vtx.Position);
                    }
                  var  tvs = new TypedValue[] { new TypedValue((int)DxfCode.Start, "LWPOLYLINE"),
                                                                new TypedValue((int)DxfCode.LayerName,"JMD")};

                    var result = ed.SelectCrossingPolygon(collection, new SelectionFilter(tvs));
                    if (result.Status == PromptStatus.OK)
                    {
                        foreach (var id in result.Value.GetObjectIds())
                        {
                            var pline = tr.GetObject(id, OpenMode.ForRead) as Polyline;
                            if (pline == null||!pline.Closed) continue;
                            ed.WriteMessage("\n" + pline.ObjectId.ToString());
                            var house = gethouse(pline);
                            if (house != null)
                            {
                                ed.WriteMessage("\n" + house.Area.ToString() + "--" + house.Structure);
                            }
                        }
                    }
                    
                    tr.Commit();
                }
                
            }
        }


        [DllImport("USER32.DLL")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);  //导入模拟键盘的方法
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
  