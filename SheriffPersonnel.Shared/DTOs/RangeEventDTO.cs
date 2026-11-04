using System;
using System.Text.Json.Serialization;
using SheriffPersonnel.Shared.Models;

namespace SheriffPersonnel.Shared.DTOs
{
    public class RangeEventDTO
    {
        public int RangeEventID { get; set; }
        public DateOnly RangeDate { get; set; }
        public RangeOfficer? RangeOfficer { get; set; }
        public string? Location { get; set; }
        public string? EventType { get; set; }
        public string? Notes { get; set; }
        public ICollection<RangeStatistic>? RangeStatistics { get; set; }
        
        [JsonInclude]
        public string? LastUpdatedInfo => $"Last updated by {_lastUser} on {_lastUpdated.ToString("MM/dd/yyyy")} at {_lastUpdated.ToString("hh:mm tt")}";
        
        private string? _lastUser { get; set; }
        private DateTime _lastUpdated { get; set; }    
    }
}

