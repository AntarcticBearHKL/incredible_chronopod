using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.Services;
using backend.DTOs;

namespace backend.Tests;

public class NoteServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly NoteService _noteService;

    public NoteServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _noteService = new NoteService(_context);
        
        // 初始化测试数据
        SeedTestData();
    }

    private void SeedTestData()
    {
        var notes = new List<Note>
        {
            new Note
            {
                Id = 1,
                Title = "测试记事1",
                Content = "这是第一条测试记事的内容",
                Tags = "测试,工作",
                Category = "工作",
                Priority = "高",
                Status = "已发布",
                IsPinned = true,
                IsFavorite = false,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new Note
            {
                Id = 2,
                Title = "测试记事2",
                Content = "这是第二条测试记事的内容，内容比较长一些，用来测试搜索功能",
                Tags = "测试,生活",
                Category = "生活",
                Priority = "中",
                Status = "已发布",
                IsPinned = false,
                IsFavorite = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            }
        };

        _context.Notes.AddRange(notes);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetNotesAsync_ShouldReturnPagedResults()
    {
        // Arrange
        var searchDto = new NoteSearchDto
        {
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _noteService.GetNotesAsync(searchDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Data.Count);
    }

    [Fact]
    public async Task GetNoteByIdAsync_WithValidId_ShouldReturnNote()
    {
        // Act
        var result = await _noteService.GetNoteByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("测试记事1", result.Title);
        Assert.Equal("这是第一条测试记事的内容", result.Content);
    }

    [Fact]
    public async Task GetNoteByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _noteService.GetNoteByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateNoteAsync_ShouldCreateNewNote()
    {
        // Arrange
        var createDto = new CreateNoteDto
        {
            Title = "新建测试记事",
            Content = "这是新建的测试记事内容",
            Tags = "新建,测试",
            Category = "测试",
            Priority = "中",
            Status = "已发布"
        };

        // Act
        var result = await _noteService.CreateNoteAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(createDto.Title, result.Title);
        Assert.Equal(createDto.Content, result.Content);
        Assert.True(result.Id > 0);

        // 验证数据库中确实创建了记事
        var noteInDb = await _context.Notes.FindAsync(result.Id);
        Assert.NotNull(noteInDb);
    }

    [Fact]
    public async Task UpdateNoteAsync_WithValidId_ShouldUpdateNote()
    {
        // Arrange
        var updateDto = new UpdateNoteDto
        {
            Title = "更新后的标题",
            Content = "更新后的内容",
            Tags = "更新,测试",
            Category = "更新分类",
            Priority = "低",
            Status = "草稿"
        };

        // Act
        var result = await _noteService.UpdateNoteAsync(1, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updateDto.Title, result.Title);
        Assert.Equal(updateDto.Content, result.Content);
        Assert.Equal(updateDto.Category, result.Category);
    }

    [Fact]
    public async Task UpdateNoteAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var updateDto = new UpdateNoteDto
        {
            Title = "更新后的标题",
            Content = "更新后的内容"
        };

        // Act
        var result = await _noteService.UpdateNoteAsync(999, updateDto);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteNoteAsync_WithValidId_ShouldReturnTrue()
    {
        // Act
        var result = await _noteService.DeleteNoteAsync(1);

        // Assert
        Assert.True(result);

        // 验证记事确实被删除了
        var noteInDb = await _context.Notes.FindAsync(1);
        Assert.Null(noteInDb);
    }

    [Fact]
    public async Task DeleteNoteAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Act
        var result = await _noteService.DeleteNoteAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task TogglePinAsync_ShouldTogglePinStatus()
    {
        // 初始状态是置顶的
        var initialNote = await _context.Notes.FindAsync(1);
        Assert.True(initialNote!.IsPinned);

        // Act - 取消置顶
        var result1 = await _noteService.TogglePinAsync(1);
        Assert.False(result1);

        // 验证状态已改变
        var updatedNote1 = await _context.Notes.FindAsync(1);
        Assert.False(updatedNote1!.IsPinned);

        // Act - 重新置顶
        var result2 = await _noteService.TogglePinAsync(1);
        Assert.True(result2);

        // 验证状态已改变
        var updatedNote2 = await _context.Notes.FindAsync(1);
        Assert.True(updatedNote2!.IsPinned);
    }

    [Fact]
    public async Task ToggleFavoriteAsync_ShouldToggleFavoriteStatus()
    {
        // 初始状态是未收藏的
        var initialNote = await _context.Notes.FindAsync(1);
        Assert.False(initialNote!.IsFavorite);

        // Act - 收藏
        var result1 = await _noteService.ToggleFavoriteAsync(1);
        Assert.True(result1);

        // 验证状态已改变
        var updatedNote1 = await _context.Notes.FindAsync(1);
        Assert.True(updatedNote1!.IsFavorite);

        // Act - 取消收藏
        var result2 = await _noteService.ToggleFavoriteAsync(1);
        Assert.False(result2);

        // 验证状态已改变
        var updatedNote2 = await _context.Notes.FindAsync(1);
        Assert.False(updatedNote2!.IsFavorite);
    }

    [Fact]
    public async Task SearchNotesAsync_ShouldReturnMatchingNotes()
    {
        // Act
        var result = await _noteService.SearchNotesAsync("第一条", 10);

        // Assert
        Assert.Single(result);
        Assert.Equal("测试记事1", result[0].Title);
    }

    [Fact]
    public async Task GetAllTagsAsync_ShouldReturnDistinctTags()
    {
        // Act
        var result = await _noteService.GetAllTagsAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains("测试", result);
        Assert.Contains("工作", result);
        Assert.Contains("生活", result);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnDistinctCategories()
    {
        // Act
        var result = await _noteService.GetAllCategoriesAsync();

        // Assert
        Assert.NotEmpty(result);
        Assert.Contains("工作", result);
        Assert.Contains("生活", result);
    }

    [Fact]
    public async Task GetStatisticsAsync_ShouldReturnCorrectStats()
    {
        // Act
        var result = await _noteService.GetStatisticsAsync();

        // Assert
        Assert.Equal(2, result["total"]);
        Assert.Equal(1, result["pinned"]);
        Assert.Equal(1, result["favorites"]);
        Assert.Equal(0, result["drafts"]);
    }

    [Fact]
    public async Task GetNotesAsync_WithKeywordFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var searchDto = new NoteSearchDto
        {
            Keyword = "第一条",
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _noteService.GetNotesAsync(searchDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Data);
        Assert.Equal("测试记事1", result.Data[0].Title);
    }

    [Fact]
    public async Task GetNotesAsync_WithCategoryFilter_ShouldReturnFilteredResults()
    {
        // Arrange
        var searchDto = new NoteSearchDto
        {
            Category = "工作",
            Page = 1,
            PageSize = 10
        };

        // Act
        var result = await _noteService.GetNotesAsync(searchDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Data);
        Assert.Equal("工作", result.Data[0].Category);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}