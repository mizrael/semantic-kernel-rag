internal record Fact
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required string Text { get; init; }
    public string? Description { get; init; } = null;
    public string? Metadata { get; init; } = null;
}


internal static class Facts
{
    public static IEnumerable<Fact> GetFacts()
    {
        return new List<Fact>
        {
            new Fact
            {
                Text = "David Guida was born in Italy",
                Description = "This is a question about David Guida's nationality.",
                Metadata = "Nationality"
            },
            new Fact
            {
                Text = "David Guida is a Software Engineer and works for Microsoft",
                Description = "This is a question about David Guida's job.",
                Metadata = "Job"
            },
            new Fact
            {
                Text = "David Guida favourite hobby is playing Dungeons & Dragons",
                Description = "This is a question about David Guida's hobbies.",
                Metadata = "Hobbies"
            }
        };
    }
}