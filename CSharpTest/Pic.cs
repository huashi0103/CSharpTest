using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace WinDraw
{
    public class Pic
    {
        const int DIMi = 1920;
        const int DIMj = 1080;
        Func<int, int> _sq = x => x * x;
        public Bitmap pic1(int index)
        {
            Bitmap bit = new Bitmap(DIMi, DIMj);
            for (int i = 0; i < DIMi; i++)
            {
                for (int j = 0; j < DIMj; j++)
                {
                    switch (index)
                    {
                        case 1:
                            bit.SetPixel(i, j, GetColor1(i,j));
                            break;
                        case 2:
                            bit.SetPixel(i, j, GetColor2(i, j)); 
                            break;
                        case 3:
                            bit.SetPixel(i, j, GetColor3(i, j)); 
                            break;
                    }
                }
            }
            return bit;
        }


        private Color GetColor1(int i, int j)
        {
            double s = 3d / (j + 99);
            double y = (j + Math.Sin((i * i + _sq(j - 700) * 5) / 100d / DIMi) * 35) * s;
            int rd = ((int)((i + DIMi) * s + y) % 2 + (int)((DIMi * 2 - i) * s + y) % 2) * 127;

            s = 3d / (j + 99);
            y = (j + Math.Sin((i * i + _sq(j - 700) * 5) / 100d / DIMi) * 35) * s;
            int gr = ((int)(5 * ((i + DIMi) * s + y)) % 2 + (int)(5 * ((DIMi * 2 - i) * s + y)) % 2) * 127;

            s = 3d / (j + 99);
            y = (j + Math.Sin((i * i + _sq(j - 700) * 5) / 100d / DIMi) * 35) * s;
            int bl = ((int)(29 * ((i + DIMi) * s + y)) % 2 + (int)(29 * ((DIMi * 2 - i) * s + y)) % 2) * 127;
            return Color.FromArgb(rd, gr, bl);
        }
        private Color GetColor2(int i, int j)
        {
            double x=0,y=0;int k;
            for(k=0;k++<256;)
            {double a=x*x-y*y+(i-768.0)/512;y=2*x*y+(j-512.0)/512;x=a;if(x*x+y*y>4)break;}
            int rd=(int)Math.Log(k)*47;
      
            x=0;y=0;
            for (k = 0; k++ < 256; ) { double a = x * x - y * y + (i - 768.0) / 512; y = 2 * x * y + (j - 512.0) / 512; x = a; if (x * x + y * y > 4)break; }
            int gr = (int)Math.Log(k) * 47;

            x = 0; y = 0; for (k = 0; k++ < 256; ) { double a = x * x - y * y + (i - 768.0) / 512; y = 2 * x * y + (j - 512.0) / 512; x = a; if (x * x + y * y > 4)break; }
            int bl = 128 - (int)Math.Log(k) * 23;
            return Color.FromArgb(rd, gr, bl);

        }
        private Color GetColor3(int i, int j)
        {
            double a=0,b=0,c,d,n=0;
            while((c=a*a)+(d=b*b)<4&&n++<880)
            {
                b=2*a*b+j*8e-9-.645411;
                a=c-d+i*8e-9+.356888;
            }
            int rd = (int)(255 * Math.Pow((n - 80) / 800, 0.5));
            int gr = (int)(150 * Math.Pow((n - 80) / 800, 1));
            int bl = (int)(200 * Math.Pow((n - 80) / 800, 3));
            return Color.FromArgb(255,rd, gr, bl);
        }
    }

    public class PointD
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }
        public PointD(double xx, double yy, double zz=0)
        {
            x = xx;
            z = yy;
            z = zz;
 
        }
    }
    public class CLine
    {
        public CLine(PointD pt1, PointD pt2)
        {
            p1 = pt1;
            p2 = pt2;
        }
        public PointD p1;
        public PointD p2;
        public double Point2LineDistance(PointD pt)
        {
            double a = p2.y - p1.y;
            double b = p1.x - p2.x;
            double c = p2.x * p1.y - p1.x * p2.y;

            double dis = Math.Abs(a * pt.x + b * pt.y + c) / Math.Sqrt(a * a + b * b);

	        return dis;
        }

    }
    public class Triangle
    {
       
        	public PointD p1;
	        public PointD p2;
	        public PointD p3;

	        public CLine l1;
	        public CLine l2;
	        public CLine l3;
        	public PointD center;
	        public double  radiu;

            public Triangle(PointD pt1, PointD pt2, PointD pt3)
            {	
	            p1 = pt1;
	            p2 = pt2;
	            p3 = pt3;

	            l1 = new CLine(p1,p2);
	            l2 = new CLine(p2,p3);
	            l3 = new CLine(p3,p1);

	            double dis1 = distance(p1,p2);
                double dis2 = distance(p2, p3);
                double dis3 = distance(p3, p1);

	            radiu = dis1*dis2*dis3/TriangleArea()/4;

                double c1, c2;
                double xA, yA, xB, yB, xC, yC;  

	            xA = p1.x; yA = p1.y;   
	            xB = p2.x; yB = p2.y;   
	            xC = p3.x; yC = p3.y;   
	            c1 = (xA * xA + yA * yA - xB * xB - yB * yB) / 2;   
	            c2 = (xA * xA + yA * yA - xC * xC - yC * yC) / 2;   

	            center.x = (c1 * (yA - yC) - c2 * (yA - yB)) /    
		            ((xA - xB) * (yA - yC) - (xA - xC) * (yA - yB));    
	            center.y = (c1 * (xA - xC) - c2 * (xA - xB)) /    
		            ((yA - yB) * (xA - xC) - (yA - yC) * (xA - xB));    

            }

            public double TriangleArea()
            {
	            return Math.Abs(p1.x * p2.y + p2.x * p3.y    
		            + p3.x * p1.y - p2.x * p1.y   
		            - p3.x * p2.y - p1.x * p3.y) / 2; 
            }

           public bool CheckInCircle(PointD pt)
            {
	            if (distance(center,pt) <= radiu)
	            {
		            return true;
	            }
	            return false;
            }

           public CLine FindNearestLine(PointD pt)
            {
	            double dis1 = l1.Point2LineDistance(pt);
	            double dis2 = l2.Point2LineDistance(pt);
	            double dis3 = l3.Point2LineDistance(pt);

	            if (dis1 <= dis2 && dis1 <= dis3)
	            {
		            return l1;
	            }
	            if (dis2 <= dis1 && dis2 <= dis3)
	            {
		            return l2;
	            }

	            return l3;
            }

            public CLine FindCommonLine(Triangle tg)
            {
	            if (this.l1 == tg.l1 || this.l1 == tg.l2 || this.l1 == tg.l3)
	            {
		            return  l1;
	            }

	            if (this.l2 == tg.l1 || this.l2 == tg.l2 || this.l2 == tg.l3)
	            {
		            return l2;
	            }

	            if (this.l3 == tg.l1 || this.l3 == tg.l2 || this.l3 == tg.l3)
	            {
		            return l3;
	            }

	            return null;
            }

            public PointD GetOtherPoint(PointD pt1, PointD pt2)
            {
	            if (!(p1 == pt1) && !(p1 == pt2))
	            {
		            return p1;
	            }

	            if (!(p2 == pt1) && !(p2 == pt2))
	            {
		            return p2;
	            }

	            return p3;
            }

            public bool CheckPointExist(PointD pt)
            {
	            if (pt == p1 || pt == p2 || pt == p3)
	            {
		            return true;
	            }

	            return false;
            }

           public override bool  Equals(object o)
            {
                var t = o as Triangle;
                if (t == null) return false;
	            if ((p1 == t.p1) && (p2 == t.p2) && (p3 == t.p3))
	            {
		            return true;
	            }
	            if ((p1 == t.p1) && (p3 == t.p2) && (p2 == t.p3))
	            {
		            return true;
	            }

	            if ((p2 == t.p1) && (p1 == t.p2) && (p3 == t.p3))
	            {
		            return true;
	            }
	            if ((p2 == t.p1) && (p3 == t.p2) && (p1 == t.p3))
	            {
		            return true;
	            }

	            if ((p3 == t.p1) && (p2 == t.p2) && (p1 == t.p3))
	            {
		            return true;
	            }
	            if ((p3 == t.p1) && (p1 == t.p2) && (p2 == t.p3))
	            {
		            return true;
	            }

	            return false;
            }

           private double distance(PointD p1, PointD p2)
           {
               return Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
           }
    }
   /* 
    public class CBowyerWatson
    {
	    List<PointD>	m_lstBowyerWatsonPointList;
	    List<CLine>		m_lstBowyerWatsonLineList;
	    List<Triangle>	m_lstBowyerWatsonTriangleList;
	    List<PointD>	m_lstAddPointList;
	    PointD[] mHelperPoints=new PointD[4];
	    static CBowyerWatson  m_pBowyerWatson;
	    bool	m_bUpdateDrawFlag;
        static	CBowyerWatson GetInstance()
        {
            	if( !m_pBowyerWatson )
		            m_pBowyerWatson = new CBowyerWatson;
	            return m_pBowyerWatson;
        }

	    void	ClearBowyerWatson()
        {
            m_bUpdateDrawFlag = false;

	        
        }

	    void		CreateHelperPoint(PointD pt1, PointD pt2, PointD pt3, PointD pt4)
        {
            mHelperPoints[0] = pt1;
	        mHelperPoints[1] = pt2;
	        mHelperPoints[2] = pt3;
	        mHelperPoints[3] = pt4;

	        //加入辅助点4个
	        AddBowyerWatsonPoint(pt1);
	        AddBowyerWatsonPoint(pt2);
	        AddBowyerWatsonPoint(pt3);
	        AddBowyerWatsonPoint(pt4);

	        //加入辅助窗体的5条边
	        CLine line1 = CLine(pt1,pt2);
	        CLine line2 = CLine(pt2,pt3);
	        CLine line3 = CLine(pt3,pt4);
	        CLine line4 = CLine(pt4,pt1);
	        CLine line5 = CLine(pt2,pt4);
	        AddBowyerWatsonLine(line1);
	        AddBowyerWatsonLine(line2);
	        AddBowyerWatsonLine(line3);
	        AddBowyerWatsonLine(line4);
	        AddBowyerWatsonLine(line5);

	        //加入辅助三角形2个
	        CTriangle tg1 = CTriangle(pt1,pt2,pt4);
	        CTriangle tg2 = CTriangle(pt2,pt3,pt4);
	        AddBowyerWatsonTriangle(tg1);
	        AddBowyerWatsonTriangle(tg2);
        }

	    void		AddNewPoint(PointD pt)
        {
            	bool existflag = false;
	            std::list<CPoint*>::iterator iter_point = m_lstAddPointList.begin();
	            for ( ;iter_point != m_lstAddPointList.end();iter_point++)
	            {
		            if (pt == (**iter_point))
		            {
			            existflag = true;
		            }
	            }

	            if (!existflag)
	            {
		            CPoint* newPoint = new CPoint(pt.x,pt.y);
		            m_lstAddPointList.push_back(newPoint);
	            }
        }
	    void		UpdateNewPoint()
        {
            std::list<CPoint*>::iterator iter_point = m_lstAddPointList.begin();
	        while (iter_point != m_lstAddPointList.end())
	        {
		        ProcessNewPoint(**iter_point);

		        std::list<CPoint*>::iterator iter_pointNext = iter_point;
		        iter_pointNext++;

		        SAFE_DELETE(*iter_point);
		        m_lstAddPointList.erase(iter_point);

		        iter_point = iter_pointNext;
	        }//Point

	        //剔除辅助边
	        std::list<CLine*>::iterator iter = m_lstBowyerWatsonLineList.begin();
	        while(iter != m_lstBowyerWatsonLineList.end())
	        {
		        CLine line = (**iter);
		        if (line.CheckPointExist(mHelperPoints[0]) || line.CheckPointExist(mHelperPoints[1]) || \
			        line.CheckPointExist(mHelperPoints[2]) || line.CheckPointExist(mHelperPoints[3]))
		        {
			        std::list<CLine*>::iterator iter_next = iter;
			        iter_next++;
			        SAFE_DELETE(*iter);
			        m_lstBowyerWatsonLineList.erase(iter);

			        iter = iter_next;
		        }
		        else{
			        iter++;
		        }
	        }

	        //剔除辅助三角形
	        std::list<CTriangle*>::iterator iter_triangle = m_lstBowyerWatsonTriangleList.begin();
	        while(iter_triangle != m_lstBowyerWatsonTriangleList.end())
	        {
		        CTriangle triangle = (**iter_triangle);
		        if (triangle.CheckPointExist(mHelperPoints[0]) || triangle.CheckPointExist(mHelperPoints[1]) || \
			        triangle.CheckPointExist(mHelperPoints[2]) || triangle.CheckPointExist(mHelperPoints[3]))
		        {
			        std::list<CTriangle*>::iterator iter_nextTriangle = iter_triangle;
			        iter_nextTriangle++;
			        SAFE_DELETE(*iter_triangle);
			        m_lstBowyerWatsonTriangleList.erase(iter_triangle);

			        iter_triangle = iter_nextTriangle;
		        }
		        else{
			        iter_triangle++;
		        }
	        }
        }

	    void		AddBowyerWatsonPoint(PointD pt)
        {
	        bool existflag = false;
	        std::list<CPoint*>::iterator iter_point = m_lstBowyerWatsonPointList.begin();
	        for ( ;iter_point != m_lstBowyerWatsonPointList.end();iter_point++)
	        {
		        if (pt == (**iter_point))
		        {
			        existflag = true;
		        }
	        }

	        if (!existflag)
	        {
		        CPoint* newPoint = new CPoint(pt.x,pt.y);
		        m_lstBowyerWatsonPointList.push_back(newPoint);
	        }

        }

	    void		AddBowyerWatsonLine(CLine line)
        {
            	bool existflag = false;
	            std::list<CLine*>::iterator iter_line = m_lstBowyerWatsonLineList.begin();
	            for ( ;iter_line != m_lstBowyerWatsonLineList.end();iter_line++)
	            {
		            if (line == (**iter_line))
		            {
			            existflag = true;
		            }
	            }

	            if (!existflag)
	            {
		            CLine* newLine = new CLine(line.p1,line.p2);
		            m_lstBowyerWatsonLineList.push_back(newLine);
	            }
        }
	    void		DelBowyerWatsonLine(CLine line)
        {
            	std::list<CLine*>::iterator iter_line =	m_lstBowyerWatsonLineList.begin();
	            while (iter_line != m_lstBowyerWatsonLineList.end())
	            {
		            if (line == (**iter_line))
		            {
			            SAFE_DELETE(*iter_line);
			            m_lstBowyerWatsonLineList.erase(iter_line);
			            break;
		            }
		            else
			            iter_line++;
	            }//line

	            std::list<CTriangle*>::iterator iter_Triangle =	m_lstBowyerWatsonTriangleList.begin();
	            while (iter_Triangle != m_lstBowyerWatsonTriangleList.end())
	            {
		            if ((*iter_Triangle)->l1 == line || (*iter_Triangle)->l2 == line || (*iter_Triangle)->l3 == line )
		            {
			            SAFE_DELETE(*iter_Triangle);
			            m_lstBowyerWatsonTriangleList.erase(iter_Triangle);
			            break;
		            }
		            else
			            iter_Triangle++;
	            }//Triangle
        }

	    void		AddBowyerWatsonTriangle(CTriangle triangle)
        {
            	bool existflag = false;
	            std::list<CTriangle*>::iterator iter_Triangle = m_lstBowyerWatsonTriangleList.begin();
	            for ( ;iter_Triangle != m_lstBowyerWatsonTriangleList.end();iter_Triangle++)
	            {
		            if (triangle == (**iter_Triangle))
		            {
			            existflag = true;
		            }
	            }

	            if (!existflag)
	            {
		            CTriangle* newTriangle = new CTriangle(triangle.p1,triangle.p2,triangle.p3);
		            m_lstBowyerWatsonTriangleList.push_back(newTriangle);
	            }
        }
	    void		DelBowyerWatsonTriangle(CTriangle triangle)
        {
            	std::list<CTriangle*>::iterator iter_Triangle =	m_lstBowyerWatsonTriangleList.begin();
	            while (iter_Triangle != m_lstBowyerWatsonTriangleList.end())
	            {
		            if (triangle == (**iter_Triangle))
		            {
			            SAFE_DELETE(*iter_Triangle);
			            m_lstBowyerWatsonTriangleList.erase(iter_Triangle);
			            return;
		            }
		            else
			            iter_Triangle++;
	            }//line
        }

	    void		ProcessNewPoint(PointD pt)
        {
            std::list<CLine*>	lineList ;
	        std::list<CTriangle*> triangleList;
	        std::vector<CTriangle*> commonTriangleVector;

	        std::list<CLine*>::iterator iter_line =	m_lstBowyerWatsonLineList.begin();
	        for(;iter_line != m_lstBowyerWatsonLineList.end();iter_line++)
	        {
		        CLine* newline = new CLine();
		        memcpy(newline, *iter_line, sizeof(CLine));

		        lineList.push_back(newline);
	        }
	        std::list<CTriangle*>::iterator iter_triangle =	m_lstBowyerWatsonTriangleList.begin();
	        for(;iter_triangle != m_lstBowyerWatsonTriangleList.end();iter_triangle++)
	        {
		        CTriangle* newtriangle = new CTriangle();
		        memcpy(newtriangle, *iter_triangle, sizeof(CTriangle));

		        triangleList.push_back(newtriangle);
	        }

	        iter_triangle = triangleList.begin();
	        while (iter_triangle != triangleList.end())
	        {
		        //是否存在三角形外接圆内
		        if ((*iter_triangle)->CheckInCircle(pt))
		        {
			        commonTriangleVector.push_back(*iter_triangle);
		        }
		        iter_triangle++;
	        }// triangle

	        if (commonTriangleVector.size() == 1)
	        {
		        std::vector<CTriangle*>::iterator iter_v =	commonTriangleVector.begin();

		        ////////////////////////////////
		        //删除三角形
		        DelBowyerWatsonTriangle(**iter_v);

		        /////////////////////////////////
		        //连接三角形三点
		        CLine line1 = CLine(pt,(*iter_v)->p1);
		        CLine line2 = CLine(pt,(*iter_v)->p2);
		        CLine line3 = CLine(pt,(*iter_v)->p3);
		        AddBowyerWatsonLine(line1);
		        AddBowyerWatsonLine(line2);
		        AddBowyerWatsonLine(line3);

		        //加入新三角形
		        if (CheckTriangleLinesExist(pt, (*iter_v)->p1, (*iter_v)->p2))
		        {
			        CTriangle tg1 = CTriangle(pt,(*iter_v)->p1,(*iter_v)->p2);
			        AddBowyerWatsonTriangle(tg1);
		        }
		        if (CheckTriangleLinesExist(pt, (*iter_v)->p2, (*iter_v)->p3))
		        {
			        CTriangle tg2 = CTriangle(pt,(*iter_v)->p2,(*iter_v)->p3);
			        AddBowyerWatsonTriangle(tg2);
		        }
		        if (CheckTriangleLinesExist(pt, (*iter_v)->p3, (*iter_v)->p1))
		        {
			        CTriangle tg3 = CTriangle(pt,(*iter_v)->p3,(*iter_v)->p1);
			        AddBowyerWatsonTriangle(tg3);
		        }
	        }

	        if (commonTriangleVector.size() > 1)
	        {
		        for (int i = 0;i < (commonTriangleVector.size()-1);i++)
		        {
			        for (int j = i+1;j <commonTriangleVector.size();j++)
			        {
				        CTriangle* trg1 =	*(commonTriangleVector.begin() + i);
				        CTriangle* trg2 =	*(commonTriangleVector.begin() +j);

				        CLine* commonLine = trg1->FindCommonLine(*trg2);
				        if (commonLine != NULL)
				        {
					        ////////////////////////////////
					        //删除影响三角形
					        DelBowyerWatsonTriangle(*trg1);
					        DelBowyerWatsonTriangle(*trg2);

					        //删除公共边
					        DelBowyerWatsonLine(*commonLine);

					        /////////////////////////////////
					        //连接三角形三点
					        CLine line1_1 = CLine(pt,trg1->p1);
					        CLine line1_2 = CLine(pt,trg1->p2);
					        CLine line1_3 = CLine(pt,trg1->p3);
					        CLine line2_1 = CLine(pt,trg2->p1);
					        CLine line2_2 = CLine(pt,trg2->p2);
					        CLine line2_3 = CLine(pt,trg2->p3);

					        AddBowyerWatsonLine(line1_1);
					        AddBowyerWatsonLine(line1_2);
					        AddBowyerWatsonLine(line1_3);
					        AddBowyerWatsonLine(line2_1);
					        AddBowyerWatsonLine(line2_2);
					        AddBowyerWatsonLine(line2_3);

					        //加入新三角形
					        if (CheckTriangleLinesExist(pt, trg1->p1, trg1->p2))
					        {
						        CTriangle tg1 = CTriangle(pt, trg1->p1, trg1->p2);
						        AddBowyerWatsonTriangle(tg1);
					        }
					        if (CheckTriangleLinesExist(pt, trg1->p2, trg1->p3))
					        {
						        CTriangle tg2 = CTriangle(pt,trg1->p2,trg1->p3);
						        AddBowyerWatsonTriangle(tg2);
					        }
					        if (CheckTriangleLinesExist(pt, trg1->p3, trg1->p1))
					        {
						        CTriangle tg3 = CTriangle(pt, trg1->p3, trg1->p1);
						        AddBowyerWatsonTriangle(tg3);
					        }

					        if (CheckTriangleLinesExist(pt, trg2->p1, trg2->p2))
					        {
						        CTriangle tg1 = CTriangle(pt, trg2->p1, trg2->p2);
						        AddBowyerWatsonTriangle(tg1);
					        }
					        if (CheckTriangleLinesExist(pt, trg2->p2, trg2->p3))
					        {
						        CTriangle tg2 = CTriangle(pt,trg2->p2,trg2->p3);
						        AddBowyerWatsonTriangle(tg2);
					        }
					        if (CheckTriangleLinesExist(pt, trg2->p3, trg2->p1))
					        {
						        CTriangle tg3 = CTriangle(pt, trg2->p3, trg2->p1);
						        AddBowyerWatsonTriangle(tg3);
					        }

				        }
			        }
		        }
	        }

	        AddBowyerWatsonPoint(pt);

	        iter_line =	lineList.begin();
	        while (iter_line != lineList.end())
	        {
		        std::list<CLine*>::iterator iter_lineNext = iter_line;
		        iter_lineNext++;

		        SAFE_DELETE(*iter_line);
		        lineList.erase(iter_line);

		        iter_line = iter_lineNext;
	        }//line

	        iter_triangle =	triangleList.begin();
	        while (iter_triangle != triangleList.end())
	        {
		        std::list<CTriangle*>::iterator iter_triangleNext = iter_triangle;
		        iter_triangleNext++;

		        SAFE_DELETE(*iter_triangle);
		        triangleList.erase(iter_triangle);

		        iter_triangle = iter_triangleNext;
	        }//Triangle
        }
	    bool		CheckTriangleLinesExist(PointD pt1, PointD pt2, PointD pt3)
        {
            bool exist_line1 = false;
	        bool exist_line2 = false;
	        bool exist_line3 = false;

	        CLine line1 = CLine(pt1, pt2);
	        CLine line2 = CLine(pt2, pt3);
	        CLine line3 = CLine(pt3, pt1);

	        std::list<CLine*>::iterator iter_line = m_lstBowyerWatsonLineList.begin();
	        for ( ;iter_line != m_lstBowyerWatsonLineList.end();iter_line++)
	        {
		        if (line1 == (**iter_line))
		        {
			        exist_line1 = true;
			        continue;
		        }
		        if (line2 == (**iter_line))
		        {
			        exist_line2 = true;
			        continue;
		        }
		        if (line3 == (**iter_line))
		        {
			        exist_line3 = true;
		        }
	        }

	        if (exist_line1 && exist_line2 && exist_line3)
	        {
		        return true;
	        }

	        return false;
        }

	    void		DrawMesh()
        {
            std::list<CLine*>::iterator iter = m_lstBowyerWatsonLineList.begin(); 
	        for ( ;iter != m_lstBowyerWatsonLineList.end();iter++)
	        {
		        //(*iter)->p1.x, (*iter)->p1.y
		        //(*iter)->p2.x, (*iter)->p2.y
	        }
        }

	    //void		SetUpdateDrawFlag(bool flag){m_bUpdateDrawFlag = flag;};
	    void		Update()
        {
            	 if (m_bUpdateDrawFlag)
	            {
		            DrawMesh();
	            }
        }

	    public  List<CLine> BowyerWatsonLines(){return m_lstBowyerWatsonLineList;};
	    const List<Triangle>& GetBowyerWatsonTriangles(){return m_lstBowyerWatsonTriangleList;};
    }
*/
}
