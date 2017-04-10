using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CSharpGDI
{
    class Program
    {
        static void Main(string[] args)
        {
            KNSplitImage img = new KNSplitImage();
            img.Load(args[0]);

            List<int> pos = new List<int>();
            pos.Add(50);    // 第一个小块的图片在原图的X=50的位置
            pos.Add(0);     // 第二个小块的图片在原图的X=0的位置

            img.SetOptions(pos, 50, 100);
            img.SaveToFile(args[1]);
        }
    }
}
