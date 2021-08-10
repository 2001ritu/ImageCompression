using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ImageMagick;
using NetVips;
using static NetVips.Enums;
using Image_Compression.Models;
namespace Image_Compression.Services
{
    public class ImageCompresser
    {

        //for single image
        //working watermarks
        public async Task<byte[]> compress(string path)
        {
            byte[] originalBytes = null;
            byte[] compressedBytes = null;
            bool f = false;
            try
            {
                string fileName = Path.GetFileName(path);
                using var image = NetVips.Image.NewFromFile(path);
                using (WebClient webClient = new WebClient())
                {
                    originalBytes = webClient.DownloadData(path);
                }
                NetVips.Image img1 = ByteToImg(originalBytes);
                NetVips.Image img = img1.Smartcrop((int)(img1.Width * 0.8), (int)(img1.Height * 0.8), Interesting.Centre);
                compressedBytes = img.TiffsaveBuffer(ForeignTiffCompression.Jpeg);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured");
            }
            return compressedBytes;
        }

        //For multiple image
        string fileName = "";
            public async Task<byte[]> compress(string path, string watermarkpath)
            {
                byte[] originalBytes = null;
                bool f = false;
                try
                {
                    //  count++;
                    originalBytes = getImageBytes(path);
                     fileName = Path.GetFileName(path);

                    String writePath = "C:\\Users\\richa\\Pictures\\Vips-compress\\" + fileName;
                    // byte[] croppedImage = getCroppedImage(originalBytes);

                 //    byte[] bytesWithWatermark = insertImageWaterMark(originalBytes, watermarkpath, writePath);
                 //   byte[] insertTextWatermarkBytes= insertTextWatermark(originalBytes, "Perspectify!!");


                     byte[] compressedImage = getComprssedImage(originalBytes,watermarkpath);
                    // byte[] compressedImage = getComprssedImage(insertTextWatermarkBytes);


                    // Console.WriteLine(compressedImage);
                    // insertImageWaterMark(compressedImage, watermarkpath, writePath);

                    using (Stream file = File.OpenWrite(writePath))
                    {
                        file.Write(compressedImage, 0, compressedImage.Length);
                    }
                return compressedImage;

            }
            catch (Exception e)
                {
                    Console.WriteLine("Exception occured");
                    throw e;
                }
            }


        public async Task<byte[]> compress ( byte[] originalBytes, string watermarkpath)
        {
            bool f = false;
            try
            {

                String writePath = "C:\\Users\\richa\\Pictures\\Vips-compress\\" + "temp.jpg";

                // byte[] croppedImage = getCroppedImage(originalBytes);

                // byte[] bytesWithWatermark = insertImageWaterMark(originalBytes, watermarkpath, writePath);
             //   byte[] insertTextWatermarkBytes = insertTextWatermark(originalBytes, "This is the property of Perspectify!!");


                // byte[] compressedImage = getComprssedImage(bytesWithWatermark);
                byte[] compressedImage = getComprssedImage(originalBytes,watermarkpath);
              //  byte[] insertTextWatermarkBytes = insertTextWatermark(compressedImage, "This is the property of Perspectify!!");



                // insertImageWaterMark(compressedImage, watermarkpath, writePath);

                using (Stream file = File.OpenWrite(writePath))
                {
                    file.Write(compressedImage, 0, compressedImage.Length);
                }
             return compressedImage;

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception occured");
                throw e;
            }
        }
        private byte[] insertTextWatermark(byte[] imageBytes, string watermarkText)
         {
            NetVips.Image img = ByteToImg(imageBytes);
            Stream stream = new MemoryStream(imageBytes);
             Bitmap bitmap = new Bitmap(stream);
             Bitmap tempBitMap = new Bitmap(bitmap, bitmap.Width, bitmap.Height);
             Graphics graphicsImage = Graphics.FromImage(tempBitMap);
             StringFormat stringformat1 = new StringFormat();
             stringformat1.Alignment = StringAlignment.Far;
             Color StringColor1 = ColorTranslator.FromHtml("#FFFFFF");
             graphicsImage.DrawString(watermarkText, new Font("arail", 40, FontStyle.Regular), new SolidBrush(StringColor1), new Point(img.Width-100,img.Height-400), stringformat1);
            // bitmap.Save(writePath);
            using (var ms = new MemoryStream())
            {
                tempBitMap.Save(ms, bitmap.RawFormat);
                return ms.ToArray();
            }

         }
      
