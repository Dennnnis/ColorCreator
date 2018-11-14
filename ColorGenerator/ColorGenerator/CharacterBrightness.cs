using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace ColorGenerator
{
    static class CharacterBrightness
    {

        private static SizeF MeasureString(string s, Font f)
        {
            using (var image = new Bitmap(1, 1))
            {
                using (var g = Graphics.FromImage(image))
                {
                    return g.MeasureString(s, f,int.MaxValue,StringFormat.GenericTypographic);
                }
            }
        }

        public static Dictionary<char,float> GetData(char[] chars,Font font)
        {
            Dictionary<char,float> final = new Dictionary<char, float>();

            foreach (char c in chars)
            {
                SizeF sizeF = MeasureString(c.ToString(),font);
                Size size = new Size((int)sizeF.Width, (int)sizeF.Height);

                Image img = new Bitmap((int)size.Width,(int)size.Height);

                Console.Write($"Scanning [{c}]...");
                using (Graphics gph = Graphics.FromImage(img))
                {
                    gph.FillRectangle(Brushes.White, new Rectangle(0,0,size.Width,size.Height));
                    gph.DrawString(c.ToString(), font, Brushes.Black,0,0,StringFormat.GenericTypographic);
                }

                img.Save($"d{(int)c}.png");

                int white = 0;

                for (int y = 0; y < size.Height; ++y)
                {
                    for (int x = 0; x < size.Width; ++x)
                    {
                        if (((Bitmap)img).GetPixel(x,y).R > 0)
                            ++white;
                    }
                }

                float prec = (float)white / (size.Width * size.Height);

                Console.WriteLine($" {prec * 100}% White");
                final.Add(c,prec);
            }

            Console.WriteLine($"finsihed scanning {final.Count} colors.");
            return final;            
        }
    }
}
