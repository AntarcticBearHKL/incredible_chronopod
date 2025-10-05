using Microsoft.AspNetCore.Mvc;
using backend.DTOs;
using backend.Services.Interfaces;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    /// <summary>
    /// 获取记事列表（支持搜索和分页）
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<NoteListDto>>> GetNotes([FromQuery] NoteSearchDto searchDto)
    {
        try
        {
            var result = await _noteService.GetNotesAsync(searchDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResult($"获取记事列表失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 根据ID获取记事详情
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NoteDto>>> GetNote(int id)
    {
        try
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null)
            {
                return NotFound(ApiResponse<NoteDto>.ErrorResult($"找不到ID为{id}的记事"));
            }
            
            return Ok(ApiResponse<NoteDto>.SuccessResult(note, "获取记事详情成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<NoteDto>.ErrorResult($"获取记事详情失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 创建新记事
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<NoteDto>>> CreateNote([FromBody] CreateNoteDto createNoteDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<NoteDto>.ErrorResult("输入数据验证失败", errors));
            }

            var note = await _noteService.CreateNoteAsync(createNoteDto);
            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, 
                ApiResponse<NoteDto>.SuccessResult(note, "创建记事成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<NoteDto>.ErrorResult($"创建记事失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 更新记事
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<NoteDto>>> UpdateNote(int id, [FromBody] UpdateNoteDto updateNoteDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(ApiResponse<NoteDto>.ErrorResult("输入数据验证失败", errors));
            }

            var note = await _noteService.UpdateNoteAsync(id, updateNoteDto);
            if (note == null)
            {
                return NotFound(ApiResponse<NoteDto>.ErrorResult($"找不到ID为{id}的记事"));
            }

            return Ok(ApiResponse<NoteDto>.SuccessResult(note, "更新记事成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<NoteDto>.ErrorResult($"更新记事失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 删除记事
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteNote(int id)
    {
        try
        {
            var deleted = await _noteService.DeleteNoteAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.ErrorResult($"找不到ID为{id}的记事"));
            }

            return Ok(ApiResponse<object>.SuccessResult(null, "删除记事成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResult($"删除记事失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 批量删除记事
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteNotes([FromBody] List<int> ids)
    {
        try
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(ApiResponse<object>.ErrorResult("请提供要删除的记事ID列表"));
            }

            var deletedCount = await _noteService.DeleteNotesAsync(ids);
            return Ok(ApiResponse<object>.SuccessResult(new { deletedCount }, $"成功删除{deletedCount}条记事"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResult($"批量删除记事失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 切换记事置顶状态
    /// </summary>
    [HttpPatch("{id}/pin")]
    public async Task<ActionResult<ApiResponse<object>>> TogglePin(int id)
    {
        try
        {
            var isPinned = await _noteService.TogglePinAsync(id);
            var message = isPinned ? "置顶成功" : "取消置顶成功";
            return Ok(ApiResponse<object>.SuccessResult(new { isPinned }, message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResult($"切换置顶状态失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 切换记事收藏状态
    /// </summary>
    [HttpPatch("{id}/favorite")]
    public async Task<ActionResult<ApiResponse<object>>> ToggleFavorite(int id)
    {
        try
        {
            var isFavorite = await _noteService.ToggleFavoriteAsync(id);
            var message = isFavorite ? "收藏成功" : "取消收藏成功";
            return Ok(ApiResponse<object>.SuccessResult(new { isFavorite }, message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResult($"切换收藏状态失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 获取所有标签
    /// </summary>
    [HttpGet("tags")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetAllTags()
    {
        try
        {
            var tags = await _noteService.GetAllTagsAsync();
            return Ok(ApiResponse<List<string>>.SuccessResult(tags, "获取标签列表成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<string>>.ErrorResult($"获取标签列表失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 获取所有分类
    /// </summary>
    [HttpGet("categories")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetAllCategories()
    {
        try
        {
            var categories = await _noteService.GetAllCategoriesAsync();
            return Ok(ApiResponse<List<string>>.SuccessResult(categories, "获取分类列表成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<string>>.ErrorResult($"获取分类列表失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 获取记事统计信息
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult<ApiResponse<Dictionary<string, object>>>> GetStatistics()
    {
        try
        {
            var stats = await _noteService.GetStatisticsAsync();
            return Ok(ApiResponse<Dictionary<string, object>>.SuccessResult(stats, "获取统计信息成功"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<Dictionary<string, object>>.ErrorResult($"获取统计信息失败: {ex.Message}"));
        }
    }

    /// <summary>
    /// 快速搜索记事
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<List<NoteListDto>>>> SearchNotes([FromQuery] string keyword, [FromQuery] int limit = 10)
    {
        try
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return BadRequest(ApiResponse<List<NoteListDto>>.ErrorResult("搜索关键词不能为空"));
            }

            var notes = await _noteService.SearchNotesAsync(keyword, limit);
            return Ok(ApiResponse<List<NoteListDto>>.SuccessResult(notes, "搜索完成"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<List<NoteListDto>>.ErrorResult($"搜索记事失败: {ex.Message}"));
        }
    }
}