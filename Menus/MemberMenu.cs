
using LibraryManagement.Helper;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Menus;

public class MemberMenu
{
    private readonly MemberService _service;

    public MemberMenu(MemberService service)
    {
        _service = service;
    }

    public async Task RunAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n===== MEMBER MENU =====");
            Console.WriteLine("1. Add Member");
            Console.WriteLine("2. View All Members");
            Console.WriteLine("3. View Member By ID");
            Console.WriteLine("4. Update Member");
            Console.WriteLine("5. Delete Member");
            Console.WriteLine("0. Back");

            if (!int.TryParse(
                InputHelper.ReadRequiredInput("Enter choice: "),
                out int choice))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }

            try
            {
                switch (choice)
                {
                    case 1:
                        await AddAsync();
                        break;
                    case 2:
                        await ViewAllAsync();
                        break;
                    case 3:
                        await ViewByIdAsync();
                        break;
                    case 4:
                        await UpdateAsync();
                        break;
                    case 5:
                        await DeleteAsync();
                        break;
                    case 0:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (DbUpdateException)
            {
                Console.WriteLine(
                    "Database operation failed. Check database constraints.");
            }
        }
    }

    private async Task AddAsync()
    {
        string name = InputHelper.ReadRequiredInput("Full name: ");
        string email = InputHelper.ReadRequiredInput("Email: ");

        Console.Write("Phone (optional): ");
        string? phone = Console.ReadLine();

        var member = new Member
        {
            FullName = name,
            Email = email,
            Phone = phone,
            IsActive = true
        };

        int id = await _service.AddAsync(member);

        Console.WriteLine($"Member added successfully. ID: {id}");
    }

    private async Task ViewAllAsync()
    {
        var members = await _service.GetAllAsync();

        if (members.Count == 0)
        {
            Console.WriteLine("No members found.");
            return;
        }

        foreach (var member in members)
            Display(member);
    }

    private async Task ViewByIdAsync()
    {
        int id = InputHelper.ReadInt("Member ID: ", 1);
        var member = await _service.GetByIdAsync(id);

        if (member == null)
        {
            Console.WriteLine("Member not found.");
            return;
        }

        Display(member);
    }

    private async Task UpdateAsync()
    {
        int id = InputHelper.ReadInt("Member ID to update: ", 1);
        var existing = await _service.GetByIdAsync(id);

        if (existing == null)
        {
            Console.WriteLine("Member not found.");
            return;
        }

        Console.WriteLine("Press Enter to keep the existing value.");

        Console.Write($"Full name ({existing.FullName}): ");
        string? name = Console.ReadLine();

        Console.Write($"Email ({existing.Email}): ");
        string? email = Console.ReadLine();

        Console.Write($"Phone ({existing.Phone ?? "none"}): ");
        string? phone = Console.ReadLine();

        Console.Write($"Active ({(existing.IsActive ? "y" : "n")}) (y/n): ");
        string? activeInput = Console.ReadLine();

        bool isActive = existing.IsActive;

        if (!string.IsNullOrWhiteSpace(activeInput))
        {
            if (activeInput.Trim().Equals(
                "y", StringComparison.OrdinalIgnoreCase))
                isActive = true;
            else if (activeInput.Trim().Equals(
                "n", StringComparison.OrdinalIgnoreCase))
                isActive = false;
            else
            {
                Console.WriteLine("Enter y or n. Update cancelled.");
                return;
            }
        }

        var updated = new Member
        {
            MemberId = id,
            FullName = string.IsNullOrWhiteSpace(name)
                ? existing.FullName : name.Trim(),
            Email = string.IsNullOrWhiteSpace(email)
                ? existing.Email : email.Trim(),
            Phone = string.IsNullOrWhiteSpace(phone)
                ? existing.Phone : phone.Trim(),
            IsActive = isActive
        };

        bool result = await _service.UpdateAsync(id, updated);

        Console.WriteLine(result
            ? "Member updated successfully."
            : "Member not found.");
    }

    private async Task DeleteAsync()
    {
        int id = InputHelper.ReadInt("Member ID to delete: ", 1);
        var member = await _service.GetByIdAsync(id);

        if (member == null)
        {
            Console.WriteLine("Member not found.");
            return;
        }

        Display(member);

        string confirm = InputHelper.ReadRequiredInput(
            "Confirm deletion? (y/n): ");

        if (!confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Deletion cancelled.");
            return;
        }

        bool result = await _service.DeleteAsync(id);

        Console.WriteLine(result
            ? "Member deleted successfully."
            : "Member not found.");
    }

    private static void Display(Member member)
    {
        Console.WriteLine($"""
            
            ----------------------------
            Member ID: {member.MemberId}
            Full Name: {member.FullName}
            Email: {member.Email}
            Phone: {member.Phone ?? "N/A"}
            Active: {(member.IsActive ? "Yes" : "No")}
            ----------------------------
            """);
    }
}
