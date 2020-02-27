﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Linq;
using System.Web.Script.Serialization;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace _2k2mv
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            if (File.Exists(iniPath))
            {
                settings = js.Deserialize<Settings>(File.ReadAllText(iniPath));
            }
            else
            {
                settings.inputPath = "";
                settings.outputPath = "";
                settings.rtpPath = "";
                settings.copyMissingImagesFromRTP = false;
                settings.updateOnlyTileDataIfOutputMapExists = true;
                File.WriteAllText(iniPath, js.Serialize(settings).Replace(",", ",\n\t").Replace("{", "{\n\t").Replace("}", "\n}"));
            }
            if (settings.rtpPath == "")
            {
                rtpPath = (string)Registry.GetValue("HKEY_CURRENT_USER\\Software\\ASCII\\RPG2000", "RuntimePackagePath", "");
            }
            else
            {
                rtpPath = settings.rtpPath;
            }
            inputPath = settings.inputPath;
            if (inputPath != "")
            {
                label_inputDir_status.Text = inputPath;
                label_inputDir_status.ForeColor = System.Drawing.Color.Green;
            }
            outputPath = settings.outputPath;
            if (outputPath != "")
            {
                label_outputDir_status.Text = outputPath;
                label_outputDir_status.ForeColor = System.Drawing.Color.Green;
            }
            if (inputPath != "" && outputPath != "")
            {
                button_iconv.Enabled = true;
                button_dconv.Enabled = true;
                button_mconv.Enabled = true;
            }
            copyMissingImagesCheckBox.Checked = settings.copyMissingImagesFromRTP;
            updateOnlyMapDataCheckBox.Checked = settings.updateOnlyTileDataIfOutputMapExists;
            js.MaxJsonLength = Int32.MaxValue;
        }
        string inputPath;
        string outputPath;
        string rtpPath;
        Settings settings = new Settings();
        string iniPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "2k2mv.ini");
        JavaScriptSerializer js = new JavaScriptSerializer();
        

        Bitmap image;
        static int[] x = new int[] { 0, 144, 0, 144, 0, 144, 0, 144, 288, 432, 288, 432, 288, 432, 288, 432 }; // coordinates of autotiles upper left corner for rm2k
        static int[] y = new int[] { 0, 0, 192, 192, 384, 384, 576, 576, 0, 0, 192, 192, 384, 384, 576, 576 };
        static int[] x2 = new int[] { 0, 0, 0, 0, 0, 96, 192, 288, 384, 480, 576, 672, 0, 96, 192, 288 }; // coordinates of autotiles upper left corner for rmmv
        static int[] y2 = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 144, 144, 144, 144 };
        static List<int> aatn = new List<int> { 0, 50, 100, 150, 400, 450, 500, 550, 200, 250, 300, 350, 600, 650, 700, 750 }; //numbering sequence for interfaced animated autotiles

        private void button_iconv_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                label_iconv_status.Invoke(new Action(() => label_iconv_status.Text = "Working..."));
                label_iconv_status.Invoke(new Action(() => label_iconv_status.ForeColor = System.Drawing.Color.Red));
                disableButtons();

                foreach (var imagePath in EnumerateFilesSafely(Path.Combine(inputPath, "ChipSet"), "*.png", System.IO.SearchOption.AllDirectories, null))
                {
                    var onlyName = Path.GetFileNameWithoutExtension(imagePath);
                    var Name = Path.GetFileName(imagePath);
                    image = new Bitmap(imagePath, true);
                    image = new Bitmap(image); //convert to 32bppArgb
                    image = ResizeBitmap(image, 1440, 768);
                    string newPath = imagePath.Replace(inputPath, outputPath);
                    newPath = newPath.Replace("ChipSet", "img\\tilesets");
                    string path = Path.GetDirectoryName(newPath);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var tcolor = image.GetPixel(864, 384);
                    image.MakeTransparent(tcolor);
                    Bitmap image_B = extractLowerTiles(image);
                    image_B.Save(Path.Combine(path, onlyName + @"_B.png"), ImageFormat.Png);
                    image_B.Dispose();

                    Bitmap image_C = extractUpperTiles(image);
                    image_C.Save(Path.Combine(path, onlyName + @"_C.png"), ImageFormat.Png);
                    image_C.Dispose();

                    Bitmap image_A2 = extractAutoTiles(image);
                    image_A2.Save(Path.Combine(path, onlyName + @"_A2.png"), ImageFormat.Png);
                    image_A2.Dispose();

                    Bitmap image_A1 = extractAnimTiles(image);
                    image_A1.Save(Path.Combine(path, onlyName + @"_A1.png"), ImageFormat.Png);
                    image_A1.Dispose();

                    Bitmap image_D = extractAutoTilesAsNormalTiles(image);
                    image_D.Save(Path.Combine(path, onlyName + @"_D.png"), ImageFormat.Png);
                    image_D.Dispose();

                    image.Dispose();
                }
                label_iconv_status.Invoke(new Action(() => label_iconv_status.Text = "Done"));
                label_iconv_status.Invoke(new Action(() => label_iconv_status.ForeColor = System.Drawing.Color.Green));
                enableButtons();                
            });
            
        }


        public static Bitmap extractLowerTiles(Bitmap image)
        {
            Bitmap image_B = new Bitmap(768, 768);
            CopyRegionIntoImage(image, new Rectangle(576,0,288,768),ref image_B, new Rectangle(48, 0, 288, 768));
            CopyRegionIntoImage(image, new Rectangle(864, 0, 288, 384), ref image_B, new Rectangle(432, 0, 288, 384));
            return image_B;
        }

        public static Bitmap extractUpperTiles(Bitmap image)
        {
            Bitmap image_C = new Bitmap(768, 768);
            CopyRegionIntoImage(image, new Rectangle(864, 384, 288, 384), ref image_C, new Rectangle(48, 384, 288, 384));
            CopyRegionIntoImage(image, new Rectangle(1152, 0, 288, 768), ref image_C, new Rectangle(432, 0, 288, 768));            
            return image_C;
        }

        public static Bitmap extractAutoTilesAsNormalTiles(Bitmap image)
        {
            Bitmap image_D = new Bitmap(768, 768);
            CopyRegionIntoImage(image, new Rectangle(0, 384, 288, 384), ref image_D, new Rectangle(48, 0, 288, 384));
            CopyRegionIntoImage(image, new Rectangle(288, 0, 288, 384), ref image_D, new Rectangle(48, 384, 288, 384));
            CopyRegionIntoImage(image, new Rectangle(288, 384, 288, 384), ref image_D, new Rectangle(432, 0, 288, 384));
            int zeroX = 0;
            int zeroY = 0;
            for (int n = 0; n <= 11; n++)
            {
                zeroX = (int)(48 + 144 * (n % 2) + 384 * Math.Floor((double)n / 8));
                zeroY = (int)(192 * Math.Floor((double)n / 2) - 768 * Math.Floor((double)n / 8));
                //copy left+right borders to unused top center tile
                CopyRegionIntoImage(image_D, new Rectangle(zeroX, zeroY + 96, 24, 48), ref image_D, new Rectangle(zeroX + 48, zeroY, 24, 48));
                CopyRegionIntoImage(image_D, new Rectangle(zeroX + 120, zeroY + 96, 24, 48), ref image_D, new Rectangle(zeroX + 72, zeroY, 24, 48));
            }
            return image_D;
        }

        public static Bitmap extractAutoTiles(Bitmap image)
        {
            Bitmap image_A2 = new Bitmap(768, 576);
            for (int n=4; n<= 15; n++)
            {
                Bitmap tile = new Bitmap(144, 192);
                CopyRegionIntoImage(image, new Rectangle(x[n], y[n], 144, 192), ref tile, new Rectangle(0, 0, 144, 192));
                tile = convertAutoTile(tile);
                CopyRegionIntoImage(tile, new Rectangle(0, 0, 96, 144), ref image_A2, new Rectangle(x2[n], y2[n], 96, 144));
                tile.Dispose(); 
            }
            return image_A2;
        }

        public static Bitmap extractAnimTiles(Bitmap image)
        {
            Bitmap image_A1 = new Bitmap(768, 576);
            Bitmap tile = new Bitmap(768, 576);
            tile = convertAnimTile(image);
            CopyRegionIntoImage(tile, new Rectangle(0, 0, 288, 144), ref image_A1, new Rectangle(0, 0, 288, 144));//copy first animated autotiles
            CopyRegionIntoImage(tile, new Rectangle(0, 144, 288, 144), ref image_A1, new Rectangle(384, 0, 288, 144));//copy second animated autotiles
            CopyRegionIntoImage(tile, new Rectangle(384, 0, 288, 144), ref image_A1, new Rectangle(0, 288, 288, 144));//copy third animated autotile ("deep water")
            CopyRegionIntoImage(tile, new Rectangle(384, 144, 288, 144), ref image_A1, new Rectangle(0, 432, 288, 144));
            CopyRegionIntoImage(tile, new Rectangle(0, 288, 288, 144), ref image_A1, new Rectangle(0, 144, 288, 144));
            tile.Dispose();
            //copy waterfall tiles
            CopyRegionIntoImage(image, new Rectangle(144, 192, 48, 144), ref image_A1, new Rectangle(672, 0, 48, 144));
            CopyRegionIntoImage(image, new Rectangle(144, 192, 48, 144), ref image_A1, new Rectangle(720, 0, 48, 144));

            CopyRegionIntoImage(image, new Rectangle(192, 192, 48, 144), ref image_A1, new Rectangle(672, 144, 48, 144));
            CopyRegionIntoImage(image, new Rectangle(192, 192, 48, 144), ref image_A1, new Rectangle(720, 144, 48, 144));

            CopyRegionIntoImage(image, new Rectangle(240, 192, 48, 144), ref image_A1, new Rectangle(672, 288, 48, 144));
            CopyRegionIntoImage(image, new Rectangle(240, 192, 48, 144), ref image_A1, new Rectangle(720, 288, 48, 144));

            return image_A1;
        }

         public static Bitmap convertAnimTile(Bitmap image) 
        {

            Bitmap tile = new Bitmap(768, 576);
            //f - frame (0-2), n - number (0 - first, 1 - second), t type (0 - normal, 1 - deep)
            for (int f = 0; f <= 2; f++)
            {
                for (int n = 0; n <= 1; n++)
                {
                    for (int t = 0; t <= 1; t++)
                    {
                        CopyRegionIntoImage(image, new Rectangle(0 + f * 48 + n * 144 * (1 - t), 0 + t * 288 - t * n * 48, 48, 48), ref tile, new Rectangle(0 + f * 96 + t * 384, 0 + n * 144, 48, 48));//square
                        CopyRegionIntoImage(image, new Rectangle(0 + f * 48 + n * 144 * (1 - t), 144 + t * 144 - t * n * 48, 48, 48), ref tile, new Rectangle(48 + f * 96 + t * 384, 0 + n * 144, 48, 48));//cross

                        CopyRegionIntoImage(image, new Rectangle(0 + f * 48 + n * 144 * (1 - t), 0 + t * 288 - t * n * 48, 24, 24), ref tile, new Rectangle(0 + f * 96 + t * 384, 48 + n * 144, 24, 24));//upper left corner
                        CopyRegionIntoImage(image, new Rectangle(0 + f * 48 + n * 144 * (1 - t), 48 + t * 288 - t * n * 144, 24, 24), ref tile, new Rectangle(0 + f * 96 + t * 384, 96 + n * 144, 24, 24));//left border 1
                        CopyRegionIntoImage(image, new Rectangle(0 + f * 48 + n * 144 * (1 - t), 72 + t * 288 - t * n * 144, 24, 24), ref tile, new Rectangle(0 + f * 96 + t * 384, 72 + n * 144, 24, 24));//left border 2
                        CopyRegionIntoImage(image, new Rectangle(0 + f * 48 + n * 144 * (1 - t), 24 + t * 288 - t * n * 48, 24, 24), ref tile, new Rectangle(0 + f * 96 + t * 384, 120 + n * 144, 24, 24));//lower left corner

                        CopyRegionIntoImage(image, new Rectangle(0 + f * 48 + n * 144 * (1 - t), 96 + t * 240 - t * n * 144, 24, 24), ref tile, new Rectangle(48 + f * 96 + t * 384, 48 + n * 144, 24, 24));//upper border 1
                        CopyRegionIntoImage(image, new Rectangle(24 + f * 48 + n * 144 * (1 - t), 96 + t * 240 - t * n * 144, 24, 24), ref tile, new Rectangle(24 + f * 96 + t * 384, 48 + n * 144, 24, 24));//upper border 2
                        CopyRegionIntoImage(image, new Rectangle(0 + f * 48, 192 + t * 144 - t * n * 144, 48, 48), ref tile, new Rectangle(24 + f * 96 + t * 384, 72 + n * 144, 48, 48));//center
                        CopyRegionIntoImage(image, new Rectangle(0 + f * 48 + n * 144 * (1 - t), 120 + t * 240 - t * n * 144, 24, 24), ref tile, new Rectangle(48 + f * 96 + t * 384, 120 + n * 144, 24, 24));//lower border 1
                        CopyRegionIntoImage(image, new Rectangle(24 + f * 48 + n * 144 * (1 - t), 120 + t * 240 - t * n * 144, 24, 24), ref tile, new Rectangle(24 + f * 96 + t * 384, 120 + n * 144, 24, 24));//lower border 2

                        CopyRegionIntoImage(image, new Rectangle(24 + f * 48 + n * 144 * (1 - t), 0 + t * 288 - t * n * 48, 24, 24), ref tile, new Rectangle(72 + f * 96 + t * 384, 48 + n * 144, 24, 24));//upper right corner
                        CopyRegionIntoImage(image, new Rectangle(24 + f * 48 + n * 144 * (1 - t), 48 + t * 288 - t * n * 144, 24, 24), ref tile, new Rectangle(72 + f * 96 + t * 384, 96 + n * 144, 24, 24));//right border 1
                        CopyRegionIntoImage(image, new Rectangle(24 + f * 48 + n * 144 * (1 - t), 72 + t * 288 - t * n * 144, 24, 24), ref tile, new Rectangle(72 + f * 96 + t * 384, 72 + n * 144, 24, 24));//right border 2
                        CopyRegionIntoImage(image, new Rectangle(24 + f * 48 + n * 144 * (1 - t), 24 + t * 288 - t * n * 48, 24, 24), ref tile, new Rectangle(72 + f * 96 + t * 384, 120 + n * 144, 24, 24));//lower right corner
                    }

                }
            }
            CopyRegionIntoImage(tile, new Rectangle(0, 0, 288, 144), ref tile, new Rectangle(0, 288, 288, 144)); //combine first and third animated autotiles
            CopyRegionIntoImage(tile, new Rectangle(408, 72, 48, 48), ref tile, new Rectangle(24, 360, 48, 48));
            CopyRegionIntoImage(tile, new Rectangle(504, 72, 48, 48), ref tile, new Rectangle(120, 360, 48, 48));
            CopyRegionIntoImage(tile, new Rectangle(600, 72, 48, 48), ref tile, new Rectangle(216, 360, 48, 48));
            return tile;
        }

        public static Bitmap convertAutoTile(Bitmap image)
        {
            Bitmap tile = new Bitmap(96, 144);
            CopyRegionIntoImage(image, new Rectangle(0, 0, 48, 96), ref tile, new Rectangle(0, 0, 48, 96));
            CopyRegionIntoImage(image, new Rectangle(96, 0, 48, 96), ref tile, new Rectangle(48, 0, 48, 96));
            CopyRegionIntoImage(image, new Rectangle(0, 144, 48, 48), ref tile, new Rectangle(0, 96, 48, 48));
            CopyRegionIntoImage(image, new Rectangle(96, 144, 48, 48), ref tile, new Rectangle(48, 96, 48, 48));
            return tile;
        }

        public static void CopyRegionIntoImage(Bitmap srcBitmap, Rectangle srcRegion, ref Bitmap destBitmap, Rectangle destRegion)
        {
            using (Graphics grD = Graphics.FromImage(destBitmap))
            {
                grD.DrawImage(srcBitmap, destRegion, srcRegion, GraphicsUnit.Pixel);
            }
        }

        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.DrawImage(sourceBMP, 0, 0, width, height);
            }
            return result;
        }

        public static IEnumerable<string> EnumerateFilesSafely(string path, string searchPattern, System.IO.SearchOption searchOptions = System.IO.SearchOption.TopDirectoryOnly, Action<string> onAccessDenied = null)
        {
            try
            {
                var fileNames = Enumerable.Empty<string>();

                if (searchOptions == System.IO.SearchOption.AllDirectories)
                {
                    fileNames = Directory.EnumerateDirectories(path).SelectMany(x => EnumerateFilesSafely(x, searchPattern, searchOptions, onAccessDenied));
                }
                return fileNames.Concat(Directory.EnumerateFiles(path, searchPattern));
            }
            catch (UnauthorizedAccessException)
            {
                if (onAccessDenied != null)
                {
                    onAccessDenied(path);
                }
                return Enumerable.Empty<string>();
            }
        }

        private void button_inputDir_Click(object sender, EventArgs e)
        {
            if (inputPath == "")
            {
                folderBrowserDialogInput.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                folderBrowserDialogInput.SelectedPath = inputPath;
            }
            
            if (folderBrowserDialogInput.ShowDialog() == DialogResult.OK)
            {
                inputPath = folderBrowserDialogInput.SelectedPath;
                label_inputDir_status.Text = inputPath;
                label_inputDir_status.ForeColor = System.Drawing.Color.Green;
                settings.inputPath = inputPath;
                File.WriteAllText(iniPath, js.Serialize(settings).Replace(",", ",\n\t").Replace("{", "{\n\t").Replace("}", "\n}"));
                if (inputPath != "" && outputPath != "")
                {
                    button_iconv.Enabled = true;
                    button_dconv.Enabled = true;
                    button_mconv.Enabled = true;
                }
                label_dconv_status.Text = "Waiting to start";
                label_dconv_status.ForeColor = System.Drawing.Color.Black;
                label_iconv_status.Text = "Waiting to start";
                label_iconv_status.ForeColor = System.Drawing.Color.Black;
                label_mconv_status.Text = "Waiting to start";
                label_mconv_status.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void button_outputDir_Click(object sender, EventArgs e)
        {
            if (outputPath == "")
            {
                folderBrowserDialogInput.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                folderBrowserDialogInput.SelectedPath = outputPath;
            }
                
            if (folderBrowserDialogInput.ShowDialog() == DialogResult.OK)
            {
                outputPath = folderBrowserDialogInput.SelectedPath;
                label_outputDir_status.Text = outputPath;
                label_outputDir_status.ForeColor = System.Drawing.Color.Green;
                settings.outputPath = outputPath;
                File.WriteAllText(iniPath, js.Serialize(settings).Replace(",", ",\n\t").Replace("{", "{\n\t").Replace("}", "\n}"));
                if (inputPath != "" && outputPath != "")
                {
                    button_iconv.Enabled = true;
                    button_dconv.Enabled = true;
                    button_mconv.Enabled = true;
                }
                label_dconv_status.Text = "Waiting to start";
                label_dconv_status.ForeColor = System.Drawing.Color.Black;
                label_iconv_status.Text = "Waiting to start";
                label_iconv_status.ForeColor = System.Drawing.Color.Black;
                label_mconv_status.Text = "Waiting to start";
                label_mconv_status.ForeColor = System.Drawing.Color.Black;
            }                
        }

        private void button_dconv_Click(object sender, EventArgs e)
        {
            string dbPath = Path.Combine(inputPath, "RPG_RT.edb");
            if (!File.Exists(dbPath))
            {
                MessageBox.Show("RPG_RT.edb not found. Run LCF2XML in the input directory.");
                return;
            }
            Task.Factory.StartNew(() =>
            {
                label_dconv_status.Invoke(new Action(() => label_dconv_status.Text = "Working..."));
                label_dconv_status.Invoke(new Action(() => label_dconv_status.ForeColor = System.Drawing.Color.Red));
                disableButtons();


                if (!Directory.Exists(Path.Combine(outputPath, "data")))
                {
                    Directory.CreateDirectory(Path.Combine(outputPath, "data"));
                }

                XDocument dbXML = null;

                using (StreamReader oReader = new StreamReader(dbPath, Encoding.GetEncoding("shift_jis")))
                {
                    dbXML = XDocument.Load(oReader);
                }

                var chipsets = dbXML.Descendants("chipsets").SingleOrDefault();
                int count_chipsets = chipsets.Elements("Chipset").Count();

                var terrains = dbXML.Descendants("terrains").SingleOrDefault();
                int count_terrains = terrains.Elements("Terrain").Count();

                string chipset_name = "";
                List<int> passable_data_lower;
                List<int> passable_data_upper;
                List<int> terrain_data;
                int rm2k_flags;
                int mv_flags = 0;
                int terrain_num;
                int tileID;
                string jsonData;
                List<Tileset> tilesets = new List<Tileset>();
                tilesets.Add(null);

                string missingChipsetImages = "";

                foreach (var chipset in chipsets.Elements("Chipset"))
                {
                    Tileset tileset = new Tileset();
                    tileset.id = int.Parse(chipset.Attribute("id").Value.TrimStart(new Char[] { '0' }));
                    tileset.name = chipset.Element("name").Value;
                    tileset.note = "";
                    tileset.mode = 1;
                    chipset_name = chipset.Element("chipset_name").Value;

                    if (chipset_name == "")
                    {
                        tileset.tilesetNames = new string[] { "", "", "", "", "", "", "", "", "" };
                        tileset.flags = new int[1];
                        tileset.flags[0] = 16;
                    }
                    else
                    {
                        tileset.tilesetNames = new string[] { chipset_name + "_A1", chipset_name + "_A2", "", "", "", chipset_name + "_B", chipset_name + "_C", chipset_name + "_D", "" };

                        tileset.flags = new int[3392]; //reserve tile IDs up to A2's 12th autotile
                        tileset.flags[0] = 16; //set first blank tile

                        passable_data_lower = chipset.Element("passable_data_lower").Value.Split(' ').ToList().Select(s => int.Parse(s)).ToList();
                        passable_data_upper = chipset.Element("passable_data_upper").Value.Split(' ').ToList().Select(s => int.Parse(s)).ToList();
                        terrain_data = chipset.Element("terrain_data").Value.Split(' ').ToList().Select(s => int.Parse(s)).ToList();

                        for (int n = 0; n <= 161; n++) //lower layer
                        {
                            rm2k_flags = passable_data_lower[n];
                            terrain_num = terrain_data[n];
                            if ((rm2k_flags & 1) == 0) { mv_flags += 1; } //0x0001: Impassable downward
                            if ((rm2k_flags & 2) == 0) { mv_flags += 2; } //0x0002: Impassable leftward
                            if ((rm2k_flags & 4) == 0) { mv_flags += 4; } //0x0004: Impassable rightward
                            if ((rm2k_flags & 8) == 0) { mv_flags += 8; } //0x0008: Impassable upward
                            if ((rm2k_flags & 16) != 0) { mv_flags += 16; } //0x0010: Display on normal character
                            if ((rm2k_flags & 64) != 0) { mv_flags += 128; } //0x0080: Counter from 0x0040
                            if (selectById(terrains, terrain_num, "bush_depth") != "0") { mv_flags += 64; } //0x0040: Bush
                            if (selectById(terrains, terrain_num, "damage") != "0") { mv_flags += 256; } //0x0100: Damage floor
                            if (selectById(terrains, terrain_num, "boat_pass") == "F") { mv_flags += 512; } //0x0200: Impassable by boat
                            if (selectById(terrains, terrain_num, "ship_pass") == "F") { mv_flags += 1024; } //0x0400: Impassable by ship
                            if (selectById(terrains, terrain_num, "airship_land") == "F") { mv_flags += 2048; } //0x0800: Airship cannot land                            

                            if (n >= 18) //normal tiles
                            {
                                tileID = (int)(n - 18 + 2 * Math.Floor(((double)n - 17.5) / 6) + 1);
                                tileset.flags[tileID] = mv_flags;
                            }
                            else if (n >= 6) //static autotiles
                            {
                                for (int i = 0; i <= 47; i++)
                                {
                                    tileID = 2816 + (n - 6) * 48 + i;
                                    tileset.flags[tileID] = mv_flags;                                      
                                }
                                for (int i = 0; i <= 11; i++)//make a copy of autotiles as normal tiles in D sheet
                                {
                                    tileID = (int)(i + 5 * Math.Floor(((double)i + 0.5) / 3) + 3 * Math.Ceiling(((double)n - 6) / 2) + 29 * Math.Floor(((double)n - 6) / 2) + 1 + 512);
                                    tileset.flags[tileID] = mv_flags;
                                }
                            }
                            else if (n == 5) //third waterfall
                            {
                                for (int i = 0; i <= 47; i++)
                                {
                                    tileID = 2672 + i;
                                    tileset.flags[tileID] = mv_flags;
                                }
                            }
                            else if (n == 4) //second waterfall
                            {
                                for (int i = 0; i <= 47; i++)
                                {
                                    tileID = 2384 + i;
                                    tileset.flags[tileID] = mv_flags;
                                }
                            }
                            else if (n == 3) //first waterfall
                            {
                                for (int i = 0; i <= 47; i++)
                                {
                                    tileID = 2288 + i;
                                    tileset.flags[tileID] = mv_flags;
                                }
                            }
                            else if (n == 2) //third animated autotile - deep water
                            {
                                for (int i = 0; i <= 47; i++)
                                {
                                    tileID = 2096 + i;
                                    tileset.flags[tileID] = mv_flags;
                                    tileID = 2432 + i;
                                    tileset.flags[tileID] = mv_flags;
                                    tileID = 2528 + i;
                                    tileset.flags[tileID] = mv_flags;
                                }
                            }
                            else if (n == 1) //second animated autotile
                            {
                                for (int i = 0; i <= 47; i++)
                                {
                                    tileID = 2240 + i;
                                    tileset.flags[tileID] = mv_flags;
                                }
                            }
                            else //first animated autotile
                            {
                                for (int i = 0; i <= 47; i++)
                                {
                                    tileID = 2048 + i;
                                    tileset.flags[tileID] = mv_flags;
                                }
                            }

                            mv_flags = 0;
                        }

                        for (int n = 0; n <= 143; n++) //upper layer
                        {
                            rm2k_flags = passable_data_upper[n];
                            if ((rm2k_flags & 1) == 0) { mv_flags += 1; } //0x0001: Impassable downward
                            if ((rm2k_flags & 2) == 0) { mv_flags += 2; } //0x0002: Impassable leftward
                            if ((rm2k_flags & 4) == 0) { mv_flags += 4; } //0x0004: Impassable rightward
                            if ((rm2k_flags & 8) == 0) { mv_flags += 8; } //0x0008: Impassable upward
                            if ((rm2k_flags & 16) != 0) { mv_flags += 16; } //0x0010: Display on normal character
                            if ((rm2k_flags & 64) != 0) { mv_flags += 128; } //0x0080: Counter from 0x0040                    
                            mv_flags += 512; //0x0200: Impassable by boat
                            mv_flags += 1024; //0x0400: Impassable by ship
                            mv_flags += 2048; //0x0800: Airship cannot land

                            tileID = (int)(n + 2 * Math.Floor(((double)n + 0.5) / 6) + 320 + 1);
                            tileset.flags[tileID] = mv_flags;

                            mv_flags = 0;
                        }

                        for(int i = 0; i <= 15; i++) //set various passability to last 16 transparent tiles in D tilesheet for use with "star" flagged tiles
                        {
                            tileset.flags[752 + i] = i;
                        }

                        if (!File.Exists(Path.Combine(inputPath, "ChipSet", chipset_name + ".png"))) //check for missing chipsets and copy them to the input directory
                        {
                            missingChipsetImages += chipset_name + ".png";
                            if (File.Exists(Path.Combine(rtpPath, "ChipSet", chipset_name + ".png")))
                            {
                                missingChipsetImages += " (Found in RTP)";
                                if (settings.copyMissingImagesFromRTP)
                                {
                                    File.Copy(Path.Combine(rtpPath, "ChipSet", chipset_name + ".png"), Path.Combine(inputPath, "ChipSet", chipset_name + ".png"));
                                }
                            }
                            missingChipsetImages += "\n";
                        }
                    }

                    tilesets.Add(tileset);
                }

                string tilesetPath = Path.Combine(outputPath, "data", "Tilesets.json");

                if (File.Exists(tilesetPath)) //if "Tilesets.json" already exists, it will be updated instead of overwritten
                {
                    List<Tileset> tilesetsEx = js.Deserialize<List<Tileset>>(File.ReadAllText(tilesetPath));
                    var tilesetsExA = tilesetsEx.ToArray();
                    if (tilesets.Count <= tilesetsEx.Count)
                    {
                        tilesets.CopyTo(tilesetsExA);
                    }
                    tilesets = tilesetsExA.ToList();
                }

                jsonData = js.Serialize(tilesets).Replace("[null,{", "[\nnull,\n{").Replace("},{", "},\n{").Replace("]}]", "]}\n]").Replace("\\u0027", "'");
                File.WriteAllText(tilesetPath, jsonData);

                if (missingChipsetImages != "") //write the list of missing chipset images to text file
                {
                    File.WriteAllText(Path.Combine(outputPath, "Missing Chipset Images.txt"), missingChipsetImages);
                }
                else
                {
                    if (File.Exists(Path.Combine(outputPath, "Missing Chipset Images.txt")))
                    {
                        File.Delete(Path.Combine(outputPath, "Missing Chipset Images.txt"));
                    }
                }

                label_dconv_status.Invoke(new Action(() => label_dconv_status.Text = "Done"));
                label_dconv_status.Invoke(new Action(() => label_dconv_status.ForeColor = System.Drawing.Color.Green));
                enableButtons();
            });
        }

        public static string selectById (XElement xdoc, int id, string node)
        {
            var query = from c in xdoc.Elements()
                        where (int)c.Attribute("id") == id
                        select c.Element(node).Value;
            return query.FirstOrDefault().ToString();
        }

        public class Settings
        {
            public string inputPath { get; set; }
            public string outputPath { get; set; }
            public string rtpPath { get; set; }
            public bool copyMissingImagesFromRTP { get; set; }
            public bool updateOnlyTileDataIfOutputMapExists { get; set; }
        }

        public class Tileset
        {
            public int id { get; set; }
            public int[] flags { get; set; }
            public int mode { get; set; }
            public string name { get; set; }
            public string note { get; set; }
            public string[] tilesetNames { get; set; }
        }

        public class BGX
        {
            public string name { get; set; }
            public int pan { get; set; }
            public int pitch { get; set; }
            public int volume { get; set; }
        }

        public class Map
        {
            public bool autoplayBgm { get; set; }
            public bool autoplayBgs { get; set; }
            public string battleback1Name { get; set; }
            public string battleback2Name { get; set; }
            public BGX bgm { get; set; }
            public BGX bgs { get; set; }
            public bool disableDashing { get; set; }
            public string displayName { get; set; }
            public string encounterList { get; set; }
            public int encounterStep { get; set; }
            public int height { get; set; }
            public string note { get; set; }
            public bool parallaxLoopX { get; set; }
            public bool parallaxLoopY { get; set; }
            public string parallaxName { get; set; }
            public bool parallaxShow { get; set; }
            public int parallaxSx { get; set; }
            public int parallaxSy { get; set; }
            public int scrollType { get; set; }
            public bool specifyBattleback { get; set; }
            public int tilesetId { get; set; }
            public int width { get; set; }
            public int[] data { get; set; }
            public string events { get; set; }
        }

        public class MapTreeNode
        {
            public int id { get; set; }
            public bool expanded { get; set; }
            public string name { get; set; }
            public int order { get; set; }
            public int parentId { get; set; }
            public double scrollX { get; set; }
            public double scrollY { get; set; }
        }

        public class MapName
        {
            public int id { get; set; }
            public string name { get; set; }
            public string displayName { get; set; }
        }

        private void button_mconv_Click(object sender, EventArgs e)
        {
            if (!File.Exists(Path.Combine(inputPath, "RPG_RT.emt")))
            {
                MessageBox.Show("RPG_RT.emt not found. Run LCF2XML in the input directory.");
                return;
            }
            if (!File.Exists(Path.Combine(outputPath, "data", "Tilesets.json")))
            {
                MessageBox.Show("Tilesets.json in output folder not found. Convert tileset data first.");
                return;
            }
            Task.Factory.StartNew(() =>
            {
                label_mconv_status.Invoke(new Action(() => label_mconv_status.Text = "Working..."));
                label_mconv_status.Invoke(new Action(() => label_mconv_status.ForeColor = System.Drawing.Color.Red));
                disableButtons();

                if (!Directory.Exists(Path.Combine(outputPath, "data")))
                {
                    Directory.CreateDirectory(Path.Combine(outputPath, "data"));
                }

                XDocument mapTreeXML = null;

                using (StreamReader oReader = new StreamReader(Path.Combine(inputPath, "RPG_RT.emt"), Encoding.GetEncoding("shift_jis")))
                {
                    mapTreeXML = XDocument.Load(oReader);
                }
                var mapNodesXML = mapTreeXML.Descendants("maps").SingleOrDefault();
                List<int> treeOrder = mapTreeXML.Descendants("tree_order").SingleOrDefault().Value.Split(' ').ToList().Select(s => int.Parse(s)).ToList();

                string mapFileName = "";
                string mapFileName2k = "";
                string nodeJsonData = "";
                string mapJsonData = "";
                List<int> lowerLayer;
                List<int> upperLayer;
                List<int> autoTileLayer;
                List<int> lower_layer;
                List<int> upper_layer;
                string imagePath;
                Map mapTemp = new Map();
                string mapJsonDataTemp;
                string eventsTemp = "\n\"events\":[\n]\n}";
                int eventsPos;
                List<MapTreeNode> mapTreeNodes = new List<MapTreeNode>();
                mapTreeNodes.Add(null);
                int trimLength;

                List<MapName> mapNames = new List<MapName>();
                MapName mapName;
                bool getNamesFromFile = File.Exists(Path.Combine(outputPath, "MapNames.json"));
                if (getNamesFromFile)
                {
                    mapNames = js.Deserialize<List<MapName>>(File.ReadAllText(Path.Combine(outputPath, "MapNames.json")));
                }

                List<Tileset> tilesets;
                Tileset tileset;
                int tile;
                tilesets = js.Deserialize<List<Tileset>>(File.ReadAllText(Path.Combine(outputPath, "data", "Tilesets.json")));
                List<int> subtiles = new List<int> { 33, 43, 45, 34, 20, 36, 21, 22 };//list of passable subtiles for "square" passability autotiles
                int flags = 0;


                foreach (var mapInfo in mapNodesXML.Elements("MapInfo"))
                {
                    MapTreeNode mapTreeNode = new MapTreeNode();
                    mapFileName = "Map" + mapInfo.Attribute("id").Value.Substring(1);
                    mapFileName2k = "Map" + mapInfo.Attribute("id").Value;
                    mapTreeNode.id = int.Parse(mapInfo.Attribute("id").Value);
                    if (mapInfo.Element("expanded_node").Value == "F") { mapTreeNode.expanded = false; } else { mapTreeNode.expanded = true; }
                    mapTreeNode.name = mapInfo.Element("name").Value;
                    mapTreeNode.order = treeOrder.IndexOf(mapTreeNode.id);
                    mapTreeNode.parentId = int.Parse(mapInfo.Element("parent_map").Value);
                    mapTreeNode.scrollX = double.Parse(mapInfo.Element("scrollbar_x").Value);
                    mapTreeNode.scrollY = double.Parse(mapInfo.Element("scrollbar_y").Value);

                    if (mapFileName != "Map000")
                    {
                        mapTreeNodes.Add(mapTreeNode);

                        XDocument mapXML = null;
                        using (StreamReader oReader = new StreamReader(Path.Combine(inputPath, mapFileName2k + ".emu"), Encoding.GetEncoding("shift_jis")))
                        {
                            mapXML = XDocument.Load(oReader);
                        }
                        var mapDataXML = mapXML.Descendants("Map").SingleOrDefault();

                        Map map = new Map();

                        map.autoplayBgs = false;
                        map.battleback1Name = "";
                        map.battleback2Name = "";

                        map.bgm = new BGX { };
                        map.bgm.name = "";
                        map.bgm.pan = 0;
                        map.bgm.pitch = 100;
                        map.bgm.volume = 90;

                        map.bgs = new BGX { };
                        map.bgs.name = "";
                        map.bgs.pan = 0;
                        map.bgs.pitch = 100;
                        map.bgs.volume = 90;
                        map.disableDashing = false;

                        if (mapTreeNode.name.Length <= 4) { trimLength = 0; } else { trimLength = 5; }
                        map.displayName = mapTreeNode.name.Substring(trimLength);

                        map.encounterList = "[]";
                        map.encounterStep = int.Parse(mapInfo.Element("encounter_steps").Value);
                        map.height = int.Parse(mapDataXML.Element("height").Value);
                        map.note = "";

                        //Parallax backgronds processing
                        if (mapDataXML.Element("parallax_loop_x").Value == "F") { map.parallaxLoopX = false; } else { map.parallaxLoopX = true; }
                        if (mapDataXML.Element("parallax_loop_y").Value == "F") { map.parallaxLoopY = false; } else { map.parallaxLoopY = true; }
                        map.parallaxSx = -int.Parse(mapDataXML.Element("parallax_sx").Value);
                        map.parallaxSy = -int.Parse(mapDataXML.Element("parallax_sy").Value);
                        map.parallaxName = mapDataXML.Element("parallax_name").Value;
                        imagePath = "";
                        if (map.parallaxName != "")
                        {
                            if (File.Exists(Path.Combine(inputPath, "Panorama", map.parallaxName + ".png")))
                            {
                                imagePath = Path.Combine(inputPath, "Panorama", map.parallaxName + ".png");
                            }
                            else if (File.Exists(Path.Combine(rtpPath, "Panorama", map.parallaxName + ".png")))
                            {
                                imagePath = Path.Combine(rtpPath, "Panorama", map.parallaxName + ".png");
                            }
                            if (imagePath != "")
                            {
                                image = new Bitmap(imagePath, true);
                                image = new Bitmap(image); //convert to 32bppArgb
                                image = ResizeBitmap(image, image.Width * 3, image.Height * 3);
                                if (map.parallaxSx == 0 || map.parallaxSy == 0) //if parallax is static or scrolling only in one direction, add trailing ! to filename to make it not move with player
                                {
                                    map.parallaxName = Path.Combine(Path.GetDirectoryName(map.parallaxName), "!" + Path.GetFileName(map.parallaxName));
                                }

                                imagePath = Path.Combine(outputPath, "img", "parallaxes", map.parallaxName + ".png");
                                if (!Directory.Exists(Path.GetDirectoryName(imagePath)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(imagePath));
                                }
                                image.Save(imagePath, ImageFormat.Png);
                                image.Dispose();
                                imagePath = "";
                            }
                            map.parallaxShow = true;
                        }
                        else
                        {
                            map.parallaxShow = false;
                        }
                        //End of parallax backgronds processing

                        map.scrollType = int.Parse(mapDataXML.Element("scroll_type").Value);
                        map.specifyBattleback = false;
                        map.tilesetId = int.Parse(mapDataXML.Element("chipset_id").Value);
                        map.width = int.Parse(mapDataXML.Element("width").Value);
                        map.events = "[]";

                        //Map tile data processing
                        map.data = new int[map.width * map.height * 6];
                        lower_layer = mapDataXML.Element("lower_layer").Value.Split(' ').ToList().Select(s => int.Parse(s)).ToList();
                        upper_layer = mapDataXML.Element("upper_layer").Value.Split(' ').ToList().Select(s => int.Parse(s)).ToList();
                        upperLayer = new List<int>(new int[upper_layer.Count]);
                        lowerLayer = new List<int>(new int[lower_layer.Count]);
                        autoTileLayer = new List<int>(new int[lower_layer.Count]);
                        
                        tileset = tilesets[map.tilesetId];

                        for (int n = 0; n <= lower_layer.Count - 1; n++)
                        {
                            tile = convertTileId(lower_layer[n]);
                            if (tileset.flags.Length > 1) { flags = tileset.flags[tile]; } else { flags = 0; }

                            if (tile >= 2048)//split autotiles and normal lower layer tiles
                            {
                                autoTileLayer[n] = tile;
                                if ((flags & 16) == 16)//for star autotiles put passable transparent tile in third layer
                                {
                                    if((flags & 15) == 15)//"square" passability autotiles are 4-way impassable
                                    {
                                        if (subtiles.Contains((tile - 2816) % 48))//only subtiles from list are made passable
                                        {
                                            lowerLayer[n] = 752;
                                        }
                                    }
                                    else
                                    {
                                        lowerLayer[n] = 752 + (flags & 15);
                                    }
                                    
                                }
                            }
                            else
                            {
                                lowerLayer[n] = tile;
                                if ((flags & 16) == 16 && (flags & 15) != 15)//for star lower layer tiles put passable transparent tile in first layer
                                {
                                    autoTileLayer[n] = 752 + (flags & 15);
                                }
                            }
                            map.data[n] = autoTileLayer[n];//autotiles go in first layer (ground layer)
                            map.data[n + map.width * map.height * 2] = lowerLayer[n]; //normal lower layer tiles go in third layer (first design layer)
                        }

                        for (int n = 0; n <= upper_layer.Count - 1; n++)
                        {
                            tile = convertTileId(upper_layer[n]);
                            upperLayer[n] = tile;
                            if (tileset.flags.Length > 1) { flags = tileset.flags[tile]; } else { flags = 0; }
                            if ((flags & 16) == 16 && (flags & 15) != 15)//for star upper layer tiles put passable transparent tile in unoccupied layer
                            {
                                if(lowerLayer[n] == 0)
                                {
                                    lowerLayer[n] = 752 + (flags & 15);
                                }
                                else
                                {
                                    autoTileLayer[n] = 752 + (flags & 15);
                                }
                            }
                            map.data[n + map.width * map.height * 3] = upperLayer[n];//upper layer tiles go in fourth layer (second design layer)
                        }
                        //End of map tile data processing

                        mapName = mapNames.FirstOrDefault(a => a.id == mapTreeNode.id); //use map names from MapNames.json
                        if (getNamesFromFile && mapName != null)
                        {
                            map.displayName = mapName.displayName;
                            mapTreeNode.name = mapName.name;
                        }

                        //if set in settings, update only map tile data if map already exists in output directory
                        if (File.Exists(Path.Combine(outputPath, "data", mapFileName + ".json")) && settings.updateOnlyTileDataIfOutputMapExists)
                        {
                            mapTemp.data = new int[map.width * map.height * 6];
                            map.bgm = new BGX { };
                            map.bgs = new BGX { };
                            mapJsonDataTemp = File.ReadAllText(Path.Combine(outputPath, "data", mapFileName + ".json")).Replace("encounterList\":[]", "encounterList\":\"[]\"");
                            eventsPos = mapJsonDataTemp.IndexOf("\n\"events\":[");
                            eventsTemp = mapJsonDataTemp.Substring(eventsPos);
                            mapJsonDataTemp = mapJsonDataTemp.Substring(0, eventsPos) + "\n\"events\":\"[]\"}";
                            mapTemp = js.Deserialize<Map>(mapJsonDataTemp);
                            mapTemp.data = map.data;
                            if (getNamesFromFile)
                            {
                                mapTemp.displayName = map.displayName;
                            }
                            map = mapTemp;
                        }

                        mapJsonData = js.Serialize(map);
                        mapJsonData = mapJsonData.Replace("{\"autoplayBgm", "{\n\"autoplayBgm").Replace("encounterList\":\"[]\"", "encounterList\":[]").Replace(",\"data", ",\n\"data")
                            .Replace("\"events\":\"[]\"}", eventsTemp).Replace("\\u0027", "'");
                        File.WriteAllText(Path.Combine(outputPath, "data", mapFileName + ".json"), mapJsonData);

                        mapJsonData = "";
                        eventsTemp = "\n\"events\":[\n]\n}";
                    }
                }

                if (!File.Exists(Path.Combine(outputPath, "data", "MapInfos.json")) || !settings.updateOnlyTileDataIfOutputMapExists)//do not overwrite existing "MapInfos.json" is checkbox is set
                {
                    nodeJsonData = js.Serialize(mapTreeNodes).Replace("[null,{", "[\nnull,\n{").Replace("},{", "},\n{").Replace("}]", "}\n]").Replace("\\u0027", "'");
                    File.WriteAllText(Path.Combine(outputPath, "data", "MapInfos.json"), nodeJsonData);
                }
                
                label_mconv_status.Invoke(new Action(() => label_mconv_status.Text = "Done"));
                label_mconv_status.Invoke(new Action(() => label_mconv_status.ForeColor = System.Drawing.Color.Green));
                enableButtons();
            });
        }

        public static int convertTileId (int tileId)
        {
            if (tileId >= 10000) //upper layer
            {
                tileId = (int)(tileId - 10000 + 2 * Math.Floor(((double)tileId - 10000 + 0.5) / 6) + 320 + 1);
            }
            else if (tileId >= 5000) //lower layer
            {
                tileId = (int)(tileId - 5000 + 2 * Math.Floor(((double)tileId - 5000 + 0.5) / 6) + 1);
            }
            else if (tileId >= 4000) //static autotiles
            {
                tileId = (int)(2816 + tileId - 4000 - 2 * Math.Floor(((double)tileId - 4000) / 50));
            }
            else if (tileId >= 3100) //third waterfall
            {
                tileId = 2672;// + tileId - 3100;
            }
            else if (tileId >= 3050) //second waterfall
            {
                tileId = 2384;// + tileId - 3050;
            }
            else if (tileId >= 3000) //first waterfall
            {
                tileId = 2288;// + tileId - 3000;
            }
            else if (tileId >= 2000) //third animated autotile - deep water
            {
                if (tileId >= 2050 && tileId % 50 == 0)
                {
                    tileId = 2432 + aatn.IndexOf(tileId-2000); //for interfaced autotiles
                }
                else
                {
                    tileId = 2096 + tileId % 50;
                }
                    
            }
            else if (tileId >= 1000) //second animated autotile
            {
                //tileId = 2240 + tileId % 50;
                if (tileId >= 1050 && tileId % 50 == 0)
                {
                    tileId = 2528 + aatn.IndexOf(tileId - 1000); //for interfaced autotiles
                }
                else
                {
                    tileId = 2240 + tileId % 50;
                }
            }
            else //first animated autotile
            {
                //tileId = 2048 + tileId % 50;
                if (tileId >= 50 && tileId % 50 == 0)
                {
                    tileId = 2528 + aatn.IndexOf(tileId); //for interfaced autotiles
                }
                else
                {
                    tileId = 2048 + tileId % 50;
                }
            }

            return tileId;
        }

        private void copyMissingImagesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.copyMissingImagesFromRTP = copyMissingImagesCheckBox.Checked;
            File.WriteAllText(iniPath, js.Serialize(settings).Replace(",", ",\n\t").Replace("{", "{\n\t").Replace("}", "\n}"));
        }

        private void updateOnlyMapDataCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            settings.updateOnlyTileDataIfOutputMapExists = updateOnlyMapDataCheckBox.Checked;
            File.WriteAllText(iniPath, js.Serialize(settings).Replace(",", ",\n\t").Replace("{", "{\n\t").Replace("}", "\n}"));
        }

        private void enableButtons()
        {
            button_iconv.Invoke(new Action(() => button_iconv.Enabled = true));
            button_dconv.Invoke(new Action(() => button_dconv.Enabled = true));
            button_mconv.Invoke(new Action(() => button_mconv.Enabled = true));
            button_inputDir.Invoke(new Action(() => button_inputDir.Enabled = true));
            button_outputDir.Invoke(new Action(() => button_outputDir.Enabled = true));
            copyMissingImagesCheckBox.Invoke(new Action(() => copyMissingImagesCheckBox.Enabled = true));
            updateOnlyMapDataCheckBox.Invoke(new Action(() => updateOnlyMapDataCheckBox.Enabled = true));
            return;
        }

        private void disableButtons()
        {
            button_iconv.Invoke(new Action(() => button_iconv.Enabled = false));
            button_dconv.Invoke(new Action(() => button_dconv.Enabled = false));
            button_mconv.Invoke(new Action(() => button_mconv.Enabled = false));
            button_inputDir.Invoke(new Action(() => button_inputDir.Enabled = false));
            button_outputDir.Invoke(new Action(() => button_outputDir.Enabled = false));
            copyMissingImagesCheckBox.Invoke(new Action(() => copyMissingImagesCheckBox.Enabled = false));
            updateOnlyMapDataCheckBox.Invoke(new Action(() => updateOnlyMapDataCheckBox.Enabled = false));
            return;
        }


    }
}
