using System.ComponentModel.DataAnnotations;
using CheckPoint.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CheckPoint.ViewModels
{
    public class CrearEventoViewModel
    {
        [Required(ErrorMessage = "El título es requerido.")]
        [StringLength(120, ErrorMessage = "Máximo 120 caracteres.")]
        [Display(Name = "Título")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es requerida.")]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona un juego.")]
        [Display(Name = "Juego")]
        public string GameId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona la modalidad.")]
        [Display(Name = "Modalidad")]
        public string Type { get; set; } = "Online";

        [Display(Name = "Formato")]
        public string Format { get; set; } = string.Empty;

        [Range(2, 1024, ErrorMessage = "El cupo debe ser entre 2 y 1024.")]
        [Display(Name = "Máx. participantes")]
        public int MaxParticipants { get; set; } = 16;

        [Required]
        [Display(Name = "Inicio del evento")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddDays(7);

        [Required]
        [Display(Name = "Fin del evento")]
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddDays(8);

        [Required]
        [Display(Name = "Cierre de inscripciones")]
        public DateTime RegistrationDeadline { get; set; } = DateTime.UtcNow.AddDays(6);

        [Display(Name = "Stream URL")]
        [Url(ErrorMessage = "Introduce una URL válida.")]
        public string StreamUrl { get; set; } = string.Empty;

        [Display(Name = "Premio / Prize Pool")]
        public string PrizePool { get; set; } = string.Empty;

        [Display(Name = "Tags (separados por coma)")]
        public string TagsRaw { get; set; } = string.Empty;

        // Campos para Presencial / Hibrido
        [Display(Name = "Nombre del lugar")]
        public string VenueName { get; set; } = string.Empty;

        [Display(Name = "Dirección")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Ciudad")]
        public string City { get; set; } = string.Empty;

        [Display(Name = "País")]
        public string Country { get; set; } = string.Empty;

        // Para el <select> en la vista
        public List<SelectListItem> GameOptions { get; set; } = new();
    }

    public class EditarEventoViewModel : CrearEventoViewModel
    {
        public string Id { get; set; } = string.Empty;
    }

    public class DetalleEventoViewModel
    {
        public Evento Evento { get; set; } = null!;
        public Juego? Juego { get; set; }
        public ReglaEvento? Rules { get; set; }
        public List<Publicacion> Posts { get; set; } = new();
        public List<Inscripcion> Registrations { get; set; } = new();
        public bool IsRegistered { get; set; }
        public bool CanRegister { get; set; }
        public bool IsOrganizer { get; set; }
    }
}


