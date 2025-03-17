using System;
using PizzaShop.Repository.Models;

namespace PizzaShop.Service.Interfaces;

public interface ISectionService
{
    Task<List<Section>?> GetSectionsAsync();
}
