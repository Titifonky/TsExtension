using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Outils
{
    public static class ImageEdition
    {
        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static BitmapImage ToBitmapImage(this Image bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static Image AutoCrop(this Bitmap bmp)
        {
            if (Image.GetPixelFormatSize(bmp.PixelFormat) != 32)
                throw new InvalidOperationException("Autocrop currently only supports 32 bits per pixel images.");

            // Initialize variables
            var cropColor = Color.White;

            var bottom = 0;
            var left = bmp.Width; // Set the left crop point to the width so that the logic below will set the left value to the first non crop color pixel it comes across.
            var right = 0;
            var top = bmp.Height; // Set the top crop point to the height so that the logic below will set the top value to the first non crop color pixel it comes across.

            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

            unsafe
            {
                var dataPtr = (byte*)bmpData.Scan0;

                for (var y = 0; y < bmp.Height; y++)
                {
                    for (var x = 0; x < bmp.Width; x++)
                    {
                        var rgbPtr = dataPtr + (x * 4);

                        var b = rgbPtr[0];
                        var g = rgbPtr[1];
                        var r = rgbPtr[2];
                        var a = rgbPtr[3];

                        // If any of the pixel RGBA values don't match and the crop color is not transparent, or if the crop color is transparent and the pixel A value is not transparent
                        if ((cropColor.A > 0 && (b != cropColor.B || g != cropColor.G || r != cropColor.R || a != cropColor.A)) || (cropColor.A == 0 && a != 0))
                        {
                            if (x < left)
                                left = x;

                            if (x >= right)
                                right = x + 1;

                            if (y < top)
                                top = y;

                            if (y >= bottom)
                                bottom = y + 1;
                        }
                    }

                    dataPtr += bmpData.Stride;
                }
            }

            bmp.UnlockBits(bmpData);

            if (left < right && top < bottom)
                return bmp.Clone(new Rectangle(left, top, right - left, bottom - top), bmp.PixelFormat);

            return null; // Entire image should be cropped, so just return null
        }
    }
    public static class DictionaryListe
    {
        public static Boolean AddIfNotExist<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, value);
                return true;
            }

            return false;
        }

        public static Boolean AddIfNotExist<T>(this List<T> list, T value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
                return true;
            }

            return false;
        }

        public static Boolean AddIfNotExist<T>(this HashSet<T> list, T value)
        {
            if (!list.Contains(value))
            {
                list.Add(value);
                return true;
            }

            return false;
        }

        public static T Last<T>(this List<T> list)
        {
            return list[list.Count - 1];
        }

        public static T Pop<T>(this List<T> list)
        {
            T val = list[0];
            list.RemoveAt(0);
            return val;
        }

        public static KeyValuePair<TKey, TValue> KeyValuePair<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            return new KeyValuePair<TKey, TValue>(key, dictionary[key]);
        }

        /// <summary>
        /// Ajoute la clé ou un à la valeur
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Boolean AddIfNotExistOrPlus<TKey>(this Dictionary<TKey, int> dic, TKey key)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, 1);
                return true;
            }
            else
                dic[key]++;

            return false;
        }

        /// <summary>
        /// Ajoute la clé ou un à la valeur
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Boolean AddIfNotExistOrPlus<TKey>(this SortedDictionary<TKey, int> dic, TKey key)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, 1);
                return true;
            }
            else
                dic[key]++;

            return false;
        }

        /// <summary>
        /// Ajoute 1 à la valeur si la clé existe
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Boolean Plus<TKey>(this Dictionary<TKey, int> dic, TKey key)
        {
            if (dic.ContainsKey(key))
            {
                dic[key]++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ajoute la clé si elle n'existe pas et initialise la valeur à 1
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Boolean Ajouter<TKey>(this Dictionary<TKey, int> dic, TKey key)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, 1);
                return true;
            }

            return false;
        }

        public static void Multiplier<TKey>(this Dictionary<TKey, int> dic, int m)
        {
            foreach (var k in dic.Keys.ToList())
            {
                int v = dic[k];
                dic[k] = v * m;
            }
        }
    }
}
