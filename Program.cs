using System;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.IO;

namespace renderer_3d
{
	class Program
	{
		static double cameraHeight = -.175;
		static double verticalVel = 0;
		static double gravity = 0.01;
		static bool running = true;
		static bool mouseHeld = true;
		static double deltaX,deltaY;
		static double sensitivity = 0.05f;
		static int width = 960;
		static int height = 540;
		static int depthfog = 1;
		static int windowHeight = height+36;
		static int movement = 0;
		static Thread Renderer;
		static Thread InputParser;
		static projection projection = new projection(width,height);
		static int view = 1;
		static double step = 0.025f;
		static Window w = new Window();
		static Application app = new Application();
		static double angleX, angleY, angleZ, rotateX, rotateY, rotateZ, posX, posY, posZ;
		static string obj = "C:/Users/princ/Desktop/bastet/bastet.obj";
		[STAThread]
		public static void Main(string[] args)
		{
			ResetAngleAndPos();
			if(args.Length >= 1 && obj == "")
			{
				obj = args[0];
			}
			w.Title = "3d rendering";
			w.Content = projection.Plotter.render.i;
			w.KeyDown += new KeyEventHandler(w_KeyDown);
			w.KeyUp += new KeyEventHandler(w_KeyUp);
			w.Show();
			if(mouseHeld){setMousePos(new Point(w.Width/2,w.Height/2));}
			InputParser = new Thread(ParseKeyboard);
			Renderer = new Thread(drawFrame);
			w.MouseMove += new MouseEventHandler(w_MouseMove);
			Renderer.Start();
			InputParser.Start();
			Kb();
			project();
			app.Run();
			running = false;
		}
		private static void ParseKeyboard(object source)
		{
			while(running)
			{
				w.Dispatcher.Invoke(new KbCallback(Kb),null);
				Thread.Sleep(20);
			}
		}
		private static void drawFrame(object source)
		{
			while(running)
			{
				w.Dispatcher.Invoke(new projectCallback(project),null);
				Thread.Sleep(1);
			}
		}
		private static void setMousePos(Point position)
		{
			Point toWindow = w.PointToScreen(position);
			SetCursorPos(Convert.ToInt32(toWindow.X),Convert.ToInt32(toWindow.Y));
		}
		[DllImport("User32.dll")]
		private static extern bool SetCursorPos(int X, int Y);
		public delegate void KbCallback();
		private static void Kb()
		{
			handleMovement();
			angleX = (angleX + 720)%360;
			angleY = (angleY + 720)%360;
			angleZ = (angleZ + 720)%360;
			rotateX = (rotateX + 720)%360;
			rotateY = (rotateY + 720)%360;
			rotateZ = (rotateZ + 720)%360;
			projection.rotX = angleX;
			projection.rotY = angleY;
			projection.rotZ = angleZ;
			projection.camX = posX;
			projection.camY = posY;
			projection.camZ = posZ;
			projection.angX = rotateX;
			projection.angY = rotateY;
			projection.angZ = rotateZ;
		}
		public delegate void projectCallback();
		private static void project()
		{
			try
			{
				projection.drawObj(obj, view);
			}
			catch{}
		}
		private static void w_MouseMove(object sender, MouseEventArgs e)
		{
			if(mouseHeld)
			{
				w.Cursor = (Cursors.None);
				deltaX = (double)(e.GetPosition(w).X - w.Width/2);
				deltaY = (double)(e.GetPosition(w).Y - w.Height/2);
				angleY -= deltaX*sensitivity;
				angleX += deltaY*sensitivity;
				if(angleX > 90 && angleX < 180)
				{
					angleX = 90;
				}
				if(angleX < 270 && angleX > 180)
				{
					angleX = 270;
				}
				w.Title = "Pitch: " + angleX + "  Yaw: " + angleY;
				setMousePos(new Point(w.Width/2,w.Height/2));
			}
			else
			{
				w.Cursor = (Cursors.Arrow);
			}
		}
		private static void w_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Up:
					movement |= Convert.ToInt32("0000000001",2);
					break;
				case Key.Down:
					movement |= Convert.ToInt32("0000000010",2);
					break;
				case Key.Left:
					movement |= Convert.ToInt32("0000000100",2);
					break;
				case Key.Right:
					movement |= Convert.ToInt32("0000001000",2);
					break;
				case Key.D:
					movement |= Convert.ToInt32("0000010000",2);
					break;
				case Key.A:
					movement |= Convert.ToInt32("0000100000",2);
					break;
				case Key.S:
					movement |= Convert.ToInt32("0001000000",2);
					break;
				case Key.W:
					movement |= Convert.ToInt32("0010000000",2);
					break;
				case Key.Space:
					movement |= Convert.ToInt32("0100000000",2);
					break;
				case Key.LeftShift:
					movement |= Convert.ToInt32("1000000000",2);
					break;
				case Key.Tab:
					view ++;
					view %=5;
					break;
				case Key.Q:
					if(depthfog < 64)
					{
						depthfog *=2;
						projection.Plotter.k = 1+depthfog;
					}
					break;
				case Key.E:
					if(depthfog > 1)
					{
						depthfog /=2;
						projection.Plotter.k = 1+depthfog;
					}
					break;
				case Key.Z:
					
