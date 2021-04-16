using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Data;
using System.Linq;


namespace Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly HotelDbContext _hotelDbContext;


        public RegisterModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager, HotelDbContext hotelDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _hotelDbContext = hotelDbContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(10, ErrorMessage = "Username must be at least 3 letters"), MinLength(3)]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [Display(Name = "Firstname")]
            public string Firstname { get; set; }

            [Required]
            [Display(Name = "Middlename")]
            public string Middlename { get; set; }

            [Required]
            [Display(Name = "Lastname")]
            public string Lastname { get; set; }

            [Required]
            [MaxLength(10, ErrorMessage = "EGN must be 10 digits"), MinLength(10)]
            public string EGN { get; set; }


            [Required]
            [MaxLength(12, ErrorMessage = "Phonenumber must be 10 digits or more"), MinLength(10)]
            public string PhoneNumber { get; set; }
           
            [Required]
            public DateTime DateOfEmployment { get; set; }

            public DateTime DateOfDischargement { get; set; }

            public bool Isactive { get; set; }

            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("/");
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),

                    UserName = Input.Username,
                    Email = Input.Email,
                    Firstname = Input.Firstname,
                    Middlename = Input.Middlename,
                    Lastname = Input.Lastname,
                    EGN = Input.EGN,
                    PhoneNumber = Input.PhoneNumber,
                    DateOfEmployment = Input.DateOfEmployment,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                 if (result.Succeeded)
                  {
                      if (this._hotelDbContext.Users.Count() == 1)
                      {
                          await _userManager.AddToRoleAsync(user, "Admin");
                          //await _userManager.UpdateAsync(user);
                      }

                      else
                      {
                          await _userManager.AddToRoleAsync(user, "Employee");
                      }
                      await _signInManager.SignInAsync(user, isPersistent: false);

                      return LocalRedirect(returnUrl);

                  }
              }

                // If we got this far, something failed, redisplay form
                return Page();
            }
        }
    }

