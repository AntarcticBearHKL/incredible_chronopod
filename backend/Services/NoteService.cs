using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services.Interfaces;

namespace backend.Services;

public class NoteService : INoteService
{
    private readonly AppDbContext _context;

    public NoteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResponse<NoteListDto>> GetNotesAsync(NoteSearchDto searchDto)
    {
        var query = _context.Notes.AsQueryable();

        // 应用搜索条件
        if (!string.IsNullOrEmpty(searchDto.Keyword))
        {
            query = query.Where(n => n.Title.Contains(searchDto.Keyword) || n.Content.Contains(searchDto.Keyword));
        }

        if (!string.IsNullOrEmpty(searchDto.Tag))
        {
            query = query.Where(n => n.Tags.Contains(searchDto.Tag));
        }

        if (!string.IsNullOrEmpty(searchDto.Category))
        {
            query = query.Where(n => n.Category == searchDto.Category);
        }

        if (!string.IsNullOrEmpty(searchDto.Priority))
        {
            query = query.Where(n => n.Priority == searchDto.Priority);
        }

        if (!string.IsNullOrEmpty(searchDto.Status))
        {
            query = query.Where(n => n.Status == searchDto.Status);
        }

        if (searchDto.IsPinned.HasValue)
        {
            query = query.Where(n => n.IsPinned == searchDto.IsPinned.Value);
        }

        if (searchDto.IsFavorite.HasValue)
        {
            query = query.Where(n => n.IsFavorite == searchDto.IsFavorite.Value);
        }

        if (searchDto.StartDate.HasValue)
        {
            query = query.Where(n => n.CreatedAt >= searchDto.StartDate.Value);
        }

        if (searchDto.EndDate.HasValue)
        {
            query = query.Where(n => n.CreatedAt <= searchDto.EndDate.Value);
        }

        // 应用排序
        query = searchDto.SortBy.ToLower() switch
        {
            "title" => searchDto.SortOrder.ToLower() == "asc" 
                ? query.OrderBy(n => n.Title)
                : query.OrderByDescending(n => n.Title),
            "updated_at" => searchDto.SortOrder.ToLower() == "asc"
                ? query.OrderBy(n => n.UpdatedAt)
                : query.OrderByDescending(n => n.UpdatedAt),
            "priority" => searchDto.SortOrder.ToLower() == "asc"
                ? query.OrderBy(n => n.Priority)
                : query.OrderByDescending(n => n.Priority),
            _ => searchDto.SortOrder.ToLower() == "asc"
                ? query.OrderBy(n => n.CreatedAt)
                : query.OrderByDescending(n => n.CreatedAt)
        };

        // 置顶的记事始终在前面
        query = query.OrderByDescending(n => n.IsPinned).ThenBy(n => n.CreatedAt);

        var total = await query.CountAsync();
        var notes = await query
            .Skip((searchDto.Page - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize)
            .Select(n => new NoteListDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                TagList = n.TagList,
                Category = n.Category,
                Priority = n.Priority,
                Status = n.Status,
                IsPinned = n.IsPinned,
                IsFavorite = n.IsFavorite,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                CharacterCount = n.Content.Length
            })
            .ToListAsync();

        return PagedResponse<NoteListDto>.Create(notes, searchDto.Page, searchDto.PageSize, total);
    }

    public async Task<NoteDto?> GetNoteByIdAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return null;

        // 更新最后查看时间
        await UpdateLastViewedAsync(id);

