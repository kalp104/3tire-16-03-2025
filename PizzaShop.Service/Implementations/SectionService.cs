using System;
using System.Collections.Specialized;
using PizzaShop.Repository.Interfaces;
using PizzaShop.Repository.Models;
using PizzaShop.Service.Interfaces;

namespace PizzaShop.Service.Implementations;

public class SectionService : ISectionService
{
    private readonly IGenericRepository<Section> _section;
    public SectionService(IGenericRepository<Section> section)
    {
        _section = section;
    }

    public async Task<List<Section>?> GetSectionsAsync()
    {
        List<Section>? s = await _section.GetAllAsync();
        s = s.Where(u => u.Isdeleted == false).ToList();
        return s;
    }

}
