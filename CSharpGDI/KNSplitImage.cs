using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CSharpGDI
{
    /***************************************************************************
     * Created by maxun@live.cn
     * Timestamp 2017/04/07
     * description: 将某一个图片load，并根据block大小分割之后重新进行排列
     * 
     * 使用方法
     * 构建一个List<int>，用于描述原始图片中每一个小块的起始位置
     * 新生成的图片将根据list中的索引从左到右进行重绘，以下图为例
     * 
     * +-----------+-------------+-----------+
     * |     A     |      C      |     B     |
     * +-----------+-------------+-----------+
     * 以上图形定义了一幅图片，其实际顺序应该是ABC三个字母，通过本类对其进行重绘
     * 假设上面图形中，每一个小块的大小是25，那么，从左到右实际应该生成的新图片中每一个
     * 图片的位置应该就是0,50,25，所以首先定义这个实际坐标列表
     * List<int> imglist = new List<int>(){0, 50 ,25};
     * 然后第二步应该调用SetOption函数告诉处理函数应该如何进行分割图片，
     * SetOption(imglist, w, h);其中w是每一个小块的大小，在这个例子中也就是25，h
     * 代表高度，假设为100，设置好了之后就可以使用SAVETO*函数进行处理了
     * 
     * SaveToFile是将新生成的图片保存到一个绝对路径中
     * SaveToBitmap是将图片生成之后保存在内存中
     * 
     * 
     * 
     * 注意：在使用这个类的时候一定要自行处理异常
     ***************************************************************************/

    class KNSplitImage
    {
        public KNSplitImage()
        {
        }

        ~KNSplitImage()
        {
            Unload();
        }

        /// <summary>
        /// 清除已经调用过的资源
        /// </summary>
        public void Unload()
        {
            if (bmp_source_ != null)
            {
                bmp_source_.Dispose();
                bmp_source_ = null;
            }
            if (position_ != null && position_.Count > 0)
            {
                position_.Clear();
            }
            block_width_ = 0;
            block_height_ = 0;
        }

        /// <summary>
        /// 从一个绝对路径加载一张原图片
        /// </summary>
        /// <param name="source">要加载的文件全路径</param>
        public void Load(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new Exception("源文件路径不能为空");
            }
            else if (!System.IO.File.Exists(source))
            {
                throw new Exception("源文件不存在");
            }

            Unload();

            bmp_source_ = new Bitmap(source);
        }

        /// <summary>
        /// 指定每一个小块的位置
        /// </summary>
        /// <param name="position">每一个小块的实际X坐标(一定是实际绝对坐标)</param>
        /// <param name="block_width">小块的宽度</param>
        /// <param name="block_height">小块的高度</param>
        public void SetOptions(List<int> position, float block_width, float block_height)
        {
            if (position == null || position.Count() > 0)
            {
                throw new Exception("坐标列表不能为空，否则处理函数无法得知如何对图片进行处理");
            }
            else if (block_height == 0 || block_height == 0)
            {
                throw new Exception("块的高度或宽度不能为0");
            }
            else if (position.Where(s => s <= 0).Any())
            {
                throw new Exception("坐标列表中的值不能有负数，坐标应该表明的是块在图片中的绝对X值");
            }

            position_ = position;
            block_height_ = block_height;
            block_width_ = block_width;
            total_width_ = position.Count() * block_width;
        }

        /// <summary>
        /// 处理并将结果保存到某一个文件
        /// </summary>
        /// <param name="fullpath">要保存的文件全路径（包含文件名）</param>
        /// <param name="overwrite">是否覆盖保存</param>
        public void SaveToFile(string fullpath, bool overwrite = true)
        {
            if (overwrite)
            {
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
            }
            else if (System.IO.File.Exists(fullpath))
            {
                throw new Exception("目标文件已经存在");
            }

            SaveToBitmap().Save(fullpath);
        }

        /// <summary>
        /// 处理图片，并将图片保存到Bitmap对象进行返回
        /// </summary>
        /// <returns>返回图片对象</returns>
        public Bitmap SaveToBitmap()
        {
            if (position_ == null || position_.Count == 0)
            {
                throw new Exception("Options参数没有设置，应该首先调用SetOption告诉模块如何对图片进行处理");
            }
            if (block_height_ == 0 || block_width_ == 0 || total_width_ == 0)
            {
                throw new Exception("新生成的图片大小高度或宽度为0，请检查SetOption的列表值");
            }

            Bitmap bmp_new = new Bitmap((int)total_width_, (int)block_height_);
            for (int vx = 0, i = 0; i < position_.Count(); i++)
            {
                for (int x = position_[i]; x < position_[i] + block_width_; x++)
                {
                    for (int y = 0; y < block_height_; y++)
                    {
                        bmp_new.SetPixel(vx, y, bmp_source_.GetPixel(x, y));
                    }
                    vx++;
                }
            }

            return bmp_new;
        }


        // 原始图片，当LOAD之后，原始图片会加载到这个对象中
        private Bitmap bmp_source_ = null;
        // 保存图片中每一个小块的位置
        private List<int> position_ = null;
        // 每一个小块的高度
        private float block_height_ = 0.0f;
        // 每一个小块的宽度
        private float block_width_ = 0.0f;
        // 新图片的总宽度
        private float total_width_ = 0.0f;
    }
}
