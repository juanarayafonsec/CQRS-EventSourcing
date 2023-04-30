using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repository;

public class PostRepository : IPostRepository
{
    private readonly DatabaseContextFactory _contextFactory;

    public PostRepository(DatabaseContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task CreateAsync(PostEntity post)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Post.Add(post);

        _ = await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PostEntity post)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        context.Post.Update(post);

        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);

        if (post is null) return;

        context.Post.Remove(post);

        _ = await context.SaveChangesAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Post.Include(p => p.Comments).FirstOrDefaultAsync(x => x.PostId == postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Post.AsNoTracking().Include(p => p.Comments).AsNoTracking().ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Post.AsNoTracking().Include(p => p.Comments).AsNoTracking()
            .Where(x => x.Author.Contains(author))
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Post.AsNoTracking().Include(p => p.Comments).AsNoTracking()
            .Where(x => x.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentAsync()
    {
        await using DatabaseContext context = _contextFactory.CreateDbContext();
        return await context.Post.AsNoTracking().Include(p => p.Comments).AsNoTracking()
            .Where(x => x.Comments != null && x.Comments.Any())
            .ToListAsync();
    }
}