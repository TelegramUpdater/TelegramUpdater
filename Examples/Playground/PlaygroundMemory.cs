using Microsoft.EntityFrameworkCore;
using Playground.Models;

namespace Playground;

internal class PlaygroundMemory(DbContextOptions options) : DbContext(options)
{
    public virtual DbSet<SeenUser> SeenUsers { get; set; }
}
