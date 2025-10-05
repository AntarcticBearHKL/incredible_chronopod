using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

/// <summary>
/// 记事详情DTO
/// </summary>
public class NoteDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public List<string> TagList { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public int CharacterCount { get; set; }
}

/// <summary>
/// 创建记事DTO
/// </summary>
public class CreateNoteDto
{
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题长度不能超过200个字符")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "内容不能为空")]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "标签长度不能超过1000个字符")]
    public string Tags { get; set; } = string.Empty;
    
    [MaxLength(50, ErrorMessage = "分类长度不能超过50个字符")]
    public string Category { get; set; } = "普通";
    
    [MaxLength(20, ErrorMessage = "优先级长度不能超过20个字符")]
    public string Priority { get; set; } = "中";
    
    [MaxLength(20, ErrorMessage = "状态长度不能超过20个字符")]
    public string Status { get; set; } = "已发布";
    
    public bool IsPinned { get; set; } = false;
    public bool IsFavorite { get; set; } = false;
}

/// <summary>
/// 更新记事DTO
/// </summary>
public class UpdateNoteDto
{
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题长度不能超过200个字符")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "内容不能为空")]
    public string Content { get; set; } = string.Empty;
    
    [MaxLength(1000, ErrorMessage = "标签长度不能超过1000个字符")]
    public string Tags { get; set; } = string.Empty;
    
    [MaxLength(50, ErrorMessage = "分类长度不能超过50个字符")]
    public string Category { get; set; } = "普通";
    
    [MaxLength(20, ErrorMessage = "优先级长度不能超过20个字符")]
    public string Priority { get; set; } = "中";
    
    [MaxLength(20, ErrorMessage = "状态长度不能超过20个字符")]
    public string Status { get; set; } = "已发布";
    
    public bool IsPinned { get; set; } = false;
    public bool IsFavorite { get; set; } = false;
}

/// <summary>
/// 记事列表DTO（简化版本）
/// </summary>
public class NoteListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public List<string> TagList { get; set; } = new();
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int CharacterCount { get; set; }
    
    /// <summary>
    /// 内容预览（截取前200个字符）
    /// </summary>
    public string ContentPreview => Content.Length > 200 ? Content.Substring(0, 200) + "..." : Content;
}

/// <summary>
/// 记事搜索参数DTO
/// </summary>
public class NoteSearchDto
{
    /// <summary>
    /// 关键词搜索（标题和内容）
    /// </summary>
    public string? Keyword { get; set; }
    
    /// <summary>
    /// 标签过滤
    /// </summary>
    public string? Tag { get; set; }
    
    /// <summary>
    /// 分类过滤
    /// </summary>
    public string? Category { get; set; }
    
    /// <summary>
    /// 优先级过滤
    /// </summary>
    public string? Priority { get; set; }
    
    /// <summary>
    /// 状态过滤
    /// </summary>
    public string? Status { get; set; }
    
    /// <summary>
    /// 是否只显示置顶
    /// </summary>
    public bool? IsPinned { get; set; }
    
    /// <summary>
    /// 是否只显示收藏
    /// </summary>
    public bool? IsFavorite { get; set; }
    
    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// 结束日期
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// 排序字段：created_at, updated_at, title, priority
    /// </summary>
    public string SortBy { get; set; } = "created_at";
    
    /// <summary>
    /// 排序方向：asc, desc
    /// </summary>
    public string SortOrder { get; set; } = "desc";
    
    /// <summary>
    /// 页码
    /// </summary>
    public int Page { get; set; } = 1;
    
    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; } = 20;
}