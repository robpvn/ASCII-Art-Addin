using System;
using System.Collections.Generic;
using System.IO;
//using PaintDotNet;
//using PaintDotNet.IndirectUI;
//using PaintDotNet.PropertySystem;

namespace ILikePi.FileTypes.Ascii {
    public sealed class AsciiFactory : IFileTypeFactory {
        public FileType[] GetFileTypeInstances() {
            return new[] { new AsciiFileType() };
        }
    }

    internal class AsciiFileType : PropertyBasedFileType {
        private const string asciiChars = " .,;!vlLFE$", blocks = " \u2591\u2592\u2593\u2588";

        internal AsciiFileType()
            : base("ASCII Art", FileTypeFlags.SupportsLoading | FileTypeFlags.SupportsSaving, new[] { ".txt" }) {
        }

        private enum PropertyNames {
            HorizCellSize, VerticalCellSize, PresetCharacters, Characters
        }

        private enum PresetCharacters {
            AsciiChars, Blocks, Custom
        }

        protected unsafe override Document OnLoad(Stream input) {
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

            StreamReader reader = new StreamReader(input);
            string[] lines = reader.ReadToEnd().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            reader.Dispose();
            int w = lines[0].Length, h = lines.Length;
            BitmapLayer layer = Layer.CreateBackgroundLayer(w, h);

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

            Document doc = new Document(w, h);
            doc.Layers.Add(layer);
            return doc;
        }

        protected override bool IsReflexive(PropertyBasedSaveConfigToken token) {
            return false;
        }

        protected override unsafe void OnSaveT(Document input, Stream output, PropertyBasedSaveConfigToken token, Surface scratchSurface, ProgressEventHandler progressCallback) {
            using (RenderArgs ra = new RenderArgs(scratchSurface)) {
                input.Render(ra, true);
            }

            int w = token.GetProperty<Int32Property>(PropertyNames.HorizCellSize).Value;
            int h = token.GetProperty<Int32Property>(PropertyNames.VerticalCellSize).Value;
            int W = scratchSurface.Width, H = scratchSurface.Height;

            string chars;
            PresetCharacters preset = (PresetCharacters)token.GetProperty<StaticListChoiceProperty>(PropertyNames.PresetCharacters).Value;
            switch (preset) {
                case PresetCharacters.Custom:
                    chars = token.GetProperty<StringProperty>(PropertyNames.Characters).Value;
                    break;
                case PresetCharacters.AsciiChars:
                    chars = asciiChars;
                    break;
                case PresetCharacters.Blocks:
                    chars = blocks;
                    break;
                default:
                    chars = "";
                    break;
            }
            if (chars.Length == 0) {
                chars = asciiChars;
            }

            // Special case where 1 cell corresponds directly to 1 pixel
            // Written separately for performance
            if (h == 1 && w == 1) {
                StreamWriter writer = new StreamWriter(output);
                for (int y = 0; y < H; y++) {
                    ColorBgra* current = scratchSurface.GetRowAddressUnchecked(y);
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
                    ColorBgra* current = scratchSurface.GetRowAddressUnchecked(y);
                    for (int x = 0; x < W / w * w; x++) {
                        totals[x / w, y / h] += 1 - current->GetIntensity();
                        current++;
                    }
                }

                int ppc = w * h;
                StreamWriter writer = new StreamWriter(output);
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

        public override PropertyCollection OnCreatePropertyCollection() {
            return new PropertyCollection(new Property[] {
                new Int32Property(PropertyNames.HorizCellSize, 1, 1, 100),
                new Int32Property(PropertyNames.VerticalCellSize, 1, 1, 100),
                StaticListChoiceProperty.CreateForEnum<PresetCharacters>(PropertyNames.PresetCharacters, PresetCharacters.AsciiChars, false),
                new StringProperty(PropertyNames.Characters, string.Empty, 256)
            }, new[] {
                new ReadOnlyBoundToValueRule<object, StaticListChoiceProperty>(PropertyNames.Characters, PropertyNames.PresetCharacters, PresetCharacters.Custom, true)
            });
        }

        public override ControlInfo OnCreateConfigUI(PropertyCollection props) {
            ControlInfo info = CreateDefaultConfigUI(props);
            info.SetPropertyControlValue(PropertyNames.HorizCellSize, ControlInfoPropertyNames.DisplayName, "Horizontal cell size");
            info.SetPropertyControlValue(PropertyNames.VerticalCellSize, ControlInfoPropertyNames.DisplayName, "Vertical cell size");

            info.SetPropertyControlValue(PropertyNames.PresetCharacters, ControlInfoPropertyNames.DisplayName, "Preset characters");
            info.SetPropertyControlType(PropertyNames.PresetCharacters, PropertyControlType.RadioButton);
            PropertyControlInfo info2 = info.FindControlForPropertyName(PropertyNames.PresetCharacters);
            info2.SetValueDisplayName(PresetCharacters.AsciiChars, asciiChars);
            info2.SetValueDisplayName(PresetCharacters.Blocks, blocks);
            info2.SetValueDisplayName(PresetCharacters.Custom, "Custom (does not work with previewing and opening)");

            info.SetPropertyControlValue(PropertyNames.Characters, ControlInfoPropertyNames.DisplayName, "Custom characters");

            return info;
        }
    }
}