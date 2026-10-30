
using System.Net.Mail;
using LibraryManagement.Models;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services;

public class MemberService
{
    private readonly MemberRepository _repository;
    private readonly AuditLogService _auditLogService;

    // Constructor Injection
    public MemberService(
        MemberRepository repository,
        AuditLogService auditLogService)
    {
        _repository = repository;
        _auditLogService = auditLogService;
    }

    // Add Member
    public async Task<int> AddAsync(Member member)
    {
        ValidateMember(member);

        member.FullName = member.FullName.Trim();
        member.Email = member.Email.Trim();

        member.Phone = string.IsNullOrWhiteSpace(member.Phone)
            ? null
            : member.Phone.Trim();

        member.IsActive = true;

        int memberId = await _repository.AddAsync(member);

        await _auditLogService.LogAsync(
            "INSERT",
            "Member",
            memberId,
            "New member registered successfully");

        return memberId;
    }

    // Get All Members
    public Task<List<Member>> GetAllAsync()
    {
        return _repository.GetAllAsync();
    }

    // Get Member By ID
    public async Task<Member?> GetByIdAsync(int id)
    {
        ValidateId(id);

        return await _repository.GetByIdAsync(id);
    }

    // Update Member
    public async Task<bool> UpdateAsync(
        int id,
        Member updatedMember)
    {
        ValidateId(id);
        ValidateMember(updatedMember);

        updatedMember.FullName = updatedMember.FullName.Trim();
        updatedMember.Email = updatedMember.Email.Trim();

        updatedMember.Phone =
            string.IsNullOrWhiteSpace(updatedMember.Phone)
                ? null
                : updatedMember.Phone.Trim();

        bool result = await _repository.UpdateAsync(
            id, updatedMember);

        if (result)
        {
            await _auditLogService.LogAsync(
                "UPDATE",
                "Member",
                id,
                "Member details updated successfully");
        }

        return result;
    }

    // Delete Member
    public async Task<bool> DeleteAsync(int id)
    {
        ValidateId(id);

        bool result = await _repository.DeleteAsync(id);

        if (result)
        {
            await _auditLogService.LogAsync(
                "DELETE",
                "Member",
                id,
                "Member deleted successfully");
        }

        return result;
    }

    // Validate Member ID
    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new InvalidOperationException(
                "Member ID must be greater than zero.");
        }
    }

    // Validate Member Details
    private static void ValidateMember(Member member)
    {
        ArgumentNullException.ThrowIfNull(member);

        if (string.IsNullOrWhiteSpace(member.FullName))
        {
            throw new InvalidOperationException(
                "Member name is required.");
        }

        if (string.IsNullOrWhiteSpace(member.Email))
        {
            throw new InvalidOperationException(
                "Email is required.");
        }

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
