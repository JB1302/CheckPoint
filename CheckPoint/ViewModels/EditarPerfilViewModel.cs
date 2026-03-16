using System.ComponentModel.DataAnnotations;

namespace CheckPoint.ViewModels
{
    public class EditarPerfilViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Máximo 50 caracteres.")]
        [Display(Name = "Nombre para mostrar")]
        public string DisplayName { get; set; } = string.Empty;

        [StringLength(30, ErrorMessage = "Máximo 30 caracteres.")]
        [Display(Name = "GamerTag")]
        public string GamerTag { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Máximo 300 caracteres.")]
        [Display(Name = "Bio")]
        public string Bio { get; set; } = string.Empty;

        [Display(Name = "País")]
        public string Country { get; set; } = string.Empty;

        [Display(Name = "Juegos favoritos (separados por coma)")]
        public string FavoriteGamesRaw { get; set; } = string.Empty;

        [Display(Name = "Twitch")]
        [Url(ErrorMessage = "URL inválida.")]
        public string Twitch { get; set; } = string.Empty;

        [Display(Name = "YouTube")]
        [Url(ErrorMessage = "URL inválida.")]
        public string YouTube { get; set; } = string.Empty;

        [Display(Name = "Twitter / X")]
        [Url(ErrorMessage = "URL inválida.")]
        public string Twitter { get; set; } = string.Empty;
    }
}

