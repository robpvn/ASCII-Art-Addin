using System;
using Gtk;
using Pinta;
using Mono.Addins;

namespace ASCIIArt
{
	public partial class ASCIIOptionsDialog : Gtk.Dialog
	{
		public ASCIIOptionsDialog (int initialW, int initialH)
		{
			this.Build ();

			label1.Text = AddinManager.CurrentLocalizer.GetString ("Preset characters");
			radiobutton3.Label = AddinManager.CurrentLocalizer.GetString ("Custom (Cannot be reopened in Pinta)");
			label2.Text = AddinManager.CurrentLocalizer.GetString ("ASCII file resolution");
			label3.Text = AddinManager.CurrentLocalizer.GetString ("Horisontal cell size (in pixels)");
			label4.Text = AddinManager.CurrentLocalizer.GetString ("Vertical cell size (in pixels)");


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

