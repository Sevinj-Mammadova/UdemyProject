using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using UdemyProject.Data;
using UdemyProject.Models.Domain;

namespace UdemyProject.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext dbContext;

        public SQLWalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Walk> CreateAsync(Walk walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> DeleteAsync(Guid id)
        {
            var walkExists = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);
            if (walkExists == null)
            {
                return null;
            }
            dbContext.Walks.Remove(walkExists);
            await dbContext.SaveChangesAsync();
            return walkExists;

        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true)
        {
            //return await dbContext.Walks.Include("Difficulty").Include("Region").ToListAsync();
            var walks = dbContext.Walks.Include("Difficulty").Include("Region").AsQueryable();
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }
                if (string.IsNullOrWhiteSpace(sortBy) == false)
                {
                    if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    {
                        walks = isAscending
                            ? walks.OrderBy(x => x.Name)
                            : walks.OrderByDescending(x => x.Name);
                    }
                    else if (sortBy.Equals("WalkInKm", StringComparison.OrdinalIgnoreCase))
                    {
                        walks = isAscending
                            ? walks.OrderBy(x => x.WalkInKm)
                            : walks.OrderByDescending(x => x.WalkInKm);
                    }

                }
            }

            return await walks.ToListAsync();

        }

        public async Task<Walk?> GetByIdAsync(Guid id)
        {
            var walkExists = await dbContext.Walks
                .Include("Difficulty")
                .Include("Region")
                .FirstOrDefaultAsync(x => x.Id == id);
            return walkExists;
        }

        public async Task<Walk?> UpdateAsync(Guid id, Walk walk)
        {
            var walkExists = await dbContext.Walks.Include("Difficulty").Include("Region").FirstOrDefaultAsync(x => x.Id == id);
            if (walkExists == null)
            {
                return null;
            }

            walkExists.Name = walk.Name;
            walkExists.Description = walk.Description;
            walkExists.WalkImageUrl = walk.WalkImageUrl;
            walkExists.WalkInKm = walk.WalkInKm;
            walkExists.DifficultyId = walk.DifficultyId;
            walkExists.RegionId = walk.RegionId;
            await dbContext.SaveChangesAsync();
            walkExists.Difficulty = await dbContext.Difficulties
        .FirstOrDefaultAsync(d => d.Id == walkExists.DifficultyId);
            walkExists.Region = await dbContext.Regions
                .FirstOrDefaultAsync(r => r.Id == walkExists.RegionId);
            return walkExists;
        }
    }
}
