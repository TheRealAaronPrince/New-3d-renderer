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
	public double[] matVecMult(double[] point, double[,] matrix)
	{
		double X = point[0];
		double Y = point[1];
		double Z = point[2];
		double Xtrans = (matrix[0,0]*X)+(matrix[0,1]*Y)+(matrix[0,2]*Z);
		double Ytrans = (matrix[1,0]*X)+(matrix[1,1]*Y)+(matrix[1,2]*Z);
		double Ztrans = (matrix[2,0]*X)+(matrix[2,1]*Y)+(matrix[2,2]*Z);
		double[] output = {Xtrans,Ytrans,Ztrans};
		return output;
	}
	public double vecDot(double[] A, double[] B)
	{
		double AX = A[0];
		double AY = A[1];
		double AZ = A[2];
		double BX = B[0];
		double BY = B[1];
		double BZ = B[2];
		var output = AX*BX+AY*BY+AZ+BZ;
		return output;
	}
	public double[] vecCross(double[] A, double[] B)
	{
		double AX = A[0];
		double AY = A[1];
		double AZ = A[2];
		double BX = B[0];
		double BY = B[1];
		double BZ = B[2];
		double Xtrans = AY*BZ-BY*AZ;
		double Ytrans = AZ*BX-BZ*AX;
		double Ztrans = AX*BY-BX*AY;
		var output = new double[] {Xtrans,Ytrans,Ztrans};
		return output;
	}
	public double[] normalize(double[] point)
	{
		double X = point[0];
		double Y = point[1];
		double Z = point[2];
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
		var output = new double[] {Xtrans,Ytrans,Ztrans};
		return output;
	}
	public double length(double[] point)
	{
		double X = point[0];
		double Y = point[1];
		double Z = point[2];
		double L = (double)(Math.Sqrt(((X * X))+((Y * Y))+((Z * Z))));
		return L;
	}
	public double[] vecAdd(double[] A, double[] B)
	{
		double AX = A[0];
		double AY = A[1];
		double AZ = A[2];
		double BX = B[0];
		double BY = B[1];
		double BZ = B[2];
		double Xtrans = AX+BX;
		double Ytrans = AY+BY;
		double Ztrans = AZ+BZ;
		var output = new double[] {Xtrans,Ytrans,Ztrans};
		return output;
	}
	public double[] vecSub(double[] A, double[] B)
	{
		double AX = A[0];
		double AY = A[1];
		double AZ = A[2];
		double BX = B[0];
		double BY = B[1];
		double BZ = B[2];
		double Xtrans = AX-BX;
		double Ytrans = AY-BY;
		double Ztrans = AZ-BZ;
		var output = new double[] {Xtrans,Ytrans,Ztrans};
		return output;
	}
	public double[] centroid(double[] A, double[] B, double[] C)
	{
		double AX = A[0];
		double AY = A[1];
		double AZ = A[2];
		double BX = B[0];
		double BY = B[1];
		double BZ = B[2];
		double CX = C[0];
		double CY = C[1];
		double CZ = C[2];
		double Xtrans = (AX+BX+CX)/3;
		double Ytrans = (AY+BY+CY)/3;
		double Ztrans = (AZ+BZ+CZ)/3;
		var output = new double[] {Xtrans,Ytrans,Ztrans};
		return output;
	}
}