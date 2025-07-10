using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using plotter_2D;

public class projection
{
	private int TriangleBounds = 2;
	public Plotter Plotter;
	private bool loaded = false;
	public double[][] tris;
	private int count;
	public double camX = 0, camY = 0, camZ = 0;
	public double rotX = 0, rotY = 0, rotZ = 0;
	public double angX = 0, angY = 0, angZ = 0;
	private double aspect = 0;
	private double nearClip = 0f, farClip = 3f;
	public double fov = 45;
	public import import = new import();
	public Vector Vector = new Vector();
	public projection(int w, int h)
	{
		Plotter = new Plotter(w,h);
	}
	public void drawObj(string obj = "", int fill = 0)
	{
		if(!loaded)
		{
			import.import_obj(obj);
			count = import.tri.Length;
			tris = new double[count][];
			loaded = true;
		}
		Plotter.render.SetBackground(0,0,0,farClip*2f);
		//Parallel.For(0,count,w => { projectTri(w);});
		for(int w = 0; w < count; w++){projectTri(w);}
		Parallel.For(0,count, i => {drawTri(i,fill);});
		//for(int i = 0; i < count; i++){drawTri(i,fill);}
		Plotter.render.update();
	}
	private void projectTri(int face = 0)
	{
		double[] position =  {camX,camY,camZ};
		//defining tuples for ease of passing data between functions
		double[] point1 = TupleToArray(import.vert[import.tri[face].Item1-1]);
		double[] point2 = TupleToArray(import.vert[import.tri[face].Item2-1]);
		double[] point3 = TupleToArray(import.vert[import.tri[face].Item3-1]);
		//transforming the 3d space into a 2d projection
		double[] transformA = rotate(rotate(rotate(point1,5),4),3);
		double[] transformB = rotate(rotate(rotate(point2,5),4),3);
		double[] transformC = rotate(rotate(rotate(point3,5),4),3);
		double[] camA = rotate(rotate(rotate(Vector.vecSub(transformA,position),2),1),0);
		double[] camB = rotate(rotate(rotate(Vector.vecSub(transformB,position),2),1),0);
		double[] camC  =rotate(rotate(rotate(Vector.vecSub(transformC,position),2),1),0);
		double[] projA = perspective(camA);
		double[] projB = perspective(camB);
		double[] projC = perspective(camC);
		//calculating the normal vector
		double[] camAB = Vector.vecSub(projB,projA);
		double[] camAC = Vector.vecSub(projC,projA);
		double[] camN = Vector.normalize(Vector.vecCross(camAB,camAC));
		double[] lumAB = Vector.vecSub(transformB,transformA);
		double[] lumAC = Vector.vecSub(transformC,transformA);
		double[] lumN = Vector.normalize(Vector.vecCross(lumAB,lumAC));
		double[] luma = Vector.normalize(new double[] {-2,5,3});
		double D = Vector.vecDot(lumN,luma);
		double[] direction = Vector.vecSub(Vector.centroid(camA,camB,camC),position);
		double distance = Vector.centroid(camA,camB,camC)[2];
		double K = Vector.vecDot(Vector.normalize(direction),camN);
		tris[face] = new double[] {
			projA[0],
			projA[1],
			projA[2],
			projB[0],
			projB[1],
			projB[2],
			projC[0],
			projC[1],
			projC[2],
			distance,
			K,
			D,
			face};
	}
	private double[] TupleToArray(Tuple<double,double,double> T)
	{
		return new double[] {T.Item1,T.Item2,T.Item3};
	}
	private void drawTri(int face = 0,int fill = 0)
	{
		int index = Convert.ToInt32(tris[face][12]);
		if(tris[face][11] > 0)
		{
			tris[face][11] = 0;
		}
		//rescalling from a normalized space to screen space
		var pointA = new Tuple<long,long,double>(Convert.ToInt32((tris[face][0]+1)*(Plotter.Pwidth/2)),Convert.ToInt32((tris[face][1]+1)*(Plotter.Pheight/2)),tris[face][2]);
		var pointB = new Tuple<long,long,double>(Convert.ToInt32((tris[face][3]+1)*(Plotter.Pwidth/2)),Convert.ToInt32((tris[face][4]+1)*(Plotter.Pheight/2)),tris[face][5]);
		var pointC = new Tuple<long,long,double>(Convert.ToInt32((tris[face][6]+1)*(Plotter.Pwidth/2)),Convert.ToInt32((tris[face][7]+1)*(Plotter.Pheight/2)),tris[face][8]);
		//coloring
		double colorMult = (Math.Abs(tris[face][11]));
		double R = Math.Sqrt(import.material[import.tri[index].Item7][0]);
		double G = Math.Sqrt(import.material[import.tri[index].Item7][1]);
		double B = Math.Sqrt(import.material[import.tri[index].Item7][2]);
		if(R > 1)
		{
			R = 1;
		}
		if(G > 1)
		{
			G = 1;
		}
		if(B > 1)
		{
			B = 1;
		}
		int faceR = (int)(R*255);
		int faceG = (int)(G*255);
		int faceB = (int)(B*255);
		int edgeR = 255;
		int edgeG = 255;
		int edgeB = 255;
		//backface culling
		if(tris[face][10] < 1)
		{
			//checking if triangle is within screen bounds
			if(tris[face][2] > 0 && tris[face][5] > 0 && tris[face][8] > 0 && tris[face][9] > nearClip && tris[face][9] < farClip && Math.Abs(tris[face][0]) < TriangleBounds && Math.Abs(tris[face][1]) < TriangleBounds && Math.Abs(tris[face][3]) < TriangleBounds && Math.Abs(tris[face][4]) < TriangleBounds && Math.Abs(tris[face][6]) < TriangleBounds && Math.Abs(tris[face][7]) < TriangleBounds)
			{
				if(fill > 0)
				{
					Plotter.triangle(pointA,pointB,pointC,faceR,faceG,faceB,colorMult,import.material[import.tri[index].Item7][3],fill,import.texture[import.tri[index].Item7],import.txCoord[import.tri[index].Item4-1],import.txCoord[import.tri[index].Item5-1],import.txCoord[import.tri[index].Item6-1]);
				}
				else
				{
					Plotter.line(pointA,pointB,edgeR,edgeG,edgeB);
					Plotter.line(pointA,pointC,edgeR,edgeG,edgeB);
					Plotter.line(pointB,pointC,edgeR,edgeG,edgeB);
				}
			}
		}
	}
	private double metalic(double x, double k = 0)
	{
		double g = 2*x-(1+(k/2));
		double y = (double)((g+k*g)/(2*(-k+(2*k*Math.Abs(g))+1))+0.5);
		if(y < 0)
		{
			y = 0;
		}
		return y;
	}
	private double[] perspective(double[] point)
	{
		double X = point[0];
		double Y = point[1];
		double Z = point[2];
		aspect = (double)(Convert.ToDouble(Plotter.Pheight)/Convert.ToDouble(Plotter.Pwidth));
		double angle = (double)(fov * (Math.PI/180));
		double projectConst = (double)(1/(Math.Tan(angle/2)));
		double Xtrans = 0;
		double Ytrans = 0;
		double Xtemp = ((X)*aspect*projectConst);
		double Ytemp = ((Y)*projectConst);
		Xtrans = Xtemp/(Z);
		Ytrans = Ytemp/(Z);
		double[] output = {Xtrans,Ytrans,Z};
		return output;
	}
	private double degToRad(double deg)
	{
		return (deg/180f)*(double)Math.PI;
	}
	private double[] rotate(double[] point, int axis)
	{
		double angle;
		switch (axis)
		{
			case 0:
				angle = degToRad(rotX);
				break;
			case 1:
				angle = degToRad(rotY);
				break;
			case 2:
				angle = degToRad(rotZ);
				break;
			case 3:
				angle = degToRad(angX);
				break;
			case 4:
				angle = degToRad(angY);
				break;
			case 5:
				angle = degToRad(angZ);
				break;
			default:
				angle = 0f;
				break;
		}
		var output = Vector.matVecMult(point,Vector.rotMatrix(angle,axis%3));
		return output;
	}
}