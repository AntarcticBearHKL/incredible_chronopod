using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

public class Note
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    /// <summary>
    /// 标签，以逗号分隔的字符串形式存储
    /// </summary>
    [MaxLength(1000)]
    public string Tags { get; set; } = string.Empty;
    
    /// <summary>
    /// 记事类型：灵感、事件、想法等
    /// </summary>
    [MaxLength(50)]
    public string Category { get; set; } = "普通";
    
    /// <summary>
    /// 优先级：低、中、高
    /// </summary>
    [MaxLength(20)]
    public string Priority { get; set; } = "中";
    
    /// <summary>
    /// 记事状态：草稿、已发布、已归档
    /// </summary>
    [MaxLength(20)]
    public string Status { get; set; } = "已发布";
    
    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool IsPinned { get; set; } = false;
    
    /// <summary>
    /// 是否收藏
    /// </summary>
    public bool IsFavorite { get; set; } = false;
    
    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// 最后查看时间
    /// </summary>
    public DateTime? LastViewedAt { get; set; }
    
    /// <summary>
    /// 字符数统计
    /// </summary>
    [NotMapped]
    public int CharacterCount => Content.Length;
    
    /// <summary>
    /// 标签列表（从Tags字符串解析）
    /// </summary>
    [NotMapped]
    public List<string> TagList => string.IsNullOrEmpty(Tags) 
        ? new List<string>() 
        : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
              .Select(t => t.Trim())
              .Where(t => !string.IsNullOrEmpty(t))
              .ToList();
}