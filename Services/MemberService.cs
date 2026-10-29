
using System.Net.Mail;
using LibraryManagement.Models;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services;

public class MemberService
{
    private readonly MemberRepository _repository;

    public MemberService(MemberRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> AddAsync(Member member)
    {
        ValidateMember(member);

        member.FullName = member.FullName.Trim();
        member.Email = member.Email.Trim();
        member.Phone = string.IsNullOrWhiteSpace(member.Phone)
            ? null
            : member.Phone.Trim();

        member.IsActive = true;

        return await _repository.AddAsync(member);
    }

    public Task<List<Member>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    public async Task<Member?> GetByIdAsync(int id)
    {
        ValidateId(id);
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateAsync(
        int id, Member updatedMember)
    {
        ValidateId(id);
        ValidateMember(updatedMember);

        updatedMember.FullName = updatedMember.FullName.Trim();
        updatedMember.Email = updatedMember.Email.Trim();
        updatedMember.Phone =
            string.IsNullOrWhiteSpace(updatedMember.Phone)
                ? null
                : updatedMember.Phone.Trim();

        return await _repository.UpdateAsync(id, updatedMember);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        ValidateId(id);
        return await _repository.DeleteAsync(id);
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
            throw new InvalidOperationException(
                "Member ID must be greater than zero.");
    }

    private static void ValidateMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);

        if (string.IsNullOrWhiteSpace(member.FullName))
            throw new InvalidOperationException(
                "Member name is required.");

        if (string.IsNullOrWhiteSpace(member.Email))
            throw new InvalidOperationException(
                "Email is required.");

        try
        {
            var address = new MailAddress(member.Email.Trim());

            if (!string.Equals(
                address.Address,
                member.Email.Trim(),
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Enter a valid email address.");
            }
        }
        catch (FormatException)
        {
            throw new InvalidOperationException(
                "Enter a valid email address.");
        }
    }
}
