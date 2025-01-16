using App.core.Interface;
using App.core.Model.Product;
using App.core.Model;
using App.core.Model.Register;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.core.App.User.Command
{
    public class UpdateUserCommand : IRequest<object>
    {
        public UpdateUserDto userDto { get; set; }
    }
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, object>
    {
        private readonly IAppDbContext _appDbContext;
        private readonly IImageService _imageService;
        public UpdateUserCommandHandler(IAppDbContext appDbContext, IImageService imageService)
        {
            _appDbContext = appDbContext;
            _imageService = imageService;
        }

        public async Task<object> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userToUpdate = request.userDto;
            var updateUserId = await _appDbContext.Set<Domain.Entity.Register.User>().
                                     FirstOrDefaultAsync(u => u.UserId == request.userDto.UserId);

            if (updateUserId is null) return new
            {
                status = 404,
                message = "User not found in the database",
                data = updateUserId
            };

           // var imageUploadResult = await _imageService.Upload(userToUpdate.ProfileImage);
           // if (imageUploadResult is ResponseDto uploadResponse && uploadResponse.Status != 200)
           // {
           //     return uploadResponse;
           //}

          //  string uploadedImageUrl = (imageUploadResult as ResponseDto)?.Data?.ToString();


          //  updateUserId.ProfileImage = uploadedImageUrl;
            updateUserId.DOB = userToUpdate.DOB;
            updateUserId.Mobile = userToUpdate.Mobile;
            updateUserId.Address = userToUpdate.Address;
            updateUserId.FirstName = userToUpdate.FirstName;
            updateUserId.LastName = userToUpdate.LastName;
            updateUserId.Zipcode = userToUpdate.Zipcode;
            updateUserId.Email = userToUpdate.Email;

            await _appDbContext.SaveChangesAsync(cancellationToken);
            var response = new
            {
                status = 200,
                message = "User updated successfully",
                updateUser = updateUserId
            };
            return response;
        }

    }
}