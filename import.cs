using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Reflection;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

public class import
{
	public Tuple<double,double,double>[] vert;
	public Tuple<double,double>[] txCoord;
	public Tuple<int,int,int,int,int,int,string>[] tri;
	public double objScl = 0.05f;
	public Dictionary<string,double[]> material = new Dictionary<string, double[]>();
	public Dictionary<string,Tuple<byte[],int,int>> texture = new Dictionary<string, Tuple<byte[],int,int>>();
	private bool debug = false;
	private int vCount = 0, vtCount = 0, fCount = 0;
	public void import_obj(string filename = "")
	{
		string obj;
		if(filename != "")
		{
			obj = File.ReadAllText(filename);
		}
		else
		{
			debug = true;
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "cube.obj";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				using (StreamReader reader = new StreamReader(stream))
			{
				obj = reader.ReadToEnd();
			}
		}
		string[] result = obj.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		for(int i = 0; i < result.Length; i++)
		{
			string[] X = result[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if(X[0] == "v")
			{
				vCount++;
			}
			if(X[0] == "vt")
			{
				vtCount++;
			}
			if(X[0] == "f")
			{
				fCount++;
			}
			if(X[0] == "mtllib")
			{
				if(Convert.ToString(X[1][2]) != ":" && !debug)
				{
					mtlDecode(Path.GetDirectoryName(filename) + "/" + string.Join(" ",X).Remove(0,7));
				}
				else
				{
					mtlDecode( X[1]);
				}
			}
		}
		vert = new Tuple<double,double,double>[vCount];
		tri = new Tuple<int,int,int,int,int,int,string>[fCount];
		txCoord = new Tuple<double, double>[vtCount];
		int vRow = 0, vtRow = 0, fRow = 0;
		string currentmtl = "";
		for(int l = 0; l < result.Length; l++)
		{
			string[] X = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if(X[0] == "v")
			{
				string[] parse = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				double a = (Convert.ToDouble(parse[1])*objScl);
				double b = -(Convert.ToDouble(parse[2])*objScl);
				double c = -(Convert.ToDouble(parse[3])*objScl);
				vert[vRow] = Tuple.Create(a,b,c);
				vRow++;
			}
			if(X[0] == "vt")
			{
				string[] parse = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				txCoord[vtRow] = Tuple.Create(Convert.ToDouble(parse[1]),Convert.ToDouble(parse[2]));
				vtRow++;
			}
			if(X[0] == "usemtl")
			{
				currentmtl = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1];
			}
			if(X[0] == "f")
			{
				string[] parse = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				int a = Convert.ToInt32(parse[1].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
				int b = Convert.ToInt32(parse[2].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
				int c = Convert.ToInt32(parse[3].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[0]);
				int d = -1;
				int e = -1;
				int f = -1;
				if(parse[1].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1] != "")
				{
					d = Convert.ToInt32(parse[1].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);
					e = Convert.ToInt32(parse[2].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);
					f = Convert.ToInt32(parse[3].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);
				}
				tri[fRow] = Tuple.Create(a,b,c,d,e,f,currentmtl);
				fRow++;
			}
		}
	}
	private void mtlDecode(string mtlfile)
	{
		string mtl;
		if(!debug)
		{
			mtl = File.ReadAllText(mtlfile);
		}
		else
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "cube.mtl";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
				using (StreamReader reader = new StreamReader(stream))
			{
				mtl = reader.ReadToEnd();
			}
		}
		string[] result = mtl.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		string currentmtl = "";
		for(int l = 0; l < result.Length; l++)
		{
			string[] X = result[l].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			if(X[0] == "newmtl")
			{
				currentmtl = X[1];
				material.Add(currentmtl,new double[]{0.75,0.75,0.75,1.0});
				texture[currentmtl] = null;
			}
			if(X[0] == "Kd")
			{
				material[currentmtl][0] = Convert.ToDouble(X[1]);
				material[currentmtl][1] = Convert.ToDouble(X[2]);
				material[currentmtl][2] = Convert.ToDouble(X[3]);
			}
			if(X[0] == "d")
			{
				material[currentmtl][3] = Convert.ToDouble(X[1]);
			}
			if(X[0] == "map_Kd")
			{
				try
				{
					if(Convert.ToString(X[1][2]) != ":" && !debug)
					{
						texImport(currentmtl,Path.GetDirectoryName(mtlfile) + "/" + result[l].TrimStart("map_Kd ".ToCharArray()));
					}
					else
					{
						texImport(currentmtl, X[1]);
					}
				}
				catch
				{
					
				}
			}
		}
	}
	private void texImport(string currentmtl, string file)
	{
		Bitmap png;
		if(!debug)
		{
			png = new Bitmap(file);
		}
		else
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "test cube.png";
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				png = new Bitmap(stream);
			}
		}
		texture[currentmtl] = Tuple.Create(rgbArray(png),png.Width,png.Height);
	}
	unsafe public byte[] rgbArray(Bitmap bmp)
	{
		byte[] data = new byte[bmp.Width*bmp.Height*4];

		BitmapData bmpData = new BitmapData();

		Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

		bmpData = bmp.LockBits(rect,
		                       ImageLockMode.ReadOnly,
		                       PixelFormat.Format32bppArgb);

		byte* Scan0 = (byte*)bmpData.Scan0.ToPointer();

		int size = bmp.Width * bmp.Height * 4;

		for (int i=0; i<size-1; i++)
		{
			data[i] = *(Scan0++);
		}

		bmp.UnlockBits(bmpData);
		return data;
	}
}