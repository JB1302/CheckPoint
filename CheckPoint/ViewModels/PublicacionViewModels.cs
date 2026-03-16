using System.ComponentModel.DataAnnotations;

namespace CheckPoint.ViewModels
{
    public class CrearPublicacionViewModel
    {
        public string EventId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título es requerido.")]
        [StringLength(120)]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "El contenido es requerido.")]
        [Display(Name = "Contenido")]
        public string Content { get; set; } = string.Empty;

        /// <summary>Anuncio | Actualizacion | Resultado</summary>
        [Required]
        [Display(Name = "Tipo")]
        public string Type { get; set; } = "Announcement";
    }

    public class CrearComentarioViewModel
    {
        public string PostId { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;

        [Required(ErrorMessage = "El comentario no puede estar vacío.")]
        [StringLength(500, ErrorMessage = "Máximo 500 caracteres.")]
        [Display(Name = "Comentario")]
        public string Content { get; set; } = string.Empty;
    }
}


