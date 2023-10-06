using APPAPI.Data;
using APPAPI.Models.Domain;
using APPAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly DataBaseContext dbContext;

        public ImagesController(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, DataBaseContext dbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadDto request)
        {
            ValidateFile(request);
            if(ModelState.IsValid)
            {
                var imageModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.File.FileName,
                    FileDescription = request.FileDescription,
                };
                var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath,"Images",$"{imageModel.FileName}{imageModel.FileExtension}");
                using var stream = new FileStream(localFilePath, FileMode.Create);
                await imageModel.File.CopyToAsync(stream);
                var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Images/{imageModel.FileName}{imageModel.FileExtension}";
                imageModel.FilePath = urlFilePath;
                await dbContext.Images.AddAsync(imageModel);
                await dbContext.SaveChangesAsync();
                return Ok(imageModel);

            }
            return BadRequest(ModelState);
        }
        private void ValidateFile(ImageUploadDto request)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", "png" };
            if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension");
            }
            if(request.File.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size is more than 10MB, please select a different file");
            }
        }
    }
}
