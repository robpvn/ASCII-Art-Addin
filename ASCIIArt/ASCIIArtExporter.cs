using System;
using System.Collections.Generic;
using System.IO;
using Pinta;
using Pinta.Core;
using Gtk;

namespace ASCIIArt
{
	public class ASCIIArtExporter : IImageExporter
	{
		public ASCIIArtExporter ()
		{
		}

		#region IImageExporter implementation

		public unsafe void Export (Document document, string fileName, Window parent)
		{

			int w = 64, h = 64; //Default value to configure, pixels per block			
			string chars = DefaultCharacters.AsciiChars; //Get the selection from config

			ASCIIOptionsDialog dialog = new ASCIIOptionsDialog (w, h);
			try {
				dialog.WindowPosition = Gtk.WindowPosition.CenterOnParent;

				int response = dialog.Run ();

				if (response == (int)Gtk.ResponseType.Ok) {
					w = dialog.ExportImageWidth;
					h = dialog.ExportImageHeight;
					chars = dialog.ExportImageChars;
				} else {
					return;
				}

			} finally {
				dialog.Destroy ();
			}


			Cairo.ImageSurface surface = document.GetFlattenedImage ();			
			int W = surface.Width, H = surface.Height;


			// Special case where 1 cell corresponds directly to 1 pixel
			// Written separately for performance
			if (h == 1 && w == 1) {
				StreamWriter writer = new StreamWriter(fileName);
				for (int y = 0; y < H; y++) {
					ColorBgra* current = surface.GetRowAddressUnchecked(y);
					for (int x = 0; x < W; x++) {
						int pos = (int)((1 - current->GetIntensity()) * chars.Length);
						char c = chars[pos == chars.Length ? pos - 1 : pos];
						writer.Write(c);
						current++;
					}
					if (y != H - 1) {
						writer.WriteLine();
					}
				}
				writer.Flush();
			} else {
				double[,] totals = new double[W / w, H / h];
				for (int y = 0; y < H / h * h; y++) {
					ColorBgra* current = surface.GetRowAddressUnchecked(y);
					for (int x = 0; x < W / w * w; x++) {
						totals[x / w, y / h] += 1 - current->GetIntensity();
						current++;
					}
				}

				int ppc = w * h;
				StreamWriter writer = new StreamWriter(fileName);
				for (int y = 0; y < H / h; y++) {
					for (int x = 0; x < W / w; x++) {
						int pos = (int)(totals[x, y] / ppc * chars.Length);
						char c = chars[pos == chars.Length ? pos - 1 : pos];
						writer.Write(c);
					}
					if (y != H / h - 1) {
						writer.WriteLine();
					}
				}
				writer.Flush();
			}



		}

		#endregion
	}
}

