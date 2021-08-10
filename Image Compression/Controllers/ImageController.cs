using Image_Compression.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Image_Compression.Models;
using System.Net.Http;
using System.Collections.Generic;

namespace Image_Compression.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : Controller
    {
        //For single image
        [HttpPost]
        /*   public async Task<IActionResult> ImageCompress([FromBody] ImageModel imageobj)
           {
               byte[] compressedBytes = null;
               ImageCompresser imageCompresser = new ImageCompresser();
               foreach (string path in imageobj.images)
               {
                   compressedBytes = await imageCompresser.compress(path);
               }

               return Ok(new { compressedBytes = compressedBytes }); ;
           }*/

        //For multiple images
        [HttpPost]
        public async Task<IActionResult> ImageCompress([FromBody] ImageModel imageobj)
        {
            byte[] compressedBytes = null;
            var tasks = new List<Task>();
            ImageCompresser imageCompresser = new ImageCompresser();

            foreach (string path in imageobj.images)
            {
               // byte[] compressedBytes = null;
                Task t = Task.Run(() =>
                    {
                        compressedBytes = imageCompresser.compress(path, imageobj.watermarkpath).Result;
                    });
                tasks.Add(t);
            }

            byte[] fileBytes = System.Convert.FromBase64String(imageobj.imageByteArray);

            Task task = Task.Run(() =>
            {
                compressedBytes = imageCompresser.compress(fileBytes, imageobj.watermarkpath).Result;
            });
            tasks.Add(task);
            Task.WaitAll(tasks.ToArray());
            //return Ok("images compressed"); ;
            return Ok(new { compressedBytes = compressedBytes }); ;


        }

    }
}