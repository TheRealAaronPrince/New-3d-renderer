using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using plotter_2D;

public class projection
{
	private int TriangleBounds = 3;
	public Plotter Plotter;
	private bool loaded = false;
	public double[][] tris;
	private int count;
	public double camX = 0, camY = 0, camZ = 0;
	public double rotX = 0, rotY = 0, rotZ = 0;
	public double angX = 0, angY = 0, angZ = 0;
	private double aspect = 0;
	private double nearClip = 0.001f, farClip = 3f;
	private double scale = 2;
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
		Plotter.render.clearZBuff(farClip*4f);
		Parallel.For(0,count,w => { projectTri(w);});
		tris = tris.OrderByDescending(entry => entry[9]).ToArray();
		Parallel.For(0,count, i => {drawTri(i,fill);});
		//for(int i = 0; i < count; i++){drawTri(i,fill);}
		Plotter.render.SetBackground(0,0,0,farClip*4f);
		Plotter.render.update();
	}
	private void projectTri(int face = 0)
	{
		var position =  new Tuple<double,double,double>(camX,camY,camZ);
		//defining tuples for ease of passing data between functions
		var point1 = import.vert[import.tri[face].Item1-1];
		var point2 = import.vert[import.tri[face].Item2-1];
		var point3 = import.vert[import.tri[face].Item3-1];
		//transforming the 3d space into a 2d projection
		var transformA = rotate(rotate(rotate(point1,5),4),3);
		var transformB = rotate(rotate(rotate(point2,5),4),3);
		var transformC = rotate(rotate(rotate(point3,5),4),3);
		var camA = rotate(rotate(rotate(Vector.vecSub(transformA,position),2),1),0);
		var camB = rotate(rotate(rotate(Vector.vecSub(transformB,position),2),1),0);
		var camC  =rotate(rotate(rotate(Vector.vecSub(transformC,position),2),1),0);
		var projA = perspective(camA);
		var projB = perspective(camB);
		var projC = perspective(camC);
		//calculating the normal vector
		var camAB = Vector.vecSub(projB,projA);
		var camAC = Vector.vecSub(projC,projA);
		var camN = Vector.unit(Vector.vecCross(camAB,camAC));
		var lumAB = Vector.vecSub(transformB,transformA);
		var lumAC = Vector.vecSub(transformC,transformA);
		var lumN = Vector.unit(Vector.vecCross(lumAB,lumAC));
		var luma = Vector.unit(new Tuple<double,double,double>(-2,5,3));
		double D = Vector.vecDot(lumN,luma);
		var direction = Vector.vecSub(Vector.centroid(camA,camB,camC),position);
		double distance = Vector.centroid(camA,camB,camC).Item3;
		double K = Vector.vecDot(Vector.unit(direction),camN);
		tris[face] = new double[] {
			projA.Item1,
			projA.Item2,
			projA.Item3,
			projB.Item1,
			projB.Item2,
			projB.Item3,
			projC.Item1,
			projC.Item2,
			projC.Item3,
			distance,
			K,
			D,
			face};
	}
	private void drawTri(int face = 0,int fill = 0)
	{
		int index = Convert.ToInt32(tris[face][12]);
		if(tris[face][11] > 0)
		{
			tris[face][11] = 0;
		}
		//rescalling from a normalized space to screen space
		var pointA = new Tuple<long,long,double>(Convert.ToInt32((tris[face][0]+(scale/2))*(Plotter.Pwidth/scale)),Convert.ToInt32((tris[face][1]+(scale/2))*(Plotter.Pheight/scale)),tris[face][2]);
		var pointB = new Tuple<long,long,double>(Convert.ToInt32((tris[face][3]+(scale/2))*(Plotter.Pwidth/scale)),Convert.ToInt32((tris[face][4]+(scale/2))*(Plotter.Pheight/scale)),tris[face][5]);
		var pointC = new Tuple<long,long,double>(Convert.ToInt32((tris[face][6]+(scale/2))*(Plotter.Pwidth/scale)),Convert.ToInt32((tris[face][7]+(scale/2))*(Plotter.Pheight/scale)),tris[face][8]);
		//coloring
		double colorMult = (Math.Abs(tris[face][11]));
		double R = import.material[import.tri[index].Item7][0];
		double G = import.material[import.tri[index].Item7][1];
		double B = import.material[import.tri[index].Item7][2];
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
		double faceR = R*255;
		double faceG = G*255;
		double faceB = B*255;
		double edgeR = 128;//255-faceR;
		double edgeG = 128;//255-faceG;
		double edgeB = 128;//255-faceB;
		//backface culling
		if(tris[face][7] < 1)
		{
			//checking if triangle is within screen bounds
			if(tris[face][9] > nearClip && tris[face][9] < farClip && Math.Abs(tris[face][0]) < TriangleBounds && Math.Abs(tris[face][1]) < TriangleBounds && Math.Abs(tris[face][3]) < TriangleBounds && Math.Abs(tris[face][4]) < TriangleBounds && Math.Abs(tris[face][6]) < TriangleBounds && Math.Abs(tris[face][7]) < TriangleBounds)
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
	private Tuple<double,double,double> perspective(Tuple<double,double,double> point)
	{
		double X = point.Item1;
		double Y = point.Item2;
		double Z = point.Item3;
		aspect = (double)(Convert.ToDouble(Plotter.Pheight)/Convert.ToDouble(Plotter.Pwidth));
		double angle = (double)(fov * (Math.PI/180));
		double projectConst = (double)(1/(Math.Tan(angle/2)));
		double Xtrans = 0;
		double Ytrans = 0;
		double Xtemp = ((X)*aspect*projectConst);
		double Ytemp = ((Y)*projectConst);
		Xtrans = Xtemp/(Z);
		Ytrans = Ytemp/(Z);
		var output = new Tuple<double,double,double>(Xtrans,Ytrans,Z);
		return output;
	}
	private double degToRad(double deg)
	{
		return (deg/180f)*(double)Math.PI;
	}
	private Tuple<double,double,double> rotate(Tuple<double,double,double> point, int axis)
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