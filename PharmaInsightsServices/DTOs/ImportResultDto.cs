public class ImportResultDto
{
    public int ImportedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
