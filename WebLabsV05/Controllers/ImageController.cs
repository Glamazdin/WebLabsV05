using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebLabsV05.DAL.Entities;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;

namespace WebLabsV05.Controllers
{
    public class ImageController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        IHostingEnvironment _env;

        public ImageController(UserManager<ApplicationUser> mngr, IHostingEnvironment env)
        {           
            _userManager = mngr;
            _env = env;
        }

        public async Task<IActionResult> GetAvatar()
        {
            var user = await _userManager.GetUserAsync(User);
            if(user.AvatarImage!=null)
                return File(user.AvatarImage,user.ImageMimeType);
            else
            {                
                var avatarPath = Path.Combine("Images", "anonymous.png");
                
                var extProvider = new FileExtensionContentTypeProvider();
                var mimeType = extProvider.Mappings[".png"];
                return File(_env.WebRootFileProvider.GetFileInfo(avatarPath).CreateReadStream(), 
                            mimeType);
            }
        }
    }
}