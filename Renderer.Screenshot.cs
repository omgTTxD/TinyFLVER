using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

	class Screenshot
	{
	public static RenderTargetView renderTarget;
	public static Texture2D stagingTexture;
	public static DataBox stagingData;

	public static void Initialize()
	{
		stagingTexture = new Texture2D(d, backBuffer.Description with
		{
			Width = form.Width,
			Height = form.Height,
			Format = Format.R16G16B16A16_Float,
			Usage = ResourceUsage.Staging,
			CpuAccessFlags = CpuAccessFlags.Read,
			BindFlags = BindFlags.None
		});
		stagingData = c.MapSubresource(stagingTexture, 0, MapMode.Read, 0);
	}

	public static void Make()
		{
			c.CopyResource(backBuffer, stagingTexture);
			var bitmap = new System.Drawing.Bitmap(w, h, PixelFormat.Format32bppArgb);
			var boundsRect = new System.Drawing.Rectangle(0, 0, w, h);
			var mapDest = bitmap.LockBits(boundsRect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
			var sourcePtr = stagingData.DataPointer;
			var destPtr = mapDest.Scan0;
			for (int y = 0; y < h; y++)
			{
				Utilities.CopyMemory(destPtr, sourcePtr, w * 4);
				sourcePtr = IntPtr.Add(sourcePtr, stagingData.RowPitch);
				destPtr = IntPtr.Add(destPtr, mapDest.Stride);
			}

			bitmap.UnlockBits(mapDest);
			bitmap.Save("E:\\1.png");
			form.screenshotRequested = false;
		}
	}

