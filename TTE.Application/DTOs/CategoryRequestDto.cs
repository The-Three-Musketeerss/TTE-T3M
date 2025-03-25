using TTE.Commons.Validators;

namespace TTE.Application.DTOs
{
    public class CategoryRequestDto
    {
        public int? Id { get; set; }

        [RequiredFieldValidator]
        public string Name { get; set; } = string.Empty;
    }
}
