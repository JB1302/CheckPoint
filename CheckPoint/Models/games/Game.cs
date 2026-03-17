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

        [BsonElement("isActive")]
        [Required]
        [DisplayName("Activo")]
        public bool IsActive { get; set; }
    }
}