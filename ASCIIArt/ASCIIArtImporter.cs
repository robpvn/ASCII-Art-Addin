using System;
using System.Collections.Generic;
using System.IO;
using Gdk;
using Pinta.Core;

namespace ASCIIArt
{
	public class ASCIIArtImporter : IImageImporter
	{
		public ASCIIArtImporter ()
		{
		}

		#region IImageImporter implementation

		public unsafe void Import (string filename, Gtk.Window parent)
		{
			IDictionary<char, byte> cti = new Dictionary<char, byte>();
			cti.Add(' ', 255);
			cti.Add('.', 255 * 9 / 10);
			cti.Add(',', 255 * 8 / 10);
			cti.Add(';', 255 * 7 / 10);
			cti.Add('!', 255 * 6 / 10);
			cti.Add('v', 255 * 5 / 10);
			cti.Add('l', 255 * 4 / 10);
			cti.Add('L', 255 * 3 / 10);
			cti.Add('F', 255 * 2 / 10);
			cti.Add('E', 255 * 1 / 10);
			cti.Add('$', 0);

			cti.Add('\u2591', 255 * 3 / 4);
			cti.Add('\u2592', 255 * 2 / 4);
			cti.Add('\u2593', 255 * 1 / 4);
			cti.Add('\u2588', 0);

			StreamReader reader = new StreamReader(filename);
			string[] lines = reader.ReadToEnd().Split(new[] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
			reader.Dispose(); 
			int w = lines[0].Length, h = lines.Length;

			// Create a new document and add an initial layer.
			Document doc = PintaCore.Workspace.CreateAndActivateDocument (filename, new Size (w, h));
			doc.HasFile = true;
			doc.Workspace.CanvasSize = doc.ImageSize;
			Layer layer = doc.AddNewLayer (Path.GetFileName (filename));

			Console.WriteLine ("w = " + w + " h = " + h);

			for (int y = 0; y < h; y++) {
				ColorBgra* current = layer.Surface.GetRowAddressUnchecked(y);
				for (int x = 0; x < w; x++) {
					byte intensity;
					if (!cti.TryGetValue(lines[y][x], out intensity)) {
						intensity = 0;
					}
					current->A = 255;
					current->R = intensity;
					current->G = intensity;
					current->B = intensity;
					current++;
				}
			}
		}

		public Gdk.Pixbuf LoadThumbnail (string filename, int maxWidth, int maxHeight, Gtk.Window parent)
		{
			//TODO: Look at making this wrk
			return null;
		}

		#endregion
	}
}

