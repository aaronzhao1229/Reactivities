using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.photos
{
  public class PhotoAccessor : IPhotoAccessor
  {
    private readonly Cloudinary _cloudinary;
    public PhotoAccessor(IOptions<CloudinarySettings> config)
    {
       // what we need to do with _cloudinary is we need to create a new Cloudinary account with our configuration
       // Passing normal parameters inside the Account() and it has to be in exactly the order that the account is expecting these values.
       var account = new Account(
          config.Value.CloudName,
          config.Value.ApiKey,
          config.Value.ApiSecret
       );
       _cloudinary = new Cloudinary(account);

    }

    public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
    {
       if (file.Length > 0)
       {
          await using var stream = file.OpenReadStream(); // Use 'using' here because we want to dispose the stream after using it. The file OpenReadStream implements the disposal method so that using statement does its job of disposing whatever resources are being used here.
          var uploadParams = new ImageUploadParams
          {
            File = new FileDescription(file.FileName, stream),
            // add transformation to let Cloudinay transform the image to a square image
            Transformation = new Transformation().Height(500).Width(500).Crop("fill")
          };
          var uploadResult = await _cloudinary.UploadAsync(uploadParams);

          if (uploadResult.Error != null) 
          {
            throw new Exception(uploadResult.Error.Message);
          }

          return new PhotoUploadResult
          {
            PublicId = uploadResult.PublicId,
            Url = uploadResult.SecureUrl.ToString()
          };

       }
        return null; // if there is no file
    }
    public async Task<string> DeletePhoto(string publicId)
    {
      var deleteParams = new DeletionParams(publicId);
      var result = await _cloudinary.DestroyAsync(deleteParams);
      return result.Result == "ok" ? result.Result : null;
    }
  }
}