        return new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Tags = note.Tags,
            TagList = note.TagList,
            Category = note.Category,
            Priority = note.Priority,
            Status = note.Status,
            IsPinned = note.IsPinned,
            IsFavorite = note.IsFavorite,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            LastViewedAt = note.LastViewedAt,
            CharacterCount = note.CharacterCount
        };
    }

    public async Task<NoteDto> CreateNoteAsync(CreateNoteDto createNoteDto)
    {
        var note = new Note
        {
            Title = createNoteDto.Title,
            Content = createNoteDto.Content,
            Tags = createNoteDto.Tags,
            Category = createNoteDto.Category,
            Priority = createNoteDto.Priority,
            Status = createNoteDto.Status,
            IsPinned = createNoteDto.IsPinned,
            IsFavorite = createNoteDto.IsFavorite,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        return new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Tags = note.Tags,
            TagList = note.TagList,
            Category = note.Category,
            Priority = note.Priority,
            Status = note.Status,
            IsPinned = note.IsPinned,
            IsFavorite = note.IsFavorite,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            CharacterCount = note.CharacterCount
        };
    }

    public async Task<NoteDto?> UpdateNoteAsync(int id, UpdateNoteDto updateNoteDto)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return null;

        note.Title = updateNoteDto.Title;
        note.Content = updateNoteDto.Content;
        note.Tags = updateNoteDto.Tags;
        note.Category = updateNoteDto.Category;
        note.Priority = updateNoteDto.Priority;
        note.Status = updateNoteDto.Status;
        note.IsPinned = updateNoteDto.IsPinned;
        note.IsFavorite = updateNoteDto.IsFavorite;
        note.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new NoteDto
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Tags = note.Tags,
            TagList = note.TagList,
            Category = note.Category,
            Priority = note.Priority,
            Status = note.Status,
            IsPinned = note.IsPinned,
            IsFavorite = note.IsFavorite,
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt,
            LastViewedAt = note.LastViewedAt,
            CharacterCount = note.CharacterCount
        };
    }

    public async Task<bool> DeleteNoteAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return false;

        _context.Notes.Remove(note);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> DeleteNotesAsync(List<int> ids)
    {
        var notes = await _context.Notes.Where(n => ids.Contains(n.Id)).ToListAsync();
        var deletedCount = notes.Count;
        
        _context.Notes.RemoveRange(notes);
        await _context.SaveChangesAsync();
        
        return deletedCount;
    }

    public async Task<bool> TogglePinAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return false;

        note.IsPinned = !note.IsPinned;
        note.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return note.IsPinned;
    }

    public async Task<bool> ToggleFavoriteAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note == null) return false;

        note.IsFavorite = !note.IsFavorite;
        note.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return note.IsFavorite;
    }

    public async Task UpdateLastViewedAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note != null)
        {
            note.LastViewedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<string>> GetAllTagsAsync()
    {
        var allTags = await _context.Notes
            .Where(n => !string.IsNullOrEmpty(n.Tags))
            .Select(n => n.Tags)
            .ToListAsync();

        var tags = new HashSet<string>();
        foreach (var tagString in allTags)
        {
            var tagList = tagString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t));
            
            foreach (var tag in tagList)
            {
                tags.Add(tag);
            }
        }

        return tags.OrderBy(t => t).ToList();
    }

    public async Task<List<string>> GetAllCategoriesAsync()
    {
        return await _context.Notes
            .Where(n => !string.IsNullOrEmpty(n.Category))
            .Select(n => n.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<Dictionary<string, object>> GetStatisticsAsync()
    {
        var total = await _context.Notes.CountAsync();
        var pinned = await _context.Notes.CountAsync(n => n.IsPinned);
        var favorites = await _context.Notes.CountAsync(n => n.IsFavorite);
        var drafts = await _context.Notes.CountAsync(n => n.Status == "草稿");
        
        var categoryStats = await _context.Notes
            .GroupBy(n => n.Category)
            .Select(g => new { Category = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToDictionaryAsync(x => x.Category, x => x.Count);

        var priorityStats = await _context.Notes
            .GroupBy(n => n.Priority)
            .Select(g => new { Priority = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Priority, x => x.Count);

        return new Dictionary<string, object>
        {
            ["total"] = total,
            ["pinned"] = pinned,
            ["favorites"] = favorites,
            ["drafts"] = drafts,
            ["categories"] = categoryStats,
            ["priorities"] = priorityStats
        };
    }

    public async Task<List<NoteListDto>> SearchNotesAsync(string keyword, int limit = 10)
    {
        return await _context.Notes
            .Where(n => n.Title.Contains(keyword) || n.Content.Contains(keyword) || n.Tags.Contains(keyword))
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.UpdatedAt)
            .Take(limit)
            .Select(n => new NoteListDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                TagList = n.TagList,
                Category = n.Category,
                Priority = n.Priority,
                Status = n.Status,
                IsPinned = n.IsPinned,
                IsFavorite = n.IsFavorite,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt,
                CharacterCount = n.Content.Length
            })
            .ToListAsync();
    }
}