using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorGenerator
{
    class Program
    {
        struct CharCode
        {
            public CharCode(char c, int front, int back)
            {
                this.c = c;
                this.front = front;
                this.back = back;
            }

            public char c;
            public int front;
            public int back;
        }

        struct Color
        {
            public Color(byte r, byte g, byte b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }

            public byte r;
            public byte g;
            public byte b;

            public override string ToString() => $"({Fix($"{r}", 3, '0')} {Fix($"{g}", 3, '0')} {Fix($"{b}", 3, '0')})";

            public static float GetHue(Color c)
            {
                float r = c.r / 255;
                float g = c.g / 255;
                float b = c.b / 255;

                float max = Math.Max(Math.Max(r,g),b);
                float min = Math.Min(Math.Min(r,g),b);

                if (max == r) return (g - b) / (max - min);
                if (max == g) return 2.0f + (b - r) / (max - min);
                if (max == b) return 4.0f + (r - g) / (max - min);

                throw new Exception();
            }
        }

        static readonly Dictionary<int, Color> RGBColors = new Dictionary<int, Color>()
        {
            {0, new Color(12,12,12)},   {1, new Color(0,55,218)},
            {2, new Color(19,161,14)},  {3, new Color(58,150,221)},
            {4, new Color(197,15,31)},  {5, new Color(105,15,110)},
            {6, new Color(193,156,0)},  {7, new Color(204,204,204)},
            {8, new Color(118,118,118)},{9, new Color(59,120,255)},
            {10,new Color(22,198,12)},  {11,new Color(97,214,214)},
            {12,new Color(231,73,86)},  {13,new Color(180,0,158)},
            {14,new Color(249,241,165)},{15,new Color(242,242,242)},
        };

        static Dictionary<char, float> chars;

        static string sampleChars = "░▒▓";
        const string fontName = "raster fonts";
        const int scanQuality = 500; //Aka the size of the font

        static List<Tuple<CharCode, Color>> data = new List<Tuple<CharCode, Color>>();

        static void Main(string[] args)
        {
            //Collect chars
            chars = CharacterBrightness.GetData(sampleChars.ToArray(), new System.Drawing.Font(fontName, scanQuality));

            //Add same colors
            foreach (var c in RGBColors)
            {
                data.Add(new Tuple<CharCode, Color>(new CharCode('*', c.Key,c.Key),c.Value));
            }

            //Add other colors
            for (int back = 0; back < RGBColors.Count; ++back)
            {
                for (int front = 0; front < RGBColors.Count; ++front)
                {
                    if (front == back) continue;

                    for (int kvp = 0; kvp < chars.Count; ++kvp)
                    {
                        var c = new Color();
                        float mpr = chars.ElementAt(kvp).Value;

                        c.r = (byte)(RGBColors[back].r * mpr + RGBColors[front].r * (1 - mpr));
                        c.g = (byte)(RGBColors[back].g * mpr + RGBColors[front].g * (1 - mpr));
                        c.b = (byte)(RGBColors[back].b * mpr + RGBColors[front].b * (1 - mpr));

                        data.Add(new Tuple<CharCode, Color>(new CharCode(chars.ElementAt(kvp).Key, front, back), c));
                    }
                }
            }

            //Sort
            data = data.OrderBy(a => Color.GetHue(a.Item2)).ToList();

            Console.Clear();

            //Print
            int count = 0;
            foreach(var d in data)
            {
                ++count;
                Console.ForegroundColor = (ConsoleColor)d.Item1.front;
                Console.BackgroundColor = (ConsoleColor)d.Item1.back;

                Console.Write($"{new string(d.Item1.c,10)}");
                Console.ResetColor();
                Console.Write($"  Count: {Fix($"{count}",4,'0')} RGB: ({Fix($"{d.Item2.r}",3,'0')},{Fix($"{d.Item2.g}", 3, '0')},{Fix($"{d.Item2.b}", 3, '0')})");
                Console.Write($" Code: {Fix($"{d.Item1.front}",2,'0')}:{Fix($"{d.Item1.back}", 2, '0')} Char: {(byte)d.Item1.c}");

                Console.WriteLine();
            }

            Console.WriteLine("Finsihed.");
            Console.ReadLine();
        }

        static string Fix(string s, int l, char z)
        {
            if (s.Length == l) return s;
            return s.Length > l ? s.Substring(0, l) : new string(z, l - s.Length) + s;
        }
    }
}