        private byte[] getCroppedImage(byte[] bytes)
        {
            NetVips.Image img = ByteToImg(bytes);
            NetVips.Image croppedImage = img.Smartcrop((int)(img.Width * 0.8), (int)(img.Height * 0.8), Interesting.Centre);
            return croppedImage.WriteToMemory();
        }
        private byte[] getComprssedImage(byte[] originalBytes,string watermarkpath)
        {
            String writePath = "C:\\Users\\richa\\Pictures\\Vips-compress\\" + fileName;

            NetVips.Image img = ByteToImg(originalBytes);
            NetVips.Image croppedImage = img.Smartcrop((int)(img.Width * 0.9), (int)(img.Height * 0.9), Interesting.Centre);
            byte[] croppedImageByte = croppedImage.JpegsaveBuffer();
            byte[] bytesWithWatermark = insertImageWaterMark(croppedImageByte, watermarkpath, writePath);
            byte[] insertTextWatermarkBytes = insertTextWatermark(bytesWithWatermark, "Perspectify!!");

            NetVips.Image watermarkImage = ByteToImg(insertTextWatermarkBytes);


            //img.Tiffsave(writePath, compression: ForeignTiffCompression.Jpeg);

            byte[] compressedImage = watermarkImage.TiffsaveBuffer(compression: ForeignTiffCompression.Jpeg);
            return compressedImage;
        }
        private byte[] insertImageWaterMark(byte[] compressedImage, string watermarkpath, string writePath)
        {
            byte[] output = null;
            NetVips.Image img = ByteToImg(compressedImage);

            using (var magicimage = new MagickImage(compressedImage))
            {
                using (var watermark = new MagickImage(watermarkpath))
                {
                    magicimage.Composite(watermark, img.Width-350,img.Height-300, CompositeOperator.Over);
                }
                magicimage.Write(writePath);
                output = magicimage.ToByteArray();
            }
            return output;
        }
        private byte[] getImageBytes(string path)
        {
            byte[] bytes;
            string fileName = Path.GetFileName(path);
            using var image = NetVips.Image.NewFromFile(path);
            using (WebClient webClient = new WebClient())
            {
                bytes = webClient.DownloadData(path);
            }
            return bytes;
        }

        //ImageMagick
        /*     public async Task compress(string path, string watermarkpath)
            {
                byte[] compressedBytes = null;
                bool f = false;
                try
                {
                    string fileName = Path.GetFileName(path);
                    var optimizer = new ImageOptimizer();
                    optimizer.LosslessCompress(fileInfo(path));
                    using (WebClient webClient = new WebClient())
                    {
                        compressedBytes = webClient.DownloadData(path);
                    }
                    NetVips.Image img = ByteToImg(compressedBytes);
                   // count++;
                    String writePath = "C:\\Users\\richa\\Pictures\\Magick-compress\\" + fileName;
                  //  img.Jpegsave(writePath);
                    using (var image = new MagickImage(path))
                    {
                        using (var watermark = new MagickImage(watermarkpath))
                        {
                            image.Composite(watermark, 200, 50, CompositeOperator.Over);
                        }
                        image.Write(writePath);
                    }
                    *//* byte[] compressedImage = img.TiffsaveBuffer(compression: ForeignTiffCompression.Jpeg);
                     Stream stream = new MemoryStream(compressedImage);
                     System.Drawing.Image bitmap = Bitmap.FromStream(stream);
                     Graphics graphicsImage = Graphics.FromImage(bitmap);
                     StringFormat stringformat1 = new StringFormat();
                     stringformat1.Alignment = StringAlignment.Far;
                     Color StringColor1 = ColorTranslator.FromHtml("#000000");
                     string Str_Text10nImage = "watermark for Perspectify";
                     graphicsImage.DrawString(Str_Text10nImage, new Font("arail", 40, FontStyle.Regular), new SolidBrush(StringColor1), new Point(600, 700), stringformat1);
                     bitmap.Save(writePath);*//*
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception occured");
                }
                //return compressedBytes;
            }*/
        private NetVips.Image ByteToImg(byte[] byteArr)
        {
            var ms = new MemoryStream(byteArr);
            return NetVips.Image.NewFromStream(ms);
        }

        //Netvips Resize
        /*        public async Task<byte[]> compress(string path)
                {
                    byte[] compressedBytes = null;
                    bool f = false;
                    try
                    {
                        double scaleFactor = 0.7;
                        string fileName = Path.GetFileName(path);
                        using var image = Image.NewFromFile(path);

                        Image resize = image.Resize(scaleFactor);
                        int position = path.LastIndexOf(fileName);
                        string path2 = path.Substring(0, position) + "compressed_" + fileName;
                        resize.Jpegsave(path2);
                        using (WebClient webClient = new WebClient())
                        {
                            compressedBytes = webClient.DownloadData(path2);
                        }

                    }
                    catch (Exception e)
                    {

                    }
                    return compressedBytes;
                }*/

        //Drawing 
        /* public async Task<byte[]> compress(string path)
         {
             Bitmap myBitmap;
             ImageCodecInfo myImageCodecInfo;
             Encoder myEncoder;
             EncoderParameter myEncoderParameter;
             EncoderParameters myEncoderParameters;
             myBitmap = new Bitmap(path);
             myImageCodecInfo = GetEncoderInfo("image/jpeg");
             myEncoder = Encoder.Compression;
             myEncoderParameters = new EncoderParameters();
             myEncoderParameter = new EncoderParameter(
                 myEncoder,
                 (long)EncoderValue.CompressionRle);
             myEncoderParameters.Param[0] = myEncoderParameter;
             myBitmap.Save("C:\\Users\\HIMANI\\Pictures\\Compress Image\\Compress\\drawing.jpg", myImageCodecInfo, myEncoderParameters);
             return new byte[] { 0 };
         }
         private static ImageCodecInfo GetEncoderInfo(String mimeType)
         {
             int j;
             ImageCodecInfo[] encoders;
             encoders = ImageCodecInfo.GetImageEncoders();
             for (j = 0; j < encoders.Length; ++j)
             {
                 if (encoders[j].MimeType == mimeType)
                     return encoders[j];
             }
             return null;
         }*/

        /*  private byte[] ImgToByte(Netvips.Image image)
          {
              ImageConverter imageConverter = new ImageConverter();
              return (byte[])imageConverter.ConvertTo(image, typeof(byte[]));
          }*/
        private FileInfo fileInfo(string fileName)
        {
            FileInfo file = new FileInfo(fileName);
            return file;
        }

    }
}