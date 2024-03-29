﻿namespace Undersoft.SDK.Instant
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;
    using Uniques;


    public interface IInstant : IUnique
    {
        [JsonIgnore]
        [IgnoreDataMember]
        [NotMapped]
        object this[string propertyName] { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        [NotMapped]
        object this[int fieldId] { get; set; }

        [IgnoreDataMember]
        [JsonIgnore]
        Uscn Code { get; set; }
    }

    public interface IValueArray 
    {
        [JsonIgnore]
        [IgnoreDataMember]
        [NotMapped]
        object[] ValueArray { get; set; }
    }

    public interface IByteable
    {
        byte[] GetBytes();
    }
}
