using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace POC_SonarQubeToMSTeams.Models
{
    public partial class MSTeamsComplexCard
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("@context")]
        public Uri Context { get; set; }

        [JsonProperty("themeColor")]
        public string ThemeColor { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("sections")]
        public Section[] Sections { get; set; }

        [JsonProperty("potentialAction")]
        public PotentialAction[] PotentialAction { get; set; }
    }

    public partial class PotentialAction
    {
        [JsonProperty("@type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("targets")]
        public Target[] Targets { get; set; }

    }

    public partial class Target
    {
        [JsonProperty("os")]
        public string OS { get; set; }

        [JsonProperty("uri")]
        public Uri Uri { get; set; }
    }

    public partial class Section
    {
        [JsonProperty("activityTitle")]
        public string ActivityTitle { get; set; }

        [JsonProperty("activitySubtitle")]
        public string ActivitySubtitle { get; set; }

        [JsonProperty("activityImage")]
        public Uri ActivityImage { get; set; }

        [JsonProperty("activityText")]
        public string ActivityText { get; set; }

        [JsonProperty("facts")]
        public Fact[] Facts { get; set; }

        [JsonProperty("markdown")]
        public bool Markdown { get; set; }
    }

    public partial class Fact
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public partial class MSTeamsComplexCard
    {
        public static MSTeamsComplexCard FromJson(string json) => JsonConvert.DeserializeObject<MSTeamsComplexCard>(json, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}