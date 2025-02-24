using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SportDomain.Identity;
using System.Threading.Tasks;

[Authorize]
public class UserAgreementModel : PageModel
{
    private readonly UserManager<BetUser> _userManager;
    private readonly SignInManager<BetUser> _signInManager;

    public UserAgreementModel(UserManager<BetUser> userManager, SignInManager<BetUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        user.HasAcceptedUserAgreement = true;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }

        // Refresh authentication claims
        await _signInManager.RefreshSignInAsync(user);

        return RedirectToPage("/Index");
    }

}
