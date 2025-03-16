using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PizzaShop.Repository.ModelView;

public class ItemsViewModel
{
    public int Itemid { get; set; }
    [Required(ErrorMessage = "Item name is required")]
    [StringLength(100, ErrorMessage = "Item name cannot exceed 100 characters")]
    public string? Itemname { get; set; }

    [Required(ErrorMessage = "Category is required")]
    public int Categoryid { get; set; }

    [Required(ErrorMessage = "Item type is required")]
    [Range(1, 2, ErrorMessage = "Please select a valid item type (Veg or Non-veg)")]
    public int? Itemtype { get; set; }

    [Required(ErrorMessage = "Rate is required")]
    public decimal? Rate { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal? Quantity { get; set; }

    [Required(ErrorMessage = "Unit is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid unit")]
    public int? Units { get; set; }

    public bool Isavailabe { get; set; } 

    public bool Defaulttax { get; set; } 
    
    [Required(ErrorMessage = "Tax % required")]
    [Range(0, 100, ErrorMessage = "Tax percentage must be between 0 and 100")]
    public decimal? Taxpercentage { get; set; }

    [Required(ErrorMessage = "code is required")]
    public int? Shortcode { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    // File upload (not part of the Item model, handled separately in the controller)
    public IFormFile? UploadFiles { get; set; }
}
