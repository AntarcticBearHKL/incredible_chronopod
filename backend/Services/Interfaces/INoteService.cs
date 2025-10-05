using backend.DTOs;
using backend.Models;

namespace backend.Services.Interfaces;

public interface INoteService
{
    /// <summary>
    /// 获取所有记事（分页）
    /// </summary>
    Task<PagedResponse<NoteListDto>> GetNotesAsync(NoteSearchDto searchDto);
    
    /// <summary>
    /// 根据ID获取记事详情
    /// </summary>
    Task<NoteDto?> GetNoteByIdAsync(int id);
    
    /// <summary>
    /// 创建新记事
    /// </summary>
    Task<NoteDto> CreateNoteAsync(CreateNoteDto createNoteDto);
    
    /// <summary>
    /// 更新记事
    /// </summary>
    Task<NoteDto?> UpdateNoteAsync(int id, UpdateNoteDto updateNoteDto);
    
    /// <summary>
    /// 删除记事
    /// </summary>
    Task<bool> DeleteNoteAsync(int id);
    
    /// <summary>
    /// 批量删除记事
    /// </summary>
    Task<int> DeleteNotesAsync(List<int> ids);
    
    /// <summary>
    /// 切换置顶状态
    /// </summary>
    Task<bool> TogglePinAsync(int id);
    
    /// <summary>
    /// 切换收藏状态
    /// </summary>
    Task<bool> ToggleFavoriteAsync(int id);
    
    /// <summary>
    /// 更新最后查看时间
    /// </summary>
    Task UpdateLastViewedAsync(int id);
    
    /// <summary>
    /// 获取所有标签
    /// </summary>
    Task<List<string>> GetAllTagsAsync();
    
    /// <summary>
    /// 获取所有分类
    /// </summary>
    Task<List<string>> GetAllCategoriesAsync();
    
    /// <summary>
    /// 获取记事统计信息
    /// </summary>
    Task<Dictionary<string, object>> GetStatisticsAsync();
    
    /// <summary>
    /// 搜索记事（全文搜索）
    /// </summary>
    Task<List<NoteListDto>> SearchNotesAsync(string keyword, int limit = 10);
}