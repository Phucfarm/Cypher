using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace CypherCompiler
{
    public readonly struct Position
    {
        [JsonPropertyName("line")]
        public int Line { get; init; }
        [JsonPropertyName("character")]
        public int Character { get; init; }
        public Position(int line, int character)
        {
            Line = line;
            Character = character;
        }
    }
    public readonly struct Range
    {
        [JsonPropertyName("start")]
        public Position Start { get; init; }
        [JsonPropertyName("end")]
        public Position End { get; init; }
        public Range(Position start, Position end)
        {
            Start = start;
            End = end;
        }
    }
    public enum DiagnosticSeverity
    {
        Error = 1,
        Warning = 2,
        Info = 3,
        Hint = 4
    }
    public class Diagnostic
    {
        [JsonPropertyName("range")]
        public Range Range { get; set; }
        [JsonPropertyName("severity")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DiagnosticSeverity? Severity { get; set; }
        [JsonPropertyName("code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; set; }
        [JsonPropertyName("source")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Source { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        [JsonPropertyName("tags")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int[]? Tags { get; set; }
        [JsonPropertyName("relatedInformation")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<DiagnosticRelatedInformation>? RelatedInformation { get; set; }
        public Diagnostic(Range range, string message, DiagnosticSeverity? severity = DiagnosticSeverity.Error, string? code = null, string? source = "Cypher Compiler", int[]? tags = null, List<DiagnosticRelatedInformation>? relatedInformation = null)
        {
            Range = range;
            Message = message;
            Severity = severity;
            Code = code;
            Source = source;
            Tags = tags;
            RelatedInformation = relatedInformation;
        }
    }
    public class DiagnosticRelatedInformation
    {
        [JsonPropertyName("location")]
        public Location Location { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
        public DiagnosticRelatedInformation(Location location, string message)
        {
            Location = location;
            Message = message;
        }
    }
    public struct Location
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }
        [JsonPropertyName("range")]
        public Range Range { get; set; }
        public Location(string uri, Range range)
        {
            Uri = uri;
            Range = range;
        }
    }
}
