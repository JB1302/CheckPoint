using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CheckPoint.Models
{
    public class Evento
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("organizerId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrganizerId { get; set; } = string.Empty;

        [BsonElement("gameId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string GameId { get; set; } = string.Empty;

        /// <summary>Online | Presencial | Hibrido</summary>
        [BsonElement("type")]
        public string Type { get; set; } = "Online";

        /// <summary>Borrador | Abierto | EnCurso | Completado | Cancelado</summary>
        [BsonElement("status")]
        public string Status { get; set; } = "Draft";

        /// <summary>Ejemplo: EliminacionSimple, EliminacionDoble, RoundRobin</summary>
        [BsonElement("format")]
        public string Format { get; set; } = string.Empty;

        [BsonElement("maxParticipants")]
        public int MaxParticipants { get; set; } = 16;

        [BsonElement("currentParticipants")]
        public int CurrentParticipants { get; set; } = 0;

        [BsonElement("startDate")]
        public DateTime StartDate { get; set; }

        [BsonElement("endDate")]
        public DateTime EndDate { get; set; }

        [BsonElement("registrationDeadline")]
        public DateTime RegistrationDeadline { get; set; }

        [BsonElement("location")]
        public UbicacionEvento? Location { get; set; }

        [BsonElement("streamUrl")]
        public string StreamUrl { get; set; } = string.Empty;

        [BsonElement("prizePool")]
        public string PrizePool { get; set; } = string.Empty;

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; } = "/img/default-event.png";

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UbicacionEvento
    {
        [BsonElement("venueName")]
        public string VenueName { get; set; } = string.Empty;

        [BsonElement("address")]
        public string Address { get; set; } = string.Empty;

        [BsonElement("city")]
        public string City { get; set; } = string.Empty;

        [BsonElement("country")]
        public string Country { get; set; } = string.Empty;
    }
}


