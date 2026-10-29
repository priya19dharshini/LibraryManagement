
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class MemberRepository
{
    public async Task<int> AddAsync(Member member)
    {
        await using var db = new AppDbContext();

        db.Members.Add(member);
        await db.SaveChangesAsync();

        return member.MemberId;
    }

    public async Task<List<Member>> GetAllAsync()
    {
        await using var db = new AppDbContext();

        return await db.Members
            .AsNoTracking()
            .OrderBy(m => m.MemberId)
            .ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        await using var db = new AppDbContext();

        return await db.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MemberId == id);
    }

    public async Task<bool> UpdateAsync(
        int id, Member updatedMember)
    {
        await using var db = new AppDbContext();

        var member = await db.Members.FindAsync(id);

        if (member == null)
            return false;

        member.FullName = updatedMember.FullName;
        member.Email = updatedMember.Email;
        member.Phone = updatedMember.Phone;
        member.IsActive = updatedMember.IsActive;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await using var db = new AppDbContext();

        var member = await db.Members.FindAsync(id);

        if (member == null)
            return false;

        bool hasLoans = await db.Loans
            .AnyAsync(l => l.MemberId == id);

        if (hasLoans)
        {
            throw new InvalidOperationException(
                "Cannot delete a member with existing loan records.");
        }

        db.Members.Remove(member);
        await db.SaveChangesAsync();

        return true;
    }
}
