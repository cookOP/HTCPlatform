using FluentValidation;
using HTCPlatform.ViewModel.Models.Product;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.ViewModel.Validators.Product
{
    public  class UpdateProductViewModelValidator : AbstractValidator<UpdateProductViewModel>
    {
        public UpdateProductViewModelValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(1, 20).WithMessage("产品名称1至20个字符哦！");
            RuleFor(x => x.Amount).NotEqual(0).WithMessage("数量不能小于0哦！");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("类别名称不能为空哦！");
            RuleFor(x => x.Describe).NotEmpty().WithMessage("产品描述不能为空哦！");
            RuleFor(x => x.Logo).NotEmpty().WithMessage("产品图片不能为空哦！");
            RuleFor(x => x.Price).NotEqual(0).WithMessage("产品图片不能为空哦！");
            //RuleFor(x => x.Quantity).NotEqual(0).WithMessage("产品图片不能为空哦！");
        }
    }
}
