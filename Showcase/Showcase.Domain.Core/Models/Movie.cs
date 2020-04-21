
using System;
using System.Text.Json.Serialization;

public class Movie
{
    public Movie()
    {
    }
#pragma warning disable IDE1006 // Naming Styles
    public string body { get; set; }

    public Cardimage[] cardImages { get; set; }
    public Cast[] cast { get; set; }
    public string cert { get; set; }
    public string _class { get; set; }
    public Director[] directors { get; set; }
    public int duration { get; set; }
    public string[] genres { get; set; }
    public string headline { get; set; }
    [JsonConverter(typeof(GuidConverter))]
    public Guid id { get; set; }
    public Keyartimage[] keyArtImages { get; set; }
    public string lastUpdated { get; set; }
    public string quote { get; set; }
    public int rating { get; set; }
    public string reviewAuthor { get; set; }
    public string skyGoId { get; set; }
    public string skyGoUrl { get; set; }
    public string sum { get; set; }
    public string synopsis { get; set; }
    public string url { get; set; }
    public Video[] videos { get; set; }
    public Viewingwindow viewingWindow { get; set; }
    public string year { get; set; }
    public Gallery[] galleries { get; set; }
    public string sgid { get; set; }
    public string sgUrl { get; set; }
#pragma warning restore IDE1006 // Naming Styles
}
