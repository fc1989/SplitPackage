using System.ComponentModel.DataAnnotations;

namespace SplitPackage.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}