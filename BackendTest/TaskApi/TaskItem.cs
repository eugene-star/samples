namespace TaskApi;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public Priority Priority { get; set; }

    public byte[] RowVersion { get; set; } = new byte[8];
}

public enum Priority
{
    Low,
    Medium,
    High
}

public record TaskItemDto
{
    public string? Title { get; set; }
    public Priority? Priority { get; set; }
}