using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using UnityEngine;

public static class Common
{


    public static MotionType MotionType;

    public static float GetCross(Vector2 p1, Vector2 p2, Vector2 p)
    {
        return (p2.x - p1.x) * (p.y - p1.y) - (p.x - p1.x) * (p2.y - p1.y);
    }
    //计算一个点是否在矩形里  2d
    public static bool ContainsQuadrangle(Vector2 leftDownP2, Vector2 leftUpP1, Vector2 rightDownP3, Vector2 rightUpP4, Vector2 p)
    {

        float value1 = GetCross(leftUpP1, leftDownP2, p);

        float value2 = GetCross(rightDownP3, rightUpP4, p);

        if (value1 * value2 < 0) return false;

        float value3 = GetCross(leftDownP2, rightDownP3, p);

        float value4 = GetCross(rightUpP4, leftUpP1, p);

        if (value3 * value4 < 0) return false;

        return true;
    }



    public static List<Vector2> Sample2D(float width, float height, float r, int k = 30)
    {
        return Sample2D((int)DateTime.Now.Ticks, width, height, r, k);
    }

    public static List<Vector2> Sample2D(int seed, float width, float height, float r, int k = 30)
    {
        // STEP 0

        // 维度，平面就是2维
        var n = 2;

        // 计算出合理的cell大小
        // cell是一个正方形，为了保证每个cell内部不可能出现多个点，那么cell内的任意点最远距离不能大于r
        // 因为cell内最长的距离是对角线，假设对角线长度是r，那边长就是下面的cell_size
        var cell_size = r / Math.Sqrt(n);

        // 计算出有多少行列的cell
        var cols = (int)Math.Ceiling(width / cell_size);
        var rows = (int)Math.Ceiling(height / cell_size);

        // cells记录了所有合法的点
        var cells = new List<Vector2>();

        // grids记录了每个cell内的点在cells里的索引，-1表示没有点
        var grids = new int[rows, cols];
        for (var i = 0; i < rows; ++i)
        {
            for (var j = 0; j < cols; ++j)
            {
                grids[i, j] = -1;
            }
        }

        // STEP 1
        var random = new System.Random(seed);

        // 随机选一个起始点
        var x0 = new Vector2(random.Next((int)width), random.Next((int)height));
        var col = (int)Math.Floor(x0.x / cell_size);
        var row = (int)Math.Floor(x0.y / cell_size);

        var x0_idx = cells.Count;
        cells.Add(x0);
        grids[row, col] = x0_idx;

        var active_list = new List<int>();
        active_list.Add(x0_idx);

        // STEP 2
        while (active_list.Count > 0)
        {
            // 随机选一个待处理的点xi
            var xi_idx = active_list[random.Next(active_list.Count)]; // 区间是[0,1)，不用担心溢出。
            var xi = cells[xi_idx];
            var found = false;

            // 以xi为中点，随机找与xi距离在[r,2r)的点xk，并判断该点的合法性
            // 重复k次，如果都找不到，则把xi从active_list中去掉，认为xi附近已经没有合法点了
            for (var i = 0; i < k; ++i)
            {
                var dir = UnityEngine.Random.insideUnitCircle;
                var xk = xi + (dir.normalized * r + dir * r); // [r,2r)
                if (xk.x < 0 || xk.x >= width || xk.y < 0 || xk.y >= height)
                {
                    continue;
                }

                col = (int)Math.Floor(xk.x / cell_size);
                row = (int)Math.Floor(xk.y / cell_size);

                if (grids[row, col] != -1)
                {
                    continue;
                }

                // 要判断xk的合法性，就是要判断有附近没有点与xk的距离小于r
                // 由于cell的边长小于r，所以只测试xk所在的cell的九宫格是不够的（考虑xk正好处于cell的边缘的情况）
                // 正确做法是以xk为中心，做一个边长为2r的正方形，测试这个正方形覆盖到所有cell
                var ok = true;
                var min_r = (int)Math.Floor((xk.y - r) / cell_size);
                var max_r = (int)Math.Floor((xk.y + r) / cell_size);
                var min_c = (int)Math.Floor((xk.x - r) / cell_size);
                var max_c = (int)Math.Floor((xk.x + r) / cell_size);
                for (var or = min_r; or <= max_r; ++or)
                {
                    if (or < 0 || or >= rows)
                    {
                        continue;
                    }

                    for (var oc = min_c; oc <= max_c; ++oc)
                    {
                        if (oc < 0 || oc >= cols)
                        {
                            continue;
                        }

                        var xj_idx = grids[or, oc];
                        if (xj_idx != -1)
                        {
                            var xj = cells[xj_idx];
                            var dist = (xj - xk).magnitude;
                            if (dist < r)
                            {
                                ok = false;
                                goto end_of_distance_check;
                            }
                        }
                    }
                }

            end_of_distance_check:
                if (ok)
                {
                    var xk_idx = cells.Count;
                    cells.Add(xk);

                    grids[row, col] = xk_idx;
                    active_list.Add(xk_idx);

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                active_list.Remove(xi_idx);
            }
        }

        return cells;
    }

    /// <summary>
    /// 根据图片路径返回图片的字节流byte[]
    /// </summary>
    /// <param name="imagePath">图片路径</param>
    /// <returns>返回的字节流</returns>
    private static byte[] GetImageByte(string imagePath)
    {
        FileStream files = new FileStream(imagePath, FileMode.Open);
        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();
        return imgByte;
    }

    /// <summary>
    /// 缩放图像
    /// </summary>
    /// <param name="originalImagePath">图片原始路径</param>
    /// <param name="width">缩放图的宽</param>
    /// <param name="height">缩放图的高</param>
    /// <param name="model">缩放模式</param>
    /// <param name="size">原始尺寸</param>
    public static byte [] MakeThumNail(string originalImagePath, int width, int height, string model,out Vector2 size)
    {
        System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath);


        size.x = originalImage.Width;
        size.y = originalImage.Height;

        byte[] bytes;
        MemoryStream ms = new MemoryStream();

        if (width == originalImage.Width && height == originalImage.Height)
        {
            originalImage.Save(ms,ImageFormat.Jpeg);

            bytes = ms.GetBuffer();
            ms.Dispose();
            return bytes;
        }

        int thumWidth = width;      //缩略图的宽度
        int thumHeight = height;    //缩略图的高度

        int x = 0;
        int y = 0;

        int originalWidth = originalImage.Width;    //原始图片的宽度
        int originalHeight = originalImage.Height;  //原始图片的高度

        switch (model)
        {
            case "HW":      //指定高宽缩放,可能变形
                break;
            case "W":       //指定宽度,高度按照比例缩放
                thumHeight = originalImage.Height * width / originalImage.Width;
                break;
            case "H":       //指定高度,宽度按照等比例缩放
                thumWidth = originalImage.Width * height / originalImage.Height;
                break;
            case "Cut":
                if ((double)originalImage.Width / (double)originalImage.Height > (double)thumWidth / (double)thumHeight)
                {
                    originalHeight = originalImage.Height;
                    originalWidth = originalImage.Height * thumWidth / thumHeight;
                    y = 0;
                    x = (originalImage.Width - originalWidth) / 2;
                }
                else
                {
                    originalWidth = originalImage.Width;
                    originalHeight = originalWidth * height / thumWidth;
                    x = 0;
                    y = (originalImage.Height - originalHeight) / 2;
                }
                break;
            default:
                break;
        }

        //新建一个bmp图片
        System.Drawing.Image bitmap = new System.Drawing.Bitmap(thumWidth, thumHeight);

        //新建一个画板
        System.Drawing.Graphics graphic = System.Drawing.Graphics.FromImage(bitmap);

        //设置高质量查值法
        graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

        //设置高质量，低速度呈现平滑程度
        graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

        //清空画布并以透明背景色填充
        graphic.Clear(System.Drawing.Color.Transparent);

        //在指定位置并且按指定大小绘制原图片的指定部分
        graphic.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, thumWidth, thumHeight), new System.Drawing.Rectangle(x, y, originalWidth, originalHeight), System.Drawing.GraphicsUnit.Pixel);

       
        try
        {
           
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            bytes = ms.GetBuffer();
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            originalImage.Dispose();
            bitmap.Dispose();
            graphic.Dispose();
            ms.Dispose();
        }
        return bytes;
    }



    #region Easing Curves

    public static float Linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    public static float Clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);
        float retval = 0.0f;
        float diff = 0.0f;
        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;
        return retval;
    }

    public static float Spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static float EaseInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    public static float EaseOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    public static float EaseInOutQuad(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value + start;
        value--;
        return -end / 2 * (value * (value - 2) - 1) + start;
    }

    public static float EaseInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float EaseOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    public static float EaseInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value + 2) + start;
    }

    public static float EaseInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    public static float EaseOutQuart(float start, float end, float value)
    {
        value--;
        end -= start;
        return -end * (value * value * value * value - 1) + start;
    }

    public static float EaseInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value * value + start;
        value -= 2;
        return -end / 2 * (value * value * value * value - 2) + start;
    }

    public static float EaseInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    public static float EaseOutQuint(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value * value * value + 1) + start;
    }

    public static float EaseInOutQuint(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value * value * value + 2) + start;
    }

    public static float EaseInSine(float start, float end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
    }

    public static float EaseOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
    }

    public static float EaseInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
    }

    public static float EaseInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
    }

    public static float EaseOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
    }

    public static float EaseInOutExpo(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
    }

    public static float EaseInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
    }

    public static float EaseOutCirc(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * Mathf.Sqrt(1 - value * value) + start;
    }

    public static float EaseInOutCirc(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
    }

    /* GFX47 MOD START */
    public static float EaseInBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        return end - EaseOutBounce(0, end, d - value) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //public static float bounce(float start, float end, float value){
    public static float EaseOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < (1 / 2.75f))
        {
            return end * (7.5625f * value * value) + start;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return end * (7.5625f * (value) * value + .75f) + start;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return end * (7.5625f * (value) * value + .9375f) + start;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    public static float EaseInOutBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        if (value < d / 2) return EaseInBounce(0, end, value * 2) * 0.5f + start;
        else return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
    }
    /* GFX47 MOD END */

    public static float EaseInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1;
        float s = 1.70158f;
        return end * (value) * value * ((s + 1) * value - s) + start;
    }

    public static float EaseOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value = (value / 1) - 1;
        return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
    }

    public static float EaseInOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value /= .5f;
        if ((value) < 1)
        {
            s *= (1.525f);
            return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
        }
        value -= 2;
        s *= (1.525f);
        return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
    }

    public static float Punch(float amplitude, float value)
    {
        float s = 9;
        if (Math.Abs(value) < 0.000001f)
        {
            return 0;
        }
        if (Math.Abs(value - 1) < 0.000001f)
        {
            return 0;
        }
        float period = 1 * 0.3f;
        s = period / (2 * Mathf.PI) * Mathf.Asin(0);
        return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }

    /* GFX47 MOD START */
    public static float EaseInElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (Math.Abs(value) < 0.000001f) return start;

        if (Math.Abs((value /= d) - 1) < 0.000001f) return start + end;

        if (Math.Abs(a) < 0.000001f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    }
    /* GFX47 MOD END */

    /* GFX47 MOD START */
    //public static float elastic(float start, float end, float value){
    public static float EaseOutElastic(float start, float end, float value)
    {
        /* GFX47 MOD END */
        //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (Math.Abs(value) < 0.000001f) return start;

        if (Math.Abs((value /= d) - 1) < 0.000001f) return start + end;

        if (Math.Abs(a) < 0.0000001f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
    }

    /* GFX47 MOD START */
    public static float EaseInOutElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (Math.Abs(value) < 0.000001f) return start;

        if (Math.Abs((value /= d / 2) - 2) < 0.00000001f) return start + end;

        if (Math.Abs(a) < 0.000001f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
    }
    /* GFX47 MOD END */

    #endregion

}
