using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.Games
{
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del juego")]
        public string Id { get; set; }

        [BsonElement("name")]
        [Required]
        [StringLength(100)]
        [DisplayName("Nombre")]
        public string Name { get; set; }

        [BsonElement("genre")]
        [Required]
        [StringLength(50)]
        [DisplayName("Género")]
        public string Genre { get; set; }

        [BsonElement("platforms")]
        [Required]
        [DisplayName("Plataformas")]
        public List<string> Platforms { get; set; }

        [BsonElement("isActive")]
        [Required]
        [DisplayName("Activo")]
        public bool IsActive { get; set; }
    }
}