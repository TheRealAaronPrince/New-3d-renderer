/*
 * Created by SharpDevelop.
 * User: princ
 * Date: 27/07/2024
 * Time: 15:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

public class Vector
{
	public double[,] rotMatrix(double theta, int axis)
	{
		double[,] matrix;
		switch(axis)
		{
			case 0:
			{
				matrix = new double[,]
				{{1,0,0},{0,(double)Math.Cos(theta),-(double)Math.Sin(theta)},{0,(double)Math.Sin(theta),(double)Math.Cos(theta)}};
				break;
			}
			case 1:
			{
				matrix = new double[,]
				{{(double)Math.Cos(theta),0,(double)Math.Sin(theta)},{0,1,0},{-(double)Math.Sin(theta),0,(double)Math.Cos(theta)}};
				break;
			}
			case 2:
			{
				matrix = new double[,]
				{{(double)Math.Cos(theta),-(double)Math.Sin(theta),0},{(double)Math.Sin(theta),(double)Math.Cos(theta),0},{0,0,1}};
				break;
			}
			default:
			{
				matrix = new double[,]
				{{1,0,0},{0,1,0},{0,0,1}};
				break;
			}
		}
		return matrix;
	}
	public Tuple<double,double,double> matVecMult(Tuple<double,double,double> point, double[,] matrix)
	{
		double X = point.Item1;
		double Y = point.Item2;
		double Z = point.Item3;
		double Xtrans = (matrix[0,0]*X)+(matrix[0,1]*Y)+(matrix[0,2]*Z);
		double Ytrans = (matrix[1,0]*X)+(matrix[1,1]*Y)+(matrix[1,2]*Z);
		double Ztrans = (matrix[2,0]*X)+(matrix[2,1]*Y)+(matrix[2,2]*Z);
		var output = new Tuple<double,double,double>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public double vecDot(Tuple<double,double,double> A, Tuple<double,double,double> B)
	{
		double AX = A.Item1;
		double AY = A.Item2;
		double AZ = A.Item3;
		double BX = B.Item1;
		double BY = B.Item2;
		double BZ = B.Item3;
		var output = AX*BX+AY*BY+AZ+BZ;
		return output;
	}
	public Tuple<double,double,double> vecCross(Tuple<double,double,double> A, Tuple<double,double,double> B)
	{
		double AX = A.Item1;
		double AY = A.Item2;
		double AZ = A.Item3;
		double BX = B.Item1;
		double BY = B.Item2;
		double BZ = B.Item3;
		double Xtrans = AY*BZ-BY*AZ;
		double Ytrans = AZ*BX-BZ*AX;
		double Ztrans = AX*BY-BX*AY;
		var output = new Tuple<double,double,double>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public Tuple<double,double,double> unit(Tuple<double,double,double> point)
	{
		double X = point.Item1;
		double Y = point.Item2;
		double Z = point.Item3;
		double Xtrans;
		double Ytrans;
		double Ztrans;
		double L = length(point);
		if(L == 0)
		{
			Xtrans = 0;
			Ytrans = 0;
			Ztrans = 0;
		}
		else
		{
			Xtrans = X/L;
			Ytrans = Y/L;
			Ztrans = Z/L;
		}
		var output = new Tuple<double,double,double>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public double length(Tuple<double,double,double> point)
	{
		double X = point.Item1;
		double Y = point.Item2;
		double Z = point.Item3;
		double L = (double)(Math.Sqrt(((X * X))+((Y * Y))+((Z * Z))));
		return L;
	}
	public Tuple<double,double,double> vecAdd(Tuple<double,double,double> A, Tuple<double,double,double> B)
	{
		double AX = A.Item1;
		double AY = A.Item2;
		double AZ = A.Item3;
		double BX = B.Item1;
		double BY = B.Item2;
		double BZ = B.Item3;
		double Xtrans = AX+BX;
		double Ytrans = AY+BY;
		double Ztrans = AZ+BZ;
		var output = new Tuple<double,double,double>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public Tuple<double,double,double> vecSub(Tuple<double,double,double> A, Tuple<double,double,double> B)
	{
		double AX = A.Item1;
		double AY = A.Item2;
		double AZ = A.Item3;
		double BX = B.Item1;
		double BY = B.Item2;
		double BZ = B.Item3;
		double Xtrans = AX-BX;
		double Ytrans = AY-BY;
		double Ztrans = AZ-BZ;
		var output = new Tuple<double,double,double>(Xtrans,Ytrans,Ztrans);
		return output;
	}
	public Tuple<double,double,double> centroid(Tuple<double,double,double> A, Tuple<double,double,double> B, Tuple<double,double,double> C)
	{
		double AX = A.Item1;
		double AY = A.Item2;
		double AZ = A.Item3;
		double BX = B.Item1;
		double BY = B.Item2;
		double BZ = B.Item3;
		double CX = C.Item1;
		double CY = C.Item2;
		double CZ = C.Item3;
		double Xtrans = (AX+BX+CX)/3;
		double Ytrans = (AY+BY+CY)/3;
		double Ztrans = (AZ+BZ+CZ)/3;
		var output = new Tuple<double,double,double>(Xtrans,Ytrans,Ztrans);
		return output;
	}
}