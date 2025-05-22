using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Acme.SimpleTaskApp.Authorization;
using Acme.SimpleTaskApp.Products.Dto;
using System;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AutoMapper.Internal.Mappers;
using Acme.SimpleTaskApp.Entities.Products;
using Abp;
using Abp.UI;
using System.Collections.Generic;

namespace Acme.SimpleTaskApp.Products
{
    [AbpAuthorize]
    public class ProductAppService : SimpleTaskAppAppServiceBase, IProductAppService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IRepository<Product> _productRepository;

        public ProductAppService(IRepository<Product> productRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        [AbpAuthorize(PermissionNames.Pages_Products_Create)]
        public async System.Threading.Tasks.Task Create(ProductListDto input)
        {
            // Xử lý upload ảnh nếu có
            string imagePath = null;
            if (input.ImageUrl != null && input.ImageUrl.Length > 0)
            {
                imagePath = await SaveImageAsync(input.ImageUrl);
            }
            // Map DTO sang entity 
            var product = new Product(
                input.Name.Trim(),
                input.Price,
                imagePath,
                input.Quantity,
                input.CategoryId
            );

            // Thêm sản phẩm vào database
            await _productRepository.InsertAsync(product);
        }

        [AbpAuthorize]
        public async Task<PagedResultDto<ProductDto>> GetProductPaged(PagedProductDto input)
        {
            var query = _productRepository.GetAllIncluding(p => p.Category).AsNoTracking();

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                var keyword = input.Keyword.ToLower().Trim();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.Price.ToString().Contains(keyword) ||
                    p.Category.NameCategory.ToLower().Contains(keyword)
                );
            }

            // Lọc theo khoảng giá
            if (input.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= input.MinPrice.Value);
            }

            if (input.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= input.MaxPrice.Value);
            }

            // Lọc theo danh mục
            if (input.CategoryId.HasValue && input.CategoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == input.CategoryId.Value);
            }

            // Tính tổng số lượng sản phẩm sau khi lọc
            var totalCount = await query.CountAsync();

            // Lọc theo sắp xếp nếu có
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                query = query.OrderBy(input.Sorting);
            }
            else
            {
                // Mặc định sắp xếp theo thời gian tạo giảm dần
                query = query.OrderByDescending(p => p.CreationTime);
            }

            // Lấy kết quả phân trang
            var items = await query
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            // Chuyển kết quả thành danh sách ProductDto
            var dtos = ObjectMapper.Map<List<ProductDto>>(items);

            // Thêm thông tin danh mục cho mỗi sản phẩm
            foreach (var dto in dtos)
            {
                var product = items.First(p => p.Id == dto.Id);
                dto.NameCategory = product.Category?.NameCategory ?? "";
            }

            // Trả về kết quả phân trang
            return new PagedResultDto<ProductDto>(totalCount, dtos);
        }

        [AbpAuthorize(PermissionNames.Pages_Products_Edit)]
        public async Task Update(UpdateProductDto input)
        {
            try
            {
                var product = await _productRepository.GetAsync(input.Id);
                if (product == null)
                {
                    throw new UserFriendlyException("Sản phẩm không tồn tại");
                }

                // Cập nhật thông tin cơ bản
                product.Name = input.Name.Trim();
                product.Price = input.Price;
                product.CategoryId = input.CategoryId;

                // Xử lý số lượng
                if (input.Quantity >= 0)
                {
                    product.Quantity = input.Quantity;
                }

                // Xử lý ảnh
                if (input.ImageUrl != null && input.ImageUrl.Length > 0)
                {
                    // Nếu có ảnh mới, lưu ảnh mới và xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }
                    product.ImageUrl = await SaveImageAsync(input.ImageUrl);
                }
                else if (!string.IsNullOrEmpty(input.ExistingImageUrl))
                {
                    // Giữ nguyên ảnh cũ nếu không có ảnh mới
                    product.ImageUrl = input.ExistingImageUrl;
                }

                // Cập nhật thời gian sửa đổi
                product.LastModificationTime = DateTime.Now;

                await _productRepository.UpdateAsync(product);
                await CurrentUnitOfWork.SaveChangesAsync(); // Đảm bảo thay đổi được lưu vào database
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("Đã xảy ra lỗi khi cập nhật sản phẩm", ex);
            }
        }
        [AbpAuthorize(PermissionNames.Pages_Products_Delete)]
        public async Task Delete(int id)
        {
            await _productRepository.DeleteAsync(id);
        }


        private async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }
            // Đường dẫn thư mục lưu ảnh: wwwroot/uploads/products
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/products");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            // Tạo tên file duy nhất
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);
            // Lưu file ảnh vào thư mục
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // Lưu đường dẫn tương đối vào database
            return $"/uploads/products/{fileName}"; // Trả về đường dẫn để lưu vào database
        }

        [AbpAuthorize]
        public async Task<ProductDto> GetProducts(int id)
        {
                var product = await _productRepository.GetAsync(id);
                return ObjectMapper.Map<ProductDto>(product);
        }
    }
}
