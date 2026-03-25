using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CheckPoint.Models.Games
{
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del juego")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        [Required]
        [StringLength(100)]
        [DisplayName("Nombre")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("genre")]
        [Required]
        [StringLength(50)]
        [DisplayName("Género")]
        public string Genre { get; set; } = string.Empty;

        [BsonElement("platforms")]
        [Required]
        [DisplayName("Plataformas")]
        public List<string> Platforms { get; set; } = new List<string>();

        [BsonIgnore]
        [DisplayName("Plataforma")]
        public string Platform => Platforms?.FirstOrDefault() ?? string.Empty;

        [BsonIgnore]
        [DisplayName("Imagen de portada")]
        public string CoverImage { get; set; } = string.Empty;

        [BsonIgnore]
        [DisplayName("Descripción")]
        public string Description { get; set; } = string.Empty;

        [BsonIgnore]
        [DisplayName("Desarrollador")]
        public string Developer { get; set; } = string.Empty;

        [BsonIgnore]
        [DisplayName("Fecha de lanzamiento")]
        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;

        [BsonElement("isActive")]
        [Required]
        [DisplayName("Activo")]
        public bool IsActive { get; set; }
    }
}