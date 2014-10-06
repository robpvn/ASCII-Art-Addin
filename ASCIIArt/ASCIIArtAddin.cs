using System;
using Pinta.Core;

namespace ASCIIArt
{
	[Mono.Addins.Extension]
	public class ASCIIArtAddin :IExtension
	{
		#region IExtension implementation

		public void Initialize ()
		{
			// Register the file format.
			PintaCore.System.ImageFormats.RegisterFormat (
				new FormatDescriptor("ASCII Art", new string[] {"txt"},
			new ASCIIArtImporter (), new ASCIIArtExporter ()));
		}

		public void Uninitialize ()
		{
			PintaCore.System.ImageFormats.UnregisterFormatByExtension ("txt");
		}

		#endregion



	}
}

