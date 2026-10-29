
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class MemberRepository
{

    private readonly AppDbContext _db;

    public MemberRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<int> AddAsync(Member member)
    {
       
        _db.Members.Add(member);
        await _db.SaveChangesAsync();

        return member.MemberId;
    }

    public async Task<List<Member>> GetAllAsync()
    {
       
        return await _db.Members
            .AsNoTracking()
            .OrderBy(m => m.MemberId)
            .ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
       
        return await _db.Members
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MemberId == id);
    }

    public async Task<bool> UpdateAsync(
        int id, Member updatedMember)
    {

        var member = await _db.Members.FindAsync(id);

        if (member == null)
            return false;

        member.FullName = updatedMember.FullName;
        member.Email = updatedMember.Email;
        member.Phone = updatedMember.Phone;
        member.IsActive = updatedMember.IsActive;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
       
        var member = await _db.Members.FindAsync(id);

        if (member == null)
            return false;

        bool hasLoans = await _db.Loans
            .AnyAsync(l => l.MemberId == id);

        if (hasLoans)
        {
            throw new InvalidOperationException(
                "Cannot delete a member with existing loan records.");
        }

        
        _db.Members.Remove(member);
        await _db.SaveChangesAsync();

        return true;
    }
}
