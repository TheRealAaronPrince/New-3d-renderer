using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System;

namespace graphics
{
	public partial class render
	{
		public WriteableBitmap writeableBitmap;
		public Image i;
		public int width;
		public int height;
		//RGBA value array for the pixels
		public byte[] pixelBuffer;
		public double[] depthBuffer;
		public render(int ww = 320, int hh = 240)
		{
			i = new Image();
			i.Stretch = Stretch.Uniform;
			RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.NearestNeighbor);
			RenderOptions.SetEdgeMode(i, EdgeMode.Aliased);
			width = ww;
			height = hh;
			writeableBitmap = new WriteableBitmap(width,height,96,96,PixelFormats.Bgr32,null);
			i.Source = writeableBitmap;
			pixelBuffer = new byte[4*width*height];
			depthBuffer = new double[width*height];
		}
		//loop to set a default color for every pixel
		public void SetBackground(int colR, int colG, int colB, double far)
		{
			for(int i = 0; i < width*height; i++)
			{
				depthBuffer[i] = far*1024;
				pixelBuffer[i*4+0] = (byte)colB;
				pixelBuffer[i*4+1] = (byte)colG;
				pixelBuffer[i*4+2] = (byte)colR;
			}
		}
		//converting the array to an image
		public void update()
		{
			writeableBitmap.Lock();
			// Get a pointer to the back buffer.
			IntPtr pBackBuffer = writeableBitmap.BackBuffer;
			Marshal.Copy(pixelBuffer,0,pBackBuffer,pixelBuffer.Length);
			writeableBitmap.AddDirtyRect(new Int32Rect(0,0,width,height));
			writeableBitmap.Unlock();
		}
	}
}