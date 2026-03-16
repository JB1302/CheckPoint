using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckPoint.Models.Events
{
    public class Events
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [DisplayName("Identificador del evento")]
        public string Id { get; set; }

        [BsonElement("title")]
        [Required]
        [StringLength(150)]
        [DisplayName("Título")]
        public string Title { get; set; }

        [BsonElement("description")]
        [Required]
        [StringLength(1000)]
        [DisplayName("Descripción")]
        public string Description { get; set; }

        [BsonElement("gameId")]
        [Required]
        [DisplayName("Identificador del juego")]
        public string GameId { get; set; }

        [BsonElement("eventType")]
        [Required]
        [StringLength(50)]
        [DisplayName("Tipo de evento")]
        public string EventType { get; set; }

        [BsonElement("startDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de inicio")]
        public DateTime StartDate { get; set; }

        [BsonElement("endDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de finalización")]
        public DateTime EndDate { get; set; }

        [BsonElement("location")]
        [StringLength(150)]
        [DisplayName("Ubicación")]
        public string Location { get; set; }

        [BsonElement("onlineLink")]
        [StringLength(300)]
        [DisplayName("Enlace en línea")]
        public string OnlineLink { get; set; }

        [BsonElement("maxParticipants")]
        [Required]
        [DisplayName("Máximo de participantes")]
        public int MaxParticipants { get; set; }

        [BsonElement("organizerId")]
        [Required]
        [DisplayName("Identificador del organizador")]
        public string OrganizerId { get; set; }

        [BsonElement("status")]
        [Required]
        [StringLength(50)]
        [DisplayName("Estado")]
        public string Status { get; set; }

        [BsonElement("createdAt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [Required]
        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }
    }
}