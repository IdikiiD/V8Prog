using System;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }

    public string ImagePath1 { get; set; }
    public string ImagePath2 { get; set; }
    public string ImagePath3 { get; set; }

    public DateTime CreatedAt { get; set; } 
}