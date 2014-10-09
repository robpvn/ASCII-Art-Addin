using System;
using Gtk;

namespace ASCIIArt
{
	public partial class ASCIIOptionsDialog : Gtk.Dialog
	{
		public ASCIIOptionsDialog (int initialW, int initialH)
		{
			this.Build ();

			spinButtonW.Value = initialW;
			spinButtonH.Value = initialH;
		}

		public int ExportImageWidth { get { return spinButtonW.ValueAsInt; } }
		public int ExportImageHeight { get { return spinButtonH.ValueAsInt; } }
		public string ExportImageChars { get { 
				if (radioButtonAscii.Active) {
					return DefaultCharacters.AsciiChars;
				} else if (radioButtonBlocks.Active) {
					return DefaultCharacters.Blocks;
				} else {
					return entryFieldChars.Text;
				}
			} 
		}
	}
}

