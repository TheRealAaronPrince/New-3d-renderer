using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using graphics;
using System;

namespace plotter_2D
{
	public class Plotter
	{
		private double ambient = 0.125;
		public int Pwidth;
		public int Pheight;
		public render render;
		public double k = 1;
		private double[,] map;
		public Plotter(int w = 960, int h = 540)
		{
			Pwidth = w;
			Pheight = h;
			render = new render(Pwidth,Pheight);
			map =  new double[,]
			{
				{0.0,8.0,2.0,10.0},
				{12.0,4.0,14.0,6.0},
				{3.0,11.0,1.0,9.0},
				{15.0,7.0,13.0,5.0},
			};
		}
		public void line(Tuple<long,long,double> a, Tuple<long,long,double> b, double R = 255, double G = 255, double B = 255)
		{
			long x1 = a.Item1;
			long y1 = a.Item2;
			long x2 = b.Item1;
			long y2 = b.Item2;
			long dx = x2 - x1;
			long dy = y2 - y1;
			long x1a, x2a, y1a, y2a;
			if(x1 <= x2)
			{
				x1a = x1;
				x2a = x2;
			}
			else
			{
				x1a = x2;
				x2a = x1;
			}
			if(y1 <= y2)
			{
				y1a = y1;
				y2a = y2;
			}
			else
			{
				y1a = y2;
				y2a = y1;
			}
			if(System.Math.Abs(dy) < System.Math.Abs(dx))
			{
				unsafe
				{
					fixed(byte *pointer = render.pixelBuffer)
					{
						for(long xa = x1a; xa < x2a; xa++)
						{
							long ya = y1 + (dy * (xa - x1)) / dx;
							if(xa >= 0 && ya >= 0 && xa < render.width && ya < render.height)
							{
								long xb = Convert.ToInt64(xa);
								long yb = Convert.ToInt64(ya);
								byte *index = pointer + (((yb * render.width) + xb) * 4);
								*(index + 0) = (byte)Convert.ToInt32(B);
								*(index + 1) = (byte)Convert.ToInt32(G);
								*(index + 2) = (byte)Convert.ToInt32(R);
							}
						}
					}
				}
			}
			else
			{
				unsafe
				{
					fixed(byte *pointer = render.pixelBuffer)
					{
						for(long ya = y1a; ya < y2a; ya++)
						{
							long xa = x1 + (dx * (ya - y1)) / dy;
							if(xa >= 0 && ya >= 0 && xa < render.width && ya < render.height)
							{
								long xb = Convert.ToInt64(xa);
								long yb = Convert.ToInt64(ya);
								byte *index = pointer + (((yb * render.width) + xb) * 4);
								*(index + 0) = (byte)Convert.ToInt32(B);
								*(index + 1) = (byte)Convert.ToInt32(G);
								*(index + 2) = (byte)Convert.ToInt32(R);
							}
						}
					}
				};
			}
		}
		public void triangle(Tuple<long,long,double> a, Tuple<long,long,double> b, Tuple<long,long,double> c, double R = 255, double G = 255, double B = 255, double colorMult = 1, double O = 1, int type = 0, Tuple<byte[],int,int> texture = null, Tuple<double,double> d = null,Tuple<double,double> e  = null,Tuple<double,double> f  = null)
		{
			long x1 = a.Item1;
			long y1 = a.Item2;
			long x2 = b.Item1;
			long y2 = b.Item2;
			long x3 = c.Item1;
			long y3 = c.Item2;
			// Deltas
			long Dx12 = x1 - x2;
			long Dx23 = x2 - x3;
			long Dx31 = x3 - x1;
			long Dy12 = y1 - y2;
			long Dy23 = y2 - y3;
			long Dy31 = y3 - y1;
			// Bounding rectangle
			long[] xVal = new long[] {x1,x2,x3};
			long[] yVal = new long[] {y1,y2,y3};
			long minx = xVal.Min();
			long maxx = xVal.Max();
			long miny = yVal.Min();
			long maxy = yVal.Max();
			// Constant part of half-edge functions
			long C1 = Dy12 * x1 - Dx12 * y1;
			long C2 = Dy23 * x2 - Dx23 * y2;
			long C3 = Dy31 * x3 - Dx31 * y3;
			long Cy1 = C1 + Dx12 * miny - Dy12 * minx;
			long Cy2 = C2 + Dx23 * miny - Dy23 * minx;
			long Cy3 = C3 + Dx31 * miny - Dy31 * minx;
			long w = maxx - minx;
			long h = maxy - miny;
			long area = w*h;
			long Cx1 = 0;
			long Cx2 = 0;
			long Cx3 = 0;
			colorMult = (colorMult*(1.0-ambient))+ambient;
			unsafe
			{
				fixed(byte *pointer = render.pixelBuffer)
				{
					for(long i = 0; i < area; i++)
					{
						long relx = i%w;
						long rely = i/w;
						long x = relx + minx;
						long y = rely + miny;
						// Start value for horizontal scan
						if(relx == 0)
						{
							Cx1 = Cy1;
							Cx2 = Cy2;
							Cx3 = Cy3;
						}
						if(x >= 0 && y >= 0 && x < Pwidth && y < Pheight && Cx1 >= 0 && Cx2 >= 0 && Cx3 >= 0)
						{
							Tuple<double,double,double,double,double,double> depth = interior(a,b,c,x,y,d,e,f);
							
							var d0 = depth.Item4;
							double d1 = render.depthBuffer[y*Pwidth+x];
							if(d0 <= d1)
							{
								double db = (Math.Pow(1-(d0),k))*256;
								if(db > 255)
								{
									db = 255;
								}
								if(db < 0)
								{
									db = 0;
								}
								byte *index = pointer + (((y * Pwidth) + x) * 4);
								double Rout = 0;
								double Gout = 0;
								double Bout = 0;
								switch(type)
								{
									case 1:
										db /= 256.0;
										if(texture == null)
										{
											Bout = (((*(index + 0)*(1-O))+(B*O))*db);
											Gout = (((*(index + 1)*(1-O))+(G*O))*db);
											Rout = (((*(index + 2)*(1-O))+(R*O))*db);
										}
										else
										{
											int texX = Convert.ToInt32(depth.Item5*(texture.Item2-1));
											int texY = (texture.Item3-1)-Convert.ToInt32(depth.Item6*(texture.Item3-1));
											int tIndex = ((texY*texture.Item2) + texX)*4;
											Bout = (((*(index + 0)*(1-O))+((texture.Item1[tIndex+0])))*db);
											Gout = (((*(index + 1)*(1-O))+((texture.Item1[tIndex+1])))*db);
											Rout = (((*(index + 2)*(1-O))+((texture.Item1[tIndex+2])))*db);
										}
										break;
									case 2:
										Bout = ((*(index + 0)*(1-O))+(B*O));
										Gout = ((*(index + 1)*(1-O))+(G*O));
										Rout = ((*(index + 2)*(1-O))+(R*O));
										break;
									case 3:
										Bout = db;
										Gout = db;
										Rout = db;
										break;
									case 4:
										Bout = (depth.Item1)*255;
										Gout = (depth.Item2)*255;
										Rout = (depth.Item3)*255;
										break;
								}
								*(index + 0) = dither(Bout*colorMult,x,y);
								*(index + 1) = dither(Gout*colorMult,x,y);
								*(index + 2) = dither(Rout*colorMult,x,y);
								render.depthBuffer[y*Pwidth+x] = d0;
							}
						}
						Cx1 -= Dy12;
						Cx2 -= Dy23;
						Cx3 -= Dy31;
						if(relx == w-1)
						{
							Cy1 += Dx12;
							Cy2 += Dx23;
							Cy3 += Dx31;
						}
					}
				}
			};
		}
		public void rectangle(long x1, long y1, long x2, long y2, byte R = 255, byte G = 255, byte B = 255, double O = 1)
		{
			long[] xVal = new long[] {x1,x2};
			long[] yVal = new long[] {y1,y2};
			long minx = xVal.Min();
			long maxx = xVal.Max();
			long miny = yVal.Min();
			long maxy = yVal.Max();
			long w = maxx - minx;
			long h = maxy - miny;
			long area = w*h;
			unsafe
			{
				fixed(byte *pointer = render.pixelBuffer)
				{
					for(long i = 0; i < area; i++)
					{
						long relx = i%w;
						long rely = i/w;
						long x = relx + minx;
						long y = rely + miny;
						// Start value for horizontal scan
						if(x >= 0 && y >= 0 && x < Pwidth && y < Pheight)
						{
							byte *index = pointer +  (((y * Pwidth) + x) * 4);
							*(index + 0) = (byte)((*(index + 0)*(1-O))+(B*O));
							*(index + 1) = (byte)((*(index + 1)*(1-O))+(G*O));
							*(index + 2) = (byte)((*(index + 2)*(1-O))+(R*O));
						}
					}
				}
			};
		}
		private Tuple<double,double,double,double,double,double> interior(Tuple<long,long,double> a, Tuple<long,long,double> b, Tuple<long,long,double> c, long PX, long PY, Tuple<double,double> d, Tuple<double,double> e, Tuple<double,double> f)
		{
			double X1 = a.Item1;
			double Y1 = a.Item2;
			double X2 = b.Item1;
			double Y2 = b.Item2;
			double X3 = c.Item1;
			double Y3 = c.Item2;
			double Z1 = a.Item3/3;
			double Z2 = b.Item3/3;
			double Z3 = c.Item3/3;
			double Wa = (((Y2-Y3)*(PX-X3))+((X3-X2)*(PY-Y3)))/(((Y2-Y3)*(X1-X3))+((X3-X2)*(Y1-Y3)));
			double Wb = (((Y3-Y1)*(PX-X3))+((X1-X3)*(PY-Y3)))/(((Y2-Y3)*(X1-X3))+((X3-X2)*(Y1-Y3)));
			double Wc = 1f-(Wa+Wb);
			double depth = ((a.Item3*Wa)+(b.Item3*Wb)+(c.Item3*Wc))/3;
			double tX = -1.0;
			double tY = -1.0;
			if(d != null)
			{
				double tZ = Wa/Z1 + Wb/Z2 + Wc/Z3;
				tX = ((d.Item1*Wa/Z1)+(e.Item1*Wb/Z2)+(f.Item1*Wc/Z3))/tZ;
            	tY = ((d.Item2*Wa/Z1)+(e.Item2*Wb/Z2)+(f.Item2*Wc/Z3))/tZ;
			}
			return new Tuple<double,double,double,double,double,double> (Wa,Wb,Wc,depth,tX,tY);
		}
		byte dither(double val, long y, long x)
		{
			double dit = (map[x%4,y%4]/16);
			return (byte)Convert.ToInt32(Math.Round((val+dit-0.5)));
		}
		private double lerp()
		{
			return 0.0;
		}
	}
}