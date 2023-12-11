
using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using API.Errors;
using API.Helpers;

namespace API.Controllers;

public class ProductsController : BaseApiController
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IGenericRepository<ProductBrand> _productBrandRepository;
    private readonly IGenericRepository<ProductType> _productTypeRepository;
    private readonly IMapper _mapper;

    public ProductsController(IGenericRepository<Product> productRepository,  
                            IGenericRepository<ProductBrand> productBrandRepository, 
                            IGenericRepository<ProductType> productTypeRepository,
                            IMapper mapper)
    {
        _productRepository = productRepository;
        _productBrandRepository = productBrandRepository;
        _productTypeRepository = productTypeRepository;
        _mapper = mapper;
    }

    // [HttpGet]
    // public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(){
    //     // var products = await _productRepository.ListAllAsync();
    //     var spec = new ProductsWithTypesAndBrandsSpecification();
    //     var products = await _productRepository.ListAllWithSpecAsync(spec);

    //     return Ok(_mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products));
    // }

    [HttpGet]
    public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts(
            [FromQuery]ProductSpecParams productSpecParams){
                
        var countSpec = new ProductWithFiltersForCountSpecification(productSpecParams);
        var totalItems = await _productRepository.CountAsync(countSpec);

        var spec = new ProductsWithTypesAndBrandsSpecification(productSpecParams);
        var products = await _productRepository.ListAllWithSpecAsync(spec);

        var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);
        return Ok(new Pagination<ProductToReturnDto>(
            productSpecParams.PageIndex,
            productSpecParams.PageSize, 
            totalItems,
            data));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id){
        //var product = await _productRepository.GetByIdAsync(id);
        var spec = new ProductsWithTypesAndBrandsSpecification(id);
        var product = await _productRepository.GetEntityWithSpec(spec);
        if(product is null) return NotFound(new ApiResponse(404));
        return _mapper.Map<Product, ProductToReturnDto>(product);
    }

    [HttpGet("Brands")]
    public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands(){
        var brands = await _productBrandRepository.ListAllAsync();
        return Ok(brands);
    }
    
    [HttpGet("Types")]
    public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes(){
        var types = await _productTypeRepository.ListAllAsync();
        return Ok(types);
    }
}