					break;
				case Key.PageUp:
					step *=2;
					break;
				case Key.PageDown:
					step /=2;
					break;
				case Key.Escape:
					mouseHeld = !mouseHeld;
					break;
				case Key.Enter:
					screenshot();
					break;
				case Key.R:
					ResetAngleAndPos();
					break;
			}
		}
		private static void ResetAngleAndPos()
		{
			angleX = 0.0f;
			angleY = 0.0f;
			angleZ = 0.0f;
			rotateX = 0.0f;
			rotateY = 0.0f;
			rotateZ = 0.0f;
			posX = 0.0f;
			posY = cameraHeight;
			posZ = -1.0f;
		}
		private static void w_KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Up:
					movement &= Convert.ToInt32("1111111110",2);
					break;
				case Key.Down:
					movement &= Convert.ToInt32("1111111101",2);
					break;
				case Key.Left:
					movement &= Convert.ToInt32("1111111011",2);
					break;
				case Key.Right:
					movement &= Convert.ToInt32("1111110111",2);
					break;
				case Key.D:
					movement &= Convert.ToInt32("1111101111",2);
					break;
				case Key.A:
					movement &= Convert.ToInt32("1111011111",2);
					break;
				case Key.S:
					movement &= Convert.ToInt32("1110111111",2);
					break;
				case Key.W:
					movement &= Convert.ToInt32("1101111111",2);
					break;
				case Key.Space:
					movement &= Convert.ToInt32("1011111111",2);
					break;
				case Key.LeftShift:
					movement &= Convert.ToInt32("0111111111",2);
					break;
			}
		}
		private static void handleMovement()
		{
			if((movement & Convert.ToInt32("0000000001",2)) != 0)
			{
				rotateX -= 2.5f;
			}
			if((movement & Convert.ToInt32("0000000010",2)) != 0)
			{
				rotateX += 2.5f;
			}
			if((movement & Convert.ToInt32("0000000100",2)) != 0)
			{
				rotateY += 2.5f;
			}
			if((movement & Convert.ToInt32("0000001000",2)) != 0)
			{
				rotateY -= 2.5f;
			}
			if((movement & Convert.ToInt32("0000010000",2)) != 0)
			{
				posZ = (double)(posZ+(Math.Sin(angleY*(Math.PI/180))*step));
				posX = (double)(posX+(Math.Cos(angleY*(Math.PI/180))*step));
			}
			if((movement & Convert.ToInt32("0000100000",2)) != 0)
			{
				posZ = (double)(posZ+(Math.Sin(angleY*(Math.PI/180))*-step));
				posX = (double)(posX+(Math.Cos(angleY*(Math.PI/180))*-step));
			}
			if((movement & Convert.ToInt32("0001000000",2)) != 0)
			{
				posZ = (double)(posZ+(Math.Sin((angleY-90)*(Math.PI/180))*step));
				posX = (double)(posX+(Math.Cos((angleY-90)*(Math.PI/180))*step));
			}
			if((movement & Convert.ToInt32("0010000000",2)) != 0)
			{
				posZ = (double)(posZ+(Math.Sin((angleY-90)*(Math.PI/180))*-step));
				posX = (double)(posX+(Math.Cos((angleY-90)*(Math.PI/180))*-step));
			}
			if((movement & Convert.ToInt32("0100000000",2)) != 0)
			{
				if(Math.Abs(posY-cameraHeight) < 0.001)
				{
					verticalVel = 0.075;
				}
			}
			if((movement & Convert.ToInt32("1000000000",2)) != 0)
			{
				cameraHeight = -0.075;
			}
			else
			{
				cameraHeight = -0.175;
			}
			if(verticalVel >= -10)
			{
				verticalVel -= gravity;
			}
			posY -= verticalVel;
			if(posY > cameraHeight)
			{
				posY = cameraHeight;
			}
		}
		private static void screenshot()
		{
			string filename = Convert.ToString(Time())+".png";
			using (FileStream stream =
			       new FileStream(filename, FileMode.Create))
			{
				PngBitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(projection.Plotter.render.writeableBitmap));
				encoder.Save(stream);
			}
		}
		private static long Time()
		{
			return Convert.ToInt64(Math.Floor(DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc)).TotalSeconds));
		}
	}
}