using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;

namespace dotnet_store.Attributes
{
    public class ImageDimensionsAttribute : ValidationAttribute
    {
        private readonly int _maxWidth;
        private readonly int _maxHeight;

        public ImageDimensionsAttribute(int maxWidth, int maxHeight)
        {
            _maxWidth = maxWidth;
            _maxHeight = maxHeight;
        }


        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                try
                {
                    using var image = Image.Load(file.OpenReadStream());
                    if (image.Width > _maxWidth || image.Height > _maxHeight)
                    {
                        return new ValidationResult($"Image dimensions must be {_maxWidth}x{_maxHeight} pixels or smaller.");
                    }
                }
                catch
                {
                    return new ValidationResult("Invalid image file.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
